using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.InferenceEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CookingAgent : Agent
{
    [SerializeField] private BehaviorParameters AgentBehavior;

    [SerializeField] private Transform[] Placements;
    [SerializeField] private Sprite[] Foods;


    [SerializeField] private PlateScript Plate;
    public NpcFoodWantScript Npc;

    [SerializeField] private Animator animator;

    [Header("Placement")]
    TextMeshProUGUI ScoreBoard;

    [SerializeField] private Renderer floor;

    [Header("Extras")]
    [SerializeField] private SpriteRenderer FoodHolder;
    public Sprite CurrentlyHolding = null;

    [SerializeField] private Color winColour = Color.green;
    [SerializeField] private Color loseColour = Color.red;
    [SerializeField] private Color idleColour;

    [Header("BotSettings")]
    [SerializeField] private bool InteractActivated = false;
    [SerializeField] private bool ShiftActivated = false;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float RunSpeed = 6f;


    public float ScoreNum = 0;
    public GameObject CurrentMenu;
    public SelectScript CurrentSelectScript;
    public bool InMenu = false;
    bool MenuWait = false;
    int StunNumber = 1;

    public int MaxNum = 0;
    public int ChooseNumber = 0;

    [SerializeField] List<int> MustGrab = new List<int>();
    int MustGrabNumber = 0;

    public Vector3 CurrentTask;
    private void Start()
    {
        Plate = GameObject.FindAnyObjectByType<PlateScript>();
        ScoreBoard = GameObject.Find("ScoreBoard").GetComponent<TextMeshProUGUI>();
        idleColour = floor.material.color;
        ShiftActivated = false;
    }
    private void LateUpdate()
    {
        if (Npc == null) 
        {
            Npc = GameObject.FindAnyObjectByType<NpcFoodWantScript>();
            MustGrab = new List<int>();
            MustGrabNumber = 0;
            for (int j = 0; j < Npc.NpcWants.Count; j++)
            {
                for (int i = 0; i < Foods.Length; i++)
                {
                    if (Npc.NpcWants[j] == Foods[i])
                    { MustGrab.Add(i); break; }
                }
            }
        }
        if (CurrentlyHolding != null)
        {
            if (FoodHolder.sprite != CurrentlyHolding) { FoodHolder.sprite = CurrentlyHolding; }
        }

        if (FoodHolder.sprite != null && CurrentlyHolding == null) { FoodHolder.sprite = null; }

        ScoreBoard.text = "Score:" + ScoreNum;
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-0.21f, 0.68f, 6.157037f);

        if (GameObject.Find("FoodOrderStorer") != null) { Destroy(GameObject.Find("FoodOrderStorer")); }
        if (GameObject.Find("NPC") != null ) {Destroy(GameObject.Find("NPC")); }
        CurrentlyHolding = null;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);

        sensor.AddObservation((float)StepCount / MaxStep);
        
        
        sensor.AddObservation(MustGrab[MustGrabNumber]);

        for (int i = 0; i < Foods.Length; i++) 
        {
            if (CurrentlyHolding == null) { sensor.AddObservation(-1); break; }
            if (Foods[i] == CurrentlyHolding)
            { string DebugText = "Is Holding" + i + Foods[i]; Debug.Log(DebugText); sensor.AddObservation(i); break; }
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0] * -1;
        float moveZ = actions.ContinuousActions[1] * -1;

        if (!InMenu)
        {
            if (moveX != 0 || moveZ != 0) { animator.SetBool("Moving", true); } else if (moveX == 0 && moveZ == 0) { animator.SetBool("Moving", false); }

            if (actions.ContinuousActions[2] == 1) { ShiftActivated = true;}
            else if (actions.ContinuousActions[2] != 1) { ShiftActivated = false; }
            animator.SetBool("Running", ShiftActivated);
            if (actions.ContinuousActions[3] == 1) { InteractActivated = true; }
            else if (actions.ContinuousActions[3] != 1) { InteractActivated = false; }
            animator.SetBool("Interacting", InteractActivated);

            float OriginalDistance;
            if (CurrentlyHolding == Foods[MustGrab[MustGrabNumber]])
            {
                if (MustGrab[MustGrabNumber] == 2)
                { OriginalDistance = Vector3.Distance(Placements[6].position, transform.position); }
                else { OriginalDistance = Vector3.Distance(Placements[7].position, transform.position); }
            }
            else
            { OriginalDistance = Vector3.Distance(Placements[MustGrab[MustGrabNumber]].position, transform.position); }

            Vector3 move = new Vector3(moveX, 0f, moveZ);

            if (!ShiftActivated) { transform.localPosition += move * moveSpeed * Time.deltaTime * StunNumber; }
            else { transform.localPosition += move * RunSpeed * Time.deltaTime * StunNumber; }
            AddReward(-0.001f * move.magnitude);

            float NewDistance;
            if (CurrentlyHolding == Foods[MustGrab[MustGrabNumber]])
            {
                if (MustGrab[MustGrabNumber] == 2)
                { NewDistance = Vector3.Distance(Placements[6].position, transform.position); }
                else { NewDistance = Vector3.Distance(Placements[7].position, transform.position); }
            }
            else
            { NewDistance = Vector3.Distance(Placements[MustGrab[MustGrabNumber]].position, transform.position); }
            if (NewDistance < OriginalDistance) { AddReward(0.002f); Debug.Log("Getting Closer"); }
            else {  AddReward(-0.005f); Debug.Log("Getting Further"); }
        }
        else if (InMenu)
        {
            InteractActivated = false;
            ShiftActivated = false;

            if (actions.ContinuousActions[1] > 0)
            {

                CurrentlyHolding = CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite;
                if (CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite == Foods[MustGrab[MustGrabNumber]]) { AddReward(0.1f); Debug.Log("Grabbed the right Item"); }
                else { AddReward(-0.2f); Debug.Log("Grabbed the Wrong Item"); }
                CurrentMenu.SetActive(false);
                InMenu = false;
                CurrentSelectScript = null;
                CurrentMenu = null;
            }

            if (actions.ContinuousActions[1] < 0)
            {
                if (CurrentSelectScript.CorrectChoice == true) { AddReward(-0.2f); Debug.Log("Left without the nessecary Item"); }
                else if (CurrentSelectScript.CorrectChoice == false) { AddReward(0.2f); Debug.Log("Left the wrong area"); }
                CurrentMenu.SetActive(false);
                InMenu = false;
                CurrentSelectScript = null;
                CurrentMenu = null;
            }
            if (MenuWait == false && actions.ContinuousActions[0] > 0)
            {

                MenuWait = true;
                if (ChooseNumber < MaxNum) { ChooseNumber += 1; }
                else { ChooseNumber = 0; }
                StartCoroutine(WaitMenuStun());
                if(CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite == Foods[MustGrab[MustGrabNumber]]) { AddReward(0.1f); Debug.Log("Moved to the right Item"); }
                else { AddReward(-0.2f); Debug.Log("Moved to the Wrong Item"); }
            }
            else if (MenuWait == false && actions.ContinuousActions[0] < 0)
            {
                MenuWait = true;
                if (ChooseNumber > 0) { ChooseNumber -= 1; }
                else { ChooseNumber = MaxNum; }
                StartCoroutine(WaitMenuStun());
                if(CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite == Foods[MustGrab[MustGrabNumber]]) { AddReward(0.1f); Debug.Log("Moved to the right Item"); }
                else { AddReward(-0.2f); Debug.Log("Moved to the Wrong Item"); }
            }
        }
        // Small penalty every step so it learns to hurry
        if (MaxStep > 0) { AddReward(-1f / MaxStep); }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        var kb = Keyboard.current;
        if (kb == null) return;

        continuous[0] = (kb.dKey.isPressed ? 1f : 0f)
                      - (kb.aKey.isPressed ? 1f : 0f);
        continuous[1] = (kb.wKey.isPressed ? 1f : 0f)
                      - (kb.sKey.isPressed ? 1f : 0f);

        continuous[2] = (kb.shiftKey.isPressed ? 1f : 0f);
        continuous[3] = (kb.eKey.isPressed ? 1f : 0f);
    }
    public void Stunned()
    { StunNumber = 0; }
    public void UnStun()
    { StunNumber = 1; }
    /*public override void Initialize()   // runs once at startup
    {
        floor.material.color = idleColour;
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            AddReward(-0.1f);
            transform.localPosition = new Vector3(-0.21f, 0.68f, 6.157037f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (InteractActivated == true)
        {
            if (other.gameObject.GetComponent<OpenMenu>() != null)
            { 
                other.gameObject.GetComponent<OpenMenu>().OpeningMenu();
                if (Placements[MustGrab[MustGrabNumber]].gameObject == other.gameObject)
                { AddReward(0.1f); Debug.Log("Got to RightPlace"); other.gameObject.GetComponent<OpenMenu>().Menu.GetComponentInChildren<SelectScript>().CorrectChoice = true; }
                else if (Placements[MustGrab[MustGrabNumber]].gameObject != other.gameObject) { other.gameObject.GetComponent<OpenMenu>().Menu.GetComponentInChildren<SelectScript>().CorrectChoice = false; }
            }

            else if (other.gameObject.GetComponent<PlateScript>() != null)
            {
                if (CurrentlyHolding != null) { other.gameObject.GetComponent<PlateScript>().PlateFood(); }
                else if (CurrentlyHolding == null) { other.gameObject.GetComponent<PlateScript>().UnPlateFood(); }
                if (Placements[MustGrab[MustGrabNumber]].gameObject == other.gameObject)
                { AddReward(0.1f); Debug.Log("Got to RightPlace"); }
            }

            else if (other.gameObject.GetComponent<CookingProcess>() != null)
            { 
                other.gameObject.GetComponent<CookingProcess>().StartCooking();
                if (Placements[MustGrab[MustGrabNumber]].gameObject == other.gameObject)
                { AddReward(0.1f); Debug.Log("Got to RightPlace"); }
            }

            else if (other.gameObject.GetComponent<CuttingProcess>() != null)
            { 
                other.gameObject.GetComponent<CuttingProcess>().Cutting();
                if (Placements[MustGrab[MustGrabNumber]].gameObject == other.gameObject)
                { AddReward(0.1f); Debug.Log("Got to RightPlace"); }
            }

            else if (other.gameObject.GetComponent<BellScript>() != null)
            { 
                other.gameObject.GetComponent<BellScript>().GiveFood();
                if (Placements[MustGrab[MustGrabNumber]].gameObject == other.gameObject)
                { AddReward(0.1f); Debug.Log("Got to RightPlace"); }
            }

            else if (other.gameObject.GetComponent<TrashScript>() != null)
            { 
                other.gameObject.GetComponent<TrashScript>().TrashFood();
                if (Placements[MustGrab[MustGrabNumber]].gameObject == other.gameObject)
                { AddReward(0.05f); Debug.Log("Got to RightPlace"); }
            }
        }
    }
    IEnumerator ShortTimerCheck()
    {
        yield return new WaitForFixedUpdate();
        floor.material.color = idleColour;
    }
    IEnumerator WaitMenuStun()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        yield return MenuWait = false;   
    }

}
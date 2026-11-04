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
    [SerializeField] private CookingProcess Stove;
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
    [SerializeField] int MustGrabNumber = 0;
    [SerializeField] bool Finished = false;

    public Vector3 CurrentTask;

    float OriginalDistance;

    float NewDistance;

    bool RewardRecieved = false;
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
            Finished = false;
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

        RewardRecieved = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);

        sensor.AddObservation((float)StepCount / MaxStep);

        sensor.AddObservation(NewDistance);

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

            if (actions.ContinuousActions[2] == 1) { ShiftActivated = true; }
            else if (actions.ContinuousActions[2] != 1) { ShiftActivated = false; }
            animator.SetBool("Running", ShiftActivated);
            if (actions.ContinuousActions[3] == 1) { InteractActivated = true; }
            else if (actions.ContinuousActions[3] != 1) { InteractActivated = false; }
            animator.SetBool("Interacting", InteractActivated);

            if (MustGrab.Count > 0)
            {
                if (!Finished)
                {
                    if (CurrentlyHolding == Foods[MustGrab[MustGrabNumber]])
                    {
                        OriginalDistance = Vector3.Distance(Placements[7].position, transform.position);
                    }
                    else if (MustGrab[MustGrabNumber] == 3 && CurrentlyHolding == Foods[2])
                    {
                        OriginalDistance = Vector3.Distance(Placements[6].position, transform.position);
                    }
                    else
                    { OriginalDistance = Vector3.Distance(Placements[MustGrab[MustGrabNumber]].position, transform.position); }

                    if (Stove.IsCooking)
                    {
                        OriginalDistance = Vector3.Distance(Placements[6].position, transform.position);
                    }
                }
                else { OriginalDistance = Vector3.Distance(Placements[8].position, transform.position); }
            }

            Vector3 move = new Vector3(moveX, 0f, moveZ);

            if (!ShiftActivated) { transform.localPosition += move * moveSpeed * Time.deltaTime * StunNumber; }
            else { transform.localPosition += move * RunSpeed * Time.deltaTime * StunNumber; }
            AddReward(-0.001f * move.magnitude);

            if (MustGrab.Count > 0)
            {
                if (!Finished)
            {
                if (CurrentlyHolding == Foods[MustGrab[MustGrabNumber]])
                {
                    NewDistance = Vector3.Distance(Placements[7].position, transform.position);
                }
                else if (MustGrab[MustGrabNumber] == 3 && CurrentlyHolding == Foods[2])
                {
                    NewDistance = Vector3.Distance(Placements[6].position, transform.position);
                }
                else
                { NewDistance = Vector3.Distance(Placements[MustGrab[MustGrabNumber]].position, transform.position); }
                if (Stove.IsCooking)
                {
                    NewDistance = Vector3.Distance(Placements[6].position, transform.position);
                }
            }
            else { NewDistance = Vector3.Distance(Placements[8].position, transform.position); }
                if (RewardRecieved == false && NewDistance < 1.2f) 
                { 
                    if (GetCumulativeReward() < 0)
                    { RewardRecieved = true; SetReward(1f); Debug.Log("Got to the DesignatedPlace Set Points to 1"); }
                    else { RewardRecieved = true; AddReward(1f); Debug.Log("Got to the DesignatedPlace Added 1 Point"); }
                }
               if (!Stove.IsCooking)
                {
                    if (NewDistance < 1.1) { return; }
                    else if (NewDistance < OriginalDistance) { AddReward(0.002f); Debug.Log("Getting Closer"); }
                    else { AddReward(-0.003f); Debug.Log("Getting Further"); }
                }
                else if (Stove.IsCooking && NewDistance < 1.2)
                {
                    AddReward(0.001f); Debug.Log("Staying at Stove");
                }
                else { AddReward(-0.004f); Debug.Log("Getting Further"); }
            }
        }
        else if (InMenu)
        {
            InteractActivated = false;
            ShiftActivated = false;

            if (CurrentlyHolding != Foods[MustGrab[MustGrabNumber]] || MustGrab[MustGrabNumber] == 3 && CurrentlyHolding != Foods[MustGrab[3]])
            {
                if (CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite == Foods[MustGrab[MustGrabNumber]]) { AddReward(0.1f); Debug.Log("Moved to the right Item"); }
                else if (MustGrab[MustGrabNumber] == 3 && CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite == Foods[2]) { AddReward(0.1f); Debug.Log("Moved to the right Item"); }
                else { AddReward(-0.2f); Debug.Log("Moved to the Wrong Item"); }
            }
            else { AddReward(-0.2f); Debug.Log("Needs to leave"); }

            if (actions.ContinuousActions[1] > 0)
            {

                CurrentlyHolding = CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite;
                if (!Finished)
                {
                    if (CurrentlyHolding != Foods[MustGrab[MustGrabNumber]] || MustGrab[MustGrabNumber] == 3 && CurrentlyHolding != Foods[MustGrab[3]])
                    {
                        if (CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite == Foods[MustGrab[MustGrabNumber]]) { AddReward(0.2f); Debug.Log("Grabbed the right Item"); }
                        else if (MustGrab[MustGrabNumber] == 3 && CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite == Foods[2]) { AddReward(0.2f); Debug.Log("Grabbed the right Item"); }
                        else { AddReward(-0.4f); Debug.Log("Grabbed the Wrong Item"); }
                    }
                    else { AddReward(-0.5f); Debug.Log("Already had Right Item"); }
                }
                else { AddReward(-0.2f); Debug.Log("Dont NEED ITEM"); }
                CurrentMenu.SetActive(false);
                InMenu = false;
                CurrentSelectScript = null;
                CurrentMenu = null;
            }

            if (actions.ContinuousActions[1] < 0)
            {
                if (!Finished)
                {
                    if (CurrentlyHolding != Foods[MustGrab[MustGrabNumber]] || MustGrab[MustGrabNumber] == 3 && CurrentlyHolding != Foods[MustGrab[2]])
                    {
                        if (CurrentSelectScript.CorrectChoice == true) { AddReward(-1f); Debug.Log("Left without the nessecary Item"); }
                        else if (CurrentSelectScript.CorrectChoice == false) { AddReward(0.05f); Debug.Log("Left the wrong area"); }
                    }
                    else { Debug.Log("Left Unnessecary Item Select"); }
                }
                else { Debug.Log("Left Unnessecary Item Select"); }
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
                if (!Finished)
                {
                    if (CurrentlyHolding != Foods[MustGrab[MustGrabNumber]] || MustGrab[MustGrabNumber] == 3 && CurrentlyHolding != Foods[MustGrab[3]])
                    {
                        if (CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite == Foods[MustGrab[MustGrabNumber]]) { AddReward(0.1f); Debug.Log("Moved to the right Item"); }
                        else if (MustGrab[MustGrabNumber] == 3 && CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite == Foods[2]) { AddReward(0.1f); Debug.Log("Moved to the right Item"); }
                        else { AddReward(-0.2f); Debug.Log("Moved to the Wrong Item"); }
                    }
                    else { AddReward(-0.2f); Debug.Log("Needs to leave"); }
                }
                else { AddReward(-0.2f); Debug.Log("Needs to leave"); }
            }
            else if (MenuWait == false && actions.ContinuousActions[0] < 0)
            {
                MenuWait = true;
                if (ChooseNumber > 0) { ChooseNumber -= 1; }
                else { ChooseNumber = MaxNum; }
                StartCoroutine(WaitMenuStun());
                if (!Finished)
                {
                    if (CurrentlyHolding != Foods[MustGrab[MustGrabNumber]] || MustGrab[MustGrabNumber] == 3 && CurrentlyHolding != Foods[MustGrab[3]])
                    {
                        if (CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite == Foods[MustGrab[MustGrabNumber]]) { AddReward(0.1f); Debug.Log("Moved to the right Item"); }
                        else if (MustGrab[MustGrabNumber] == 3 && CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite == Foods[2]) { AddReward(0.1f); Debug.Log("Moved to the right Item"); }
                        else { AddReward(-0.2f); Debug.Log("Moved to the Wrong Item"); }
                    }
                    else { AddReward(0.2f); Debug.Log("Needs to leave"); }
                }
                else { AddReward(-0.2f); Debug.Log("Needs to leave"); }
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
            AddReward(-0.5f);
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
                if (CurrentlyHolding != Foods[MustGrab[MustGrabNumber]] && Placements[MustGrab[MustGrabNumber]].gameObject == other.gameObject)
                { AddReward(1f); Debug.Log("Got to RightPlace"); other.gameObject.GetComponent<OpenMenu>().Menu.GetComponentInChildren<SelectScript>().CorrectChoice = true; RewardRecieved = false; }
                else if (Placements[MustGrab[MustGrabNumber]].gameObject != other.gameObject) { AddReward(-1f); Debug.Log("The WrongPlace"); other.gameObject.GetComponent<OpenMenu>().Menu.GetComponentInChildren<SelectScript>().CorrectChoice = false; }
            }

            else if (other.gameObject.GetComponent<PlateScript>() != null)
            {
                if (CurrentlyHolding != null)
                {
                    if (CurrentlyHolding == Foods[MustGrab[MustGrabNumber]])
                    {
                        AddReward(1f);
                        Debug.Log("Placed right Item");
                        RewardRecieved = false;

                        if (MustGrabNumber < MustGrab.Count - 1)
                        { MustGrabNumber += 1; }
                        else { Finished = true; }
                    }
                    other.gameObject.GetComponent<PlateScript>().PlateFood();
                    InteractActivated = false;
                    return;
                }
                else if (CurrentlyHolding == null) { other.gameObject.GetComponent<PlateScript>().UnPlateFood(); InteractActivated = false; return; }
            }

            else if (other.gameObject.GetComponent<CookingProcess>() != null)
            {
                other.gameObject.GetComponent<CookingProcess>().StartCooking();
                if (Placements[MustGrab[MustGrabNumber]].gameObject == other.gameObject)
                { AddReward(1f); Debug.Log("Got to RightPlace"); RewardRecieved = false; InteractActivated = false; return; }
                else if (Placements[MustGrab[MustGrabNumber]].gameObject != other.gameObject)
                { AddReward(-1f); Debug.Log("The WrongPlace"); }
                InteractActivated = false;
            }

            else if (other.gameObject.GetComponent<CuttingProcess>() != null)
            {
                other.gameObject.GetComponent<CuttingProcess>().Cutting();
                if (Placements[MustGrab[MustGrabNumber]].gameObject == other.gameObject)
                { AddReward(1f); Debug.Log("Got to RightPlace"); RewardRecieved = false; }
                InteractActivated = false;
            }

            else if (other.gameObject.GetComponent<BellScript>() != null)
            {
                other.gameObject.GetComponent<BellScript>().GiveFood();
                if (Finished)
                { AddReward(1f); Debug.Log("Rung Bell"); RewardRecieved = false; }
                else if (!Finished) { AddReward(-1f); Debug.Log("Rung Bell but not done"); }
                InteractActivated = false;
            }

            else if (other.gameObject.GetComponent<TrashScript>() != null)
            {
                other.gameObject.GetComponent<TrashScript>().TrashFood();
                if (CurrentlyHolding != null && CurrentlyHolding != Foods[MustGrab[MustGrabNumber]])
                { AddReward(0.5f); Debug.Log("Binned to the Right Food"); RewardRecieved = false; }
                else if (CurrentlyHolding == Foods[MustGrab[MustGrabNumber]] || MustGrab[MustGrabNumber] == 3 && CurrentlyHolding == Foods[2])
                { AddReward(-1f); Debug.Log("Binned the needed Food"); }
            }
        }
    }
    public IEnumerator ShortWinTimerCheck()
    {
        floor.material.color = winColour;
        yield return new WaitForSecondsRealtime(0.5f);
        floor.material.color = idleColour;
    }
    public IEnumerator ShortWrongTimerCheck()
    {
        floor.material.color = loseColour;
        yield return new WaitForSecondsRealtime(0.5f);
        floor.material.color = idleColour;
    }
    IEnumerator WaitMenuStun()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        yield return MenuWait = false;   
    }

}
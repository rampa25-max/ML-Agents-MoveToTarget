using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.InferenceEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CookingAgent : Agent
{
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
    private void Start()
    {
        Plate = GameObject.FindAnyObjectByType<PlateScript>();
        ScoreBoard = GameObject.Find("ScoreBoard").GetComponent<TextMeshProUGUI>();
        idleColour = floor.material.color;
        ShiftActivated = false;
    }
    private void FixedUpdate()
    {
        if (Npc == null) {Npc = GameObject.FindAnyObjectByType<NpcFoodWantScript>(); }
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

        IList<float> Demand = new List<float>();
        for (int j = 0; j < Foods.Length; j++)
        {
            for (int i = 0; i < Foods.Length; i++)
            {
                Demand.Add(i);
            }
        }
        sensor.AddObservation(Demand);

        for (int i = 0; i < Foods.Length; i++) 
        { 
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
            if (actions.ContinuousActions[2] == 1)
            {
                ShiftActivated = true;
            }
            else if (actions.ContinuousActions[2] != 1) { ShiftActivated = false; }
            animator.SetBool("Running", ShiftActivated);
            if (actions.ContinuousActions[3] == 1) { InteractActivated = true; }
            else if (actions.ContinuousActions[3] != 1) { InteractActivated = false; }
            animator.SetBool("Interacting", InteractActivated);
            Vector3 move = new Vector3(moveX, 0f, moveZ);
            if (!ShiftActivated) { transform.localPosition += move * moveSpeed * Time.deltaTime * StunNumber; }
            else { transform.localPosition += move * RunSpeed * Time.deltaTime * StunNumber; }
            AddReward(-0.001f * move.magnitude);
        }
        else if (InMenu)
        {
            InteractActivated = false;
            ShiftActivated = false;

            if (actions.ContinuousActions[1] > 0)
            {
                CurrentlyHolding = CurrentSelectScript.Foods[ChooseNumber].GetComponent<Image>().sprite;
                CurrentMenu.SetActive(false);
                InMenu = false;
                CurrentSelectScript = null;
                CurrentMenu = null;
            }

            if (actions.ContinuousActions[1] < 0)
            {
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
            }
            else if (MenuWait == false && actions.ContinuousActions[0] < 0)
            {
                MenuWait = true;
                if (ChooseNumber > 0) { ChooseNumber -= 1; }
                else { ChooseNumber = MaxNum; }
                StartCoroutine(WaitMenuStun());
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
            transform.localPosition = new Vector3(-0.21f, 0.68f, 6.157037f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (InteractActivated == true)
        {
            if (other.gameObject.GetComponent<OpenMenu>() != null)
            { other.gameObject.GetComponent<OpenMenu>().OpeningMenu(); }

            else if (other.gameObject.GetComponent<PlateScript>() != null)
            {
                if (CurrentlyHolding != null) { other.gameObject.GetComponent<PlateScript>().PlateFood(); }
                else if (CurrentlyHolding == null) { other.gameObject.GetComponent<PlateScript>().UnPlateFood(); }
            }

            else if (other.gameObject.GetComponent<CookingProcess>() != null)
            { other.gameObject.GetComponent<CookingProcess>().StartCooking(); }

            else if (other.gameObject.GetComponent<CuttingProcess>() != null)
            { other.gameObject.GetComponent<CuttingProcess>().Cutting(); }

            else if (other.gameObject.GetComponent<BellScript>() != null)
            { other.gameObject.GetComponent<BellScript>().GiveFood(); }

            else if (other.gameObject.GetComponent<TrashScript>() != null)
            { other.gameObject.GetComponent<TrashScript>().TrashFood(); }
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
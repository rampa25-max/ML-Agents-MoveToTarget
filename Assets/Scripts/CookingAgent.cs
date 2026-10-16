using System.Collections;
using System.Collections.Generic;
using Unity.InferenceEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CookingAgent : Agent
{
    [Header("Placement")]
    [SerializeField] private Transform CookingStation;
    [SerializeField] private Transform IngredientStation;
    [SerializeField] private Transform FrozenFoodStation;
    [SerializeField] private Transform CondimentStation;
    [SerializeField] private Transform CuttingStation;

    [SerializeField] private Transform Bell;
    [SerializeField] private Transform FoodTrayPlacement;
    [SerializeField] private Renderer floor;

    [Header("Extras")]
    [SerializeField] private SpriteRenderer FoodHolder;
    public Sprite CurrentlyHolding = null;

    [SerializeField] private Color winColour = Color.green;
    [SerializeField] private Color loseColour = Color.red;
    [SerializeField] private Color idleColour;

    [Header("BotSettings")]
    private bool ShiftActivated = false;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float RunSpeed = 6f;

    private void Start()
    {
        idleColour = floor.material.color;
        ShiftActivated = false;
    }
    private void FixedUpdate()
    {
        if (CurrentlyHolding != null)
        {
            if (FoodHolder.sprite != CurrentlyHolding) { FoodHolder.sprite = CurrentlyHolding; }
        }

        if (FoodHolder.sprite != null && CurrentlyHolding == null) { FoodHolder.sprite = null; }
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-0.21f, 0.68f, 6.157037f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition); // 3
        sensor.AddObservation(CookingStation.localPosition);    // 3
        sensor.AddObservation((CookingStation.localPosition - transform.localPosition).normalized);
        float d = Vector3.Distance(transform.localPosition, CookingStation.localPosition);
        sensor.AddObservation(d / 10f);
        sensor.AddObservation((float)StepCount / MaxStep);


    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        if (actions.ContinuousActions[2] == 1) { ShiftActivated = true; }
        else if (actions.ContinuousActions[2] != 1) { ShiftActivated = false; }

        Vector3 move = new Vector3(moveX, 0f, moveZ);
        if (!ShiftActivated) { transform.localPosition += move * moveSpeed * Time.deltaTime; }
        else { transform.localPosition += move * RunSpeed * Time.deltaTime; }
        AddReward(-0.001f * move.magnitude);
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
    }
    /*public override void Initialize()   // runs once at startup
    {
        floor.material.color = idleColour;
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stove"))
        {
            SetReward(1f);
        }
    }
    IEnumerator ShortTimerCheck()
    {
        yield return new WaitForFixedUpdate();
        floor.material.color = idleColour;
    }


}
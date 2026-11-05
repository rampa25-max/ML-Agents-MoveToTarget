using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class PlateScript : MonoBehaviour
{
    public GameObject SpriteHolder;
    public GameObject ParentHolder;
    public List<Sprite> PlatedFood = new List<Sprite>();

    public List<GameObject> SpriteGameObject = new List<GameObject>();
    CookingAgent Agent;

    [SerializeField] float movement = 0;

    bool WaitTime = false;
    void Awake()
    {
        Agent = GameObject.FindAnyObjectByType<CookingAgent>();
    }

    void Update()
    {
        if (ParentHolder == null)
        { ParentHolder = GameObject.Find("PlateStorer"); }
    }
    public void PlateFood()
    {
        Debug.Log("Trying Plating Food");
        if (WaitTime == false)
        {
            if (Agent.CurrentlyHolding != null)
            {
                if (ParentHolder != null)
                {
                    PlatedFood.Add(Agent.CurrentlyHolding);
                    var FoodSprite = Instantiate(SpriteHolder, ParentHolder.transform, false);
                    FoodSprite.transform.localPosition = new Vector3(0, 0, 0);
                    FoodSprite.name = Agent.CurrentlyHolding.name;
                    FoodSprite.GetComponent<Image>().sprite = Agent.CurrentlyHolding;
                    FoodSprite.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.2f + movement, 0);
                    SpriteGameObject.Add(FoodSprite);
                    movement += 0.06f;

                    Agent.CurrentlyHolding = null;
                    Debug.Log("Placed Food");
                    WaitTime = true;
                    StartCoroutine(Cooldown());
                }
            }
        }
    }
    public void UnPlateFood()
    {
        Debug.Log("Trying Take FoodItem");
        if (WaitTime == false)
        {
            if (Agent.CurrentlyHolding == null)
            {
                if (PlatedFood.Count > 0)
                {
                    Agent.CurrentlyHolding = PlatedFood[PlatedFood.Count - 1];

                    GameObject DeleteObject = SpriteGameObject[PlatedFood.Count - 1];
                    SpriteGameObject.RemoveAt(PlatedFood.Count - 1);
                    PlatedFood.RemoveAt(PlatedFood.Count - 1);
                    movement -= 0.06f;

                    Destroy(DeleteObject);

                    if (Agent.MustGrabNumber > 0 && Agent.CurrentlyHolding == Agent.Foods[Agent.MustGrab[Agent.MustGrabNumber - 1]]) { Agent.MustGrabNumber -= 1; }

                    Debug.Log("Removed Food");
                    WaitTime = true;
                    StartCoroutine(Cooldown());
                }
            }
        }
    }

    public void SetMovement(float move)
    { movement = move; }
    IEnumerator Cooldown()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        WaitTime = false;
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;

public class PlateScript : MonoBehaviour
{
    public GameObject SpriteHolder;
    public GameObject ParentHolder;
    public List<Sprite> PlatedFood = new List<Sprite>();
    CookingAgent Agent;

    [SerializeField] float movement = 0;
    void Awake()
    {
        PlatedFood = null;
        Agent = GameObject.FindAnyObjectByType<CookingAgent>();
    }

    void Update()
    {
        
    }
    public void PlateFood()
    {
        Debug.Log("Trying Plating Food");
        if (Agent.CurrentlyHolding != null)
        {
            //PlatedFood.Add(Agent.CurrentlyHolding);
            var FoodSprite = Instantiate(SpriteHolder, ParentHolder.transform, false);
            FoodSprite.transform.localPosition = new Vector3(0, 0, 0);
            FoodSprite.name = Agent.CurrentlyHolding.name;
            FoodSprite.GetComponent<Image>().sprite = Agent.CurrentlyHolding;
            FoodSprite.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.2f + movement, 0);
            movement += 0.06f;

            Agent.CurrentlyHolding = null;
            Debug.Log("Placed Food");
        }
    }
}

using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;

public class NpcFoodWantScript : MonoBehaviour
{
    CookingAgent Agent;

    NpcSpawnScript Spawner;
    public GameObject NpcParent;

    int MaxIngredientsAllowed = 4;
    public Sprite[] FoodIngredients;
    public List<Sprite> NpcWants = new List<Sprite>();
    public GameObject SpriteSheet;

    public GameObject FoodOrderCam;
    void Awake()
    {
        Spawner = GameObject.FindAnyObjectByType<NpcSpawnScript>();
        Agent = GameObject.FindAnyObjectByType<CookingAgent>();
        FoodOrderCam = GameObject.Find("FoodOrderStorer");
                    
        NpcWants.Add(FoodIngredients[0]);
        NpcWants.Add(FoodIngredients[2]);

        int Amount = Random.Range(1, MaxIngredientsAllowed);

        for (int i = 0; i < Amount; i++)
        {
            int Ingredient = Random.Range(2, FoodIngredients.Length);
            NpcWants.Add (FoodIngredients[Ingredient]);
        }

        NpcWants.Add(FoodIngredients[1]);
    }

    private void Start()
    {
        float movement = 0;
        foreach (var item in NpcWants)
        {
            var FoodSprite = Instantiate(SpriteSheet, this.gameObject.transform, false);
            FoodSprite.transform.localPosition = new Vector3(0,0,0);
            FoodSprite.name = item.name;
            FoodSprite.GetComponent<Image>().sprite = item;
            FoodSprite.GetComponent<RectTransform>().localPosition = new Vector3(0,-0.2f + movement,0);
            movement += 0.06f;
        }
        movement = 0;
        foreach (var item in NpcWants)
        {
            var FoodSprite = Instantiate(SpriteSheet, FoodOrderCam.gameObject.transform, false);
            FoodSprite.transform.localScale = new Vector3(400, 400, 400);
            FoodSprite.transform.localPosition = new Vector3(0, 0, 0);
            FoodSprite.name = item.name;
            FoodSprite.GetComponent<Image>().sprite = item;
            FoodSprite.GetComponent<RectTransform>().localPosition = new Vector3(0, -60f + movement, 0);
            movement += 25f;
        }

    }

    public void CheckFood(List<Sprite> PlatedFood,PlateScript Plate)
    {
        float Reward = 0;
        Debug.Log("Serving Food");

        for (int i = 0; i < NpcWants.Count; i ++)
        {
            if (i < PlatedFood.Count)
            {
                if (PlatedFood[i] == NpcWants[i])
                {
                    Debug.Log("Adding Points");
                    Reward += 0.5f;
                }
                else if (PlatedFood[i] != NpcWants[i])
                {
                    Debug.Log("Deducting Points");
                    Reward -= 0.4f;
                }
            }
            else if (i >= PlatedFood.Count) { Debug.Log("Deducting Points"); Reward -= 0.3f; }

            if (i < PlatedFood.Count - 1 &&  i >= NpcWants.Count - 1) 
            { float Remove = PlatedFood.Count - NpcWants.Count; Remove = Remove * 0.2f; Reward -= Remove; }
        }

        int Victory = (PlatedFood.Count - 1);
        Victory = Mathf.RoundToInt(Victory / Reward);
        if (Victory == 2) { Agent.StartCoroutine(Agent.ShortWinTimerCheck()); }
        else { Agent.StartCoroutine(Agent.ShortWrongTimerCheck()); }

        Agent.AddReward(Reward);
        Agent.ScoreNum += Reward;
        string RewardText = "Got rewarded" + Reward.ToString() + "Points";
        Debug.Log(RewardText);

        Destroy(FoodOrderCam);
        Plate.SpriteGameObject = new List<GameObject>();


        Spawner.CurrentNpc = null;
        Destroy(NpcParent);
    }
}

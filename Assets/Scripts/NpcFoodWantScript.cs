using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class NpcFoodWantScript : MonoBehaviour
{
    public int MaxIngredientsAllowed = 5;
    public Sprite[] FoodIngredients;
    public List<Sprite> NpcWants = new List<Sprite>();
    public GameObject SpriteSheet;
    void Awake()
    {
        NpcWants.Add(FoodIngredients[0]);

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

    }

    void Update()
    {
        
    }
}

using System.Collections;
using TMPro;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BellScript : MonoBehaviour
{
    CookingAgent Agent;
    PlateScript Plate;

    public ShopHolderScript Holder;

    public TextMeshProUGUI CustomerText;

    public int Served = 0;

    public NpcFoodWantScript CurrentNpc;

    bool WaitTime = false;
    void Awake()
    {
        Plate = Holder.Plate;
        Agent = Holder.Agent;
    }


    private void Update()
    {
        if (CurrentNpc == null) { CurrentNpc = Holder.CurrentNpc; }
    }

    public void GiveFood()
    {
        if (!WaitTime)
        {
            if (CurrentNpc != null)
            {
                if (Plate.PlatedFood.Count > 0)
                {
                    WaitTime = true;
                    CurrentNpc.GetComponent<NpcFoodWantScript>().CheckFood(Plate.PlatedFood,Plate);
                    Plate.PlatedFood.Clear();

                    int ChildNum = Plate.SpriteHolder.gameObject.transform.childCount;
                    Destroy(Plate.ParentHolder.gameObject);
                    CustomerChange(Served + 1);
                    StartCoroutine(Cooldown());
                    Plate.SetMovement(0);

                }
            }
        }
    }

    public void CustomerChange(int NewNum)
    {
        if (NewNum == -1) { Served = 0; }
        else { Served = NewNum; }

        CustomerText.text = "Customers Served:" + Served;
    }
    IEnumerator Cooldown()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        WaitTime = false;
    }
}

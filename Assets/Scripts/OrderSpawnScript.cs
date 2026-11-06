using UnityEngine;

public class OrderSpawnScript : MonoBehaviour
{
    public ShopHolderScript Holder;

    public GameObject Order;

    public GameObject CurrentOrder;
    void Awake()
    {
        OrderSpawn();
    }

    void Update()
    {
        if (CurrentOrder == null)
        { OrderSpawn(); }
    }
    void OrderSpawn()
    {
        if (CurrentOrder == null)
        { GameObject CurrOrder = Instantiate(Order,this.gameObject.transform); CurrOrder.name = "FoodOrderStorer"; CurrentOrder = CurrOrder; Holder.FoodOrderCam = CurrentOrder; }
    }
}

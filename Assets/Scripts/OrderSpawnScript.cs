using UnityEngine;

public class OrderSpawnScript : MonoBehaviour
{
    public GameObject Order;

    public GameObject CurrentOrder;
    void Start()
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
        { GameObject CurrOrder = Instantiate(Order,this.gameObject.transform); CurrOrder.name = "FoodOrderStorer"; CurrentOrder = CurrOrder; }
    }
}

using UnityEngine;

public class PlateStoreSpawner : MonoBehaviour
{
    public GameObject PlateStorer;

    public GameObject CurrentStorer;
    void Start()
    {
        StorerSpawn();
    }

    void Update()
    {
        if (CurrentStorer == null)
        { StorerSpawn(); }
    }
    void StorerSpawn()
    {
        if (CurrentStorer == null)
        { GameObject CurrStorer = Instantiate(PlateStorer, this.gameObject.transform); CurrStorer.name = "PlateStorer"; CurrentStorer = CurrStorer; }
    }
}

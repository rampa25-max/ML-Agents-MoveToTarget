using UnityEngine;

public class CheckPointSpawner : MonoBehaviour
{
    public ShopHolderScript Holder;
    public GameObject CheckpointStorer;

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
        { GameObject CurrStorer = Instantiate(CheckpointStorer, this.gameObject.transform); CurrStorer.name = "Checkpoints"; CurrentStorer = CurrStorer; Holder.CheckPointHolder = CurrentStorer; }
    }
}

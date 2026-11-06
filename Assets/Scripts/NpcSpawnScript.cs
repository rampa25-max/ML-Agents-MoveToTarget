using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class NpcSpawnScript : MonoBehaviour
{
    public GameObject Npc;

    public GameObject CurrentNpc;

    public ShopHolderScript Holder;
    void Start()
    {
        NpcSpawn();
    }

    private void LateUpdate()
    {
        if (CurrentNpc == null)
        { NpcSpawn(); }
    }
    void NpcSpawn()
    {
        if (CurrentNpc == null)
        { GameObject CurrNpc = Instantiate(Npc,Holder.gameObject.transform); CurrNpc.name = "NPC";  CurrentNpc = CurrNpc; CurrentNpc.GetComponentInChildren<NpcFoodWantScript>().Spawner = this; CurrentNpc.GetComponent<NpcMovement>().EndPoint = Holder.NpcEndPlacement; Holder.CurrentNpc = CurrentNpc.GetComponentInChildren<NpcFoodWantScript>(); CurrentNpc.GetComponentInChildren<NpcFoodWantScript>().Agent = Holder.Agent; CurrentNpc.GetComponentInChildren<NpcFoodWantScript>().Spawner = this; CurrentNpc.GetComponentInChildren<NpcFoodWantScript>().Holder = Holder; Holder.CurrentNpc = CurrentNpc.GetComponentInChildren<NpcFoodWantScript>(); }
    }
}

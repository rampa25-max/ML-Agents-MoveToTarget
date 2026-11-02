using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class NpcSpawnScript : MonoBehaviour
{
    public GameObject Npc;

    public GameObject CurrentNpc;
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
        { GameObject CurrNpc = Instantiate(Npc); CurrNpc.name = "NPC";  CurrentNpc = CurrNpc; }
    }
}

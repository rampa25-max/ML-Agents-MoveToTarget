using Unity.MLAgents;
using UnityEngine;

public class TrashScript : MonoBehaviour
{
    CookingAgent Agent;
    void Awake()
    {
        Agent = GameObject.FindAnyObjectByType<CookingAgent>();
    }
    public void TrashFood()
    {
        Debug.Log("Trying to trash food");
        if (Agent.CurrentlyHolding != null)
        { Agent.CurrentlyHolding = null; Debug.Log("Binned Food"); }
    }
}

using UnityEngine;

public class NpcMovement : MonoBehaviour
{
    float SpeedNumber = 0.5f;
    GameObject Npc;
    public Transform EndPoint;
    void Start()
    {
        Npc = this.gameObject;
    }

    void Update()
    {
        if (Npc.transform.localPosition.z < EndPoint.localPosition.z)
        {
            float NewZ = SpeedNumber + Time.fixedDeltaTime;
            float NewVal = transform.localPosition.z + NewZ;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, NewVal);
        }

        if (Npc.transform.localPosition.z > EndPoint.localPosition.z) { Npc.transform.localPosition = new Vector3(Npc.transform.localPosition.x, Npc.transform.localPosition.y, EndPoint.localPosition.z); }
    }
}

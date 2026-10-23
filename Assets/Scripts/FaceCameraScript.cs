using UnityEngine;

public class FaceCameraScript : MonoBehaviour
{
    GameObject CamPos;
    void Awake()
    {
        CamPos = GameObject.FindAnyObjectByType<Camera>().gameObject;
    }
    void FixedUpdate()
    {
        this.gameObject.transform.LookAt(CamPos.transform.position);
    }
}

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MovingScriptBox : MonoBehaviour
{
    public float TimeAlive = 5;
    public float SpeedNumber = -1;

    private void Awake()
    {
        StartCoroutine(Timer());
    }
    void FixedUpdate()
    {
        float NewZ = SpeedNumber + Time.fixedDeltaTime;
        float NewVal = transform.localPosition.z + NewZ;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, NewVal);
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(TimeAlive);
        Destroy(this.gameObject);
    }
}

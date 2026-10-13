using System.Collections;
using UnityEngine;

public class MovingCreator : MonoBehaviour
{
    public GameObject TrainingRoom;
    [SerializeField] private GameObject[] Checkpoints;
    public float SpawnSpeed = 1f;

    void Start()
    {
        StartCoroutine(Spawning());
    }

    IEnumerator Spawning()
    {
        while (true)
        {
            int Randomize = Random.Range(0, Checkpoints.Length);
            var SpawnedObj = Instantiate(Checkpoints[Randomize]);
            SpawnedObj.transform.SetParent(TrainingRoom.transform, false);
            SpawnedObj.transform.localPosition = new Vector3(0,0,90);
            yield return new WaitForSeconds(SpawnSpeed);
        }
    }
}

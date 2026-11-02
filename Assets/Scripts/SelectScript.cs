using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.GameCenter;
public class SelectScript : MonoBehaviour
{
    CookingAgent Agent;
    RectTransform ThisRect;
    public List<RectTransform> Foods;
    [SerializeField] int MaxNum;
    public bool CorrectChoice = false;

    public GameObject DisableParent;
    void Awake()
    {
        Agent = GameObject.FindAnyObjectByType<CookingAgent>();
        ThisRect = this.gameObject.GetComponent<RectTransform>();
        MaxNum = Foods.Count - 1;
    }

    void OnEnable()
    {
        Agent.InMenu = true;
        Agent.ChooseNumber = 0;
        Agent.MaxNum = MaxNum;
        Agent.CurrentMenu = DisableParent;
        Agent.CurrentSelectScript = this;
    }

    void Update()
    {
        ThisRect.localPosition = Foods[Agent.ChooseNumber].localPosition;
    }
}

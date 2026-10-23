using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CuttingProcess : MonoBehaviour
{
    [Header("Requirements")]

    CookingAgent Agent;

    public Image SpriteHolder;
    public TextMeshProUGUI PercentText;

    public Sprite[] NeededSprite;

    public Sprite[] CutVersion;

    bool InUse = false;

    bool CanTake = false;

    Sprite CurrentSprite;

    [SerializeField] int CuttingPercentage = 0;

    [Header("Settings")]

    public int[] CutPercent;
    float CuttingInterval = 0.15f;

    int ArrayNum = 0;

    bool CanCut = true;
    void Awake()
    {
        Agent = GameObject.FindAnyObjectByType<CookingAgent>();
        CurrentSprite = null;
        InUse = false;
        CanTake = false;
    }
    private void FixedUpdate()
    {
        if (CurrentSprite != null) { SpriteHolder.sprite = CurrentSprite; SpriteHolder.color = new Color(SpriteHolder.color.r, SpriteHolder.color.g, SpriteHolder.color.b, 1); PercentText.text = CuttingPercentage.ToString() + "%"; }
        else { SpriteHolder.sprite = null; SpriteHolder.color = new Color(SpriteHolder.color.r, SpriteHolder.color.g, SpriteHolder.color.b, 0); PercentText.text = null; }
    }
    public void Cutting()
    {
        if (InUse == false)
        {
            for (int i = 0; i < NeededSprite.Length; i++)
            {
                if (Agent.CurrentlyHolding == NeededSprite[i])
                {
                    CuttingPercentage = 0;
                    CurrentSprite = NeededSprite[i];
                    Agent.CurrentlyHolding = null;
                    Debug.Log("Placed Food");
                    InUse = true;
                    CanTake = false;
                    CanCut = true;
                    ArrayNum = i;
                    return;
                }
            }
            Debug.LogWarning("Wrong Food Item");
        }
        else if (InUse == true && CanTake == false)
        {
            if (CanCut == true) { CanCut = false; StartCoroutine( CuttingIncrease() ); }
        }

        else if (InUse == true && CanTake == true)
        {
            Debug.Log("Taking Food");
            Agent.CurrentlyHolding = CurrentSprite;
            CurrentSprite = null;
            InUse = false;
            CanTake = false;
            Debug.Log("Took Food");
            CuttingPercentage = 0;
        }
    }
    IEnumerator CuttingIncrease()
    {
            CuttingPercentage += CutPercent[ArrayNum];
            if (CuttingPercentage >= 100)
            {
                CuttingPercentage = 100;
                CanCut = false;
                CurrentSprite = CutVersion[ArrayNum];
                yield return new WaitForSeconds(0.5f);
                CanTake = true;
            }
            else
            {
                yield return new WaitForSeconds(CuttingInterval);
                CanCut = true;
            }
    }
}
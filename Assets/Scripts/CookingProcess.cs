using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookingProcess : MonoBehaviour
{
    [Header("Requirements")]

    CookingAgent Agent;

    public Image SpriteHolder;
    public TextMeshProUGUI CountDown;

    public Sprite NeededSprite;

    public Sprite CookedVersion;

    public Sprite BurntVersion;

    bool InUse = false;

    bool CanTake = false;

    Sprite CurrentSprite;

    public bool IsCooking = false;

    [Header("Settings")]

    public int CurrentTime = 0;

    public int CookedTimer = 15;

    public int BurntTimer = 10;
    void Awake()
    {
        Agent = GameObject.FindAnyObjectByType<CookingAgent>();
        CurrentSprite = null;
        InUse = false;
        CanTake = false;
        CookedTimer = CookedTimer - 1;
    }
    private void FixedUpdate()
    {
        if (CurrentSprite != null) { SpriteHolder.sprite = CurrentSprite; SpriteHolder.color = new Color(SpriteHolder.color.r, SpriteHolder.color.g, SpriteHolder.color.b, 1); CountDown.text = CurrentTime.ToString(); }
        else { SpriteHolder.sprite = null; SpriteHolder.color = new Color(SpriteHolder.color.r, SpriteHolder.color.g, SpriteHolder.color.b, 0); CountDown.text = null; }
    }
    public void StartCooking()
    {
        if (InUse == false)
        {
            if (Agent.CurrentlyHolding == NeededSprite)
            {
                IsCooking = true;
                CurrentSprite = NeededSprite;
                StartCoroutine(WaitTimer());
                Agent.CurrentlyHolding = null;
                Debug.Log("Cooking Food");
                InUse = true;
                CanTake = false;
            }
            else { Debug.LogWarning("Wrong Food Item"); }
        }

        else if (InUse == true && CanTake == true)
        {
            IsCooking = false;
            Debug.Log("Taking Food");
            StopCoroutine(WaitTimer());
            Agent.CurrentlyHolding = CurrentSprite;
            CurrentSprite = null;
            InUse = false;
            CanTake = false;
            Debug.Log("Took Food");
        }
    }

    IEnumerator WaitTimer()
    {
        CurrentTime = CookedTimer;
        yield return new WaitForSeconds(1);
        while (CurrentTime > 0 && InUse == true)
        {
            yield return new WaitForSeconds(1f);
            CurrentTime -= 1;
        }
        CurrentTime = 0;
        if (InUse == true)
        {
            CurrentSprite = CookedVersion;
            CanTake = true;
            Debug.Log("CanTake Food");
            Debug.Log("Cooked the Food");
        }

        CurrentTime = BurntTimer;
        while (CurrentTime > 0 && InUse == true)
        {
            yield return new WaitForSeconds(1f);
            CurrentTime -= 1;
        }
        if (InUse == true)
        {
            CurrentSprite = BurntVersion;
            Debug.Log("Burnt the Food");
        }
    }
}

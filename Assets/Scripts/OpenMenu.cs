using NUnit.Framework;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class OpenMenu : MonoBehaviour
{
    public ShopHolderScript Holder;
    CookingAgent Agent;
    public GameObject Menu;

    void Awake()
    {
        Agent = Holder.Agent;
    }

    public void OpeningMenu()
    {
        if (Menu.activeInHierarchy == false)
        {
            Debug.Log("Opening Menu");
            Menu.SetActive(true);
        }
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class OpenMenu : MonoBehaviour
{
    CookingAgent Agent;
    public GameObject Menu;

    void Awake()
    {
        Agent = GameObject.FindAnyObjectByType<CookingAgent>();
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

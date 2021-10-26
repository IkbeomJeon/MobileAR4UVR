using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class: MainManager
// UI manager for tutorial and UI
public class MainManager : MonoBehaviour
{
    [SerializeField]
    private bool tutorial;

    [SerializeField]
    private GameObject ui;
    [SerializeField]
    private GameObject tutorialUI;

    private void Start()
    {
        ui.SetActive(!tutorial);
        tutorialUI.SetActive(tutorial);
    }
}

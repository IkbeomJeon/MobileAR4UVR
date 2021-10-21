using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject menu;
    public GameObject searchPannel;

    private void Start()
    {
        menu = GameObject.Find("UI/Menu");
        searchPannel = GameObject.Find("UI/SearchPannel");
    }
}

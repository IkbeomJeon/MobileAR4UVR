using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType { search, map };
public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject menu;
    //public GameObject searchPanel;
    //public GameObject mapPanel;

    public List<GameObject> panels = new List<GameObject>();


    public void Awake()
    {
        //pannels.Add(GameObject.Find("UI/Menu"));
        panels.Add(transform.Find("SearchPanel").gameObject);
        panels.Add(transform.Find("MapPanel").gameObject);

    }
    public void ShowPannel(GameObject panelToShow)
    {
        foreach (var panel in panels)
            panel.SetActive(false);
        panelToShow.SetActive(true);
    }
}

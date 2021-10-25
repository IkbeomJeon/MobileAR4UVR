using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelType { search, map };
public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject menu;
    public GameObject capturePanel;
    public GameObject searchPanel;
    public GameObject mapPanel;

    //public List<GameObject> panels = new List<GameObject>();
    public Dictionary<GameObject, bool> panelState = new Dictionary<GameObject, bool>();

    public void Awake()
    {
        capturePanel = transform.Find("CapturePanel").gameObject;
        searchPanel = transform.Find("SearchPanel").gameObject;
        mapPanel = transform.Find("MapPanel").gameObject;

        panelState.Add(capturePanel, false);
        panelState.Add(searchPanel, false);
        panelState.Add(mapPanel, false);

        foreach (var panel in panelState.Keys)
            panel.SetActive(false);

    }
    public void SwitchPanelState(GameObject panel)
    {
        if (panelState[panel])
        {
            panelState[panel] = false;
            panel.SetActive(false);
        }
        else
        {
            panelState[panel] = true;
            panel.SetActive(true);
        }
    }
    public void ChangeSearchPanelState(bool flag)
    {
        searchPanel.SetActive(flag);
        panelState[searchPanel] = flag;
    }
    public void ChangeMapPanelState(bool flag)
    {
        mapPanel.SetActive(flag);
        panelState[mapPanel] = flag;
    }

    
    //public void StartNavigation(List<Vector2d> waypoints)
    //{
    //    Debug.Log(waypoints.Count);
    //    searchPanel.SetActive(false);
    //    panelState[searchPanel] = false;

    //    mapPanel.SetActive(true);
    //    panelState[mapPanel] = true;

    //    //foreach (var panel in panelState.Keys)
    //    //{
    //    //    panelState[panel] = false;
    //    //    panel.SetActive(false);
    //    //}

    //    //turn on map panel;
    //    //SwitchPanelState(mapPanel);
    //    mapPanel.GetComponent<MapManager>().DrawNavigationRoute(waypoints);
    //}
}

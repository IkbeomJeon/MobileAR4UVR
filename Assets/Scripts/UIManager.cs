using KCTM.Network.Data;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PanelType { search, map };
public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject menu;
    public GameObject capturePanel;
    public GameObject searchPanel;
    public GameObject mapPanel;
    public GameObject configurationPanel;
    public GameObject contentPanel;

    public GameObject navStopButton;


    public delegate void ButtonCallBack();

    //public List<GameObject> panels = new List<GameObject>();
    public void Awake()
    {
        capturePanel = transform.Find("CapturePanel").gameObject;
        searchPanel = transform.Find("SearchPanel").gameObject;
        mapPanel = transform.Find("MapPanel").gameObject;
        navStopButton = transform.Find("StopNavButton").gameObject;
        contentPanel = transform.Find("ContentPanel").gameObject;
        configurationPanel = transform.Find("ConfigPanel").gameObject;

        capturePanel.SetActive(false);
        searchPanel.SetActive(false);
        mapPanel.SetActive(false);
        navStopButton.SetActive(false);
        contentPanel.SetActive(false);
        configurationPanel.SetActive(false);

    }
 
    public void SwitchSearchPanelState()
    {
        if (searchPanel.activeSelf)
            searchPanel.SetActive(false);
        else
            searchPanel.SetActive(true);


    }
    public void SwitchMapPanelState()
    {
        if (mapPanel.activeSelf)
            mapPanel.SetActive(false);

        else
            mapPanel.SetActive(true);
    }

    public void SwitchConfigurationPanelState()
    {
        if (configurationPanel.activeSelf)
            configurationPanel.SetActive(false);

        else
            configurationPanel.SetActive(true);
    }

    public void ChangeSearchPanelState(bool flag)
    {
        searchPanel.SetActive(flag);
    }
    public void ChangeMapPanelState(bool flag)
    {
        mapPanel.SetActive(flag);
    }
    public void ShowContentPanel(PreviewCard previewCard)
    {
        //remove exsisting card.
        var cardParent = contentPanel.transform.Find("CardObject/Scroll View/Viewport/Card");
        foreach (Transform child in cardParent)
            DestroyImmediate(child.gameObject);
        
        //reset canvas and create card.
        contentPanel.transform.Find("Title").GetComponent<Text>().text = previewCard.anchor.contentinfos[0].content.mediatype;
        Button close_button = contentPanel.transform.Find("DownButton").gameObject.GetComponent<Button>();
        close_button.onClick.RemoveAllListeners();

        close_button.onClick.AddListener(delegate {
            CloseContentPanel(previewCard);
        });

        var newCard = Instantiate(ResourceLoader.Instance.card_Image, cardParent);
        newCard.GetComponent<ImageCard>().Init(previewCard.anchor, "", false);

        contentPanel.SetActive(true);
    }

    public void CloseContentPanel(PreviewCard previewCard)
    {
        var cardParent = contentPanel.transform.Find("CardObject/Scroll View/Viewport/Card");
        foreach (Transform child in cardParent)
            DestroyImmediate(child.gameObject);

        //Recommendation
        previewCard.addToVisitedContent();
        DestroyImmediate(previewCard.gameObject);
        contentPanel.SetActive(false);
        
    }
    public void ChangeARButtonState(bool flag)
    {
        navStopButton.SetActive(flag);
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

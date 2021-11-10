using KCTM.Network.Data;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public enum PanelType { search, map };
public class PanelManager : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject menu;
    public GameObject searchPanel;
    public GameObject mapPanel;
    public GameObject configurationPanel;
    public GameObject contentPanel;
    public GameObject FullscreenPanel;
    public GameObject navStopButton;


    public delegate void ButtonCallBack();

    //public List<GameObject> panels = new List<GameObject>();
    public void Awake()
    {
        searchPanel = transform.Find("SearchPanel").gameObject;
        mapPanel = transform.Find("MapPanel").gameObject;
        
        navStopButton = transform.Find("StopNavButton").gameObject;
        contentPanel = transform.Find("ContentPanel").gameObject;
        configurationPanel = transform.Find("ConfigPanel").gameObject;
        FullscreenPanel = transform.Find("FullscreenPanel").gameObject;

        var button_fs = FullscreenPanel.transform.Find("Fullscreen").GetComponent<Button>();
        button_fs.onClick.RemoveAllListeners();
        button_fs.onClick.AddListener(delegate {
            FullscreenPanel.SetActive(false);
        });

        searchPanel.SetActive(false);
        mapPanel.SetActive(false);
        navStopButton.SetActive(false);
        contentPanel.SetActive(false);
        configurationPanel.SetActive(false);
        FullscreenPanel.SetActive(false);

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
        contentPanel.SetActive(true);

        //remove exsisting card.
        var cardParent = contentPanel.transform.Find("CardObject/Scroll View/Viewport/Card");
        foreach (Transform child in cardParent)
            Destroy(child.gameObject);
        
        //reset canvas and create card.
        contentPanel.transform.Find("Title").GetComponent<Text>().text = previewCard.anchor.contentinfos[0].content.mediatype;
        Button close_button = contentPanel.transform.Find("DownButton").gameObject.GetComponent<Button>();
        close_button.onClick.RemoveAllListeners();

        close_button.onClick.AddListener(delegate {
            CloseContentPanel();
        });

        string type = previewCard.anchor.contentinfos[0].content.mediatype;
        GameObject newCard;

        switch (type)
        {
            case "IMAGE":
                newCard = Instantiate(ResourceLoader.Instance.card_Image, cardParent);
                newCard.AddComponent<ImageCard>().Init(previewCard.anchor, "", false);

                break;
            case "VIDEO":
            default:
                newCard = Instantiate(ResourceLoader.Instance.card_Video, cardParent);
                newCard.AddComponent<VideoCard>().Init(previewCard.anchor, "", false, previewCard);
                break;
        }
       
    }

    public void CloseContentPanel()
    {
        var cardParent = contentPanel.transform.Find("CardObject/Scroll View/Viewport/Card");
        foreach (Transform child in cardParent)
            Destroy(child.gameObject);

        //Recommendation
        //previewCard.addToVisitedContent();
        //DestroyImmediate(previewCard.gameObject);
        contentPanel.SetActive(false);
        
    }

    public void ShowFullScreenPanel(Image image, float scale)
    {
        FullscreenPanel.SetActive(true); 

        var button_img = FullscreenPanel.transform.Find("Main").gameObject;
        button_img.GetComponent<AspectRatioFitter>().aspectRatio = scale;
        button_img.GetComponent<RawImage>().texture = image.mainTexture;
    }
    public void ShowFullScreenPanel(VideoPlayer video)
    {
        FullscreenPanel.SetActive(true);

        var button_img = FullscreenPanel.transform.Find("Main").gameObject;
        button_img.GetComponent<AspectRatioFitter>().aspectRatio = (float)video.width/ video.height;
        button_img.GetComponent<RawImage>().texture = video.targetTexture;
    }

    public void ChangeARButtonState(bool flag)
    {
        navStopButton.SetActive(flag);
    }
    
}

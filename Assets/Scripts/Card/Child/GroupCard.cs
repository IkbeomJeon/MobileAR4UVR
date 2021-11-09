using KCTM.Network;
using KCTM.Network.Data;
using Mapbox.Json;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WayPoint
{
    public Vector2d pos;
    public bool isPOI;

    public WayPoint(Vector2d pos, bool isPOI)
    {
        this.pos = pos;
        this.isPOI = isPOI;
    }
}
public class GroupCard : NormalCard
{
    GameObject stepCard;
    bool moreFlag = false;
    public Image moreButtonImage;
    public Text moreButtonText;
    private List<WayPoint> anchor_posList = new List<WayPoint>();
    List<Anchor> story = new List<Anchor>();

    public Text timeToExp;

    //ResourceLoader resourceLoader;
    public void Init(Anchor anchor, bool showGoButton=false)
    {
        //SetIcon();
        base.Init(anchor, "Card/");

        
        Transform goButton = transform.Find("Card/BottomInfo/GoButton");
        if (goButton!=null)
        {
            goButton.gameObject.SetActive(false);

          
            goButton.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();

            goButton.gameObject.GetComponent<Button>().onClick.AddListener(delegate {
                OnStartNavigation();
            });

            if (showGoButton)
                goButton.gameObject.SetActive(true);
        }

        Button more_button = transform.Find("Card/BottomInfo/MoreButton").gameObject.GetComponent<Button>();
        more_button.onClick.RemoveAllListeners();
        more_button.onClick.AddListener(delegate {
            OnMore();
        });



        moreButtonText = transform.Find("Card/BottomInfo/MoreButton/Text").gameObject.GetComponent<Text>();
        moreButtonImage = transform.Find("Card/BottomInfo/MoreButton/Image").gameObject.GetComponent<Image>();
        timeToExp = transform.Find("Card/BottomInfo/TimeToExp").GetComponent<Text>();


        stepCard = transform.Find("StepCards").gameObject;
        GetStoryTelling(anchor);
    }
 
    private void GetStoryTelling(Anchor anchor)
    {
        int idx = 0;

        for (int i = 0; i < anchor.linkedAnchors.Count; i++)
        {
            //Debug.Log(linked_anchor.point.latitude.ToString() + ", "+linked_anchor.point.longitude.ToString());
            
            //case : just middle point not poi.
            if (anchor.linkedAnchors[i].linkedAnchors.Count == 0)
            {
                double lon = anchor.linkedAnchors[i].point.longitude;
                double lat = anchor.linkedAnchors[i].point.latitude;
                anchor_posList.Add(new WayPoint(new Vector2d(lat, lon), false));
            }

            //case : poi
            for (int j = 0; j < anchor.linkedAnchors[i].linkedAnchors.Count; j++)
            {
                var linked_anchor = anchor.linkedAnchors[i].linkedAnchors[j];
                story.Add(linked_anchor);

                double lon = linked_anchor.point.longitude;
                double lat = linked_anchor.point.latitude;

                anchor_posList.Add(new WayPoint(new Vector2d(lat, lon), true));

                CreateSmallCardObject(linked_anchor, 0, idx);
               
                idx++;
            }
            
        }
        stepCard.SetActive(false);
    }
    public void OnMore()
    {
        if (!moreFlag)
        {
            stepCard.SetActive(true);
            moreFlag = true;
            moreButtonImage.sprite = ResourceLoader.Instance.moreUp;
            moreButtonText.text = "감추기";

            StartCoroutine(StartDownloadStepCardContent());
        }
        else
        {
            stepCard.SetActive(false);
            moreFlag = false;
            moreButtonImage.sprite = ResourceLoader.Instance.moreDown;
            moreButtonText.text = "더보기";
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(stepCard.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(cardContent.GetComponent<RectTransform>());
        cardContent.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        cardContent.GetComponent<ContentSizeFitter>().SetLayoutVertical();
    }

    IEnumerator StartDownloadStepCardContent()
    {
        foreach (Transform child in stepCard.transform)
        {
            yield return child.gameObject.GetComponent<NormalCard>().DownloadContent();
        }
    }

    public void OnStartNavigation()
    {
        //메뉴매니저에 UI변경
        panelManager.GetComponent<PanelManager>().ChangeSearchPanelState(false);
        panelManager.GetComponent<PanelManager>().ChangeMapPanelState(true);
        panelManager.GetComponent<PanelManager>().ChangeARButtonState(true);

        //panelManager.GetComponent<PanelManager>().StartNavigation(anchor_posList);
        //panelManager.SendMessage("StartNavigation", anchor_posList);
        //네비게이션 생성

        mapManager.GetComponent<MapManager>().ActivateMap();
        mapManager.GetComponent<MapManager>().StartNavigation(anchor_posList, story);
        

        
        //Create POI Icons.
        var parentTargetARScene = GameObject.Find("ARSceneParent_Target").transform;

        foreach (Transform child in parentTargetARScene)
            Destroy(child.gameObject);

        for (int i=0; i<story.Count;i++)
        {
            var poi = Instantiate(ResourceLoader.Instance.icon_poi, parentTargetARScene);
            poi.GetComponent<IconManager>().Init(story[i], "", GameObject.FindGameObjectWithTag("MainCamera").transform, true, i+1);
        }
       
    }
   
    private void CreateSmallCardObject(Anchor anchor, int indexCount, int index)
    {
        GameObject small_card;
        switch (anchor.contentinfos[indexCount].content.mediatype)
        {

            case "IMAGE":
                small_card = Instantiate(ResourceLoader.Instance.card_Image_small, stepCard.transform);
                small_card.AddComponent<ImageCard_Small>().Init(anchor, index+1);

             
                break;
            case "VIDEO":
            default:
                small_card = Instantiate(ResourceLoader.Instance.card_Video_small, stepCard.transform);
                small_card.AddComponent<VideoCard_Small>().Init(anchor, index + 1);
                break;
        }
    }
}

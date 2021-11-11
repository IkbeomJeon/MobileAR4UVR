using KCTM.Network;
using KCTM.Network.Data;
using Mapbox.Json;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class WayPoint
{
    public Vector2d pos;
    public double elevation;
    public bool isPOI;
    
    public WayPoint(Vector2d pos, double elevation, bool isPOI)
    {
        this.pos = pos;
        this.isPOI = isPOI;
        this.elevation = elevation;
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
        bool IsPOIAnchor(Anchor _anchor)
        {
            if (_anchor.title == "Navigation" || _anchor.title == "navigation"
                    || _anchor.linkedAnchors.Count == 0
                    || _anchor.linkedAnchors[0].title == "Navigation" || _anchor.linkedAnchors[0].title == "navigation")
                return false;
            else
                return true;
        }

        int idx_smallcard = 0;
        for (int i = 0; i < anchor.linkedAnchors.Count; i++)
        {
            //case : just middle point not poi.
            Anchor linked_anchor = anchor.linkedAnchors[i];

            if (!IsPOIAnchor(linked_anchor))
            {
                double lon = linked_anchor.point.longitude;
                double lat = linked_anchor.point.latitude;
                double elevation = 0;

                //////////////////////////////////////////////////////////////////////
                ///추가할곳
                if (ConfigurationManager.Instance.use_anchors_height == 1 && linked_anchor.linkedAnchors.Count > 0)
                    elevation = linked_anchor.linkedAnchors[0].contentinfos[0].positiony;
                /////////////////////////////////

                anchor_posList.Add(new WayPoint(new Vector2d(lat, lon), elevation, false));
            }

            else
            {
                foreach (Anchor anchor_story in linked_anchor.linkedAnchors)
                {
                    story.Add(anchor_story);

                    double lon = anchor_story.point.longitude;
                    double lat = anchor_story.point.latitude;
                    double elevation = 0;

                    if (ConfigurationManager.Instance.use_anchors_height == 1)
                        elevation = anchor_story.contentinfos[0].positiony;

                    anchor_posList.Add(new WayPoint(new Vector2d(lat, lon), elevation, true));

                    CreateSmallCardObject(anchor_story, 0, idx_smallcard++);
                }
                
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

            //StartCoroutine(StartDownloadStepCardContent());
            foreach (Transform child in stepCard.transform)
            {
                child.gameObject.GetComponent<NormalCard>().DownloadContent();
            }

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

 

    public void OnStartNavigation()
    {
        //메뉴매니저에 UI변경
        panelManager.GetComponent<PanelManager>().ChangeSearchPanelState(false);
        panelManager.GetComponent<PanelManager>().ChangeMapPanelState(true);
        panelManager.GetComponent<PanelManager>().ChangeARButtonState(true);

        //네비게이션 생성

        mapManager.GetComponent<MapManager>().ActivateMap();
        mapManager.GetComponent<MapManager>().StartNavigation(anchor_posList, story);
        
        //Create POI Icons.
        var parentTargetARScene = GameObject.Find("ARSceneParent_Target").transform;
        var parentARScene = GameObject.Find("ARSceneParent").transform;

        foreach (Transform child in parentTargetARScene)
            Destroy(child.gameObject);

        //if(list_anchorIDs_to_create.Count > 0)


        for (int i=0; i<story.Count;i++)
        {
            var poi = Instantiate(ResourceLoader.Instance.icon_poi, parentTargetARScene);
            poi.GetComponent<IconManager>().Init(story[i], "", GameObject.FindGameObjectWithTag("MainCamera").transform, true, i+1);

            //remove existing icon.
            //중복된거 제거.(저작도구에서 잘못 등록한 듯)
            var list_IconManager = parentARScene.GetComponentsInChildren<IconManager>().Where(icon => icon.anchor.title == story[i].title).ToList();

            if(list_IconManager!=null && list_IconManager.Count > 0 )
            {
                foreach(var icon in list_IconManager)
                {
                    Debug.Log("removed duplicated icon : " + icon.anchor.id);
                    DestroyImmediate(icon.gameObject);
                }
            }
        }

        //User 높이 업데이트
        float first_poi_height = (float) story[0].contentinfos[0].positiony;

        if(ConfigurationManager.Instance.use_anchors_height ==1)
        {
            var trackerClientManager = GameObject.FindObjectsOfType<TrackerClientManager>();
            if (trackerClientManager == null && trackerClientManager.Length != 1)
            {
                Debug.LogError("Can't find \"TrackerClientManager\" ");
            }

            trackerClientManager[0].UpdateGlobalHeightManually(first_poi_height);
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

﻿using KCTM.Network;
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
public class GroupCard : BaseCard
{

    GameObject stepCard;
    bool moreFlag = false;
    public Image moreButtonImage;
    private List<WayPoint> anchor_posList = new List<WayPoint>();
    public Text timeToExp;

    public Text description;
    public Text author;
    public Text upload;
    public Transform tagParent;
    ResourceLoader resourceLoader;
    public void Init(Anchor anchor)
    {
        //SetIcon();
        base.Init(anchor, "Card/");

        author = transform.Find("Card/BottomInfo/AuthorText").GetComponent<Text>();
        upload = transform.Find("Card/BottomInfo/UploadText").GetComponent<Text>();
        timeToExp = transform.Find("Card/BottomInfo/TimeText").GetComponent<Text>();
        stepCard = transform.Find("StepCards").gameObject;
        description = transform.Find("Card/TextObject/ScrollArea/Description").GetComponent<Text>();
        description.text = anchor.description;
        author.text = anchor.contentinfos[0].content.user.name;
        upload.text = Util.GetHumanTimeFormatFromMilliseconds(anchor.contentinfos[0].content.updatedtime);
        resourceLoader = GameObject.Find("ResourceLoader").GetComponent<ResourceLoader>();

        GetStoryTelling(anchor);

        // Loading Tags
        tagParent = transform.Find("Card/Tags/Scroll View/Viewport/TagContent").transform;
      

        for (int i = 0; i < anchor.tags.Count; i++)
        {
            GameObject tag = Instantiate(resourceLoader.tagObj, tagParent);
            tag.transform.GetChild(0).GetComponent<Text>().text = anchor.tags[i].tag;
        }
    }
 
    private void GetStoryTelling(Anchor anchor)
    {
        /// This is where order for anchor is determined

        //// 순서: spacetelling - POIs(arscenes) - stories(arscenes)
        //stories.Add(anchor);
        int idx = 0;
  
        for (int i = 0; i < anchor.linkedAnchors.Count; i++)
        {
            // Now add stories (arscenes)
            //Debug.Log(linked_anchor.point.latitude.ToString() + ", "+linked_anchor.point.longitude.ToString());
            ///// set position
            ///
            
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
                double lon = linked_anchor.point.longitude;
                double lat = linked_anchor.point.latitude;

                anchor_posList.Add(new WayPoint(new Vector2d(lat, lon), true));

                CreateSmallCardObject(linked_anchor, 0, idx);
               
                idx++;
            }
            
        }
        stepCard.SetActive(false);
    }
    void CreatePOIAnchorIcon(Anchor anchor, int idx)
    {
       
    }
    public void OnMore()
    {
        if (!moreFlag)
        {
            stepCard.SetActive(true);
            moreFlag = true;
            //moreButtonImage.sprite = ResourceLoader.Instance.moreDown;
        }
        else
        {
            stepCard.SetActive(false);
            moreFlag = false;
            //moreButtonImage.sprite = ResourceLoader.Instance.moreUp;
        }


        LayoutRebuilder.ForceRebuildLayoutImmediate(stepCard.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(cardContent.GetComponent<RectTransform>());
        cardContent.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        cardContent.GetComponent<ContentSizeFitter>().SetLayoutVertical();
    }

    public void OnStartNavigation()
    {
        //메뉴매니저에 UI변경
        uiManager.GetComponent<UIManager>().ChangeSearchPanelState(false);
        uiManager.GetComponent<UIManager>().ChangeMapPanelState(true);
        uiManager.GetComponent<UIManager>().ChangeARButtonState(true);

        //uiManager.GetComponent<UIManager>().StartNavigation(anchor_posList);
        //uiManager.SendMessage("StartNavigation", anchor_posList);
        //네비게이션 생성
        mapManager.GetComponent<MapManager>().DrawNavigationRoute(anchor_posList);
        mapManager.GetComponent<MapManager>().ActivateMap();

        
        //Create POI Icons.
        var parentTargetARScene = GameObject.Find("ARSceneParent_Target").transform;
        foreach (Transform child in parentTargetARScene)
            DestroyImmediate(child.gameObject);

        int idx = 0;
        for (int i = 0; i < anchor.linkedAnchors.Count; i++)
        {
            //case : poi
            for (int j = 0; j < anchor.linkedAnchors[i].linkedAnchors.Count; j++)
            {
                var linked_anchor = anchor.linkedAnchors[i].linkedAnchors[j];
                var poi = Instantiate(resourceLoader.icon_poi, parentTargetARScene);
                poi.GetComponent<IconManager>().Init(linked_anchor, "", GameObject.FindGameObjectWithTag("MainCamera").transform, 1.5f, ++idx);
            }

        }
    }
   
    private void CreateSmallCardObject(Anchor anchor, int indexCount, int index)
    {
        switch (anchor.contentinfos[indexCount].content.mediatype)
        {
            case "IMAGE":
                var small_card = Instantiate(resourceLoader.card_Image_small, stepCard.transform);
                small_card.GetComponent<ImageCard_Small>().Init(anchor, index+1);

                //card.GetComponent<ImageCardNavPrefab>().arScene = anchor;
                //card.GetComponent<ImageCardNavPrefab>().indexContent = index;
                //card.GetComponent<ImageCardNavPrefab>().navigation = navigation;
                //card.GetComponent<ImageCardNavPrefab>().searchPanel = searchPanel;
                break;
        }
    }
}
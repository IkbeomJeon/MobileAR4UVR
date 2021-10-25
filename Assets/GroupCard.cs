﻿using KCTM.Network;
using KCTM.Network.Data;
using Mapbox.Json;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GroupCard : BaseCard
{
    GameObject stepCard;
    bool moreFlag = false;
    public Image moreButtonImage;
    private List<Vector2d> anchor_posList = new List<Vector2d>();
    public Text timeToExp;


    public override void Init(Anchor anchor, string parentName="")
    {
        //SetIcon();
        base.Init(anchor, parentName);

        timeToExp = transform.Find(parentName+"BottomInfo/TimeText").GetComponent<Text>();
        stepCard = transform.Find("StepCards").gameObject;
        GetStoryTelling(anchor);
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
            for (int j = 0; j < anchor.linkedAnchors[i].linkedAnchors.Count; j++)
            {
                var linked_anchor = anchor.linkedAnchors[i].linkedAnchors[j];
                //Debug.Log(linked_anchor.point.latitude.ToString() + ", "+linked_anchor.point.longitude.ToString());
                ///// set position
                double lon = linked_anchor.point.longitude;
                double lat = linked_anchor.point.latitude;
                anchor_posList.Add(new Vector2d(lat, lon));

                CreateSmallCardObject(linked_anchor, 0, idx++);
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
        mapManager.GetComponent<MapManager>().SetWayPoints(anchor_posList);
        mapManager.GetComponent<MapManager>().ActivateMap();
    }
   
    private void CreateSmallCardObject(Anchor anchor, int indexCount, int index)
    {
        switch (anchor.contentinfos[indexCount].content.mediatype)
        {
            case "IMAGE":
                var small_card = Instantiate(ResourceLoader.Instance.card_Image_small, stepCard.transform);
                small_card.GetComponent<ImageCard_Small>().Init(anchor, index+1);

                //card.GetComponent<ImageCardNavPrefab>().arScene = anchor;
                //card.GetComponent<ImageCardNavPrefab>().indexContent = index;
                //card.GetComponent<ImageCardNavPrefab>().navigation = navigation;
                //card.GetComponent<ImageCardNavPrefab>().searchPanel = searchPanel;
                break;
        }
    }
}

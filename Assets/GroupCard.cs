using KCTM.Network;
using KCTM.Network.Data;
using Mapbox.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupCard : BaseCard
{
    GameObject stepCard;
    bool moreFlag = false;
    public Image moreButtonImage;
    private List<Anchor> stories;
    public Text timeToExp;


    public override void Init(Anchor anchor)
    {
        //title.text = anchor.title;
        //SetIcon();
        base.Init(anchor);

        //cardContent = GameObject.Find("CardContent");

        //title = transform.Find("Card/Title").GetComponent<Text>();
        //description = transform.Find("Card/TextObject/ScrollArea/Text").GetComponent<Text>();
        //author = transform.Find("Card/BottomInfo/AuthorText").GetComponent<Text>();
        //upload = transform.Find("Card/BottomInfo/UploadText").GetComponent<Text>();

        title.text = anchor.title;
        description.text = anchor.description;
        author.text = anchor.contentinfos[0].content.user.name;
        upload.text = Util.GetHumanTimeFormatFromMilliseconds(anchor.contentinfos[0].content.updatedtime);
        //author.text = anchor.

        timeToExp = transform.Find("Card/BottomInfo/TimeText").GetComponent<Text>();
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
                CreateSmallCardObject(anchor.linkedAnchors[i].linkedAnchors[j], 0, idx++);
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

    public void AddStepCard(Anchor anchor)
    {

        //var card = CreateCardObject(anchor)
    }
    private void CreateSmallCardObject(Anchor anchor, int indexCount, int index)
    {
        switch (anchor.contentinfos[indexCount].content.mediatype)
        {
            case "IMAGE":
                var small_card = Instantiate(ResourceLoader.Instance.card_Image_small, stepCard.transform);
                small_card.GetComponent<ImageCard>().Init(anchor);

                //card.GetComponent<ImageCardNavPrefab>().arScene = anchor;
                //card.GetComponent<ImageCardNavPrefab>().indexContent = index;
                //card.GetComponent<ImageCardNavPrefab>().navigation = navigation;
                //card.GetComponent<ImageCardNavPrefab>().searchPanel = searchPanel;
                break;
        }
    }
}

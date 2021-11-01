using System;
using System.Collections;
using System.Collections.Generic;
using KCTM.Network;
using KCTM.Network.Data;
using KCTM.Recommendation;
using Mapbox.Json;
using UnityEngine;
using UnityEngine.UI;

public class PreviewCard : BaseCard
{
    public DateTime start;

    private int userLiked = 1;
    public Text likeNumber;
    public Image likeImage;
    public Button likeButton;
    //public ResourceLoader rl;

    public virtual void Init(Anchor anchor,bool isStory, int index_story)
    {
        //Recommendation
        start = DateTime.Now;

        base.Init(anchor, "Card/");
        likeNumber = transform.Find("Card/LikeNumber").gameObject.GetComponent<Text>();
        likeImage = transform.Find("Card/Like").GetComponent<Image>();
        likeButton = transform.Find("Card/Like").GetComponent<Button>();
        
        if(isStory)
            transform.Find("Card/Button_Next").gameObject.SetActive(true);
    }
   
    public void OnClose()
    {
        //Recommendation
        addToVisitedContent();
        DestroyImmediate(gameObject);
    }
    public void OnMore()
    {
        //Recommendation ( addToVisitedContent() is called in UIManager/CloseContentPanel() )
        uiManager.GetComponent<UIManager>().ShowContentPanel(this); 
        gameObject.SetActive(false);
    }
    public void OnNext()
    {
        //Recommendation
        addToVisitedContent();
        additionalRecommendation();

        mapManager.GetComponent<MapManager>().RemoveCurrentPOI();
        DestroyImmediate(gameObject);
    }

    public void OnLike()
    {
        bool likedBefore = false;
        for (int i = 0; i < anchor.likes.Count; i++)
        {
            if (anchor.likes[i].user.id == AccountInfo.Instance.user.id)
            {
                likedBefore = true;
                break;
            }
        }

        if (likedBefore)
        {
            //Recommendation
            userLiked = 1;

            Texture2D likedTex = Resources.Load<Texture2D>("UI/Icon/Authoring/like - selected");
            likeImage.sprite = ResourceLoader.Instance.likedSprite;
            likeButton.interactable = false;
        }
        else
        {
            //Recommendation
            userLiked = 10;

            WWWForm formData = new WWWForm();
            formData.AddField("anchorid", anchor.id.ToString());

            string url = string.Format("/arscene/like");
            NetworkManager.Instance.Post(url, formData, LikeSuccess, FailHandler);
        }

    }
 
    private void LikeSuccess(Result result)
    {
        Debug.Log("LikeSuccess");
        Texture2D likedTex = Resources.Load<Texture2D>("UI/Icon/Authoring/like - selected");
        likeImage.sprite = ResourceLoader.Instance.likedSprite;
        likeButton.interactable = false;
    }

    //Recommendation -- Maryam
    public void addToVisitedContent()
    {
        double userLatitude = GlobalARCameraInfo.Instance.latitude;
        double userLongitude = GlobalARCameraInfo.Instance.longitude;

        DateTime end = DateTime.Now;
        double duration = end.Subtract(start).TotalSeconds;
        VisitedContent visitedContent = new VisitedContent();

        visitedContent.anchor = anchor;
        visitedContent.visitedDateTime = start;
        visitedContent.userVisitedTime = duration;
        visitedContent.liked = userLiked;
        visitedContent.userLat = userLatitude;
        visitedContent.userLon = userLongitude;
        visitedContent.user_id = PlayerPrefs.GetString("email");

        visitedContent.setContentTime();

        GameObject recom = GameObject.Find("Recommendation");
        recom.GetComponent<Recommendation>().addToVisitedList(visitedContent);
    }

    //Recommendation -- Maryam
    private void additionalRecommendation()
    {
        GameObject recom = GameObject.Find("Recommendation");
        recom.GetComponent<Recommendation>().additionalRecommendation();
    }

}


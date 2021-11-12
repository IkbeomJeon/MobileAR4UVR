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
    public Text likeNumber;
    public Image likeImage;

    public Button likeButton;
    private int userLiked = 1;
    public DateTime start;


    public virtual void Init(Anchor anchor,bool isStory, int index_story)
    {
        start = DateTime.Now;

        base.Init(anchor, "Card/");
        likeNumber = transform.Find("Card/LikeNumber").gameObject.GetComponent<Text>();
        likeImage = transform.Find("Card/Like").GetComponent<Image>();
        
        likeButton = transform.Find("Card/Like").GetComponent<Button>();
        likeButton.onClick.AddListener(delegate { OnLike(); });
        
        Button closeButton = transform.Find("Card/CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(delegate { OnClose(); });

        Button detailButton = transform.Find("Card/Button_Indetail").GetComponent<Button>();
        detailButton.onClick.AddListener(delegate { OnDetail(); });


        if (isStory)
        {
            transform.Find("Card/Button_Next").gameObject.SetActive(true);
            Button nextButton = transform.Find("Card/Button_Next").GetComponent<Button>();
            nextButton.onClick.AddListener(delegate { OnNext(); });
        }
        else
            transform.Find("Card/Button_Next").gameObject.SetActive(false);

    }


    public virtual void OnClose()
    {
        addToVisitedContent();
        DestroyImmediate(gameObject);
    }
    public virtual void OnDetail()
    {
        panelManager.GetComponent<PanelManager>().ShowContentPanel(this); 
        //gameObject.SetActive(false);
    }
    public virtual void OnNext()
    {
        mapManager.GetComponent<MapManager>().RemoveCurrentPOI();
        DestroyImmediate(gameObject);
    }

    public virtual void OnLike()
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
            userLiked = 1;

            Texture2D likedTex = Resources.Load<Texture2D>("UI/Icon/Authoring/like - selected");
            likeImage.sprite = ResourceLoader.Instance.likedSprite;
            likeButton.interactable = false;
        }
        else
        {
            userLiked = 10;
 
            WWWForm formData = new WWWForm();
            formData.AddField("anchorid", anchor.id.ToString());

            string url = string.Format("/arscene/like");
            NetworkManager.Instance.Post(url, formData, LikeSuccess, FailHandler);
        }

    }
 
    public void LikeSuccess(Result result)
    {
        Debug.Log("LikeSuccess");
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
        Debug.Log(visitedContent.user_id);
            
        visitedContent.setContentTime();

        GameObject recom = GameObject.Find("Recommendation");
        recom.GetComponent<Recommendation>().addToVisitedList(visitedContent);
    }

}


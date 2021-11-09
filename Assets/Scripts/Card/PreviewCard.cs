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

    public virtual void Init(Anchor anchor,bool isStory, int index_story)
    {

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
        }
            
    }
   
    public virtual void OnClose()
    {
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
            Texture2D likedTex = Resources.Load<Texture2D>("UI/Icon/Authoring/like - selected");
            likeImage.sprite = ResourceLoader.Instance.likedSprite;
            likeButton.interactable = false;
        }
        else
        {
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

   

}


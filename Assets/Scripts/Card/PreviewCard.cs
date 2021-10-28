using System.Collections;
using System.Collections.Generic;
using KCTM.Network;
using KCTM.Network.Data;
using Mapbox.Json;
using UnityEngine;
using UnityEngine.UI;

public class PreviewCard : BaseCard
{
    private int userLiked = 0;
    public Text likeNumber;
    public Image likeImage;
    public Button likeButton;
    //public ResourceLoader rl;

    public virtual void Init(Anchor anchor, bool isSpacetelling)
    {
        base.Init(anchor, "Card/");
        likeNumber = transform.Find("Card/LikeNumber").gameObject.GetComponent<Text>();
        likeImage = transform.Find("Card/Like").GetComponent<Image>();
        likeButton = transform.Find("Card/Like").GetComponent<Button>();
        
        if(isSpacetelling)
            transform.Find("Card/Button_Next").gameObject.SetActive(true);

        //rl = GameObject.Find("ResourceLoader").GetComponent<ResourceLoader>();
    }

   
    public void OnClose()
    {
        DestroyImmediate(gameObject);
    }
    public void OnMore()
    {
        uiManager.GetComponent<UIManager>().ShowContentPanel(anchor);
        OnClose();
    }
    public void OnNext()
    {
        mapManager.GetComponent<MapManager>().RemoveCurrentPOI();
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
            userLiked = 0;

            Texture2D likedTex = Resources.Load<Texture2D>("UI/Icon/Authoring/like - selected");
            likeImage.sprite = ResourceLoader.Instance.likedSprite;
            likeButton.interactable = false;
        }
        else
        {
            //Recommendation
            userLiked = 1;

            WWWForm formData = new WWWForm();
            formData.AddField("anchorid", anchor.id.ToString());

            string url = string.Format("/arscene/like");
            NetworkManager.Instance.Post(url, formData, LikeSuccess, FailHandler);
        }

    }
    public void FailHandler(Result result)
    {
        if (result != null)
        {
            Debug.Log("result: " + result.msg);
        }
    }
    private void LikeSuccess(Result result)
    {
        Debug.Log("LikeSuccess");
        Texture2D likedTex = Resources.Load<Texture2D>("UI/Icon/Authoring/like - selected");
        likeImage.sprite = ResourceLoader.Instance.likedSprite;
        likeButton.interactable = false;
    }


}


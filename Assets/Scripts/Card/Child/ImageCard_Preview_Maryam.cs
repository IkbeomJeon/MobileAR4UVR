using KCTM.Network;
using KCTM.Network.Data;
using KCTM.Recommendation;
using Mapbox.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCard_Preview_Maryam : PreviewCard
{
    public DateTime start;

    private int userLiked = 1;
    public Image image;

    // Start is called before the first frame update
    public override void Init(Anchor anchor, bool isStory, int index_story)
    {
        base.Init(anchor, isStory, index_story);
        
        //Recommendation
        start = DateTime.Now;

        image = transform.Find("Card/ImageObject/Image").GetComponent<Image>();

        string url = string.Format("/arscene?id={0}&secret=false", anchor.id);
        NetworkManager.Instance.Get(url, GetARScene, FailHandler);
        //Debug.Log(anchor.contentinfos[0].content.updatedtime);
    }

   
    /*
     * Function: GetARScene
     *
     * Details:
     * - Callback function for getting arscene.
     * - Once received arscene, save and set preview card.
     */
    private void GetARScene(Result result)
    {
        var anchor = JsonConvert.DeserializeObject<Anchor>(result.result.ToString());
        base.anchor = anchor;

        likeNumber.text = string.Format("¡¡æ∆ø‰ : {0}", anchor.likes.Count.ToString());
        GetTexture(anchor.contentinfos[0].content.uri);
    }

    /*
    * Function: SaveHistory
    *
    * Details:
    * - Save clicked arscene id.
    * - Not used since server keeps track of requests.
    */
    private void SaveHistory()
    {
        List<History> history;
        if (PlayerPrefs.HasKey("history"))
        {
            history = JsonConvert.DeserializeObject<List<History>>(PlayerPrefs.GetString("history"));
        }
        else
        {
            history = new List<History>();
        }
        history.Add(new History(anchor.id, anchor.contenttype, anchor.contentdepth));

        PlayerPrefs.SetString("history", JsonConvert.SerializeObject(history));
        PlayerPrefs.Save();
    }
    // Update is called once per frame
    void GetTexture(string uri)
    {
        NetworkManager.Instance.GetTexture(uri, SuccessDownloadTexture, FailTextCallback);
    }
    public void SuccessDownloadTexture(Texture2D texture)
    {
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        float scale = rect.width / rect.height;
        Sprite sprite = Sprite.Create(texture, rect, Vector2.one * 0.5f);

        if(image!=null)
        {
            image.GetComponent<AspectRatioFitter>().aspectRatio = scale;
            image.sprite = sprite;
        }
    }


    public override void OnClose()
    {
        //Recommendation
        addToVisitedContent();
        DestroyImmediate(gameObject);
    }
    public override void OnDetail()
    {
        //Recommendation ( addToVisitedContent() is called in PanelManager/CloseContentPanel() )
        panelManager.GetComponent<PanelManager>().ShowContentPanel(this);
        gameObject.SetActive(false);
    }
    public override void OnNext()
    {
        //Recommendation
        addToVisitedContent();
        additionalRecommendation();

        mapManager.GetComponent<MapManager>().RemoveCurrentPOI();
        DestroyImmediate(gameObject);
    }


    public override void OnLike()
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

    //Recommendation -- Maryam
    private void additionalRecommendation()
    {
        GameObject recom = GameObject.Find("Recommendation");
        recom.GetComponent<Recommendation>().additionalRecommendation();
    }
}

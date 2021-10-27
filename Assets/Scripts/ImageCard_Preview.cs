using KCTM.Network;
using KCTM.Network.Data;
using Mapbox.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCard_Preview : PreviewCard
{
    public Image image;
    // Start is called before the first frame update
    public override void Init(Anchor anchor, bool isSP)
    {
        base.Init(anchor, isSP);

        image = transform.Find("Card/ImageObject/Image").GetComponent<Image>();

        string url = string.Format("/arscene?id={0}&secret=false", anchor.id);
        NetworkManager.Instance.Get(url, GetARScene, FailHandler);
        SaveHistory();

     

        //Debug.Log(anchor.contentinfos[0].content.updatedtime);
    }

    /*
     * Function: SaveHistory
     *
     * Details:
     * - Save clicked arscene id.
     * - Not used since server keeps track of requests.
     */
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

    private void FailTextCallback(string result)
    {
        //Debug.Log("error in: " + arScene.id);
        Debug.LogError(result);
    }
    
}

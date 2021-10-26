using KCTM.Network;
using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCard_Small : BaseCard
{
    public Text description;
    public Text indexText;
    public Image image;
    public Text author;
    public Text upload;
    public Transform tagParent;

    public void Init(Anchor anchor,int number)
    {
        base.Init(anchor, "Card/");

        image = transform.Find("Card/ImageObject/Image").GetComponent<Image>();
        transform.Find("Index/Text").GetComponent<Text>().text = number.ToString();

        author = transform.Find("Card/BottomInfo/AuthorText").GetComponent<Text>();
        upload = transform.Find("Card/BottomInfo/UploadText").GetComponent<Text>();
        tagParent = transform.Find("Card/Tags/Scroll View/Viewport/TagContent").transform;
        description = transform.Find("Card/TextObject/ScrollArea/Description").GetComponent<Text>();
        description.text = anchor.description;

        author.text = anchor.contentinfos[0].content.user.name;
        upload.text = Util.GetHumanTimeFormatFromMilliseconds(anchor.contentinfos[0].content.updatedtime);

        GetTexture(anchor.contentinfos[0].content.uri);

        for (int i = 0; i < anchor.tags.Count; i++)
        {
            GameObject tag = Instantiate(ResourceLoader.Instance.tagObj, tagParent);
            tag.transform.GetChild(0).GetComponent<Text>().text = anchor.tags[i].tag;
        }
    }
    void GetTexture(string uri)
    {
        NetworkManager.Instance.GetTexture(uri, SuccessDownloadTexture, FailTextCallback);
    }
    public void SuccessDownloadTexture(Texture2D texture)
    {
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        float scale = rect.width / rect.height;
        Sprite sprite = Sprite.Create(texture, rect, Vector2.one * 0.5f);

        image.GetComponent<AspectRatioFitter>().aspectRatio = scale;
        image.sprite = sprite;
    }

    private void FailTextCallback(string result)
    {
        //Debug.Log("error in: " + arScene.id);
        Debug.LogError(result);
    }
}

using KCTM.Network;
using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCard_Small : NormalCard
{
    public Image image;
    public Text indexText;
   

    public void Init(Anchor anchor,int number)
    {
        base.Init(anchor, "Card/");

        image = transform.Find("Card/ImageObject/Image").GetComponent<Image>();
        indexText = transform.Find("Index/Text").GetComponent<Text>();
        indexText.text = number.ToString();
    
    }

    public override void DownloadContent()
    {
        base.DownloadContent();
        NetworkManager.Instance.GetTexture(anchor.contentinfos[0].content.uri, SuccessDownloadTexture, FailTextCallback);
    }
    public void SuccessDownloadTexture(Texture2D texture)
    {
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        float scale = rect.width / rect.height;
        Sprite sprite = Sprite.Create(texture, rect, Vector2.one * 0.5f);

        image.GetComponent<AspectRatioFitter>().aspectRatio = scale;
        image.sprite = sprite;
    }

   
}

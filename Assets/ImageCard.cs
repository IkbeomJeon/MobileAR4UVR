using KCTM.Network;
using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCard : BaseCard
{

    public Image image;
    public override void Init(Anchor anchor)
    {
        base.Init(anchor);

        image = transform.Find("Card/ImageObject/Image").GetComponent<Image>();
        GetTexture(anchor.contentinfos[0].content.uri);
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

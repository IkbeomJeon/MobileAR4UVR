using KCTM.Network;
using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCard : NormalCard
{
    public Image image;
    public GameObject fullScreenButton;
    
    public void Init(Anchor anchor, string parentName = "", bool showGoButton=false)
    {
        base.Init(anchor, parentName);

        Transform goButton = transform.Find(parentName+"BottomInfo/GoButton");
        if (goButton != null)
        {
            goButton.gameObject.SetActive(false);

            if (showGoButton)
                goButton.gameObject.SetActive(true);
        }

        image = transform.Find(parentName+"ImageObject/Image").GetComponent<Image>();
        fullScreenButton = image.transform.Find(parentName + "Button").gameObject;

        Debug.Log(anchor.contentinfos[0].content.uri);
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

        if(image!=null)
        {
            image.GetComponent<AspectRatioFitter>().aspectRatio = scale;
            image.sprite = sprite;

            fullScreenButton.SetActive(true);
                fullScreenButton.GetComponent<Button>().onClick.AddListener(delegate {
                    EnterFullScreen(image, scale);
                });
        }
  
    }

    public void EnterFullScreen(Image image, float scale)
    {
        panelManager.GetComponent<PanelManager>().ShowFullScreenPanel(image, scale);
    }
}

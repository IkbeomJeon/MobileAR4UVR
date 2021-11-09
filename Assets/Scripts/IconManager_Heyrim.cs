using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARRC_DigitalTwin_Generator;
using UnityEngine.UI;
using System;
using KCTM.Recommendation;
using Newtonsoft.Json;
using TMPro;

public class IconManager_Heyrim : IconManager
{
    public new void Init(Anchor anchor, string mediaType, Transform cameraTransform, bool spacetelling = false, int index_poi = 0)
    {
        base.Init(anchor, mediaType, cameraTransform, spacetelling, index_poi);

        Sprite sprite;
        

        switch (mediaType)
        {
            case "IMAGE":
                sprite = ResourceLoader.Instance.sprite_education;
              
                break;

            case "VIDEO":
            default:
                sprite = ResourceLoader.Instance.sprite_admission;
                break;
       }
        buttonImage.sprite = sprite;

        if (!string.IsNullOrEmpty(mediaType)) //if it's null, it is poi anchor.
        {
            var tag_image = transform.Find("Canvas/Summary/Tag_Image").GetComponent<Image>();
            tag_image.color = getTagColor(mediaType);

            var tag_text = transform.Find("Canvas/Summary/Tag_Image/Tag").GetComponent<TextMeshProUGUI>();
            tag_text.text = mediaType;

        }

    }


    private Color32 getTagColor(string type)
    {
        Color32 color = new Color32(255, 255, 225, 100);
        switch (type)
        {
            case "VIDEO":
                color = new Color32(64, 224, 208, 100);
                break;

            case "IMAGE":
            default:
                color = new Color32(245, 247, 103, 100);
                break;
        }

        return color;
    }
}


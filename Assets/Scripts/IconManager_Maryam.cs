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

public class IconManager_Maryam : IconManager
{
    // recommendation -- Maryam
    bool recommended;
    private int userLiked = 1;
   
    public void Init(Anchor anchor, string type, Transform cameraTransform, float default_height = 0, bool spacetelling = false, int index_poi = 0, bool isRecommned=false)
    {
        base.Init(anchor, type, cameraTransform, default_height, spacetelling, index_poi);

        Sprite sprite;

        if(isRecommned)
            sprite = ResourceLoader.Instance.sprite_recommendation;

        else
        {
            switch (type)
            {
                case "Admission":
                    sprite = ResourceLoader.Instance.sprite_admission;
                    break;
                case "Research":
                    sprite = ResourceLoader.Instance.sprite_research;
                    break;
                case "Campus life":
                    sprite = ResourceLoader.Instance.sprite_campusLife;
                    break;
                case "News":
                    sprite = ResourceLoader.Instance.sprite_news;
                    break;
                case "Education":
                case " Education":
                    sprite = ResourceLoader.Instance.sprite_education;
                    break;

                default: //about
                    sprite = ResourceLoader.Instance.sprite_about;
                    break;
            }
        }
       
        buttonImage.sprite = sprite;

        if (!string.IsNullOrEmpty(type)) //if it's null, it is poi anchor.
        {
            var tag_image = transform.Find("Canvas/Summary/Tag_Image").GetComponent<Image>();
            tag_image.color = getTagColor(type);

            var tag_text = transform.Find("Canvas/Summary/Tag_Image/Tag").GetComponent<TextMeshProUGUI>();
            tag_text.text = type;

        }
      
    }
    //Recommendation -- Maryam
    public void updateIcon()
    {
        recommended = true;
        //SetIcon(arScene.contentinfos[0].content.mediatype);
    }



    private Color32 getTagColor(string type)
    {
        Color32 color = new Color32(255, 255, 225, 100);
        switch (type)
        {
            case "Admission":
                color = new Color32(64, 224, 208, 100);
                break;
            case "Research":
                color = new Color32(95, 191, 249, 100);
                break;
            case "Campus life":
                color = new Color32(255, 105, 180, 100);
                break;
            case "News":
                color = new Color32(255, 95, 94, 100);
                break;
            case "Education":
            case " Education":
                color = new Color32(255, 165, 0, 100);
                break;
            case "Recommenation":
                color = new Color32(245, 247, 103, 100);
                break;
            default: //About
                color = new Color32(253, 192, 138, 100);
                break;

        }

        return color;
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

}

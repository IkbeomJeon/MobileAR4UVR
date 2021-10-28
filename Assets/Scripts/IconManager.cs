using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARRC_DigitalTwin_Generator;
using UnityEngine.UI;
using System;
using KCTM.Recommendation;
using Newtonsoft.Json;

public class IconManager : MonoBehaviour
{
    public Anchor anchor;
    Transform cameraTransform;


    public bool isSpacetelling;
    public int index_spacetelling;


    bool recommended;
    // recommendation -- Maryam
    private int userLiked = 1;
    


    private void OnEnable()
    {
        StartCoroutine(UpdateLookat());
    }
    private void OnDisable()
    {
        StopCoroutine(UpdateLookat());
    }

    IEnumerator UpdateLookat()
    {
        while (true)
        {
            transform.LookAt(cameraTransform);
            transform.Rotate(Vector3.up, 180f);

            yield return new WaitForSeconds(0.3f);
        }
    }

    public void Init(Anchor anchor, string category, Transform cameraTransform, float default_height = 0, bool spacetelling = false, int index_poi = 0)
    {
        ///// set position
        double lon = anchor.point.longitude;
        double lat = anchor.point.latitude;

        //convert "lon,lat" to "x, y" in unity.
        Vector3 pos = TerrainUtils.LatLonToWorldWithElevation(TerrainContainer.Instance, lat, lon);
        transform.localPosition = new Vector3(pos.x, pos.y + default_height, pos.z);

        this.cameraTransform = cameraTransform;
        this.anchor = anchor;
        
        //get components
        var title_text = transform.Find("Canvas/Summary/Title").GetComponent<TMPro.TextMeshProUGUI>();
        title_text.text = anchor.title;

        if(!string.IsNullOrEmpty(category)) //if it's null, it is poi anchor.
        {
            var tag_image = transform.Find("Canvas/Summary/Tag_Image").GetComponent<Image>();
            tag_image.color = getTagColor(category);

            var tag_text = transform.Find("Canvas/Summary/Tag_Image/Tag").GetComponent<TMPro.TextMeshProUGUI>();
            tag_text.text = category;
        }

        var desc_text = transform.Find("Canvas/Summary/Description").GetComponent<TMPro.TextMeshProUGUI>();
        desc_text.text = anchor.description;

        if (spacetelling)//poi icon.
        {
            isSpacetelling = true;
            index_spacetelling = index_poi;

            var index_text = transform.Find("Canvas/Number/Text").GetComponent<TMPro.TextMeshProUGUI>();
            index_text.text = index_poi.ToString();
        }

        if(category == "Recommenation")
        {

        }
    }
    //Recommendation -- Maryam
    public void updateIcon()
    {
        recommended = true;
        //SetIcon(arScene.contentinfos[0].content.mediatype);
    }

   

    private Color32 getTagColor(string tag)
    {
        Color32 color = new Color32(255, 255, 225, 100);
        switch (tag)
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

    public void ShowPreviewCard()
    {
        SaveHistory();

        switch(anchor.contentinfos[0].content.mediatype)
        {
            case "IMAGE":
                GameObject previewCard = Instantiate(ResourceLoader.Instance.card_Image_preview);
                previewCard.GetComponent<ImageCard_Preview>().Init(anchor, isSpacetelling, index_spacetelling);
                break;
        }
    }
}

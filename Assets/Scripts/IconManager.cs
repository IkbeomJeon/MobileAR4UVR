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
    public Transform buttonTransform;
    public Image buttonImage;

    public bool isSpacetelling;
    public int index_spacetelling;

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

    public void Init(Anchor anchor, string category, Transform cameraTransform, bool spacetelling = false, int index_poi = 0)
    {
        buttonImage = transform.Find("Canvas/Button").GetComponent<Image>();
        ///// set position
        double lon = anchor.point.longitude;
        double lat = anchor.point.latitude;

        //convert "lon,lat" to "x, y" in unity.
        Vector3 pos = TerrainUtils.LatLonToWorldWithElevation(TerrainContainer.Instance, lat, lon);
        transform.localPosition = new Vector3(pos.x, pos.y, pos.z);
        float size = ConfigurationManager.Instance.size_anchors;
        transform.localScale = new Vector3(size, size, size);
        this.cameraTransform = cameraTransform;
        this.anchor = anchor;

        //get components

        buttonTransform = transform.Find("Canvas/Button").transform;
        var title_text = transform.Find("Canvas/Summary/Title").GetComponent<TMPro.TextMeshProUGUI>();
        title_text.text = anchor.title;

        var desc_text = transform.Find("Canvas/Summary/Description").GetComponent<TMPro.TextMeshProUGUI>();
        desc_text.text = anchor.description;

        if (spacetelling)//poi icon.
        {
            isSpacetelling = true;
            index_spacetelling = index_poi;

            var index_text = transform.Find("Canvas/Number/Text").GetComponent<TMPro.TextMeshProUGUI>();
            index_text.text = index_poi.ToString();
        }

        transform.Find("Canvas/Button").GetComponent<Button>().onClick.AddListener(delegate { OnButtonClick(); });

    }

    public void OnButtonClick()
    {
        Transform uiParent = GameObject.Find("UI").transform;

        GameObject previewCard;
        switch (anchor.contentinfos[0].content.mediatype)
        {
            case "IMAGE":
                previewCard = Instantiate(ResourceLoader.Instance.card_Image_preview, uiParent);
                previewCard.AddComponent<ImageCard_Preview>().Init(anchor, isSpacetelling, index_spacetelling);
                break;

            case "VIDEO":
                previewCard = Instantiate(ResourceLoader.Instance.card_Video_preview, uiParent);
                previewCard.AddComponent<VideoCard_Preview>().Init(anchor, isSpacetelling, index_spacetelling);
                break;
        }
    }
}

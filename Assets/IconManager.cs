using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARRC_DigitalTwin_Generator;
using UnityEngine.UI;

public class IconManager : MonoBehaviour
{
    public Anchor anchor;
    Transform cameraTransform;
    public GameObject[] icon_Prefab = new GameObject[6];


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("UpdateLookat");
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    IEnumerator UpdateLookat()
    {
        while (true)
        {

            transform.LookAt(cameraTransform);
            transform.Rotate(Vector3.up, 180f);

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void Init(Anchor anchor, string title, string tag, string decription, Transform cameraTransform)
    {

        ///// set position
        double lon = anchor.point.longitude;
        double lat = anchor.point.latitude;

            //convert "lon,lat" to "x, y" in unity.
        Vector3 pos = TerrainUtils.LatLonToWorldWithElevation(TerrainContainer.Instance, lat, lon);
        transform.localPosition = new Vector3(pos.x, pos.y, pos.z);


        this.cameraTransform = cameraTransform;
        this.anchor = anchor;

        
        //get components
        var title_text = transform.Find("Canvas/Summary/Title").GetComponent<TMPro.TextMeshProUGUI>();
        title_text.text = title;

        var tag_image= transform.Find("Canvas/Summary/Tag_Image").GetComponent<Image>();
        tag_image.color = getTagColor(tag);

        var tag_text = transform.Find("Canvas/Summary/Tag_Image/Tag").GetComponent<TMPro.TextMeshProUGUI>();
        tag_text.text = tag;

        var desc_text = transform.Find("Canvas/Summary/Description").GetComponent<TMPro.TextMeshProUGUI>();
        desc_text.text = decription;
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
            default: //About
                color = new Color32(253, 192, 138, 100);
                break;

        }

        return color;
    }
}

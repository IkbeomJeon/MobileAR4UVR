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

    public Image thubnailImage;
    public Text thubnameText_title;
    public Text thubnameText_tag;
    public Text thubnameText_description;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cameraTransform);
        transform.Rotate(Vector3.up, 180f);
    }

    public void Init(Anchor anchor, Transform cameraTransform)
    {

        thubnailImage = transform.Find("Canvas/Button").GetComponent<Image>();
        thubnameText_title = transform.Find("Canvas/Title/Text").GetComponent<Text>();

        ///// set position
        double lon = anchor.point.longitude;
        double lat = anchor.point.latitude;

            //convert "lon,lat" to "x, y" in unity.
        Vector3 pos = TerrainUtils.LatLonToWorldWithElevation(TerrainContainer.Instance, lat, lon);
        transform.localPosition = new Vector3(pos.x, pos.y, pos.z);


        this.cameraTransform = cameraTransform;
        this.anchor = anchor;

        ///// set icon thumbnail.
        //SetThumbnail(anchor.contentinfos[0].content.mediatype);

        
    }

    private void SetThumbnail(string type)
    {
        Texture2D texture;
        Texture2D numTexture = Resources.Load<Texture2D>("UI/Icon/1_content icon/heritage num@3x");

        string icon_location = getCampusTourIconImagePath(anchor);

        if (icon_location != "")
        {
            texture = Resources.Load<Texture2D>(icon_location);
            //thubnailImage.color = getTagColor(icon_location);
            thubnailImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100f);
            //thumbnailObject.SetActive(true);
        }


        //switch (type)
        //{
        //    case "IMAGE":
                
        //        GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100f);
        //        break;

        //    case "TEXT":
        //        texture = Resources.Load<Texture2D>("UI/Icon/1_content icon/text icon@3x");
               

        //        break;
        //    case "AUDIO":
        //        texture = Resources.Load<Texture2D>("UI/Icon/1_content icon/audio icon@3x");
        //        icon.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100f);
        //        break;

        //    case "VIDEO":
        //        texture = Resources.Load<Texture2D>("UI/Icon/1_content icon/video icon@3x");
        //        icon.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100f);
        //        if (navigationResult)
        //        {
        //            numTexture = Resources.Load<Texture2D>("UI/Icon/1_content icon/video num@3x");
        //        }
        //        break;
        //    default:
        //        texture = Resources.Load<Texture2D>("UI/Icon/1_content icon/spacetelling@3x");
        //        icon.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100f);
        //        if (navigationResult)
        //        {
        //            numTexture = Resources.Load<Texture2D>("UI/Icon/1_content icon/spacetelling num@3x");
        //        }
        //        break;
        //}

        //if (navigationResult)
        //{
        //    texture = Resources.Load<Texture2D>("UI/Icon/1_content icon/spacetelling@3x");
        //    icon.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100f);
        //    numTexture = Resources.Load<Texture2D>("UI/Icon/1_content icon/spacetelling num@3x");
        //}

        //if (recommended)
        //{
        //    texture = Resources.Load<Texture2D>("UI/Icon/1_content icon/image_recommend@3x");
        //    thumbnailObject.GetComponent<Image>().color = getTagColor("UI/Icon/1_content icon/image_recommend@3x");
        //    //thumbnailObject.GetComponent<Renderer>().enabled = true;
        //    icon.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100f);
        //}

        //if (navigationResult)
        //{
        //    if (currentIndex != -1)
        //    {
        //        if (!storyStart)
        //        {
        //            number.SetActive(true);
        //            number.GetComponent<Image>().sprite = Sprite.Create(numTexture, new Rect(0, 0, numTexture.width, numTexture.height), Vector2.one * 0.5f, 100f);
        //            var text = numberText.GetComponent<TMPro.TextMeshPro>();
        //            text.text = currentIndex.ToString();
        //        }
        //    }
        //    else
        //    {
        //        number.SetActive(false);
        //    }

        //}
        //if (arScene.title != null)
        //{

        //    string shortenTitle;
        //    if (arScene.title.Length >= 20)
        //        shortenTitle = arScene.title.Substring(0, 17) + "...";
        //    else
        //        shortenTitle = arScene.title;

        //    titleBubbleText.text = shortenTitle;

        //    //titleBubbleText.text = arScene.title;

        //    String descText = arScene.description.Replace("\n", " ");
        //    if (descText.Length >= 35)
        //    {

        //        descriptionText.text = descText.Substring(0, 35) + " ...";
        //    }
        //    else if (descText.Length > 0 && descText.Length < 35)
        //    {
        //        descriptionText.text = descText;
        //    }

        //    /*
        //    if(type == "IMAGE")
        //    {
        //        String uri = arScene.contentinfos[0].content.uri;
        //        PostRequest.texture = null;
        //        PostRequest.setImage(uri);
        //        if (PostRequest.texture != null)
        //       {
        //         thumbnailObject.GetComponent<Image>().overrideSprite = Sprite.Create(PostRequest.texture, new Rect(0, 0, PostRequest.texture.width, PostRequest.texture.height), new Vector2(0, 0));
        //      }
        //    }
        //    */

        //}
        //else
        //    titleBubble.SetActive(false);

    }

    private string getCampusTourIconImagePath(Anchor anchor)
    {
        string icon_image_path = "";

        for (int a = 0; a < anchor.tags.Count; a++)
        {
            if (anchor.tags[a].category == "InterestTag")
            {
                //set Tag text
                //thubnameText_title.text = anchor.tags[a].tag;

                switch (anchor.tags[a].tag)
                {
                    case "Admission":
                        icon_image_path = "UI/Icon/1_content icon/image cyan icon@3x";
                        break;
                    case "Research":
                        icon_image_path = "UI/Icon/1_content icon/image light blue icon@3x";
                        break;
                    case "Campus life":
                        icon_image_path = "UI/Icon/1_content icon/image light pink icon@3x";
                        break;
                    case "News":
                        icon_image_path = "UI/Icon/1_content icon/image light red icon@3x";
                        break;
                    case "Education":
                    case " Education":
                        icon_image_path = "UI/Icon/1_content icon/image orang icon@3x";
                        break;
                    case "About":
                        icon_image_path = "UI/Icon/1_content icon/image peach icon@3x";
                        break;
                }
            }
        }

        return icon_image_path;
    }
    private Color32 getTagColor(string arSceneIcon)
    {

        string file_name = arSceneIcon.Split(char.Parse("/"))[3];
        Color32 color = new Color32(255, 255, 225, 100);
        switch (file_name)
        {
            case "image cyan icon@3x":
                color = new Color32(64, 224, 208, 100);
                break;
            case "image light blue icon@3x":
                color = new Color32(95, 191, 249, 100);
                break;
            case "image light pink icon@3x":
                color = new Color32(255, 105, 180, 100);
                break;
            case "image light red icon@3x":
                color = new Color32(255, 95, 94, 100);
                break;
            case "image orang icon@3x":
                color = new Color32(255, 165, 0, 100);
                break;
            case "image peach icon@3x":
                color = new Color32(253, 192, 138, 100);
                break;
            case "image purple icon@3x":
                color = new Color32(218, 112, 214, 100);
                break;
            case "image_recommend@3x":
                color = new Color32(245, 247, 103, 100);
                break;

        }

        return color;
    }
}

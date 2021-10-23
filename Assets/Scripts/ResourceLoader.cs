using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    //public GameObject card_Audio;
    //cards
    public GameObject card_Group_nav;
    public GameObject card_Image_nav;
    public GameObject card_Image_preview;
    public GameObject card_Image_small;
    public GameObject tagObj;
    //public GameObject card_Model;
    //public GameObject card_Text;
    //public GameObject card_Video;


    //icons
    public GameObject icon_about;
    public GameObject icon_admission;
    public GameObject icon_campusLife;
    public GameObject icon_education;
    public GameObject icon_news;
    public GameObject icon_research;

    //sprite
    public Sprite moreDown, moreUp;


    //small cards

    private void Awake()
    {
        //cards
        card_Group_nav = Resources.Load<GameObject>("Prefabs/Cards/Nav/GroupNavCard");

        card_Image_nav = Resources.Load<GameObject>("Prefabs/Cards/Nav/ImageNavCard");
        card_Image_preview = Resources.Load<GameObject>("Prefabs/Cards/Preview/ImagePreviewCard");
        card_Image_small = Resources.Load<GameObject>("Prefabs/Cards/Small/ImageCard_small");
        tagObj = Resources.Load<GameObject>("Prefabs/Cards/TagObj");

        //icons
        icon_about = Resources.Load<GameObject>("Prefabs/Icons/Icon_About");
        icon_admission = Resources.Load<GameObject>("Prefabs/Icons/Icon_Admission");
        icon_campusLife = Resources.Load<GameObject>("Prefabs/Icons/Icon_CampusLife");
        icon_education = Resources.Load<GameObject>("Prefabs/Icons/Icon_Education");
        icon_news = Resources.Load<GameObject>("Prefabs/Icons/Icon_News");
        icon_research = Resources.Load<GameObject>("Prefabs/Icons/Icon_Research");

        //textures
        Texture2D mUp = Resources.Load<Texture2D>("Textures/more - reversedarrow@3x");
        Texture2D mDown = Resources.Load<Texture2D>("Textures/more - arrow@3x");

        moreUp = Sprite.Create(mUp, new Rect(0, 0, mUp.width, mUp.height), Vector2.one * 0.5f, 1000f);
        moreDown = Sprite.Create(mDown, new Rect(0, 0, mDown.width, mDown.height), Vector2.one * 0.5f, 1000f);
    }
   

    private static ResourceLoader instance;

    public static ResourceLoader Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = GameObject.Find("ResourceLoader");
                if (obj == null)
                {
                    obj = new GameObject("ResourceLoader");
                    instance = obj.AddComponent<ResourceLoader>();
                }
                else
                {
                    instance = obj.GetComponent<ResourceLoader>();
                }
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }

}

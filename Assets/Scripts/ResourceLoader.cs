using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    //public GameObject card_Audio;
    //cards
    
    public GameObject card_Group;
    
    public GameObject card_Image;
    public GameObject card_Image_preview;
    public GameObject card_Image_small;

    public GameObject card_Video;
    public GameObject card_Video_preview;
    public GameObject card_Video_small;

    public GameObject tagObj;
    //public GameObject card_Model;
    //public GameObject card_Text;
    //public GameObject card_Video;


    //icons
    public GameObject icon;
    public GameObject icon_poi;
    //public GameObject icon_about;
    //public GameObject icon_admission;
    //public GameObject icon_campusLife;
    //public GameObject icon_education;
    //public GameObject icon_news;
    //public GameObject icon_research;

    //public GameObject icon_recommendation;

    public Sprite sprite_about;
    public Sprite sprite_admission;
    public Sprite sprite_campusLife;
    public Sprite sprite_education;
    public Sprite sprite_news;
    public Sprite sprite_research;
    public Sprite sprite_poi;
    public Sprite sprite_recommendation;



    //sprite
    public Sprite moreDown, moreUp;
    public Sprite likedSprite;
    //user pin
    public GameObject pinPoint;
    public GameObject poiPoint;


 
    private void Awake()
    {
        try
        {
            instance = this;

            //cards
            card_Group = Resources.Load("Prefabs/Cards/Normal/GroupCard") as GameObject;

            card_Image = Resources.Load("Prefabs/Cards/Normal/ImageCard") as GameObject;
            card_Image_preview = Resources.Load("Prefabs/Cards/Preview/ImagePreviewCard") as GameObject;
            card_Image_small = Resources.Load("Prefabs/Cards/Small/ImageCard_small") as GameObject;
            tagObj = Resources.Load("Prefabs/Cards/TagObj") as GameObject;

            //icons
            //icon_about = Resources.Load("Prefabs/Icons/Icon_About") as GameObject;
            //icon_admission = Resources.Load("Prefabs/Icons/Icon_Admission") as GameObject;
            //icon_campusLife = Resources.Load("Prefabs/Icons/Icon_CampusLife") as GameObject;
            //icon_education = Resources.Load("Prefabs/Icons/Icon_Education") as GameObject;
            //icon_news = Resources.Load("Prefabs/Icons/Icon_News") as GameObject;
            //icon_research = Resources.Load("Prefabs/Icons/Icon_Research") as GameObject;
            //icon_poi = Resources.Load("Prefabs/Icons/Icon_POI") as GameObject;
            //icon_recommendation = Resources.Load("Prefabs/Icons/icon_Recommendation") as GameObject;

            icon_poi = Resources.Load("Prefabs/Icon_POI") as GameObject;
            icon = Resources.Load("Prefabs/Icon") as GameObject;
            Texture2D tex_about = Resources.Load("Textures/Icons/image peach icon3x") as Texture2D;
            Texture2D tex_admission = Resources.Load("Textures/Icons/image cyan icon3x") as Texture2D;
            Texture2D tex_campusLife = Resources.Load("Textures/Icons/image light pink icon3x") as Texture2D;
            Texture2D tex_education = Resources.Load("Textures/Icons/image orang icon3x") as Texture2D;
            Texture2D tex_news = Resources.Load("Textures/Icons/image light red icon3x") as Texture2D;
            Texture2D tex_research = Resources.Load("Textures/Icons/image light blue icon3x") as Texture2D;
            Texture2D tex_poi = Resources.Load("Textures/Icons/spacetelling3x") as Texture2D;
            Texture2D tex_recommendation = Resources.Load("Textures/Icons/image_recommend3x") as Texture2D;


            sprite_about = Sprite.Create(tex_about, new Rect(0, 0, tex_about.width, tex_about.height), Vector2.one * 0.5f, 1000f);
            sprite_admission = Sprite.Create(tex_admission, new Rect(0, 0, tex_admission.width, tex_admission.height), Vector2.one * 0.5f, 1000f);
            sprite_campusLife = Sprite.Create(tex_campusLife, new Rect(0, 0, tex_campusLife.width, tex_campusLife.height), Vector2.one * 0.5f, 1000f);
            sprite_education = Sprite.Create(tex_education, new Rect(0, 0, tex_education.width, tex_education.height), Vector2.one * 0.5f, 1000f);
            sprite_news = Sprite.Create(tex_news, new Rect(0, 0, tex_news.width, tex_news.height), Vector2.one * 0.5f, 1000f);
            sprite_research = Sprite.Create(tex_research, new Rect(0, 0, tex_research.width, tex_research.height), Vector2.one * 0.5f, 1000f);
            sprite_poi = Sprite.Create(tex_poi, new Rect(0, 0, tex_poi.width, tex_poi.height), Vector2.one * 0.5f, 1000f);
            sprite_recommendation = Sprite.Create(tex_recommendation, new Rect(0, 0, tex_recommendation.width, tex_recommendation.height), Vector2.one * 0.5f, 1000f);
            
            //textures
            Texture2D mUp = Resources.Load<Texture2D>("Textures/more - reversedarrow@3x");
            Texture2D mDown = Resources.Load<Texture2D>("Textures/more - arrow@3x");

            moreUp = Sprite.Create(mUp, new Rect(0, 0, mUp.width, mUp.height), Vector2.one * 0.5f, 1000f);
            moreDown = Sprite.Create(mDown, new Rect(0, 0, mDown.width, mDown.height), Vector2.one * 0.5f, 1000f);

            Texture2D likedTex = Resources.Load<Texture2D>("Textures/like - selected");
            likedSprite = Sprite.Create(likedTex, new Rect(0, 0, likedTex.width, likedTex.height), Vector2.one * 0.5f, 100f);

            pinPoint = Resources.Load("Prefabs/PinPoint") as GameObject;
            poiPoint = Resources.Load("Prefabs/POIPoint") as GameObject;
        }
        catch(Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }
        
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

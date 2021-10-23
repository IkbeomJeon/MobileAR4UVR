using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    //icons
    public GameObject icon_about;
    public GameObject icon_admission;
    public GameObject icon_campusLife;
    public GameObject icon_education;
    public GameObject icon_news;
    public GameObject icon_research;

    //cards


    //small cards

    private void Awake()
    {
        icon_about = Resources.Load<GameObject>("Prefabs/Icons/Icon_About");
        icon_admission = Resources.Load<GameObject>("Prefabs/Icons/Icon_Admission");
        icon_campusLife = Resources.Load<GameObject>("Prefabs/Icons/Icon_CampusLife");
        icon_education = Resources.Load<GameObject>("Prefabs/Icons/Icon_Education");
        icon_news = Resources.Load<GameObject>("Prefabs/Icons/Icon_News");
        icon_research = Resources.Load<GameObject>("Prefabs/Icons/Icon_Research");
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

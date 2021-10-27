using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseCard : MonoBehaviour
{
    public Text title;

    public Anchor anchor;
    public GameObject loader;
    public GameObject uiManager;
    public GameObject mapManager;

    public GameObject cardContent;

    // Start is called before the first frame update

    public virtual void Init(Anchor anchor, string parentName="")
    {
        this.anchor = anchor;
        cardContent = GameObject.Find("CardContent");
        loader = GameObject.Find("Loader");
        uiManager = GameObject.Find("UI");
        mapManager = GameObject.Find("MapManager");

        title = transform.Find(parentName+"Title").GetComponent<Text>();
       
       

        title.text = anchor.title;
        
    }
}

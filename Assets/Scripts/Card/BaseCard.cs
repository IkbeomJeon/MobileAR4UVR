using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseCard : MonoBehaviour
{
    public Text title;
    public Text author;
    public Text upload;

    public Anchor anchor;
    public GameObject loader;
    public GameObject panelManager;
    public GameObject mapManager;

    public GameObject cardContent;

    // Start is called before the first frame update

    public virtual void Init(Anchor anchor, string parentName="")
    {
        this.anchor = anchor;
        cardContent = GameObject.Find("CardContent");
        loader = GameObject.Find("Loader");
        panelManager = GameObject.Find("Panels");
        mapManager = GameObject.Find("MapManager");

        title = transform.Find(parentName+"Title").GetComponent<Text>();
        title.text = anchor.title;
        
    }
    public virtual void FailHandler(Result result)
    {
        if (result != null)
        {
            Debug.Log("result: " + result.msg);
        }
    }
    public  virtual void FailTextCallback(string result)
    {
        //Debug.Log("error in: " + arScene.id);
        Debug.LogError(result);
    }
}

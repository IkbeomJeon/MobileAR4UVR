using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{


    //private GameObject CreateCard(Anchor anchor, int type, int index)
    //{
    //    GameObject card;
    //    switch (anchor.contentinfos[index].content.mediatype)
    //    {
    //        case "IMAGE":
    //            card = Instantiate(ResourceLoader.Instance.card_Image_nav, cardContentParent);
    //            //card.GetComponent<ImageCardNavPrefab>().arScene = anchor;
    //            //card.GetComponent<ImageCardNavPrefab>().indexContent = index;
    //            //card.GetComponent<ImageCardNavPrefab>().navigation = navigation;
    //            //card.GetComponent<ImageCardNavPrefab>().searchPanel = searchPanel;
    //            //var script = card.GetComponent<ImageCard>();
    //            //script.Init(anchor);
    //            break;
    //    }

    //    return card;
    //}
    
    private void Awake()
    {
        instance = this;
    }
    private static CardGenerator instance;

    public static CardGenerator Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = GameObject.Find("CardGenerator");
                if (obj == null)
                {
                    obj = new GameObject("CardGenerator");
                    instance = obj.AddComponent<CardGenerator>();
                }
                else
                {
                    instance = obj.GetComponent<CardGenerator>();
                }
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }
}

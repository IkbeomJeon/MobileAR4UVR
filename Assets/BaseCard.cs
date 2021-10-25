using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseCard : MonoBehaviour
{
    public Text title;
    public Text description;
    public Text author;
    public Text upload;

    public GameObject loader;
    public GameObject uiManager;
    public GameObject mapManager;

    public GameObject cardContent;
    public Transform tagParent;

    // Start is called before the first frame update

    public virtual void Init(Anchor anchor, string parentName="")
    {
        cardContent = GameObject.Find("CardContent");

        loader = GameObject.Find("Loader");
        uiManager = GameObject.Find("UI");
        mapManager = GameObject.Find("MapManager");

        title = transform.Find(parentName+"Title").GetComponent<Text>();
        description = transform.Find(parentName + "TextObject/ScrollArea/Description").GetComponent<Text>();
        author = transform.Find(parentName + "BottomInfo/AuthorText").GetComponent<Text>();
        upload = transform.Find(parentName + "BottomInfo/UploadText").GetComponent<Text>();
        tagParent = transform.Find(parentName+"Tags/Scroll View/Viewport/TagContent").transform;

        title.text = anchor.title;
        description.text = anchor.description;
        author.text = anchor.contentinfos[0].content.user.name;
        upload.text = Util.GetHumanTimeFormatFromMilliseconds(anchor.contentinfos[0].content.updatedtime);

        // Loading Tags
        for (int i = 0; i < anchor.tags.Count; i++)
        {
            GameObject tag = Instantiate(ResourceLoader.Instance.tagObj, tagParent);
            tag.transform.GetChild(0).GetComponent<Text>().text = anchor.tags[i].tag;
        }
    }
}

using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalCard : BaseCard
{
    public Transform tagParent;
    public Text description;

    // Start is called before the first frame update

    public virtual void DownloadContent() {}
    public override void Init(Anchor anchor, string parentName = "")
    {
        base.Init(anchor, parentName);
        
        author = transform.Find(parentName+"BottomInfo/AuthorText").GetComponent<Text>();
        upload = transform.Find(parentName+"BottomInfo/UploadText").GetComponent<Text>();
        tagParent = transform.Find(parentName + "Tags/Scroll View/Viewport/TagContent").transform;
        //description = transform.Find(parentName + "TextObject/ScrollArea/Description").GetComponent<Text>();
        description = transform.Find(parentName + "Description").GetComponent<Text>();
        description.text = anchor.description;

        author.text = anchor.contentinfos[0].content.user.name;
        upload.text = Util.GetHumanTimeFormatFromMilliseconds(anchor.contentinfos[0].content.updatedtime);

        // Loading Tags
        tagParent = transform.Find(parentName + "Tags/Scroll View/Viewport/TagContent").transform;

      
        for (int i = 0; i < anchor.tags.Count; i++)
        {
            GameObject tag = Instantiate(ResourceLoader.Instance.tagObj, tagParent);
            tag.transform.GetChild(0).GetComponent<Text>().text = anchor.tags[i].tag;
        }
    }
}

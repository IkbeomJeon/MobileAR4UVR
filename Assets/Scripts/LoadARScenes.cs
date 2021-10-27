using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KCTM.Network;
using KCTM.Network.Data;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.UI;

public class LoadARScenes : MonoBehaviour
{
    public float default_height = 1.5f;

    // For debuggin login. Set true for login using debug email/password 
    public bool autoLogin = false;
    // Debug login info
    string uri;
    public string email;
    public string password;


    public double minLatitude, minLongitude, maxLatitude, maxLongitude;

    GameObject errorPanel;
    
    Transform worldParent;
    Transform cameraTransform;

    public Transform arScenesParent;
    public Transform cardContentParent;
    // Start is called before the first frame update
    
    void Start()
    {
        NetworkManager.Instance.basicUri = uri;

        if (autoLogin)
            Signin();

        else
        {
            Debug.Log("Email: " + PlayerPrefs.GetString("email"));
            Debug.Log("password: " + PlayerPrefs.GetString("password"));
        }
    }

    // Update is called once per frame

    private void Awake()
    {
        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
        worldParent = GameObject.Find("Real World").transform;

        arScenesParent = GameObject.Find("ARSceneParent").transform;

        uri = ServerURL.Instance.uri;
        
        //Initialize coordinate system.
        double latFrom, lonFrom, latTo, lonTo;
        ARRC_DigitalTwin_Generator.TerrainUtils.GetCoord(minLatitude, minLongitude, maxLatitude, maxLongitude, out latFrom, out lonFrom, out latTo, out lonTo);
        ARRC_DigitalTwin_Generator.TerrainContainer.Instance.SetCoordinates(latFrom, lonFrom, latTo, lonTo);
        ARRC_DigitalTwin_Generator.TerrainContainer.Instance.SetTerrain(GameObject.FindGameObjectWithTag("KAIST Terrain").GetComponent<Terrain>());
    }

    /*
 * Function: Signin
 *
 * Details:
 * - Function used to login using debug account: contacts server
 * - Email: asdf@asdf.com
 * - Password: 123456
 */
    public void Signin()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("email", email);
        data.Add("password", password);

        NetworkManager.Instance.Post("/user/signin", data, ResponseHandler, SiginCallback, FailureHandler);
    }
    /*
 * Function: SiginCallback
 *
 * Details:
 * - Function used to login using debug account: saves login info to PlayerPrefs and calls LoadARIcons
 */
    private void SiginCallback(Result result)
    {
        AccountInfo.Instance.user = JsonConvert.DeserializeObject<User>(result.result.ToString());

        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("password", password);

        PlayerPrefs.Save();

        
    }

    /*
   * Function: LoadARIcons
   *
   * Details:
   * - Function that calls server to send ARScene within GPS range.
   * - If success, go to GetARSceneResultList function.
   * - If fail, go to FailureHandler.
   */
    private void LoadARIcons_Campustour()
    {
        string url = string.Format("/arscene/list?minlatitude={0}&minlongitude={1}&maxlatitude={2}&maxlongitude={3}", minLatitude, minLongitude, maxLatitude, maxLongitude);
        NetworkManager.Instance.Get(url, GetARSceneResultList_Campustour, FailureHandler);
    }
    private void LoadARIcons_DramaKAIST()
    {
        string url = string.Format("/arscene/list?minlatitude={0}&minlongitude={1}&maxlatitude={2}&maxlongitude={3}", minLatitude, minLongitude, maxLatitude, maxLongitude);
        NetworkManager.Instance.Get(url, GetARSceneResultList_DramaKAIST, FailureHandler);
    }
    public void GetARSceneResultList_Campustour(Result result)
    {
        var anchorList = JsonConvert.DeserializeObject<List<Anchor>>(result.result.ToString());

        var campustour_anchors = anchorList.Where(e => (e.tags.Exists(e2 => e2.tag == "CampusTour") && e.contentinfos.Count != 0));
     
        StartCoroutine("CreateAnchorIcon", campustour_anchors.ToList());
        //StartCoroutine("CheckIsIconVisible");
        //CreateAnchorIcon2(campustour_anchors.ToList());
        //CreateAnchorIcon(anchor);
    }

    public void GetARSceneResultList_DramaKAIST(Result result)
    {
        var anchorList = JsonConvert.DeserializeObject<List<Anchor>>(result.result.ToString());

        // We are only interested in anchors where each anchor has "Campus tour tag".
        var campustour_anchors = anchorList.Where(e => (e.tags.Exists(e2 => e2.tag == "DramaKAIST") && e.contentinfos.Count != 0));
 
        StartCoroutine("CreateAnchorIcon", campustour_anchors.ToList());
    }

    public IEnumerator CreateAnchorIcon(List<Anchor> campustour_anchors)
    {
        foreach (Anchor anchor in campustour_anchors)
        {
            if (anchor.contentinfos.Count == 0)
                continue;

            switch(anchor.contentinfos[0].content.mediatype)
            {
                case "IMAGE":

                    for (int a = 0; a < anchor.tags.Count; a++)
                    {
                        if (anchor.tags[a].category == "InterestTag")
                        {
                            //set Tag text
                            //thubnameText_title.text = anchor.tags[a].tag;
                            GameObject newIcon;
                            switch (anchor.tags[a].tag)
                            {
                                case "Admission":
                                    newIcon = Instantiate(ResourceLoader.Instance.icon_admission, Vector3.zero, Quaternion.identity, arScenesParent);
                                    break;
                                case "Research":
                                    newIcon = Instantiate(ResourceLoader.Instance.icon_research, Vector3.zero, Quaternion.identity, arScenesParent);
                                    break;
                                case "Campus life":
                                    newIcon = Instantiate(ResourceLoader.Instance.icon_campusLife, Vector3.zero, Quaternion.identity, arScenesParent);
                                    break;
                                case "News":
                                    newIcon = Instantiate(ResourceLoader.Instance.icon_news, Vector3.zero, Quaternion.identity, arScenesParent);
                                    break;
                                case "Education":
                                case " Education":
                                    newIcon = Instantiate(ResourceLoader.Instance.icon_education, Vector3.zero, Quaternion.identity, arScenesParent);
                                    break;
                                default:
                                    newIcon = Instantiate(ResourceLoader.Instance.icon_about, Vector3.zero, Quaternion.identity, arScenesParent);
                                    break;
                            }

                            var script = newIcon.GetComponent<IconManager>();
                            script.Init(anchor, anchor.title, anchor.tags[a].tag, anchor.description, cameraTransform, default_height);
                            //newIcon.SetActive(false);
                        }
                    }
                    break;
                case "VIDEO":
                    //"not  implement yet."
                    //ar script = newIcon.GetComponent<IconManager>();
                    //script.Init(anchor, anchor.title, anchor.tags[a].tag, anchor.description, cameraTransform, default_height);
                    break;

                default:
                    break;
            }
            
            yield return null;
        }
        Debug.Log("Icon Creation Done.");
    }

    private void ResponseHandler()
    {
    }

    private void FailureHandler(Result result)
    {
        // Fail to get ARScene
        Debug.LogError(result.error + " : " + result.msg);
        errorPanel.SetActive(true);
    }

    public void SearchTag(InputField inputfield)
    {
        cardContentParent = GameObject.Find("CardContent").transform;

        if (!string.IsNullOrWhiteSpace(inputfield.text))
        {
            StopCoroutine("CreateAnchorIcon");

            foreach (Transform child in cardContentParent)
                Destroy(child.gameObject);

            foreach (Transform child in arScenesParent)
            {
                Destroy(child.gameObject);
            }


            //Debug.Log("Searchtag: " + searchTag);
            string url = string.Format("/arscene/list?minlatitude={0}&minlongitude={1}&maxlatitude={2}&maxlongitude={3}&tags={4}", minLatitude, minLongitude, maxLatitude, maxLongitude, inputfield.text);
            NetworkManager.Instance.Get(url, GetSearchTagResultList, FailHandler);

            url = string.Format("/spacetelling/list?minlatitude={0}&minlongitude={1}&maxlatitude={2}&maxlongitude={3}&tags={4}", minLatitude, minLongitude, maxLatitude, maxLongitude, inputfield.text);
            NetworkManager.Instance.Get(url, GetSearchSpaceTellingTagResultList, FailHandler);

            if (inputfield.text == "캠퍼스투어")
            {
                LoadARIcons_Campustour();
            }
            else if(inputfield.text == "드라마 카이스트")
            {
                LoadARIcons_DramaKAIST();
            }
        }
    }

    public void GetSearchTagResultList(Result result)
    {
        var searchList = JsonConvert.DeserializeObject<List<Anchor>>(result.result.ToString());

        for (int i = 0; i < searchList.Count; i++)
        {
            var anchor = searchList[i];
            for (int j = 0; j < anchor.contentinfos.Count; j++)
            {
                SetMediaAsset(anchor, j);
            }
        }
    }

    public void GetSearchSpaceTellingTagResultList(Result result)
    {
        string url;
        var searchList = JsonConvert.DeserializeObject<List<Anchor>>(result.result.ToString());
        for (int i = 0; i < searchList.Count; i++)
        {
            url = string.Format("/spacetelling?id=" + searchList[i].id);
            NetworkManager.Instance.Get(url, GetSpaceTellingAnchor, FailHandler);
        }
    }

    private void GetSpaceTellingAnchor(Result result)
    {
        var anchor = JsonConvert.DeserializeObject<Anchor>(result.result.ToString(), new AnchorConverter(true));
        var card = Instantiate(ResourceLoader.Instance.card_Group_nav, cardContentParent);
        var script = card.GetComponent<GroupCard>();
        script.Init(anchor);
    }

    private void SetMediaAsset(Anchor anchor, int index)
    {
        GameObject card;
        switch (anchor.contentinfos[index].content.mediatype)
        {
            case "IMAGE":
                card = Instantiate(ResourceLoader.Instance.card_Image_nav, cardContentParent);
                //card.GetComponent<ImageCardNavPrefab>().arScene = anchor;
                //card.GetComponent<ImageCardNavPrefab>().indexContent = index;
                //card.GetComponent<ImageCardNavPrefab>().navigation = navigation;
                //card.GetComponent<ImageCardNavPrefab>().searchPanel = searchPanel;
                //var script = card.GetComponent<ImageCard>();
                //script.Init(anchor);
                break;
        }
      
    }

    private void FailHandler(Result result)
    {
        // Fail to get ARScene
        //Debug.Log("Fail");
        errorPanel.SetActive(true);
        //errorPanelText.text = "Sorry, server is not responsive... Please try again later.";
    }

}

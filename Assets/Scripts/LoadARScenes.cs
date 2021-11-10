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
    public Transform arScenesParent_poi;
    public Transform recommendedParent;
    public Transform cardContentParent;

    public List<Anchor> targetAnchors;
    //for maryam
    public List<Anchor> targetAnchors_recommendation;

    public void Awake()
    {
        uri = ServerURL.Instance.uri;
    }

    void Start()
    {
        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
        worldParent = GameObject.Find("Real World").transform;

        arScenesParent = GameObject.Find("ARSceneParent").transform;
        arScenesParent_poi = GameObject.Find("ARSceneParent_Target").transform;

        recommendedParent = GameObject.Find("RecommendedParent").transform;

        arScenesParent.localPosition = new Vector3(0, ConfigurationManager.Instance.height_anchors, 0);
        arScenesParent_poi.localPosition = new Vector3(0, ConfigurationManager.Instance.height_anchors, 0);
        recommendedParent.localPosition = new Vector3(0, ConfigurationManager.Instance.height_anchors, 0);

        //Initialize coordinate system.
        double latFrom, lonFrom, latTo, lonTo;
        ARRC_DigitalTwin_Generator.TerrainUtils.GetCoord(minLatitude, minLongitude, maxLatitude, maxLongitude, out latFrom, out lonFrom, out latTo, out lonTo);
        ARRC_DigitalTwin_Generator.TerrainContainer.Instance.SetCoordinates(latFrom, lonFrom, latTo, lonTo);
        ARRC_DigitalTwin_Generator.TerrainContainer.Instance.SetTerrain(GameObject.FindGameObjectWithTag("KAIST Terrain").GetComponent<Terrain>());

        NetworkManager.Instance.basicUri = uri;
        
        if (autoLogin)
            Signin();

        else
        {
            email = PlayerPrefs.GetString("email");
            password = PlayerPrefs.GetString("password");
            Debug.Log("Email: " + PlayerPrefs.GetString("email"));
            Debug.Log("password: " + PlayerPrefs.GetString("password"));
        }
      
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
    private void LoadARIcons_Recommendation()
    {
        string url = string.Format("/arscene/list?minlatitude={0}&minlongitude={1}&maxlatitude={2}&maxlongitude={3}", minLatitude, minLongitude, maxLatitude, maxLongitude);
        NetworkManager.Instance.Get(url, GetARSceneResultList_Recommendation, FailureHandler);
    }
    private void LoadARIcons_DramaKAIST()
    {
        string url = string.Format("/arscene/list?minlatitude={0}&minlongitude={1}&maxlatitude={2}&maxlongitude={3}", minLatitude, minLongitude, maxLatitude, maxLongitude);
        NetworkManager.Instance.Get(url, GetARSceneResultList_DramaKAIST, FailureHandler);
    }
    public void GetARSceneResultList_Campustour(Result result)
    {
        var anchorList = JsonConvert.DeserializeObject<List<Anchor>>(result.result.ToString());

        //compustour anchors.
        var media_anchor = anchorList.Where(e => e.contentinfos[0].content.mediatype == "IMAGE");
        //targetAnchors = media_anchor.Where(anchors => (anchors.tags.Exists(tags => tags.tag == "CampusTour") && anchors.contentinfos.Count != 0)).ToList();

        //Recommendation
        GameObject recom = GameObject.Find("Recommendation");
        recom.GetComponent<Recommendation>().anchorList = targetAnchors;
        recom.GetComponent<Recommendation>().loadUserHistory();

        StartCoroutine(CreateAnchorIcon_Mariam(targetAnchors, false));
    }

    public void GetARSceneResultList_Recommendation(Result result)
    {
        var anchorList = JsonConvert.DeserializeObject<List<Anchor>>(result.result.ToString());

        //recommendation anchors.
        targetAnchors_recommendation = anchorList.Where(_anchors => (_anchors.tags.Exists(tags => tags.tag == "recommendation") && _anchors.contentinfos.Count != 0)).ToList();

        StartCoroutine(CreateAnchorIcon_Mariam(targetAnchors_recommendation, true));
    }

    public void GetARSceneResultList_DramaKAIST(Result result)
    {
        var anchorList = JsonConvert.DeserializeObject<List<Anchor>>(result.result.ToString());
        var media_anchor = anchorList.Where(e => (e.contentinfos[0].content.mediatype == "IMAGE" || e.contentinfos[0].content.mediatype == "VIDEO"));
        targetAnchors = media_anchor.Where(e => e.tags.Exists(e2 => e2.tag == "DramaKAIST") && (e.contentinfos.Count != 0) && (e.id != 2972)).ToList();

        CreateAnchorIcon_Hyerim(targetAnchors);
    }

    public void CreateAnchorIcon_Hyerim(List<Anchor> anchors)
    {
        foreach (Anchor anchor in anchors)
        {
            string url = string.Format("/arscene?id={0}&secret=false", anchor.id);
            NetworkManager.Instance.Get(url, CreateAnchorIcon, FailHandler);
        }
    }

    private void CreateAnchorIcon(Result result)
    {
        var anchor = JsonConvert.DeserializeObject<Anchor>(result.result.ToString());

        string mediaType = anchor.contentinfos[0].content.mediatype; //must be "IMAGE" or "VIDEO"
        GameObject newIcon = Instantiate(ResourceLoader.Instance.icon, Vector3.zero, Quaternion.identity, arScenesParent);
        var script = newIcon.AddComponent<IconManager_Heyrim>();
        script.Init(anchor, mediaType, cameraTransform);
        newIcon.SetActive(false);
    }

    public IEnumerator CreateAnchorIcon_Mariam(List<Anchor> anchors, bool isRecommended)
    {
        foreach (Anchor anchor in anchors)
        {
            //string mediaType = anchor.contentinfos[0].content.mediatype; //must be "IMAGE"
            string category = anchor.tags.Where(t1 => t1.category == "InterestTag").Select(t2 => t2.tag).ToArray()[0];
            GameObject newIcon = Instantiate(ResourceLoader.Instance.icon, Vector3.zero, Quaternion.identity, arScenesParent);
            var script = newIcon.AddComponent<IconManager_Maryam>();
            script.Init(anchor, category, cameraTransform, isRecommended);
            newIcon.SetActive(false);
          
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
            foreach (Transform child in arScenesParent_poi)
            {
                Destroy(child.gameObject);
            }

            //Debug.Log("Searchtag: " + searchTag);
            string url = string.Format("/arscene/list?minlatitude={0}&minlongitude={1}&maxlatitude={2}&maxlongitude={3}&tags={4}", minLatitude, minLongitude, maxLatitude, maxLongitude, inputfield.text);
            NetworkManager.Instance.Get(url, GetSearchTagResultList, FailHandler);

            url = string.Format("/spacetelling/list?minlatitude={0}&minlongitude={1}&maxlatitude={2}&maxlongitude={3}&tags={4}", minLatitude, minLongitude, maxLatitude, maxLongitude, inputfield.text);
            NetworkManager.Instance.Get(url, GetSearchSpaceTellingTagResultList, FailHandler);

            //Mariam's icons.
            if (inputfield.text == "캠퍼스투어")
            {
                LoadARIcons_Campustour();
            }
            //Hyerim's icons.
            else if (inputfield.text == "드라마투어")
            {
                LoadARIcons_DramaKAIST();
            }
            //For debuging.
            else if (inputfield.text == "rec")
            {
                LoadARIcons_Recommendation();
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
        var card = Instantiate(ResourceLoader.Instance.card_Group, cardContentParent);
        var script = card.AddComponent<GroupCard>();
        script.Init(anchor, true);
    }

    private void SetMediaAsset(Anchor anchor, int index)
    {
        GameObject card;
        switch (anchor.contentinfos[index].content.mediatype)
        {
            case "IMAGE":
                card = Instantiate(ResourceLoader.Instance.card_Image, cardContentParent);
                //card.GetComponent<ImageCardNavPrefab>().arScene = anchor;
                //card.GetComponent<ImageCardNavPrefab>().indexContent = index;
                //card.GetComponent<ImageCardNavPrefab>().navigation = navigation;
                //card.GetComponent<ImageCardNavPrefab>().searchPanel = searchPanel;
                //var script = card.GetComponent<ImageCard>();
                //script.Init(anchor);
                break;
            case "VIDEO":
            default:
                card = Instantiate(ResourceLoader.Instance.card_Video, cardContentParent);
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

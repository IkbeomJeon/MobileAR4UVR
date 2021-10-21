using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KCTM.Network;
using KCTM.Network.Data;
using Newtonsoft.Json;
using System.Linq;

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

    public GameObject contentIcon;
    public GameObject arScenesParent;
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
            LoadARScene();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
        worldParent = GameObject.Find("Real World").transform;
        contentIcon = Resources.Load("Prefabs/ContentIcon") as GameObject;

        uri = ServerURL.Instance.uri;
        
        //Initialize coordinate system.
        double latFrom, lonFrom, latTo, lonTo;
        ARRC_DigitalTwin_Generator.TerrainUtils.GetCoord(minLatitude, minLongitude, maxLatitude, maxLongitude, out latFrom, out lonFrom, out latTo, out lonTo);
        ARRC_DigitalTwin_Generator.TerrainContainer.Instance.SetCoordinates(latFrom, lonFrom, latTo, lonTo);
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
 * - Function used to login using debug account: saves login info to PlayerPrefs and calls LoadARScene
 */
    private void SiginCallback(Result result)
    {
        AccountInfo.Instance.user = JsonConvert.DeserializeObject<User>(result.result.ToString());

        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("password", password);

        PlayerPrefs.Save();

        LoadARScene();
    }

    /*
   * Function: LoadARScene
   *
   * Details:
   * - Function that calls server to send ARScene within GPS range.
   * - If success, go to GetARSceneResultList function.
   * - If fail, go to FailureHandler.
   */
    private void LoadARScene()
    {
        string url = string.Format("/arscene/list?minlatitude={0}&minlongitude={1}&maxlatitude={2}&maxlongitude={3}", minLatitude, minLongitude, maxLatitude, maxLongitude);
        NetworkManager.Instance.Get(url, GetARSceneResultList, FailureHandler);
    }
    public void GetARSceneResultList(Result result)
    {
        //Recommendation
        List<Anchor> recommendedList = new List<Anchor>();

        List<Anchor> anchorList = JsonConvert.DeserializeObject<List<Anchor>>(result.result.ToString());

        arScenesParent = new GameObject("ARSceneParent");
        arScenesParent.transform.parent = worldParent;


        
        foreach (Anchor anchor in anchorList)
        {
            // We are only interested in anchors where each anchor has "Campus tour tag".
            if (!anchor.tags.Exists(e => e.tag == "CampusTour"))
                continue;

            // GameObject newIcon = Instantiate(contentIcon, Vector3.zero, Quaternion.identity, arScenesParent.transform) as GameObject;
            // var script = newIcon.GetComponent<IconManager>();
            //script.Init(anchor, cameraTransform);

            //var tags = anchor.tags.Where(e => e.category == "InterestTag").Select(e=>e.tag);

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
                            newIcon = Instantiate(ResourceLoader.Instance.icon_admission, Vector3.zero, Quaternion.identity, arScenesParent.transform) as GameObject;
                            break;
                        case "Research":
                            newIcon = Instantiate(ResourceLoader.Instance.icon_research, Vector3.zero, Quaternion.identity, arScenesParent.transform) as GameObject;
                            break;
                        case "Campus life":
                            newIcon = Instantiate(ResourceLoader.Instance.icon_campusLife, Vector3.zero, Quaternion.identity, arScenesParent.transform) as GameObject;
                            break;
                        case "News":
                            newIcon = Instantiate(ResourceLoader.Instance.icon_news, Vector3.zero, Quaternion.identity, arScenesParent.transform) as GameObject;
                            break;
                        case "Education":
                        case " Education":
                            newIcon = Instantiate(ResourceLoader.Instance.icon_education, Vector3.zero, Quaternion.identity, arScenesParent.transform) as GameObject;
                            break;
                        default:
                            newIcon = Instantiate(ResourceLoader.Instance.icon_about, Vector3.zero, Quaternion.identity, arScenesParent.transform) as GameObject;
                            break;
                    }

                    var script = newIcon.GetComponent<IconManager>();
                    script.Init(anchor, cameraTransform);
                }
            }
            



        }
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
}

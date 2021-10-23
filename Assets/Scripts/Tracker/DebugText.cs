using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugText : MonoBehaviour
{

    public string[] strArray = new string[10];

    GameObject arCamera;
    GameObject world;

    static DebugText instance;
    
    public static DebugText Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = GameObject.Find("Debug_Text");
                if (obj == null)
                {
                    //obj = new GameObject("Global AR Camera");
                    obj = Instantiate(Resources.Load("Canvas_Debug")) as GameObject;
                    instance = obj.transform.GetChild(0).gameObject.AddComponent<DebugText>();
                }
                else
                {
                    instance = obj.GetComponent<DebugText>();
                }
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }


    void Start()
    {
        arCamera = GameObject.FindWithTag("MainCamera");
        world = GameObject.Find("Real World");
        // arSessionOrigin.transform.position = new Vector3(100, 100, 100);
    }

    // Update is called once per frame
    void Update()
    {
        //arSessionOrigin.transform.position = new Vector3(100, 100, 100);

        Vector3 eulerGcam = GlobalARCameraInfo.Instance.globalRotation.eulerAngles;
        Quaternion rotationWorld = world.transform.rotation;

        string strGPS = string.Format("{0:f6}, {1:f6}, {2:f3}", GlobalARCameraInfo.Instance.latitude, GlobalARCameraInfo.Instance.longitude, GlobalARCameraInfo.Instance.altitude);
        string strGCam = string.Format("({0:f3}, {1:f3}, {2:f3}), ({3}, {4}, {5})"
            , GlobalARCameraInfo.Instance.globalPosition.x, GlobalARCameraInfo.Instance.globalPosition.y, GlobalARCameraInfo.Instance.globalPosition.z
            , eulerGcam.x, eulerGcam.y, eulerGcam.z);

        string strWorld = string.Format("\n\n\n({0:f3}, {1:f3}, {2:f3})\n, ({3}, {4}, {5},{6}))"
            , world.transform.position.x, world.transform.position.y, world.transform.position.z
            , rotationWorld.x, rotationWorld.y, rotationWorld.z, rotationWorld.w);
        
        string strLCam = string.Format("{0}, {1}", arCamera.transform.position.ToString(), arCamera.transform.rotation.eulerAngles.ToString());
        string text = string.Format("Build Version : {0}\nLat/Lon : {1}\nGCam : {2}\nWorld : {3}\nLCam : {4}\n"
            , Application.version.ToString(), strGPS, strGCam, strWorld, strLCam);

        string more = strWorld + '\n';

        foreach(string str in strArray)
        {
            text += str +"\n";
        }

        GetComponent<TextMeshProUGUI>().text = more;
    }
}

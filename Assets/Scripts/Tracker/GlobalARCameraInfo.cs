using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalARCameraInfo : MonoBehaviour
{
    public Vector3 globalPosition;
    public Quaternion globalRotation;
    public double latitude, longitude, altitude;
    
    static GlobalARCameraInfo instance;

    public static GlobalARCameraInfo Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject obj = GameObject.Find("Global AR Camera");
                if(obj == null)
                {
                    obj = new GameObject("Global AR Camera");
                    instance = obj.AddComponent<GlobalARCameraInfo>();
                }
                else
                {
                    instance = obj.GetComponent<GlobalARCameraInfo>();
                }
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorController : MonoBehaviour
{
    static SensorController instance;
    public static SensorController Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = GameObject.Find("SensorController");
                if (obj == null)
                {
                    obj = new GameObject("SensorController");
                    instance = obj.AddComponent<SensorController>();
                }
                else
                {
                    instance = obj.GetComponent<SensorController>();
                }
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }

    public double latitude = 0.0;
    public double longitude = 0.0;
    public float fHeading;

    public bool bInit = false;
    bool bGPS = false;
    // Start is called before the first frame update
    public void PreperSensor()
    {
        Input.compass.enabled = true;
        //DebugText.Instance.strArray[1] = "compass enabled : " + Input.compass.enabled.ToString();
        StartCoroutine(StartLocationService());
        StartCoroutine(StartCalculate());
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.compass.enabled)
        {
            fHeading = Input.compass.trueHeading;
        }
    }

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            //DebugText.Instance.strArray[2] = "user has not enabled gps.";
            //use has not enabled gps
            //gpsText.text = "user has not enabled gps";
            yield break;
        }
        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait <= 0)
        {
            //Timed out
            //gpsText.text = "Timed out";
            //DebugText.Instance.strArray[3] = "Timed out";
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            //Unable to determine device location
            //gpsText.text = "Unable to determine device location";
            //DebugText.Instance.strArray[4] = "Unable to determine device location";
            yield break;
        }
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;

        //GlobalARCameraInfo.Instance.latitude = Input.location.lastData.latitude;
        //GlobalARCameraInfo.Instance.longitude = Input.location.lastData.longitude;

        bInit = true;
        bGPS = true;
        yield break;
    }
    private IEnumerator StartCalculate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (bGPS)
            {
                latitude = Input.location.lastData.latitude;
                longitude = Input.location.lastData.longitude;
                //DebugText.Instance.strArray[5] = "GPS Upadated : " + latitude.ToString() + ", " + longitude.ToString();
            }
        }
    }
}

using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public bool init;
    public GameObject map;
    public GameObject userPin;
    public GameObject dummy;
    // Start is called before the first frame update
    void Start()
    {
       
        
    }

    private void OnEnable()
    {
        if (!init) Init();

        var pos = new Vector2d(GlobalARCameraInfo.Instance.latitude, GlobalARCameraInfo.Instance.longitude);
        map.GetComponent<AbstractMap>().SetCenterLatitudeLongitude(pos);
    }

    // Update is called once per frame
    void Update()
    {
        if(init)
        {
            var pos = new Vector2d(GlobalARCameraInfo.Instance.latitude, GlobalARCameraInfo.Instance.longitude);
            var geoPos = map.GetComponent<AbstractMap>().GeoToWorldPosition(pos);
            userPin.transform.position = new Vector3(geoPos.x, 0, geoPos.z);

            Vector3 angle = GlobalARCameraInfo.Instance.globalRotation.eulerAngles;
            userPin.transform.rotation = Quaternion.Euler(90f, angle.y, 0f);

            
            
        }
    }

    public void Init()
    {
        GameObject arScenesParent = GameObject.Find("ARSceneParent");
        Instantiate(dummy, arScenesParent.transform);

        dummy.transform.localPosition = GlobalARCameraInfo.Instance.globalPosition;
        dummy.transform.localRotation = GlobalARCameraInfo.Instance.globalRotation;

        map = transform.Find("Map").gameObject;
        userPin = Instantiate(ResourceLoader.Instance.pinPoint, map.transform);
        init = true;
    }
    

}

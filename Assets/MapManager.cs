using ARRC_DigitalTwin_Generator;
using KCTM.Network.Data;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public bool mapActive = false;
    public LineRenderer lr;
    public AbstractMap map;
    public GameObject mapCamera;
    public GameObject userPin;
    public List<Vector2d> waypoints;

    void Awake()
    {
        map = transform.Find("Map").gameObject.GetComponent<AbstractMap>();
        mapCamera = transform.Find("Map Camera").gameObject;
        //amapaamera = transform.Find("Map Camera").gameObject;
        lr = transform.Find("Line Renderer").GetComponent<LineRenderer>();
        userPin = Instantiate(ResourceLoader.Instance.pinPoint, map.transform);
       
    }
    // Update is called once per frame
    void Update()
    {
        if (mapActive)
        {
            //show user pin on map.
            //Debug.Log("Track");
            var geoPos = new Vector2d(GlobalARCameraInfo.Instance.latitude, GlobalARCameraInfo.Instance.longitude);
            //map.SetCenterLatitudeLongitude(geoPos);

            var worldPos_user = map.GeoToWorldPosition(geoPos);
            userPin.transform.position = new Vector3(worldPos_user.x, 1, worldPos_user.z);
            //lr.SetPosition(0, worldPos_user);

            Vector3 angle = GlobalARCameraInfo.Instance.globalRotation.eulerAngles;
            userPin.transform.rotation = Quaternion.Euler(90f, angle.y, 0f);

            if (waypoints!=null && waypoints.Count > 0)
                DrawNavigationRoute(map.transform.localScale.x);
        }
    }
    public void ActivateMap()
    {
        mapActive = true;
        mapCamera.SetActive(mapActive);
      
        Debug.Log("SwitchMapState");
        var pos = new Vector2d(GlobalARCameraInfo.Instance.latitude, GlobalARCameraInfo.Instance.longitude);
        //map.SetZoom(17);
        map.SetCenterLatitudeLongitude(pos);
        //map.transform.localScale = new Vector3(1, 1, 1);
    }
    public void DeactivateMap()
    {
        mapActive = false;
        mapCamera.SetActive(mapActive);
    }
    public void SwitchMapState()
    {
        if (mapActive)
            DeactivateMap();
        else
            ActivateMap();
    }
    public void SetWayPoints(List<Vector2d> points)
    {
        foreach(var pt in points)
        {
            Vector2 pos = TerrainUtils.LatLonToWorld(TerrainContainer.Instance, pt.x, pt.y);
            GameObject newObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newObj.transform.position = new Vector3(pos.x, 1.5f, pos.y);
        }
        waypoints = points;
    }

    public void DrawNavigationRoute(float scale)
    {
        lr.startWidth = 2f * scale;
        lr.endWidth = 2f * scale;

        //Draw on mapbox image.
        var geoPos_user = new Vector2d(GlobalARCameraInfo.Instance.latitude, GlobalARCameraInfo.Instance.longitude);
        var wPos_user = map.GeoToWorldPosition(geoPos_user);
        var worldPos_user = new Vector3(wPos_user.x, 1, wPos_user.z);

        lr.positionCount = waypoints.Count + 1;
        lr.SetPosition(0, worldPos_user);

        for (int i=0; i<waypoints.Count; i++)
        {
            //Debug.Log(waypoints[i].ToString());
            var wPos = map.GeoToWorldPosition(waypoints[i]);
            var worldPos = new Vector3(wPos.x, 1, wPos.z);
            lr.SetPosition(i+1, worldPos);
        }

   
    }

}

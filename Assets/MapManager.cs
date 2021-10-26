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
    public Transform realworldTransform;
    public LineRenderer lr2D;
    public LineRenderer lr3D;
    public AbstractMap map;
    public GameObject mapCamera;
    public GameObject userPin;
    public List<Vector2d> waypoints;

    public bool navigationOn;
    public float height_way3D = 0.5f;
    public float height_way2D = 1;

    void Awake()
    {
        realworldTransform = GameObject.Find("Real World").transform;
        map = transform.Find("Map").gameObject.GetComponent<AbstractMap>();
        mapCamera = transform.Find("Map Camera").gameObject;
        //amapaamera = transform.Find("Map Camera").gameObject;
        lr2D = transform.Find("Line Renderer_2D").GetComponent<LineRenderer>();
        lr3D = transform.Find("Line Renderer_3D").GetComponent<LineRenderer>();
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

            var mapPos_user = map.GeoToWorldPosition(geoPos);
            userPin.transform.position = new Vector3(mapPos_user.x, height_way2D, mapPos_user.z);
            //lr.SetPosition(0, worldPos_user);

            Vector3 angle = GlobalARCameraInfo.Instance.globalRotation.eulerAngles;
            userPin.transform.rotation = Quaternion.Euler(90f, angle.y, 0f);

            if (navigationOn)
            {
                DrawNavigationRouteOnMap(map.transform.localScale.x);
            }
        }
                
        else if(navigationOn)
        {
            DrawNavigationRouteOnWorld();
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
        navigationOn = true;
        waypoints = points;
        
        DrawNavigationRouteOnWorld();
    }

    public void DrawNavigationRouteOnMap(float scale)
    {
        lr2D.positionCount = waypoints.Count + 1;
        lr2D.startWidth = 2f * scale;
        lr2D.endWidth = 2f * scale;

        //Draw on mapbox image.
        var geoPos_user = new Vector2d(GlobalARCameraInfo.Instance.latitude, GlobalARCameraInfo.Instance.longitude);
        var wPos_user = map.GeoToWorldPosition(geoPos_user);
        var mapPos_user = new Vector3(wPos_user.x, height_way2D, wPos_user.z);
        lr2D.SetPosition(0, mapPos_user);

        for (int i=0; i<waypoints.Count; i++)
        {
            //Debug.Log(waypoints[i].ToString());
            var wPos = map.GeoToWorldPosition(waypoints[i]);
            var mapPos_waypoint = new Vector3(wPos.x, height_way2D, wPos.z);
            lr2D.SetPosition(i+1, mapPos_waypoint);
        }
    }
    public void DrawNavigationRouteOnWorld()
    {
        lr2D.startWidth = 5f;
        lr2D.endWidth = 5f;

        lr3D.positionCount = waypoints.Count + 1;

        var worldPos_user = GlobalARCameraInfo.Instance.globalPosition;
        Matrix4x4 mat_Realworld2ARworld = realworldTransform.localToWorldMatrix;
        //Debug.Log(mat_Realworld2ARworld.ToString());
        Vector3 result_pos = mat_Realworld2ARworld.MultiplyPoint(new Vector4(worldPos_user.x, worldPos_user.y - 1.5f + height_way3D, worldPos_user.z));
        lr3D.SetPosition(0, new Vector3(result_pos.x, result_pos.y, result_pos.z));

        for (int i = 0; i < waypoints.Count; i++)
        {
            //Debug.Log(waypoints[i].ToString());
            Vector3 wpos_wp = TerrainUtils.LatLonToWorldWithElevation(TerrainContainer.Instance, waypoints[i].x, waypoints[i].y);
            var worldPos_wp = new Vector3(wpos_wp.x, wpos_wp.y + height_way3D, wpos_wp.z);
            Vector3 result_pos_wp = mat_Realworld2ARworld.MultiplyPoint(new Vector4(worldPos_wp.x, worldPos_wp.y, worldPos_wp.z));
            lr3D.SetPosition(i + 1, result_pos_wp);
        }
    }

    public void StopNavigation()
    {
        navigationOn = false;
        lr2D.positionCount = 0;
        lr3D.positionCount = 0;
    }


}

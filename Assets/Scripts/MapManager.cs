using ARRC_DigitalTwin_Generator;
using KCTM.Network.Data;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public bool mapActive = false;
    public Transform realworldTransform;
    public LineRenderer lr2D;
    public LineRenderer lr3D;
    public AbstractMap map;
    public Transform poiNumParent;
    public GameObject mapCamera;
    public GameObject userPin;

    public Dictionary<WayPoint, GameObject> poi_numObject = new Dictionary<WayPoint, GameObject>();
    public List<WayPoint> waypoints = new List<WayPoint>();
   
    public bool navigationOn;
    //public float height_way3D = 0.5f;
    public float height_way2D = 1;

    //for recommendation.
    public List<Anchor> stories = new List<Anchor>();
    public int spaceTellingIndex = 0;

    //public ResourceLoader rl;
    void Awake()
    {
        realworldTransform = GameObject.Find("Real World").transform;
        map = transform.Find("Map").gameObject.GetComponent<AbstractMap>();
        poiNumParent = transform.Find("Map/POIParent").transform;
        mapCamera = transform.Find("Map Camera").gameObject;
        //amapaamera = transform.Find("Map Camera").gameObject;
        lr2D = transform.Find("Line Renderer_2D").GetComponent<LineRenderer>();
        lr3D = transform.Find("Line Renderer_3D").GetComponent<LineRenderer>();
        userPin = transform.Find("Map/PinPoint").gameObject;
        //rl = GameObject.Find("ResourceLoader").GetComponent<ResourceLoader>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (mapActive)
        {
            DrawNavigationRouteOn2DMap(map.transform.localScale.x);
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
      
        //Debug.Log("SwitchMapState");
        var pos = new Vector2d(GlobalARCameraInfo.Instance.latitude, GlobalARCameraInfo.Instance.longitude);
        //map.SetZoom(17);
        map.SetCenterLatitudeLongitude(pos);
        //map.SetZoom(15);
        map.UpdateMap();
        //map.transform.localScale = new Vector3(1, 1, 1);

        StartCoroutine(WaitUntil(0.1f));
        //
    }
    IEnumerator WaitUntil(float time)
    {
        yield return new WaitForSeconds(time);
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

  
    public void StartNavigation(List<WayPoint> points, List<Anchor> stories)
    {
      
        StopNavigation();

        var pos = new Vector2d(GlobalARCameraInfo.Instance.latitude, GlobalARCameraInfo.Instance.longitude);
        //map.SetZoom(17);
        map.SetCenterLatitudeLongitude(pos);
        //StartCoroutine(WaitUntil(0.1f));
        //map.transform.localScale = new


        navigationOn = true;

        //deep copy.
        foreach(var anchor in stories)
        {
            this.stories.Add(anchor);
        }   

        //deep copy.
        int poiNum = 0;
        foreach (var point in points)
        {
            waypoints.Add(point);
            
            if (point.isPOI)
            {
                //create number object.
                GameObject poi = Instantiate(ResourceLoader.Instance.poiPoint, poiNumParent);
                var wPos = map.GeoToWorldPosition(point.pos);
                var mapPos_waypoint = new Vector3(wPos.x, height_way2D, wPos.z);

                poi.transform.position = mapPos_waypoint;
                poi.transform.Find("Text").GetComponent<TextMeshPro>().text = (++poiNum).ToString();
                poi_numObject.Add(point, poi);
                
                //poi_num.Add(poiPoint);
            }
            


        }

        // removed by Jeon. 201109.
        //GameObject recom = GameObject.Find("Recommendation");
        //recom.GetComponent<Recommendation>().calMidPoints(stories, waypoints);
        //DrawNavigationRouteOn2DMap(map.transform.localScale.x);
        //DrawNavigationRouteOnWorld();
    }

    public void DrawNavigationRouteOn2DMap(float scale)
    {
        //Draw user position.
        var geoPos_user = new Vector2d(GlobalARCameraInfo.Instance.latitude, GlobalARCameraInfo.Instance.longitude);
        var wPos_user = map.GeoToWorldPosition(geoPos_user);
        var mapPos_user = new Vector3(wPos_user.x, height_way2D, wPos_user.z);
    
        Vector3 angle = GlobalARCameraInfo.Instance.globalRotation.eulerAngles;
        userPin.transform.position = mapPos_user;
        userPin.transform.rotation = Quaternion.Euler(90f, angle.y, 0f);
        userPin.transform.localScale = new Vector3(1f / scale, 1f/ scale, 1f/ scale);


        //Draw navigation route.
        lr2D.positionCount = waypoints.Count + 1;
        lr2D.startWidth = 2f * scale;
        lr2D.endWidth = 2 * scale;
        lr2D.SetPosition(0, mapPos_user);
       
        for (int i=0; i< waypoints.Count; i++)
        {
            var wPos = map.GeoToWorldPosition(waypoints[i].pos);
            var mapPos_waypoint = new Vector3(wPos.x, height_way2D, wPos.z);

            if(poi_numObject.ContainsKey(waypoints[i]))
            {
                poi_numObject[waypoints[i]].transform.position = mapPos_waypoint;
                poi_numObject[waypoints[i]].transform.localScale = new Vector3(1 / scale, 1 / scale, 1 / scale);
            }

            lr2D.SetPosition(i + 1, mapPos_waypoint);

        }
    }

    public void DrawNavigationRouteOnWorld()
    {
        //float a = Time.time;
        lr2D.startWidth = 5f;
        lr2D.endWidth = 5f;
        Matrix4x4 mat_Realworld2ARworld = realworldTransform.localToWorldMatrix;

        lr3D.positionCount = waypoints.Count + 1;

        //ReDrawUserPositionOnWorld();

        float height_way3D = ConfigurationManager.Instance.height_navigation_3d;
        var worldPos_user = GlobalARCameraInfo.Instance.globalPosition;
        float user_height = ConfigurationManager.Instance.height_user;
        Vector3 result_pos_user = mat_Realworld2ARworld.MultiplyPoint(new Vector4(worldPos_user.x, worldPos_user.y - user_height + height_way3D, worldPos_user.z));
        lr3D.SetPosition(0, new Vector3(result_pos_user.x, result_pos_user.y, result_pos_user.z));

        if (waypoints.Count > 0)
        {
            //remove waypoint by distance, if it is not poi.
            if (!waypoints[0].isPOI)
            {
                Vector3 wpos_wp = TerrainUtils.LatLonToWorldWithElevation(TerrainContainer.Instance, waypoints[0].pos.x, waypoints[0].pos.y);
                var worldPos_wp = new Vector3(wpos_wp.x, wpos_wp.y + height_way3D, wpos_wp.z);
                Vector3 result_pos_wp = mat_Realworld2ARworld.MultiplyPoint(new Vector4(worldPos_wp.x, worldPos_wp.y, worldPos_wp.z));

                if (Vector3.Distance(result_pos_user, result_pos_wp) < ConfigurationManager.Instance.distance_to_remove_midle_waypoint)
                    waypoints.RemoveAt(0);
            }
        }

        for (int i = 0; i < waypoints.Count; i++)
        {
            Vector3 wpos_wp = TerrainUtils.LatLonToWorldWithElevation(TerrainContainer.Instance, waypoints[i].pos.x, waypoints[i].pos.y);
            var worldPos_wp = new Vector3(wpos_wp.x, wpos_wp.y + height_way3D, wpos_wp.z);
            Vector3 result_pos_wp = mat_Realworld2ARworld.MultiplyPoint(new Vector4(worldPos_wp.x, worldPos_wp.y, worldPos_wp.z));

            lr3D.SetPosition(i + 1, result_pos_wp);
        }
        //Debug.Log(string.Format("{0}", Time.time - a));
    }

    public void StopNavigation()
    {
        navigationOn = false;

        foreach(GameObject poi in poi_numObject.Values)
            Destroy(poi);

        waypoints.Clear();
        poi_numObject.Clear();
        stories.Clear();
        
        spaceTellingIndex = 0;

        lr2D.positionCount = 0;
        lr3D.positionCount = 0;

       
    }

    public void RemoveCurrentPOI()
    {
        if(waypoints.Count>0)
        {
            if(waypoints[0].isPOI)
            {
                waypoints.RemoveAt(0);
                spaceTellingIndex++;
            }
        }
    }


}

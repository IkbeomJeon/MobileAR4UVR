using ARRC_DigitalTwin_Generator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]

public class TrackerClientManager : MonoBehaviour
{
    public string server_ip;
    public int port;

    Transform realworldTransform;
    GameObject arCamera;

    ARSession arSession;
    ARTrackedImageManager trackerManager;
    ARTrackedImage lastestTrackedImage;
    string lastestTrackedImage_guid = "null";
    Matrix4x4 mat_ImageAnchorfromARworld = Matrix4x4.identity;

    Matrix4x4 mat_Realworld2ARworld;
    Matrix4x4 mat_plane; //마커 plane의 정면을 카메라로 향하도록 하는 변환.

    //초기 트래킹 시작을 체크하는 플래그 by 전진우

    bool runTracking = false;
    bool scanModeOn = false;
    GameObject completeHandler;

    public IDictionary<string, GameObject> dicImageAnchors = new Dictionary<string, GameObject>();

    public float active_distance = 20f;
    public Transform arScenesParent;
    private void Awake()
    {
        trackerManager = GetComponent<ARTrackedImageManager>();
        arCamera = GameObject.FindWithTag("MainCamera");
        arCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Image Anchor"));

        realworldTransform = GameObject.Find("Real World").transform;
        mat_Realworld2ARworld = realworldTransform.localToWorldMatrix.inverse;


        //compensate the plane coordinate.
        mat_plane = new Matrix4x4();
        mat_plane.SetColumn(0, new Vector4(-1, 0, 0, 0));
        mat_plane.SetColumn(1, new Vector4(0, 0, -1, 0));
        mat_plane.SetColumn(2, new Vector4(0, -1, 0, 0));
        mat_plane.SetColumn(3, new Vector4(0, 0, 0, 1));

        //image anchor
        GameObject[] imageAnchorObjects = GameObject.FindGameObjectsWithTag("Image Anchor");
        foreach (GameObject go in imageAnchorObjects)
        {
            string guid = go.GetComponent<ARRCImageAnchorProperty>().guid;
            dicImageAnchors.Add(guid, go);
            Debug.Log(guid);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        arSession = GameObject.Find("AR Session").GetComponent<ARSession>();
        arScenesParent = GameObject.Find("ARSceneParent").transform;

        if (TerrainContainer.Instance.Terrain == null)
            Debug.LogWarning("Set Terrain First");

        TrackerClient.Instance.Connect(server_ip, port);

        //try
        //{
        //    //Reqeust Image Anchor Pose.
        //    List<XRReferenceImage>.Enumerator em = trackerManager.referenceLibrary.GetEnumerator();

        //    while (em.MoveNext())
        //    {
        //        XRReferenceImage val = em.Current;
        //        string guid_library = val.guid.ToString();
        //        TrackerClient.Instance.RequestImageAnchorPose(guid_library);
        //    }
        //}catch(NullReferenceException ex)
        //{
        //    Debug.LogWarning(ex.Message.ToString());
        //}


        SensorController.Instance.PreperSensor();

        string filename = PlayerPrefs.GetString("email").Replace("@", "_");
        StartCoroutine(CheckIsIconVisible());
        StartCoroutine(WriteTrajectory(filename + ".txt", 3));

    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        if (!runTracking)
        {
            mat_Realworld2ARworld = LocalizationbyImageTracker_Psuedo();
            runTracking = true;
        }

#else
        //초기 트래킹 시작 감지 중 by 전진우
        if (!runTracking && SensorController.Instance.bInit)
        {
            //초기 오리진 생성
            mat_Realworld2ARworld = GetInitialGlobalPoseMatrixfromSenser(1.5f).inverse;
            runTracking = true;
        }
#endif


        //초기 오리진이 생성되면 트래킹 시작 by 전진우
        //마커 기반으로 자기 자신 위치 업데이트
        //DebugText.Instance.strArray[0] = "Sensor init : " + SensorController.Instance.bInit.ToString();

        if (runTracking)
        {
           //image tracker를 이용한 localize.
            if (lastestTrackedImage != null)
                mat_Realworld2ARworld = LocalizationbyImageTracker();

            // real world를 ar world에 맞게 변환한다.(ARFoundation의 다양한 기능들을 사용하려면 AR Session Origin은 바뀌어선 안되기 때문에)
            realworldTransform.position = mat_Realworld2ARworld.GetColumn(3);
            realworldTransform.rotation = mat_Realworld2ARworld.rotation;

            Matrix4x4 mat_transformed = mat_Realworld2ARworld.inverse * arCamera.transform.localToWorldMatrix;

            //현재 카레라의 위도, 고도를 트랙킹하기 위해서.
            GlobalARCameraInfo.Instance.globalPosition = mat_transformed.GetColumn(3);
            GlobalARCameraInfo.Instance.globalRotation = mat_transformed.rotation;

            TerrainUtils.WorldToLatLon(TerrainContainer.Instance
                , GlobalARCameraInfo.Instance.globalPosition, out GlobalARCameraInfo.Instance.latitude, out GlobalARCameraInfo.Instance.longitude);

            GlobalARCameraInfo.Instance.altitude = TerrainContainer.Instance.GetHeight(GlobalARCameraInfo.Instance.globalPosition);

            if(TrackerClient.Instance.isConnected)
            {
                //전달 
                TransformData data = new TransformData
                {
                    cameraPosition = GlobalARCameraInfo.Instance.globalPosition,
                    cameraRotation = GlobalARCameraInfo.Instance.globalRotation
                };
                //TrackerClient.Instance.UpdateCameraPose2Server(data);
                TrackerClient.Instance.Broadcast(data);
            }
           

        }
    }

    private void OnEnable()
    {
        trackerManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackerManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnApplicationQuit()
    {
        if(TrackerClient.Instance.isConnected)
            TrackerClient.Instance.Disconnect();
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UnityEngine.XR.ARSubsystems.TrackingState state = trackedImage.trackingState;

            if (state == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                //주의 : 한프레임에 여러개의 이미지가 트랙킹될 때는 첫번째 이미지가 선택됨.
                lastestTrackedImage = trackedImage;
                break;
            }
        }
    }


    Matrix4x4 LocalizationbyImageTracker_Psuedo()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Image Anchor Pseudo");

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        bool isHit = Physics.Raycast(arCamera.transform.position, arCamera.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask);

        Debug.DrawRay(arCamera.transform.position, arCamera.transform.TransformDirection(Vector3.forward) * 1000, Color.white);

        if (isHit)
        {
            Debug.DrawRay(arCamera.transform.position, arCamera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            if (scanModeOn && Input.GetKeyDown(KeyCode.Space))  //Tracking Compelte condition.
            {
                //AR Origin에서의 마커 pose.
                lastestTrackedImage_guid = hit.transform.gameObject.GetComponent<ARRCImageAnchorProperty>().guid; 
                mat_ImageAnchorfromARworld.SetTRS(hit.transform.position, hit.transform.rotation, new Vector3(1, 1, 1)); // scale을 111로 만들기 위해 새로운 매트릭스 생성.
                //mat_ImageAnchorfromARworld에 이미 plane frame이 곱해져 있음.

                completeHandler.SendMessage("CompleteScan");
            }
                       
        }

        //Realworld Origin에서의  마커 Pose.
        Matrix4x4 mat_ImageAnchorfromRealWorld = GetGlobalPoseMatrixfromTrackedImage(lastestTrackedImage_guid);

        //Transform : Real world ->  AR world
        return mat_ImageAnchorfromARworld * mat_ImageAnchorfromRealWorld.inverse;
    }

    Matrix4x4 LocalizationbyImageTracker()
    {
        Debug.Log("in LocalizationbyImageTracker");
        if (scanModeOn && lastestTrackedImage.trackingState == TrackingState.Tracking) //Tracking Compelte condition.
        {
            //AR Origin에서의 마커 pose.
            //ARfoundation의 마커 축이 x축으로 -90도여서 , +90을 보상해줌.
            lastestTrackedImage_guid = lastestTrackedImage.referenceImage.guid.ToString();
            mat_ImageAnchorfromARworld = lastestTrackedImage.transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0)) * mat_plane;
           
            completeHandler.SendMessage("CompleteScan");
        }

        //Realworld Origin에서의  마커 Pose.
        Matrix4x4 mat_ImageAnchorfromRealWorld = GetGlobalPoseMatrixfromTrackedImage(lastestTrackedImage_guid); //mat_palne : unity plane의 방향 보상.

        //Transform : Real world ->  AR world
        return mat_ImageAnchorfromARworld * mat_ImageAnchorfromRealWorld.inverse; ;
    }

    Matrix4x4 GetGlobalPoseMatrixfromTrackedImage(string guid)
    {
        Matrix4x4 res;

        lock (TrackerClient.Instance.lockImageAnchor)
        {
            if (TrackerClient.Instance.dicImageAnchor.ContainsKey(guid))
            {
                res = TrackerClient.Instance.dicImageAnchor[guid];

                //아래가 필요한 이유는, 실시간으로 서버로부터 imageAnchor 자세가 바뀔 때 이를 게임오브젝트에 반영하기 위함.
                if (dicImageAnchors.ContainsKey(guid))
                {
                    dicImageAnchors[guid].transform.localPosition = res.GetColumn(3);
                    dicImageAnchors[guid].transform.localRotation = res.rotation;
                    //Debug.Log(dicImageAnchors[guid].transform.position.ToString());
                    //Debug.Log(dicImageAnchors[guid].transform.rotation.ToString());
                }
            }
            else if (guid == "null")
            {
                res = Matrix4x4.identity;
            }
            else 
            {
                res = Matrix4x4.identity;
                Debug.Log("No registered image anchor.");
            }

        }
    
        return res;
    }

    Matrix4x4 GetInitialGlobalPoseMatrixfromSenser(float userheight)
    {
        // 필터링을 통해 카메라의 초기 position과 orientation을 추정.
        Vector3 pos = TerrainUtils.LatLonToWorldWithElevation(TerrainContainer.Instance, SensorController.Instance.latitude, SensorController.Instance.longitude);
        Vector4 colPos = new Vector4(pos.x, pos.y + userheight, pos.z, 1f);
        Matrix4x4 globalOrigin = Matrix4x4.Rotate(Quaternion.Euler(0f, SensorController.Instance.fHeading, 0f));
        globalOrigin.SetColumn(3, colPos);

        return globalOrigin;
    }

    public void StartScanMode(GameObject _completehandler)
    {
        Debug.Log("in StartScanMode");
        scanModeOn = true;
        this.completeHandler = _completehandler;
        arCamera.GetComponent<Camera>().cullingMask |=  (1 << LayerMask.NameToLayer("Image Anchor"));

    }
    
    public void CompleteScanMode()
    {
        arCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Image Anchor"));
        scanModeOn = false;
	}

    public void InitLocalization()
    {
        runTracking = false;

        arCamera.transform.localPosition = Vector3.zero;
        arCamera.transform.localRotation = Quaternion.identity;

        //Reset AR Sesstion
        arSession.Reset();
    }

    public void UpdateGlobalPositionManually(int dir)
    {
        float dist = 1f; //20cm
        Vector3 addedPos = Vector3.zero;
        Quaternion addRot = Quaternion.identity;
        switch(dir)
        {
            case 0:
                addedPos = Vector3.left * dist;
                break;
            case 1:
                addedPos = Vector3.right * dist;
                break;

            case 2:
                addedPos = Vector3.forward * dist;
                break;

            case 3:
                addedPos = Vector3.back * dist;
                break;

            case 4:
                addRot = Quaternion.Euler(0, -5, 0);
                break;
            case 5:
                addRot = Quaternion.Euler(0, 5, 0);
                break;

        }
        Vector3 gl_pos = GlobalARCameraInfo.Instance.globalPosition;
        Quaternion gl_rot = GlobalARCameraInfo.Instance.globalRotation;

        Matrix4x4 gl_mat = Matrix4x4.TRS(gl_pos, gl_rot, new Vector3(1, 1, 1));

        Matrix4x4 add_mat = Matrix4x4.TRS(addedPos, addRot, new Vector3(1, 1, 1));

        mat_Realworld2ARworld = arCamera.transform.localToWorldMatrix * add_mat.inverse * gl_mat.inverse;

        realworldTransform.position = mat_Realworld2ARworld.GetColumn(3);
        realworldTransform.rotation = mat_Realworld2ARworld.rotation;

        //Matrix4x4 positionMat = Matrix4x4.identity;
        //positionMat.SetColumn(3, new Vector4(addedPos.x, addedPos.y, addedPos.z, 1));

        //mat_Realworld2ARworld *= positionMat.inverse;


    }
    public IEnumerator WriteTrajectory(string filename, float delay)
    {
        while(true)
        {
            if (runTracking)
            {
                DateTime centuryBegin = new DateTime(2001, 1, 1);
                DateTime currentDate = DateTime.Now;

                long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
                TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);


                string time = elapsedSpan.TotalSeconds.ToString();
                string strline = string.Format("{0}:{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}"
                    , time, GlobalARCameraInfo.Instance.latitude
                    , GlobalARCameraInfo.Instance.longitude
                    , GlobalARCameraInfo.Instance.altitude
                    , GlobalARCameraInfo.Instance.globalPosition.x.ToString()
                    , GlobalARCameraInfo.Instance.globalPosition.y.ToString()
                    , GlobalARCameraInfo.Instance.globalPosition.z.ToString()
                    , GlobalARCameraInfo.Instance.globalRotation.x.ToString()
                    , GlobalARCameraInfo.Instance.globalRotation.y.ToString()
                    , GlobalARCameraInfo.Instance.globalRotation.z.ToString()
                    , GlobalARCameraInfo.Instance.globalRotation.w.ToString());

                TrajectoryWritor.WriteStringToFile(strline, filename);
            }
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator CheckIsIconVisible()
    {
        while (true)
        {
            foreach (Transform icon in arScenesParent)
            {
                if (Vector3.Distance(GlobalARCameraInfo.Instance.globalPosition, icon.localPosition) < active_distance)
                {
                    icon.gameObject.SetActive(true);
                }
                else
                {
                    icon.gameObject.SetActive(false);
                }
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
}

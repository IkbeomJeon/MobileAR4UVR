using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEngine.XR.ARSubsystems;


[InitializeOnLoad]
public class MasterClientWindow : EditorWindow
{
    public static EditorWindow wnd;

    //public string server_IP = "13.209.43.184"; //장윤
    //public string server_IP = "127.0.0.1";
    public string server_IP = "54.180.31.56"; //ARRC
    public int server_Port = 65432;

    // GUI
    private Vector2 scrollPos = Vector2.zero;
    bool bShowConnectionField = true;
    bool bShowImageAnchorsField = true;
    bool bVisualizeClients;
    bool bUpdate2ServerWithoutConfirm;

    Color colorMain = new Color(1, 1, 0, 0.5f);
    //Color colorOutline = Color.white;

    //float outlineWidth = 0.03f;
    //float alpha = 0.35f;

    [SerializeField]
    public XRReferenceImageLibrary referenceImageLibrary;
    public DictionaryOfStringAndGameObject dicImageAnchorsGO = new DictionaryOfStringAndGameObject();
    public DictionaryOfStringAndGameObject dicConnectedClientsGO = new DictionaryOfStringAndGameObject();

    [MenuItem("Tools/MasterClientWindow")]
    public static EditorWindow OpenWindow()
    {
        wnd = GetWindow<MasterClientWindow>(false, "Master Client Window");

        return wnd;
    }

    private void Awake()
    {
        Init();
        UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += SceneOpenedCallback;
    }
    void SceneOpenedCallback(UnityEngine.SceneManagement.Scene _scene, UnityEditor.SceneManagement.OpenSceneMode _mode)
    {
        Init();
    }

    void Init()
    {
        //Debug.Log("Init Call");
        MasterClient.Instance.Destroy();

        bShowConnectionField = true;
        bShowImageAnchorsField = true;
        bVisualizeClients = false;

        referenceImageLibrary = null;
        dicImageAnchorsGO.Clear();
        dicConnectedClientsGO.Clear();
    }
  
    public void Update()
    {
        //Debug.Log("Update Call.");
        //if (MasterClient.Instance.isNewData_ImageAnchor)

        if (MasterClient.Instance.isConnected)
            UpdateImageAnchorObjects();

        if (bVisualizeClients)
            VisualizeClient();

        if (bUpdate2ServerWithoutConfirm)
            Update2ServerImageAnchorPoses(false);

    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.BeginVertical(GUI.skin.box);
        bShowConnectionField = EditorGUILayout.Foldout(bShowConnectionField, "Connection");
        if (bShowConnectionField)
        {
            if (!MasterClient.Instance.isConnected)
            {
                //string[] path = EditorApplication.currentScene.Split(char.Parse("/"));
                //sceneName = EditorGUILayout.TextField("Scene ID", path[path.Length - 1]); 
                server_IP = EditorGUILayout.TextField("Server IP", server_IP);
                server_Port = EditorGUILayout.IntField("Port", server_Port);

                if (GUILayout.Button("Connect to Server"))
                {
                    Connect();
                }
            }

            else
            {
                if (GUILayout.Button("Disconnect"))
                {
                    Disconnect();
                }

                if (GUILayout.Button("Get ImageAnchors from server"))
                {
                    //RequestAllImageAnchorPoses();
                    if (EditorUtility.DisplayDialog("Confirm to request", "Confirm to request?", "Yes", "No"))
                        RequestImageAnchorPoseinDictionary();
                }

                if (GUILayout.Button("Set ImageAnchors to server"))
                {
                    if (EditorUtility.DisplayDialog("Confirm to update", "Confirm to update?", "Yes", "No"))
                    {
                        Update2ServerImageAnchorPoses(true);
                    }

                }
                bUpdate2ServerWithoutConfirm = EditorGUILayout.Toggle("Update image anchor without confirm", bUpdate2ServerWithoutConfirm);
                bVisualizeClients = EditorGUILayout.Toggle("Visualize connected clients", bVisualizeClients);

                if (!bVisualizeClients)
                    ClearTrackerClientObjects();
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        bShowImageAnchorsField = EditorGUILayout.Foldout(bShowImageAnchorsField, "Image Anchors");

        if (bShowImageAnchorsField)
        {
            // Reference Image Library 관련 GUI
            ScriptableObject target = this;
            SerializedObject serializedObject = new SerializedObject(target);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("referenceImageLibrary"), true); // True means show children

            //Color
            colorMain = EditorGUILayout.ColorField("Main Color", colorMain);
            //colorOutline = EditorGUILayout.ColorField("Outline Color", colorOutline);
            //alpha = EditorGUILayout.FloatField("Alpha", alpha);
            //outlineWidth = EditorGUILayout.FloatField("Outline width", outlineWidth);
            serializedObject.ApplyModifiedProperties(); // Remember to apply modified properties
            serializedObject.Update();

            if (EditorGUI.EndChangeCheck())
            {
                //CreateImageAnchorObjects();
                //RequestImageAnchorPoseinDictionary();
            }

            SerializedProperty stringsProperty = serializedObject.FindProperty("dicImageAnchorsGO");
            EditorGUILayout.PropertyField(stringsProperty, true);

            serializedObject.ApplyModifiedProperties(); // Remember to apply modified properties
            serializedObject.Update();


        }

        if (GUILayout.Button("Apply"))
        {
            CreateImageAnchorObjects();
        }
        EditorGUILayout.EndVertical();
        if (GUILayout.Button("Test Code"))
        {

            //Matrix4x4 mat_plane = new Matrix4x4();
            //mat_plane.SetColumn(0, new Vector4(-1, 0, 0, 0));
            //mat_plane.SetColumn(1, new Vector4(0, 0, -1, 0));
            //mat_plane.SetColumn(2, new Vector4(0, -1, 0, 0));

            //mat_plane.SetColumn(3, new Vector4(0, 0, 0, 1));
            //GameObject go = GameObject.Find("Gwanduk");
            //Matrix4x4 mat = go.transform.localToWorldMatrix;

            //GameObject newgo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //newgo.transform.position = new Vector3(10, 10, 0);

            //Matrix4x4 res = mat * mat_plane.inverse * newgo.transform.localToWorldMatrix;

            //newgo.transform.position = res.GetColumn(3);
            //newgo.transform.rotation = res.rotation;

            RequestAllImageAnchorPoses();

        }
        GUILayout.EndScrollView();
    }
  
    void Connect()
    {
        MasterClient.Instance.Connect(server_IP, server_Port);
        //RequestImageAnchorPoseinDictionary();
    }

    void Disconnect()
    {
        MasterClient.Instance.Disconnect();

        ClearTrackerClientObjects();
        bVisualizeClients = false;
    }

    void VisualizeClient()
    {
   
        lock (MasterClient.Instance.lockDataPackage)
        {
            foreach (KeyValuePair<string, Matrix4x4> item in MasterClient.Instance.dicClientInfo)
            {
                GameObject go;
                if (!dicConnectedClientsGO.ContainsKey(item.Key))
                {
                    GameObject world = GameObject.Find("Real World");
                    if (world == null)
                        world = new GameObject("Real World");

                    go = Instantiate(Resources.Load("MasterClient/Tracker Client") as GameObject);
                    //go.transform.parent = world.transform;
                    go.name = go.name + "_" + item.Key;
                    dicConnectedClientsGO.Add(item.Key, go);
                }
                else
                {
                    go = dicConnectedClientsGO[item.Key] as GameObject;
                }

                go.transform.localPosition = item.Value.GetColumn(3);
                go.transform.localRotation = item.Value.rotation;
            }
        }
    }

    void RequestImageAnchorPoseinDictionary()
    {
        if (MasterClient.Instance.isConnected)
        {
            foreach (KeyValuePair<string, GameObject> item in dicImageAnchorsGO)
            {
                string guid = item.Key;
                MasterClient.Instance.RequestImageAnchorPose(guid);
            }
        }
        else
            Debug.LogError("No connection to server.");
    }

    void RequestAllImageAnchorPoses()
    {
        if (MasterClient.Instance.isConnected)
        {
            MasterClient.Instance.RequestAllImageAnchorPoses();
        }
        else
            Debug.LogError("No connection to server.");
    }

    void UpdateImageAnchorObjects()
    {
        lock (MasterClient.Instance.lockImageAnchor)
        {
            //if (MasterClient.Instance.isNewData_ImageAnchor)
            {
                // 씬에 있는 오브젝트들 검색.
                foreach (string guid in dicImageAnchorsGO.Keys)
                {
                    //string guid = imgAO.GetComponent<ARRCImageAnchorProperty>().guid;
                    //string name = dicImageAnchorsGO[guid].GetComponent<ARRCImageAnchorProperty>().name;

                    //서버에도 있으면 서버 값으로 업데이트.
                    if (MasterClient.Instance.dicImageAnchor.ContainsKey(guid))
                    {
                        dicImageAnchorsGO[guid].transform.localPosition = MasterClient.Instance.dicImageAnchor[guid].GetColumn(3);
                        dicImageAnchorsGO[guid].transform.localRotation = MasterClient.Instance.dicImageAnchor[guid].rotation;
                        MasterClient.Instance.dicImageAnchor.Remove(guid);
                    }
                    //서버에 없으면 초기화 필요 에러 출력.
                    else
                    {
                        //서버에서 no found protocol 필요함.
                        //Debug.LogError("Image Anchor " + name + "(" + guid + ") : is not registered with the server. It's initialized to idendity.");
                    }
                }
                //MasterClient.Instance.isNewData_ImageAnchor = false;
            }
        }

    }

    void RegisterImageAnchorGameObject(string guid, string name, Vector2 size, Texture2D texture)
    {
        GameObject world = GameObject.Find("Real World");
        if(world == null)
            world = new GameObject("Real World");

        GameObject parent = GameObject.Find("Image Anchors");

        if (parent == null)
        {
            parent = new GameObject("Image Anchors");
            parent.transform.parent = world.transform;
            parent.transform.localPosition = Vector3.zero;
            parent.transform.localRotation = Quaternion.identity;
        }
        

        GameObject newImageAnchor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        newImageAnchor.name = name;
        newImageAnchor.tag = "Image Anchor";
        newImageAnchor.layer = LayerMask.NameToLayer("Image Anchor");
        newImageAnchor.transform.localScale = new Vector3(size.x / 10, 1, size.y / 10);
        newImageAnchor.transform.parent = parent.transform;

        Renderer rend = newImageAnchor.GetComponent<MeshRenderer>();
        rend.sharedMaterial = new Material(Shader.Find("Somian/Unlit/Transparent"));
        rend.sharedMaterial.SetTexture("_MainTex", texture);
        ChangeMaterialProperty(rend);

        ARRCImageAnchorProperty propertyScript = newImageAnchor.AddComponent<ARRCImageAnchorProperty>();
        propertyScript.guid = guid;
        propertyScript.size = size;

        dicImageAnchorsGO[guid] = newImageAnchor;
    }
    void ChangeMaterialProperty(Renderer rend)
    {
        rend.sharedMaterial.color = colorMain;
        //rend.sharedMaterial.SetFloat("_Alpha", alpha);
        //rend.sharedMaterial.SetColor("_OutlineColor", colorOutline);
        //rend.sharedMaterial.SetFloat("_Outline", outlineWidth);
    }
    void CreateImageAnchorObjects()
    {
        dicImageAnchorsGO.Clear();

        //Parent

        if (referenceImageLibrary)
        {

            GameObject[] imageAnchorObjects = GameObject.FindGameObjectsWithTag("Image Anchor");
            List<XRReferenceImage>.Enumerator em = referenceImageLibrary.GetEnumerator();

            //Library에 없는 게임오브젝트는 새로 생성.
            em = referenceImageLibrary.GetEnumerator();
            while (em.MoveNext())
            {
                XRReferenceImage val = em.Current;
                string guid_library = val.guid.ToString();

                bool bFind = false;
                foreach (GameObject go in imageAnchorObjects)
                {
                    if (guid_library == go.GetComponent<ARRCImageAnchorProperty>().guid)
                    {
                        bFind = true;
                        dicImageAnchorsGO[guid_library] = go;
                        ChangeMaterialProperty(go.GetComponent<MeshRenderer>());
                    }
                }

                if (!bFind)
                    RegisterImageAnchorGameObject(guid_library, val.name, val.size, val.texture);
            }
        }

    }

    void ClearImageAnchorObjects()
    {

        GameObject[] imageAnchorObjects = GameObject.FindGameObjectsWithTag("Image Anchor");
        foreach (GameObject imgAO in imageAnchorObjects)
        {
            DestroyImmediate(imgAO);
        }
    }

    void ClearTrackerClientObjects()
    {
        foreach (KeyValuePair<string, GameObject> item in dicConnectedClientsGO)
            DestroyImmediate(item.Value);

        dicConnectedClientsGO.Clear();
    }

    void Update2ServerImageAnchorPoses(bool showLog)
    {
        //리펙토링 : 코루틴으로 보내자?
        foreach (KeyValuePair<string, GameObject> item in dicImageAnchorsGO)
        {
            string guid = item.Key;
            GameObject go = item.Value;

            TransformData imageAnchorData = new TransformData();
            //pose.Rotate(pose.transform.)

            imageAnchorData.imageAnchorGUID = guid;
            imageAnchorData.imageAnchorPosition = go.transform.position;
            imageAnchorData.imageAnchorRotation = go.transform.rotation;

            if (MasterClient.Instance.isConnected)
            {
                MasterClient.Instance.RequestUpdateImageAnchor(imageAnchorData);
                string str = string.Format("{0}({1}) is requsted to update.", go.name, guid);
                if(showLog)
                    Debug.Log(str);
            }


            else
                Debug.LogError("No connection to server.");
        }
    }
}

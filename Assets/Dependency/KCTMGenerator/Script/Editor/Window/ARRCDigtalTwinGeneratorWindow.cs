using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Xml;

namespace ARRC_DigitalTwin_Generator
{

    public class ARRCDigtalTwinGeneratorWindow : UnityEditor.EditorWindow
    {
        //preference
        TerrainElevationProvider elevationProvider = TerrainElevationProvider.VWorld;
        TerrainTextureProvider textureProvider = TerrainTextureProvider.VWorld;
        BuldingProvider buildingProvider = BuldingProvider.VWorld;
        StreetViewProvider streetviewProvider = StreetViewProvider.Google;

        public bool bGenerateTerrains = false;
        public bool bGenerateBuilding = false;
        public bool bGenerateStreetView = false;
        public bool bGenerateKCTMMetadata = false;

        //double latFrom;
        //double lonFrom;
        //double latTo;
        //double lonTo;

        double latFrom = 33.516498;
        double lonFrom = 126.51999;
        double latTo = 33.51245;
        double lonTo = 126.525;

        //GUI
        bool bShowCoordinates = true;
        bool bShowTerrains = true;
        bool bShowBuilding = true;
        bool bShowStreetView = true;
        bool bShowKCTMMetadata = true;
        bool bShowPointCloud = true;

        //path
        string plyPath;

        private Vector2 scrollPos = Vector2.zero;

        bool isCapturing;
        static ARRCDigtalTwinGeneratorWindow wnd;

        float progress
        {
            get { return LiveStreetVRPhaseManager.Instance.activePhase.phaseProgress; }
        }
        float totalSize
        {
            get { return LiveStreetVRPhaseManager.Instance.activePhase.totalSize; }
        }

        public static ARRCDigtalTwinGeneratorWindow Instance
        {
            get
            {
                if(wnd)
                    return wnd;
                else
                {
                    wnd = GetWindow<ARRCDigtalTwinGeneratorWindow>(false, "ARRC Generator");
                    return wnd;
                }
            }
        }

        [MenuItem("Tools/DigitalTwinGenerator")]
        public static ARRCDigtalTwinGeneratorWindow OpenWindow()
        {
            wnd = GetWindow<ARRCDigtalTwinGeneratorWindow>(false, "ARRC Generator");
            return wnd;
        }

        void OnGUI()
        {

            if (!isCapturing)
            {
                Toolbar();
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                ////not yet
                EditorGUILayout.BeginVertical(GUI.skin.box);
                bShowCoordinates = EditorGUILayout.Foldout(bShowCoordinates, "Decimal coordinates");
                if (bShowCoordinates) Coordinates();
                EditorGUILayout.EndVertical();

                /*
                EditorGUILayout.BeginVertical(GUI.skin.box);
                bShowTerrains = EditorGUILayout.Foldout(bShowTerrains, "Terrain");
                if (bShowTerrains) TerrainGUI();
                EditorGUILayout.EndVertical();
                */

                EditorGUILayout.BeginVertical(GUI.skin.box);
                bShowBuilding = EditorGUILayout.Foldout(bShowBuilding, "Building");
                if (bShowBuilding) BuildingGUI();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                bShowStreetView = EditorGUILayout.Foldout(bShowStreetView, "StreetView");
                if (bShowStreetView) StreetViewGUI();
                EditorGUILayout.EndVertical();

                
                EditorGUILayout.BeginVertical(GUI.skin.box);
                bShowKCTMMetadata = EditorGUILayout.Foldout(bShowKCTMMetadata, "KCTM Metadata");
                if (bShowKCTMMetadata) KCTMMetadataGUI();
                EditorGUILayout.EndVertical();


                EditorGUILayout.BeginVertical(GUI.skin.box);
                bShowPointCloud = EditorGUILayout.Foldout(bShowPointCloud, "Point Cloud");
                PointCloudGUI();
                EditorGUILayout.EndVertical();

                GUILayout.EndScrollView();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Apply"))
                    StartCapture();
                        
                GUILayout.EndHorizontal();

            }
            else
                OnGenerate();


        }

        void Update()
        {
            if (LiveStreetVRPhaseManager.Instance.activePhase == null) return;

            try
            {
                if (LiveStreetVRPhaseManager.Instance.activePhase.isComplete)
                {
                    bool noNextPhase = LiveStreetVRPhaseManager.Instance.StartNextPhase();
                    if (noNextPhase)
                        Dispose();
                }

                else
                    LiveStreetVRPhaseManager.Instance.activePhase.Enter();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
                Dispose();
            }
        }

        void OnDestroy()
        {
            isCapturing = false;
            wnd = null;
        }

        void OnEnable()
        {
            wnd = this;
        }

        void StartCapture()
        {

            //궁동
            //float LON_A = 127.34889999999996f;
            //float LAT_A = 36.36312f;
            //float LON_B = 127.35126396560668f;
            //float LAT_B = 36.36072463865601f;

            //float LON_A = 127.35010242530825f;
            //float LAT_A = 36.36289750704699f;
            //float LON_B = 127.35119562400814f;
            //float LAT_B = 36.36215787323677f;

            //관덕정
            //double LAT_A = 33.515014847067846;
            //double LON_A = 126.52084843292232;
            //double LAT_B = 33.51296418982448;
            //double LON_B = 126.52331968688964;

            //경복궁
            //double LAT_A = 37.5776476;
            //double LON_A = 126.974575;
            //double LAT_B = 37.5758232;
            //double LON_B = 126.9775363;

            //둔산동 갤러리아
            //double LAT_A = 36.3561183;
            //double LON_A = 127.3717533;
            //double LAT_B = 36.3512583;
            //double LON_B = 127.3774753;

            //TerrainUtils.GetCoord(LAT_A, LON_A, LAT_B, LON_B, out latFrom, out lonFrom, out latTo, out lonTo);

            //다운로드 리스트를 downloadManager에 등록.
            if (!TerrainUtils.CheckCoordValidation(latFrom, lonFrom, latTo, lonTo)) return;

            isCapturing = true;

            int zoom = 3;

            //terrain 좌표계 생성 생성.
            TerrainContainer.Instance.SetCoordinates(latFrom, lonFrom, latTo, lonTo);

            //Phase를 구성.
            LiveStreetVRPhaseManager.Instance.Init(latFrom, lonFrom, latTo, lonTo, zoom, wnd);

            //Phase 시작.
            LiveStreetVRPhaseManager.Instance.StartNextPhase();

        }


        void Dispose()
        {
            isCapturing = false;
            //RealWorldTerrainImporter.showMessage = true;

            //if (thread != null)
            //{
            //    thread.Abort();
            //    thread = null;
            //}

            LiveStreetVRPhaseManager.Instance.DisposeAllPhase();
            EditorUtility.UnloadUnusedAssetsImmediate();

            wnd.Repaint();
            GC.Collect();
        }

        void OnGenerate()
        {
            int completed = Mathf.FloorToInt(totalSize * progress);
            GUILayout.Label(LiveStreetVRPhaseManager.Instance.activePhase.title + " (" + completed + " of " + totalSize + " mb)");

            Rect r = EditorGUILayout.BeginVertical();
            string strProgress = Mathf.FloorToInt(progress * 100f).ToString();
            EditorGUI.ProgressBar(r, progress, strProgress + "%");
            GUILayout.Space(16);
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Cancel"))
                Dispose();

            GUILayout.Label("Warning: Keep this window open.");
        }

        void Coordinates()
        {
            //EditorGUILayout.TextField("Title", prefs.title);
            GUILayout.Space(10);

            GUILayout.Label("Top-Left");
            EditorGUI.indentLevel++;
            latFrom = EditorGUILayout.DoubleField("Latitude", latFrom);
            lonFrom = EditorGUILayout.DoubleField("Longitude", lonFrom);
            EditorGUI.indentLevel--;
            GUILayout.Space(10);

            GUILayout.Label("Bottom-Right");
            EditorGUI.indentLevel++;
            latTo = EditorGUILayout.DoubleField("Latitude", latTo);
            lonTo = EditorGUILayout.DoubleField("Longitude", lonTo);

            EditorGUI.indentLevel--;
            GUILayout.Space(10);

            GUI.SetNextControlName("InsertCoordsButton");
            if (GUILayout.Button("Insert the coordinates from the clipboard")) InsertCoords();
            if (GUILayout.Button("Run the helper")) RunHelper();
            //if (prefs.resultType == RealWorldTerrainResultType.terrain && GUILayout.Button("Get the best settings for the specified coordinates")) RealWorldTerrainSettingsGeneratorWindow.OpenWindow();

            GUILayout.Space(10);
        }

        void RunHelper()
        {
            string helperPath = "file://" + Directory.GetFiles(Application.dataPath, "Helper_StreetView.html", SearchOption.AllDirectories)[0].Replace('\\', '/');
            if (Application.platform == RuntimePlatform.OSXEditor) helperPath = helperPath.Replace(" ", "%20");
            //prefs.Save();
            Application.OpenURL(helperPath);
        }

        public void InsertCoords()
        {
            GUI.FocusControl("InsertCoordsButton");
            string nodeStr = EditorGUIUtility.systemCopyBuffer;
            if (string.IsNullOrEmpty(nodeStr)) return;

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(nodeStr);
                XmlNode fnode = doc.FirstChild;
                if (fnode.Name == "Coords" && fnode.Attributes != null)
                {
                    lonFrom = XMLExt.GetAttribute<float>(fnode, "tlx");
                    latFrom = XMLExt.GetAttribute<float>(fnode, "tly");
                    lonTo = XMLExt.GetAttribute<float>(fnode, "brx");
                    latTo = XMLExt.GetAttribute<float>(fnode, "bry");
                    //prefs.Save();
                }
            }
            catch { }
        }

        void LatLongToMercat(double x, double y, out double mx, out double my)
        {
            const double DEG2RAD = Math.PI / 180;
            double sy = Math.Sin(y * DEG2RAD);
            mx = (x + 180) / 360;
            my = 0.5 - Math.Log((1 + sy) / (1 - sy)) / (Math.PI * 4);
        }

        void Toolbar()
        {
            GUIStyle buttonStyle = new GUIStyle(EditorStyles.toolbarButton);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("History", buttonStyle, GUILayout.ExpandWidth(false)))
            {
                //RealWorldTerrainHistoryWindow.OpenWindow();
            }

            // if (RealWorldTerrainUpdaterWindow.hasNewVersion)
            {
                //Color defColor = GUI.backgroundColor;
                //GUI.backgroundColor = new Color(1, 0.5f, 0.5f);
                //if (GUILayout.Button("New version available!!! Click here to update.", buttonStyle))
                //{
                //    wnd.Close();
                //    RealWorldTerrainUpdaterWindow.OpenWindow();
                //}
                //GUI.backgroundColor = defColor;
            }
            //else
            GUILayout.Label("", buttonStyle);

            if (GUILayout.Button("Settings", buttonStyle, GUILayout.ExpandWidth(false)))
            {
                //RealWorldTerrainSettingsWindow.OpenWindow();
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        void TerrainGUI()
        {
            bGenerateTerrains = EditorGUILayout.Toggle("Generate Terrain", bGenerateTerrains);
            elevationProvider = (TerrainElevationProvider)EditorGUILayout.EnumPopup("Elevation Provider", elevationProvider);
            textureProvider = (TerrainTextureProvider)EditorGUILayout.EnumPopup("Texture Provider", textureProvider);
        }
        void BuildingGUI()
        {
            bGenerateBuilding = EditorGUILayout.Toggle("Generate Building", bGenerateBuilding);
            buildingProvider = (BuldingProvider)EditorGUILayout.EnumPopup("Building Provider", buildingProvider);
        }
    
        void StreetViewGUI()
        {
            bGenerateStreetView = EditorGUILayout.Toggle("Generate StreetView", bGenerateStreetView);
            streetviewProvider = (StreetViewProvider)EditorGUILayout.EnumPopup("StreetView Provider", streetviewProvider);
        }
        void KCTMMetadataGUI()
        {
            bGenerateKCTMMetadata = EditorGUILayout.Toggle("Generate KCTM metadata", bGenerateKCTMMetadata);
        }

        void PointCloudGUI()
        {
            if (GUILayout.Button("Select .ply file"))
            {
                //EditorUtility.DisplayDialog("Select Texture", "You must select a texture first!", "OK");
                string path = EditorUtility.OpenFilePanel("Select .ply file", Application.streamingAssetsPath, "ply");
                //int LastIndex = path.LastIndexOf ("/");
                //mTotalPath = path;
                plyPath = path;
                
            }
            if (plyPath != "")
            {
                EditorGUILayout.LabelField("filepath path : " + plyPath, EditorStyles.boldLabel);


                if (GUILayout.Button("Generate PointCloud"))
                {
                    GameObject parent = GameObject.Find("Point Cloud");
                    if (parent == null)
                        parent = new GameObject("Point Cloud");

                    PointCloudGeneratorWarpper.Instance.BuildCloud(plyPath, parent.transform);
                }
            }
        }
    }
}

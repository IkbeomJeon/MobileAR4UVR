using PrimitivesPro.GameObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ARRC_DigitalTwin_Generator
{
    class StreetViewGenerator : ARRCGenerator
    {
        private static string cacheFolder = Utils.cacheFolder + "/StreetView";
        private static string targetFolder = ARRC_DigitalTwin_Generator.Utils.dataFolder;

        //street view xml리스트 관련.
        private string filepath_xmlList;
        private const float STEP_SIZE = 0.00005f; //지도에서 StreetView 검색 간격.

        //streetivew image 관련.
        private static List<StreetViewInfo> streetViewInfoList;

        private const int tileSize = 512;
        private const int tileSize_real = 416;

        private int zoom;
        private int num_tiles_X, num_tiles_X_request; //이론적인 tile수, 실제 요청에 사용되는 타일 수
        private int num_tiles_Y;
        private int width_texture;
        private int height_texture;

        private int width_tempTexture;
        private int height_tempTexture;

        //<for singleton>
        private static StreetViewGenerator instance;
        public static StreetViewGenerator GetInstance()
        {
            if (instance == null)
            {
                instance = new StreetViewGenerator();
            }
            return instance;

        }
        public static void ResetInstance()
        {
            instance = null;
        }
        //<for singleton>

        public IEnumerator MakeDownloadList_StreetViewXMLList(double latFrom, double lonFrom, double latTo, double lonTo, int zoom)
        {
            currentState = "Making download list for XMLs data...";
            isComplete = false;

            List<DownloadItem> downloadItemList_streetviewXML = new List<DownloadItem>();

            string subFolder_xml = Path.Combine(cacheFolder, "XML");

            if (!Directory.Exists(subFolder_xml)) Directory.CreateDirectory(subFolder_xml);

            //string filename = DateTime.Now.ToString("yyMMddHHmmss") + ".txt";
            //string filename = "from" + from_coord.ToString() + "to" + to_coord.ToString() + ".txt";
            string filename = "from(" + lonFrom.ToString() + ", "+latFrom.ToString() + ")to(" + lonTo.ToString()+", "+latTo.ToString() + ").txt";
            filepath_xmlList = Path.Combine(subFolder_xml, filename);
            if (!File.Exists(filepath_xmlList))
            {
                //StreetView List
                double total_progress = (latFrom - latTo);
                int count_progress = 0;
                for (double lat = latFrom; lat >= latTo; lat -= STEP_SIZE)
                {
                    if (isComplete)
                        break;

                    progress = (count_progress++ * STEP_SIZE) / (float)total_progress;

                    for (double lon = lonFrom; lon <= lonTo; lon += STEP_SIZE)
                    {
                        string url = string.Format("http://maps.google.com/cbk?output=xml&ll={0},{1}&dm=1", lat, lon);
                        downloadItemList_streetviewXML.Add(new DownloadItem_StreetViewInfo(url)
                        {
                            filename = filepath_xmlList,
                            averageSize = Utils.AVERAGE_STREETVIEW_XML_SIZE
                        });
                    }

                    yield return new WaitForFixedUpdate();
                }
            }
            items = downloadItemList_streetviewXML;
            isComplete = true;
        }

        public IEnumerator MakeDownloadList_StreetViewImage(int zoom)
        {
            currentState = "Making download list for StreetView Images...";

            isComplete = false;

            this.zoom = zoom;

            List<DownloadItem> downloadItemList_streetviewImage = new List<DownloadItem>();

            streetViewInfoList = ReadStreetViewInfoListfromFile(filepath_xmlList);

            string subFolder_Image = Path.Combine(cacheFolder + "/Images", zoom.ToString()); //이미지 저장 폴더
            if (!Directory.Exists(subFolder_Image)) Directory.CreateDirectory(subFolder_Image);

            num_tiles_X = (int)Math.Pow(2, (double)zoom);
            num_tiles_Y = (int)Math.Pow(2, (double)zoom - 1);

            width_texture = tileSize_real * num_tiles_X;
            height_texture = tileSize_real * num_tiles_Y;

            width_tempTexture = tileSize * num_tiles_X;
            height_tempTexture = tileSize * num_tiles_Y;

            num_tiles_X_request = (zoom == 3) ? num_tiles_X - 1 : num_tiles_X;

            float count_progress = 0;
            foreach (StreetViewInfo svinfo in streetViewInfoList)
            {
                if (isComplete)
                    break;

                progress = (float)(count_progress++ / streetViewInfoList.Count);

                for (int y = 0; y < num_tiles_Y; y++)
                {
                    for (int x = 0; x < num_tiles_X_request; x++)
                    {
                        int index = y * num_tiles_X_request + x;
                        //string url = "http://geo0.ggpht.com/cbk?cb_client=maps_sv.tactile&authuser=0&hl=en&panoid=" + panoid + "&output=tile&x=" + x + "&y=" + y + "&zoom=" + zoom + "&nbt&fover=2";
                        string url = string.Format("http://maps.google.com/cbk?output=tile&panoid={0}&zoom={1}&x={2}&y={3}", svinfo.panoid, zoom, x, y);
                        string filepath = Path.Combine(subFolder_Image, svinfo.panoid + x.ToString() + "_" + y.ToString() + ".png");

                        svinfo.info_partImage.Add(index, filepath); //partimage의 인덱스와 filepath 저장.

                        if (!File.Exists(filepath))
                        {
                            downloadItemList_streetviewImage.Add(new DownloadItemUnityWebRequest(url)
                            {
                                filename = filepath,
                                averageSize = Utils.AVERAGE_STREETVIEW_SIZE
                            });
                        }


                    }
                }
                yield return new WaitForFixedUpdate();
            }
            items = downloadItemList_streetviewImage;
            isComplete = true;
        }

        public IEnumerator GenerateStreetViews()
        {
            currentState = "Generating gameobjects for streetview images...";

            //Transform parent = TerrainContainer.container
            isComplete = false;
            GameObject parent_Images = new GameObject("SteetViews");
            

            //local disk에 파일 쓰기.
            for (int i = 0; i < streetViewInfoList.Count; i++)
            {
                if (isComplete)
                    break;

                progress = (float)i / streetViewInfoList.Count;

                WriteStreetViewImage(streetViewInfoList[i], i);

                yield return new WaitForFixedUpdate();
            }
            UnityEditor.AssetDatabase.Refresh();

            List<GameObject> imgObjList = new List<GameObject>();

            //disk에 저장된 이미지를 로드해서 게임오브젝트 생성
            for (int i = 0; i < streetViewInfoList.Count; i++)
            {
                GameObject objImg = GenerateImageObject(streetViewInfoList[i], i);
                GameObject objCam = GenerateCameraObject(objImg.transform);

                objImg.transform.parent = parent_Images.transform;
                objCam.transform.parent = objImg.transform;

                objImg.GetComponent<Renderer>().enabled = false;
                objImg.AddComponent<StreetViewController>().panoCam = objCam;

                imgObjList.Add(objImg);
            }
            foreach(GameObject imgObj in imgObjList)
                imgObj.GetComponent<StreetViewController>().imgObjList = imgObjList;

            ResizeChildObject comp = parent_Images.AddComponent<ResizeChildObject>();
            comp.imageObjectSize = 1;

            // elevation 오차 
            parent_Images.transform.Translate(0, -18.36f, 0); 
            isComplete = true;
        }

        public List<StreetViewInfo> ReadStreetViewInfoListfromFile(string _filepath_xmlList)
        {
            HashSet<string> mySet = new HashSet<string>();
            List<StreetViewInfo> svInfoList = new List<StreetViewInfo>();

            string[] lines = File.ReadAllLines(_filepath_xmlList);

            foreach (string line in lines)
            {
                string[] data = line.Split(',');

                string panoid = data[0];
                string image_date = data[1];
                string lat = data[2];
                string lon = data[3];
                string original_lat = data[4];
                string original_lon = data[5];
                string elevation_wgs84_m = data[6];
                string pano_yaw_deg = data[7];
                string tilt_yaw_deg = data[8];
                string tilt_pitch_deg = data[9];

                if (!mySet.Contains(panoid))
                {
                    mySet.Add(panoid);
                    svInfoList.Add(new StreetViewInfo(panoid, lat, lon, image_date, elevation_wgs84_m, pano_yaw_deg, tilt_yaw_deg, tilt_pitch_deg));
                }
            }
            return svInfoList;

        }

        private void WriteStreetViewImage(StreetViewInfo svInfo, int idx)
        {
            string textureFilePath
              = Path.Combine(targetFolder, "zoom_" + zoom + "/" + svInfo.panoid + ".png");

            svInfo.savedTextureFilePath = textureFilePath;

            FileInfo info = new FileInfo(textureFilePath);

            if (info.Exists) return;

            if (!info.Directory.Exists)
                info.Directory.Create();

            Texture2D texture = new Texture2D(width_texture, height_texture, TextureFormat.RGB24, false);
            Texture2D tempTexture = new Texture2D(width_tempTexture, height_tempTexture, TextureFormat.RGB24, false);
            Texture2D tempPartTexture = new Texture2D(tileSize, tileSize);

            foreach (KeyValuePair<int, string> partImage in svInfo.info_partImage)
            {
                int index = partImage.Key;
                int y = index / num_tiles_X_request;
                int x = index % num_tiles_X_request;

                tempPartTexture.wrapMode = TextureWrapMode.Clamp;
                tempPartTexture.LoadImage(File.ReadAllBytes(partImage.Value));

                if (tempPartTexture != null)
                {
                    tempTexture.SetPixels(x * tileSize, (num_tiles_Y - y - 1) * tileSize, tileSize, tileSize, tempPartTexture.GetPixels());
                }
            }

            texture.SetPixels(tempTexture.GetPixels(0, (tileSize - tileSize_real) * num_tiles_Y, width_texture, height_texture));
            texture.Apply();

            File.WriteAllBytes(textureFilePath, texture.EncodeToPNG());

        }

        private GameObject GenerateImageObject(StreetViewInfo svInfo, int idx)
        {
            string assetPath = "Assets" + svInfo.savedTextureFilePath.Split(new string[] { Application.dataPath }, StringSplitOptions.None).Last();
            //int idx  = textureFilename.LastIndexOfAny("Assets".ToCharArray());
            //string path =textureFilename.Substring(idx);
            Texture2D texture = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D));


            double lat = double.Parse(svInfo.lat);
            double ele = double.Parse(svInfo.elevation_wgs84_m);
            double lon = double.Parse(svInfo.lon);

            //Vector3 pos = TerrainUtils.LatLonToWorldWithElevation(new Vector3(lon, ele, lat));
            Vector3 pos = TerrainUtils.LatLonToWorldWithElevation(TerrainContainer.Instance, lat, lon);
            float yaw = float.Parse(svInfo.pano_yaw_deg);
            float pitch = float.Parse(svInfo.tilt_pitch_deg);

            string name = "StreetView_" + svInfo.panoid;    
            
            //GameObject newObj = Editor360VRSystem.CreateImageObject(CAMERA_TYPE.Panorama, texture, name, pos, Quaternion.Euler(0, 0, 0));
            GameObject newObj = CreateIcosahedronImageSphere(name, texture, 5, 4, pos, Quaternion.Euler(0, 0, 0));
            //newObj.layer = LayerMask.NameToLayer("Image");

            newObj.transform.Rotate(new Vector3(0, yaw, 0), Space.World);
            newObj.transform.Rotate(new Vector3(pitch, 0, 0), Space.World);

            newObj.AddComponent<StreetViewInfo>();
            newObj.GetComponent<StreetViewInfo>().panoid = svInfo.panoid;
            newObj.GetComponent<StreetViewInfo>().image_date = svInfo.image_date;
            newObj.GetComponent<StreetViewInfo>().lat = svInfo.lat;
            newObj.GetComponent<StreetViewInfo>().lon = svInfo.lon;
            newObj.GetComponent<StreetViewInfo>().elevation_wgs84_m = svInfo.elevation_wgs84_m;
            newObj.GetComponent<StreetViewInfo>().pano_yaw_deg = svInfo.pano_yaw_deg;
            newObj.GetComponent<StreetViewInfo>().tilt_yaw_deg = svInfo.tilt_yaw_deg;
            newObj.GetComponent<StreetViewInfo>().tilt_pitch_deg = svInfo.tilt_pitch_deg;

            return newObj;
        }

        public static GameObject CreateIcosahedronImageSphere(string filename, Texture2D tex, float radius, int subdivision, Vector3 pos, Quaternion rot, Transform parent = null)
        {
            GameObject go = new GameObject();

            if (go) go.transform.parent = parent;
            go.name = filename;
            go.AddComponent<MeshFilter>();

            Renderer rend = go.AddComponent<MeshRenderer>();
            rend.material = new Material(Shader.Find("SphericalMapping/UnlitTexture"));
            rend.sharedMaterial.mainTexture = tex;

            go.AddComponent<GeoSphere>();
            go.GetComponent<GeoSphere>().baseType = PrimitivesPro.Primitives.GeoSpherePrimitive.BaseType.Icosahedron;
            go.GetComponent<GeoSphere>().pivotPosition = PrimitivesPro.Primitives.PivotPosition.Center;
            go.GetComponent<GeoSphere>().normalsType = PrimitivesPro.Primitives.NormalsType.Face;
            go.GetComponent<GeoSphere>().radius = radius;
            go.GetComponent<GeoSphere>().subdivision = subdivision;
            go.GetComponent<GeoSphere>().GenerateGeometry();

            go.transform.position = pos;
            go.transform.rotation = rot;

            return go;
        }

        public static GameObject GenerateCameraObject(Transform trans)
        {
            string name = "Pano Camera";
            GameObject newCam = new GameObject(name);
            newCam.transform.position = trans.position;
            newCam.transform.rotation = trans.rotation;
            //newCam.AddComponent<Camera>().enabled = false;
            newCam.AddComponent<Camera>();
            newCam.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            newCam.SetActive(false);
            return newCam;
        }



    }
}

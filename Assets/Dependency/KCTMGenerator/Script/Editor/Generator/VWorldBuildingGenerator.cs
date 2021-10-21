using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using System.Collections;


namespace ARRC_DigitalTwin_Generator
{


    class VWorldBuildingGenerator : ARRCGenerator
    {
        public static VWorldBuildingGenerator _instance;

        static string cacheFolder = Utils.cacheFolder + "/VWorld";
        static string targetFolder = Utils.dataFolder + "/VWorld";
        //static string targetFolder = Utils.cacheFolder + "/VWorld";

        static string url3 = "http://xdworld.vworld.kr:8080/XDServer/requestLayerNode?APIKey=";
        static string url4 = "http://xdworld.vworld.kr:8080/XDServer/requestLayerObject?APIKey=";
        static string apiKey = "CEB52025-E065-364C-9DBA-44880E3B02B8";
        static string referer = "http://localhost:4141"; //apikey를 신청할 때 입력하는 호스트 주소

        //교량은 레벨 14에서 받아와야 한다.
        //static string layerName = "facility_bridge";
        //static int level = 14;

        //건물은 레벨 15에서 받아와야 한다.
        static string layerName = "facility_build";
        static int level = 15;

        //독도의 지형과 건물은 레벨 13에서 받아와야 한다. 독도의 지형 데이터는 DEM 쪽에 없고 여기에 함께 포함되어 있다.
        //static string layerName = "facility_dokdo";
        //static int level = 13;
        static double unit = 360 / (Math.Pow(2, level) * 10); //15레벨의 격자 크기(단위:경위도)


        //중복 다운로드를 피하기 위해 현재 있는 파일들 목록을 구한다.
        HashSet<string> jpgList;
        HashSet<string> fileNamesXdo;

        //다운로드받은 xdo파일에 대한 filename, lat,lon, altitude 정보.
        Dictionary<string, VWorldBuildingInfo> dicBuildingInfo;

        private ArrayList idxIdyList;

        public static VWorldBuildingGenerator GetInstance()
        {
            if (_instance == null)
            {
                _instance = new VWorldBuildingGenerator();
            }
            return _instance;

        }

        public static void ResetInstance()
        {
            _instance = null;
        }

        public IEnumerator MakeDownloadItemList_XDODataList(double latFrom, double lonFrom, double latTo, double lonTo)
        {
            currentState = "Making download list on XDO data...";
            isComplete = false;

            string[] folders_in_cacheDir = { "xdo_dat", "xdo_Files", "xdo_List", "obj" };
            string[] folders_in_targetDir = { "obj" };

            //if (!Directory.Exists(cacheFolder)) Directory.CreateDirectory(cacheFolder);
            foreach (string name in folders_in_cacheDir)
            {
                string subfolder = Path.Combine(cacheFolder, name);
                if (!Directory.Exists(subfolder))
                    Directory.CreateDirectory(subfolder);
            }

            foreach (string name in folders_in_targetDir)
            {
                string subfolder = Path.Combine(targetFolder, name);
                if (!Directory.Exists(subfolder))
                    Directory.CreateDirectory(subfolder);
            }

            //원래는 request와 response를 통해 idx idy 목록들을 받아와야 하지만, 간단한 계산을 통해 구할 수 있으므로 직접 한다.
            int minIdx = (int)Math.Floor((lonFrom + 180) / unit);  //minLon
            int minIdy = (int)Math.Floor((latTo + 90) / unit); //minLat
            int maxIdx = (int)Math.Floor((lonTo + 180) / unit); //maxLon
            int maxIdy = (int)Math.Floor((latFrom + 90) / unit); //maxLat

            //Debug.Log(minIdx + " , " + minIdy + " | " + maxIdx + " , " + maxIdy);
            List<DownloadItem> downloadItemList_xdodat = new List<DownloadItem>();

            int index = 0;
            idxIdyList = new ArrayList();

            for (int i = minIdx; i <= maxIdx; i++)
            {
                for (int j = minIdy; j <= maxIdy; j++)
                {
                    idxIdyList.Add(new Vector2(i, j));
                    index++;
                }
            }

            float total_progress = idxIdyList.Count;
            int count_progress = 0;

            foreach (Vector2 idxidy in idxIdyList)
            {
                if (isComplete)
                    break;

                progress = (count_progress++) / total_progress;

                int i = (int)idxidy.x;
                int j = (int)idxidy.y;

                string address3 = url3 + apiKey + "&Layer=" + layerName + "&Level=" + level
                + "&IDX=" + i.ToString() + "&IDY=" + j.ToString();
                string nameXdo = "xdoList" + i.ToString() + "_" + j.ToString() + ".dat";
                string filepathXdo = Path.Combine(cacheFolder, "xdo_dat/" + nameXdo);

                //Debug.Log("file :" + i.ToString() + "_" + j.ToString() + "세션 시작....." + (idx + 1).ToString());
                downloadItemList_xdodat.Add(new DownloadItemUnityWebRequest(address3)
                {
                    filename = filepathXdo,
                    averageSize = Utils.AVERAGE_VWORLD_DAT
                });

                yield return new WaitForFixedUpdate();
            }

            items = downloadItemList_xdodat;
            isComplete = true;
        }

        //XDO data파일을 Parsing하여 .txt로 저장하고 이를 다시 로드하여 관련된 xdo파일들을 requeset한다.
        public IEnumerator MakeDownloadItemList_XDOData()
        {
            currentState = "Making download list on XDO Data...";

            isComplete = false;
            //저장될 정보들을 담을 Collection들을 초기화.

            List<DownloadItem> downloadItemList_xdo = new List<DownloadItem>();
            dicBuildingInfo = new Dictionary<string, VWorldBuildingInfo>();

            for (int i = 0; i < idxIdyList.Count; i++)
            {
                if (isComplete)
                    break;

                progress = (float)i / idxIdyList.Count;

                Vector2 idxidy = (Vector2)idxIdyList[i];
                string idx = idxidy.x.ToString();
                string idy = idxidy.y.ToString();

                string fileNameXdo = "xdoList" + idx + "_" + idy + ".dat";
                string filepathXdo = Path.Combine(cacheFolder, "xdo_dat/" + fileNameXdo);

                if (!CheckDat(filepathXdo))
                {
                    Debug.Log(filepathXdo + " : VWorld's Buliding : No data");
                    continue;
                }
                string nameParsedXdo = "xdoList_parsed" + idx + "_" + idy + ".txt";
                string filePathParsedXdo = Path.Combine(cacheFolder, "xdo_List/" + nameParsedXdo);

                //if(//check )
                datParser(filepathXdo, filePathParsedXdo);
                AddXDOtoDownloadItemList(filePathParsedXdo, idx, idy, downloadItemList_xdo); //개별적인 xdo들을 호출하여 obj 파일로 만든다.			

                yield return new WaitForFixedUpdate();

            }
            items = downloadItemList_xdo;
            isComplete = true;
        }

        //다운로드 된 Xdo 파일들을 Parsing하여 Obj파일+Material파일로 변환한다.xdo에서 parsing된 jpg파일명을 이용해 texture파일을 request한다.
        public IEnumerator WriteOBJFiles()
        {
            currentState = "Converting and Writing to .OBJ...";

            isComplete = false;

            List<DownloadItem> downloadItemList_texture = new List<DownloadItem>();

            int idx = 0;
            foreach (KeyValuePair<string, VWorldBuildingInfo> it in dicBuildingInfo)
            {
                if (isComplete)
                    break;

                progress = (float)idx++ / dicBuildingInfo.Count;

                string xdoFilePath = it.Key;

                VWorldBuildingInfo binfo = it.Value;

                string version = binfo.version;
                string nodeIDX = binfo.nodeIDX;
                string nodeIDY = binfo.nodeIDY;

                //다운받은 xdo파일을 파싱하여 vertex정보는 obj 파일로 쓰고, 다운받을 texture들을 downloadmanager에 등록한다.
                string objName = binfo.xdofileName_without_ext + ".obj";
                string mtlName = binfo.xdofileName_without_ext + ".mtl";

                string objFilePath = Path.Combine(cacheFolder + "/obj", objName);
                string mtlFilePath = Path.Combine(cacheFolder + "/obj", mtlName);

                //Obj 기록
                FileStream fs = new FileStream(objFilePath, FileMode.Create, FileAccess.Write);
                StreamWriter bw = new StreamWriter(fs);

                //bw.WriteLine("# Rhino");
                bw.WriteLine();
                bw.WriteLine("mtllib " + mtlName);

                //Material 파일 기록
                FileStream fwm = new FileStream(mtlFilePath, FileMode.Create, FileAccess.Write);
                StreamWriter bwm = new StreamWriter(fwm);

                if (version.Equals("1"))
                    xdo31Parser_planer(xdoFilePath, binfo.xdofileName_without_ext + ".xdo", bw, getAddressForJpgFile("", nodeIDX, nodeIDY), bwm, downloadItemList_texture, binfo.lat, binfo.lon, out binfo.textureFileName_lod0);

                else if (version.Equals("2"))
                    xdo32Parser_planer(xdoFilePath, binfo.xdofileName_without_ext + ".xdo", bw, getAddressForJpgFile("", nodeIDX, nodeIDY), bwm, downloadItemList_texture, binfo.lat, binfo.lon, out binfo.textureFileName_lod0);// 다시 xdo 파일을 읽어서 파싱한 후 저장한다.

                bw.Close();
                bwm.Close();

                yield return new WaitForFixedUpdate();
            }

            items = downloadItemList_texture;
            isComplete = true;
        }

        public IEnumerator GenerateGameObjects_Building()
        {
            currentState = "Generating VWorld's 3D Models ...";
            isComplete = false;
            int idx = 0;

            GameObject parent = new GameObject("Buildings");
            foreach (KeyValuePair<string, VWorldBuildingInfo> it in dicBuildingInfo)
            {
                if (isComplete)
                    break;

                progress = (float)idx++ / dicBuildingInfo.Count;

                VWorldBuildingInfo binfo = it.Value;

                //다운받은 xdo파일을 파싱하여 vertex정보는 obj 파일로 쓰고, 다운받을 texture들을 downloadmanager에 등록한다.
                string objName = binfo.xdofileName_without_ext + ".obj";
                string mtlName = binfo.xdofileName_without_ext + ".mtl";
                string textureName = binfo.textureFileName_lod0;

                string objFilePath = Path.Combine(cacheFolder + "/obj", objName);
                string mtlFilePath = Path.Combine(cacheFolder + "/obj", mtlName);
                string textureFilePath = Path.Combine(cacheFolder + "/obj", textureName);

                string target_objFilePath = Path.Combine(targetFolder + "/obj", objName);
                string target_mtlFilePath = Path.Combine(targetFolder + "/obj", mtlName);
                string target_textureFilePath = Path.Combine(targetFolder + "/obj", textureName);

                //Copy to Unity Asset Folder.
                if (!File.Exists(target_objFilePath) && File.Exists(objFilePath))
                    File.Copy(objFilePath, target_objFilePath);

                if (!File.Exists(target_mtlFilePath) && File.Exists(mtlFilePath))
                    File.Copy(mtlFilePath, target_mtlFilePath);

                if (!File.Exists(target_textureFilePath) && File.Exists(textureFilePath))
                    File.Copy(textureFilePath, target_textureFilePath);

                //OBJ파일 로드
                if (File.Exists(target_objFilePath))
                {
                    GameObject loadedObject = ObjectLoader.Load(targetFolder + "/obj/", objName);

                    loadedObject.transform.parent = parent.transform;
                    VWorldBuildingInfo compo = loadedObject.AddComponent<VWorldBuildingInfo>();

                    compo.key = binfo.key;
                    compo.lat = binfo.lat;
                    compo.lon = binfo.lon;
                    compo.altitude = binfo.altitude;
                    compo.xdofileName_without_ext = binfo.xdofileName_without_ext;
                    compo.version = binfo.version;
                    compo.nodeIDX = binfo.nodeIDX;
                    compo.nodeIDY = binfo.nodeIDY;
                    compo.textureFileName_lod0 = binfo.textureFileName_lod0;

                    //Vector3 pos = TerrainUtils.LatLonToWorldWithElevation(new Vector3((float)binfo.lon, binfo.altitude, (float)binfo.lat));
                    Vector3 pos = TerrainUtils.LatLonToWorldWithElevation(TerrainContainer.Instance, binfo.lat, binfo.lon);
                    loadedObject.transform.position = pos;

                    int buildinglayer = LayerMask.NameToLayer("Building");
                    if (buildinglayer == -1)
                        Debug.LogError("Building Layer must be assigned in Layer Manager.");
                    else
                        loadedObject.layer = buildinglayer;
                }
                else
                {
                    Debug.Log(objFilePath + " is not exsit: Fail to generate gameobject");
                    continue;
                }


                yield return new WaitForFixedUpdate();
            }

            UnityEditor.AssetDatabase.Refresh();
            isComplete = true;

        }

        private static void datParser(string fileName, string fileNameW)
        {
            FileStream bis = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            FileStream fs = new FileStream(fileNameW, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);


            int[] datHeader = new int[4];
            string[] datHeaderName = { "level", "IDX", "IDY", "ObjectCount" };

            //Header 읽기
            for (int i = 0; i < 4; i++)
            {
                datHeader[i] = VWorldXDOParsor.pU32(bis);
                sw.WriteLine(datHeaderName[i] + "=" + datHeader[i]);
            }


            //Real3D Model Object 읽기
            for (int i = 0; i < datHeader[3]; i++)
            {
                string r_version = VWorldXDOParsor.pU8(bis) + "." + VWorldXDOParsor.pU8(bis) + "." + VWorldXDOParsor.pU8(bis) + "." + VWorldXDOParsor.pU8(bis);
                int r_type = VWorldXDOParsor.pU8(bis);
                int r_keylen = VWorldXDOParsor.pU8(bis);

                string r_key = VWorldXDOParsor.pChar(bis, r_keylen);

                double[] r_CenterPos = { VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis) };

                float r_altitude = VWorldXDOParsor.pFloat(bis);

                double[] r_box = { VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis) };

                int r_imgLevel = VWorldXDOParsor.pU8(bis);
                int r_dataFileLen = VWorldXDOParsor.pU8(bis);
                string r_dataFile = VWorldXDOParsor.pChar(bis, r_dataFileLen);

                int r_imgFileNameLen = VWorldXDOParsor.pU8(bis);
                string r_imgFileName = VWorldXDOParsor.pChar(bis, r_imgFileNameLen);

                sw.WriteLine(r_version + "|" + r_type + "|" + r_keylen + "|" + r_key + "|" + r_CenterPos[0] + "|" + r_CenterPos[1]
                                + "|" + r_altitude + "|" + r_box[0] + "|" + r_box[1] + "|" + r_box[2] + "|" + r_box[3] + "|" + r_box[4] + "|" + r_box[5] + "|"
                                + r_imgLevel + "|" + r_dataFileLen + "|" + r_dataFile + "|" + r_imgFileNameLen + "|" + r_imgFileName);
            }

            sw.Close();
            bis.Close();
            fs.Close();
        }

        private static bool CheckDat(string fileNameXdo)
        {
            FileStream fs = new FileStream(fileNameXdo, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);

            //첫줄에 해당 내용이 있다.
            string line = sr.ReadLine();
            sr.Close();
            int check = line.IndexOf("ERROR_SERVICE_FILE_NOTTHING");

            if (check == -1) return true;
            else return false;

        }

        private void AddXDOtoDownloadItemList(string filePathParsedXdo, string nodeIDX, string nodeIDY, List<DownloadItem> downloadItemList_xdo)
        {

            //읽기
            FileStream fr = new FileStream(filePathParsedXdo, FileMode.Open, FileAccess.Read);
            StreamReader br = new StreamReader(fr);

            string line;
            string[] temp;

            //네 줄은 파일목록이 아니므로 건너뛴다.
            line = br.ReadLine();
            line = br.ReadLine();
            line = br.ReadLine();
            line = br.ReadLine();

            //xdoList에서 xdo 파일이름을 하나하나 읽어들이면서 obj파일을 기록한다.
            //하나의 xdoList에 있는 건물들은 하나의 obj파일에 넣는다.
            while ((line = br.ReadLine()) != null)
            {
                temp = line.Split('|');
                string version = temp[0].Split('.')[3];

                string xdofileName = temp[15];
                string key = temp[3];
                double lon = double.Parse(temp[4]);
                double lat = double.Parse(temp[5]);
                float altitude = float.Parse(temp[6]);

                string url = getAddressForXdoFile(xdofileName, nodeIDX, nodeIDY);
                string filepath = Path.Combine(cacheFolder + "/xdo_Files", xdofileName);

                //저장될 xdo파일의 정보를 미리 저장한다.

                string xdofileName_without_ext = xdofileName.Substring(0, xdofileName.Length - 4); //확장자 제외.
                dicBuildingInfo.Add(filepath, new VWorldBuildingInfo(key, xdofileName_without_ext, lat, lon, altitude, version, nodeIDX, nodeIDY));

                //다운로드리스트에 추가.
                downloadItemList_xdo.Add(new DownloadItemUnityWebRequest(url)
                {
                    filename = filepath,
                    averageSize = Utils.AVERAGE_VWORLD_XDO
                });

            }

            br.Close();
        }

        private void xdo31Parser_planer(string xdofilePath, string xdoFileName, StreamWriter bw, string queryAddrForJpg, StreamWriter bwm, List<DownloadItem> downloadItemList_texture, double lat, double lon, out string texturefilename_lod0)
        {
            texturefilename_lod0 = "";
            int nnP = 0;

            FileStream bis = new FileStream(xdofilePath, FileMode.Open, FileAccess.Read);

            int type = VWorldXDOParsor.pU8(bis);
            if (type != 8)
            {
                Debug.LogError(xdofilePath + "The file not exist on server.");
                return;
            }
            int objectId = VWorldXDOParsor.pU32(bis);
            int keyLen = VWorldXDOParsor.pU8(bis);
            string key = VWorldXDOParsor.pChar(bis, keyLen);
            double[] objectBox = { VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis) };
            float altitude = VWorldXDOParsor.pFloat(bis);

            double objX = (objectBox[0] + objectBox[3]) / 2;
            double objY = (objectBox[1] + objectBox[4]) / 2;
            double objZ = (objectBox[2] + objectBox[5]) / 2;

            float[] objxyz = rotate3d((float)objX, (float)objY, (float)objZ, lon, lat);

            int vertexCount = VWorldXDOParsor.pU32(bis);

            //데이터 오류일 경우 건너뜀
            if (vertexCount <= 0)
            {
                Debug.LogError(xdofilePath + "Error : vertexCount <= 0");
                bis.Close();
                return;
            }

            double[,] vertex = new double[vertexCount, 8];

            for (int i = 0; i < vertexCount; i++)
            {
                float vx = VWorldXDOParsor.pFloat(bis);
                float vy = VWorldXDOParsor.pFloat(bis);
                float vz = VWorldXDOParsor.pFloat(bis);
                float vnx = VWorldXDOParsor.pFloat(bis);
                float vny = VWorldXDOParsor.pFloat(bis);
                float vnz = VWorldXDOParsor.pFloat(bis);
                float vtu = VWorldXDOParsor.pFloat(bis);
                float vtv = VWorldXDOParsor.pFloat(bis);

                //Vector3 xyz_planar = TerrainUtils.Transform2PlanarEarth(vx, vy, vz, lat, lon);
                float[] xyz = rotate3d(vx, vy, vz, lon, lat);

                //vertex[i][0] = p2.x + xyz[0]; // p2는 lat, lon의 ecef 좌표값
                //vertex[i][1] = p2.y - 1 * (xyz[1]);
                //vertex[i][2] = xyz[2] + objxyz[2] - 6378137;

                vertex[i, 0] = xyz[0];
                vertex[i, 1] = xyz[1] + objxyz[1] - 6378137; ////vworld이 참조하고 있는 world wind는 타원체가 아니라 6,378,137m의 반지름을 가지는 구면체다.
                vertex[i, 2] = xyz[2];

                vertex[i, 3] = vnx;
                vertex[i, 4] = vny;
                vertex[i, 5] = vnz;
                vertex[i, 6] = vtu;
                vertex[i, 7] = (1.0f - vtv);
            }

            int indexedNumber = VWorldXDOParsor.pU32(bis);

            short[] indexed = new short[indexedNumber];

            for (int i = 0; i < indexedNumber; i++)
            {
                indexed[i] = (short)(VWorldXDOParsor.pU16(bis) + 1);
            }

            int colorA = VWorldXDOParsor.pU8(bis);
            int colorR = VWorldXDOParsor.pU8(bis);
            int colorG = VWorldXDOParsor.pU8(bis);
            int colorB = VWorldXDOParsor.pU8(bis);

            int imageLevel = VWorldXDOParsor.pU8(bis);
            int imageNameLen = VWorldXDOParsor.pU8(bis);
            string imageName = VWorldXDOParsor.pChar(bis, imageNameLen);
            texturefilename_lod0 = imageName;

            int nailSize = VWorldXDOParsor.pU32(bis);
            //writeNailData(bis, imageName, nailSize);

            string url = queryAddrForJpg + imageName;
            downloadItemList_texture.Add(new DownloadItemUnityWebRequest(url)
            {
                filename = Path.Combine(cacheFolder + "/obj", imageName),
                averageSize = Utils.AVERAGE_VWORLD_TEXTURE
            });

            ////저장장소에 있는 텍스쳐 파일을 obj와 같은 곳에 복사해준다.
            //else
            //    fileCopy(targetFolder + "/jpg/" + imageName, targetFolder + "/xdo_obj/" + imageName);

            bw.WriteLine("g " + key);

            //material의 기본적 속성은 임의로 아래와 같이 쓴다.
            //mtl 파일의 자세한 스펙은 아래를 참조
            //http://paulbourke.net/dataformats/mtl/
            mtlSubWriter(bwm, key, imageName);

            for (int i = 0; i < vertexCount; i++)
            {
                bw.WriteLine("v " + vertex[i, 0] + " " + vertex[i, 1] + " " + vertex[i, 2]);
            }
            for (int i = 0; i < vertexCount; i++)
            {
                bw.WriteLine("vt " + vertex[i, 6] + " " + vertex[i, 7]);
            }
            for (int i = 0; i < vertexCount; i++)
            {
                bw.WriteLine("vn " + vertex[i, 3] + " " + vertex[i, 4] + " " + vertex[i, 5]);
            }
            bw.WriteLine("usemtl " + key);
            for (int i = 0; i < indexedNumber; i = i + 3)
            {
                // unity의 left-handed 좌표계에 맞추기 위해 index[i+2]와 index[i+1]의 순서를 바꾸었다. byIB 19620
                bw.Write("f ");
                bw.Write((indexed[i] + nnP) + "/" + (indexed[i] + nnP) + "/" + (indexed[i] + nnP) + " ");
                bw.Write((indexed[i + 2] + nnP) + "/" + (indexed[i + 2] + nnP) + "/" + (indexed[i + 2] + nnP) + " ");
                bw.Write((indexed[i + 1] + nnP) + "/" + (indexed[i + 1] + nnP) + "/" + (indexed[i + 1] + nnP));
                bw.WriteLine();
            }
            bis.Close();
        }

        private void xdo32Parser_planer(string xdofilePath, string xdoFileName, StreamWriter bw, string queryAddrForJpg, StreamWriter bwm, List<DownloadItem> downloadItemList_texture, double lat, double lon, out string texturefilename_lod0)
        {
            texturefilename_lod0 = "";
            int nnP = 0;

            FileStream bis = new FileStream(xdofilePath, FileMode.Open, FileAccess.Read);

            int type = VWorldXDOParsor.pU8(bis);
            if (type != 8)
            {
                Debug.LogError(xdofilePath + "The file not exist on server.");
                return;
            }
            int objectId = VWorldXDOParsor.pU32(bis);
            int keyLen = VWorldXDOParsor.pU8(bis);
            string key = VWorldXDOParsor.pChar(bis, keyLen);
            double[] objectBox = { VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis), VWorldXDOParsor.pDouble(bis) };
            float altitude = VWorldXDOParsor.pFloat(bis);

            double objX = (objectBox[0] + objectBox[3]) / 2;
            double objY = (objectBox[1] + objectBox[4]) / 2;
            double objZ = (objectBox[2] + objectBox[5]) / 2;

            float[] objxyz = rotate3d((float)objX, (float)objY, (float)objZ, lon, lat);

            int faceNum = VWorldXDOParsor.pU8(bis);

            if (xdoFileName.Equals("dongdo.xdo") && faceNum < 0) faceNum *= -1; //독도중 동도의 특별한 데이터 오류를 정정한다

            for (int j = 0; j < faceNum; j++)
            {
                int vertexCount = VWorldXDOParsor.pU32(bis);

                //데이터 오류일 경우 건너뜀
                if (vertexCount <= 0)
                {
                    Debug.LogError(xdofilePath + "Error : vertexCount <= 0");
                    continue;
                }

                double[,] vertex = new double[vertexCount, 8];

                for (int i = 0; i < vertexCount; i++)
                {
                    float vx = VWorldXDOParsor.pFloat(bis);
                    float vy = VWorldXDOParsor.pFloat(bis);
                    float vz = VWorldXDOParsor.pFloat(bis);
                    float vnx = VWorldXDOParsor.pFloat(bis);
                    float vny = VWorldXDOParsor.pFloat(bis);
                    float vnz = VWorldXDOParsor.pFloat(bis);
                    float vtu = VWorldXDOParsor.pFloat(bis);
                    float vtv = VWorldXDOParsor.pFloat(bis);

                    //Vector3 xyz_planar = TerrainUtils.Transform2PlanarEarth(vx, vy, vz, lat, lon);
                    float[] xyz = rotate3d(vx, vy, vz, lon, lat);

                    //vertex[i][0] = p2.x + xyz[0];
                    //vertex[i][1] = p2.y - 1 * (xyz[1]);
                    //vertex[i][2] = xyz[2] + objxyz[2] - 6378137;

                    vertex[i, 0] = xyz[0];
                    vertex[i, 1] = xyz[1] + objxyz[1] - 6378137;
                    vertex[i, 2] = xyz[2];

                    vertex[i, 3] = vnx;
                    vertex[i, 4] = vny;
                    vertex[i, 5] = vnz;
                    vertex[i, 6] = vtu;
                    vertex[i, 7] = (1.0f - vtv);
                }

                int indexedNumber = VWorldXDOParsor.pU32(bis);

                short[] indexed = new short[indexedNumber];

                for (int i = 0; i < indexedNumber; i++)
                {
                    indexed[i] = (short)(VWorldXDOParsor.pU16(bis) + 1);
                }

                int colorA = VWorldXDOParsor.pU8(bis);
                int colorR = VWorldXDOParsor.pU8(bis);
                int colorG = VWorldXDOParsor.pU8(bis);
                int colorB = VWorldXDOParsor.pU8(bis);

                int imageLevel = VWorldXDOParsor.pU8(bis);
                int imageNameLen = VWorldXDOParsor.pU8(bis);
                string imageName = VWorldXDOParsor.pChar(bis, imageNameLen);


                if (j == 0)
                    texturefilename_lod0 = imageName;

                int nailSize = VWorldXDOParsor.pU32(bis);

                //이렇게 강제로 읽어서 해당 부분의 바이트를 소모해주지 않으면 다음 루프에서 읽어야 할 곳을 읽지 못해 에러가 난다.
                VWorldXDOParsor.writeNailData_fake(bis, imageName, nailSize);

                string url = queryAddrForJpg + imageName;
                downloadItemList_texture.Add(new DownloadItemUnityWebRequest(url)
                {
                    filename = Path.Combine(cacheFolder + "/obj", imageName),
                    averageSize = Utils.AVERAGE_VWORLD_TEXTURE
                });

                ////저장장소에 있는 텍스쳐 파일을 obj와 같은 곳에 복사해준다.
                //else
                //    fileCopy(targetFolder + "/jpg/" + imageName, targetFolder + "/xdo_obj/" + imageName);

                bw.WriteLine("g " + key + "_" + j);

                //material의 기본적 속성은 임의로 아래와 같이 쓴다.
                //mtl 파일의 자세한 스펙은 아래를 참조
                //http://paulbourke.net/dataformats/mtl/
                mtlSubWriter(bwm, key + "_" + j, imageName);

                for (int i = 0; i < vertexCount; i++)
                {
                    bw.WriteLine("v " + vertex[i, 0] + " " + vertex[i, 1] + " " + vertex[i, 2]);
                }
                for (int i = 0; i < vertexCount; i++)
                {
                    bw.WriteLine("vt " + vertex[i, 6] + " " + vertex[i, 7]);
                }
                for (int i = 0; i < vertexCount; i++)
                {
                    bw.WriteLine("vn " + vertex[i, 3] + " " + vertex[i, 4] + " " + vertex[i, 5]);
                }
                bw.WriteLine("usemtl " + key + "_" + j);
                for (int i = 0; i < indexedNumber; i = i + 3)
                {
                    // unity의 left-handed 좌표계에 맞추기 위해 index[i+2]와 index[i+1]의 순서를 바꾸었다. byIB 19620
                    bw.Write("f ");
                    bw.Write((indexed[i] + nnP) + "/" + (indexed[i] + nnP) + "/" + (indexed[i] + nnP) + " ");
                    bw.Write((indexed[i + 1] + nnP) + "/" + (indexed[i + 1] + nnP) + "/" + (indexed[i + 1] + nnP) + " ");
                    bw.Write((indexed[i + 2] + nnP) + "/" + (indexed[i + 2] + nnP) + "/" + (indexed[i + 2] + nnP));
                    bw.WriteLine();
                }
            }
            bis.Close();
        }

        private static void mtlSubWriter(StreamWriter bw, string key, string imageName)
        {
            bw.WriteLine("newmtl " + key);
            bw.WriteLine("Ka 0.000000 0.000000 0.000000");
            bw.WriteLine("Kd 1.000000 1.000000 1.000000");
            bw.WriteLine("Ks 1.000000 1.000000 1.000000");
            bw.WriteLine("Tf 0.0000 0.0000 0.0000");
            bw.WriteLine("d 1.0000");
            bw.WriteLine("Ns 0");
            bw.WriteLine("map_Kd " + imageName);
            bw.WriteLine();
        }

        private static void fileCopy(string inFileName, string outFileName)
        {

            /*
            //http://fruitdev.tistory.com/87 	
            FileInputStream inputStream = new FileInputStream(inFileName);
        FileOutputStream outputStream = new FileOutputStream(outFileName);

        FileChannel fcin = inputStream.getChannel();
        FileChannel fcout = outputStream.getChannel();

        long size = fcin.size();
        fcin.transferTo(0, size, fcout);

        fcout.close();
        fcin.close();		  
        outputStream.close();
        inputStream.close();
        */
        }

        private static string getAddressForXdoFile(string dataFile, string nodeIDX, string nodeIDY)
        {

            string address = url4 + apiKey + "&Layer=" + layerName + "&Level=" + level + "&IDX=" + nodeIDX + "&IDY=" + nodeIDY
                    + "&DataFile=" + dataFile;
            return address;
        }

        private static string getAddressForJpgFile(string jpgFile, string nodeIDX, string nodeIDY)
        {
            string address = url4 + apiKey + "&Layer=" + layerName + "&Level=" + level + "&IDX=" + nodeIDX + "&IDY=" + nodeIDY
                    + "&DataFile=" + jpgFile;
            return address;
        }

        private static float[] rotate3d(float vx, float vy, float vz, double lon, double lat)
        {
            float x, y, z;

            double p = (lon) / 180 * Math.PI;
            double t = (90 - lat) / 180 * Math.PI;

            //원래 회전공식대로 하니까 90도 회전된 결과가 나와 z축을 중심으로 다시 -90도 회전을 했다.
            x = (float)(Math.Sin(p) * vx + Math.Cos(p) * vy);
            z = (float)(Math.Cos(t) * Math.Cos(p) * vx - Math.Cos(t) * Math.Sin(p) * vy - Math.Sin(t) * vz);
            y = (float)(Math.Sin(t) * Math.Cos(p) * vx - Math.Sin(t) * Math.Sin(p) * vy + Math.Cos(t) * vz);

            //x = (float)(Math.Sin(p) * vx - Math.Cos(p) * vz);
            //y = (float)(Math.Cos(t)*Math.Cos(p) * vx - Math.Sin(t) * vy + Math.Cos(t) * Math.Sin(p)*vz);
            //z = (float)(Math.Sin(t)*Math.Cos(p)*vx + Math.Cos(t)*vy  + Math.Sin(t)*Math.Sin(p)*vz);
            //x = (float)(Math.Cos(p) * vx - Math.Sin(p) * vy);
            //y = (float)(Math.Cos(t) * Math.Sin(p) * vx - Math.Cos(t) * Math.Cos(p) * vy - Math.Sin(t) * vz);
            //z = (float)(Math.Sin(t) * Math.Sin(p) * vx - Math.Sin(t) * Math.Cos(p) * vy + Math.Cos(t) * vz);

            return new float[] { -x, y, -z };

            //Vector3 vertex = new Vector3(vx, vy, vz);
            //Matrix4x4 rotMat = Matrix4x4.Rotate(Quaternion.AngleAxis(90, Vector3.right));

            //Vector3 rot_vertex = rotMat.MultiplyPoint(vertex);
            //return new float[] { rot_vertex.x, rot_vertex.y, rot_vertex.z };
        }



    }
}

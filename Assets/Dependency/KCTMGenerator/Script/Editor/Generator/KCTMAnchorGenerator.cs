using PrimitivesPro.GameObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KCTM.Network.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace ARRC_DigitalTwin_Generator
{
    class KCTMAnchorGenerator : ARRCGenerator
    {
        private string cacheFolder = Utils.cacheFolder + "/KCTM";
        private string targetFolder = ARRC_DigitalTwin_Generator.Utils.dataFolder + "/KCTM";

        public string basicUri = "http://54.180.86.59:9000";

        string filepath;

        //<for singleton>
        private static KCTMAnchorGenerator instance;
        public static KCTMAnchorGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KCTMAnchorGenerator();
                }
                return instance;
            }
        }
        //<for singleton>


        public static void Dispose()
        {
            instance = null;
        }

        public IEnumerator MakeDownloadList(double latFrom, double lonFrom, double latTo, double lonTo)
        {
            currentState = "Making download list for KCTM data...";
            isComplete = false;
            
            string subFolder_xml = Path.Combine(cacheFolder, "jason");

            if (!Directory.Exists(subFolder_xml)) Directory.CreateDirectory(subFolder_xml);

            List<DownloadItem> downloadItemList = new List<DownloadItem>();

            string filename = "from(" + lonFrom.ToString() + ", " + latFrom.ToString() + ")to(" + lonTo.ToString() + ", " + latTo.ToString() + ").txt";
            filepath = Path.Combine(subFolder_xml, filename);
            
            //if (!File.Exists(filepath)) 
            {
                string url = string.Format("/geotaggedcontent/list?&minLatitude={2}&minLongitude={1}&maxLatitude={0}&maxLongitude={3}", latFrom, lonFrom, latTo, lonTo);

                downloadItemList.Add(new DownloadItemUnityWebRequest_KCTM(AppendUri(url), GenerateAnchorObjects, null)
                //downloadItemList.Add(new DownloadItemUnityWebRequest(AppendUri(url))
                {
                    filename = filepath,
                    averageSize = Utils.AVERAGE_STREETVIEW_XML_SIZE
                });

                progress = 1;
             
            }
            items = downloadItemList;
            isComplete = true;

            yield return null;
        }
        public void GenerateAnchorObjects(Result result)
        {
            List<Anchor> anchorList = JsonConvert.DeserializeObject<List<Anchor>>(result.result.ToString());
            GameObject parent = new GameObject("KCTM Anchors");

            for (int i = 0; i < anchorList.Count; i++)
            {
                double lon = anchorList[i].point.longitude;
                double lat = anchorList[i].point.latitude;
                double alt = anchorList[i].point.altitude;

                Vector3 pos = TerrainUtils.LatLonToWorldWithElevation(TerrainContainer.Instance, lat, lon);

                //poiMarker.GetComponent<ArscenePrefab>().arCamera = arCamera;

                //poiMarker.transform.parent = arScenesParent.transform;
                ////Debug.Log("ContentLinker: " + anchor.content.contentlinkers.Count);

                //arScenes.Add(poiMarker);
                //arAnchors.Add(anchor);

                //counter += 0.5f;
            }
        }


        private string AppendUri(string uri)
        {
            StringBuilder sb = new StringBuilder();
            if (basicUri.EndsWith("/"))
            {
                if (uri.StartsWith("/"))
                {
                    sb.Append(basicUri);
                    sb.Append(uri.Remove(0));
                }
                else
                {
                    sb.Append(basicUri);
                    sb.Append(uri);
                }
            }
            else
            {
                if (uri.StartsWith("/"))
                {
                    sb.Append(basicUri);
                    sb.Append(uri);
                }
                else
                {
                    sb.Append(basicUri);
                    sb.Append("/");
                    sb.Append(uri);
                }
            }

            return sb.ToString();
        }
    


    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;
using System.IO;
using System;

namespace ARRC_DigitalTwin_Generator
{
    public class DownloadItem_StreetViewInfo : DownloadItem
    {
        public UnityWebRequest uwr;
        public Dictionary<string, string> headers;
        //public StreetViewInfo svdata;
        public DownloadItem_StreetViewInfo(string url) : this(UnityWebRequest.Get(url))
        {
        }

        public DownloadItem_StreetViewInfo(UnityWebRequest uwr)
        {
            //DownloadManager.Add(this);

            this.uwr = uwr;
        }

        public override float progress
        {
            get { return uwr.downloadProgress; }
        }
        public override bool exists
        {
            get { return false; }
        }
        public override void CheckComplete()
        {
            if (!uwr.isDone) return;

            if (string.IsNullOrEmpty(uwr.error))
            {
                string text = uwr.downloadHandler.text;

                SaveSVInfoData(text);
                //svdata = new StreetViewInfo(lat, lon, image_date, elevation);
                //byte[] bytes = uwr.downloadHandler.data;
                //SaveWWWData(bytes);
                //DispatchCompete(ref bytes);
            }
            else Debug.LogWarning("Download failed: " + uwr.url + "\n" + uwr.error);

            DownloadManager.completeSize += averageSize;
            complete = true;

            Dispose();
        }

        public override void Dispose()
        {
            base.Dispose();

            uwr.Dispose();
            uwr = null;
        }

        public override void Start()
        {
            if (headers != null)
            {
                foreach (var header in headers) uwr.SetRequestHeader(header.Key, header.Value);
            }
#if UNITY_2018_2_OR_NEWER
            uwr.SendWebRequest();
#else
            uwr.Send();
#endif
        }

        void SaveSVInfoData(string text)
        {

            if (!string.IsNullOrEmpty(filename))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(text);
                    XmlNode fnode = doc.SelectSingleNode("panorama/data_properties ");

                    string pano_id = XMLExt.GetAttribute<string>(fnode, "pano_id");
                    string image_date = XMLExt.GetAttribute<string>(fnode, "image_date");
                    string lat = XMLExt.GetAttribute<string>(fnode, "lat");
                    string lon = XMLExt.GetAttribute<string>(fnode, "lng");
                    string origianl_lat = XMLExt.GetAttribute<string>(fnode, "original_lat");
                    string original_lng = XMLExt.GetAttribute<string>(fnode, "original_lng");
                    string elevation_wgs84 = XMLExt.GetAttribute<string>(fnode, "elevation_wgs84_m");

                    fnode = doc.SelectSingleNode("panorama/projection_properties ");
                    string pano_yaw_deg = XMLExt.GetAttribute<string>(fnode, "pano_yaw_deg");
                    string tilt_yaw_deg = XMLExt.GetAttribute<string>(fnode, "tilt_yaw_deg");
                    string tilt_pitch_deg = XMLExt.GetAttribute<string>(fnode, "tilt_pitch_deg");
                    //string filepath = Path.Combine(directory, pano_id+".txt");

                    using (StreamWriter outputFile = new StreamWriter(filename, true))
                    {
                        string content = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", pano_id, image_date, lat, lon, origianl_lat, original_lng, elevation_wgs84, pano_yaw_deg, tilt_yaw_deg, tilt_pitch_deg);
                        outputFile.WriteLine(content);
                    }

                }
                catch (Exception e)
                {
                    //어떨 때 발생하는 지는 모르겠으나 parsing 안되는 경우 있음. 

                    //Debug.LogError(e.Message + "\n" + e.StackTrace);
                    //CancelCapture();
                }

            }

        }
    }
}

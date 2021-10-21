using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ARRC_DigitalTwin_Generator
{
    public class StreetViewInfo : MonoBehaviour
    {
        public Dictionary<int, string> info_partImage = new Dictionary<int, string>();
        //public List<>
        public string savedTextureFilePath;
        public string panoid;
        public string lat, lon;
        public string elevation_wgs84_m;
        public string image_date;
        public string pano_yaw_deg;
        public string tilt_yaw_deg;
        public string tilt_pitch_deg;

        public StreetViewInfo(string panoid, string lat, string lon, string image_date, string elevation_wgs84_m, string pano_yaw_deg, string tilt_yaw_deg, string tilt_pitch_deg)
        {
            this.panoid = panoid;
            this.lat = lat;
            this.lon = lon;
            this.image_date = image_date;
            this.elevation_wgs84_m = elevation_wgs84_m;
            this.pano_yaw_deg = pano_yaw_deg;
            this.tilt_yaw_deg = tilt_yaw_deg;
            this.tilt_pitch_deg = tilt_pitch_deg;
        }
    }
}

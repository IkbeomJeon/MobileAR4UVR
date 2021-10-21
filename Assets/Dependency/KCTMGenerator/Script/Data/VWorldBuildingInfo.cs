using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ARRC_DigitalTwin_Generator
{
    public class VWorldBuildingInfo : MonoBehaviour
    {
        public string key;
        public string xdofileName_without_ext;
        public double lat, lon;
        public float altitude;
        public string version;
        public string nodeIDX;
        public string nodeIDY;
        public string textureFileName_lod0; //가장 고해상도 이미지의 파일명.

        public VWorldBuildingInfo(string key, string xdofileName_without_ext, double lat, double lon, float altitude, string version, string nodeIDX, string nodeIDY)
        {
            this.key = key;
            this.xdofileName_without_ext = xdofileName_without_ext;
            this.lat = lat;
            this.lon = lon;
            this.altitude = altitude;
            this.version = version;
            this.nodeIDX = nodeIDX;
            this.nodeIDY = nodeIDY;
        }
    }
}

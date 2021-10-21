using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using System.IO;

namespace ARRC_DigitalTwin_Generator
{
    class VWorldTerrainGenerator : ARRCGenerator
    {
        public static VWorldTerrainGenerator _instance;

        static string cacheFolder = Utils.cacheFolder + "/VWorld";
        static string targetFolder = Utils.dataFolder + "/VWorld";

        static string url3 = "http://xdworld.vworld.kr:8080/XDServer/requestLayerNode?APIKey=";
        static string apiKey = "CEB52025-E065-364C-9DBA-44880E3B02B8";

        //아래에서 값 참고
        //https://github.com/nasa/World-Wind-Java/blob/master/WorldWind/src/gov/nasa/worldwind/globes/Earth.java
        public const double WGS84_EQUATORIAL_RADIUS = 6378137.0; // ellipsoid equatorial getRadius, in meters
        public const double WGS84_POLAR_RADIUS = 6356752.3; // ellipsoid polar getRadius, in meters
        public const double WGS84_ES = 0.00669437999013; // eccentricity squared, semi-major axis / 이심률 제곱 / 이심률 = Math.sqrt(1-(장반경제곱/단반경제곱))

        public const double ELEVATION_MIN = -11000d; // Depth of Marianas trench
        public const double ELEVATION_MAX = 8500d; // Height of Mt. Everest.

        static int level = 15;
        /*
        level 15 = 1.5m grid (대략적으로)
        level 14 = 3m grid
        level 13 = 6m grid
        level 12 = 12m grid
        level 11 = 24m grid
        level 10 = 48m grid
        level 9 = 96m grid
        level 8 = 192m grid
        level 7 = 284m grid
        */
        static double unit = 360 / (Math.Pow(2, level) * 10); //15레벨의 격자 크기(단위:경위도)

        public static VWorldTerrainGenerator GetInstance()
        {
            if (_instance == null)
            {
                _instance = new VWorldTerrainGenerator();
            }
            return _instance;

        }

        public IEnumerator MakeDownloadItemList(Vector2 from_coord, Vector2 to_coord)
        {
            currentState = "Making download list...";
            isComplete = false;

            string[] folders_in_cacheDir = { "DEM bil", "DEM txt_Cartesian", "DEM txt_latlon", "DEM txt_UTMK", "DEM dds" };
            string[] folders_in_targetDir = { "DEM obj" };

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

            string layerName = "dem";
            string layerName2 = "tile";

            //string[] latlon = getCoordination(); //어떤 영역을 가져올지 정한다.
            //string minLon = from_coord. / 경도
            //string minLat = latlon[1]; //위도	 
            //string maxLon = latlon[2];
            //string maxLat = latlon[3];

            yield return null;

        }

    }
  
}

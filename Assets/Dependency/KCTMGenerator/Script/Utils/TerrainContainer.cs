using System;
using UnityEngine;

namespace ARRC_DigitalTwin_Generator
{
    public class TerrainContainer
    {
        static TerrainContainer _instance;
        /// <summary>
        /// Maximal value of elevation
        /// </summary>
        public double maxElevation;

        /// <summary>
        /// Minimal value of elevation
        /// </summary>
        public double minElevation;

        /// <summary>
        /// Scale of terrains
        /// </summary>
        public Vector3 scale;

        /// <summary>
        /// Size of terrains in world units
        /// </summary>
        public Vector3 size;

        public Bounds bounds;

        /// <summary>
        /// Top latitude
        /// </summary>
        public double topLatitude;

        /// <summary>
        /// Top latitude in Mercator projection (0-1)
        /// </summary>
        public double topMercator;

        /// <summary>
        /// Left longitude
        /// </summary>
        public double leftLongitude;

        /// <summary>
        /// Left longitude in Mercator projection (0-1)
        /// </summary>
        public double leftMercator;

        /// <summary>
        /// Bottom latitude
        /// </summary>
        public double bottomLatitude;

        /// <summary>
        /// Bottom latitude in Mercator projection (0-1)
        /// </summary>
        public double bottomMercator;

        /// <summary>
        /// Right longitude
        /// </summary>
        public double rightLongitude;

        /// <summary>
        /// Right longitude in Mercator projection (0-1)
        /// </summary>
        public double rightMercator;

        /// <summary>
        /// Width. Right longitude - left longitude
        /// </summary>
        public double width;

        /// <summary>
        /// Height. Top latitude - bottom latitude 
        /// </summary>
        public double height;
        /// <summary>
        /// Right longitude in Mercator projection (0-1)
        /// </summary>

        public double mercatorWidth;
        public double mercatorHeight;
        public Terrain Terrain { get; private set; }

        public static TerrainContainer Instance
        {
            get{
                if (_instance == null)
                {
                    _instance = new TerrainContainer();
                }

                return _instance;
            }
        }

        public void SetTerrain(Terrain terrain)
        {
            Terrain = terrain;
        }

        public void SetCoordinates(double latFrom, double lonFrom, double latTo, double lonTo)
        {
            const float scaleYCoof = 1112.0f;
            const float baseScale = 1000;

            Vector2Int tCount = new Vector2Int(1, 1);
            Vector3 terrainScale = Vector3.one;

            double rangeX = lonTo - lonFrom;
            double rangeY = latFrom - latTo;

            double scfY = Math.Sin(latFrom * Mathf.Deg2Rad);
            double sctY = Math.Sin(latTo * Mathf.Deg2Rad);
            double ccfY = Math.Cos(latFrom * Mathf.Deg2Rad);
            double cctY = Math.Cos(latTo * Mathf.Deg2Rad);

            double cX = Math.Cos(rangeX * Mathf.Deg2Rad);
            double sizeX1 = Math.Abs(TerrainUtils.EARTH_RADIUS * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
            double sizeX2 = Math.Abs(TerrainUtils.EARTH_RADIUS * Math.Acos(sctY * sctY + cctY * cctY * cX));

            double sizeX = (sizeX1 + sizeX2) / 2.0;
            double sizeZ = TerrainUtils.EARTH_RADIUS * Math.Acos(scfY * sctY + ccfY * cctY);

            maxElevation = TerrainUtils.MAX_ELEVATION;
            minElevation = -TerrainUtils.MAX_ELEVATION;

            //double maxEl, minEl;
            //RealWorldTerrainElevationGenerator.GetElevationRange(out minEl, out maxEl);
            //maxElevation = maxEl + prefs.autoDetectElevationOffset.y;
            //minElevation = minEl - prefs.autoDetectElevationOffset.x;

            double sX = Math.Round(sizeX / tCount.x * baseScale * terrainScale.x);
            double sY = Math.Round((maxElevation - minElevation) / scaleYCoof * baseScale * terrainScale.y);
            double sZ = Math.Round(sizeZ / tCount.y * baseScale * terrainScale.z);

            if (sY < 1) sY = 1;

            scale = new Vector3((float)(sX * tCount.x / rangeX), (float)(sY / (maxElevation - minElevation)), (float)(sZ * tCount.y / rangeY));
            size = new Vector3((float)sX, (float)sY, (float)sZ);
            bounds = new Bounds(size / 2, size);

            leftLongitude = lonFrom;
            topLatitude = latFrom;
            rightLongitude = lonTo;
            bottomLatitude = latTo;

            width = rightLongitude - leftLongitude;
            height = bottomLatitude - topLatitude;

            TerrainUtils.LatLongToMercat(lonFrom, latFrom, out leftMercator, out topMercator);
            TerrainUtils.LatLongToMercat(lonTo, latTo, out rightMercator, out bottomMercator);

            mercatorWidth = rightMercator - leftMercator;
            mercatorHeight = bottomMercator - topMercator;
        }

        public float GetHeight(Vector2 worldXZ)
        {
            if (Terrain == null)
            {
                Debug.LogError("Terrain is null. It returns to 0.");
                return 0;
            }
                
            else
                return Terrain.SampleHeight(new Vector3(worldXZ.x, 0, worldXZ.y));
        }
        public float GetHeight(Vector3 worldXYZ)
        {
            if (Terrain == null)
            {
                //Debug.LogError("Terrain is null. It returns to 0.");
                return 0;
            }

            else
                return Terrain.SampleHeight(worldXYZ);
        }
    }
}

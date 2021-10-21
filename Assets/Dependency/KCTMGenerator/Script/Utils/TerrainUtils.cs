using System;
using UnityEngine;

namespace ARRC_DigitalTwin_Generator
{
    public static class TerrainUtils
    {
        public const double DEG2RAD = Math.PI / 180;
        public const double EARTH_RADIUS = 6371;
        public const int MAX_ELEVATION = 15000;

        /// <summary>
        /// Size of tile.
        /// </summary>
        /// 

        public const short TILE_SIZE = 256;

        public static double Clamp(double n, double minValue, double maxValue)
        {
            if (n < minValue) return minValue;
            if (n > maxValue) return maxValue;
            return n;
        }

        public static double Repeat(double n, double minValue, double maxValue)
        {
            if (double.IsInfinity(n) || double.IsInfinity(minValue) || double.IsInfinity(maxValue) || double.IsNaN(n) || double.IsNaN(minValue) || double.IsNaN(maxValue)) return n;

            double range = maxValue - minValue;
            while (n < minValue || n > maxValue)
            {
                if (n < minValue) n += range;
                else if (n > maxValue) n -= range;
            }
            return n;
        }

        public static void TileToLatLong(double tx, double ty, int zoom, out double lx, out double ly)
        {
            double mapSize = TILE_SIZE << zoom;
            lx = 360 * (Repeat(tx * TILE_SIZE, 0, mapSize - 1) / mapSize - 0.5);
            ly = 90 - 360 * Math.Atan(Math.Exp(-(0.5 - Clamp(ty * TILE_SIZE, 0, mapSize - 1) / mapSize) * 2 * Math.PI)) / Math.PI;
        }

        public static void MercatToLatLong(double mx, double mz, out double lon, out double lat)
        {
            uint mapSize = (uint)TILE_SIZE << 20;
            double px = Clamp(mx * mapSize + 0.5, 0, mapSize - 1);
            double py = Clamp(mz * mapSize + 0.5, 0, mapSize - 1);
            mx = px / TILE_SIZE;
            mz = py / TILE_SIZE;
            TileToLatLong(mx, mz, 20, out lon, out lat);
        }

        public static void LatLongToMercat(double lon, double lat, out double mx, out double mz)
        {
            double sy = Math.Sin(lat * DEG2RAD);
            mx = (lon + 180) / 360;
            mz = 0.5 - Math.Log((1 + sy) / (1 - sy)) / (Math.PI * 4);
        }

        public static Vector2 MercatToWorld(TerrainContainer container, double mx, double mz)
        {

            double wx = (mx - container.leftMercator) / (container.rightMercator - container.leftMercator) * container.size.x;
            double wz = (1 - (mz - container.topMercator) / (container.bottomMercator - container.topMercator)) * container.size.z;

            return new Vector2((float)wx, (float)wz);
        }

        public static void WorldToMercat(TerrainContainer container, Vector3 world, out double mx, out double y, out double mz)
        {

            mx = (container.rightMercator - container.leftMercator) * world.x / container.size.x + container.leftMercator;
            y = world.y;
            mz = (1 - world.z / container.size.z) * (container.bottomMercator - container.topMercator) + container.topMercator;
            
        }

        public static void WorldToLatLon(TerrainContainer container, Vector3 world, out double lat, out double lon)
        {
            double mx, y, mz;
            WorldToMercat(container, world, out mx, out y, out mz);

            MercatToLatLong(mx, mz, out lon, out lat);
        }

        public static Vector3 LatLonToWorldWithElevation(TerrainContainer container, double lat, double lon)
        {
            LatLongToMercat(lon, lat, out double  mx, out double  mz);

            Vector2 worldXZ = MercatToWorld(container, mx, mz);

            //float elevation = container.GetHeight(worldXZ);
            float elevation = 0;

            return new Vector3(worldXZ.x, elevation, worldXZ.y);
        }

        public static void LatLon2ECEF(double _lat, double _lon, double _alt, out double x, out double y, out double z)
        {
            //WGS84 (Lat/Lon) -> ECEF(LH) 변환 공식 : R 은 지구 반지름 추정값
            //x = R * cos(lat) * cos(lon)
            //y = R * cos(lat) * sin(lon) * -1(왼손좌표계)
            //z = R * sin(lat)

            //Vector3 result = new Vector3();

            double lon = (_lon * Math.PI) /180;
            double lat = (_lat * Math.PI) /180;

            x = (EARTH_RADIUS * 1000 * Math.Cos(lat) * Math.Cos(lon));
            y = (EARTH_RADIUS * 1000 * Math.Cos(lat) * Math.Sin(lon) * -1);
            z = (EARTH_RADIUS * 1000 * Math.Sin(lat));

            //// WGS84 ellipsoid constants:
            //double a = 6378137;
            //double e = 0.081819190842622;

            //// intermediate calculation
            //// (prime vertical radius of curvature)
            //double N = a / Math.Sqrt(1 - e*e * Math.Sin(lat)* Math.Sin(lat));

            //// results:
            //double x = (N + alt) * Math.Cos(lat) * Math.Cos(lon);
            //double y = (N + alt) * Math.Cos(lat) * Math.Sin(lon);
            //double z = ((1 - e*e) * N + alt) * Math.Sin(lat);

            //return result;
        }

        public static Vector2 ECEF2LatLon(double x, double y, double z)
        {
            //ECEF(LH)-- > WGS84 Lon Lat 변환
            //lat = asin(z / R)
            //lon = atan2(y, x) * -1(왼손좌표계)

            Vector2 wgs84 = new Vector2();
            //wgs84.x = (float)(Mathf.Asin(z / (EARTH_RADIUS * 1000)) * 180) / Math.PI; //Lat
            //wgs84.y = (Mathf.Atan2(y, x) * -1 * 180) / Math.PI;                //Lon

            return wgs84;
        }

        public static Vector3 Transform2PlanarEarth(float vx, float vy, float vz, double lat, double lon)
        {
            Vector3 result = new Vector3();

            double ecef_x, ecef_y, ecef_z;
            LatLon2ECEF(lat, lon, 0, out ecef_x, out ecef_y, out ecef_z);

            Vector3 W = (new Vector3((float)ecef_x, (float)ecef_y, (float)ecef_z)).normalized;//P.normalized;                                //y
            Vector3 V = (new Vector3(0, 0, 1) - W);      //z
            Vector3 U = (Vector3.Cross(V, W));        //x

            Matrix4x4 mat = new Matrix4x4();
            //mat.SetRow(0, U);
            //mat.SetRow(1, W);
            //mat.SetRow(2, V);
            //mat.SetRow(3, new Vector4(0, 0, 0, 1));

            mat.SetColumn(0, U);
            mat.SetColumn(1, W);
            mat.SetColumn(2, V);
            mat.SetColumn(3, new Vector4(0, 0, 0, 1));

            //Matrix4x4 mat_inverse = mat.inverse;
            //Debug.Log(mat_inverse);

            result = mat.inverse.MultiplyPoint(new Vector3(vx, vy, vz));

            return result;
        }

        public static void GetCoord(double LAT_A, double LON_A, double LAT_B, double LON_B, out double latFrom, out double lonFrom, out double latTo, out double lonTo)
        {
            if (LON_A < LON_B)
            {
                lonFrom = LON_A;
                lonTo = LON_B;
            }
            else
            {
                lonFrom = LON_B;
                lonTo = LON_A;
            }

            if (LAT_A < LAT_B)
            {
                latFrom = LAT_B;
                latTo = LAT_A;
            }
            else
            {
                latFrom = LAT_A;
                latTo = LAT_B;
            }
        }

        public static bool CheckCoordValidation(double latFrom, double lonFrom, double latTo, double lonTo)
        {
            if (lonFrom >= lonTo)
            {
                Debug.Log("Bottom-Right Longitude must be greater than Top-Left Longitude");
                return false;
            }
            if (latFrom <= latTo)
            {
                Debug.Log("Top-Left Latitude must be greater than Bottom-Right Latitude");
                return false;
            }
            if (lonFrom < -180 || lonTo < -180 || lonFrom > 180 || lonTo > 180)
            {
                Debug.Log("Longitude must be between -180 and 180.");
                return false;
            }
            return true;
        }

    }
}

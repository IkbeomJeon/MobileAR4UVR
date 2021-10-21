using UnityEngine;

namespace ARRC_DigitalTwin_Generator
{
    public static class Utils
    {
        public const int MB = 1048576;

        public const long AVERAGE_STREETVIEW_XML_SIZE = 6069;
        public const long AVERAGE_STREETVIEW_SIZE = 27904;
        public const long AVERAGE_VWORLD_DAT = 2467;
        public const long AVERAGE_VWORLD_XDO = 1675;
        public const long AVERAGE_VWORLD_TEXTURE = 3955;
        public const int TILE_SIZE_STREETVIEW = 512;

        public static string cacheFolder = Application.dataPath + "/../ARRC_Cache/";
        public static string dataFolder = Application.dataPath + "/ARRC_Storage";
    }
}
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KCTM.Network;
using KCTM.Network.Data;


public static class Util
{

    public static string GetHumanTimeFormatFromMilliseconds(long milliseconds)
    {
        DateTime date = (new DateTime(1970, 1, 1)).AddMilliseconds(milliseconds);
        TimeZoneInfo localZone = TimeZoneInfo.Local;
        DateTime convertedDate = TimeZoneInfo.ConvertTimeFromUtc(date, localZone);

        string message = convertedDate.ToString("MM/dd/yyyy HH:mm:ss");
        return message;

    }

    public static long GetCurrentTimeinMilliseconds()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        long currentEpochTime = (long)(DateTime.UtcNow - epochStart).TotalSeconds;

        return currentEpochTime;
    }

    public static float PointDiff(Point x, Point y)
    {
        return Mathf.Sqrt(Mathf.Pow(((float)x.latitude - (float)y.latitude), 2) + Mathf.Pow(((float)x.longitude - (float)y.longitude), 2));
    }

    public static float VectorDiff(Vector3 v1, Vector3 v2)
    {
        return Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.y - v2.y), 2) + Mathf.Pow((v1.z - v2.z), 2));
    }

    public static AudioType GetAudioType(string extension)
    {
        if (extension.Equals(".mp3"))
            return AudioType.MPEG;
        else if (extension.Equals(".ogg"))
            return AudioType.OGGVORBIS;
        else if (extension.Equals(".wav"))
            return AudioType.WAV;
        else if (extension.Equals(".aiff") || extension.Equals(".aif"))
            return AudioType.AIFF;
        else if (extension.Equals(".mod"))
            return AudioType.MOD;
        else if (extension.Equals(".it"))
            return AudioType.IT;
        else if (extension.Equals(".s3m"))
            return AudioType.S3M;
        else if (extension.Equals(".xm"))
            return AudioType.XM;
        else
            return AudioType.UNKNOWN;
    }

    public static bool IsitImage(string extension)
    {
        if (extension.Equals(".png") ||
            extension.Equals(".jpg") ||
            extension.Equals(".bmp") ||
            extension.Equals(".exr") ||
            extension.Equals(".gif") ||
            extension.Equals(".iff") ||
            extension.Equals(".hdr") ||
            extension.Equals(".pict"))
            return true;
        else
            return false;
    }

    public static bool IsitModel(string extension)
    {
        if (extension.Equals(".glb"))
            return true;
        else
            return false;
    }

    /*
    public static float GetCanvasScale(CanvasScaler canvasScaler)
    {
        //float referenceWidth = canvasScaler.referenceResolution.x;
        float referenceWidth = Screen.width;

        //float referenceHeight = canvasScaler.referenceResolution.y;
        float referenceHeight = Screen.height;

        //float match = canvasScaler.matchWidthOrHeight;
        float match = canvasScaler.;


        float offect = (Screen.width / referenceWidth) * (1 - match) + (Screen.height / referenceHeight) * match;

        return offect;
    }
    */


    public static float GetCanvasScale(CanvasScaler canvasScaler)
    {
        var scalerReferenceResolution = canvasScaler.referenceResolution;
        var widthScale = Screen.width / scalerReferenceResolution.x;
        var heightScale = Screen.height / scalerReferenceResolution.y;

        switch (canvasScaler.screenMatchMode)
        {
            case CanvasScaler.ScreenMatchMode.MatchWidthOrHeight:
                var matchWidthOrHeight = canvasScaler.matchWidthOrHeight;

                return Mathf.Pow(widthScale, 1f - matchWidthOrHeight) *
                      Mathf.Pow(heightScale, matchWidthOrHeight);

            case CanvasScaler.ScreenMatchMode.Expand:
                return Mathf.Min(widthScale, heightScale);

            case CanvasScaler.ScreenMatchMode.Shrink:
                return Mathf.Max(widthScale, heightScale);

            default:
                throw new ArgumentOutOfRangeException();
        }

    }
}

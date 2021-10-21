using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ARRC_DigitalTwin_Generator
{
    public static class DownloadManager
    {
        private static int maxDownloadItem;

        public static long completeSize;
        public static bool isComplete;

        private static List<DownloadItem> activeItems;
        private static List<DownloadItem> items;
        private static long totalSize;

        public static int count
        {
            get
            {
                if (items == null) return -1;
                return items.Count;
            }
        }

        public static double progress
        {
            get
            {
                if (activeItems == null || activeItems.Count == 0) return 0;
                double localProgress = activeItems.Sum(i =>
                {
                    if (i.ignoreRequestProgress) return 0;
                    return (double)i.progress * i.averageSize;
                }) / totalSize;
                double totalProgress = completeSize / (double)totalSize + localProgress;
                return totalProgress;
            }
        }

        public static int totalSizeMB
        {
            get { return Mathf.RoundToInt(totalSize / (float)Utils.MB); }
        }

        public static void Add(DownloadItem item)
        {
            if (items == null) items = new List<DownloadItem>();
            items.Add(item);
        }

        public static bool CheckComplete()
        {
            if (isComplete)
                return true;

            foreach (DownloadItem item in activeItems) item.CheckComplete();

            activeItems.RemoveAll(i => i.complete);
            while (activeItems.Count < maxDownloadItem && items.Count > 0)
            {
                if (!StartNextDownload()) break;
            }
            if (activeItems.Count == 0 && items.Count == 0)
            {
                isComplete = true;
                return true;
            }
            else
                return false;
        }

        public static void Dispose()
        {
            if (activeItems != null)
            {
                foreach (DownloadItem item in activeItems) item.Dispose();
                activeItems = null;
            }

            items = null;
        }

        public static string EscapeURL(string url)
        {
#if UNITY_2018_3_OR_NEWER
            return UnityWebRequest.EscapeURL(url);
#else
            return WWW.EscapeURL(url);
#endif
        }

        public static void Start(List<DownloadItem> _items, int _maxDownloadItem)
        {
            items = _items;
            isComplete = false;
            completeSize = 0;
            maxDownloadItem = _maxDownloadItem;
            if (items == null || items.Count == 0)
            {
                isComplete = true;
                return;
            }

            activeItems = new List<DownloadItem>();

            try
            {
                totalSize = items.Sum(i => i.averageSize);
            }
            catch
            {
                //RealWorldTerrainWindow.isCapturing = false;
                Dispose();
                return;
            }

            CheckComplete();
        }

        public static void Start(int _maxDownloadItem)
        {
            isComplete = false;
            completeSize = 0;
            maxDownloadItem = _maxDownloadItem;
            if (items == null || items.Count == 0)
            {
                isComplete = true;
                return;
            }

            activeItems = new List<DownloadItem>();

            try
            {
                totalSize = items.Sum(i => i.averageSize);
            }
            catch
            {
                Dispose();
                return;
            }

            CheckComplete();
        }
        public static bool StartNextDownload()
        {
            if (items == null || items.Count == 0) return false;

            int index = 0;
            DownloadItem dItem = null;
            while (index < items.Count)
            {
                DownloadItem item = items[index];
                if (item.exclusiveLock != null)
                {
                    bool hasAnotherSomeRequest = false;
                    foreach (DownloadItem activeItem in activeItems)
                    {
                        if (item.exclusiveLock == activeItem.exclusiveLock)
                        {
                            hasAnotherSomeRequest = true;
                            break;
                        }
                    }
                    if (!hasAnotherSomeRequest)
                    {
                        dItem = item;
                        break;
                    }
                }
                else
                {
                    dItem = item;
                    break;
                }
                index++;
            }

            if (dItem == null) return false;

            items.RemoveAt(index);
            if (dItem.exists)
                return true;
            dItem.Start();
            activeItems.Add(dItem);
            return true;
        }

    }
}

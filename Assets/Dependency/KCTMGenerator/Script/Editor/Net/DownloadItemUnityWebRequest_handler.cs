using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using Newtonsoft.Json;

using KCTM.Network.Data;

namespace ARRC_DigitalTwin_Generator
{
    public class DownloadItemUnityWebRequest_KCTM : DownloadItem
    {
        public UnityWebRequest uwr;
        public Dictionary<string, string> headers;
        public Dictionary<string, string> headers_response;
        public delegate void SuccessHandler(Result result);
        public delegate void FailHandler(Result result);

        SuccessHandler successHandler;
        FailHandler failHandler;

        public DownloadItemUnityWebRequest_KCTM(string url, SuccessHandler successHandler, FailHandler failHandler) : this(UnityWebRequest.Get(url))
        {
            this.successHandler = successHandler;
            this.failHandler = failHandler;
        }

        public DownloadItemUnityWebRequest_KCTM(string url, Dictionary<string, string> data, SuccessHandler successHandler, FailHandler failHandler) : this(UnityWebRequest.Post(url, data))
        {
            this.successHandler = successHandler;
            this.failHandler = failHandler;
        }

        public DownloadItemUnityWebRequest_KCTM(UnityWebRequest uwr)
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
            get { return File.Exists(filename) || File.Exists(errorFilename); }
        }

        public override void CheckComplete()
        {
            if (!uwr.isDone) return;

            byte[] bytes = uwr.downloadHandler.data;

            if (string.IsNullOrEmpty(uwr.error))
            {
                SaveCookies(uwr.GetResponseHeader("Set-Cookie"));
                successHandler?.Invoke(ResponseToResult(bytes));
                DispatchCompete(ref bytes);
            }
            else
            {
                Debug.LogWarning("Download failed: " + uwr.url + "\n" + uwr.error);
                failHandler?.Invoke(ResponseToResult(bytes));
            }

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

        private Result ResponseToResult(byte[] response)
        {
            string responseString = Encoding.UTF8.GetString(response);
            Result result = JsonConvert.DeserializeObject<Result>(responseString);

            return result;
        }

        private void SaveCookies(string cookiesString)
        {
            if (cookiesString == null ||
                cookiesString.Length <= 0)
            {
                return;
            }

            string[] cookieStrings = cookiesString.Split(';');

            foreach (string cookie in cookieStrings)
            {
                string[] pair = cookie.Split('=');
                if (headers_response.ContainsKey(pair[0]))
                {
                    headers_response[pair[0]] = pair[1];
                }
                else
                {
                    headers_response.Add(pair[0], pair[1]);
                }
            }
        }
    }
}

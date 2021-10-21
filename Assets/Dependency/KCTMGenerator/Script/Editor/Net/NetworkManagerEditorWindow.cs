using System.Text;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using KCTM.Network.Data;
using Newtonsoft.Json;
using UnityEditor;
using EditorCoroutines;

namespace ARRC_DigitalTwin_Generator
{
    public class NetworkManagerEditorWindow
    {
        public class RequestInfo
        {
            public string url;
            public string method;
            public Dictionary<string, string> data;
        }

        public void Post(string uri, WWWForm formData, SuccessHandler successHandler, FailHandler failHandler)
        {
            Post(uri, formData, null, successHandler, failHandler);
        }

        public void Post(string uri, WWWForm formData, ResponseHandler responseHandler, SuccessHandler successHandler, FailHandler failHandler)
        {
            var request = UnityWebRequest.Post(AppendUri(uri), formData);
            request.chunkedTransfer = false;
            request.useHttpContinue = false;
            request.SetRequestHeader("Verified-Requested-With", "*");
            if (cookies.Count > 0)
            {
                request.SetRequestHeader("Cookie", ToCookieString());
            }


            editorWnd.StartCoroutine(WaitUntilDone(request.SendWebRequest(), responseHandler, successHandler, failHandler));
        }

        public void Post(string uri, Dictionary<string, string> data, SuccessHandler successHandler, FailHandler failHandler)
        {
            Post(uri, data, null, successHandler, failHandler);
        }

        public void Post(string uri, Dictionary<string, string> data, ResponseHandler responseHandler, SuccessHandler successHandler, FailHandler failHandler)
        {
            var request = UnityWebRequest.Post(AppendUri(uri), data);
            request.chunkedTransfer = false;
            request.useHttpContinue = false;
            request.SetRequestHeader("Verified-Requested-With", "*");
            if (cookies.Count > 0)
            {
                request.SetRequestHeader("Cookie", ToCookieString());
            }

            editorWnd.StartCoroutine(WaitUntilDone(request.SendWebRequest(), responseHandler, successHandler, failHandler));
        }

        public void Get(string uri, SuccessHandler successHandler, FailHandler failHandler)
        {
            Get(uri, null, successHandler, failHandler);
        }

        public void Get(string uri, ResponseHandler responseHandler, SuccessHandler successHandler, FailHandler failHandler)
        {
            var request = UnityWebRequest.Get(AppendUri(uri));
            request.SetRequestHeader("Cookie", ToCookieString());
            request.chunkedTransfer = false;

            editorWnd.StartCoroutine(WaitUntilDone(request.SendWebRequest(), responseHandler, successHandler, failHandler));
        }

        IEnumerator WaitUntilDone(UnityWebRequestAsyncOperation oper, ResponseHandler responseHandler, SuccessHandler successHandler, FailHandler failHandler)
        {
            while (!oper.isDone)
                yield return new WaitForFixedUpdate(); // Editor에서 다시 렌더링 되는 시간만큼 기다림. play에서는 null이 좋음.

            var request = oper.webRequest;

            responseHandler?.Invoke();

            if (request.isHttpError ||
                request.isNetworkError ||
                request.responseCode >= 400)
            {
                Debug.LogError(request.responseCode);
                failHandler?.Invoke(ResponseToResult(request.downloadHandler.data));
            }
            else
            {
                SaveCookies(request.GetResponseHeader("Set-Cookie"));
                successHandler?.Invoke(ResponseToResult(request.downloadHandler.data));
            }

            request.Dispose();
        }

        public void GetText(string uri, SuccessTextHandler successTextHandler, FailTextHandler failHandler)
        {
            var request = UnityWebRequest.Get(uri);
            request.chunkedTransfer = false;
            request.useHttpContinue = false;
            editorWnd.StartCoroutine(GetTextBackground(request.SendWebRequest(), successTextHandler, failHandler));
        }

        private IEnumerator GetTextBackground(UnityWebRequestAsyncOperation oper, SuccessTextHandler successTextHandler, FailTextHandler failTextHandler)
        {
            while (!oper.isDone)
                yield return null;

            var request = oper.webRequest;

            if (request.isHttpError ||
                request.isNetworkError ||
                request.responseCode >= 400)
            {
                failTextHandler(Encoding.UTF8.GetString(request.downloadHandler.data));
            }
            else
            {
                successTextHandler(Encoding.UTF8.GetString(request.downloadHandler.data));
            }
            request.Dispose();
        }

        public void GetTexture(string uri, SuccessTextureHandler successTextureHandler, FailTextHandler failTextHandler)
        {
            var request = UnityWebRequestTexture.GetTexture(uri);
            editorWnd.StartCoroutine(GetTextureBackground(request.SendWebRequest(),
                                            successTextureHandler,
                                            failTextHandler));
        }

        private IEnumerator GetTextureBackground(UnityWebRequestAsyncOperation oper, SuccessTextureHandler successTextureHandler, FailTextHandler failTextHandler)
        {
            while (!oper.isDone)
                yield return null;

            var request = oper.webRequest;

            if (request.isHttpError ||
                request.isNetworkError ||
                request.responseCode >= 400)
            {
                failTextHandler(Encoding.UTF8.GetString(request.downloadHandler.data));
            }
            else
            {
                successTextureHandler(DownloadHandlerTexture.GetContent(request));
            }

            request.Dispose();
        }

        private string ToCookieString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var pair in cookies)
            {
                if (builder.Length > 0)
                {
                    builder.Append(";");
                }

                builder.Append(pair.Key);
                builder.Append("=");
                builder.Append(pair.Value);
            }

            return builder.ToString();
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
                if (pair.Length != 2)
                    continue;
                if (cookies.ContainsKey(pair[0]))
                {
                    cookies[pair[0]] = pair[1];
                }
                else
                {
                    cookies.Add(pair[0], pair[1]);
                }
            }
        }

        public void SetCookie(string key, string value)
        {
            cookies.Add(key, value);
        }

        public void DeleteCookie(string key)
        {
            cookies.Remove(key);
        }

        public void ClearCookie()
        {
            cookies.Clear();
        }

        private Result ResponseToResult(byte[] response)
        {
            string responseString = Encoding.UTF8.GetString(response);
            Result result = JsonConvert.DeserializeObject<Result>(responseString);

            return result;
        }

        private string AppendUri(string uri)
        {
            StringBuilder sb = new StringBuilder();
            if (basicUri.EndsWith("/"))
            {
                if (uri.StartsWith("/"))
                {
                    sb.Append(basicUri);
                    sb.Append(uri.Remove(0));
                }
                else
                {
                    sb.Append(basicUri);
                    sb.Append(uri);
                }
            }
            else
            {
                if (uri.StartsWith("/"))
                {
                    sb.Append(basicUri);
                    sb.Append(uri);
                }
                else
                {
                    sb.Append(basicUri);
                    sb.Append("/");
                    sb.Append(uri);
                }
            }

            return sb.ToString();
        }

        private static NetworkManagerEditorWindow instance;
        public static NetworkManagerEditorWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NetworkManagerEditorWindow();
                }

                return instance;
            }
        }
        public static void Dispose()
        {
            instance = null;
        }
        public string basicUri = "";

        public Dictionary<string, string> cookies = new Dictionary<string, string>();
        public EditorWindow editorWnd;

        public delegate void ResponseHandler();

        public delegate void SuccessHandler(Result result);
        public delegate void FailHandler(Result result);

        public delegate void FailTextHandler(string text);

        public delegate void SuccessTextHandler(string text);
        public delegate void SuccessTextureHandler(Texture2D texture);
    }
}
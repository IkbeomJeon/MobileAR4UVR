using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Networking;
using KCTM.Network.Data;
using Newtonsoft.Json;

namespace KCTM
{
    namespace Network
    {
        public class NetworkManager : MonoBehaviour
        {
            public class RequestInfo
            {
                public string url;
                public string method;
                public Dictionary<string, string> data;
            }

            public string basicUri = "";

            public Dictionary<string, string> cookies;

            //public Dictionary<string, UnityWebRequest> processingRequests;

            public delegate void ResponseHandler();
            public delegate void SuccessHandler(Result result);
            public delegate void FailureHandler(Result result);
            public delegate void FailureTextHandler(string text);

            public delegate void SuccessTextHandler(string text);
            public delegate void SuccessTextureHandler(Texture2D texture);

            private static NetworkManager instance;

            public static NetworkManager Instance
            {
                get
                {
                    if (instance == null)
                    {
                        GameObject obj = GameObject.Find("NetworkManager");
                        if (obj == null)
                        {
                            obj = new GameObject("NetworkManager");
                            instance = obj.AddComponent<NetworkManager>();
                        }
                        else
                        {
                            instance = obj.GetComponent<NetworkManager>();
                        }
                        DontDestroyOnLoad(obj);
                    }

                    return instance;
                }
            }
  

            private void Awake()
            {
                instance = this;
                cookies = new Dictionary<string, string>();
                //processingRequests = new Dictionary<string, UnityWebRequest>();
            }


            public void Post(string uri, WWWForm formData, SuccessHandler successHandler, FailureHandler failureHandler)
            {
                Post(uri, formData, null, successHandler, failureHandler);
            }

            public void Post(string uri, WWWForm formData, ResponseHandler responseHandler, SuccessHandler successHandler, FailureHandler failureHandler)
            {
                /*
                if(processingRequests.ContainsKey(uri))
                {
                    processingRequests[uri].Abort();
                    processingRequests.Remove(uri);
                }
                */
                var request = UnityWebRequest.Post(AppendUri(uri), formData);
                request.chunkedTransfer = false;
                request.useHttpContinue = false;
                request.SetRequestHeader("Verified-Requested-With", "*");
                if (cookies.Count > 0)
                {
                    request.SetRequestHeader("Cookie", ToCookieString());
                }

                //processingRequests.Add(uri, request);

                StartCoroutine(WaitUntilDone(request.SendWebRequest(), responseHandler, successHandler, failureHandler));
            }

            public void Post(string uri, Dictionary<string, string> data, SuccessHandler successHandler, FailureHandler failureHandler)
            {
                Post(uri, data, null, successHandler, failureHandler);
            }

            public void Post(string uri, Dictionary<string, string> data, ResponseHandler responseHandler, SuccessHandler successHandler, FailureHandler failureHandler)
            {
                /*
                if (processingRequests.ContainsKey(uri))
                {
                    processingRequests[uri].Abort();
                    processingRequests.Remove(uri);
                }
                */
                var request = UnityWebRequest.Post(AppendUri(uri), data);
                request.useHttpContinue = false;
                request.chunkedTransfer = false;
                request.SetRequestHeader("Verified-Requested-With", "*");
                if (cookies.Count > 0)
                {
                    request.SetRequestHeader("Cookie", ToCookieString());
                }

                //processingRequests.Add(uri, request);

                StartCoroutine(WaitUntilDone(request.SendWebRequest(), responseHandler, successHandler, failureHandler));
            }

            public void Get(string uri, SuccessHandler successHandler, FailureHandler failureHandler)
            {
                Get(uri, null, successHandler, failureHandler);
            }

            public void Get(string uri, ResponseHandler responseHandler, SuccessHandler successHandler, FailureHandler failureHandler)
            {
                /*
                if (processingRequests.ContainsKey(uri))
                {
                    processingRequests[uri].Abort();
                    processingRequests.Remove(uri);
                }
                */
                var request = UnityWebRequest.Get(AppendUri(uri));
                request.SetRequestHeader("Cookie", ToCookieString());

                //processingRequests.Add(uri, request);

                StartCoroutine(WaitUntilDone(request.SendWebRequest(), responseHandler, successHandler, failureHandler));
            }

            
            IEnumerator WaitUntilDone(UnityWebRequestAsyncOperation oper, ResponseHandler responseHandler, SuccessHandler successHandler, FailureHandler failureHandler)
            {
                while (!oper.isDone)
                    yield return null;

                var request = oper.webRequest;

                responseHandler?.Invoke();

                if (request.result == UnityWebRequest.Result.ProtocolError||
                    request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.DataProcessingError ||
                    request.responseCode >= 400)
                {
                    Debug.LogWarning(request.responseCode + ":" + request.result.ToString());
                    //failureHandler?.Invoke(ResponseToResult(request.downloadHandler.data));
                }
                else
                {
                    SaveCookies(request.GetResponseHeader("Set-Cookie"));
                    successHandler?.Invoke(ResponseToResult(request.downloadHandler.data));
                }

                //processingRequests.Remove(request.uri.AbsolutePath);
                request.Dispose();
            }

            public void GetText(string uri, SuccessTextHandler successTextHandler, FailureTextHandler failureHandler)
            {
                /*
                if (processingRequests.ContainsKey(uri))
                {
                    processingRequests[uri].Abort();
                    processingRequests.Remove(uri);
                }
                */

                var request = UnityWebRequest.Get(uri);
                request.useHttpContinue = false;

               // processingRequests.Add(uri, request);

                StartCoroutine(GetTextBackground(request.SendWebRequest(), successTextHandler, failureHandler));
            }

            private IEnumerator GetTextBackground(UnityWebRequestAsyncOperation oper, SuccessTextHandler successTextHandler, FailureTextHandler failTextHandler)
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
                //processingRequests.Remove(request.uri.AbsolutePath);
                request.Dispose();
            }

            public void GetTexture(string uri, SuccessTextureHandler successTextureHandler, FailureTextHandler failTextHandler)
            {
                /*
                if (processingRequests.ContainsKey(uri))
                {
                    processingRequests[uri].Abort();
                    processingRequests.Remove(uri);
                }
                */
                var request = UnityWebRequestTexture.GetTexture(uri);
                //processingRequests.Add(uri, request);

                StartCoroutine(GetTextureBackground(request.SendWebRequest(),
                                                successTextureHandler,
                                                failTextHandler));
            }

            private IEnumerator GetTextureBackground(UnityWebRequestAsyncOperation oper, SuccessTextureHandler successTextureHandler, FailureTextHandler failTextHandler)
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
                //processingRequests.Remove(request.uri.AbsolutePath);
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
                    else if (cookies.ContainsKey(pair[0]))
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
                if(basicUri.EndsWith("/"))
                {
                    if(uri.StartsWith("/"))
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
                else if (uri.Contains("localhost:8080"))  //For recommendation Test -- Maryam
                {
                    sb.Append(uri);
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

            
        }
    }
}
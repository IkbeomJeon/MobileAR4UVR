using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ARRC_DigitalTwin_Generator;
using KCTM.Network;
using KCTM.Network.Data;
using Newtonsoft.Json;
using System.Text;
using EditorCoroutines;

namespace KCTM
{
    public class IntroManagerEditorWindow : UnityEditor.EditorWindow
    {
        public static IntroManagerEditorWindow wnd;

        //public string uri = "http://54.180.86.59:9000";
        public string uri = "http://13.209.21.131:9000";


        string email = "asdf@asdf.com";
        string password = "123456";

        [MenuItem("Tools/KCTM Metadata Login")]
        public static IntroManagerEditorWindow OpenWindow()
        {
            wnd = GetWindow<IntroManagerEditorWindow>(false, "KCTM Metadata Login");

            return wnd;
        }

        void Awake()
        {
            Debug.Log("Awake");

            if (PlayerPrefs.HasKey("email") && PlayerPrefs.HasKey("password"))
            {
                email = PlayerPrefs.GetString("email");
                password = PlayerPrefs.GetString("password");
                Signin();
            }
        }

        void Start()
        {
            Debug.Log("Start");
        }

        private void OnGUI()
        {
            uri = EditorGUILayout.TextField("uri", uri);

            email = EditorGUILayout.TextField("email", email);
            password = EditorGUILayout.PasswordField("Password:", password);

            if (GUILayout.Button("Signin"))
            {
                Signin();
            }
        }

        public void Signin()
        {
            //Sign in.
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("email", email);
            data.Add("password", password);

            NetworkManagerEditorWindow.Instance.editorWnd = this;
            NetworkManagerEditorWindow.Instance.basicUri = uri;
            NetworkManagerEditorWindow.Instance.Post("/user/signin", data, ResponseHandler, SiginCallback, FailCallback);
        }


        private void SiginCallback(Result result)
        {
            AccountInfo.Instance.user = JsonConvert.DeserializeObject<User>(result.result.ToString());

            PlayerPrefs.SetString("email", email);
            PlayerPrefs.SetString("password", password);
            PlayerPrefs.Save();

            GetFriendList(AccountInfo.Instance.user.id);

            Debug.Log("Succed to signin");
        }
        private void ResponseHandler()
        {
            //loadingPanel.SetActive(false);
        }
        private void FailCallback(Result result)
        {
            Debug.LogError(result.error + " : " + result.msg);

            //signinPanel.SetActive(true);
            //signupPanel.SetActive(false);
        }
        public void GetFriendList(long id)
        {
            //NetworkManager.Instance.Get("/user/friend/list", FriendListCallback, FailCallback);
        }
        private void FriendListCallback(Result result)
        {
            AccountInfo.Instance.userRelationships = JsonConvert.DeserializeObject<List<User>>(result.result.ToString());

            //var oper = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
            //loadingPanel.SetActive(true);
            //StartCoroutine(LoadScene(oper));
        }


    }
}
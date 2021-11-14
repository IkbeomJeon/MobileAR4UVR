using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ContentLinkConverter : EditorWindow
{
    //public List<string> address = new List<string>();
    string basicURL = "https://arrc.synology.me:8989/fsdownload";

    public string before = "http://gofile.me/47gCy/KXDBVHqS5";
    public string filename = "테스트.mp4";

    string url;
    public void OnGUI()
    {

        before = EditorGUILayout.TextField("공유 주소 입력", before);
        filename = EditorGUILayout.TextField("파일명(확장자 포함)", filename);



        if (GUILayout.Button("변환"))
        {
            if (!filename.Contains("."))
                Debug.LogError("파일이름에 확장자 입력 바람.");

            else
            {
                char[] tokens = { '/', };

                string[] parts = before.Split(tokens);
                string uid = parts[parts.Length - 1];

                url = string.Format("{0}/{1}/{2}", basicURL, uid, WWW.EscapeURL(filename));
            }
        }
        EditorGUILayout.TextField("변환 결과", url);
    }

    static EditorWindow wnd;
    [MenuItem("Tools/KCTM/Address Converter")]
    public static EditorWindow OpenWindow()
    {
        wnd = GetWindow<ContentLinkConverter>(false, "Address Converter");

        return wnd;
    }

}

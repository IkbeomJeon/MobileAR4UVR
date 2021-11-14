using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ContentLinkConverter : EditorWindow
{
    //public List<string> address = new List<string>();
    string basicURL = "https://arrc.synology.me:8989/fsdownload";

    public string before = "http://gofile.me/47gCy/KXDBVHqS5";
    public string filename = "�׽�Ʈ.mp4";

    string url;
    public void OnGUI()
    {

        before = EditorGUILayout.TextField("���� �ּ� �Է�", before);
        filename = EditorGUILayout.TextField("���ϸ�(Ȯ���� ����)", filename);



        if (GUILayout.Button("��ȯ"))
        {
            if (!filename.Contains("."))
                Debug.LogError("�����̸��� Ȯ���� �Է� �ٶ�.");

            else
            {
                char[] tokens = { '/', };

                string[] parts = before.Split(tokens);
                string uid = parts[parts.Length - 1];

                url = string.Format("{0}/{1}/{2}", basicURL, uid, WWW.EscapeURL(filename));
            }
        }
        EditorGUILayout.TextField("��ȯ ���", url);
    }

    static EditorWindow wnd;
    [MenuItem("Tools/KCTM/Address Converter")]
    public static EditorWindow OpenWindow()
    {
        wnd = GetWindow<ContentLinkConverter>(false, "Address Converter");

        return wnd;
    }

}

using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

// Class: ServerURL
// Saves url address of server. All functions should reference from this address.
public class ServerURL : MonoBehaviour
{
    
    private static ServerURL instance;

    public string uri = "http://13.209.21.131:9000";
    //public string uri = "http://54.180.86.59:9000";
    //public string uri = "http://143.248.97.210:9000";

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

	public static ServerURL Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject obj = GameObject.Find("Server");
				if (obj == null)
				{
					obj = new GameObject("Server");

					instance = obj.AddComponent<ServerURL>();
				}
				else
				{
					instance = obj.GetComponent<ServerURL>();
				}
			}

			return instance;
		}
	}
}

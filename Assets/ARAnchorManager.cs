using KCTM.Network.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARAnchorManager : MonoBehaviour
{

    public List<Anchor> all_anchorList;

    private static ARAnchorManager instance;

    public static ARAnchorManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = GameObject.Find("ARAnchorManager");
                if (obj == null)
                {
                    obj = new GameObject("ARAnchorManager");
                    instance = obj.AddComponent<ARAnchorManager>();
                }
                else
                {
                    instance = obj.GetComponent<ARAnchorManager>();
                }
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }

}

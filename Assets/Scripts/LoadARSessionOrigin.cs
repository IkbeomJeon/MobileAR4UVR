using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadARSessionOrigin : MonoBehaviour
{
    public GameObject arsessionOrigin;
    
    private void Awake()
    {
        GameObject prefab;
        if (Application.platform == RuntimePlatform.WindowsEditor)
            prefab = Resources.Load("Prefabs/ARSession/AR Session Origin_FPS") as GameObject;

        else
            prefab = Resources.Load("Prefabs/ARSession/AR Session Origin") as GameObject;

        arsessionOrigin = Instantiate(prefab);
    }

}

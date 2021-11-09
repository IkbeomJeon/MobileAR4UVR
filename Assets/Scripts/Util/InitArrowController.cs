using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitArrowController : MonoBehaviour
{
    public TrackerClientManager[] trackerClientManager;
    void Start()
    {
        trackerClientManager = GameObject.FindObjectsOfType<TrackerClientManager>();
        if(trackerClientManager == null && trackerClientManager.Length!=1)
        {
            Debug.LogError("Can't find \"TrackerClientManager\" ");
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int dir_num = i;
                var button = transform.GetChild(i).gameObject.GetComponent<Button>();
                button.onClick.AddListener(delegate { trackerClientManager[0].UpdateGlobalPositionManually(dir_num); });
            }
        }
       
    }

   
}

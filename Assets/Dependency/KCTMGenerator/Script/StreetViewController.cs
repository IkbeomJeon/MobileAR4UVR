using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
#endif   

public class StreetViewController : MonoBehaviour
{

    public GameObject panoCam;
    public List<GameObject> imgObjList;
    public bool isSelected = false;
    public float camera_farClipPlane = 200;

    float px, py;
    bool showBuilding = true;

    void OnDrawGizmos()
    {
        if (!isSelected)
            Gizmos.color = new Color(1f, 1f, 1f, 0.8f);
        else
            Gizmos.color = new Color(1f, 0f, 0f, 0.8f);

        Gizmos.DrawSphere(transform.position, transform.lossyScale.x);
        Gizmos.color = new Color(1f, 0f, 1f, 1f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);

    }

    public void SetInitialState()
    {
        isSelected = false;
        //panoCam.GetComponent<Camera>().enabled = false;
        panoCam.SetActive(false);
        panoCam.GetComponent<Camera>().farClipPlane = camera_farClipPlane;
        GetComponent<Renderer>().enabled = false;
        //streetViewParentObject.GetComponent<StreetView>
    }
    public void SetSelectedState()
    {
        isSelected = true;
        panoCam.SetActive(true);
        //panoCam.GetComponent<Camera>().enabled = true;
        GetComponent<Renderer>().enabled = true;
    }

    void OnGUI()
    {
        //Debug.Log("1");
        if (isSelected)
        {
#if UNITY_EDITOR
            if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint)
            {
                EditorUtility.SetDirty(this); // this is important, if omitted, "Mouse down" will not be display
            }
#endif
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0) //left mouse down
            {
                Event e = Event.current;
                px = e.mousePosition.x;
                py = e.mousePosition.y;
            }
            else if (Event.current.type == EventType.MouseDown && Event.current.button == 1) //right mouse down
            {
                if (showBuilding)
                {
                    panoCam.GetComponent<Camera>().cullingMask = 1;
                }
                else
                {
                    panoCam.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("Building");
                }
                showBuilding = !showBuilding;
            }
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                Event e = Event.current;


                float cx = e.mousePosition.x;
                float cy = e.mousePosition.y;

                float angle_axisY = (px - cx);
                float angle_axisX = (py - cy);

                panoCam.transform.Rotate(new Vector3(0, angle_axisY, 0), Space.World);
                panoCam.transform.Rotate(new Vector3(angle_axisX, 0, 0), Space.Self);

                px = cx;
                py = cy;

            }
        }

    }
}



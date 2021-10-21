using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResizeChildObject : MonoBehaviour
{
    public float imageObjectSize = 1f;

    public void ResizeImageObjects()
    {
        Vector3 newScale;

        newScale.x = imageObjectSize / transform.localScale.x;
        newScale.y = imageObjectSize / transform.localScale.y;
        newScale.z = imageObjectSize / transform.localScale.z;


        foreach (Transform child in transform)
        {
            child.localScale = newScale;
        }

    }
}

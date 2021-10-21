using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ResizeChildObject))]

public class ResizeChildObjectEditor : Editor
{
    void OnSceneGUI()
    {
        ResizeChildObject script = ((ResizeChildObject)target);
        script.ResizeImageObjects();
    }

}


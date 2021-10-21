using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StreetViewController))]
public class StreetViewControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!((StreetViewController)target).isSelected)
        {
            foreach (GameObject go in ((StreetViewController)target).imgObjList)
                go.GetComponent<StreetViewController>().SetInitialState();

            ((StreetViewController)target).SetSelectedState();
        }

    }
}

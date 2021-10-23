using UnityEngine;
using System.Collections;
using UnityEditor;

// Creates a custom Label on the inspector for all the scripts named ScriptName
// Make sure you have a ScriptName script in your
// project, else this will not work.

[CustomEditor(typeof(ChangeMaterial))]
public class ChangeMaterialEditor : Editor
{
    ChangeMaterial script;


    private void OnEnable()
    {
        script = (ChangeMaterial)target;
        script.GetRendererFromChildren();
    }

    public override void OnInspectorGUI()
	{
        base.OnInspectorGUI();

        Color mainColor = script.mainColor;
        Color outlineColor = script.outlineColor;
        float alpha = script.alpha;
        float outlineWidth = script.outlineWidth;

        script.SetAlpha(alpha);
        script.SetColor(mainColor, outlineColor);
        script.SetWidth(outlineWidth);

        if(GUILayout.Button("Set Alpha Outline Material"))
        {
            script.SetAlphaOutlineShader();
        }
    }

}
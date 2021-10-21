// Version 2.3.3
// �2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using PrimitivesPro.Editor;
using PrimitivesPro.Primitives;
using UnityEditor;

[CustomEditor(typeof(PrimitivesPro.GameObjects.GeoSphere))]
public class CreateGeoSphere : CreatePrimitive
{
    private bool useFlipNormals = false;

    [MenuItem(MenuDefinition.GeoSphere)]
    public static void Create()
    {
        var obj = PrimitivesPro.GameObjects.GeoSphere.Create(1, 2, GeoSpherePrimitive.BaseType.Icosahedron, NormalsType.Vertex, PivotPosition.Center);
        obj.SaveStateAll();

        Selection.activeGameObject = obj.gameObject;
    }

    public override void OnInspectorGUI()
    {
        if (!base.IsValid())
        {
            return;
        }

        var obj = Selection.activeGameObject.GetComponent<PrimitivesPro.GameObjects.GeoSphere>();

        if (target != obj)
        {
            return;
        }

		PrimitivesPro.Editor.Utils.Toggle("Show scene handles", ref obj.showSceneHandles);
		bool colliderChange = PrimitivesPro.Editor.Utils.MeshColliderSelection(obj);

        EditorGUILayout.Separator();

        useFlipNormals = obj.flipNormals;
        bool uiChange = false;

		uiChange |= PrimitivesPro.Editor.Utils.SliderEdit("Radius", 0, 100, ref obj.radius);
		uiChange |= PrimitivesPro.Editor.Utils.SliderEdit("Subdivision", 0, 6, ref obj.subdivision);

        var oldBaseType = obj.baseType;
        EditorGUILayout.BeginHorizontal();
        obj.baseType = (GeoSpherePrimitive.BaseType)EditorGUILayout.EnumPopup("Base type", obj.baseType);
        EditorGUILayout.EndHorizontal();
        uiChange |= oldBaseType != obj.baseType;

        EditorGUILayout.Separator();

		uiChange |= PrimitivesPro.Editor.Utils.NormalsType(ref obj.normalsType);
		uiChange |= PrimitivesPro.Editor.Utils.PivotPosition(ref obj.pivotPosition);

		uiChange |= PrimitivesPro.Editor.Utils.Toggle("Flip normals", ref useFlipNormals);
		uiChange |= PrimitivesPro.Editor.Utils.Toggle("Share material", ref obj.shareMaterial);
		uiChange |= PrimitivesPro.Editor.Utils.Toggle("Fit collider", ref obj.fitColliderOnChange);

		PrimitivesPro.Editor.Utils.StatWindow(Selection.activeGameObject);

		PrimitivesPro.Editor.Utils.ShowButtons<PrimitivesPro.GameObjects.GeoSphere>(this);

        if (uiChange || colliderChange)
        {
            if (obj.generationMode == 0 && !colliderChange)
            {
                obj.GenerateGeometry();

                if (useFlipNormals)
                {
                    obj.FlipNormals();
                }
            }
            else
            {
                obj.GenerateColliderGeometry();
            }
        }
    }
}

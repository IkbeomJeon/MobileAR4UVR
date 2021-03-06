namespace Mapbox.Editor
{
	using UnityEditor;
	using UnityEngine;
	using Mapbox.Unity.Map;

	[CustomPropertyDrawer(typeof(CameraBoundsTileProviderOptions))]
	public class CameraBoundsTileProviderOptionsDrawer : PropertyDrawer
	{
		static float _lineHeight = EditorGUIUtility.singleLineHeight;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var camera = property.FindPropertyRelative("camera");
			EditorGUILayout.PropertyField(camera, new GUIContent
			{
				text = camera.displayName,
				tooltip = "Camera to control map extent."
			}, GUILayout.Height(_lineHeight));
		}
	}
}
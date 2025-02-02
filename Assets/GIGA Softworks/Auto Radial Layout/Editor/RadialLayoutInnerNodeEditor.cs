#if UNITY_EDITOR
using GIGA.AutoRadialLayout;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GIGA.AutoRadialLayout.Editor
{
	[CustomEditor(typeof(RadialLayoutInnerNode))]
	public class RadialLayoutInnerNodeEditor : UnityEditor.Editor
	{
		// Serialized Properties
		SerializedProperty s_layout;
		SerializedProperty s_nodeRadius;

		private void OnEnable()
		{
			s_layout = serializedObject.FindProperty("layout");
			s_nodeRadius = serializedObject.FindProperty("nodeRadius");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var t = target as RadialLayoutInnerNode;

			s_layout.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Layout", ""), s_layout.objectReferenceValue, typeof(RadialLayout), false);

			EditorGUILayout.BeginHorizontal();
			s_nodeRadius.floatValue = EditorGUILayout.FloatField(new GUIContent("Node Radius", "Radius of the node, this will determine the effective starting/ending point when filling links progress."), s_nodeRadius.floatValue);
			t.showNodeRadiusGizmo = EditorGUILayout.Toggle(new GUIContent("Show", ""), t.showNodeRadiusGizmo);
			EditorGUILayout.EndHorizontal();

			serializedObject.ApplyModifiedProperties();

		}
	}
}

#endif

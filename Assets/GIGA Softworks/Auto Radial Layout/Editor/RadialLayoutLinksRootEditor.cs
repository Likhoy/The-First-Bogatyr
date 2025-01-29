#if UNITY_EDITOR
using GIGA.AutoRadialLayout;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GIGA.AutoRadialLayout.Editor
{
	[CustomEditor(typeof(RadialLayoutLinksRoot))]
	public class RadialLayoutLinksRootEditor : UnityEditor.Editor
	{
		// Serialized Properties
		SerializedProperty s_parentLayout;

		private void OnEnable()
		{
			s_parentLayout = serializedObject.FindProperty("parentLayout");

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var t = target as RadialLayoutLinksRoot;

			EditorGUI.BeginDisabledGroup(true);
			s_parentLayout.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Parent Layout", "The layout this object is associated to"), s_parentLayout.objectReferenceValue, typeof(RadialLayout), false);
			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();

		}
	}
}

#endif

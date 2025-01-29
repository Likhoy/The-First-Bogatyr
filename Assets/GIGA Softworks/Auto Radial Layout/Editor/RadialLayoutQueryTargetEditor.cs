#if UNITY_EDITOR
using GIGA.AutoRadialLayout.QuerySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GIGA.AutoRadialLayout.Editor
{
	[CustomEditor(typeof(RadialLayoutQueryTarget))]
	[CanEditMultipleObjects]
	public class RadialLayoutQueryTargetEditor : UnityEditor.Editor
	{
		// Serialized Properties
		SerializedProperty s_tags;

		private void OnEnable()
		{
			s_tags = serializedObject.FindProperty("tags");

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var t = target as RadialLayoutQueryTarget;

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.IntField(new GUIContent("Unique ID", "Unique integer identifier assigned to this element."), t.UniqueId);
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.PropertyField(s_tags, new GUIContent("Tags", ""));


			serializedObject.ApplyModifiedProperties();

		}
	}
}

#endif

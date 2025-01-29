#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GIGA.AutoRadialLayout.Editor
{
	[CustomEditor(typeof(RadialLayoutLink))]
	public class RadialLayoutLinkEditor : UnityEditor.Editor
	{
		private void OnEnable()
		{

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var t = target as RadialLayoutLink;

			DrawDefaultInspector();

			if (t.progress != null)
			{
				float prevProgress = t.ProgressValue;
				float progress = EditorGUILayout.Slider(new GUIContent("Progress Value","Used to preview the fill effect on this link. This value will NOT be serialized."), t.ProgressValue, 0, 1);
				if (progress != prevProgress)
				{
					t.ProgressValue = progress;
					UnityEditor.EditorUtility.SetDirty(t.progress);
				}
			}
			else
			{
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.Slider("Progress Value", 0, 0, 1);
				EditorGUI.EndDisabledGroup();
			}

			serializedObject.ApplyModifiedProperties();



		}
	}
}

#endif

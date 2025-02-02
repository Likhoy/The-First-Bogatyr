#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GIGA.AutoRadialLayout.Editor
{
	[CustomEditor(typeof(RadialLayoutNode))]
	public class RadialLayoutNodeEditor : UnityEditor.Editor
	{
		// Styles
		GUIStyle style_header;

		// Serialized Properties
		SerializedProperty s_distanceOffset;
		SerializedProperty s_angleOffset;
		SerializedProperty s_fanOffset;
		SerializedProperty s_fanSpanOverride;
		SerializedProperty s_overrideFanSpan;
		SerializedProperty s_branchLength;
		SerializedProperty s_overrideBranchLength;
		SerializedProperty s_nodeRadius;

		private void OnEnable()
		{
			s_distanceOffset = serializedObject.FindProperty("distanceOffset");
			s_angleOffset = serializedObject.FindProperty("angleOffset");
			s_fanOffset = serializedObject.FindProperty("fanOffset");
			s_fanSpanOverride = serializedObject.FindProperty("fanSpanOverride");
			s_overrideFanSpan = serializedObject.FindProperty("overrideFanSpan");
			s_branchLength = serializedObject.FindProperty("branchLength");
			s_overrideBranchLength = serializedObject.FindProperty("overrideBranchLength");
			s_nodeRadius = serializedObject.FindProperty("nodeRadius");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var t = target as RadialLayoutNode;

			// Creating styles if null
			if (this.style_header == null)
			{
				this.style_header = new GUIStyle("box");
				style_header.stretchWidth = true;
				style_header.normal.background = RadialLayoutEditor.MakeTex(2, 2, RadialLayoutEditor.headerColor);
				style_header.normal.textColor = Color.white;
				style_header.alignment = TextAnchor.MiddleLeft;
				style_header.fontStyle = FontStyle.Bold;
			}

			// Pre-checks
			bool isSubLayout = t.IsSubLayout;


			EditorGUILayout.Space(10);

			if (isSubLayout)
			{
				EditorGUILayout.HelpBox("This node is a sub-layout", MessageType.Info);
				RadialLayoutEditor.BeginSelectorButtonGroup();
				if (GUILayout.Button("Select Sub-Layout"))
				{
					Selection.activeGameObject = t.GetSubLayout().gameObject;
				}
				RadialLayoutEditor.EndSelectorButtonGroup();
			}

			// Settings
			EditorGUILayout.BeginHorizontal(style_header);
			EditorGUILayout.LabelField("Node Settings",style_header);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel++;
			s_distanceOffset.floatValue = EditorGUILayout.FloatField(new GUIContent("Distance Offset", "Changes the distance of the node from the default placement."), s_distanceOffset.floatValue);
			s_angleOffset.floatValue = EditorGUILayout.FloatField(new GUIContent("Angle Offset", "Changes the angle of the node from the default placement."), s_angleOffset.floatValue);
			EditorGUI.BeginDisabledGroup(!t.HasChildren);
			s_fanOffset.floatValue = EditorGUILayout.FloatField(new GUIContent("Fan Offset", "Changes the angle of every child nodes."), s_fanOffset.floatValue);
			EditorGUI.EndDisabledGroup();

			// fan span
			EditorGUILayout.BeginHorizontal();
			if (t.gameObject.scene.name != null && t.Layout != null)
			{
				// Prefab is in scene and inside a layout
				if (!t.overrideFanSpan)
				{
					EditorGUI.BeginDisabledGroup(true);
					if (t.Layout.nodesDistribution == RadialLayout.NodesDistribution.Branches)
						EditorGUILayout.FloatField(new GUIContent("Fan Span", "Angle in degree of the child nodes fan amplitude."), t.FanSpan);
					else
					{
						EditorGUILayout.FloatField(new GUIContent("Fan Span Modifier", "Modifier in degree from the default amplitude of the child nodes fan."), 0);
						// Resetting
						s_fanSpanOverride.floatValue = t.CalculateDefaultFanSpan(); 
						t.ResetUpdateParameters();
					}
					EditorGUI.EndDisabledGroup();
				}
				else
				{
					if (t.Layout.nodesDistribution == RadialLayout.NodesDistribution.Branches)
						s_fanSpanOverride.floatValue = EditorGUILayout.FloatField(new GUIContent("Fan Span", "Angle in degree of the child nodes fan amplitude."), s_fanSpanOverride.floatValue);
					else
					{
						float defaultValue = t.CalculateDefaultFanSpan();
						float modifierValue = EditorGUILayout.FloatField(new GUIContent("Fan Span Modifier", "Modifier in degree from the default amplitude of the child nodes fan."), s_fanSpanOverride.floatValue - defaultValue);
						s_fanSpanOverride.floatValue = defaultValue + modifierValue;
						t.ResetUpdateParameters();
					}
				}
			}
			else
			{
				// Prefab is not instantiated yet
				if (!t.overrideFanSpan)
				{
					EditorGUI.BeginDisabledGroup(true);
					EditorGUILayout.FloatField(new GUIContent("Fan Span", "Angle in degree of the child nodes fan amplitude."), t.FanSpan);
					EditorGUI.EndDisabledGroup();
				}
				else
				{
					s_fanSpanOverride.floatValue = EditorGUILayout.FloatField(new GUIContent("Fan Span", "Angle in degree of the child nodes fan amplitude."), s_fanSpanOverride.floatValue);
				}
			}
			s_overrideFanSpan.boolValue = EditorGUILayout.Toggle(new GUIContent("Override", "Override"), s_overrideFanSpan.boolValue);
			EditorGUILayout.EndHorizontal();

			// branch length
			if (t.Layout != null && t.Layout.nodesDistribution == RadialLayout.NodesDistribution.Branches)
			{
				EditorGUILayout.BeginHorizontal();
				if (t.gameObject.scene.name != null && t.Layout != null)
				{
					// Prefab is in scene and inside a layout
					if (!t.overrideBranchLength)
					{
						EditorGUI.BeginDisabledGroup(true);
						t.branchLength = t.BranchLength - t.Layout.circleRadius; // Resetting to default value
						EditorGUILayout.FloatField(new GUIContent("Branches Length", "Branches length"), t.branchLength);
						EditorGUI.EndDisabledGroup();
					}
					else
					{
						s_branchLength.floatValue = EditorGUILayout.FloatField(new GUIContent("Branches Length", "Branches length"), s_branchLength.floatValue);
					}
				}
				else
				{
					// Prefab is not instantiated yet
					if (!t.overrideBranchLength)
					{
						EditorGUI.BeginDisabledGroup(true);
						EditorGUILayout.FloatField(new GUIContent("Branches Length", "Branches length"), t.branchLength);
						EditorGUI.EndDisabledGroup();
					}
					else
					{
						s_branchLength.floatValue = EditorGUILayout.FloatField(new GUIContent("Branches Length", "Branches length"), s_branchLength.floatValue);
					}
				}
				s_overrideBranchLength.boolValue = EditorGUILayout.Toggle(new GUIContent("Override", "Override"), s_overrideBranchLength.boolValue);
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			s_nodeRadius.floatValue = EditorGUILayout.FloatField(new GUIContent("Node Radius", "Radius of the node, this will determine the effective starting/ending point when filling links progress."), s_nodeRadius.floatValue);
			t.showNodeRadiusGizmo = EditorGUILayout.Toggle(new GUIContent("Show", ""), t.showNodeRadiusGizmo);

			EditorGUILayout.EndHorizontal();


			EditorGUI.indentLevel--;

			// Actions
			EditorGUILayout.Space(10);
			EditorGUILayout.BeginHorizontal(style_header);
			EditorGUILayout.LabelField("Actions" + (isSubLayout ? " (Some actions will affect the sub-layout)" : ""),style_header);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel++;

			if (t.Layout != null)
			{

				if (GUILayout.Button("Add Child Node"))
				{
					if (!isSubLayout)
						t.Layout.AddNode("", t);
					else
						t.GetSubLayout().AddNode();

					EditorUtility.SetDirty(t);
				}

				if (GUILayout.Button("Delete This Node"))
				{
					if (t.HasChildren)
					{
						if (EditorUtility.DisplayDialog("Warning", "This node has child nodes, every child will be destroyed and this action cannot be undone. Do you want to continue?", "Yes", "No"))
						{
							t.Layout.DeleteNode(t, true);
						}
					}
					else if (isSubLayout)
					{
						if (EditorUtility.DisplayDialog("Warning", "This node is a sub-layout. Each node in the sub-layout will be deleted and this action cannot be undone. Do you want to continue?", "Yes", "No"))
						{
							t.GetSubLayout().Destroy();
							t.Layout.DeleteNode(t, true);
						}
					}

					else
					{
						t.Layout.DeleteNode(t, false);
					}
					return;
				}

				EditorGUI.BeginDisabledGroup(!t.HasChildren);
				{
					if (GUILayout.Button("Delete Child Nodes"))
					{
						if (t.HasChildren)
						{
							if (EditorUtility.DisplayDialog("Warning", "Every child will be destroyed and this action cannot be undone. Do you want to continue?", "Yes", "No"))
							{
								foreach (var childNode in t.GetChildNodes())
									t.Layout.DeleteNode(childNode, true);
								EditorUtility.SetDirty(t);
							}
						}
						return;
					}
				}

				EditorGUI.EndDisabledGroup();

				if (!isSubLayout)
				{
					if (GUILayout.Button("Convert to sub-layout"))
					{
						if (EditorUtility.DisplayDialog("Warning", "This will convert current node to a sub-layout. Do you want to continue?", "Yes", "No"))
						{
							t.ConvertToSubLayout();
							EditorUtility.SetDirty(t);
						}
					}
				}
				else
				{
					if (GUILayout.Button("Convert to node"))
					{
						if (EditorUtility.DisplayDialog("Warning", "This will convert current sub-layout to standard node. Do you want to continue?", "Yes", "No"))
						{
							t.ConvertToNode();
							EditorUtility.SetDirty(t);
						}
					}
				}


				if (GUILayout.Button("Clear Offsets"))
				{
					if (EditorUtility.DisplayDialog("Clear Offsets", "Clear all position offsets and restore original node position?", "Yes", "No"))
					{
						t.ClearOffsets();
						EditorUtility.SetDirty(t);
					}
				}

				RadialLayoutEditor.BeginSelectorButtonGroup();
				EditorGUI.BeginDisabledGroup(t.ParentNode == null && t.Layout == null);
				if (GUILayout.Button("Select Parent Node"))
				{
					if(t.ParentNode != null)
						Selection.activeGameObject = t.ParentNode.gameObject;
					else if(t.Layout != null)
						Selection.activeGameObject = t.Layout.gameObject;
				}
				EditorGUI.EndDisabledGroup();

				EditorGUI.BeginDisabledGroup(!t.HasChildren || isSubLayout);
				if (GUILayout.Button("Select Child Node"))
				{
					Selection.activeObject = t.GetChildNodes()[0].gameObject;
				}
				EditorGUI.EndDisabledGroup();

				EditorGUI.BeginDisabledGroup(t.GetSiblingsCount() < 2);
				if (GUILayout.Button("Select Sibling Node"))
				{
					if (t.depth > 0)
						Selection.activeObject = t.ParentNode.GetChildNodes()[(t.GetSiblingIndex() + 1) % t.GetSiblingsCount()].gameObject;
					else
					{
						List<RadialLayoutNode> rootNodes = new List<RadialLayoutNode>();
						foreach (var n in t.Layout.Nodes)
							if (n.depth == 0)
								rootNodes.Add(n);
						Selection.activeObject = rootNodes[(rootNodes.IndexOf(t) + 1) % rootNodes.Count].gameObject;
					}
				}
				EditorGUI.EndDisabledGroup();

				if (GUILayout.Button("Select Parent Layout"))
				{
					Selection.activeObject = t.Layout.gameObject;
				}

				RadialLayoutEditor.EndSelectorButtonGroup();
			}

			EditorGUI.indentLevel--;

			// Version label
			EditorGUILayout.Space(10);
			GUIStyle rightcentederLabel = new GUIStyle(GUI.skin.label);
			rightcentederLabel.fontStyle = FontStyle.Italic;
			rightcentederLabel.fontSize = Mathf.RoundToInt(GUI.skin.label.fontSize * 0.85f);
			rightcentederLabel.alignment = TextAnchor.UpperRight;
			rightcentederLabel.normal.textColor = new Color(0.25f, 0.25f, 0.25f, 1f);
			if (GUILayout.Button("GIGA Softworks - Auto Radial Layout ver. " + RadialLayout.VERSION, rightcentederLabel))
				RadialLayoutAboutDialog.Init();

			serializedObject.ApplyModifiedProperties();
		}
	}
}

#endif

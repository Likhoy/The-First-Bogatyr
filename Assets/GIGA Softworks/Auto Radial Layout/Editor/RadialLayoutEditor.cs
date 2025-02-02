#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GIGA.AutoRadialLayout.Editor
{
	[CustomEditor(typeof(RadialLayout))]
	public class RadialLayoutEditor : UnityEditor.Editor
	{

		private const float MAX_HANDLE_SIZE = 10;
		private const float MIN_HANDLE_SIZE = 1;
		public static Color headerColor = new Color32(0X14, 0X5C, 0X9E, 255);
		public static Color selectorButtonColor = new Color32(0xa5, 0xd8, 0xff, 255);
		public static Color color3 = new Color32(0x0B, 0x4F, 0x6C, 255);
		public static Color color4 = new Color32(0xC5, 0xD8, 0x6D, 255);
		public static Color color5 = new Color32(0xEC, 0xA4, 0x00, 255);

		// Styles
		GUIStyle style_header;
		GUIStyle style_header_editor;
		private static Color exColor;

		// Serialized Properties
		SerializedProperty s_autoRebuildMode;
		SerializedProperty s_drawEditorGizmos;
		SerializedProperty s_drawEditorGizmos_circleRadius;
		SerializedProperty s_enableSeparateExternalRadii;
		SerializedProperty s_externalCircleRadiusMultiplier;
		SerializedProperty s_drawEditorGizmos_nodes;
		SerializedProperty s_drawEditorGizmos_handles;
		SerializedProperty s_handle_lockRadius;
		SerializedProperty s_handle_lockRotation;
		SerializedProperty s_handle_snap;
		SerializedProperty s_handle_snap_intValues;
		SerializedProperty s_handle_snap_resolution;
		SerializedProperty s_prefab_link;
		SerializedProperty s_prefab_node;
		SerializedProperty s_circleRadius;
		SerializedProperty s_circleSlice;
		SerializedProperty s_rotationOffset;
		SerializedProperty s_relativeRotationMode;
		SerializedProperty s_scaleNodeWithRadius;
		SerializedProperty s_nodeScaleFactor;
		SerializedProperty s_nodeScale_min;
		SerializedProperty s_nodeScale_max;
		SerializedProperty s_nodesDistribution;
		SerializedProperty s_fanDistributionCommonSpan;
		SerializedProperty s_branchesCommonLength;
		SerializedProperty s_forceNodesSelection;
		SerializedProperty s_showInnerNode;
		SerializedProperty s_innerNodeScale;
		SerializedProperty s_showInnerLinks;
		SerializedProperty s_useQuerySystem;
		SerializedProperty s_keepPrefabLinking;
		SerializedProperty s_linksProgressMode;
		SerializedProperty s_linksProgressSpeed;
		SerializedProperty s_linksRebuildMode;

		// Private flags
		private bool showDebugInfo;


		private void OnEnable()
		{
			s_autoRebuildMode = serializedObject.FindProperty("autoRebuildMode");
			s_drawEditorGizmos = serializedObject.FindProperty("drawEditorGizmos");
			s_drawEditorGizmos_circleRadius = serializedObject.FindProperty("drawEditorGizmos_circleRadius");
			s_enableSeparateExternalRadii = serializedObject.FindProperty("enableSeparateExternalRadii");
			s_externalCircleRadiusMultiplier = serializedObject.FindProperty("externalCircleRadiusMultiplier");
			s_drawEditorGizmos_nodes = serializedObject.FindProperty("drawEditorGizmos_nodes");
			s_drawEditorGizmos_handles = serializedObject.FindProperty("drawEditorGizmos_handles");
			s_handle_lockRadius = serializedObject.FindProperty("handle_lockRadius");
			s_handle_lockRotation = serializedObject.FindProperty("handle_lockRotation");
			s_handle_snap = serializedObject.FindProperty("handle_snap");
			s_handle_snap_intValues = serializedObject.FindProperty("handle_snap_intValues");
			s_handle_snap_resolution = serializedObject.FindProperty("handle_snap_resolution");
			s_prefab_link = serializedObject.FindProperty("prefab_link");
			s_prefab_node = serializedObject.FindProperty("prefab_node");
			s_circleRadius = serializedObject.FindProperty("circleRadius");
			s_circleSlice = serializedObject.FindProperty("circleSlice");
			s_rotationOffset = serializedObject.FindProperty("rotationOffset");
			s_relativeRotationMode = serializedObject.FindProperty("relativeRotationMode");
			s_scaleNodeWithRadius = serializedObject.FindProperty("scaleNodeWithRadius");
			s_nodeScaleFactor = serializedObject.FindProperty("nodeScaleFactor");
			s_nodeScale_min = serializedObject.FindProperty("nodeScale_min");
			s_nodeScale_max = serializedObject.FindProperty("nodeScale_max");
			s_nodesDistribution = serializedObject.FindProperty("nodesDistribution");
			s_fanDistributionCommonSpan = serializedObject.FindProperty("fanDistributionCommonSpan");
			s_branchesCommonLength = serializedObject.FindProperty("branchesCommonLength");
			s_forceNodesSelection = serializedObject.FindProperty("forceNodesSelection");
			s_showInnerNode = serializedObject.FindProperty("showInnerNode");
			s_innerNodeScale = serializedObject.FindProperty("innerNodeScale");
			s_showInnerLinks = serializedObject.FindProperty("showInnerLinks");
			s_useQuerySystem = serializedObject.FindProperty("_useQuerySystem");
			s_keepPrefabLinking = serializedObject.FindProperty("keepPrefabLinking");
			s_linksProgressMode = serializedObject.FindProperty("linksProgressMode");
			s_linksProgressSpeed = serializedObject.FindProperty("linksProgressSpeed");
			s_linksRebuildMode = serializedObject.FindProperty("linksRebuildMode");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var t = target as RadialLayout;

			if (t == null)
				return;

			// Creating styles if null
			if (this.style_header == null)
			{
				this.style_header = new GUIStyle("box");
				this.style_header_editor = new GUIStyle("box");
				style_header.stretchWidth = true;
				style_header_editor.stretchWidth = true;
				style_header.normal.textColor = Color.white;
				style_header.normal.background = MakeTex(2, 2, headerColor);
				style_header.normal.textColor = Color.white;
				style_header.alignment = TextAnchor.MiddleLeft;
				style_header.fontStyle = FontStyle.Bold;
				style_header_editor.normal.background = MakeTex(2, 2,color4);
				style_header_editor.normal.textColor = Color.black;
				style_header_editor.alignment = TextAnchor.MiddleLeft;
				style_header_editor.fontStyle = FontStyle.Bold;
			}

			// Pre-checks
			bool isSubLayout = t.IsSubLayout;
			// Checking if inside a canvas
			if (t.GetComponentInParent<Canvas>() == null)
				EditorGUILayout.HelpBox("Layout should be contained in a Canvas object.", MessageType.Error);

			// Checking if scale is != 1 a canvas
			if (t.transform.localScale.x != 1 ||  t.transform.localScale.y != 1)
				EditorGUILayout.HelpBox("Layout scale is different than 1, rebuild may not work. Enable the Scale Node flag and tweak the Scaling Factor if you want to scale nodes size.", MessageType.Error);


			if (isSubLayout)
			{
				EditorGUILayout.HelpBox("This layout is a sub-layout", MessageType.Info);
				BeginSelectorButtonGroup();
				if (GUILayout.Button("Select parent node"))
				{
					Selection.activeGameObject = t.GetComponentInParent<RadialLayoutNode>().gameObject;
				}
				EndSelectorButtonGroup();
			}

			// Settings
			EditorGUILayout.Space(10);
			EditorGUILayout.BeginHorizontal(style_header);
			EditorGUILayout.LabelField("Layout Settings",style_header);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel++;

			EditorGUI.BeginDisabledGroup(isSubLayout);
			if (!isSubLayout)
			{
				s_autoRebuildMode.enumValueIndex = (int)(RadialLayout.AutoRebuildMode)EditorGUILayout.EnumPopup(new GUIContent("Auto Rebuild Layout", "Always: rebuilds layout both in editor and during play.\n\nEditor Only: rebuilds layout only in editor.\n\nNever: never rebuilds layout automatically."), (RadialLayout.AutoRebuildMode)s_autoRebuildMode.enumValueIndex);
			}
			else
			{
				EditorGUILayout.EnumPopup(new GUIContent("Auto Rebuild Layout", "Always: rebuilds layout both in editor and during play.\n\nEditor Only: rebuilds layout only in editor.\n\nNever: never rebuilds layout automatically.\n\n(Controlled by master layout)"), (RadialLayout.AutoRebuildMode)t.GetMasterLayout().autoRebuildMode);
			}
			EditorGUI.EndDisabledGroup();

			if (!t.enabled && (Application.isPlaying && s_autoRebuildMode.enumValueIndex == (int)RadialLayout.AutoRebuildMode.Always || !Application.isPlaying && s_autoRebuildMode.enumValueIndex != (int)RadialLayout.AutoRebuildMode.Never)  )
				EditorGUILayout.HelpBox("Monobehaviour is disabled, changes won't be applied!", MessageType.Warning);
			else if ((Application.isPlaying && s_autoRebuildMode.enumValueIndex == (int)RadialLayout.AutoRebuildMode.EditorOnly))
				EditorGUILayout.HelpBox("Rebuild mode is set to Editor Only, changes won't be applied unless Force Rebuild is called", MessageType.Warning);


			s_circleRadius.floatValue = Mathf.Max(0,EditorGUILayout.FloatField(new GUIContent("Circle Radius", "Radius of the layout."), s_circleRadius.floatValue));
			s_circleSlice.floatValue = Mathf.Max(0, EditorGUILayout.Slider(new GUIContent("Circle Slice", "Modify this if you want to use only a slice of the full layout circle."), s_circleSlice.floatValue,10,360));
			s_enableSeparateExternalRadii.boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Multiple Depth Radius", "Turn on if you want to specify a different radius multiplier for each depth."), s_enableSeparateExternalRadii.boolValue);
			if (s_enableSeparateExternalRadii.boolValue)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(s_externalCircleRadiusMultiplier, new GUIContent("Depth Radius Multipliers", "Set here the desired radius multiplier for each depth."), true);
				if (EditorGUI.EndChangeCheck())
				{
					// Clamping values
					serializedObject.ApplyModifiedProperties();
					for (int k = 0; k < t.externalCircleRadiusMultiplier.Length; k++)
						if (t.externalCircleRadiusMultiplier[k] < 0)
							t.externalCircleRadiusMultiplier[k] = 0;
				}
			}

			s_rotationOffset.floatValue = EditorGUILayout.FloatField(new GUIContent("Rotation offset", "Offset of the layout rotation in degrees."), s_rotationOffset.floatValue);
			if (isSubLayout)
			{
				// Relativer rotation mode
				s_relativeRotationMode.enumValueIndex = (int)(RadialLayout.RelativeRotation)EditorGUILayout.EnumPopup(new GUIContent("Relative Rotation", "RelativeToParent: keeps the rotation of this sub-layout relative to parent, KeepOwn: this sub-layout has its own independent rotation"), (RadialLayout.RelativeRotation)s_relativeRotationMode.enumValueIndex);

			}
			s_scaleNodeWithRadius.boolValue = EditorGUILayout.Toggle(new GUIContent("Scale Nodes" + (isSubLayout ? " (Override master)" : ""), "Scales nodes, relative to circle radius."), s_scaleNodeWithRadius.boolValue);
			if (s_scaleNodeWithRadius.boolValue)
			{
				s_nodeScaleFactor.floatValue = Mathf.Max(0,EditorGUILayout.FloatField(new GUIContent("Scaling Factor " , "Scaling factor relative to the radius (eg. a scale factor of 100 and a radius of 100 will result in a scale equal to 1)"), s_nodeScaleFactor.floatValue));

				float scaleFactor = t.circleRadius /s_nodeScaleFactor.floatValue;

				GUIStyle warningText = new GUIStyle(GUI.skin.textField);
				warningText.normal.textColor = Color.red;/* new Color32(0xa3,0x72,0x00,255);*/

				s_nodeScale_min.floatValue = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Min Node Scale", ""), s_nodeScale_min.floatValue, scaleFactor <= t.nodeScale_min ? warningText : GUI.skin.textField),0,s_nodeScale_max.floatValue);
				s_nodeScale_max.floatValue = Mathf.Max(s_nodeScale_min.floatValue,EditorGUILayout.FloatField(new GUIContent("Max Node Scale", ""), s_nodeScale_max.floatValue, scaleFactor >= t.nodeScale_max ? warningText : GUI.skin.textField));
			}
			s_nodesDistribution.enumValueIndex = (int)(RadialLayout.NodesDistribution)EditorGUILayout.EnumPopup(new GUIContent("Nodes Distribution", "Concentric: place subnodes on concentric circles starting from the layout root, Fan: each node controls its children placing them on a fan distribution."), (RadialLayout.NodesDistribution)s_nodesDistribution.enumValueIndex);

			if (s_nodesDistribution.enumValueIndex == (int)RadialLayout.NodesDistribution.Branches)
			{
				s_fanDistributionCommonSpan.floatValue = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Default Fan Span", "Default span in degrees. Can be overriden in Node Settings"), s_fanDistributionCommonSpan.floatValue),0,180);

				s_branchesCommonLength.floatValue = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Branches Length Multiplier", "Default branches lenght multiplier, relative to main circle radius. Can be overriden in Node Settings"), s_branchesCommonLength.floatValue), 0, 10);
			}

			s_showInnerNode.boolValue = EditorGUILayout.Toggle(new GUIContent("Show Inner Node", "Shows/Hides the central node (aesthetical only"), s_showInnerNode.boolValue);

			if (s_showInnerNode.boolValue)
				s_innerNodeScale.floatValue = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Inner Node Scale", "Sets the scale of the inner node."), s_innerNodeScale.floatValue), 0, 10);

			s_showInnerLinks.boolValue = EditorGUILayout.Toggle(new GUIContent("Show Inner Links", "Shows/Hides links from layout center to base nodes"), s_showInnerLinks.boolValue);

			EditorGUI.BeginDisabledGroup(isSubLayout);
			{
				if (!isSubLayout)
				{
					s_linksRebuildMode.enumValueIndex = (int)(RadialLayout.LinksRebuildMode)EditorGUILayout.EnumPopup(new GUIContent("Links Rebuild Mode", "Regenerate: links are destroyed and regenerated at each rebuild. Ensures consistency but may impact performance if rebuild is called ofted during play.\n\nKeep Existing: rebuild reuse existing links, useful if you call rebuild often or if you want to keep customization over link gameobjects.\n"), (RadialLayout.LinksRebuildMode)s_linksRebuildMode.enumValueIndex);
				}
				else
				{
					EditorGUILayout.EnumPopup(new GUIContent("Links Rebuild Mode", "Regenerate: links are destroyed and regenerated at each rebuild. Ensures consistency but may impact performance if rebuild is called ofted during play.\n\nKeep Existing: rebuild reuse existing links, useful if you call rebuild often or if you want to keep customization over link gameobjects.\n\n(Controlled by master layout)"), t.GetMasterLayout().linksRebuildMode);
				}
			}
			EditorGUI.EndDisabledGroup();


			s_linksProgressMode.enumValueIndex = (int)(RadialLayoutLink.ProgressMode)EditorGUILayout.EnumPopup(new GUIContent("Links Progress Mode", "Immediate: links will update to target fill value immediately, Animated: the progress will gradually fill/deplete over time to reach target value"), (RadialLayoutLink.ProgressMode)s_linksProgressMode.enumValueIndex);

			if(s_linksProgressMode.enumValueIndex == 1)
				s_linksProgressSpeed.intValue = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Link Progress Speed", ""), s_linksProgressSpeed.intValue), 0,200);

			// Query System
			EditorGUI.BeginDisabledGroup(isSubLayout);
			if (!isSubLayout)
			{
				var useQuerySystem = EditorGUILayout.Toggle(new GUIContent("Use Query System", "Enables the use of the Query System for this layout"), s_useQuerySystem.boolValue);
				if (useQuerySystem != t.UseQuerySystem)
				{
					// In editor mode, asking for confirmation
					if (!Application.isPlaying)
					{
						string text = useQuerySystem ? "This will add a RadialLayoutQueryTarget component to each node of this layout and assign an unique ID to them. Continue?" : "This will remove all RadialLayoutQueryTarget components from nodes of this layout. All previously assigned unique IDs will be lost. Continue?";
						if (EditorUtility.DisplayDialog("Modify Query System", text, "Yes", "No"))
						{
							t.UseQuerySystem = useQuerySystem;
							s_useQuerySystem.boolValue = useQuerySystem;
						}
					}
					else
					{
						t.UseQuerySystem = s_useQuerySystem.boolValue;
					}
				}
			}
			else
			{
				EditorGUILayout.Toggle(new GUIContent("Use Query System", "Enables the use of the Query System for this layout. (Controlled by master layout)"), t.UseQuerySystem);
			}
			EditorGUI.EndDisabledGroup();


			EditorGUI.indentLevel--;

			EditorGUILayout.Space(10);

			// Prefabs
			EditorGUILayout.BeginHorizontal(style_header);
			EditorGUILayout.LabelField("Elements Prefabs",style_header);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel++;
			s_prefab_node.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Node Prefab", "Prefab to be used when creating new nodes. If not specified, an empty node GameObject will be created."), s_prefab_node.objectReferenceValue, typeof(RadialLayoutNode), false);
			s_keepPrefabLinking.boolValue = EditorGUILayout.Toggle(new GUIContent("Keep prefab linking", "If TRUE, new nodes will be instantiated as prefab, mantaining link to the original. If FALSE, new nodes will be instantiated as new GameObjects"), s_keepPrefabLinking.boolValue);
			s_prefab_link.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Link Prefab", ""), s_prefab_link.objectReferenceValue, typeof(RadialLayoutLink), false);

			EditorGUI.indentLevel--;

			// Actions
			EditorGUILayout.Space(10);
			EditorGUILayout.BeginHorizontal(style_header);
			EditorGUILayout.LabelField("Actions", style_header);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel++;

			if (t.gameObject.scene.name != null)
			{

				if (GUILayout.Button("Add Node"))
				{
					t.AddNode();
					EditorUtility.SetDirty(t);
				}

				if (GUILayout.Button("Delete Last Node"))
				{
					RadialLayoutNode lastNode = t.GetNodeByChildIndex(t.CountNodesOfDepth(0) - 1);
					if (lastNode != null)
					{
						if (lastNode.HasChildren)
						{
							if (EditorUtility.DisplayDialog("Warning", "This node has child nodes, every child will be destroyed and this action cannot be undone.Do you want to continue?", "Yes", "No"))
							{
								t.DeleteNode(lastNode, true);
								EditorUtility.SetDirty(t);
							}
						}
						else if (lastNode.IsSubLayout)
						{
							if (EditorUtility.DisplayDialog("Warning", "This node is a sub-layout, every child nodes will be destroyed and this action cannot be undone. Do you want to continue?", "Yes", "No"))
							{
								t.DeleteNode(lastNode, true);
								EditorUtility.SetDirty(t);
							}
						}
						else
						{
							t.DeleteNode(lastNode, false);
							EditorUtility.SetDirty(t);
						}
					}
				}

				if (GUILayout.Button("Clear All Offsets"))
				{
					if (EditorUtility.DisplayDialog("Clear Offsets", "Are you sure you want to clear all position offsets and restore all nodes to the original layout position? This action cannot be undone.", "Yes", "No"))
					{
						t.rotationOffset = 0;
						foreach (var node in t.Nodes)
							node.ClearOffsets();
						EditorUtility.SetDirty(t);
					}
				}

				RadialLayoutEditor.BeginSelectorButtonGroup();
				EditorGUI.BeginDisabledGroup(t.Nodes == null || t.Nodes.Count == 0);
				if (GUILayout.Button("Select Child Node"))
				{
					Selection.activeGameObject = t.Nodes[0].gameObject;
				}
				EditorGUI.EndDisabledGroup();
				RadialLayoutEditor.EndSelectorButtonGroup();

				exColor = GUI.backgroundColor;
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Force Rebuild"))
				{
					t.Rebuild();
				}
				GUI.backgroundColor = exColor;

			}
			else
			{
				EditorGUILayout.HelpBox("Prefab is not in scene", MessageType.Info);
			}
			EditorGUI.indentLevel--;

			// Editor Settings
			EditorGUILayout.Space(10);
			EditorGUILayout.BeginHorizontal(style_header_editor);
			EditorGUILayout.LabelField("Editor Settings", style_header_editor);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel++;
			if(!isSubLayout)
				s_forceNodesSelection.boolValue = EditorGUILayout.Toggle(new GUIContent("Force Nodes Selection", "Forces the selection of node objects when working in Scene View."), s_forceNodesSelection.boolValue);
			s_drawEditorGizmos.boolValue = EditorGUILayout.Toggle(new GUIContent("Draw Gizmos", "Enable/Disable debug gizmos in the editor."), s_drawEditorGizmos.boolValue);
			if (s_drawEditorGizmos.boolValue)
			{
				s_drawEditorGizmos_circleRadius.boolValue = EditorGUILayout.Toggle(new GUIContent("Circle Radius", "Enable/Disable layout radius gizmos."), s_drawEditorGizmos_circleRadius.boolValue);
				s_drawEditorGizmos_nodes.boolValue = EditorGUILayout.Toggle(new GUIContent("Nodes", "Enable/Disable nodes gizmos."), s_drawEditorGizmos_nodes.boolValue);
				s_drawEditorGizmos_handles.boolValue = EditorGUILayout.Toggle(new GUIContent("Resizing Handles", "Enable/Disable resizing handles."), s_drawEditorGizmos_handles.boolValue);
				if (s_drawEditorGizmos_handles.boolValue)
				{
					EditorGUILayout.LabelField("Handle Locks", EditorStyles.boldLabel);
					EditorGUILayout.BeginHorizontal();
					s_handle_lockRadius.boolValue = EditorGUILayout.Toggle(new GUIContent("Radius", "Locks radius when moving handles."), s_handle_lockRadius.boolValue);
					s_handle_lockRotation.boolValue = EditorGUILayout.Toggle(new GUIContent("Rotation", "Locks rotation when moving handles."), s_handle_lockRotation.boolValue);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.LabelField("Snap Settings", EditorStyles.boldLabel);
					EditorGUI.indentLevel++;
					s_handle_snap.boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Snap", "Enables handles snap(Hold CTRL to activate)."), s_handle_snap.boolValue);
					EditorGUI.BeginDisabledGroup(!s_handle_snap.boolValue);
					s_handle_snap_resolution.floatValue = EditorGUILayout.FloatField(new GUIContent("Resolution", "Snap resolution."), s_handle_snap_resolution.floatValue);
					if (s_handle_snap_resolution.floatValue < 0)
						s_handle_snap_resolution.floatValue = 0;
					s_handle_snap_intValues.boolValue = EditorGUILayout.Toggle(new GUIContent("Integer values", "Rounds snapping to integer values only."), s_handle_snap_intValues.boolValue);
					EditorGUI.EndDisabledGroup();
					EditorGUI.indentLevel--;
				}

				var exFontStyle = EditorStyles.label.fontStyle;
				if(this.showDebugInfo)
					EditorStyles.label.fontStyle = FontStyle.Bold;
				this.showDebugInfo = EditorGUILayout.Toggle(new GUIContent("Show Debug Info", ""), this.showDebugInfo);
				EditorStyles.label.fontStyle = exFontStyle;

				if (this.showDebugInfo)
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.LabelField("Nodes Count: " + t.NodeCount);
					EditorGUILayout.LabelField("Sub-Layouts Count: " + t.SubLayoutsCount);
					EditorGUI.indentLevel--;
				}
			}
			EditorGUI.indentLevel--;

			serializedObject.ApplyModifiedProperties();
			

			// Version label
			EditorGUILayout.Space(10);
			GUIStyle rightcentederLabel = new GUIStyle(GUI.skin.label);
			rightcentederLabel.fontStyle = FontStyle.Italic;
			rightcentederLabel.fontSize = Mathf.RoundToInt(GUI.skin.label.fontSize * 0.75f);
			rightcentederLabel.alignment = TextAnchor.UpperRight;
			rightcentederLabel.normal.textColor = new Color(0.25f, 0.25f, 0.25f, 1f);
			if (GUILayout.Button("GIGA Softworks - Auto Radial Layout ver. " + RadialLayout.VERSION, rightcentederLabel))
				RadialLayoutAboutDialog.Init();
		}

		public void OnSceneGUI()
		{
			// Gizmos
			var t = target as RadialLayout;
			if (t.drawEditorGizmos)
			{
				Color exColor = Handles.color;
				// Main circle
				if (t.drawEditorGizmos_circleRadius)
				{
					Handles.color = new Color(1, 1, 1, 0.5f);
					for (int k = 0; k < t.MaxDepth + 1; k++)
					{
						// if branches distribution, drawing only main circle
						if (t.nodesDistribution == RadialLayout.NodesDistribution.Branches && k > 0)
							break;
						float radius = t.circleRadius * t.GetNodesRadiusMultiplier(k) * (k + 1) * t.Canvas.scaleFactor;
						Handles.DrawWireDisc(t.transform.position, Vector3.forward, radius);
					}
				}

				// Circle slice
				if (t.circleSlice != 360)
				{
					Handles.color = new Color(1, 1, 0, 0.5f);
					Vector2 upVector = Vector2.up.Rotate(t.rotationOffset * Mathf.Deg2Rad);
					Handles.DrawWireArc(t.transform.position, Vector3.back, upVector, t.circleSlice, t.circleRadius * t.Canvas.scaleFactor);
				}

				// Nodes
				if (t.drawEditorGizmos_nodes && t.Nodes != null)
				{
					Handles.color = new Color(0, 0, 0.6f, 0.5f);
					foreach (var node in t.Nodes)
					{
						if(node != null)
							Handles.DrawSolidDisc(node.transform.position, Vector3.forward, Mathf.Clamp(t.circleRadius / t.nodeScaleFactor, t.nodeScale_min, t.nodeScale_max) * t.Canvas.pixelRect.height / 100f);
					}
				}

				// Resizing handles
				if (t.drawEditorGizmos_handles)
				{
					this.DrawResizingHandles(t);
				}

				Handles.color = exColor;
			}

		}

				 
		private void DrawResizingHandles(RadialLayout layout)
		{
			float size = Mathf.Clamp(layout.circleRadius * 0.1f, MIN_HANDLE_SIZE, MAX_HANDLE_SIZE);

			Vector3 snap = layout.handle_snap ? Vector3.one * layout.handle_snap_resolution : Vector3.zero;
			Color exColor = Handles.color;

			Vector3 prevHandlePos = layout.transform.position + layout.circleRadius * layout.Canvas.scaleFactor * (Vector3.left * Mathf.Sin((layout.transform.eulerAngles.z + layout.rotationOffset) * Mathf.Deg2Rad) + Vector3.up * Mathf.Cos(-(layout.transform.eulerAngles.z + layout.rotationOffset) * Mathf.Deg2Rad));

			// Drawing handles
			Handles.color = Color.green;
#if UNITY_2022_3_OR_NEWER
			Vector3 newHandlePos = Handles.FreeMoveHandle(prevHandlePos, size, snap, Handles.CircleHandleCap);
#else
			Vector3 newHandlePos = Handles.FreeMoveHandle(prevHandlePos, Quaternion.identity, size, snap, Handles.CircleHandleCap);
#endif

			if (newHandlePos != prevHandlePos)
			{
				Undo.RecordObject(layout, "Radial Layout Resizing");

				// Snapping to grid if enabled
				if (snap != Vector3.zero)
					newHandlePos = Handles.SnapValue(newHandlePos, snap);

				Vector3 radiusVec = newHandlePos - layout.transform.position;
				float radius = radiusVec.Magnitude2D();
				float angle = -Vector3.Angle(Vector3.up, radiusVec);
				if (snap != Vector3.zero && layout.handle_snap_intValues)
				{
					if (Event.current.control)
					{
						//Ctrl key pressed.
						radius = Mathf.RoundToInt(radius);
						angle = Mathf.RoundToInt(Mathf.RoundToInt(angle / layout.handle_snap_resolution) * layout.handle_snap_resolution);
					}
				}

				if (newHandlePos.x < layout.transform.position.x)
					angle = -angle;
				if (!layout.handle_lockRadius)
					layout.circleRadius = radius / layout.Canvas.scaleFactor;
				if (!layout.handle_lockRotation)
					layout.rotationOffset = angle;

				EditorUtility.SetDirty(layout);
			}

			Handles.color = exColor;
		}

		public static Texture2D MakeTex(int width, int height, Color col)
		{
			Color[] pix = new Color[width * height];
			for (int i = 0; i < pix.Length; ++i)
			{
				pix[i] = col;
			}
			Texture2D result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();
			return result;
		}

		public static void BeginSelectorButtonGroup()
		{
			exColor = GUI.backgroundColor;
			GUI.backgroundColor = selectorButtonColor;
		}

		public static void EndSelectorButtonGroup()
		{
			GUI.backgroundColor = exColor;
		}

	}
}

#endif

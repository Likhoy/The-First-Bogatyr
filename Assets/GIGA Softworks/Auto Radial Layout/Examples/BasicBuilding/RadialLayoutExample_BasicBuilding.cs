using GIGA.AutoRadialLayout;
using GIGA.AutoRadialLayout.QuerySystem;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GIGA.AutoRadialLayout.Examples
{
	[ExecuteInEditMode]
	public class RadialLayoutExample_BasicBuilding : MonoBehaviour
	{
		public Canvas canvas;
		public RadialLayout layout;
		public Text text_title, text_left, text_right, errorText;

		private int phase = 0;
		private int last_phase = -1;
		private TutorialPhase currentPhase;

		private void OnEnable()
		{
			phase = 0;
			last_phase = -1;
			this.text_title.text = "Basic building tutorial";
			this.errorText.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (!Application.isPlaying)
			{
				// if no layout found, resetting to phase 0
				if (this.canvas.GetComponentInChildren<RadialLayout>() == null)
				{
					this.phase = 0;
					this.layout = null;
				}

				if (phase != last_phase)
				{
					// phase switch
					switch (phase)
					{
						case 0:
							// Layout placement
							this.currentPhase = new Phase_PlaceLayout(this);
							break;
						case 1:
							// Add nodes 1
							this.currentPhase = new Phase_AddNodes1(this);
							break;
						case 2:
							// Add nodes 2
							this.currentPhase = new Phase_AddNodes2(this);
							break;
						case 3:
							// Child nodes
							this.currentPhase = new Phase_ChildNodes(this);
							break;
						case 4:
							// Node rearrangement
							this.currentPhase = new Phase_NodeRearrangement(this);
							break;
						case 5:
							// Node rearrangement
							this.currentPhase = new Phase_SubLayouts(this);
							break;
						case 6:
							// Inner node
							this.currentPhase = new Phase_InnerLinks(this);
							break;
						default:
							// Complete
							this.currentPhase = new Phase_TutorialComplete(this);
							break;
					}

					this.text_title.text = currentPhase.Title;
					this.text_left.text = this.currentPhase.GetChecksText();
					this.text_right.text = this.currentPhase.GetHintText();
					this.last_phase = phase;
					this.OnGUI();
				}

				if (this.currentPhase != null)
				{
					if (this.currentPhase.CheckAdvance())
						this.phase++;

					if (this.currentPhase.dirtyChecks)
					{
						this.text_left.text = this.currentPhase.GetChecksText();
						this.OnGUI();
					}

					if (this.currentPhase.dirtyHint)
						this.text_right.text = this.currentPhase.GetHintText();

				}

			}
			else
			{
				this.errorText.gameObject.SetActive(true);
			}

		}

		private void OnGUI()
		{

		}


		#region Phases

		private abstract class TutorialPhase
		{
			public virtual string Title => "";

			protected StringBuilder sb_checks = new StringBuilder();
			protected StringBuilder sb_hints = new StringBuilder();
			protected List<(bool, string)> checks;
			public bool dirtyChecks = true;
			public bool dirtyHint = true;
			protected RadialLayoutExample_BasicBuilding exampleScript;

			public TutorialPhase(RadialLayoutExample_BasicBuilding exampleScript)
			{
				this.exampleScript = exampleScript;
			}

			public string GetChecksText()
			{
				if (dirtyChecks)
				{
					sb_checks.Clear();
					dirtyChecks = false;
					foreach (var check in this.checks)
					{
						sb_checks.AppendLine($"[{(check.Item1 ? "X" : "  ")}] {check.Item2}");
					}
				}
				return sb_checks.ToString();
			}

			public string GetHintText()
			{
				return sb_hints.ToString();
			}

			public abstract bool CheckAdvance();

			protected void SetCheck(int index)
			{
				this.checks[index] = (true, this.checks[index].Item2);
				this.dirtyChecks = true;
			}
		}

		private class Phase_PlaceLayout : TutorialPhase
		{
			public override string Title => "Layout placement";

			public Phase_PlaceLayout(RadialLayoutExample_BasicBuilding exampleScript) : base(exampleScript)
			{
				this.checks = new List<(bool, string)>
					{
					 (false,"Place a <b>prefab_RadialLayout</b> object inside the canvas." )
					};
				this.sb_hints.Append("Prefab can be found in the Prefabs folder, or can be added via menu: GameObject | UI | Radial Layout");
			}

			public override bool CheckAdvance()
			{
				// Checking layout presence
				if (this.exampleScript.canvas.GetComponentInChildren<RadialLayout>() != null)
				{
					this.exampleScript.layout = this.exampleScript.canvas.GetComponentInChildren<RadialLayout>();
					return true;
				}
				return false;
			}
		}

		private class Phase_AddNodes1 : TutorialPhase
		{
			public override string Title => "Adding Nodes (1)";

			private float startingRadius;

			public Phase_AddNodes1(RadialLayoutExample_BasicBuilding exampleScript) : base(exampleScript)
			{
#if UNITY_EDITOR
				exampleScript.layout.drawEditorGizmos = true;
#endif
				this.startingRadius = exampleScript.layout.circleRadius;
				this.checks = new List<(bool, string)>
					{
						(false,"Use the green gizmo to change radius and rotation."),
						(false,"Add 3 Nodes")
					};
				this.sb_hints.Append("Layout gizmos can be disabled in the layout Inspector under Editor Settings");
			}

			public override bool CheckAdvance()
			{
				// Checking radius change
				if (this.checks[0].Item1 == false && this.exampleScript.layout.circleRadius != this.startingRadius)
				{
					this.SetCheck(0);
					this.sb_hints.Clear();
					this.sb_hints.Append("Nodes can be added using the <b>Add Node</b> button in the layout inspector");
					this.dirtyHint = true;
				}

				// Checking 3 nodes
				if (this.checks[1].Item1 == false && this.exampleScript.layout.NodeCount >= 3)
				{
					this.SetCheck(1);
				}

				// Adding delete all
				if (this.checks.Count == 2 && this.checks[0].Item1 && this.checks[1].Item1)
				{
					this.checks.Add((false, "Delete all nodes"));
					this.dirtyChecks = true;
					this.sb_hints.Clear();
					this.sb_hints.Append("Nodes can be deleted using the <b>Delete Last Node</b> button in the layout inspector");
					this.dirtyHint = true;
				}
				else if (this.checks.Count == 3 && !this.checks[2].Item1 && this.exampleScript.layout.NodeCount == 0)
				{
					this.SetCheck(2);
				}
				return this.checks.Count == 3 && this.checks[0].Item1 && this.checks[1].Item1 && this.checks[2].Item1;
			}
		}

		private class Phase_AddNodes2 : TutorialPhase
		{
			public override string Title => "Adding Nodes (2)";

			public Phase_AddNodes2(RadialLayoutExample_BasicBuilding exampleScript) : base(exampleScript)
			{
				this.checks = new List<(bool, string)>
					{
						(false,"Set <b>Node Prefab</b> in the Layout inspector"),
					};
				this.sb_hints.Append("In the previous example you added empty nodes.\nThis example shows how you can add nodes from prefabs.\n(Prefab can be found in the Prefabs\\Nodes folder)");
			}

			public override bool CheckAdvance()
			{
				// Checking prefab set
				if (this.checks[0].Item1 == false && this.exampleScript.layout.prefab_node != null)
				{
					this.SetCheck(0);
				}
				else if (this.checks.Count == 1 && this.checks[0].Item1)
				{
					// Adding Add 4 nodes check
					this.checks.Add((false, "Add 4 nodes"));
					this.dirtyChecks = true;
					this.sb_hints.Clear();
					this.sb_hints.Append("Nodes can be added using the <b>Add Node</b> button in the layout inspector");
					this.dirtyHint = true;
				}

				// Checking 4 nodes
				if (this.checks.Count == 2 && this.checks[1].Item1 == false && this.exampleScript.layout.NodeCount >= 4)
				{
					this.SetCheck(1);
				}


				return this.checks.Count == 2 && this.checks[0].Item1 && this.checks[1].Item1;
			}
		}

		private class Phase_ChildNodes : TutorialPhase
		{
			public override string Title => "Child Nodes";

			public Phase_ChildNodes(RadialLayoutExample_BasicBuilding exampleScript) : base(exampleScript)
			{
				this.checks = new List<(bool, string)>
					{
						(false,"Add 3 child nodes to every node"),
					};
				this.sb_hints.Append("Select a Node and use the <b>Add Child Node</b> button to add a child node to it.");
			}

			public override bool CheckAdvance()
			{
				// Checking child nodes
				if (this.checks[0].Item1 == false)
				{
					bool complete = true;
					foreach (var node in this.exampleScript.layout.Nodes)
					{
						if (node.depth == 0 && node.GetChildNodes().Length < 3)
						{
							complete = false;
							break;
						}
					}

					if (complete)
					{
						this.SetCheck(0);
					}
				}

				return this.checks[0].Item1;
			}
		}

		private class Phase_NodeRearrangement : TutorialPhase
		{
			public override string Title => "Nodes Rearrangement";

			Dictionary<RadialLayoutNode, float> startingFanSpan;
			Dictionary<RadialLayoutNode, float> startingDistanceOffset;
			Dictionary<RadialLayoutNode, float> startingAngleOffset;

			public Phase_NodeRearrangement(RadialLayoutExample_BasicBuilding exampleScript) : base(exampleScript)
			{
				if (exampleScript.layout == null)
					exampleScript.layout = exampleScript.canvas.GetComponentInChildren<RadialLayout>();

				this.checks = new List<(bool, string)>
					{
						(false,"Override the <b>fan span modifier</b> of a parent node"),
						(false,"Change the <b>distance offset</b> of a child node"),
						(false,"Change the <b>angle offset</b> of a child node"),
					};
				this.sb_hints.Append("Default layout can be rearranged using specific fields in the <b>Node inspector</b>");

				startingFanSpan = new Dictionary<RadialLayoutNode, float>();
				startingDistanceOffset = new Dictionary<RadialLayoutNode, float>();
				startingAngleOffset = new Dictionary<RadialLayoutNode, float>();

				foreach (var node in this.exampleScript.layout.Nodes)
				{
					if (node.depth == 0)
					{
						startingFanSpan.Add(node, node.FanSpan);

						foreach (var childNode in node.GetChildNodes())
						{
							startingDistanceOffset.Add(childNode, childNode.distanceOffset);
							startingAngleOffset.Add(childNode, childNode.angleOffset);
						}
					}
				}
			}

			public override bool CheckAdvance()
			{
				// Checking fan span change
				if (!this.checks[0].Item1)
				{
					foreach (var node in this.exampleScript.layout.Nodes)
					{
						if (node.depth == 0 && node.FanSpan != startingFanSpan[node])
							this.SetCheck(0);
					}
				}

				// Checking distance offset change
				if (!this.checks[1].Item1)
				{
					foreach (var node in this.startingDistanceOffset)
					{
						if (node.Key.distanceOffset != node.Value)
							this.SetCheck(1);
					}
				}

				// Checking angle offset change
				if (!this.checks[2].Item1)
				{
					foreach (var node in this.startingAngleOffset)
					{
						if (node.Key.angleOffset != node.Value)
							this.SetCheck(2);
					}
				}

				return this.checks[0].Item1 && this.checks[1].Item1 && this.checks[2].Item1;
			}
		}

		private class Phase_SubLayouts : TutorialPhase
		{
			public override string Title => "Sub-Layouts";

			private RadialLayoutNode targetNode = null;

			public Phase_SubLayouts(RadialLayoutExample_BasicBuilding exampleScript) : base(exampleScript)
			{
				if (exampleScript.layout == null)
					exampleScript.layout = exampleScript.canvas.GetComponentInChildren<RadialLayout>();

				this.checks = new List<(bool, string)>
					{
						(false,"Add 3 children nodes to a node of depth 1"),
					};

				this.sb_hints.Append("Select a Node and use the <b>Add Child Node</b> button to add a child node to it.");
			}

			public override bool CheckAdvance()
			{
				// Checking 3 children
				if (!this.checks[0].Item1)
				{
					foreach (var node in this.exampleScript.layout.Nodes)
					{
						if (node.depth == 1 && node.GetChildNodes().Length >= 3)
						{
							this.SetCheck(0);
							this.targetNode = node;
							// Adding convert check
							this.checks.Add((false, "Convert Node to Sub-Layout"));
							this.dirtyChecks = true;
							this.sb_hints.Clear();
							this.sb_hints.Append("Nodes can be converted to sub-layouts using the <b>Convert to sub-layout</b> button in <b>Node Inspector</b>");
							this.dirtyHint = true;
							break;
						}
					}
				}

				// checking conversion
				if (this.checks.Count == 2 && !this.checks[1].Item1 && this.targetNode != null && this.targetNode.IsSubLayout)
				{
					this.SetCheck(1);
					// Adding convert back check
					this.checks.Add((false, "Reconvert Sub-Layout to Node"));
					this.dirtyChecks = true;
					this.sb_hints.Clear();
					this.sb_hints.Append("This Node is a <b>Sub-Layout</b> now and can be worked on same way as the main layout. You can reconvert it to a normal node use the <b>Convert to Node</b> button in the <b>Node Inspector</b>");
					this.dirtyHint = true;
				}

				// checking conversion
				if (this.checks.Count == 3 && !this.checks[2].Item1 && this.targetNode != null && !this.targetNode.IsSubLayout)
				{
					this.SetCheck(2);
				}

				return this.checks.Count == 3 && this.checks[0].Item1 && this.checks[1].Item1 && this.checks[2].Item1;
			}
		}

		private class Phase_InnerLinks : TutorialPhase
		{
			public override string Title => "Inner Links";

			public Phase_InnerLinks(RadialLayoutExample_BasicBuilding exampleScript) : base(exampleScript)
			{
				if (exampleScript.layout == null)
					exampleScript.layout = exampleScript.canvas.GetComponentInChildren<RadialLayout>();

				this.checks = new List<(bool, string)>
					{
						(false,"Enable the Inner Node of the layout"),
						(false,"Enable Inner Links of the layout"),
					};

				this.sb_hints.Append("If you want you can enable inner node and links in the <b>Layout Inspector</b>.\nThe Inner Node is not a real node, it's just decorative.");
			}

			public override bool CheckAdvance()
			{
				// Checking inner node
				if (!this.checks[0].Item1 && this.exampleScript.layout.showInnerNode)
				{
					this.SetCheck(0);
				}

				// Checking inner links
				if (!this.checks[1].Item1 && this.exampleScript.layout.showInnerLinks)
				{
					this.SetCheck(1);
				}

				return this.checks.Count == 2 && this.checks[0].Item1 && this.checks[1].Item1;
			}
		}


		private class Phase_TutorialComplete : TutorialPhase
		{
			public override string Title => "Tutorial completed";

			public Phase_TutorialComplete(RadialLayoutExample_BasicBuilding exampleScript) : base(exampleScript)
			{
				this.checks = new List<(bool, string)>();
			}

			public override bool CheckAdvance()
			{
				return false;
			}
		}

		#endregion



	}
}

using GIGA.AutoRadialLayout;
using GIGA.AutoRadialLayout.QuerySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIGA.AutoRadialLayout.Examples
{
    public class RadialLayoutExample_Links : MonoBehaviour
    {
        public RadialLayout layout;

        private enum QueryButtonsIds
        {
            ProgressMode = 1,
            Symbol = 2,
            Symbol2 = 3,
        }

        private Dictionary<int, string[]> queryButtons = new Dictionary<int, string[]>();
        private Dictionary<int, int> queryButtonsState = new Dictionary<int, int>();
        private float targetProgress = 1;
        private int selectedId1 = 1, selectedId2 = 1;
		private bool chainedAnimationRunning;

        private void OnGUI()
        {
            GUIStyle areaStyle = new GUIStyle("box");
            areaStyle.normal.background = RadialLayoutExamplesHelper.MakeTex(2, 2, new Color32(0x00,0x4f,0xe1,255));
            GUIStyle boldLabel = new GUIStyle("label");
            boldLabel.fontStyle = FontStyle.Bold;

            RadialLayoutQueryResult result = null;

            GUILayout.BeginArea(new Rect(0, 0, Screen.width / 4f + 50, Screen.height),areaStyle);

            GUILayout.BeginVertical(GUILayout.Width(Screen.width / 4f));
            
            GUILayout.Label("Links search and progress examples",boldLabel);

			bool forceImmediate = false;
			float prevProgress = this.targetProgress;

			if (!chainedAnimationRunning)
			{

				this.CreateQueryButton((int)QueryButtonsIds.ProgressMode, "Progress Mode:", 0, "Animated", "Instant");
				string progressMode = this.queryButtons[(int)QueryButtonsIds.ProgressMode][this.queryButtonsState[(int)QueryButtonsIds.ProgressMode]];
				this.layout.linksProgressMode = progressMode == "Instant" ? RadialLayoutLink.ProgressMode.Instant : RadialLayoutLink.ProgressMode.Animated;
				foreach (var sublayout in this.layout.GetSubLayouts(true))
					sublayout.linksProgressMode = progressMode == "Instant" ? RadialLayoutLink.ProgressMode.Instant : RadialLayoutLink.ProgressMode.Animated;

				GUILayout.Label("Target Progress: " + (int)(this.targetProgress * 100) + "%");
				this.targetProgress = GUILayout.HorizontalSlider(this.targetProgress, 0, 1);

				GUILayout.Space(20);
				GUILayout.Label("Select all", boldLabel);
				// Clear all links
				if (GUILayout.Button("Clear All"))
				{
					result = RadialLayoutQueryManager.Begin(this.layout).GetAllLinks();
					targetProgress = 0;
					forceImmediate = true;
				}

				// Highlight all links
				if (GUILayout.Button("Set All"))
				{
					result = RadialLayoutQueryManager.Begin(this.layout).GetAllLinks();
				}

				GUILayout.Space(20);
				GUILayout.Label("Starting from", boldLabel);
				this.CreateQueryButton((int)QueryButtonsIds.Symbol, "From:", 0, "hearts", "spades", "clubs", "diamonds");
				if (GUILayout.Button("Run"))
				{
					string searchSymbol = this.queryButtons[(int)QueryButtonsIds.Symbol][this.queryButtonsState[(int)QueryButtonsIds.Symbol]];
					var symbolResult = RadialLayoutQueryManager.Begin(this.layout).GetNodesWithTags(searchSymbol);
					result = RadialLayoutQueryManager.Begin(this.layout).GetLinksStartingFrom(symbolResult.Nodes);
				}

				GUILayout.Space(20);
				GUILayout.Label("Going to", boldLabel);
				this.CreateQueryButton((int)QueryButtonsIds.Symbol2, "To:", 0, "hearts", "spades", "clubs", "diamonds");
				if (GUILayout.Button("Run"))
				{
					string searchSymbol = this.queryButtons[(int)QueryButtonsIds.Symbol2][this.queryButtonsState[(int)QueryButtonsIds.Symbol2]];
					var symbolResult = RadialLayoutQueryManager.Begin(this.layout).GetNodesWithTags(searchSymbol);
					result = RadialLayoutQueryManager.Begin(this.layout).GetLinksGoingTo(symbolResult.Nodes);
				}


				GUILayout.Space(20);
				GUILayout.Label("Connecting 2 nodes", boldLabel);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Node 1:");
				this.selectedId1 = (int)GUILayout.HorizontalSlider(this.selectedId1, 1, 31);
				GUILayout.Label(this.selectedId1.ToString());
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Node 2:");
				this.selectedId2 = (int)GUILayout.HorizontalSlider(this.selectedId2, 1, 31);
				GUILayout.Label(this.selectedId2.ToString());

				GUILayout.EndHorizontal();

				if (GUILayout.Button("Run"))
				{
					var queryBase = RadialLayoutQueryManager.Begin(this.layout);
					RadialLayoutNode node1 = queryBase.GetNodeWithUniqueId(this.selectedId1);
					RadialLayoutNode node2 = queryBase.GetNodeWithUniqueId(this.selectedId2);
					RadialLayoutLink link = queryBase.GetLinkConnecting(node1, node2);
					if (link != null)
						link.ProgressValue = this.targetProgress;
				}

				GUILayout.Space(20);
				GUILayout.Label("Events & Chaining", boldLabel);
				if (GUILayout.Button("Run Example"))
				{
					if (!this.chainedAnimationRunning)
						this.StartCoroutine(this.ChainedAnimationExample());

				}
			}
			else
			{
				GUILayout.Label("Chained animation example running...");
			}

			GUILayout.EndVertical();

            GUILayout.EndArea();

			if (result != null)
			{
				var prevProgressMode = this.layout.linksProgressMode;
				if (forceImmediate)
				{
					this.layout.linksProgressMode = RadialLayoutLink.ProgressMode.Instant;
					foreach (var sublayout in this.layout.GetSubLayouts(true))
						sublayout.linksProgressMode = RadialLayoutLink.ProgressMode.Instant;
				}

				foreach (var link in result.Links)
				{
					link.ProgressValue = targetProgress;
				}

				this.layout.linksProgressMode = prevProgressMode;
				foreach (var sublayout in this.layout.GetSubLayouts(true))
					sublayout.linksProgressMode = prevProgressMode;

				if (forceImmediate)
					this.targetProgress = prevProgress;
			}


		}

        private void CreateQueryButton(int id, string label, int minWidth, params string[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            if (!this.queryButtons.ContainsKey(id))
            {
                this.queryButtonsState.Add(id, 0);
                this.queryButtons.Add(id, options);
            }
            if (GUILayout.Button(options[this.queryButtonsState[id]], GUILayout.MinWidth(minWidth)))
            {
                this.queryButtonsState[id] = (this.queryButtonsState[id] + 1) % this.queryButtons[id].Length;
            }
            GUILayout.EndHorizontal();
        }

		private IEnumerator ChainedAnimationExample()
		{
			this.chainedAnimationRunning = true;

			RadialLayoutLink.ProgressMode prevProgressMode = this.layout.linksProgressMode;
			var queryBase = RadialLayoutQueryManager.Begin(this.layout);

			// Clearing all
			var result = queryBase.GetAllLinks();
			this.layout.linksProgressMode = RadialLayoutLink.ProgressMode.Instant;
			foreach (var sublayout in this.layout.GetSubLayouts(true))
				sublayout.linksProgressMode = RadialLayoutLink.ProgressMode.Instant;
			foreach (var l in result.Links)
				l.ProgressValue = 0;

			// Setting mode to animated
			this.layout.linksProgressMode = RadialLayoutLink.ProgressMode.Animated;
			foreach (var sublayout in this.layout.GetSubLayouts(true))
				sublayout.linksProgressMode = RadialLayoutLink.ProgressMode.Animated;

			bool wait = true;

			// First animation
			var targetNode = queryBase.GetNodeWithUniqueId(1);
			var link = queryBase.GetLinkConnecting(this.layout, targetNode);
			link.OnProgressCompleted.RemoveAllListeners();
			link.OnProgressCompleted.AddListener((p) => { wait = false; });
			link.ProgressValue = 1;
			while (wait)
				yield return null;
			link.OnProgressCompleted.RemoveAllListeners();
			targetNode.GetComponent<Animator>().SetTrigger("Highlight");

			// Second animation
			wait = true;
			targetNode = queryBase.GetNodeWithUniqueId(11);
			link = queryBase.GetLinkConnecting(queryBase.GetNodeWithUniqueId(1), targetNode);
			link.OnProgressCompleted.RemoveAllListeners();
			link.OnProgressCompleted.AddListener((p)=> { wait = false; });
			link.ProgressValue = 1;
			while (wait)
				yield return null;
			link.OnProgressCompleted.RemoveAllListeners();
			targetNode.GetComponent<Animator>().SetTrigger("Highlight");

			// Third animation
			wait = true;
			targetNode = queryBase.GetNodeWithUniqueId(4);
			link = queryBase.GetLinkConnecting(queryBase.GetNodeWithUniqueId(11), targetNode);
			link.OnProgressCompleted.RemoveAllListeners();
			link.OnProgressCompleted.AddListener((p) => { wait = false; });
			link.ProgressValue = 1;
			while (wait)
				yield return null;
			link.OnProgressCompleted.RemoveAllListeners();
			targetNode.GetComponent<Animator>().SetTrigger("Highlight");

			// Fourth animation, this time 3 links at once, animating their own target node
			int fillsReached = 0;
			var links = queryBase.GetLinksStartingFrom(targetNode).Links;
			foreach (var l in links)
			{
				l.OnProgressCompleted.RemoveAllListeners();
				l.OnProgressCompleted.AddListener((p) => {
					fillsReached++;
					l.to.GetComponent<Animator>().SetTrigger("Highlight");
					l.OnProgressCompleted.RemoveAllListeners();
				});
				l.ProgressValue = 1;
			}
			while (fillsReached < links.Length)
				yield return null;
			


			// Resetting
			this.layout.linksProgressMode = prevProgressMode;
			foreach (var sublayout in this.layout.GetSubLayouts(true))
				sublayout.linksProgressMode = prevProgressMode;

			this.chainedAnimationRunning = false;
		}
    }
}

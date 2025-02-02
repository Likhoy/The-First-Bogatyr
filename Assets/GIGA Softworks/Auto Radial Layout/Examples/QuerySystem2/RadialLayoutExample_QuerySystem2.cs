using GIGA.AutoRadialLayout;
using GIGA.AutoRadialLayout.QuerySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIGA.AutoRadialLayout.Examples
{

    public class RadialLayoutExample_QuerySystem2 : MonoBehaviour
    {
        public RadialLayout layout;

        private Dictionary<int, string[]> queryButtons = new Dictionary<int, string[]>();
        private Dictionary<int, int> queryButtonsState = new Dictionary<int, int>();

        private void OnGUI()
        {
            GUIStyle areaStyle = new GUIStyle("box");
            areaStyle.normal.background = RadialLayoutExamplesHelper.MakeTex(2, 2, new Color32(0x00,0x4f,0xe1,255));
            GUIStyle boldLabel = new GUIStyle("label");
            boldLabel.fontStyle = FontStyle.Bold;

            RadialLayoutQueryResult result = null;

            GUILayout.BeginArea(new Rect(0, 0, Screen.width / 4f + 50, Screen.height),areaStyle);

            GUILayout.BeginVertical(GUILayout.Width(Screen.width / 4f));
            
            GUILayout.Label("This is a collection of fixed searches to be used as reference",boldLabel);

            // Get all nodes
            if (GUILayout.Button("All Nodes"))
                result = RadialLayoutQueryManager.Begin(this.layout).GetAllNodes();

            // Get all nodes except sub-layouts
            if (GUILayout.Button("All Nodes except sub-layouts"))
                result = RadialLayoutQueryManager.Begin(this.layout).GetAllNodes(false);

            // Get all layouts
            if (GUILayout.Button("All Layouts"))
                result = RadialLayoutQueryManager.Begin(this.layout).GetAllLayouts();

            // Red nodes
            if (GUILayout.Button("Red Nodes"))
                result = RadialLayoutQueryManager.Begin(this.layout).GetNodesWithTags("red");

            // Hearts or Clubs
            if (GUILayout.Button("Hearts or Clubs"))
                result = RadialLayoutQueryManager.Begin(this.layout).GetNodesWithTags("hearts,clubs");

            // Red and eveb
            if (GUILayout.Button("Red and even"))
                result = RadialLayoutQueryManager.Begin(this.layout).GetNodesWithTags("red") & RadialLayoutQueryManager.Begin(this.layout).GetNodesWithTags("two,four,six,eight,ten");

            // All nodes except threes
            if (GUILayout.Button("All Nodes except 3s"))
                result = RadialLayoutQueryManager.Begin(this.layout).GetAllNodes() - RadialLayoutQueryManager.Begin(this.layout).GetNodesWithTags("three");

            // All sub-layouts containing red nodes
            if (GUILayout.Button("Sub-layouts containing red nodes"))
                result = RadialLayoutQueryManager.Begin(this.layout).GetAllLayouts(true) & RadialLayoutQueryManager.Begin(this.layout).GetNodesWithTags("red");

            // All sub-layouts with black tag
            if (GUILayout.Button("Sub-layouts tagged \"black\""))
                result = RadialLayoutQueryManager.Begin(this.layout).GetLayoutsWithTags("black",true);


            GUILayout.EndVertical();

            GUILayout.EndArea();

            if (result != null)
            {
                foreach (var node in result.Nodes)
                {
                    node.GetComponent<Animator>().SetTrigger("Highlight");
                }
                foreach (var layout in result.Layouts)
                {
                    if(layout.GetComponent<Animator>() != null)
                        layout.GetComponent<Animator>().SetTrigger("Highlight");
                    else if(layout.GetComponentInParent<Animator>() != null)
                        layout.GetComponentInParent<Animator>().SetTrigger("Highlight");
                }
            }

        }
    }
}

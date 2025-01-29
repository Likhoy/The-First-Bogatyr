using GIGA.AutoRadialLayout;
using GIGA.AutoRadialLayout.QuerySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIGA.AutoRadialLayout.Examples
{

    public class RadialLayoutExample_QuerySystem : MonoBehaviour
    {
        private int selectedID = 5;
        private int lastSelectedID = 5;
        private bool[] numbers = new bool[] {true,true,true,true,true, true, true, true, true, true };
        private string[] numbersTags = new string[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" };
        private enum QueryButtonsIds {
            Color = 1,
            Operator1 = 2,
            Symbol = 3,
            Operator2 = 4,
            SymbolExcept = 5,
        }

        public RadialLayout layout;

        private Dictionary<int, string[]> queryButtons = new Dictionary<int, string[]>();
        private Dictionary<int, int> queryButtonsState = new Dictionary<int, int>();

        void Update()
        {
            if (selectedID != lastSelectedID)
            {
                var result = RadialLayoutQueryManager.Begin(this.layout).GetNodeResultWithUniqueId(selectedID);
                if (result != null && result.Nodes.Length == 1)
                    result.Nodes[0].GetComponent<Animator>().SetTrigger("Highlight");
            }

            this.lastSelectedID = this.selectedID;
        }

        private void OnGUI()
        {
            GUIStyle areaStyle = new GUIStyle("box");
            areaStyle.normal.background = RadialLayoutExamplesHelper.MakeTex(2, 2, new Color32(0x00,0x4f,0xe1,255));
            GUIStyle boldLabel = new GUIStyle("label");
            boldLabel.fontStyle = FontStyle.Bold;

            GUILayout.BeginArea(new Rect(0, 0, Screen.width / 4f + 50, Screen.height),areaStyle);

            GUILayout.BeginVertical(GUILayout.Width(Screen.width / 4f));
            
            GUILayout.Label("Direct selection:",boldLabel);
            GUILayout.Label("UniqueID: " + this.selectedID);

            this.selectedID = (int)GUILayout.HorizontalSlider(this.selectedID, 5, 44);

            GUILayout.Label("Query Builder: ",boldLabel);

            this.CreateQueryButton((int)QueryButtonsIds.Color, "Color:", 0,"red", "black","red,black");
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            this.CreateQueryButton((int)QueryButtonsIds.Operator1, "",100 ,"and", "or");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            this.CreateQueryButton((int)QueryButtonsIds.Symbol, "Symbol:",0 ,"hearts", "spades","clubs","diamonds");
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            this.CreateQueryButton((int)QueryButtonsIds.Operator2, "",100 ,"and", "or");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Label("Number:");

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            for (int k = 0; k < 5; k++)
                this.numbers[k] = GUILayout.Toggle(this.numbers[k],this.numbersTags[k]);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            for (int k = 5; k < 10; k++)
                this.numbers[k] = GUILayout.Toggle(this.numbers[k], this.numbersTags[k]);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            if (GUILayout.Button("Run Query"))
            {
                this.RunQuery();
            }

            // All nodes except
            GUILayout.Space(20);

            GUILayout.Label("All nodes except:", boldLabel);

            this.CreateQueryButton((int)QueryButtonsIds.SymbolExcept, "Get all except:", 0, "nothing" ,"hearts", "spades", "clubs", "diamonds");

            if (GUILayout.Button("Get nodes"))
            {
                var allNodesResult = RadialLayoutQueryManager.Begin(this.layout).GetAllNodes();

                string exception = this.queryButtons[(int)QueryButtonsIds.SymbolExcept][this.queryButtonsState[(int)QueryButtonsIds.SymbolExcept]];
                if (exception != "nothing")
                {
                    var exceptionResult = RadialLayoutQueryManager.Begin(this.layout).GetNodesWithTags(exception);
                    allNodesResult = allNodesResult - exceptionResult;
                }

                foreach (var node in allNodesResult.Nodes)
                {
                    node.GetComponent<Animator>().SetTrigger("Highlight");
                }
            }

            GUILayout.EndVertical();

            GUILayout.EndArea();

        }

        private void CreateQueryButton(int id, string label,int minWidth,params string[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            if (!this.queryButtons.ContainsKey(id))
            {
                this.queryButtonsState.Add(id, 0);
                this.queryButtons.Add(id, options);
            }
            if (GUILayout.Button(options[this.queryButtonsState[id]],GUILayout.MinWidth(minWidth)))
            {
                this.queryButtonsState[id] = (this.queryButtonsState[id] + 1) % this.queryButtons[id].Length;
            }
            GUILayout.EndHorizontal();
        }

        private void RunQuery()
        {
            // Color
            string color = this.queryButtons[(int)QueryButtonsIds.Color][this.queryButtonsState[(int)QueryButtonsIds.Color]];
            string symbol = this.queryButtons[(int)QueryButtonsIds.Symbol][this.queryButtonsState[(int)QueryButtonsIds.Symbol]];
            string operator1 = this.queryButtons[(int)QueryButtonsIds.Operator1][this.queryButtonsState[(int)QueryButtonsIds.Operator1]];
            string operator2 = this.queryButtons[(int)QueryButtonsIds.Operator2][this.queryButtonsState[(int)QueryButtonsIds.Operator2]];

            RadialLayoutQueryResult result = null;

            if (operator1 == "and")
            {
                // AND operations can be done by simply chaining query calls
                result = RadialLayoutQueryManager.Begin(this.layout).GetNodesWithTags(color).GetNodesWithTags(symbol); // color AND symbol
            }
            else
            {
                // OR operations can be done by combining results with the overloaded operator |
                var result_colors = RadialLayoutQueryManager.Begin(this.layout).GetNodesWithTags(color);
                var result_symbols = RadialLayoutQueryManager.Begin(this.layout).GetNodesWithTags(symbol);
                result = result_colors | result_symbols;
            }

            string numberTagsString = "";
            for (int k = 0; k < 10; k++)
            {
                if (numbers[k])
                {
                    if (k > 0)
                        numberTagsString += ",";
                    numberTagsString += numbersTags[k];
                }
            }

            var result_numbers = RadialLayoutQueryManager.Begin(this.layout).GetNodesWithTags(numberTagsString);

            if (operator2 == "and")
            {
                // AND can also be done wiwth the overloaded operator &
                result = result & result_numbers;
            }
            else
            {
                result = result | result_numbers;
            }


            foreach (var node in result.Nodes)
			{
                node.GetComponent<Animator>().SetTrigger("Highlight");
            }

        }
    }
}

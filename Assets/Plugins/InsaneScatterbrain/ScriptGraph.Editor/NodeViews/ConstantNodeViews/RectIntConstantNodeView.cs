
using UnityEngine;

namespace InsaneScatterbrain.ScriptGraph.Editor
{
    [ConstantNodeView(typeof(RectInt))]
    public class RectIntConstantNodeView : ConstantNodeView
    {
        public RectIntConstantNodeView(ConstantNode node, ScriptGraphView graphView) : base(node, graphView)
        {
            var rectIntValue = (RectInt) node.Value;
            AddDefaultField<RectInt, UnityEngine.UIElements.RectIntField>(rectIntValue);
        }
    }
}
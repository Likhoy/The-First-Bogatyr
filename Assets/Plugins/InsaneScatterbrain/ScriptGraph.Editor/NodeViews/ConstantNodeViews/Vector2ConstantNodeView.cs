
using UnityEngine;

namespace InsaneScatterbrain.ScriptGraph.Editor
{
    [ConstantNodeView(typeof(Vector2))]
    public class Vector2ConstantNodeView : ConstantNodeView
    {
        public Vector2ConstantNodeView(ConstantNode node, ScriptGraphView graphView) : base(node, graphView)
        {
            var vector2Value = (Vector2) node.Value;
            AddDefaultField<Vector2, UnityEngine.UIElements.Vector2Field>(vector2Value);
        }
    }
}
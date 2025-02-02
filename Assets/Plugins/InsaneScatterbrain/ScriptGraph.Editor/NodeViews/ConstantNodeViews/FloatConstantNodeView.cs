

namespace InsaneScatterbrain.ScriptGraph.Editor
{
    [ConstantNodeView(typeof(float))]
    public class FloatConstantNodeView : ConstantNodeView
    {
        public FloatConstantNodeView(ConstantNode node, ScriptGraphView graphView) : base(node, graphView)
        {
            var floatValue = (float) node.Value;
            AddDefaultField<float, UnityEngine.UIElements.FloatField>(floatValue);
        }
    }
}
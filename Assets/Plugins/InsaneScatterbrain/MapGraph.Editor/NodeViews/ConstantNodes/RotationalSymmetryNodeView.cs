using System;
using InsaneScatterbrain.ScriptGraph;
using InsaneScatterbrain.ScriptGraph.Editor;


namespace InsaneScatterbrain.MapGraph.Editor
{
    [ConstantNodeView(typeof(RotationalSymmetry))]
    public class RotationalSymmetryNodeView : ConstantNodeView
    {
        public RotationalSymmetryNodeView(ConstantNode node, ScriptGraphView graphView) : base(node, graphView)
        {
            var value = (RotationalSymmetry) node.Value;
            var field = AddDefaultField<Enum, UnityEngine.UIElements.EnumField>(value);
            field.Init(value);
        }
    }
}
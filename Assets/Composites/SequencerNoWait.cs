using TheKiwiCoder;
using UnityEngine.UIElements;

[System.Serializable]
public class SequencerNoWait : Sequencer
{

    protected override State OnUpdate() 
    {
        current = 0;
        var status = base.OnUpdate();
        
        if (status == State.Failure)
        {
            foreach (var child in children)
            {
                if (child.started && child.state == State.Running)
                    child.Abort();
            }
        } 

        return status;
    }
}

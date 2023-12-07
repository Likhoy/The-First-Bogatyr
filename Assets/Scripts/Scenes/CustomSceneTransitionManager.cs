using PixelCrushers;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneTransitionManager : StandardSceneTransitionManager
{
    public bool areScenesCorrect = false;

    public override IEnumerator LeaveScene()
    {
        if (areScenesCorrect)
            yield return base.LeaveScene();
        yield return null;
    }

    public override IEnumerator EnterScene()
    {
        if (areScenesCorrect)
            yield return base.EnterScene();
        yield return null;

        areScenesCorrect = false;
    }
}

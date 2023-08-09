using PixelCrushers;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneTransitionManager : StandardSceneTransitionManager
{
    [SerializeField] private string leavingSceneName;
    [SerializeField] private string enteringSceneName;

    private string previousSceneName;
    private bool areScenesCorrect;

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    private void SceneManager_sceneUnloaded(Scene previousScene)
    {
        previousSceneName = previousScene.name;
    }

    private void SceneManager_activeSceneChanged(Scene previousScene, Scene nextScene)
    {
        if (previousSceneName == leavingSceneName && nextScene.name == enteringSceneName)
        {
            areScenesCorrect = true;
        }
        else
            areScenesCorrect = false;
    }

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
    }
}

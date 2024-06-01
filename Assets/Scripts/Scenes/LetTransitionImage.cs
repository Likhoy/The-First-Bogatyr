using PixelCrushers.DialogueSystem;
using UnityEngine;

public class LetTransitionImage : MonoBehaviour
{
    public void LetShowSceneTransitionImage()
    {
        DialogueManager.Instance.GetComponent<CustomSceneTransitionManager>().areScenesCorrect = true;
    }
}

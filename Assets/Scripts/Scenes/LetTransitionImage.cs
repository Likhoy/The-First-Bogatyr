using UnityEngine;

public class LetTransitionImage : MonoBehaviour
{
    public void LetShowSceneTransitionImage()
    {
        GameObject.FindGameObjectWithTag(Settings.dialogueManager).GetComponent<CustomSceneTransitionManager>().areScenesCorrect = true;
    }
}

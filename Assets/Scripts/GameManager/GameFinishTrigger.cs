using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishTrigger : MonoBehaviour
{
    private void OnDestroy()
    {
        GameManager.Instance.StartCoroutine(GameManager.Instance.FinishGameRoutine());
    }
}

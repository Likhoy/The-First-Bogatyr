using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishTrigger : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (health.currentHealth <= 0f)
            GameManager.Instance.StartCoroutine(GameManager.Instance.FinishGameRoutine());
    }
}

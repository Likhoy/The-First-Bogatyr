using UnityEngine;
using System;
public class BossLocalSpawner : MonoBehaviour
{

    //private int enemiesSpawnedSoFar = EnemySpawner.Instance.EnemiesSpawnedSoFar;
    private Enemy enemy;
    private bool[] needsToSpawn = new bool[3] { true, false, false };
    private int countCurrrentShadow = 0;


    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        enemy.healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (needsToSpawn[0] && healthEventArgs.healthPercent * 100 < 75f)
        {
            needsToSpawn[0] = false;
            needsToSpawn[1] = true;
            SpawnLittleEnemies();
            enemy.defendingStageStartedEvent.CallDefendingStageStartedEvent();
            GetComponent<PolygonCollider2D>().enabled = false;
            GetComponent<EnemyWeaponAI>().enabled = false;
            GetComponent<EnemyMovementAI>().enabled = false;
        }
        else if (needsToSpawn[1] && healthEventArgs.healthPercent * 100 < 50f)
        {
            needsToSpawn[1] = false;
            needsToSpawn[2] = true;
            SpawnLittleEnemies();
            enemy.defendingStageStartedEvent.CallDefendingStageStartedEvent();
            GetComponent<PolygonCollider2D>().enabled = false;
            GetComponent<EnemyWeaponAI>().enabled = false;
            GetComponent<EnemyMovementAI>().enabled = false;
        }
        else if (needsToSpawn[2] && healthEventArgs.healthPercent * 100 < 25f)
        {
            needsToSpawn[2] = false;
            SpawnLittleEnemies();
            enemy.defendingStageStartedEvent.CallDefendingStageStartedEvent();
            GetComponent<PolygonCollider2D>().enabled = false;
            GetComponent<EnemyWeaponAI>().enabled = false;
            GetComponent<EnemyMovementAI>().enabled = false;
        }
        else if (!needsToSpawn[2] && healthEventArgs.healthPercent * 100 < 25f)
            enemy.healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;

    }

    private void SpawnLittleEnemies()
    {
        if (enemy.enemyDetails.spawningImmediately)
        {
            for (int i = 0; i < enemy.enemyDetails.littleEnemySpawnDatas.Length; i++)
            {
                for (int j = 0; j < enemy.enemyDetails.littleEnemySpawnDatas[i].CountEnemies; j++)
                {
                    SpawnLittleEnemy(enemy.enemyDetails.littleEnemySpawnDatas[i].enemyDetails);
                }
            }
            
        }
        //else
        //{
        //    GameObject littleEnemy = SpawnLittleEnemy(enemy.enemyDetails.littleEnemySpawnDatas.enemyDetails);
        //    // subscribe to enemy destroyed event
        //    littleEnemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
        //}
    }

    private GameObject SpawnLittleEnemy(EnemyDetailsSO enemyDetails)
    {
        Vector2 spawnPosition = new Vector2(UnityEngine.Random.Range(transform.position.x - enemy.enemyDetails.spawnRadius, transform.position.x + enemy.enemyDetails.spawnRadius),
                                            UnityEngine.Random.Range(transform.position.y - enemy.enemyDetails.spawnRadius, transform.position.y + enemy.enemyDetails.spawnRadius));
        GameObject littleEnemy = Instantiate(enemyDetails.enemyPrefab, spawnPosition, Quaternion.identity, transform);
        littleEnemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, EnemySpawner.Instance.EnemiesSpawnedSoFar);
        ++countCurrrentShadow;
        ++EnemySpawner.Instance.EnemiesSpawnedSoFar;
        return littleEnemy;
    }

    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;
        --countCurrrentShadow;
        if (countCurrrentShadow == 0)
        {
            enemy.defendingStageEndedEvent.CallDefendingStageEndedEvent();
            GetComponent<PolygonCollider2D>().enabled = true;
            GetComponent<EnemyWeaponAI>().enabled = true;
            GetComponent<EnemyMovementAI>().enabled = true;
        }
        //if (enemy.enemyDetails.enemiesToSpawn < enemiesSpawnedSoFar)
        //{
        //    GameObject littleEnemy = SpawnLittleEnemy();
        //    // subscribe to enemy destroyed event
        //    littleEnemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
        //}
    }

    /*private void FirstShadowLogic()
    {
        if (currentHealth == startingHealth - 1 && FirstShadow != null)
        {
            FirstShadow.SetActive(true);
        }
    }
    private void SecondShadowLogic()
    {
        if (FirstShadow != null && SecondShadow != null) return;
        SecondShadow.SetActive(true);
    }
    private void ThirdShadowLogic()
    {
        if (SecondShadow != null && ThirdShadow != null) return;
        SecondShadow.SetActive(true);
    }*/


}


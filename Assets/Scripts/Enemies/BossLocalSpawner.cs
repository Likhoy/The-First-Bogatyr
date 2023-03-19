using UnityEngine;
using System;
public class BossLocalSpawner : MonoBehaviour
{

    private int enemiesSpawnedSoFar = 1; // configure
    private Enemy enemy;
    private bool[] needsToSpawn = new bool[3] { true, false, false };

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
        if (needsToSpawn[0] && healthEventArgs.healthPercent - 75f < 0)
        {
            needsToSpawn[0] = false;
            needsToSpawn[1] = true;
            SpawnLittleEnemies();
        }
        else if (needsToSpawn[1] && healthEventArgs.healthPercent - 50f < 0)
        {
            needsToSpawn[1] = false;
            needsToSpawn[2] = true;
            SpawnLittleEnemies();
        }
        else if (needsToSpawn[2] && healthEventArgs.healthPercent - 25f < 0)
        {
            needsToSpawn[2] = false;
            SpawnLittleEnemies();
        }
        else if (healthEventArgs.healthPercent < 75)
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
        Vector2 spawnPosition = new Vector2(UnityEngine.Random.Range(transform.position.x - enemy.enemyDetails.spawnRadius / 2, transform.position.x + enemy.enemyDetails.spawnRadius / 2),
                                            UnityEngine.Random.Range(transform.position.y - enemy.enemyDetails.spawnRadius / 2, transform.position.y + enemy.enemyDetails.spawnRadius / 2));
        GameObject littleEnemy = Instantiate(enemyDetails.enemyPrefab, spawnPosition, Quaternion.identity, transform);
        littleEnemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, enemiesSpawnedSoFar);

        ++enemiesSpawnedSoFar;
        return littleEnemy;
    }

    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;

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


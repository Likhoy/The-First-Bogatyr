using UnityEngine;
public class BossLocalSpawner : MonoBehaviour
{

    private Enemy enemy;
    private new Rigidbody2D rigidbody2D;
    private EnemyWeaponAI enemyWeaponAI;
    private PolygonCollider2D polygonCollider2D;

    private bool[] needsToSpawn = new bool[3] { true, false, false };
    private int countCurrrentShadow = 0;


    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        enemyWeaponAI = GetComponent<EnemyWeaponAI>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
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
            ToggleDefendingStageEvent(true);
        }
        else if (needsToSpawn[1] && healthEventArgs.healthPercent * 100 < 50f)
        {
            needsToSpawn[1] = false;
            needsToSpawn[2] = true;
            SpawnLittleEnemies();
            ToggleDefendingStageEvent(true);
        }
        else if (needsToSpawn[2] && healthEventArgs.healthPercent * 100 < 25f)
        {
            needsToSpawn[2] = false;
            SpawnLittleEnemies();
            ToggleDefendingStageEvent(true);
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
        GameObject littleEnemy = Instantiate(enemyDetails.enemyPrefabs[0], spawnPosition, Quaternion.identity, transform.parent); // handle enemy prefabs
        littleEnemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, EnemySpawner.Instance.EnemiesSpawnedSoFar);
        ++countCurrrentShadow;
        ++EnemySpawner.Instance.EnemiesSpawnedSoFar;
        // subscribe to enemy destroyed event
        littleEnemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
        return littleEnemy;
    }

    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;
        --countCurrrentShadow;
        if (countCurrrentShadow == 0)
        {
            ToggleDefendingStageEvent(false);
        }
        //if (enemy.enemyDetails.enemiesToSpawn < enemiesSpawnedSoFar)
        //{
        //    GameObject littleEnemy = SpawnLittleEnemy();
        //    // subscribe to enemy destroyed event
        //    littleEnemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
        //}
    }

    /// <summary>
    /// Handle enemy defending stage event
    /// </summary>
    private void ToggleDefendingStageEvent(bool isStarting)
    {
        polygonCollider2D.enabled = !isStarting;
        //enemy.enemyMovementAI.enabled = !isStarting;
        enemyWeaponAI.enabled = !isStarting;
        if (isStarting)
        {
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            enemy.defendingStageStartedEvent.CallDefendingStageStartedEvent();
        }
        else
        {
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            enemy.defendingStageEndedEvent.CallDefendingStageEndedEvent();
        }
    }

}


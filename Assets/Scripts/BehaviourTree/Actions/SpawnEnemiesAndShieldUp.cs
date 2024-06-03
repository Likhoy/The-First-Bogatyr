using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class SpawnEnemiesAndShieldUp : ActionNode
{
    private DefendingStageStartedEvent defendingStageStartedEvent;

    private DefendingStageEndedEvent defendingStageEndedEvent;

    private PolygonCollider2D polygonCollider;

    private int enemiesSpawnedLeft;

    [SerializeField] private EnemyDetailsSO enemyDetails;

    [SerializeField] private NodeProperty<bool> defending;

    public override void OnInit()
    {
        base.OnInit();

        polygonCollider = context.gameObject.GetComponent<PolygonCollider2D>();

        defendingStageStartedEvent = context.gameObject.GetComponent<DefendingStageStartedEvent>();

        defendingStageEndedEvent = context.gameObject.GetComponent<DefendingStageEndedEvent>();
    }

    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate() 
    {
        polygonCollider.enabled = false;

        SpawnEnemies();

        defendingStageStartedEvent.CallDefendingStageStartedEvent();

        defending.Value = true;

        return State.Success;
    }

    private void SpawnEnemies()
    {
        if (enemyDetails.spawningImmediately)
        {
            for (int i = 0; i < enemyDetails.littleEnemySpawnDatas.Length; i++)
            {
                for (int j = 0; j < enemyDetails.littleEnemySpawnDatas[i].CountEnemies; j++)
                {
                    SpawnEnemy(enemyDetails.littleEnemySpawnDatas[i].enemyDetails);
                }
            }

        }

    }

    private GameObject SpawnEnemy(EnemyDetailsSO enemyDetails)
    {
        Vector2 spawnPosition = new Vector2(Random.Range(context.transform.position.x - enemyDetails.spawnRadius, context.transform.position.x + enemyDetails.spawnRadius),
                                            Random.Range(context.transform.position.y - enemyDetails.spawnRadius, context.transform.position.y + enemyDetails.spawnRadius));
        
        GameObject littleEnemy = Object.Instantiate(enemyDetails.enemyPrefabs[0], spawnPosition, Quaternion.identity, context.transform.parent); // handle enemy prefabs
        
        littleEnemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, EnemySpawner.Instance.EnemiesSpawnedSoFar);
        
        ++enemiesSpawnedLeft;
        ++EnemySpawner.Instance.EnemiesSpawnedSoFar;
        
        // subscribe to enemy destroyed event
        littleEnemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
        return littleEnemy;
    }

    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;
        
        --enemiesSpawnedLeft;
        
        if (enemiesSpawnedLeft == 0)
        {
            polygonCollider.enabled = true;

            defendingStageEndedEvent.CallDefendingStageEndedEvent();

            defending.Value = false;
        }
    }

}

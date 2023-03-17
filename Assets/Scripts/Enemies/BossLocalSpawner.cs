using UnityEngine;
public class Chernobog : MonoBehaviour
{
    /*private GameObject FirstShadow;
    private GameObject SecondShadow;
    private GameObject ThirdShadow;
    [SerializeField] private GameObject FirstShadowPrefab;
    [SerializeField] private GameObject SecondShadowPrefab;
    [SerializeField] private GameObject ThirdShadowPrefab;*/

    private int enemiesSpawnedSoFar = 1; // configure
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        enemy.healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    /*private void Update()
    {
       
    }*/

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        SpawnLittleEnemies();
        enemy.healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void SpawnLittleEnemies()
    {

        if (enemy.enemyDetails.spawningImmediately)
        {
            for (int i = 0; i < enemy.enemyDetails.enemiesToSpawn; i++)
            {
                SpawnLittleEnemy();
            }
        }
        else
        {
            GameObject littleEnemy = SpawnLittleEnemy();
            // subscribe to enemy destroyed event
            littleEnemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
        }
    }

    

    private GameObject SpawnLittleEnemy()
    {
        Vector2 spawnPosition = new Vector2(Random.Range(transform.position.x - enemy.enemyDetails.spawnRadius / 2, transform.position.x + enemy.enemyDetails.spawnRadius / 2),
                                                     Random.Range(transform.position.y - enemy.enemyDetails.spawnRadius / 2, transform.position.y + enemy.enemyDetails.spawnRadius / 2));
        GameObject littleEnemy = Instantiate(enemy.enemyDetails.enemyPrefab, spawnPosition, Quaternion.identity, transform);
        littleEnemy.GetComponent<Enemy>().EnemyInitialization(enemy.enemyDetails.littleEnemyDetails, enemiesSpawnedSoFar);

        ++enemiesSpawnedSoFar;
        return littleEnemy;
    }

    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;

        if (enemy.enemyDetails.enemiesToSpawn < enemiesSpawnedSoFar)
        {
            GameObject littleEnemy = SpawnLittleEnemy();
            // subscribe to enemy destroyed event
            littleEnemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
        }
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


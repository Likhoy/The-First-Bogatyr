using UnityEngine;

[System.Serializable]
public struct EnemySpawnData
{
    public EnemyDetailsSO enemyDetails;
    public Vector2 spawnPosition;
}

[System.Serializable]
public struct EnemiesGroupWaveSpawnData
{
    public EnemyDetailsSO[] enemiesBaseData;
    public int amountOfEnemiesToSpawn; // maybe we can make a relative strength of enemies limiter 
    public float delayAfterPreviousSpawn;
}

[System.Serializable]
public struct LittleEnemySpawnData
{
    public EnemyDetailsSO enemyDetails;
    public int CountEnemies;
}



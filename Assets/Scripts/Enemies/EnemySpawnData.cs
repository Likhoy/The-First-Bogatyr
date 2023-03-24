using UnityEngine;

[System.Serializable]
public struct EnemySpawnData
{
    public EnemyDetailsSO enemyDetails;
    public Vector2Int spawnPosition;
}



[System.Serializable]
public struct LittleEnemySpawnData
{
    public EnemyDetailsSO enemyDetails;
    public int CountEnemies;
}



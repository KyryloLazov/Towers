using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public EnemyConfig normalEnemyConfig;
    public GameObject normalEnemyPrefab;

    public EnemyConfig tankEnemyConfig;
    public GameObject tankEnemyPrefab;

    public EnemyConfig fastEnemyConfig;
    public GameObject fastEnemyPrefab;
    public float spawnRate;
}

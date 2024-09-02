using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public static int enemiesAlive = 0;

    public int baseCount = 5;
    public float turretMult = 1.5f;

    public Transform spawnPoint;
    public Wave[] waves;
    public Text WaveCountdown;
    public GameManager manager;

    public float timeBetweenWaves = 20f;
    private float counter = 2f;

    public int WaveNumber = 0;

    private void Start()
    {
        WaveNumber = 0;
    }

    private void Update()
    {      
        if (enemiesAlive > 0)
        {
            return;
        }

        if (WaveNumber == waves.Length)
        {
            manager.WinLevel();
            this.enabled = false;
        }

        if (counter <= 0f)
        {
            StartCoroutine(SpawnWave());

            counter = timeBetweenWaves;
        }

        counter -= Time.deltaTime;

        counter = Mathf.Clamp(counter, 0f, Mathf.Infinity);

        WaveCountdown.text = string.Format("{0:00.00}", counter);
    }

    IEnumerator SpawnWave()
    {       
        PlayerStats.Stats.Rounds++;

        Wave wave = waves[WaveNumber];
        int amount = CalculateEnemiesCount();

        for (int i = 0; i < amount; i++)
        {
            GameObject enemyToSpawn = SelectEnemyType(wave);

            if (enemyToSpawn == wave.Roque)
            {
                for (int j = 0; j < 3; j++)
                {
                    SpawnEnemy(enemyToSpawn);
                    yield return new WaitForSeconds(1f / wave.spawnRate);
                }
            }
            else
            {
                SpawnEnemy(enemyToSpawn);
                yield return new WaitForSeconds(1f / wave.spawnRate);
            }
        }

        WaveNumber++;       
    }

    GameObject SelectEnemyType(Wave wave)
    {
        float randomValue = Random.Range(0f, 1f);
        float laserProbability = BuildManager.NumOfLasTurrets / (float)(BuildManager.NumOfTurrets + 1);
        float rocketProbability = BuildManager.NumOfRockTurrets / (float)(BuildManager.NumOfTurrets + 1); ;

        if (randomValue < laserProbability)
        {
            return wave.Roque; 
        }
        else if (randomValue < laserProbability + rocketProbability)
        {
            return wave.Tanks; 
        }
        else
        {
            return wave.Default; 
        }
    }

    private int CalculateEnemiesCount()
    {
        int additionalEnemies = Mathf.RoundToInt(WaveNumber * GameManager.difficultyRate + turretMult * BuildManager.NumOfTurrets);

        return baseCount + additionalEnemies;
    }

    void SpawnEnemy(GameObject Enemy)
    {       
        Instantiate(Enemy, spawnPoint.position, spawnPoint.rotation);
        enemiesAlive++;
    }
}

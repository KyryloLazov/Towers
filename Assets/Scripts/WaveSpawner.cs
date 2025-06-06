using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public static int enemiesAlive = 0;

    public int baseCount = 3;
    public float turretMult = 1.5f;

    public Transform spawnPoint;
    public Wave[] waves;
    public Text WaveCountdown;
    public GameManager manager;
    [SerializeField] BuildManager buildManager;
    [SerializeField] Transform waypointParent;

    public float timeBetweenWaves = 20f;
    private float counter = 2f;

    public int WaveNumber = 0;

    public int populationSize = 20;
    public int generations = 10;
    public float mutationRate = 0.1f;
    public int waveMaxIndividualEnemies = 15;

    private float distance = 0;

    private void Start()
    {
        WaveNumber = 0;
        if (waypointParent != null) {
            for (int i = 0; i < waypointParent.childCount; i++)
            {
                if (i + 1 < waypointParent.childCount)
                {
                    distance += Vector3.Distance(waypointParent.GetChild(i).transform.position,
                        waypointParent.GetChild(i + 1).transform.position);
                }
            }
        }
        Debug.Log($"Distance = {distance}");
        counter = timeBetweenWaves;
    }

    private void Update()
    {
        if (enemiesAlive > 0)
        {
            return;
        }

        if (WaveNumber == waves.Length && waves.Length > 0)
        {
            if (manager != null) manager.WinLevel();
            this.enabled = false;
            return;
        }
        
        if (waves.Length == 0) 
        {
            if (manager != null) manager.WinLevel();
            this.enabled = false;
            return;
        }


        if (counter <= 0f)
        {
            StartCoroutine(SpawnWave());
            counter = timeBetweenWaves;
        }

        counter -= Time.deltaTime;
        counter = Mathf.Clamp(counter, 0f, Mathf.Infinity);

        if (WaveCountdown != null) WaveCountdown.text = string.Format("{0:00.00}", counter);
    }

    IEnumerator SpawnWave()
    {
        if (PlayerStats.Stats != null) PlayerStats.Stats.Rounds++;

        int currentWaveMaxEnemies = CalculateEnemiesCount();

        List<BaseTurretConfig> activeTurrets = buildManager != null ? buildManager.GetActiveTurretConfigs() : new List<BaseTurretConfig>();

        Dictionary<EnemyType, EnemyConfig> enemyConfigsForGA = new Dictionary<EnemyType, EnemyConfig>();

        if (WaveNumber < waves.Length)
        {
            Wave currentWaveData = waves[WaveNumber];
            if (currentWaveData.normalEnemyConfig != null)
                enemyConfigsForGA[EnemyType.Normal] = currentWaveData.normalEnemyConfig;
            if (currentWaveData.tankEnemyConfig != null)
                enemyConfigsForGA[EnemyType.Tank] = currentWaveData.tankEnemyConfig;
            if (currentWaveData.fastEnemyConfig != null)
                enemyConfigsForGA[EnemyType.Fast] = currentWaveData.fastEnemyConfig;
            
            if (enemyConfigsForGA.Count < 3) {
                 Debug.LogWarning($"Not all enemy configs are available for wave {WaveNumber}. GA might be suboptimal.");
            }
        } else {
            Debug.LogError($"WaveNumber {WaveNumber} is out of bounds for waves array. Stopping spawn.");
            yield break;
        }
        
        if (enemyConfigsForGA.Count == 0) {
            Debug.LogError("No enemy configs loaded for GA. Aborting wave spawn.");
            yield break;
        }


        GeneticAlgorithm ga = new GeneticAlgorithm(populationSize, generations, mutationRate,
            currentWaveMaxEnemies, distance, enemyConfigsForGA);

        WaveGenome optimalGenome = ga.RunGA(activeTurrets);

        Debug.Log($"Optimal Wave - Normal: {optimalGenome.normalCount}, Tanks: {optimalGenome.tankCount}, Fast Groups: {optimalGenome.fastGroupCount} (Total units: {optimalGenome.normalCount + optimalGenome.tankCount + optimalGenome.fastGroupCount * 3})");

        List<GameObject> enemiesToSpawn = new List<GameObject>();
        Wave wavePrefabs = waves[WaveNumber];

        for (int i = 0; i < optimalGenome.normalCount; i++)
            if(wavePrefabs.normalEnemyPrefab != null) enemiesToSpawn.Add(wavePrefabs.normalEnemyPrefab);

        for (int i = 0; i < optimalGenome.tankCount; i++)
            if(wavePrefabs.tankEnemyPrefab != null) enemiesToSpawn.Add(wavePrefabs.tankEnemyPrefab);

        for (int i = 0; i < optimalGenome.fastGroupCount; i++)
        {
            for (int j = 0; j < 3; j++)
                if(wavePrefabs.fastEnemyPrefab != null) enemiesToSpawn.Add(wavePrefabs.fastEnemyPrefab);
        }
        
        Debug.Log($"WaveSpawner: Attempting to spawn {enemiesToSpawn.Count} enemies.");
        if (enemiesToSpawn.Count == 0 && (optimalGenome.normalCount > 0 || optimalGenome.tankCount > 0 || optimalGenome.fastGroupCount > 0)) {
            Debug.LogWarning("Optimal genome has counts > 0, but no enemies will be spawned. Check prefab assignments in Wave array.");
            if (wavePrefabs.normalEnemyPrefab == null) Debug.LogWarning("NormalEnemyPrefab is NULL for current wave.");
            if (wavePrefabs.tankEnemyPrefab == null) Debug.LogWarning("TankEnemyPrefab is NULL for current wave.");
            if (wavePrefabs.fastEnemyPrefab == null) Debug.LogWarning("FastEnemyPrefab is NULL for current wave.");
        }


        enemiesToSpawn = ShuffleList(enemiesToSpawn);

        float spawnInterval = (wavePrefabs.spawnRate > 0) ? 1f / wavePrefabs.spawnRate : 1f;
        foreach (GameObject enemyPrefab in enemiesToSpawn)
        {
            if (enemyPrefab != null) SpawnEnemy(enemyPrefab);
            yield return new WaitForSeconds(spawnInterval);
        }

        WaveNumber++;
    }

    private List<GameObject> ShuffleList(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
        return list;
    }

    private int CalculateEnemiesCount()
    {
        int baseEnemies = baseCount;
        float difficultyRate = GameManager.difficultyRate;
        int currentNumTurrets = (buildManager != null) ? buildManager.NumOfTurrets : 0;


        float waveScaling = 1 + Mathf.Log(WaveNumber + 1) * difficultyRate;
        float turretScaling = 1 + (turretMult * currentNumTurrets) / 10f;
        float additionalEnemies = baseEnemies * waveScaling * turretScaling;
        int totalEnemies = Mathf.RoundToInt(additionalEnemies);

        totalEnemies = Mathf.Clamp(totalEnemies, baseCount, waveMaxIndividualEnemies);

        Debug.Log($"Calculated Max Enemies for GA: {totalEnemies}");
        return totalEnemies;
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (spawnPoint == null) {
            Debug.LogError("SpawnPoint is not assigned in WaveSpawner!");
            return;
        }
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        enemiesAlive++;
    }
}

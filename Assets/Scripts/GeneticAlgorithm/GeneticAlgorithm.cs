using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
{
    public int populationSize;
    public int generations;
    public float mutationRate;
    public int maxTotalEnemyUnits;
    public float pathLength;

    private List<WaveGenome> population; 
    private List<float> fitnesses;

    private Dictionary<EnemyType, EnemyConfig> enemyConfigsMap;

    public GeneticAlgorithm(int populationSize, int generations, float mutationRate,
        int maxEnemyUnits, float distance,
        Dictionary<EnemyType, EnemyConfig> enemyConfigs)
    {
        this.populationSize = populationSize;
        this.generations = generations;
        this.mutationRate = mutationRate;
        this.maxTotalEnemyUnits = maxEnemyUnits;
        this.pathLength = distance;
        this.enemyConfigsMap = enemyConfigs;

        population = new List<WaveGenome>();
        fitnesses = new List<float>();
    }

    public WaveGenome RunGA(List<BaseTurretConfig> activePlayerTurrets)
    {
        InitializePopulation();
        WaveGenome overallBestGenome = population.Count > 0 ? population[0] : new WaveGenome(0,0,0);
        float overallBestFitness = float.MinValue;

        for (int gen = 0; gen < generations; gen++)
        {
            EvaluateFitness(activePlayerTurrets); 

            if (population.Count > 0 && fitnesses.Count == population.Count)
            {
                float currentGenBestFitness = float.MinValue;
                WaveGenome currentGenBestGenome = population[0]; 
                if (population.Count > 0) 
                {
                    currentGenBestGenome = population[0];
                    for (int i = 0; i < population.Count; i++)
                    {
                        if (fitnesses[i] > currentGenBestFitness)
                        {
                            currentGenBestFitness = fitnesses[i];
                            currentGenBestGenome = population[i];
                        }
                    }
                }


                long totalUnitsInBest = (long)currentGenBestGenome.normalCount + currentGenBestGenome.tankCount + (long)currentGenBestGenome.fastGroupCount * 3;
                Debug.Log($"Generation {gen + 1}: Best Fitness (AvgNormDist) = {currentGenBestFitness:F4}, Wave = [N: {currentGenBestGenome.normalCount}, T: {currentGenBestGenome.tankCount}, FG: {currentGenBestGenome.fastGroupCount}], Total Units: {totalUnitsInBest}, MaxAllowed: {this.maxTotalEnemyUnits}");

                if (totalUnitsInBest > this.maxTotalEnemyUnits)
                {
                    Debug.LogError($"CRITICAL: Gen {gen + 1} best genome (Total: {totalUnitsInBest}) exceeds maxTotalEnemyUnits ({this.maxTotalEnemyUnits})!");
                }

                if (currentGenBestFitness > overallBestFitness)
                {
                    overallBestFitness = currentGenBestFitness;
                    overallBestGenome = currentGenBestGenome;
                }
            }

            List<WaveGenome> selected = Selection();
            List<WaveGenome> offspring = Crossover(selected);
            Mutate(offspring);
            population = offspring;
        }

        EvaluateFitness(activePlayerTurrets);
        float finalEvalBestFitness = float.MinValue;
        WaveGenome finalEvalBestGenome = population.Count > 0 ? population[0] : new WaveGenome(0,0,0);


        if (population.Count > 0 && fitnesses.Count == population.Count)
        {
             if (population.Count > 0) 
             {
                finalEvalBestGenome = population[0]; 
                for (int i = 0; i < population.Count; i++)
                {
                    if (fitnesses[i] > finalEvalBestFitness)
                    {
                        finalEvalBestFitness = fitnesses[i];
                        finalEvalBestGenome = population[i];
                    }
                }
             }
             if (finalEvalBestFitness > overallBestFitness) {
                overallBestFitness = finalEvalBestFitness; 
                overallBestGenome = finalEvalBestGenome;
             }
        }
        
        long finalTotalUnits = (long)overallBestGenome.normalCount + overallBestGenome.tankCount + (long)overallBestGenome.fastGroupCount * 3;
        Debug.Log($"GA Final Check: Best Genome Total Units: {finalTotalUnits}, MaxAllowed: {this.maxTotalEnemyUnits}. Fitness: {overallBestFitness:F4}");
        if (finalTotalUnits > this.maxTotalEnemyUnits)
        {
            Debug.LogError($"GA IS RETURNING A GENOME (Total: {finalTotalUnits}) LARGER THAN maxTotalEnemyUnits ({this.maxTotalEnemyUnits})!");
        }

        return overallBestGenome;
    }

    private void InitializePopulation()
    {
        population.Clear();
        for (int i = 0; i < populationSize; i++)
        {
            int normalMax = this.maxTotalEnemyUnits;
            int n = Random.Range(0, normalMax + 1);

            int tankMax = this.maxTotalEnemyUnits - n;
            int t = Random.Range(0, Mathf.Max(0, tankMax) + 1);

            int fastUnitsMax = this.maxTotalEnemyUnits - n - t;
            int s_groups = (fastUnitsMax > 0) ? Random.Range(0, Mathf.Max(0, fastUnitsMax / 3) + 1) : 0;

            WaveGenome genome = new WaveGenome(n, t, s_groups);
            ValidateGenomeCounts(ref genome); 
            population.Add(genome);
        }
    }

    private void EvaluateFitness(List<BaseTurretConfig> activePlayerTurrets)
    {
        fitnesses.Clear();
        foreach (var genome in population)
        {
            float fitness = EstimateFitnessByDistance(genome, activePlayerTurrets, this.enemyConfigsMap, this.pathLength);
            fitnesses.Add(fitness);
        }
    }

    private float EstimateFitnessByDistance(WaveGenome genome, List<BaseTurretConfig> activePlayerTurrets,
                                           Dictionary<EnemyType, EnemyConfig> enemyConfigs, float currentPathLength)
    {
        if (enemyConfigs == null || !enemyConfigs.ContainsKey(EnemyType.Normal) ||
            !enemyConfigs.ContainsKey(EnemyType.Tank) || !enemyConfigs.ContainsKey(EnemyType.Fast) || currentPathLength <= 0)
        {
            return 0; 
        }

        float totalNormalizedDistance = 0;
        int totalUnitsInWave = genome.normalCount + genome.tankCount + (genome.fastGroupCount * 3);
        if (totalUnitsInWave == 0) return 0; 

        float difficultyMultiplier = GameManager.difficultyRate;

        if (genome.normalCount > 0 && enemyConfigs.ContainsKey(EnemyType.Normal))
        {
            float normalHP = enemyConfigs[EnemyType.Normal].StartHealth * difficultyMultiplier;
            float baseNormalSpeed = enemyConfigs[EnemyType.Normal].StartSpeed;
            float effectiveNormalSpeed = CalculateEffectiveSpeedForType(baseNormalSpeed, EnemyType.Normal, activePlayerTurrets);
            float potentialDamageToNormal = CalculatePotentialDamageToEnemyType(
                EnemyType.Normal, effectiveNormalSpeed, currentPathLength, activePlayerTurrets, genome, normalHP
            );
            totalNormalizedDistance += CalculateDistanceContribution(
                genome.normalCount, normalHP, potentialDamageToNormal, effectiveNormalSpeed, currentPathLength
            );
        }

        if (genome.tankCount > 0 && enemyConfigs.ContainsKey(EnemyType.Tank))
        {
            float tankHP = enemyConfigs[EnemyType.Tank].StartHealth * difficultyMultiplier;
            float baseTankSpeed = enemyConfigs[EnemyType.Tank].StartSpeed;
            float effectiveTankSpeed = CalculateEffectiveSpeedForType(baseTankSpeed, EnemyType.Tank, activePlayerTurrets);
            float potentialDamageToTank = CalculatePotentialDamageToEnemyType(
                EnemyType.Tank, effectiveTankSpeed, currentPathLength, activePlayerTurrets, genome, tankHP
            );
            totalNormalizedDistance += CalculateDistanceContribution(
                genome.tankCount, tankHP, potentialDamageToTank, effectiveTankSpeed, currentPathLength
            );
        }
        
        if (genome.fastGroupCount > 0 && enemyConfigs.ContainsKey(EnemyType.Fast))
        {
            float fastHP = enemyConfigs[EnemyType.Fast].StartHealth * difficultyMultiplier; 
            float baseFastSpeed = enemyConfigs[EnemyType.Fast].StartSpeed;
            float effectiveFastSpeed = CalculateEffectiveSpeedForType(baseFastSpeed, EnemyType.Fast, activePlayerTurrets);
            float potentialDamageToFast = CalculatePotentialDamageToEnemyType(
                EnemyType.Fast, effectiveFastSpeed, currentPathLength, activePlayerTurrets, genome, fastHP
            );
            totalNormalizedDistance += CalculateDistanceContribution(
                genome.fastGroupCount * 3, fastHP, potentialDamageToFast, effectiveFastSpeed, currentPathLength
            );
        }
        
        return totalNormalizedDistance / totalUnitsInWave; 
    }

    private float CalculateEffectiveSpeedForType(float baseSpeed, EnemyType type, List<BaseTurretConfig> turrets)
    {
        float currentSpeed = baseSpeed;
        if (currentSpeed <= 0) return 0.01f; 

        float maxSlowFactor = 0f;
        foreach (var turretConfig in turrets)
        {
            if (turretConfig is LaserTurretConfig laserCfg)
            {
                maxSlowFactor = Mathf.Max(maxSlowFactor, laserCfg.slowFactor);
            }
        }
        if (maxSlowFactor > 0)
        {
            currentSpeed *= (1f - Mathf.Clamp01(maxSlowFactor)); 
        }
        return Mathf.Max(0.01f, currentSpeed); 
    }

    private float CalculatePotentialDamageToEnemyType(
        EnemyType enemyType, float effectiveSpeed, float pathLength,
        List<BaseTurretConfig> activeTurretConfigs, WaveGenome genome, float enemyHP)
    {
        float totalDamage = 0f;
        if (effectiveSpeed <= 0 && pathLength > 0) return enemyHP > 0 ? 0 : float.MaxValue; 
        if (pathLength <= 0) return 0; 

        float maxTimeOnPath = pathLength / effectiveSpeed;
        
        foreach (var turretConfig in activeTurretConfigs)
        {
            float range = turretConfig.Range;
            if (range <= 0) continue;

            float timeInThisTurretRange = range / effectiveSpeed;
            
            float engagementTime = Mathf.Min(timeInThisTurretRange, maxTimeOnPath);
            if (engagementTime <= 0) continue;

            if (turretConfig is BulletTurretConfig bulletCfg)
            {
                totalDamage += bulletCfg.Damage * bulletCfg.FireRate * engagementTime;
            }
            else if (turretConfig is LaserTurretConfig laserCfg)
            {
                totalDamage += laserCfg.damageOverTime * engagementTime;
            }
        }

        float totalEffectiveRocketDpsTimesTargets = 0f;
        foreach (var turretConfig in activeTurretConfigs)
        {
            if (turretConfig is RocketTurretConfig rocketCfg)
            {
                totalEffectiveRocketDpsTimesTargets += rocketCfg.Damage * rocketCfg.fireRate * rocketCfg.ApproxTargetsInExplosion;
            }
        }

        int totalEnemyUnitsInWave = genome.normalCount + genome.tankCount + (genome.fastGroupCount * 3);
        if (totalEnemyUnitsInWave > 0 && totalEffectiveRocketDpsTimesTargets > 0)
        {
            float waveDurationEstimate = maxTimeOnPath; 
            float totalAoEDamageFromRocketsInWave = totalEffectiveRocketDpsTimesTargets * waveDurationEstimate;
            float aoeDamageBonusPerEnemyUnit = totalAoEDamageFromRocketsInWave / totalEnemyUnitsInWave;
            totalDamage += aoeDamageBonusPerEnemyUnit;
        }
        return totalDamage;
    }
    
    private float CalculateDistanceContribution(
        int count, float hp, float potentialDamage, float effectiveSpeed, float pathLength)
    {
        if (count == 0 || pathLength <= 0) return 0;
        if (hp <= 0) return 0; 

        if (potentialDamage >= hp) 
        {
            if (hp <= 0) return 0; 
            if (effectiveSpeed <= 0) return 0; 

            float maxTimeOnPathIfSurvived = pathLength / effectiveSpeed;
            if (maxTimeOnPathIfSurvived <= 0) return 0; 

            float averageDpsToThisEnemy = potentialDamage / maxTimeOnPathIfSurvived;
            if (averageDpsToThisEnemy <= 0) return 0; 

            float timeToKill = hp / averageDpsToThisEnemy;
            float distanceTraveled = Mathf.Min(pathLength, effectiveSpeed * timeToKill);
            return (distanceTraveled / pathLength) * count;
        }
        
        return 1.0f * count; 
        
    }

    private List<WaveGenome> Selection()
    {
        List<WaveGenome> selected = new List<WaveGenome>();
        if (population.Count == 0 || fitnesses.Count != population.Count)
        {
            return new List<WaveGenome>(population);
        }

        for (int i = 0; i < populationSize; i++)
        {
            int tournamentSize = Mathf.Min(3, population.Count); 
            if (tournamentSize == 0) continue;

            int bestIndexInTournament = Random.Range(0, population.Count);
            if (bestIndexInTournament >= fitnesses.Count) {
                if (population.Count > 0) bestIndexInTournament = 0; else continue;
            }

            float bestFitnessInTournament = fitnesses[bestIndexInTournament];

            for (int j = 1; j < tournamentSize; j++)
            {
                int competitorIndex = Random.Range(0, population.Count);
                 if (competitorIndex >= fitnesses.Count) {
                    if (population.Count > 0) competitorIndex = 0; else continue;
                 }
                if (fitnesses[competitorIndex] > bestFitnessInTournament)
                {
                    bestIndexInTournament = competitorIndex;
                    bestFitnessInTournament = fitnesses[competitorIndex];
                }
            }
            selected.Add(population[bestIndexInTournament]);
        }
        return selected;
    }

    private List<WaveGenome> Crossover(List<WaveGenome> parents)
    {
        List<WaveGenome> offspring = new List<WaveGenome>();
        if (parents.Count == 0) return offspring;

        for (int i = 0; i < parents.Count - 1; i++)
        {
            int randomIndex = Random.Range(i, parents.Count);
            (parents[i], parents[randomIndex]) = (parents[randomIndex], parents[i]);
        }

        for (int i = 0; i < parents.Count; i += 2)
        {
            if (i + 1 >= parents.Count) 
            {
                WaveGenome singleParent = parents[i];
                ValidateGenomeCounts(ref singleParent); 
                offspring.Add(singleParent);
                continue;
            }
            WaveGenome parent1 = parents[i];
            WaveGenome parent2 = parents[i + 1];

            WaveGenome child1 = new WaveGenome(
                Random.value < 0.5f ? parent1.normalCount : parent2.normalCount,
                Random.value < 0.5f ? parent1.tankCount : parent2.tankCount,
                Random.value < 0.5f ? parent1.fastGroupCount : parent2.fastGroupCount
            );
            WaveGenome child2 = new WaveGenome(
                Random.value < 0.5f ? parent2.normalCount : parent1.normalCount, 
                Random.value < 0.5f ? parent2.tankCount : parent1.tankCount,
                Random.value < 0.5f ? parent2.fastGroupCount : parent1.fastGroupCount
            );

            ValidateGenomeCounts(ref child1);
            ValidateGenomeCounts(ref child2);

            offspring.Add(child1);
            offspring.Add(child2);
        }

        while (offspring.Count < populationSize && parents.Count > 0)
        {
            WaveGenome parentCopy = parents[Random.Range(0, parents.Count)];
            ValidateGenomeCounts(ref parentCopy);
            offspring.Add(parentCopy);
        }
        if (offspring.Count > populationSize)
        {
            offspring = offspring.GetRange(0, populationSize);
        }
        return offspring;
    }

    private void ValidateGenomeCounts(ref WaveGenome genome)
    {
        genome.normalCount = Mathf.Max(0, genome.normalCount);
        genome.tankCount = Mathf.Max(0, genome.tankCount);
        genome.fastGroupCount = Mathf.Max(0, genome.fastGroupCount);

        int attempts = 0; 
        while ((genome.normalCount + genome.tankCount + (genome.fastGroupCount * 3)) > this.maxTotalEnemyUnits && attempts < 200) 
        {
            attempts++;
            if (genome.fastGroupCount > 0 && (this.maxTotalEnemyUnits == 0 || (genome.normalCount + genome.tankCount + (genome.fastGroupCount * 3) - 3) >= this.maxTotalEnemyUnits -3 || (genome.normalCount + genome.tankCount + (genome.fastGroupCount * 3) - 3) >=0 )) { 
                genome.fastGroupCount--; 
            }
            else if (genome.tankCount > 0 && (this.maxTotalEnemyUnits == 0 || (genome.normalCount + genome.tankCount + (genome.fastGroupCount * 3) - 1) >= this.maxTotalEnemyUnits -1 || (genome.normalCount + genome.tankCount + (genome.fastGroupCount * 3) - 1) >=0 )) { 
                genome.tankCount--; 
            }
            else if (genome.normalCount > 0) { 
                genome.normalCount--; 
            }
            else { 
                break; 
            }
        }

        while ((genome.normalCount + genome.tankCount + (genome.fastGroupCount * 3)) > this.maxTotalEnemyUnits)
        {
            if (genome.fastGroupCount > 0) genome.fastGroupCount--;
            else if (genome.tankCount > 0) genome.tankCount--;
            else if (genome.normalCount > 0) genome.normalCount--;
            else
            {
                break;
            }
        }
        
        genome.normalCount = Mathf.Max(0, genome.normalCount);
        genome.tankCount = Mathf.Max(0, genome.tankCount);
        genome.fastGroupCount = Mathf.Max(0, genome.fastGroupCount);
    }


    private void Mutate(List<WaveGenome> offspring)
    {
        for (int i = 0; i < offspring.Count; i++)
        {
            if (Random.value < mutationRate)
            {
                WaveGenome genome = offspring[i]; 
                int geneToMutate = Random.Range(0, 3);

                int currentNormals = genome.normalCount;
                int currentTanks = genome.tankCount;
                int currentFastGroups = genome.fastGroupCount;

                switch (geneToMutate)
                {
                    case 0: 
                        int maxN = this.maxTotalEnemyUnits - currentTanks - (currentFastGroups * 3);
                        genome.normalCount = Random.Range(0, Mathf.Max(0, maxN) + 1);
                        break;
                    case 1: 
                        int maxT = this.maxTotalEnemyUnits - currentNormals - (currentFastGroups * 3);
                        genome.tankCount = Random.Range(0, Mathf.Max(0, maxT) + 1);
                        break;
                    case 2: 
                        int maxFastUnits = this.maxTotalEnemyUnits - currentNormals - currentTanks;
                        genome.fastGroupCount = (maxFastUnits >= 3) ? Random.Range(0, Mathf.Max(0, maxFastUnits / 3) + 1) : 0;
                        break;
                }

                ValidateGenomeCounts(ref genome); 
                offspring[i] = genome; 
            }
        }
    }
}
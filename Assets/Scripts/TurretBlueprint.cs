using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurretBlueprint{
    public GameObject prefab;
    public string Type;
    public int cost;

    public GameObject upgradedPrefab;
    public int upgradeCost;

    public int getSellCost { get { return cost / 2; } }
}

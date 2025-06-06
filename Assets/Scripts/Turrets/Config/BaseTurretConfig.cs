using UnityEngine;

public abstract class BaseTurretConfig : ScriptableObject
{
    [field: SerializeField] public TurretType TurretType { get; private set; }
    [field: SerializeField] public string TurretName { get; private set; }
    [field: SerializeField] public GameObject TurretPrefab { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
    [field: SerializeField] public float Range { get; private set; }
    [field: SerializeField] public float TurnSpeed { get; private set; }
    
    [field: SerializeField] public bool IsUpgradable  { get; private set; }
    [field: SerializeField] public int UpgradeCost { get; private set; }
    [field: SerializeField] public GameObject UpgradedPrefab { get; private set; }

    public int SellCost => Cost / 2;
}

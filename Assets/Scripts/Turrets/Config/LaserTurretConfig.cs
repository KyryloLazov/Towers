using UnityEngine;

[CreateAssetMenu(fileName = "LaserTurretConfig", menuName = "Configs/Turrets/LaserTurretConfig")]
public class LaserTurretConfig : BaseTurretConfig
{
    [field: SerializeField] public int damageOverTime { get; private set; } = 30;
    [field: SerializeField] public float slowFactor { get; private set; } = 0.5f;
}

using UnityEngine;

[CreateAssetMenu(fileName = "RocketTurretConfig", menuName = "Configs/Turrets/RocketTurretConfig")]
public class RocketTurretConfig : BaseTurretConfig
{ 
    [field: SerializeField] public float fireRate = 1f;
    [field: SerializeField] public int Damage = 25;
    [field: SerializeField] public int ApproxTargetsInExplosion { get; private set; } = 3;
}

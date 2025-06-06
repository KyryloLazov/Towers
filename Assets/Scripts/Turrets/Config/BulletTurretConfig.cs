using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BulletTurretConfig", menuName = "Configs/Turrets/BulletTurretConfig")]
public class BulletTurretConfig : BaseTurretConfig
{
    [field: SerializeField] public float FireRate = 1f;
    [field: SerializeField] public int Damage = 50;
}

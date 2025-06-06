using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "EnemyConfig",menuName = "Configs/Enemy/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [field: SerializeField] public EnemyType Type { get; private set; }
    [field: SerializeField] public float StartSpeed { get; private set; } = 10f;
    [field: SerializeField] public float StartHealth { get; private set; } = 100;
    [field: SerializeField] public int MoneyAdd { get; private set; } = 50;
}

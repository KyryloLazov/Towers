using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] public EnemyConfig Config { get; private set; }
    public float Speed { get; private set; }
    
    private float _startHealth = 100;
    private float _health;

    public GameObject deathEffect;
    public Image healthBar;

    private bool isDead = false;

    private void Start()
    {
        Speed = Config.StartSpeed;
        _startHealth = Config.StartHealth * GameManager.difficultyRate;
        _health = _startHealth;
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;

        healthBar.fillAmount = _health / _startHealth;

        if (_health <= 0 && !isDead)
        {
            Death();
        }
    }

    public void Slow(float amount)
    {
        Speed = Config.StartSpeed * (1f - amount);
    }

    void Death()
    {
        isDead = true;
        
        PlayerStats.Stats.Add(Config.MoneyAdd);

        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);       
        Destroy(effect, 5f);

        WaveSpawner.enemiesAlive--;

        Destroy(gameObject);
    }

    public void SetSpeed(float speed)
    {
        Speed = speed;
    }
}

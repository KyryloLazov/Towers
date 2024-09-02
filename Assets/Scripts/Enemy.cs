using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float startSpeed = 10f;
    [HideInInspector]
    public float speed;
    
    public float startHealth = 100;
    public float health;
    public int moneyAdd = 50;

    public GameObject deathEffect;

    public Image healthBar;

    private bool isDead = false;

    private void Start()
    {
        speed = startSpeed;
        startHealth *= GameManager.difficultyRate;
        health = startHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        healthBar.fillAmount = health / startHealth;

        if (health <= 0 && !isDead)
        {
            Death();
        }
    }

    public void Slow(float amount)
    {
        speed = startSpeed * (1f - amount);
    }

    void Death()
    {
        isDead = true;
        
        PlayerStats.Stats.Add(moneyAdd);

        GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);       
        Destroy(effect, 5f);

        WaveSpawner.enemiesAlive--;

        Destroy(gameObject);
    }
}

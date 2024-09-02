using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    private Enemy enemy;
    private Transform target;
    private int waypointIndex = 0;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        
        target = Waypoints.points[0];
    }

    void Update()
    {
        Vector3 Move = target.position - transform.position;
        transform.Translate(Move.normalized * enemy.speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) < 0.4f)
        {
            ChangeWaypoint();
        }

        enemy.speed = enemy.startSpeed;
    }

    void ChangeWaypoint()
    {
        if (waypointIndex >= Waypoints.points.Length - 1)
        {
            PlayerStats.Stats.Damage();
            WaveSpawner.enemiesAlive--;
            Destroy(gameObject);
            return;
        }

        waypointIndex++;
        target = Waypoints.points[waypointIndex];
    }
}

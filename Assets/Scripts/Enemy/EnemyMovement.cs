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
        Vector3 direction = target.position - transform.position;
        transform.Translate(direction.normalized * (enemy.Speed * Time.deltaTime), Space.World);
        
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(transform.position, target.position) < 0.4f)
        {
            ChangeWaypoint();
        }

        enemy.SetSpeed(enemy.Config.StartSpeed);
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
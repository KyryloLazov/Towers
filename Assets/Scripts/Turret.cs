using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("General")]
    public float range = 10f;

    [Header("Use Bullets (default)")]
    public GameObject Bullet;
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    [Header("Use Laser")]
    public int damageOverTime = 30;
    public float slowRate = 0.5f;

    public bool useLaser = false;
    public Light impactLight;
    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;

    [Header("Unity Setup Fields")]
    public float turnSpeed = 10f;
    public Transform Head;
    public Transform firePoint;

    private GameObject target;
    private Enemy TargetEnemy;
    void Start()
    {
        InvokeRepeating("FindTarget", 0f, 0.5f);
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDis = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach(GameObject enemy in enemies) { 
            float DistanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if(DistanceToEnemy < shortestDis)
            {
                shortestDis = DistanceToEnemy;
                nearestEnemy = enemy;
            }

        }

        if(nearestEnemy != null && shortestDis <= range)
        {
            target = nearestEnemy;
            TargetEnemy = nearestEnemy.GetComponent<Enemy>();
        }
        else
        {
            target = null;
        }
    }
    void Update()
    {
        if (target == null)
        {
            if (useLaser)
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                    impactLight.enabled = false;
                    impactEffect.Stop();
                }
            }
            return;
        }
        LockOnTarget();

        if (useLaser)
        {
            UseLaser();
        }
        else
        {
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1 / fireRate;
            }
            fireCountdown -= Time.deltaTime;
        }
        
    }

    void LockOnTarget()
    {
        Vector3 dir = target.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(Head.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        Head.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }
    void UseLaser()
    {
        TargetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
        TargetEnemy.Slow(slowRate);
        
        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            impactLight.enabled = true;
            impactEffect.Play();
        }
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.transform.position);

        Vector3 dir = firePoint.position - target.transform.position;

        impactEffect.transform.position = target.transform.position + dir.normalized;

        impactEffect.transform.rotation = Quaternion.LookRotation(dir); 
    }
    void Shoot() 
    {
        GameObject bulletGO = Instantiate(Bullet, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Chase(target.transform);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

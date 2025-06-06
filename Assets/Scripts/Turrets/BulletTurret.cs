using UnityEngine;

public class BulletTurret : BaseTurret
{
    [Header("Bullet Turret Specifics")]
    [SerializeField] private GameObject bulletPrefab;

    private BulletTurretConfig _bulletTurretConfig;
    private float fireCountdown = 0f;

    protected override void Start()
    {
        base.Start();
        
        if(Config is BulletTurretConfig bulletConfig)
            _bulletTurretConfig = bulletConfig;
    }

    protected override void PerformAttack()
    {
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / _bulletTurretConfig.FireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || currentTarget == null) return;

        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bulletGO.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Chase(currentTarget.transform, _bulletTurretConfig.Damage);
        }
    }
}
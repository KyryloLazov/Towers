using UnityEngine;

public class RocketTurret : BaseTurret
{
    [Header("Rocket Turret Specifics")]
    [SerializeField] private GameObject rocketPrefab;
    private RocketTurretConfig _rocketTurretConfig;
    private float fireCountdown = 0f;

    protected override void Start()
    {
        base.Start();

        if (Config is RocketTurretConfig rocketConfig)
            _rocketTurretConfig = rocketConfig;
    }

    protected override void PerformAttack()
    {
        if (fireCountdown <= 0f)
        {
            ShootRocket();
            if(Config is RocketTurretConfig rocketConfig)
                fireCountdown = 1f / rocketConfig.fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    void ShootRocket()
    {
        if (rocketPrefab == null || firePoint == null || currentTarget == null) return;

        GameObject rocketGO = Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);
        Rocket rocketScript = rocketGO.GetComponent<Rocket>();
        if (rocketScript != null)
        {
            rocketScript.Chase(currentTarget.transform, _rocketTurretConfig.Damage);
        }
    }
}
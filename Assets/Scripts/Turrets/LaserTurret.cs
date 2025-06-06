using UnityEngine;

public class LaserTurret : BaseTurret
{
    [Header("Laser Visuals")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private Light impactLight;

    protected override void Start()
    {
        base.Start();

        if (lineRenderer != null) lineRenderer.enabled = false;
        if (impactLight != null) impactLight.enabled = false;
        if (impactEffect != null && impactEffect.isPlaying) impactEffect.Stop();
    }

    protected override void PerformAttack()
    {
        if (currentTargetEnemy == null)
        {
            HandleNoTarget();
            return;
        }

        if (Config is LaserTurretConfig laserConfig)
        {
            currentTargetEnemy.TakeDamage(laserConfig.damageOverTime * Time.deltaTime);
            currentTargetEnemy.Slow(laserConfig.slowFactor);
        }
        
        

        EnableLaserEffects();

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, currentTarget.transform.position);
        }

        if (impactEffect != null)
        {
            Vector3 directionToTarget = currentTarget.transform.position - firePoint.position;
            impactEffect.transform.position = currentTarget.transform.position - directionToTarget.normalized * 0.1f; // Невеликий зсув
            impactEffect.transform.rotation = Quaternion.LookRotation(directionToTarget);
        }
    }

    protected override void HandleNoTarget()
    {
        base.HandleNoTarget();
        DisableLaserEffects();
    }

    void EnableLaserEffects()
    {
        if (lineRenderer != null && !lineRenderer.enabled) lineRenderer.enabled = true;
        if (impactLight != null && !impactLight.enabled) impactLight.enabled = true;
        if (impactEffect != null && !impactEffect.isPlaying) impactEffect.Play();
    }

    void DisableLaserEffects()
    {
        if (lineRenderer != null && lineRenderer.enabled) lineRenderer.enabled = false;
        if (impactLight != null && impactLight.enabled) impactLight.enabled = false;
        if (impactEffect != null && impactEffect.isPlaying) impactEffect.Stop();
    }
}

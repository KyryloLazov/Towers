using UnityEngine;

public class Rocket : BaseProjectile
{
    [Header("Rocket Specifics")]
    [SerializeField] private float explosionRadius = 5f;

    protected override void ApplyDamageEffect()
    {
        if (explosionRadius > 0f)
        {
            Explode();
        }
        else
        {
            if (currentTarget != null)
            {
                DamageEnemy(currentTarget, damage);
            }
        }
    }

    void Explode()
    {
        Collider[] collidersHit = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hitCollider in collidersHit)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                DamageEnemy(hitCollider.transform, damage);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
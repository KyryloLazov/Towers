using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    [Header("Base Projectile Settings")]
    [SerializeField] protected float speed = 70f;
    [SerializeField] protected GameObject impactEffectPrefab;
    
    protected int damage = 50;
    protected Transform currentTarget;
    
    public virtual void Chase(Transform _target, int bulletDamage)
    {
        currentTarget = _target;
        damage = bulletDamage;
    }
    
    protected virtual void Update()
    {
        if (currentTarget == null)
        {
            Destroy(gameObject);
            return;
        }

        MoveTowardsTarget();
    }

    protected virtual void MoveTowardsTarget()
    {
        Vector3 direction = currentTarget.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;
        
        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }
        
        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        transform.LookAt(currentTarget);
    }
    
    protected virtual void HitTarget()
    {
        if (impactEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(impactEffectPrefab, transform.position, transform.rotation);
            Destroy(effectInstance, 4f);
        }
        
        ApplyDamageEffect();

        Destroy(gameObject);
    }
    
    protected abstract void ApplyDamageEffect();
    
    protected void DamageEnemy(Transform enemyTransform, int damageAmount)
    {
        if (enemyTransform == null) return;

        Enemy enemyComponent = enemyTransform.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.TakeDamage(damageAmount);
        }
    }
}

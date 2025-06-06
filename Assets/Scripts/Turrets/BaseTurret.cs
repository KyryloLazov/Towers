using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public abstract class BaseTurret : MonoBehaviour
{
    [field: SerializeField] public BaseTurretConfig Config { get; private set; }

    [Header("Unity Setup Fields")]
    [SerializeField] protected Transform headToRotate;
    [SerializeField] protected Transform firePoint;

    protected GameObject currentTarget;
    protected Enemy currentTargetEnemy;
    protected virtual void Start()
    {
        InvokeRepeating("FindTarget", 0f, 0.5f);
    }
    
    protected virtual void Update()
    {
        if (currentTarget == null)
        {
            HandleNoTarget();
            return;
        }

        LockOnTarget();
        PerformAttack();
    }

    protected void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemyGO in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemyGO.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemyGO;
            }
        }

        if (nearestEnemy != null && shortestDistance <= Config.Range)
        {
            currentTarget = nearestEnemy;
            currentTargetEnemy = currentTarget.GetComponent<Enemy>();
        }
        else
        {
            currentTarget = null;
            currentTargetEnemy = null;
        }
    }

    protected void LockOnTarget()
    {
        if (headToRotate == null) return;

        Vector3 direction = currentTarget.transform.position - headToRotate.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        Vector3 rotation = Quaternion.Lerp(headToRotate.rotation, lookRotation, Time.deltaTime * Config.TurnSpeed).eulerAngles;
        headToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }
    
    protected abstract void PerformAttack();
    
    protected virtual void HandleNoTarget()
    {

    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Config.Range);
    }
}

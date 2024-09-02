using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    public float speed;
    public int damage = 50;
    public float explosionRadius = 0f;
    public GameObject imapctEffect;

    public void Chase(Transform _targer)
    {
        target = _targer;
    }
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        
        Vector3 dir = target.position - transform.position;
        float distancePerFrame = speed * Time.deltaTime;
        if(dir.magnitude <= distancePerFrame )
        {
            HitTarget();
            return;
        }
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        transform.LookAt(target);
    }

    void HitTarget()
    {
        GameObject effect = Instantiate(imapctEffect, transform.position, transform.rotation);
        Destroy(effect, 4f);

        if(explosionRadius > 0f)
        {
            Explode();
        }
        else
        {
            Damage(target);
        }

        
        Destroy(gameObject);
    }

    void Damage(Transform enemy)
    {
        Enemy e = enemy.GetComponent<Enemy>();

        if (e != null)
        {
            e.TakeDamage(damage);
        }
        
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider coll in colliders)
        {
            if(coll.tag == "Enemy")
            {
                Damage(coll.transform);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

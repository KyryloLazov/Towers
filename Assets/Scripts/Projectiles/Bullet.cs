public class Bullet : BaseProjectile
{
    protected override void ApplyDamageEffect()
    {
        if (currentTarget != null)
        {
            DamageEnemy(currentTarget, damage);
        }
    }
}
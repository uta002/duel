using UnityEngine;

public class ProjectileBaseNoDestroy_Mono : ProjectileBase_Mono
{
    [SerializeField] protected OnceDetectOverlapSphere sphereCheck;
    [SerializeField] float radius;
    [SerializeField] Damager damager;

    public override void Init(Dueler_Mono owner, int timestamp, int projectileID, Vector3 origin, Vector3 direction, float lifeTime, float shotSpeed, IProjectileOnHit[] onHitEffect)
    {
        base.Init(owner, timestamp, projectileID, origin, direction, lifeTime, shotSpeed, onHitEffect);
        sphereCheck.Init(OnHitAction, radius);
    }

    protected override void Update()
    {
        transform.position = Vector3.Lerp(origin, targetPos, NormalizedTime);
        sphereCheck.Check(transform.position);
        if (timestamp.IsOverLifeTime)
        {
            Destroy();
        }
    }

    void OnHitAction(Vector3 pos, Collider col)
    {
        if(col.TryGetComponent(out IDamageable damageable))
        {
            if(damageable.TeamID != owner.TeamID)
            {
                damager.DealDamage(owner, col.transform.position - pos, damageable);
            }
        }
    }

    protected override void OnHit(RaycastHit hit)
    {
        foreach (var e in onHitEffect)
        {
            e.ProjectileOnHit(owner, this, hit);
        }
    }
}

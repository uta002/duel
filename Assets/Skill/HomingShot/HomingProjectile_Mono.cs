using UnityEngine;

public class HomingProjectile_Mono : ProjectileBase_Mono
{
    ITargetable target;
    [SerializeField] Rigidbody rb;
    [SerializeField] LookTarget lookTarget;
    [SerializeField] TargetPosType homingType;
    float shotSpeed;


    public void Init(Dueler_Mono owner, int timestamp, ITargetable target, int projectileID, Vector3 origin, Vector3 direction, float shotSpeed, float homing, float angularDrag, float lifeTime, IProjectileOnHit[] onHitEffect)
    {
        this.target = target;
        rb.angularDrag = angularDrag;
        this.shotSpeed = shotSpeed;
        lookTarget.RotateIntensity = homing;

        base.Init(owner, timestamp, projectileID, origin, direction, lifeTime, shotSpeed, onHitEffect);
    }

    protected override void Update()
    {
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            lookTarget.LookTargetPos(rb, transform.position, transform.rotation, AimManager_Mono.GetTargetPos(transform.position, target, homingType, shotSpeed));
        }
        rb.velocity = transform.forward * shotSpeed;

        hitCheck.Update(transform.position, OnHit);
        if (timestamp.NormalizedTime >= 1f)
        {
            Destroy();
        }
    }
}

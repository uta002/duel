using Photon.Pun;
using UnityEngine;

public class HomingBullet_Mono : MonoBehaviour
{
    Vector3 origin;
    ITargetable target;
    [SerializeField] Rigidbody rb;
    [SerializeField] DamageDealer_Mono damageDealer;
    [SerializeField] LookTarget_Mono lookTarget;
    [SerializeField] BulletDestroyer_Mono bulletDestroyer;
    [SerializeField] float impactForce = 100f;
    [SerializeField] TargetPosManager targetPos;
    float shotSpeed;

    public void Init(int timestamp, int sourceTeamId, int sourceID, ITargetable target, int projectileID, Vector3 origin, Vector3 direction, float shotSpeed, float homing, float angularDrag, float lifeTime)
    {
        this.target = target;
        rb.angularDrag = angularDrag;
        lookTarget.RotateIntensity = homing;
        this.shotSpeed = shotSpeed;
        this.origin = origin;
        transform.position = origin;

        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            lookTarget.LookTargetPos(rb, targetPos.GetTargetPos(target, shotSpeed));
        }
        rb.velocity = transform.forward * shotSpeed;
    }

    public void OnHitSomething(RaycastHit hitInfo)
    {
        damageDealer.DealDamage(hitInfo, transform.forward * impactForce);
        bulletDestroyer.OnHitSomething(hitInfo);
    }
}

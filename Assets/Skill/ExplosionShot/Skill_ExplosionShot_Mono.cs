using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Skill_ExplosionShot_Mono : Skill_PosDir_Mono, IProjectileOnHit
{
    [SerializeField] float needAirTime;
    [SerializeField] float radius = 1f;
    [SerializeField] AreaDamage_Mono endEmit;

    [SerializeField] AimShotRigidityAction shotAction = new AimShotRigidityAction(
        new Shot(
            new ProjectileDischarger(), 
            new PosDegNoChange()));

    public override ILinePredictionVariable LinePredictionVariable => shotAction;

    public override bool CanUse => base.CanUse && !owner.IsGround && needAirTime <= owner.TimeElapsedFromLastGround;

    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        base.Init(owner, so, skillSlotID, input);
        shotAction.Init(owner, this);
    }

    protected override void AfterRPCAction(Vector3 pos, float x, float y, PhotonMessageInfo info)
    {
        shotAction.StartShot(pos, x, y, info);
    }

    public void ProjectileOnHit(Dueler_Mono owner, ProjectileBase_Mono projectile, RaycastHit hit)
    {
        var e = Instantiate(endEmit, hit.point, Quaternion.LookRotation(Vector3.up));
        e.Init(owner, radius);
    }
}

[System.Serializable]
public class LineMovement
{
    [SerializeField] float lifeTime = 1f;
    [SerializeField] float speed = 10f;
    protected Vector3 origin;
    protected Vector3 targetPos;
    protected TimestampLifeTime timestamp;

    public void Init(int timestamp, Vector3 origin, Vector3 direction)
    {
        this.timestamp = new TimestampLifeTime(timestamp, lifeTime);
        targetPos = origin + lifeTime * speed * direction.normalized;
    }

    public Vector3 UpdatePosition()
    {
        return Vector3.Lerp(origin, targetPos, timestamp.NormalizedTime);
    }
}

[Serializable]
public class OnceDetectOverlapSphere : DetectOverlapSphere
{
    List<Collider> damagedList;

    public override void Init(Action<Vector3, Collider> onHitAction, float radius)
    {
        base.Init(onHitAction, radius);
        damagedList = new List<Collider>();
    }

    public override void OnHit(Vector3 pos, Collider target)
    {
        if (!damagedList.Contains(target))
        {
            base.OnHit(pos, target);
            damagedList.Add(target);
        }
        
    }

}

public interface IOnHitProjectile
{
    void OnHitEffect(ProjectileBase_Mono projectile, RaycastHit hitInfo);
}


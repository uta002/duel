using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Skill_BulletShot_Mono : Skill_PosDir_Mono
{
    [SerializeField] AimShotRigidityAction shot = new AimShotRigidityAction(new Shot(new ProjectileDischarger(), new PosDegNoChange()));
    [SerializeField] Damager bulletDamager;

    public override ILinePredictionVariable LinePredictionVariable => shot;

    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        base.Init(owner, so, skillSlotID, input);
        shot.Init(owner, bulletDamager);
    }
    protected override void AfterRPCAction(Vector3 pos, float x, float y, PhotonMessageInfo info)
    {
        shot.StartShot(pos, x, y, info);
    }
}

[System.Serializable]
public class TargetShot : ShotBase, ITargetShot
{
    [SerializeReference] public ITargetDischarger targetDischarger;
    [SerializeReference] protected IPosDegModifier posDegModifier;
    public override IDischarger Discharger => targetDischarger;
    public ITargetDischarger TargetDischarger => targetDischarger;
    protected override IPosDegModifier PosDegModifier => posDegModifier;

    public TargetShot(ITargetDischarger targetDischarger, IPosDegModifier posDegModifier)
    {
        this.targetDischarger = targetDischarger;
        this.posDegModifier = posDegModifier;
    }

    public void SetTarget(ITargetable target)
    {
        targetDischarger.SetTarget(target);
    }
}

[System.Serializable]
public class Shot : ShotBase
{
    [SerializeReference] protected IDischarger discharger;
    [SerializeReference] protected IPosDegModifier posDegModifier;

    public Shot(IDischarger discharger, IPosDegModifier posDegModifier)
    {
        this.discharger = discharger;
        this.posDegModifier = posDegModifier;
    }

    public override IDischarger Discharger => discharger;
    protected override IPosDegModifier PosDegModifier => posDegModifier;
}

public abstract class ShotBase : IShot
{
    Dueler_Mono owner;
    public abstract IDischarger Discharger { get; }
    protected abstract IPosDegModifier PosDegModifier { get; }
    public int ShotNum => PosDegModifier.ShotNum;

    public void Init(Dueler_Mono owner, IProjectileOnHit onHitEffect) => Init(owner, new IProjectileOnHit[] { onHitEffect });

    public void Init(Dueler_Mono owner, IProjectileOnHit[] onHitEffect)
    {
        this.owner = owner;
        Discharger.Init(owner, onHitEffect);
        PosDegModifier.Init(owner, DischargerShot);
    }

    public void Shot(int timestamp, int syncId, Vector3 origin, float x, float y)
    {
        PosDegModifier.Shot(timestamp, syncId, origin, x, y);
    }

    void DischargerShot(int timestamp, int syncId, Vector3 origin, Vector3 degree)
    {
        Discharger.Shot(timestamp, syncId, origin, PhotonUtil3D.Degree2Direction(degree));
    }
}

public interface ITargetShot : IShot, ITargetAction
{
    ITargetDischarger TargetDischarger { get; }
}

public interface IShot
{
    int ShotNum { get; }
    IDischarger Discharger { get; }
    void Init(Dueler_Mono owner, IProjectileOnHit onHitEffect);
    void Init(Dueler_Mono owner, IProjectileOnHit[] onHitEffect);
    void Shot(int timestamp, int syncId, Vector3 origin, float x, float y);
}

public interface IProjectileOnHit
{
    void ProjectileOnHit(Dueler_Mono owner, ProjectileBase_Mono projectile, RaycastHit hit);
}

[System.Serializable]
public class PosDegNoChange : IPosDegModifier
{
    Dueler_Mono owner;
    System.Action<int, int, Vector3, Vector3> action;
    public int ShotNum => 1;

    public void Init(Dueler_Mono owner, System.Action<int, int, Vector3, Vector3> action)
    {
        this.owner = owner;
        this.action = action;
    }


    public void Shot(int timestamp, int syncId, Vector3 origin, float x, float y)
    {
        action?.Invoke(timestamp, syncId, origin, new Vector3(x, y));
    }
}

[System.Serializable]
public class PosNoChange : IPosition
{
    public Vector3 GetPosition(Dueler_Mono owner, Vector3 shotOrigin, Quaternion rot, int num, int total) => shotOrigin;
}

[System.Serializable]
public class DegNoChange : IDegree
{
    public Vector3 GetDegree(Dueler_Mono owner, float x, float y, int num, int total) => new Vector3(x, y);
}


public abstract class DischargerBase<T> : IDischarger where T : UnityEngine.Object
{
    [SerializeField] protected T projectile;
    [SerializeField] protected float lifeTime;
    [SerializeField] protected float shotSpeed;
    [SerializeField] protected int delay = 50;

    protected Dueler_Mono owner;
    protected IProjectileOnHit[] onHitEffect;

    public float ShotSpeed => shotSpeed;
    public int DelayInMilliSec => delay;

    System.Action<T> shotcallback;

    public void Init(Dueler_Mono owner, IProjectileOnHit[] onHitEffect)
    {
        this.owner = owner;
        this.onHitEffect = onHitEffect;
    }

    public void AddShotCallbacks(System.Action<T> callback)
    {
        shotcallback = callback;
    }

    public void Shot(int timestamp, int syncId, Vector3 origin, Vector3 direction)
    {
        T p = GameObject.Instantiate(projectile);
        shotcallback?.Invoke(p);
        ShotInit(p, timestamp, syncId, origin, direction);
    }

    public abstract void ShotInit(T projectile, int timestamp, int syncId, Vector3 origin, Vector3 direction);


}

[System.Serializable]
public class ProjectileDischarger : DischargerBase<ProjectileBase_Mono>
{
    public override void ShotInit(ProjectileBase_Mono projectile, int timestamp, int syncId, Vector3 origin, Vector3 direction)
    {
        projectile.Init(owner, timestamp, syncId, origin, direction, lifeTime, shotSpeed, onHitEffect);
    }
}

[System.Serializable]
public abstract class DamagerBase
{
    [SerializeField] protected DamageType damageType;
    [SerializeField] protected ImpactType impactType;
    [SerializeField] protected float impactForce;
    //protected abstract float Damage { get; }


}


public static class DamageUtil
{
    public static void ApplyDamage(RaycastHit hitInfo, IDamageDealer damageDealer)
    {
        if(hitInfo.collider.TryGetComponent(out IDamageable damageable))
        {
            damageDealer.DealDamage(damageable);
        }
    }
}

public interface IDamageDealer
{
    void DealDamage(IDamageable damageable);
}


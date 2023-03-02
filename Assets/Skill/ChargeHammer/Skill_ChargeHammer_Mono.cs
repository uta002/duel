using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class Skill_ChargeHammer_Mono : Skill_Base_Mono, IReadySheathWeapon
{
    [SerializeField] RPC_Activate_Mono rpc_chargeStart;
    [SerializeField] RPC_PosAngleY_Mono rpc_attack;


    [SerializeField] GameObject weaponPrefab;

    [SerializeField] DuelerState_ChargingHammer state_Charging;
    [SerializeField] DuelerState_HammerAttack state_Attack;
    [SerializeField] DuelerStateRigidity state_Rigidity;

    [SerializeField] ChargeDamager damager;

    GameObject weapon;
    int chargeStartTimestamp;
    float chargedTime;
    public override bool CanUse => base.CanUse && owner.IsGround;
    bool IsCharging => owner.CurrentState == state_Charging;

    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        base.Init(owner, so, skillSlotID, input);

        rpc_chargeStart.Init(ChargeStart);
        rpc_attack.Init(owner, Attack);


        state_Charging.Init(this);
        state_Attack.Init(d => d.ChangeState(state_Rigidity), this, OnHit);
        state_Rigidity.Init(ReadyWeapon, SheathWeapon);

        InitWeapon();
        SheathWeapon();
    }

    void InitWeapon()
    {
        weapon = Instantiate(weaponPrefab, owner.HandPos_L);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
    }

    protected override void Activated()
    {

    }

    public override void ButtonDown()
    {
        if (CanUse)
        {
            rpc_chargeStart.ActivateRPC();
        }
    }

    public override void ButtonUp()
    {
        if (IsCharging)
        {
            rpc_attack.ActivateRPC();
            cooldown.Use();
        }

    }

    void ChargeStart(PhotonMessageInfo info)
    {
        chargeStartTimestamp = info.SentServerTimestamp;
        owner.ChangeState(state_Charging);
    }

    void Attack(Vector3 pos, float angleY, PhotonMessageInfo info)
    {
        if(IsCharging)
        {
            chargedTime = PhotonUtil3D.ElapsedTime(chargeStartTimestamp, info.SentServerTimestamp);
            owner.ChangeState(state_Attack);
        }
        else
        {
            chargedTime = 0f;
        }
    }

    void OnHit(Vector3 pos, Collider col)
    {
        if (col.TryGetComponent(out IDamageable dmaagable))
        {
            var direction = owner.Root.forward;
            damager.DealChargedDamage(owner, direction, dmaagable, chargedTime);
        }
    }

    public void ReadyWeapon()
    {
        weapon.gameObject.SetActive(true);
    }

    public void SheathWeapon()
    {
        weapon.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class ChargeDamager : Damager
{
    [SerializeField] float maxDamage;
    [SerializeField] float maxChargeTime;
    [SerializeField] float impactDirectionAdd_Up;

    [SerializeField] float maxImpactForce;

    public void DealChargedDamage(Dueler_Mono owner, Vector3 direction, IDamageable damageable, float chargedTime)
    {
        if (CanDamage(owner, damageable))
        {
            var chargeNormalized = Mathf.Clamp01(chargedTime / maxChargeTime);
            var calced_Damage = this.damage + (maxDamage - this.damage) * chargeNormalized;
            float calced_impactForce = impactForce + (maxImpactForce - impactForce) * chargeNormalized;
            damageable.TakeDamage(new DamageInfo(owner.TeamID, owner.ID, damageType, impactType, (direction.normalized + Vector3.up * impactDirectionAdd_Up).normalized * calced_impactForce, calced_Damage));
        }
    }
}

[System.Serializable]
public class DuelerState_ChargingHammer : DuelerStateDefault
{
    const int targetLayer = 1;
    [SerializeField] float noSelfDamageLimitTime;
    [SerializeField] Cooldown selfDamageCooldown;
    [SerializeField] DamagerSelfInclude timeOverSelfDamager;

    Timestamp timestamp = new Timestamp();
    public override bool CanUseSkill => false;
    IReadySheathWeapon weapon;

    public void Init(IReadySheathWeapon weapon)
    {
        this.weapon = weapon;
    }



    public override void OnEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        base.SetPhysics(owner);
        owner.Animator.SetLayerWeight(targetLayer, 1f);
        owner.Animator.CrossFade("ChargeHammer", 0.25f, targetLayer);
        weapon.ReadyWeapon();
        timestamp.Init();
        selfDamageCooldown.ForceReady();

    }

    public override void Update(Dueler_Mono owner)
    {
        base.Update(owner);
        if(noSelfDamageLimitTime < timestamp.TimeElapsed)
        {
            if (selfDamageCooldown.CanUse)
            {
                timeOverSelfDamager.DealDamage(owner, Vector3.zero, owner);
                selfDamageCooldown.Use();
            }
        }
    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        owner.Animator.SetLayerWeight(targetLayer, 0f);
        base.OnExit(owner, nextState);
        weapon.SheathWeapon();
    }
}

[System.Serializable]
public class DuelerState_HammerAttack : DuelerStateEndAction
{
    [SerializeField] float checkStartTime;
    [SerializeField] float checkEndTime;
    [SerializeField] Vector3 checkOffset;
    [SerializeField] OnceDetectOverlapBox damageCheck;

    IReadySheathWeapon weapon;

    public DuelerState_HammerAttack(float duration, Action<Dueler_Mono> endAction) : base(duration, endAction)
    {
    }

    public void Init(Action<Dueler_Mono> endAction,IReadySheathWeapon weapon, System.Action<Vector3, Collider> OnHit)
    {
        Init(endAction);
        this.weapon = weapon;
        damageCheck.Init(OnHit);
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.Animator.CrossFade("HammerAttack", 0.25f);
        weapon.ReadyWeapon();
        damageCheck.ClearDamagedList();
        owner.Rb.velocity = Vector3.zero;
    }

    protected override void StateUpdate(Dueler_Mono owner)
    {
        if (timestamp.CheckTimeElapsedBetween(checkStartTime, checkEndTime))
        {
            damageCheck.Check(owner.HeartPos + owner.Root.rotation * checkOffset, owner.Root.rotation);
        }
    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        weapon.SheathWeapon();
    }
}


[System.Serializable]
public class DetectOverlapBox
{
    [SerializeField] LayerMask checkMask;
    [SerializeField] Vector3 halfExtents;

    System.Action<Vector3, Collider> OnHitAction;

    public virtual void Init(System.Action<Vector3, Collider> onHitAction) => OnHitAction = onHitAction;

    public virtual void Init(System.Action<Vector3, Collider> onHitAction, Vector3 halfExtents)
    {
        Init(onHitAction);
        this.halfExtents = halfExtents;
    }

    public void Check(Vector3 pos, Quaternion rot)
    {
        var colliders = Physics.OverlapBox(pos, halfExtents, rot, checkMask);

        for (int i = 0; i < colliders.Length; ++i)
        {
            OnHit(pos, colliders[i]);
        }
    }

    public virtual void OnHit(Vector3 pos, Collider target)
    {
        OnHitAction?.Invoke(pos, target);
    }
}

[Serializable]
public class OnceDetectOverlapBox : DetectOverlapBox
{
    List<Collider> damagedList;

    public override void Init(Action<Vector3, Collider> onHitAction)
    {
        base.Init(onHitAction);
        damagedList = new List<Collider>();
    }

    public void ClearDamagedList()
    {
        damagedList.Clear();
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
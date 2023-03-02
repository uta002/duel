using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Skill_Blade_Mono : Skill_PosAimPos_Mono, IMeleeSkill, ILinePredictionVariable
{
    [SerializeField] DuelerStatePreBlade preBladeState;
    [SerializeField] DuelerStateBlade bladeState;
    [SerializeField] DuelerStateRigidity rigidityState;

    [SerializeField] MeleeWeapon_Mono weaponPrefab;

    [HideInInspector] public MeleeWeaponBase_Mono weapon;
    public MeleeWeaponBase_Mono MeleeWeapon => weapon;

    Vector3 direction;
    public override bool CanUse => base.CanUse && owner.IsGround && !owner.HasSnare;

    public override ILinePredictionVariable LinePredictionVariable => this;
    public float ProjectileSpeed => preBladeState.MoveSpeed;
    public float DelayInSec => 0f;
    public Vector3 Origin => owner.HeartPos;
    [SerializeField] bool parryable;
    [SerializeField] ParryableDamager damager;
    public void OnHitBlade(RaycastHit hitInfo)
    {
        if (parryable)
        {
            damager.ParryCheckDamage(owner, direction, hitInfo);
        }
        else
        {
            damager.DealDamage(owner, direction, hitInfo);
        }
    }

    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        base.Init(owner, so, skillSlotID, input);

        preBladeState.Init(OnEndPreBlade, this);
        bladeState.Init(OnHitBlade, OnEndBlade, this);
        rigidityState.Init(ReadyWeapon, SheathWeapon);

        InitWeapon();
        SheathWeapon();
    }

    void InitWeapon()
    {
        weapon = Instantiate(weaponPrefab, owner.HandPos_R);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
    }

    void OnRemove()
    {
        if(weapon != null)
        {
            Destroy(weapon?.gameObject);
        }
    }


    protected override void AfterRPCAction(Vector3 pos, Vector3 aimPos, PhotonMessageInfo info)
    {
        direction = aimPos - pos;
        preBladeState.BeforeEnterInit(aimPos);
        owner.ChangeState(preBladeState);
        SendNotify(info.SentServerTimestamp, pos);
    }

    void SendNotify(int timestamp, Vector3 pos)
    {
        //MySubject<float>.Notify(2f);
        MySubject<Blade_Notification>.Notify(
            new Blade_Notification(
                    SkillSO.id,
                    timestamp,
                    owner,
                    owner.TargetSystem.CurrentTarget,
                    preBladeState,
                    damager.Damage,
                    pos,
                    direction,
                    parryable
            )
        );
        //MySubject<float>.Notify(2f);
    }

    void OnEndPreBlade(Dueler_Mono owner)
    {
        owner.ChangeState(bladeState);
    }

    void OnEndBlade(Dueler_Mono owner)
    {
        owner.ChangeState(rigidityState);
    }

    public void ReadyWeapon()
    {
        weapon.gameObject.SetActive(true);
    }

    public void SheathWeapon()
    {
        weapon.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        OnRemove();
    }
}

[System.Serializable]
public class DuelerStatePreBlade : DuelerStateEndAction
{
    IMeleeSkill meleeSkill;
    Vector3 aimPos;

    Vector3 direction;
    [SerializeField] float attackDistance;
    [SerializeField] float moveSpeed;
    [SerializeField] string animationTrigger = "BladePre";
    public float MoveSpeed => moveSpeed;

    public DuelerStatePreBlade Init(System.Action<Dueler_Mono> endAction, IMeleeSkill meleeSkill)
    {
        this.meleeSkill = meleeSkill;
        base.Init(endAction);
        return this;
    }

    public void BeforeEnterInit(Vector3 aimPos)
    {
        this.aimPos = aimPos;
    }


    public DuelerStatePreBlade(Vector3 aimPos, IMeleeSkill skill_Blade, System.Action<Dueler_Mono> action) : base(0f, action)
    {
        this.aimPos = aimPos;
        this.meleeSkill = skill_Blade;
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        direction = aimPos - owner.transform.position;
        direction.y = 0f;

        owner.SetRootRot(Quaternion.LookRotation(direction).eulerAngles.y);
        owner.Animator.SetTrigger(animationTrigger);
        meleeSkill.ReadyWeapon();
    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        meleeSkill.SheathWeapon();
    }
    Vector3 d;
    protected override void StateUpdate(Dueler_Mono owner)
    {
        d = aimPos - owner.transform.position;
        d.y = 0f;
        if (d.magnitude < attackDistance)
        {
            endAction?.Invoke(owner);
        }
        else
        {
            var target = direction.normalized * moveSpeed;
            target.y = owner.Rb.velocity.y;
            owner.Rb.velocity = target;

            owner.CheckDash();
        }
    }
}

[System.Serializable]
public class DuelerStateBlade : DuelerStateEndAction
{
    IMeleeSkill meleeSkill;
    System.Action<RaycastHit> onHitBlade;

    [SerializeField] float checkStartTime;
    [SerializeField] string animationTrigger = "Blade";

    bool checkStarted;
    public void Init(System.Action<RaycastHit> onHitBlade, System.Action<Dueler_Mono> endAction, IMeleeSkill meleeSkill)
    {
        this.meleeSkill = meleeSkill;

        this.onHitBlade = onHitBlade;
        Init(endAction);
    }

    public DuelerStateBlade(IMeleeSkill meleeSkill, float checkStartTime, float duration, System.Action<Dueler_Mono> action) : base(duration, action)
    {
        this.meleeSkill = meleeSkill;
        this.checkStartTime = checkStartTime;
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.Rb.velocity = Vector3.zero;
        owner.Rb.isKinematic = true;

        meleeSkill.ReadyWeapon();
        owner.Animator.SetTrigger(animationTrigger);
        checkStarted = false;
    }

    protected override void StateUpdate(Dueler_Mono owner)
    {
        if(timestamp.TimeElapsed > checkStartTime)
        {
            if (!checkStarted)
            {
                meleeSkill.MeleeWeapon.CheckStart();
                checkStarted = true;
            }
            meleeSkill.MeleeWeapon.CheckCollisions(onHitBlade);
        }
    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        owner.Rb.isKinematic = false;

        meleeSkill.SheathWeapon();
        meleeSkill.MeleeWeapon.CheckEnd();
        owner.Rb.velocity = Vector3.zero;
    }
}

public interface IReadySheathWeapon
{
    void SheathWeapon();
    void ReadyWeapon();
}

public interface IMeleeSkill : IReadySheathWeapon
{
    MeleeWeaponBase_Mono MeleeWeapon { get; }
}

[System.Serializable]
public class Damager : DamagerBase, IProjectileOnHit
{
    [SerializeField] protected float damage;
    public virtual float Damage => damage;
    

    protected virtual bool CanDamage(Dueler_Mono source, IDamageable target) => source.TeamID != target.TeamID;

    public virtual void DealDamage(Dueler_Mono owner, Vector3 direction, RaycastHit hitInfo)
    {
        if (hitInfo.collider.TryGetComponent(out IDamageable damageable))
        {
            DealDamage(owner, direction, damageable);
        }
    }

    public virtual void DealDamage(Dueler_Mono owner, Vector3 direction, IDamageable damageable)
    {
        if (CanDamage(owner, damageable))
        {
            damageable.TakeDamage(new DamageInfo(owner.TeamID, owner.ID, damageType, impactType, direction.normalized * impactForce, Damage));
        }
    }

    public void ProjectileOnHit(Dueler_Mono owner, ProjectileBase_Mono projectile, RaycastHit hit)
    {
        DealDamage(owner, projectile.transform.forward, hit);
    }
}

[System.Serializable]
public class ParryableDamager : Damager
{
    public void ParryCheckDamage(Dueler_Mono owner, Vector3 direction, RaycastHit hitInfo)
    {
        if (hitInfo.collider.TryGetComponent(out Dueler_Mono dueler))
        {
            if (dueler.TeamID != owner.TeamID)
            {
                if (dueler.CurrentState is DuelerStateParry parryState)
                {
                    if (Vector3.Dot(Quaternion.Euler(0f, parryState.Skill_Parry.ParryDegY, 0f) * Vector3.forward, direction) < 0f)
                    {
                        parryState.Skill_Parry.ParrySuccessRPC(owner, dueler.transform.position);
                        return;
                    }
                }
                
                
                dueler.TakeDamage(new DamageInfo(owner.TeamID, owner.ID, damageType, impactType, direction.normalized * impactForce, Damage));
                return;
                
            }
        }
        else
        {
            DealDamage(owner, direction, hitInfo);
        }
    }
}

[System.Serializable]
public class DamagerSelfInclude : Damager
{
    [SerializeField] bool includeSelf = true;
    protected override bool CanDamage(Dueler_Mono source, IDamageable target)
    {
        if (includeSelf)
        {
            return base.CanDamage(source, target) || (IDamageable)source == target;
        }
        else
        {
            return base.CanDamage(source, target);
        }
    }
}

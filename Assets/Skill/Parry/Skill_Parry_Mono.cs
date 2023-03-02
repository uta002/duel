using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class Skill_Parry_Mono : Skill_PosAngleY_Mono, IReadySheathWeapon
{
    [SerializeField] DuelerStateParry parryState;
    [SerializeField] DuelerStatePenaltyRigidity parryRigidityState;

    [SerializeField] DuelerStateParrySuccess parrySuccessState;
    [SerializeField] DuelerStateRigidity counterAttackRigidityState;

    [SerializeField] GameObject bladePrefab;
    GameObject blade;

    [SerializeField] Damager damager;

    //public override bool CanUse => cooldown.CanUse && owner.IsGround;
    public override bool CanUse => base.CanUse && owner.IsGround;

    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        base.Init(owner, so, skillSlotID, input);
        parryState.Init(this, OnParryEnd);
        parryRigidityState.Init(ReadyWeapon, SheathWeapon);

        parrySuccessState.Init(this, OnEndCounterAttack);
        counterAttackRigidityState.Init(ReadyWeapon, SheathWeapon);

        InitWeapon();
        SheathWeapon();
    }

    void InitWeapon()
    {
        blade = Instantiate(bladePrefab, owner.HandPos_R);
        blade.transform.localPosition = Vector3.zero;
        blade.transform.localRotation = Quaternion.identity;
    }

    float parryDegY;
    public float ParryDegY => parryDegY;
    protected override void AfterRPCAction(Vector3 pos, float degreeY, PhotonMessageInfo info)
    {
        parryDegY = degreeY;
        owner.SetRootRot(degreeY);
        owner.ChangeState(parryState);
    }

    public void OnParryEnd(Dueler_Mono owner)
    {
        owner.ChangeState(parryRigidityState);
    }


    public void ReadyWeapon() => blade?.SetActive(true);
    public void SheathWeapon() => blade?.SetActive(false);


    Dueler_Mono target;
    Vector3 direction;
    public void ParrySuccessRPC(Dueler_Mono target, Vector3 parryManPos)
    {
        photonView.RPC(nameof(ParrySuccess), RpcTarget.All, target.ID, parryManPos);
    }

    [PunRPC]
    void ParrySuccess(int targetID, Vector3 parryManPos)
    {
        Dueler_Mono target = DuelerManager_Mono.GetDueler(targetID);
        if(target!= null)
        {
            this.target = target;
            direction = target.transform.position - parryManPos;
            owner.transform.position = parryManPos;

            owner.SetRootRot(Quaternion.LookRotation(direction).eulerAngles.y);
            owner.ChangeState(parrySuccessState);
        }
    }




    public void OnEndCounterAttack(Dueler_Mono parryMan)
    {
        damager.DealDamage(owner, direction.normalized + Vector3.up * 0.3f, target);
        parryMan.ChangeState(counterAttackRigidityState);
    }

    private void OnDestroy()
    {
        Destroy(blade);
    }
}

[System.Serializable]

public class DuelerStateParry : DuelerStateEndAction
{
    Skill_Parry_Mono skill;
    public Skill_Parry_Mono Skill_Parry => skill;
    public void Init(Skill_Parry_Mono parry, Action<Dueler_Mono> endAction)
    {
        this.skill = parry;
        Init(endAction);
    }

    public DuelerStateParry(Skill_Parry_Mono skill, float duration, System.Action<Dueler_Mono> action) : base(duration, action)
    {
        this.skill = skill;
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.Rb.velocity = Vector3.zero;
        owner.Rb.isKinematic = true;


        skill.ReadyWeapon();
        owner.Animator.SetTrigger("Parry");
    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        skill.SheathWeapon();
    }
}


[System.Serializable]
public class DuelerStateParrySuccess : DuelerStateEndAction
{
    IReadySheathWeapon skill;
    [SerializeField] string animationTrigger = "Blade03";

    public void Init(IReadySheathWeapon skill, Action<Dueler_Mono> endAction)
    {
        this.skill = skill;
        Init(endAction);
    }

    public DuelerStateParrySuccess(IReadySheathWeapon parry, Dueler_Mono target, float duration, Action<Dueler_Mono> endAction) : base(duration, endAction)
    {
        this.skill = parry;
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.Rb.velocity = Vector3.zero;
        owner.Rb.isKinematic = true;

        skill.ReadyWeapon();
        owner.Animator.SetTrigger(animationTrigger);
    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        owner.Rb.velocity = Vector3.zero;
        owner.Rb.isKinematic = false;
        //skill.DamageDealer.DealDamage(target.GetComponent<IDamageable>(), (target.transform.position - owner.transform.position) * parry.ImpactForce + Vector3.up * 70f);
        skill.SheathWeapon();
    }
}

[System.Serializable]
public class DuelerStatePenaltyRigidity : DuelerStateRigidity
{
    public DuelerStatePenaltyRigidity(Action onEnterAction, Action onExitAction, float duration) : base(onEnterAction, onExitAction, duration)
    {
    }

    protected override void StateUpdate(Dueler_Mono owner)
    {
    }
}
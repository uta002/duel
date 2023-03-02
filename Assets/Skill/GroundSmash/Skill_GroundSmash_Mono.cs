using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Skill_GroundSmash_Mono : Skill_Base_Mono
{
    [SerializeField] RPC_Pos_Mono rpc_pos;
    [SerializeField] DuelerState_GroundSmash groundSmashState;
    [SerializeField] float neededAirTime = 1f;
    public override bool CanUse => base.CanUse && !owner.IsGround && CheckJumpTime;

    bool CheckJumpTime =>  owner.TimeElapsedFromLastGround >= neededAirTime;

    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        rpc_pos.Init(owner, AfterRPCAction);
        base.Init(owner, so, skillSlotID, input);
    }
    protected override void Activated()
    {
        rpc_pos.ActivateRPC(owner.transform.position);
    }

    void AfterRPCAction(Vector3 ownerPos, PhotonMessageInfo info)
    {
        owner.ChangeState(groundSmashState);
    }
}

[System.Serializable]
public class DuelerState_GroundSmash : DuelerStateDuration
{
    [SerializeField] float velocity = 30f;
    [SerializeField] AreaDamage_Mono damagePrefab;
    [SerializeField] float damageRadius = 2f;

    [SerializeField] float rigidityDuration = 0.4f;

    public DuelerState_GroundSmash(float duration) : base(duration)
    {
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.Rb.useGravity = false;
        owner.RigidbodyView.enabled = false;
    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        owner.Rb.useGravity = true;
        owner.RigidbodyView.enabled = true;
    }

    protected override void StateUpdate(Dueler_Mono owner)
    {
        owner.Rb.velocity = -Vector3.up * velocity;
        if (owner.GroundCheck())
        {
            owner.IsGround = true;
            owner.RecoverCurrentJumpCount();
            var e = GameObject.Instantiate(damagePrefab, owner.GroundHitInfo.point, Quaternion.LookRotation(Vector3.up));
            e.Init(owner, damageRadius);

            owner.ChangeState(new DuelerStateRigidity(()=>OnEnterRigidity(owner), ()=>OnExitRigidity(owner), rigidityDuration));
        }
    }

    protected override void TimeOver(Dueler_Mono owner)
    {
        owner.Rb.velocity = Vector3.zero;
        owner.ChangeState(new DuelerStateAir(0f));
    }

    void OnEnterRigidity(Dueler_Mono owner)
    {
        //owner.Animator.CrossFadeInFixedTime("GroundSmashRigidity", 0.25f, 0, 0f);
        owner.Animator.CrossFade("GroundSmashRigidity", 0.25f, 0, 0f);
    }

    void OnExitRigidity(Dueler_Mono owner)
    {
        owner.Animator.CrossFadeInFixedTime("Default", 0.25f, 0, 0f);
    }
}

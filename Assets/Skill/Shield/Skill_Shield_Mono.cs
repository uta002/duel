using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Shield_Mono : Skill_PosDir_Mono, ILinePredictionVariable
{
    public float duration = 1f;
    public float cancelableTime = 0.25f;

    [SerializeField] GameObject shieldPrefab;
    GameObject shield;

    public Vector3 Origin => owner.ShotOrigin;

    public float ProjectileSpeed => 0f;

    public float DelayInSec => 0f;

    public override ILinePredictionVariable LinePredictionVariable => this;

    public void OnStateEnter(Dueler_Mono owner)
    {
        shield = Instantiate(shieldPrefab, Origin, Quaternion.LookRotation(owner.AimDirection));
    }

    public void OnStateExit(Dueler_Mono owner)
    {
        Destroy(shield);
    }

    protected override void AfterRPCAction(Vector3 pos, float x, float y, PhotonMessageInfo info)
    {
        owner.ChangeState(new DuelerStateShield(this, cancelableTime, duration));
    }
}

public class DuelerStateShield : DuelerStateDuration
{
    float cancelableTime;
    Skill_Shield_Mono skill;
    public override bool CanUseSkill => timestamp.TimeElapsed >= cancelableTime;
    public DuelerStateShield(Skill_Shield_Mono skill, float cancelableTime, float duration) : base(duration)
    {
        this.skill = skill;
        this.cancelableTime = cancelableTime;
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.SetRootRot();
        owner.SetAnimatorAimDegreeX();
        owner.AimAnimOn();
        skill.OnStateEnter(owner);
        owner.Rb.isKinematic = true;
        owner.Rb.velocity = Vector3.zero;
    }

    protected override void StateUpdate(Dueler_Mono owner)
    {
        owner.CheckDash();
        owner.CheckJump();
    }
    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        skill.OnStateExit(owner);
        owner.AimAnimOff();
        owner.Rb.isKinematic = false;

    }

    protected override void TimeOver(Dueler_Mono owner)
    {
        owner.SetState();
    }
}
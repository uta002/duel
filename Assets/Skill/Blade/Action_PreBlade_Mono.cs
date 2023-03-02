using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DuelerState_PreBlade : DuelerStateEndAction
{
    Vector3 aimPos;
    IMeleeSkill skill_Blade;

    Vector3 direction;
    string animationName;
    float activationDistance;
    float speed;
    public DuelerState_PreBlade(string animationName, Vector3 aimPos, IMeleeSkill skill_Blade, float activationDistance, float maxDuration, float speed, System.Action<Dueler_Mono> action) : base(maxDuration, action)
    {
        this.aimPos = aimPos;
        this.skill_Blade = skill_Blade;
        this.animationName = animationName;
        this.activationDistance = activationDistance;
        this.speed = speed;
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        direction = aimPos - owner.transform.position;
        direction.y = 0f;

        owner.SetRootRot(Quaternion.LookRotation(direction).eulerAngles.y);
        owner.Animator.SetTrigger(animationName);
        skill_Blade.ReadyWeapon();
    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        skill_Blade.SheathWeapon();
    }
    Vector3 d;
    protected override void StateUpdate(Dueler_Mono owner)
    {
        d = aimPos - owner.transform.position;
        d.y = 0f;
        if (d.magnitude < activationDistance)
        {
            endAction?.Invoke(owner);
        }
        else
        {
            var target = direction.normalized * speed;
            target.y = owner.Rb.velocity.y;
            owner.Rb.velocity = target;

            owner.CheckDash();
        }
    }
}
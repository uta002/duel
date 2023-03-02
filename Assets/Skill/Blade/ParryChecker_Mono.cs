using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class ParryChecker_Mono : MonoBehaviourPunCallbacks
{
    [SerializeField] Skill_Blade_Mono skill_Blade;
    [SerializeField] RaycastHitEvent hitEvent;
    [SerializeField] float parryedStateDuration = 2f;
    Dueler_Mono owner => skill_Blade.Owner;

    public bool HitCheck(RaycastHit hitInfo)
    {
        if (hitInfo.collider.TryGetComponent(out Dueler_Mono dueler))
        {
            return dueler.CurrentState is DuelerStateParry;
        }
        return false;
    }

    [PunRPC]
    public void Parryed(Vector3 pos, int parryManID, Vector3 parryManPos)
    {
        owner.transform.position = pos;
        owner.ChangeState(new DuelerStateParryed(skill_Blade, parryedStateDuration));

        var parryMan = PhotonNetwork.GetPhotonView(parryManID).GetComponent<Dueler_Mono>();
        if(parryMan.CurrentState is DuelerStateParry parryState)
        {
            parryState.Skill_Parry.ParrySuccessRPC(owner, parryManPos);
        }

    }


}

public class DuelerStateParryed : DuelerStateDuration
{
    Skill_Blade_Mono skill_Blade;
    public DuelerStateParryed(Skill_Blade_Mono skill_Blade, float duration) : base(duration)
    {
        this.skill_Blade = skill_Blade;
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.Rb.isKinematic = true;
        owner.Animator.SetTrigger("Parryed");
        skill_Blade.ReadyWeapon();
    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        owner.Rb.isKinematic = false;

        skill_Blade.SheathWeapon();
    }

    protected override void TimeOver(Dueler_Mono owner)
    {
        owner.SetState();
    }
}



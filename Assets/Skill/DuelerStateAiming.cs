using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DuelerStateAiming : DuelerStateEndAction
{
    public DuelerStateAiming(float duration, System.Action<Dueler_Mono> endAction) : base(duration, endAction)
    {

    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.Rb.isKinematic = true;
        owner.Rb.useGravity = false;
        owner.Rb.velocity = Vector3.zero;
        owner.SetRootRot();
        owner.SetAnimatorAimDegreeX();
        owner.AimAnimOn();
    }

    protected override void StateUpdate(Dueler_Mono owner)
    {
        //owner.CheckDash();
    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        owner.AimAnimOff();
        owner.Rb.isKinematic = false;
    }
}

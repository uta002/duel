using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Skill_HomingShot_Mono : Skill_PosDirTargetRand_Mono
{
    [SerializeField] Damager damager;
    [SerializeField] AimTargetShotRigidityAction aim_TargetShot_Rigidity = new AimTargetShotRigidityAction(
        new TargetShot(
            new HomingBulletDischarger(), 
            new PosDegModifier(
                new RandomPosition(), 
                new RandomDegree()
                )
            )
        );

    public override ILinePredictionVariable LinePredictionVariable => aim_TargetShot_Rigidity;

    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        base.Init(owner, so, skillSlotID, input);
        aim_TargetShot_Rigidity.Init(owner, damager);
    }

    protected override void AfterRPCAction(Vector3 pos, float x, float y, PhotonMessageInfo info)
    {
        aim_TargetShot_Rigidity.SetTarget(currentTarget);
        aim_TargetShot_Rigidity.StartShot(pos, x, y, info);
    }
}
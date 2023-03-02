using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_AntiAirShot : Skill_PosDirTarget_Mono
{
    [SerializeField] AimTargetShotRigidityAction groundAction = new AimTargetShotRigidityAction(
            new TargetShot(
                new HomingBulletDischarger(),
                new PosDegModifier(
                    new PositionArray(),
                    new DegreeArray()
                    )
                )
        );

    [SerializeField]
    AimTargetShotRigidityAction antiAirAction = new AimTargetShotRigidityAction(
        new TargetShot(
            new HomingBulletDischarger(),
            new PosDegModifier(
                new PositionArray(),
                new DegreeArray()
                )
            )
    );

    [SerializeField] Damager groundDamager;
    [SerializeField] Damager antiAirDamager;

    public override ILinePredictionVariable LinePredictionVariable => groundAction;

    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        base.Init(owner, so, skillSlotID, input);
        groundAction.Init(owner, groundDamager);
        antiAirAction.Init(owner, antiAirDamager);
    }

    protected override void Activated()
    {
        var aimDegree = AimDegree;
        var target = Owner.TargetSystem.CurrentTarget;

        if (owner.TargetSystem.CurrentTarget is Dueler_Mono targetDueler && !targetDueler.IsGround && owner.IsGround)
        {
            photonView.RPC(nameof(StartShotAntiAir), RpcTarget.All, owner.transform.position, aimDegree.x, aimDegree.y, target.ID);
        }
        else
        {
            photonView.RPC(nameof(PosDirTargetSkill), RpcTarget.All, Owner.transform.position, aimDegree.x, aimDegree.y, target.ID);
        }
    }

    [PunRPC]
    protected void StartShotAntiAir(Vector3 pos, float angleX, float angleY, int targetID, PhotonMessageInfo info)
    {
        Owner.transform.position = pos;
        Owner.CameraPiv.rotation = Quaternion.Euler(angleX, angleY, 0f);
        currentTarget = TargetableManger.GetTargetable(targetID);

        antiAirAction.SetTarget(currentTarget);
        antiAirAction.StartShot(pos, angleX, angleY, info);
    }

    protected override void AfterRPCAction(Vector3 pos, float x, float y, PhotonMessageInfo info)
    {
        groundAction.SetTarget(currentTarget);
        groundAction.StartShot(pos, x, y, info);
    }
}

[System.Serializable]
public class PositionArray : IPosition
{
    [SerializeField] Vector3[] posArray;
    public Vector3 GetPosition(Dueler_Mono owner, Vector3 shotOrigin, Quaternion rot, int num, int total)
    {
        return shotOrigin + rot * posArray[num % posArray.Length];
    }
}

[System.Serializable]
public class DegreeArray : IDegree
{
    [SerializeField] Vector3[] dirArray;
    public Vector3 GetDegree(Dueler_Mono owner, float x, float y, int num, int total)
    {
        var rot = Quaternion.Euler(x, y, 0f);
        return PhotonUtil3D.Direction2Degree(rot * dirArray[num % dirArray.Length]);
    }
}

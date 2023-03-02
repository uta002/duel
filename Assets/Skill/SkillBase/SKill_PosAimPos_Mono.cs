using Photon.Pun;
using UnityEngine;

public abstract class Skill_PosAimPos_Mono : Skill_Aim_Mono
{
    protected override void Activated()
    {
        photonView.RPC(nameof(PosDirSkill), RpcTarget.All, Owner.transform.position, AimManager_Mono.GetAimPos(owner, LinePredictionVariable.Origin, aimType, LinePredictionVariable.ProjectileSpeed, LinePredictionVariable.DelayInSec));
    }

    [PunRPC]
    public virtual void PosDirSkill(Vector3 pos, Vector3 aimPos, PhotonMessageInfo info)
    {
        Owner.transform.position = pos;
        Owner.CameraPiv.LookAt(aimPos);
        Owner.SetRootRot();
        AfterRPCAction(pos, aimPos, info);
    }
    protected abstract void AfterRPCAction(Vector3 pos, Vector3 aimPos, PhotonMessageInfo info);
}

public abstract class Skill_PosAngleY_Mono : Skill_Base_Mono
{
    protected override void Activated()
    {
        photonView.RPC(nameof(PosAngleYSkill), RpcTarget.All, Owner.transform.position, Owner.CameraPiv.eulerAngles.y);
    }

    [PunRPC]
    public virtual void PosAngleYSkill(Vector3 pos, float angleY, PhotonMessageInfo info)
    {
        Owner.transform.position = pos;
        Owner.SetRootRot(angleY);
        AfterRPCAction(pos, angleY, info);
    }

    protected abstract void AfterRPCAction(Vector3 pos, float angleY, PhotonMessageInfo info);
}
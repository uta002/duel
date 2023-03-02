using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class Skill_PosDir_Mono : Skill_Aim_Mono
{
    protected override void Activated()
    {
        var aimDegree = AimDegree;
        photonView.RPC(nameof(PosDirSkill), RpcTarget.All, Owner.transform.position, aimDegree.x, aimDegree.y);
    }

    [PunRPC]
    public virtual void PosDirSkill(Vector3 pos, float x, float y, PhotonMessageInfo info)
    {
        Owner.transform.position = pos;
        Owner.CameraPiv.rotation = Quaternion.Euler(x, y, 0f);
        AfterRPCAction(pos, x, y, info);
    }
    protected abstract void AfterRPCAction(Vector3 pos, float x, float y, PhotonMessageInfo info);
}



public abstract class Skill_Aim_Mono : Skill_Base_Mono
{
    [SerializeField] protected AimType aimType;
    public abstract ILinePredictionVariable LinePredictionVariable { get; }

    public Vector3 AimDegree => AimManager_Mono.GetAimDegree(
        Owner, 
        LinePredictionVariable.Origin, 
        aimType, 
        LinePredictionVariable.ProjectileSpeed, 
        LinePredictionVariable.DelayInSec);
}

public interface ILinePredictionVariable
{
    Vector3 Origin { get; }
    float ProjectileSpeed { get; }
    float DelayInSec { get; }
}


[System.Serializable]
public class RPCEvent : UnityEvent<PhotonMessageInfo> { }
[System.Serializable]
public class RPCAimPosEvent : UnityEvent<Vector3, PhotonMessageInfo> { }
[System.Serializable]
public class RPCTargetEvent : UnityEvent<ITargetable, PhotonMessageInfo> { }

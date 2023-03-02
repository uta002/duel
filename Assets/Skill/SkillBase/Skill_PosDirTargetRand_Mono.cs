using Photon.Pun;
using UnityEngine;

public abstract class Skill_PosDirTargetRand_Mono : Skill_PosDirTarget_Mono
{
    protected override void Activated()
    {
        var aimDegree = AimManager_Mono.GetAimDegree(Owner, aimType, LinePredictionVariable.ProjectileSpeed);
        photonView.RPC(nameof(PosDirTargetRandSkill), RpcTarget.All, Owner.transform.position, aimDegree.x, aimDegree.y, Random.Range(int.MinValue, int.MaxValue), Owner.TargetSystem.CurrentTarget.ID);
    }

    [PunRPC]
    public virtual void PosDirTargetRandSkill(Vector3 pos, float x, float y, int randomSeed, int targetID, PhotonMessageInfo info)
    {
        Random.InitState(randomSeed);
        PosDirTargetSkill(pos, x, y, targetID, info);
    }

}

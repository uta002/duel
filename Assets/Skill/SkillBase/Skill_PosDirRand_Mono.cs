using Photon.Pun;
using UnityEngine;

public abstract class Skill_PosDirRand_Mono : Skill_PosDir_Mono
{
    protected override void Activated()
    {
        var aimDegree = AimManager_Mono.GetAimDegree(Owner, aimType, LinePredictionVariable.ProjectileSpeed);
        photonView.RPC(nameof(PosDirRandSkill), RpcTarget.All, Owner.transform.position, aimDegree.x, aimDegree.y, Random.Range(int.MinValue, int.MaxValue));
    }

    [PunRPC]
    public virtual void PosDirRandSkill(Vector3 pos, float x, float y, int randomSeed, PhotonMessageInfo info)
    {
        Random.InitState(randomSeed);
        base.PosDirSkill(pos, x, y, info);
    }

}

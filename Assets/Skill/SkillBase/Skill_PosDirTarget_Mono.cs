using Photon.Pun;
using UnityEngine;

public abstract class Skill_PosDirTarget_Mono : Skill_Aim_Mono
{
    public override bool CanUse => base.CanUse && owner.TargetSystem.TargetExisting;
    protected ITargetable currentTarget;

    protected override void Activated()
    {
        var aimDegree = AimDegree;
        photonView.RPC(nameof(PosDirTargetSkill), RpcTarget.All, Owner.transform.position, aimDegree.x, aimDegree.y, Owner.TargetSystem.CurrentTarget.ID);
    }

    [PunRPC]
    public void PosDirTargetSkill(Vector3 pos, float x, float y, int targetID, PhotonMessageInfo info)
    {
        Owner.transform.position = pos;
        Owner.CameraPiv.rotation = Quaternion.Euler(x, y, 0f);
        currentTarget = TargetableManger.GetTargetable(targetID);
        AfterRPCAction(pos, x, y, info);
    }

    protected abstract void AfterRPCAction(Vector3 pos, float x, float y, PhotonMessageInfo info);
}




[System.Serializable]
public class AimShotRigidityAction : AimShotRigidityActionBase
{
    [SerializeReference] protected IShot shot;
    public override IShot Shot => shot;
    public AimShotRigidityAction(IShot shot)
    {
        this.shot = shot;
    }

}


[System.Serializable]
public class AimTargetShotRigidityAction : AimShotRigidityActionBase, ITargetAction
{
    [SerializeReference] protected ITargetShot targetShot;
    public override IShot Shot => targetShot;

    public AimTargetShotRigidityAction(ITargetShot targetShot) => this.targetShot = targetShot;
    public void SetTarget(ITargetable target)
    {
        targetShot.SetTarget(target);
    }
}

public abstract class AimShotRigidityActionBase : ILinePredictionVariable
{
    protected Dueler_Mono owner;
    [SerializeField] protected DuelerStateAiming aimState;
    [SerializeField] protected DuelerStateRigidity rigidityState;
    public abstract IShot Shot { get; }

    public Vector3 Origin => owner.ShotOrigin;

    public float ProjectileSpeed => Shot.Discharger.ShotSpeed;

    public float DelayInSec => aimState.Duration + PhotonUtil3D.MilliSec2Sec(Shot.Discharger.DelayInMilliSec);

    float x;
    float y;
    int shotTimestamp;
    int syncId;

    public void Init(Dueler_Mono owner, IProjectileOnHit onHitEffect) => Init(owner, new IProjectileOnHit[] { onHitEffect });

    public void Init(Dueler_Mono owner, IProjectileOnHit[] onHitEffect)
    {
        this.owner = owner;
        Shot.Init(owner, onHitEffect);

        aimState.Init(OnEndAim);
        rigidityState.Init(owner.AimAnimOn, owner.AimAnimOff);
    }

    public void StartShot(Vector3 pos, float x, float y, PhotonMessageInfo info)
    {
        this.x = x;
        this.y = y;
        shotTimestamp = info.SentServerTimestamp + PhotonUtil3D.Sec2MilliSec(aimState.Duration);
        owner.ChangeState(aimState);
    }

    protected void OnEndAim(Dueler_Mono owner)
    {
        Shot.Shot(shotTimestamp, syncId, Origin, x, y);
        owner.ChangeState(rigidityState);
    }
}

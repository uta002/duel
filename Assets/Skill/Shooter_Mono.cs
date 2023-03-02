using UnityEngine;
using Photon.Pun;

public class Shooter_Mono : ShotRPC_Base_Mono
{
    [SerializeField] ShotInfoType shotInfoType;

    protected override void DirectionShotRPC_Detail(Vector3 origin, float x, float y)
    {
        if (photonView.IsMine)
        {
            switch (shotInfoType)
            {
                case ShotInfoType.None:
                    photonView.RPC(nameof(None_Shot), RpcTarget.All, origin, x, y);
                    break;
                case ShotInfoType.SyncId:
                    photonView.RPC(nameof(SyncId_Shot), RpcTarget.All, origin, x, y, SyncId);
                    break;
                case ShotInfoType.RandomSeed:
                    photonView.RPC(nameof(RandomSeed_Shot), RpcTarget.All, origin, x, y, RandomSeed);
                    break;
                case ShotInfoType.Both:
                    photonView.RPC(nameof(SyncId_RandomSeed_Shot), RpcTarget.All, origin, x, y,SyncId, RandomSeed);
                    break;
                default:
                    break;
            }
        }
    }
    protected override void DirectionTargetShotRPC_Detail(Vector3 origin, float x, float y, int targetID)
    {
        if (photonView.IsMine)
        {
            switch (shotInfoType)
            {
                case ShotInfoType.None:
                    photonView.RPC(nameof(None_TargetShot), RpcTarget.All, origin, x, y, targetID);
                    break;
                case ShotInfoType.SyncId:
                    photonView.RPC(nameof(SyncId_TargetShot), RpcTarget.All, origin, x, y, SyncId, targetID);
                    break;
                case ShotInfoType.RandomSeed:
                    photonView.RPC(nameof(RandomSeed_TargetShot), RpcTarget.All, origin, x, y, RandomSeed, targetID);
                    break;
                case ShotInfoType.Both:
                    photonView.RPC(nameof(SyncId_RandomSeed_TargetShot), RpcTarget.All, origin, x, y, SyncId, RandomSeed, targetID);
                    break;
                default:
                    break;
            }
        }
    }

    [PunRPC]
    public virtual void None_Shot(Vector3 origin, float x, float y, PhotonMessageInfo info)
    {
        AllInfo_Shot(origin, x, y, SyncIdFromInfo(info), ForceGetTarget(), info);
    }
    [PunRPC]
    public virtual void SyncId_Shot(Vector3 origin, float x, float y, int syncId, PhotonMessageInfo info)
    {
        AllInfo_Shot(origin, x, y, syncId, ForceGetTarget(), info);

    }
    [PunRPC]
    public virtual void RandomSeed_Shot(Vector3 origin, float x, float y, int randomSeed, PhotonMessageInfo info)
    {
        Random.InitState(randomSeed);
        AllInfo_Shot(origin, x, y, SyncIdFromInfo(info), ForceGetTarget(), info);

    }
    [PunRPC]
    public virtual void SyncId_RandomSeed_Shot(Vector3 origin, float x, float y, int syncId, int randomSeed, PhotonMessageInfo info)
    {
        Random.InitState(randomSeed);

        AllInfo_Shot(origin, x, y, syncId, ForceGetTarget(), info);
    }

    [PunRPC]
    public virtual void None_TargetShot(Vector3 origin, float x, float y, int targetId, PhotonMessageInfo info)
    {
        AllInfo_Shot(origin, x, y, SyncIdFromInfo(info), TargetableManger.GetTargetable(targetId), info);

    }
    [PunRPC]
    public virtual void SyncId_TargetShot(Vector3 origin, float x, float y, int syncId, int targetId, PhotonMessageInfo info)
    {
        AllInfo_Shot(origin, x, y, syncId, TargetableManger.GetTargetable(targetId), info);
    }
    [PunRPC]
    public virtual void RandomSeed_TargetShot(Vector3 origin, float x, float y, int randomSeed, int targetId, PhotonMessageInfo info)
    {
        Random.InitState(randomSeed);

        AllInfo_Shot(origin, x, y, SyncIdFromInfo(info), TargetableManger.GetTargetable(targetId), info);
    }
    [PunRPC]
    public virtual void SyncId_RandomSeed_TargetShot(Vector3 origin, float x, float y, int syncId, int randomSeed, int targetId, PhotonMessageInfo info)
    {
        Random.InitState(randomSeed);

        AllInfo_Shot(origin, x, y, syncId, TargetableManger.GetTargetable(targetId), info);
    }


}

public abstract class ShotRPC_Base_Mono : MonoBehaviourPunCallbacks
{
    const int SYNCID_MULTIPLY = 100;

    [SerializeField] Skill_Base_Mono skill;
    [SerializeField] DischargerBaseMono discharger;
    [SerializeField] int delay = 50;

    [SerializeField] protected int shotNum;
    [SerializeField] IPosition position;
    [SerializeField] IDegree degree;

    public float DelayInSec => delay * 0.001f;
    public int ShotNum => shotNum;
    protected Dueler_Mono Owner => skill.Owner;
    public DischargerBaseMono Discharger => discharger;

    int currentSyncId = 0;
    protected int SyncId
    {
        get
        {
            int syncId = currentSyncId;
            currentSyncId += ShotNum;
            return syncId;
        }
    }
    protected int RandomSeed => Random.Range(int.MinValue, int.MaxValue);

    protected virtual int SyncIdFromInfo(PhotonMessageInfo info) => unchecked(info.SentServerTimestamp * SYNCID_MULTIPLY);
    protected virtual ITargetable ForceGetTarget() => Owner.TargetSystem.CurrentTarget;

    public void DirectionShotRPC(Vector3 origin, float x, float y)
    {
        if (photonView.IsMine)
        {
            DirectionShotRPC_Detail(origin, x, y);
        }
    }
    protected abstract void DirectionShotRPC_Detail(Vector3 origin, float x, float y);
    public void DirectionTargetShotRPC(Vector3 origin, float x, float y, ITargetable target)
    {
        if (photonView.IsMine)
        {
            DirectionTargetShotRPC_Detail(origin, x, y, target.ID);
        }
    }
    protected abstract void DirectionTargetShotRPC_Detail(Vector3 origin, float x, float y, int targetId);


    int GetTimestamp(PhotonMessageInfo info) => unchecked(info.SentServerTimestamp + delay);
    public void AllInfo_Shot(Vector3 origin, float x, float y, int syncId, ITargetable target, PhotonMessageInfo info)
    {
        var rot = Quaternion.Euler(x, y, 0f);
        for (int n = 0; n < ShotNum; n++)
        {
            discharger.Shot(
                Owner, 
                position.GetPosition(Owner, origin, rot, n, ShotNum), 
                PhotonUtil3D.Degree2Direction(degree.GetDegree(Owner, x, y, n, ShotNum)), 
                syncId + n,
                target, 
                GetTimestamp(info), 
                info
                );
        }
    }
}

public class ShotRPC_None_Mono : ShotRPC_Base_Mono
{
    protected override void DirectionShotRPC_Detail(Vector3 origin, float x, float y)
    {
        photonView.RPC(nameof(None_Shot), RpcTarget.All, origin, x, y);
    }

    protected override void DirectionTargetShotRPC_Detail(Vector3 origin, float x, float y, int targetId)
    {
        photonView.RPC(nameof(None_TargetShot), RpcTarget.All, origin, x, y, targetId);
    }

    [PunRPC]
    public virtual void None_Shot(Vector3 origin, float x, float y, PhotonMessageInfo info)
    {
        AllInfo_Shot(origin, x, y, SyncIdFromInfo(info), ForceGetTarget(), info);
    }

    [PunRPC]
    public virtual void None_TargetShot(Vector3 origin, float x, float y, int targetId, PhotonMessageInfo info)
    {
        AllInfo_Shot(origin, x, y, SyncIdFromInfo(info), TargetableManger.GetTargetable(targetId), info);

    }
}

public class ShotRPC_SyncId_Mono : ShotRPC_Base_Mono
{
    protected override void DirectionShotRPC_Detail(Vector3 origin, float x, float y)
    {
        photonView.RPC(nameof(SyncId_Shot), RpcTarget.All, origin, x, y, SyncId);
    }

    protected override void DirectionTargetShotRPC_Detail(Vector3 origin, float x, float y, int targetId)
    {
        photonView.RPC(nameof(SyncId_TargetShot), RpcTarget.All, origin, x, y, SyncId, targetId);
    }

    [PunRPC]
    public virtual void SyncId_Shot(Vector3 origin, float x, float y, int syncId, PhotonMessageInfo info)
    {
        AllInfo_Shot(origin, x, y, syncId, ForceGetTarget(), info);

    }

    [PunRPC]
    public virtual void SyncId_TargetShot(Vector3 origin, float x, float y, int syncId, int targetId, PhotonMessageInfo info)
    {
        AllInfo_Shot(origin, x, y, syncId, TargetableManger.GetTargetable(targetId), info);
    }
}

public class ShotRPC_RandomSeed_Mono : ShotRPC_Base_Mono
{
    protected override void DirectionShotRPC_Detail(Vector3 origin, float x, float y)
    {
        photonView.RPC(nameof(RandomSeed_Shot), RpcTarget.All, origin, x, y, RandomSeed);
    }

    protected override void DirectionTargetShotRPC_Detail(Vector3 origin, float x, float y, int targetId)
    {
        photonView.RPC(nameof(RandomSeed_TargetShot), RpcTarget.All, origin, x, y, RandomSeed, targetId);
    }

    [PunRPC]
    public virtual void RandomSeed_Shot(Vector3 origin, float x, float y, int randomSeed, PhotonMessageInfo info)
    {
        Random.InitState(randomSeed);

        AllInfo_Shot(origin, x, y, SyncIdFromInfo(info), ForceGetTarget(), info);

    }
    [PunRPC]
    public virtual void RandomSeed_TargetShot(Vector3 origin, float x, float y, int randomSeed, int targetId, PhotonMessageInfo info)
    {
        Random.InitState(randomSeed);

        AllInfo_Shot(origin, x, y, SyncIdFromInfo(info), TargetableManger.GetTargetable(targetId), info);
    }
}

public class ShotRPC_SyncId_RandomSeed_Mono : ShotRPC_Base_Mono
{
    protected override void DirectionShotRPC_Detail(Vector3 origin, float x, float y)
    {
        photonView.RPC(nameof(SyncId_RandomSeed_Shot), RpcTarget.All, origin, x, y, SyncId, RandomSeed);
    }

    protected override void DirectionTargetShotRPC_Detail(Vector3 origin, float x, float y, int targetId)
    {
        photonView.RPC(nameof(SyncId_RandomSeed_TargetShot), RpcTarget.All, origin, x, y, SyncId, RandomSeed, targetId);
    }

    [PunRPC]
    public virtual void SyncId_RandomSeed_Shot(Vector3 origin, float x, float y, int syncId, int randomSeed, PhotonMessageInfo info)
    {
        Random.InitState(randomSeed);

        AllInfo_Shot(origin, x, y, syncId, ForceGetTarget(), info);
    }

    [PunRPC]
    public virtual void SyncId_RandomSeed_TargetShot(Vector3 origin, float x, float y, int syncId, int randomSeed, int targetId, PhotonMessageInfo info)
    {
        Random.InitState(randomSeed);

        AllInfo_Shot(origin, x, y, syncId, TargetableManger.GetTargetable(targetId), info);
    }
}

public enum ShotInfoType
{
    None,
    SyncId,
    RandomSeed,
    Both,
}

public interface IPosition
{
    Vector3 GetPosition(Dueler_Mono owner, Vector3 shotOrigin, Quaternion rot, int num, int total);
}

public interface IDegree
{
    Vector3 GetDegree(Dueler_Mono owner, float x, float y, int num, int total);
}


public abstract class DischargerBaseMono<ProjectileType> : DischargerBaseMono where ProjectileType : UnityEngine.Object
{
    [SerializeField] protected ProjectileType prefab;
    [SerializeField] protected float shotSpeed;

    public override float ShotSpeed => shotSpeed;
}

public abstract class DischargerBaseMono : MonoBehaviourPunCallbacks
{
    public abstract void Shot(Dueler_Mono owner, Vector3 origin, Vector3 direction, int syncId, ITargetable target, int timestamp, PhotonMessageInfo info);
    public abstract float ShotSpeed { get; }
}
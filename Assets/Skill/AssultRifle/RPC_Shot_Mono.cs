using UnityEngine;
using Photon.Pun;

public class RPC_Shot_Mono : MonoBehaviourPunCallbacks
{
    event System.Action<Vector3, float, float, PhotonMessageInfo> action;
    public void Init(System.Action<Vector3, float, float, PhotonMessageInfo> action)
    {
        this.action = action;
    }
    public void ShotRPC(Vector3 origin, float x, float y)
    {
        photonView.RPC(nameof(PosDirSkill), RpcTarget.All, origin, x, y);
    }

    [PunRPC]
    protected virtual void PosDirSkill(Vector3 pos, float x, float y, PhotonMessageInfo info)
    {
        action?.Invoke(pos, x, y, info);
    }
}



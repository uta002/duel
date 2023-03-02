using UnityEngine;
using Photon.Pun;

public class RPC_Pos_Mono : MonoBehaviourPunCallbacks
{
    Dueler_Mono owner;
    event System.Action<Vector3, PhotonMessageInfo> action;
    public void Init(Dueler_Mono owner, System.Action<Vector3, PhotonMessageInfo> action)
    {
        this.owner = owner;
        this.action = action;
    }
    public void ActivateRPC(Vector3 ownerPos)
    {
        photonView.RPC(nameof(AfterRPC), RpcTarget.All, ownerPos);
    }

    [PunRPC]
    protected void AfterRPC(Vector3 ownerPos, PhotonMessageInfo info)
    {
        owner.transform.position = ownerPos;
        action?.Invoke(ownerPos, info);
    }
}

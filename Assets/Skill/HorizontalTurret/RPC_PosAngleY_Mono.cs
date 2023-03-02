using UnityEngine;
using Photon.Pun;

public class RPC_PosAngleY_Mono : MonoBehaviourPunCallbacks
{

    Dueler_Mono owner;
    event System.Action<Vector3, float, PhotonMessageInfo> action;
    public void Init(Dueler_Mono owner, System.Action<Vector3, float, PhotonMessageInfo> action)
    {
        this.owner = owner;
        this.action = action;
    }

    public void ActivateRPC()
    {
        photonView.RPC(nameof(AfterRPC), RpcTarget.All, owner.transform.position, owner.AngleY);
    }

    [PunRPC]
    protected void AfterRPC(Vector3 ownerPos, float angleY, PhotonMessageInfo info)
    {
        owner.transform.position = ownerPos;
        owner.SetRootRot(angleY);
        action?.Invoke(ownerPos, angleY, info);
    }
}

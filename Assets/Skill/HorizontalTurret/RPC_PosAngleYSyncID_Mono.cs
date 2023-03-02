using UnityEngine;
using Photon.Pun;

public class RPC_PosAngleYSyncID_Mono : MonoBehaviourPunCallbacks
{
    int currentID;
    Dueler_Mono owner;
    event System.Action<Vector3, float, int, PhotonMessageInfo> action;
    public void Init(Dueler_Mono owner, System.Action<Vector3, float, int, PhotonMessageInfo> action)
    {
        this.owner = owner;
        this.action = action;
        currentID = 0;
    }

    public void ActivateRPC()
    {
        photonView.RPC(nameof(AfterRPC), RpcTarget.All, owner.transform.position, owner.AngleY, currentID);
        currentID++;
    }

    [PunRPC]
    protected void AfterRPC(Vector3 ownerPos, float angleY, int syncID, PhotonMessageInfo info)
    {
        owner.transform.position = ownerPos;
        owner.SetRootRot(angleY);
        action?.Invoke(ownerPos, angleY, syncID, info);
    }
}

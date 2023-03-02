using Photon.Pun;
using UnityEngine;

public class RPC_PosDir_Mono : MonoBehaviourPunCallbacks
{
    protected Dueler_Mono owner;
    event System.Action<Vector3, float, float, PhotonMessageInfo> action;

    protected virtual Vector3 AimDegree => owner.CameraPiv.eulerAngles;
    public void Init(Dueler_Mono owner, System.Action<Vector3, float, float, PhotonMessageInfo> action)
    {
        this.owner = owner;
        this.action = action;
    }

    public void ActivateRPC()
    {
        var aimDegree = AimDegree;
        photonView.RPC(nameof(Activate), RpcTarget.All, owner.transform.position, aimDegree.x, aimDegree.y);
    }

    [PunRPC]
    protected void Activate(Vector3 pos, float x, float y, PhotonMessageInfo info)
    {
        owner.transform.position = pos;
        owner.CameraPiv.rotation = Quaternion.Euler(x, y, 0f);
        action?.Invoke(pos, x, y, info);
    }
}
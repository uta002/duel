using Photon.Pun;

public class RPC_Activate_Mono : MonoBehaviourPunCallbacks
{
    event System.Action<PhotonMessageInfo> action;
    public void Init(System.Action<PhotonMessageInfo> action)
    {
        this.action = action;
    }
    public void ActivateRPC()
    {
        photonView.RPC(nameof(Activate), RpcTarget.All);
    }

    [PunRPC]
    protected void Activate(PhotonMessageInfo info)
    {
        action?.Invoke(info);
    }
}



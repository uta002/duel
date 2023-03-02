using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraRotSync_Mono : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] Transform target;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

            var degree = PhotonUtil3D.Direction2Degree(target.forward);
            stream.SendNext(degree.y);
            stream.SendNext(degree.x);
        }
        else
        {
            float y = (float)stream.ReceiveNext();
            float x = (float)stream.ReceiveNext();
            target.rotation = Quaternion.Euler(x, y, 0f);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Land_Mono : MonoBehaviourPunCallbacks
{
    [PunRPC]
    public void OnLand(Vector3 pos)
    {

    }
}

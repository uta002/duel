using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraSetter : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform cameraPos;
    void Start()
    {
        if (photonView.IsMine)
        {
            Camera.main.transform.SetParent(cameraPos, false);
        }   
    }


}

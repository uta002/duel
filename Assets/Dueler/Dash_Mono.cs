using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Dash_Mono : MonoBehaviourPunCallbacks
{
    Dueler_Mono owner;
    public bool IsDashing => isDashing;
    bool isDashing;

    public bool DashStun => dashStun;
    bool dashStun;
    
    private void Awake()
    {
        owner = GetComponent<Dueler_Mono>();
    }
    void Update()
    {
        if (photonView.IsMine)
        {
            if (owner.DashInput)
            {
                var velocity = owner.WorldMoveDirection;
                velocity = velocity == Vector3.zero ? owner.CameraPiv.forward : velocity;
                velocity.y = 0f;
                velocity = velocity.normalized * owner.DashVel;
                photonView.RPC("Dash", RpcTarget.All, transform.position, velocity);
            }
        }
    }

    [PunRPC]
    void Dash(Vector3 pos, Vector3 vel)
    {
        owner.ChangeState(new DuelerStateDash(pos, vel, owner.DashDuration));
    }

}




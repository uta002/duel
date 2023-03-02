using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Jump_Mono : MonoBehaviourPunCallbacks
{
    public void Jump(Dueler_Mono owner)
    {
        var velocity = owner.Rb.velocity;
        velocity.y = owner.JumpVelY;
        owner.OnAirborne(owner.transform.position, velocity);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Movement_Mono : MonoBehaviourPunCallbacks
{
    public void Move(Dueler_Mono moveParams)
    {
        if (moveParams.IsGround)
        {
            var moveVec = moveParams.WorldMoveDirection;
            moveVec.y = 0f;
            Vector3 addForceVector = (moveVec.normalized * moveParams.MoveSpeed - moveParams.Rb.velocity) * moveParams.Acceleration;

            addForceVector.y = 0f;
            if (moveParams.GroundNormal.y >= Mathf.Cos(moveParams.WalkableDegree * Mathf.Deg2Rad))
            {
                //Quaternion rotDiff = Quaternion.FromToRotation(Vector3.up, moveParams.GroundNormal);
                //addForceVector = rotDiff * addForceVector;
                addForceVector = Vector3.ProjectOnPlane(addForceVector, moveParams.GroundNormal);
            }
            moveParams.Rb.AddForce(addForceVector);
        }
    }
}

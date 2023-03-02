using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class LookTarget_Mono : MonoBehaviour
{
    public float RotateIntensity { get; set; }


    public void LookTargetPos(Rigidbody rb, Vector3 targetPos)
    {
        LookTargetDir(rb, targetPos - transform.position);
    }

    public void LookTargetDir(Rigidbody rb, Vector3 targetDir)
    {
        Quaternion r = default;
        if (targetDir.magnitude > 0.01f && targetDir.magnitude < 10000.0f)
            r = Quaternion.LookRotation(targetDir) * Quaternion.Inverse(transform.rotation);
        if (r.w < 0)
        {
            r.x = -r.x;
            r.y = -r.y;
            r.z = -r.z;
            r.w = -r.w;
        }
        Vector3 rotVec = new Vector3(r.x, r.y, r.z);
        rb.AddTorque(rotVec * RotateIntensity);
    }
}

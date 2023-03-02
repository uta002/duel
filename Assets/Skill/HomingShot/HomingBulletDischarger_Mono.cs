using UnityEngine;
using Photon.Pun;
using System;

[System.Serializable]
public class HomingBulletDischarger : DischargerBase<HomingProjectile_Mono>, ITargetDischarger
{
    [SerializeField] float homing = 5f;
    [SerializeField] float angularDrag = 10f;
    ITargetable target;
    public void SetTarget(ITargetable target)
    {
        this.target = target;
    }

    public override void ShotInit(HomingProjectile_Mono projectile, int timestamp, int syncId, Vector3 origin, Vector3 direction)
    {
        projectile.Init(owner, timestamp, target, syncId, origin, direction, ShotSpeed, homing, angularDrag, lifeTime, onHitEffect);
    }
}

[Serializable]
public class LookTarget
{
    [SerializeField] float rotateIntensity;
    public float RotateIntensity { get => rotateIntensity; set => rotateIntensity = value; }

    public void LookTargetPos(Rigidbody rb, Vector3 currentPos, Quaternion currentRot, Vector3 targetPos)
    {
        LookTargetDir(rb, currentRot, targetPos - currentPos);
    }

    public void LookTargetDir(Rigidbody rb, Quaternion currentRot, Vector3 targetDir)
    {
        Quaternion r = default;
        if (targetDir.magnitude > 0.01f && targetDir.magnitude < 10000.0f)
            r = Quaternion.LookRotation(targetDir) * Quaternion.Inverse(currentRot);
        if (r.w < 0)
        {
            r.x = -r.x;
            r.y = -r.y;
            r.z = -r.z;
            r.w = -r.w;
        }
        Vector3 rotVec = new Vector3(r.x, r.y, r.z);
        rb.AddTorque(rotVec * rotateIntensity);
    }
}

[System.Serializable]
public class RandomPosition : IPosition
{
    [SerializeField] float max_pos_spread = .5f;

    public Vector3 GetPosition(Dueler_Mono owner, Vector3 shotOrigin, Quaternion rot, int num, int total)
    {
        return shotOrigin + rot * UnityEngine.Random.insideUnitCircle * max_pos_spread;
    }
}

public interface ITargetDischarger : IDischarger, ITargetAction { }

using UnityEngine;

public class AimManager_Mono : MonoBehaviour
{
    [SerializeField] AimType aimDirecitonType;



    public static Vector3 GetTargetPos(Vector3 shotPosition, ITargetable target, TargetPosType targetPosType, float shotSpeed)
    {
        switch (targetPosType)
        {
            case TargetPosType.Direct:
                return target.CurrentPos;
            case TargetPosType.Predict:
                return LinePrediction.LinePrediction2(shotPosition, target.CurrentPos, target.Velocity, shotSpeed);
            default:
                return target.CurrentPos;
        }
    }

    public Vector3 GetAimPos(Dueler_Mono owner, float shotSpeed) => GetAimPos(owner, aimDirecitonType, shotSpeed);

    public static Vector3 GetAimPos(Dueler_Mono owner, AimType aimType, float shotSpeed)
    {
        switch (aimType)
        {
            case AimType.Reticle:
                return owner.DefaultAimPos;
            case AimType.Direct:
                return owner.AimPos;
            case AimType.Predict:
                if (owner.TargetSystem.TargetExisting)
                {
                    return GetTargetPos(owner.ShotOrigin, owner.TargetSystem.CurrentTarget, TargetPosType.Predict, shotSpeed);
                }
                return owner.AimPos;
            default:
                return owner.AimPos;
        }
    }

    public Vector3 GetAimDirection(Dueler_Mono owner, float shotSpeed) => GetAimDirection(owner, aimDirecitonType, shotSpeed);

    public static Vector3 GetAimDirection(Dueler_Mono owner, AimType aimType, float shotSpeed)
    {
        return GetAimPos(owner, aimType, shotSpeed) - owner.CameraPiv.position;
    }

    public Vector3 GetAimDegree(Dueler_Mono owner, float shotSpeed) => GetAimDegree(owner, aimDirecitonType, shotSpeed);


    public static Vector3 GetAimDegree(Dueler_Mono owner, AimType aimType, float shotSpeed)
    {
        return PhotonUtil3D.Direction2Degree(GetAimDirection(owner, aimType, shotSpeed));
    }

    public static Vector3 GetAimDegree(Dueler_Mono owner, Vector3 shotOrigin, AimType aimType, float shotSpeed, int delayInMilliSec)
    {
        return GetAimDegree(owner, shotOrigin, aimType, shotSpeed, PhotonUtil3D.MilliSec2Sec(delayInMilliSec));
    }

    public static Vector3 GetAimDegree(Dueler_Mono owner, AimType aimType, ILinePredictionVariable line)
    {
        return GetAimDegree(owner, line.Origin, aimType, line.ProjectileSpeed, line.DelayInSec);
    }

    public static Vector3 GetAimDegree(Dueler_Mono owner, Vector3 shotOrigin, AimType aimType, float shotSpeed, float delayInSec)
    {
        return PhotonUtil3D.Direction2Degree(GetAimPos(owner, shotOrigin, aimType, shotSpeed, delayInSec) - shotOrigin);
    }

    public static Vector3 GetAimPos(Dueler_Mono owner, Vector3 shotOrigin, AimType aimType, float shotSpeed, float delayInSec)
    {
        switch (aimType)
        {
            case AimType.Reticle:
                return owner.DefaultAimPos;
            case AimType.Direct:
                return owner.AimPos;
            case AimType.Predict:
                var target = owner.TargetSystem.CurrentTarget;
                if (target != null)
                {
                    return LinePrediction.LinePredictionWithDelay(shotOrigin, target.CurrentPos, target.Velocity, shotSpeed, delayInSec);
                }
                return owner.AimPos;
            default:
                return owner.AimPos;
        }
    }


}
public enum AimType
{
    Reticle,
    Direct,
    Predict,
}
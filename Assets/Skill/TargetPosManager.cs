using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPosManager : MonoBehaviour
{
    [SerializeField] TargetPosType targetPosType;
    public Vector3 GetTargetPos(ITargetable target, float shotSpeed)
    {
        switch (targetPosType)
        {
            case TargetPosType.Direct:
                return target.CurrentPos;
            case TargetPosType.Predict:
                return LinePrediction.LinePrediction2(transform.position, target.CurrentPos, target.Velocity, shotSpeed);
            default:
                return target.CurrentPos;
        }
    }
}

public enum TargetPosType
{
    Direct,
    Predict,
}

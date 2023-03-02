using Photon.Pun;
using UnityEngine;

public class RPC_AimPosDir_Mono : RPC_PosDir_Mono
{
    [SerializeField] AimType aimType;
    ILinePredictionVariable linePrediction;

    protected override Vector3 AimDegree => AimManager_Mono.GetAimDegree(owner, aimType, linePrediction);
    public void Init(Dueler_Mono owner, ILinePredictionVariable linePrediction, System.Action<Vector3, float, float, PhotonMessageInfo> action)
    {
        this.linePrediction = linePrediction;
        base.Init(owner, action);
    }
}

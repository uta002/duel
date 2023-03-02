using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_MovingShield_Mono : Skill_Base_Mono
{
    [SerializeField] RPC_PosAngleY_Mono rpc_posAngleY;
    [SerializeField] GameObject shieldPrefab;
    [SerializeField] DuelerState_MovingShield state_movingShield;

    GameObject shield;
    float angleY;

    public override bool CanUse => base.CanUse && owner.IsGround;

    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        rpc_posAngleY.Init(owner, Shift_StateMovingShield);
        state_movingShield.Init(OnEnter, OnExit);
        base.Init(owner, so, skillSlotID, input);
    }

    protected override void Activated()
    {
        rpc_posAngleY.ActivateRPC();
    }

    void Shift_StateMovingShield(Vector3 pos, float angleY, PhotonMessageInfo info)
    {
        this.angleY = angleY;
        owner.ChangeState(state_movingShield);
    }

    void OnEnter(Dueler_Mono owner)
    {
        Vector3 pos = owner.HeartPos + owner.CameraPiv.forward * 0.9f;
        shield = Instantiate(shieldPrefab, pos, Quaternion.Euler(0f, angleY, 0f), owner.CameraPiv);
    }

    void OnExit(Dueler_Mono owner)
    {
        Destroy(shield);
    }


}



[System.Serializable]
public class DuelerState_MovingShield : DuelerStateDefault
{
    [SerializeField] Vector3 cameraPos;
    [SerializeField] TimestampLifeTime stateLifeTime;

    //public override bool CanUseSkill => false;

    System.Action<Dueler_Mono> onEnter;
    System.Action<Dueler_Mono> onExit;
    public void Init(System.Action<Dueler_Mono> onEnter, System.Action<Dueler_Mono> onExit)
    {
        this.onEnter = onEnter;
        this.onExit = onExit;
    }

    public override void OnEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.CameraPos.localPosition = cameraPos;
        owner.AimAnimOn();
        onEnter?.Invoke(owner);
        stateLifeTime.Init();
        base.OnEnter(owner, prevState);
    }

    public override void Update(Dueler_Mono owner)
    {
        base.Update(owner);
        owner.SetAnimatorAimDegreeX();
        if (stateLifeTime.IsOverLifeTime)
        {
            owner.SetState();
        }
    }


    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        owner.CameraPos.localPosition = Vector3.zero;
        owner.AimAnimOff();
        onExit?.Invoke(owner);

        base.OnExit(owner, nextState);
    }
}
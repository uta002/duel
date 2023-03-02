using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Skill_AssultRifle_Mono : Skill_Base_Mono
{
    [SerializeField] RPC_Activate_Mono rpc_activate;
    [SerializeField] RPC_Shot_Mono rpc_shot;

    [SerializeField] MagazineCoolDown magazineCoolDown;
    [SerializeField] DuelerState_ADS state_ADS;
    [SerializeReference] IShot shot = new Shot(new ProjectileDischarger(), new PosDegModifier(new PosNoChange(), new RandomDegree()));
    [SerializeField] Damager bulletDamager;

    [SerializeField] AimType aimType;

    bool IsAdsing => state_ADS.IsAdsing;
    Vector3 ShotOrigin => owner.ShotOrigin;
    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        magazineCoolDown.Init();
        shot.Init(owner, bulletDamager);
        rpc_activate.Init(Shift_State_ADS);
        rpc_shot.Init(Shot);
        base.Init(owner, so, skillSlotID, input);
    }

    void Shift_State_ADS(PhotonMessageInfo info)
    {
        owner.ChangeState(state_ADS);
    }

    void Shot(Vector3 origin, float x, float y, PhotonMessageInfo info)
    {
        shot.Shot(info.SentServerTimestamp, info.SentServerTimestamp, origin, x, y);
        state_ADS.ResetLifeTime();

    }

    public override bool CanUse
    {
        get
        {
            if (IsAdsing)
            {
                return magazineCoolDown.CanUse;
            }
            else
            {
                return owner.IsGround && owner.CanUseSkills;
            }
        }
    }

    protected override void Activated()
    {
        if (IsAdsing)
        {
            var deg = AimManager_Mono.GetAimDegree(owner, ShotOrigin, aimType, shot.Discharger.ShotSpeed, shot.Discharger.DelayInMilliSec);
            rpc_shot.ShotRPC(ShotOrigin, deg.x, deg.y);
        }
        else
        {
            rpc_activate.ActivateRPC();
        }
    }

    public override void ButtonDown()
    {
    }

    public override void Button()
    {
        TryActivate();
    }

    public override void TryActivate()
    {
        if (CanUse)
        {
            Activated();
            magazineCoolDown.Use();
        }
    }
}

public class DuelerState_TransitionADS : DuelerStateDefault
{
    public override bool CanUseSkill => false;

    public override void OnEnter(Dueler_Mono owner, IDuelerState prevState)
    {
    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
    }

    public override void Update(Dueler_Mono owner)
    {
    }
}

[System.Serializable]
public class DuelerState_ADS : DuelerStateDefault
{
    [SerializeField] Vector3 cameraPos;
    [SerializeField] TimestampLifeTime stateLifeTime;
    bool isAdsing;
    public bool IsAdsing => isAdsing;
    //public override bool CanUseSkill => false;

    public override void OnEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        isAdsing = true;
        owner.CameraPos.localPosition = cameraPos;
        owner.AimAnimOn();
        ResetLifeTime();
        base.OnEnter(owner, prevState);
    }
    public void ResetLifeTime() => stateLifeTime.Init();

    public override void Update(Dueler_Mono owner)
    {
        owner.SetAnimatorAimDegreeX();
        base.Update(owner);
        if (stateLifeTime.IsOverLifeTime)
        {
            owner.SetState();
        }
    }


    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        isAdsing = false;
        owner.CameraPos.localPosition = Vector3.zero;
        owner.AimAnimOff();
        base.OnExit(owner, nextState);
    }
}

[System.Serializable]
public class MagazineCoolDown : ICooldown
{
    [SerializeField] int maxCapacity = 30;
    int currentAmmo;
    [SerializeField] Cooldown shotCooldown;
    [SerializeField] CooldownCallbacks reloadCooldown;

    public void Init()
    {
        currentAmmo = 30;
        reloadCooldown.Init(()=> currentAmmo = maxCapacity);
    }

    public bool CanUse
    {
        get
        {
            if (currentAmmo > 0)
            {
                return shotCooldown.CanUse;
            }
            else
            {
                reloadCooldown.Use();
                return false;
            }
        }
    }

    public void Use()
    {
        currentAmmo--;
        shotCooldown.Use();
    }

}

[System.Serializable]
public class CooldownCallbacks : Cooldown
{
    System.Action callback;
    bool notify;
    public void Init(System.Action callback)
    {
        this.callback = callback;
    }

    public override bool CanUse
    {
        get
        {
            if (base.CanUse && notify)
            {
                SendCallBack();
            }
            return base.CanUse;
        }
    }

    protected override void SetStartTime()
    {
        notify = true;
        base.SetStartTime();
    }

    public override void ForceReady()
    {
        SendCallBack();
        base.ForceReady();
    }

    void SendCallBack()
    {
        callback?.Invoke();
        notify = false;
    }
}
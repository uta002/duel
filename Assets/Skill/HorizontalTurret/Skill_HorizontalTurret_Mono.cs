using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class Skill_HorizontalTurret_Mono : Skill_Base_Mono
{
    [SerializeField] RPC_PosAngleY_Mono rpc_PutTurret;
    [SerializeField] RPC_Activate_Mono rpc_ShotStart;
    [SerializeField] HorizontalTurret_Mono HorizontalTurretPrefab;


    [SerializeField] Cooldown shotAvairableCoolDown;
    [SerializeField] DuelerState_Place state_Place;
    [SerializeField] HorizontalTurret horizontalTurret;
    [SerializeField] float placeDistance = 0.5f;
    [SerializeField] int delay = 50;

    bool turretExisting;
    HorizontalTurret_Mono horiTurret;

    public override bool CanUse => base.CanUse && owner.IsGround;
    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        rpc_ShotStart.Init(ShotStart);
        rpc_PutTurret.Init(owner, AfterRPCAction);
        base.Init(owner, so, skillSlotID, input);
        turretExisting = false;
    }
    public override void ButtonDown()
    {
        if (turretExisting && shotAvairableCoolDown.CanUse )
        {
            rpc_ShotStart.ActivateRPC();
            turretExisting = false;
            cooldown.ForceCountStart();
            
        }
        else if(CanUse)
        {
            rpc_PutTurret.ActivateRPC();

            shotAvairableCoolDown.ForceCountStart();
            cooldown.ForceCountStart();
        }
    }

    protected void AfterRPCAction(Vector3 pos, float angleY, PhotonMessageInfo info)
    {
        state_Place.Init(d => OnEndPlace(pos, angleY, info, state_Place.Delay));
        owner.ChangeState(state_Place);

        //OnEndPlace(pos, angleY, info, state_Place.Delay);
    }

    protected void OnEndPlace(Vector3 pos, float angleY, PhotonMessageInfo info, int placeStateDelay)
    {
        var rot = Quaternion.Euler(0f, angleY, 0f);
        var instantiatePos = owner.Root.position + rot * Vector3.forward * placeDistance;
        horiTurret = Instantiate(HorizontalTurretPrefab, instantiatePos, rot);
        turretExisting = true;
    }

    protected void ShotStart(PhotonMessageInfo info)
    {
        if(horiTurret != null)
        {
            int timestamp = unchecked(delay + info.SentServerTimestamp);
            horiTurret.Init(owner, horizontalTurret, timestamp);
        }
    }

    protected override void Activated()
    {
        
    }
}

[Serializable]
public class DuelerState_Place : DuelerStateEndAction
{
    public DuelerState_Place(float duration, Action<Dueler_Mono> endAction) : base(duration, endAction)
    {
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.Animator.CrossFade("Place", 0.25f);
        owner.Rb.isKinematic = true;
        owner.Rb.velocity = Vector3.zero;
    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        owner.Rb.isKinematic = false;
    }

    protected override void TimeOver(Dueler_Mono owner)
    {
        base.TimeOver(owner);
        owner.SetState();
    }
}

[System.Serializable]
public class HorizontalTurret
{
    [SerializeField] TimestampLifeTime lifeTime;
    [SerializeField] Cooldown shotCooldown;
    [SerializeField] Shot shot = new Shot(new ProjectileDischarger(), new PosDegNoChange());
    [SerializeField] DamagerDecrease damager;
    [SerializeField] int shotNum = 4;
    Vector3 shotOrigin;
    int currentShotNum;
    int timestamp;
    float angleY;

    public bool IsOverLifeTime => lifeTime.IsOverLifeTime;

    public void Init(Dueler_Mono owner, int timestamp, float angleY, Vector3 shotOrigin)
    {
        shot.Init(owner, damager);
        lifeTime.Init();
        this.timestamp = timestamp;
        currentShotNum = 0;
        this.angleY = angleY;
        this.shotOrigin = shotOrigin;
        damager.Init();
    }

    public void Update()
    {
        if (shotCooldown.CanUse && currentShotNum < shotNum)
        {
            int shotTime = timestamp + PhotonUtil3D.Sec2MilliSec(shotCooldown.CoolDownTime) * currentShotNum;
            //int shotTime = PhotonNetwork.ServerTimestamp;
            shot.Shot(shotTime, shotTime, shotOrigin, 0f, angleY);
            currentShotNum++;
            shotCooldown.Use();
        }
    }
}

[System.Serializable]
public class DamagerDecrease : DamagerBase, IProjectileOnHit
{
    [SerializeField] float baseDamage;
    [SerializeField] float decreaseRate;
    Dictionary<IDamageable, int> damageables = new Dictionary<IDamageable, int>();

    public void Init()
    {
        damageables.Clear();
    }
    public void ProjectileOnHit(Dueler_Mono owner, ProjectileBase_Mono p, RaycastHit hit)
    {
        if (hit.collider.TryGetComponent(out IDamageable damageable))
        {
            if (damageable.TeamID != owner.TeamID)
            {
                if (!damageables.ContainsKey(damageable))
                {
                    damageables.Add(damageable, 0);
                }

                var damage = baseDamage * Mathf.Pow(decreaseRate, damageables[damageable]);
                damageable.TakeDamage(new DamageInfo(owner.TeamID, owner.ID, damageType, impactType, p.transform.forward * impactForce, damage));
                damageables[damageable] += 1;
            }
        }
    }
}
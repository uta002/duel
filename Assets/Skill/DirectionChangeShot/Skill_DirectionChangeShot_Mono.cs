using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Skill_DirectionChangeShot_Mono : Skill_PosDirRand_Mono
{
    [SerializeField] AimShotRigidityAction shot = new AimShotRigidityAction(
        new Shot(
            new DirectionChangeBulletDischarger(), 
            new PosDegModifier(
                new RandomPosition(), 
                new DegNoChange()
                )
            )
        );
    [SerializeField] Damager bulletDamager;
    DirectionChangeBulletDischarger bulletDischarger;

    public override ILinePredictionVariable LinePredictionVariable => shot;

    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        base.Init(owner, so, skillSlotID, input);
        shot.Init(owner, bulletDamager);
        bulletDischarger = shot.Shot.Discharger as DirectionChangeBulletDischarger;
    }
    protected override void AfterRPCAction(Vector3 pos, float x, float y, PhotonMessageInfo info)
    {
        shot.StartShot(pos, x, y, info);
    }

    protected void OnHitEffect(ProjectileBase_Mono projectile, RaycastHit hit)
    {
        bulletDamager.DealDamage(owner, projectile.transform.forward, hit);
    }

    bool NewShot => bulletDischarger.NewShot;

    public override bool CanUse => NewShot ? base.CanUse : owner.TargetSystem.TargetExisting;

    public override void ButtonDown()
    {
        if (NewShot)
        {
            base.ButtonDown();
        }
        else
        {
            photonView.RPC(nameof(ChangeDirection), RpcTarget.All);
        }
    }

    [PunRPC]
    void ChangeDirection(PhotonMessageInfo info)
    {
        bulletDischarger.ChnageDirection(owner, info);
    }
}


[System.Serializable]
public class DirectionChangeBulletDischarger : DischargerBase<DirectionChangeBullet_Mono>
{

    [SerializeField] int secondDelay = 50;
    [SerializeField] float secondLifeTime = 1f;
    [SerializeField] float secondShotSpeed = 10f;
    [SerializeField] AimType secondAimType;

    List<DirectionChangeBullet_Mono> shotList = new List<DirectionChangeBullet_Mono>();
    public bool NewShot => shotList.Count == 0;

    public void ChnageDirection(Dueler_Mono owner, PhotonMessageInfo info)
    {
        Vector3 targetPos;
        var secondDelayInSec = PhotonUtil3D.MilliSec2Sec(secondDelay);
        foreach (var b in shotList)
        {
            if (b == null)
                continue;

            targetPos = AimManager_Mono.GetAimPos(owner, b.transform.position, secondAimType, secondShotSpeed, secondDelayInSec);

            var timestamp = unchecked(info.SentServerTimestamp + secondDelay);
            b.ChangeDirection(timestamp, b.transform.position, targetPos, secondLifeTime, secondShotSpeed);
        }
        shotList.Clear();
    }

    public override void ShotInit(DirectionChangeBullet_Mono b, int timestamp, int syncId, Vector3 origin, Vector3 direction)
    {
        b.Init(owner, timestamp, syncId, origin, direction, lifeTime, shotSpeed, onHitEffect);
        shotList.Add(b);
    }
}
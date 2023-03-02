using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Skill_ShotGun_Mono : Skill_PosDirRand_Mono
{
    [SerializeField] AimShotRigidityAction action = new AimShotRigidityAction(
        new Shot(
            new ProjectileDischarger(), 
            new PosDegModifier(
                new PosNoChange(), 
                new RandomDegree()
                )));

    [SerializeField] ShotGunBulletDamgaer damager;
    public override ILinePredictionVariable LinePredictionVariable => action;


    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        base.Init(owner, so, skillSlotID, input);
        action.Init(owner, damager);
    }

    protected override void AfterRPCAction(Vector3 pos, float x, float y, PhotonMessageInfo info)
    {
        damager.Init();
        action.StartShot(pos, x, y, info);
    }
}

[System.Serializable]
public class ShotGunBulletDamgaer : DamagerBase ,IProjectileOnHit
{
    [SerializeField] float damage;
    [SerializeField] int needBullet_Cancel;
    [SerializeField] int needBullet_Ragdoll;
    Dictionary<IDamageable, int> damageables;

    public void Init()
    {
        damageables = new Dictionary<IDamageable, int>();
    }
    public void DealDamage(Dueler_Mono owner, ProjectileBase_Mono p, RaycastHit hit)
    {


        if (hit.collider.TryGetComponent(out IDamageable damageable))
        {
            if(damageable.TeamID != owner.TeamID)
            {
                var impact = ImpactType.None;
                if (!damageables.ContainsKey(damageable))
                {
                    damageables.Add(damageable, 0);
                }

                int currentCount = damageables[damageable];
                if(currentCount == needBullet_Ragdoll)
                {
                    impact = ImpactType.Ragdoll;
                }
                else if(currentCount == needBullet_Cancel)
                {
                    impact = ImpactType.Cancel;
                }
                damageables[damageable] += 1;
                damageable.TakeDamage(new DamageInfo(owner.TeamID, owner.ID, damageType, impact, p.transform.forward * impactForce, damage));
            }
        }
    }

    public void ProjectileOnHit(Dueler_Mono owner, ProjectileBase_Mono projectile, RaycastHit hit)
    {
        DealDamage(owner, projectile, hit);
    }
}

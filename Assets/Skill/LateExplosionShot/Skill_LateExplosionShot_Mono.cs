using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_LateExplosionShot_Mono : Skill_Base_Mono
{
    [SerializeField] RPC_Activate_Mono rpc_activate;
    [SerializeField] RPC_AimPosDir_Mono rpc_posDir;

    [SerializeField] AimShotRigidityAction shotAction = new AimShotRigidityAction(
            new Shot(
                new ProjectileDischarger(),
                new PosDegNoChange()
        )
    );

    [SerializeField] AreaDamage_Mono explosionPrefab;
    [SerializeField] float radius;

    ProjectileBase_Mono currentProjectile;

    bool NewShot => currentProjectile == null;

    public override bool CanUse => NewShot ? base.CanUse : true;

    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        shotAction.Init(owner, new ProjectileOnHitDummy());
        rpc_posDir.Init(owner, shotAction, shotAction.StartShot);
        rpc_activate.Init(Explosion);
        base.Init(owner, so, skillSlotID, input);
        if(shotAction.Shot.Discharger is ProjectileDischarger pDischarger)
        {
            pDischarger.AddShotCallbacks(OnShot);
        }
    }

    protected override void Activated()
    {
        rpc_posDir.ActivateRPC();
    }

    void OnShot(ProjectileBase_Mono p)
    {
        currentProjectile = p;
    }

    protected void Explosion(PhotonMessageInfo info)
    {
        if(currentProjectile != null)
        {
            var e = Instantiate(explosionPrefab, currentProjectile.transform.position, Quaternion.LookRotation(Vector3.up));
            e.Init(owner, radius);
            Destroy(currentProjectile.gameObject);
        }
    }

    public override void ButtonDown()
    {
        if (NewShot)
        {
            TryActivate();
        }
        else
        {
            rpc_activate.ActivateRPC();
        }
    }
}

public class ProjectileOnHitDummy : IProjectileOnHit
{
    public void ProjectileOnHit(Dueler_Mono owner, ProjectileBase_Mono projectile, RaycastHit hit)
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DischargerAntiAirBullet : DischargerBaseMono<HomingBullet_Mono>
{
    [SerializeField] HomingBullet_Mono boosetd_bulletPrefab;
    [SerializeField] int boosted_delay = 50;
    [SerializeField] float boosted_damage = 10f;
    [SerializeField] float boosted_lifeTime = 1f;
    [SerializeField] float boosted_shotSpeed = 10f;
    [SerializeField] float boosted_homing = 5f;
    [SerializeField] float boosted_angularDrag = 10f;


    [SerializeField] float damage = 10f;
    [SerializeField] float lifeTime = 1f;
    [SerializeField] float homing = 5f;
    [SerializeField] float angularDrag = 10f;

    public override void Shot(Dueler_Mono owner, Vector3 origin, Vector3 direction, int syncId, ITargetable target, int timestamp, PhotonMessageInfo info)
    {
        //Instantiate(prefab).Init(timestamp, owner.TeamID, owner.ID, target, syncId, damage, origin, direction, shotSpeed, homing, angularDrag, lifeTime);
    }


    /*
    public void ShotAntiAir(Vector3 origin, float angleX, float angleY)
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(ShotAntiAirRPC), RpcTarget.All, origin, angleX, angleY, targetID, 0);

        }
    }

    [PunRPC]
    protected void ShotAntiAirRPC(Vector3 origin, float angleX, float angleY, int targetID, int startProjectileId, PhotonMessageInfo info)
    {
        var target = TargetableManger.GetTargetable(targetID);
        //Debug.Log(owner);
        var rot = Quaternion.Euler(angleX, angleY, 0f);
        for (int n = 0; n < shotDirections.Length; ++n)
        {
            var bullet = Instantiate(boosetd_bulletPrefab);
            var timestamp = unchecked(info.SentServerTimestamp + boosted_delay);
            bullet.Init(timestamp, owner.TeamID, owner.ID, target, startProjectileId, boosted_damage, origin + rot * shotDirections[n].normalized * shotOffsetLength, rot * shotDirections[n], boosted_shotSpeed, boosted_homing, boosted_angularDrag, boosted_lifeTime);
            Destroy(bullet.gameObject, boosted_lifeTime + (boosted_delay * 0.001f));
        }
    }
    */
}

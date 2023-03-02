using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Skill_PoisonGranade_Mono : Skill_Base_Mono
{
    [SerializeField] RPC_PosDir_Mono rpc_shot;

    [SerializeField] AimShotRigidityAction action = new AimShotRigidityAction(
            new Shot(
                new GranadeDischarger(
                    new PoisonGasEmitter()
                    ),
                new PosDegModifier(
                    new PosNoChange(),
                    new DegreeOffset()
                    )
            )
        );

    [SerializeField] DegreeArray deg = new DegreeArray();
    [SerializeField] Vector3[] test;
    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        action.Init(owner, new ProjectileOnHitDummy());
        rpc_shot.Init(owner, action.StartShot);
        base.Init(owner, so, skillSlotID, input);
    }

    protected override void Activated()
    {
        rpc_shot.ActivateRPC();
    }
}

[System.Serializable]
public class GranadeDischarger : IDischarger
{
    Dueler_Mono owner;
    [SerializeField] Granade_Mono granadePrefab;
    [SerializeField] float force = 30f;
    [SerializeField] float gravityForce = 1f;
    [SerializeField] float mass = 1f;
    [SerializeField] float drag = 0f;

    [SerializeReference] IGranadeOnLand granadeOnLand;

    public GranadeDischarger(IGranadeOnLand granadeOnLand)
    {
        this.granadeOnLand = granadeOnLand;
    }

    public int DelayInMilliSec => 0;
    public float ShotSpeed => force;

    public void Init(Dueler_Mono owner, IProjectileOnHit[] onHitEffect)
    {
        this.owner = owner;
    }

    public void Shot(int timestamp, int syncId, Vector3 origin, Vector3 direction)
    {
        var projectile = GameObject.Instantiate(granadePrefab);
        projectile.Init(owner, origin, direction, force, granadeOnLand, gravityForce, mass, drag);

    }
}


[System.Serializable]
public class PoisonGasEmitter : IGranadeOnLand
{
    [SerializeField] IntervalAreaDamage_Mono gasPrefab;
    [SerializeField] float scale;


    public void OnLand(Dueler_Mono owner, GameObject granade)
    {
        
        var gas = GameObject.Instantiate(gasPrefab, granade.transform.position, Quaternion.identity);
        gas.Init(owner, scale);
        GameObject.Destroy(granade, gas.LifeTime);
    }
}

public interface IGranadeOnLand
{
    void OnLand(Dueler_Mono owner, GameObject granade);
}


[System.Serializable]
public class DetectOverlapSphere
{
    [SerializeField] LayerMask checkMask;
    float radius;

    System.Action<Vector3, Collider> OnHitAction;

    public virtual void Init(System.Action<Vector3, Collider> onHitAction, float radius)
    {
        OnHitAction = onHitAction;
        this.radius = radius;
    }

    public void Check(Vector3 pos)
    {
        var colliders = Physics.OverlapSphere(pos, radius, checkMask);

        for (int i = 0; i < colliders.Length; ++i)
        {
            OnHit(pos, colliders[i]);
        }
    }

    public virtual void OnHit(Vector3 pos, Collider target)
    {
        OnHitAction?.Invoke(pos, target);
    }
}

[System.Serializable]
public class IntervalDetectOverlapSphere : DetectOverlapSphere
{
    [SerializeField] float interval = .5f;
    Dictionary<Collider, int> damagedList = new Dictionary<Collider, int>();

    public override void Init(System.Action<Vector3, Collider> onHitAction, float radius)
    {
        base.Init(onHitAction, radius);
        damagedList.Clear();
    }

    public override void OnHit(Vector3 pos, Collider target)
    {
        if (!damagedList.ContainsKey(target))
        {
            damagedList.Add(target, PhotonNetwork.ServerTimestamp);
        }

        if(PhotonUtil3D.ElapsedTime(damagedList[target]) > interval)
        {
            base.OnHit(pos, target);
            damagedList[target] = PhotonNetwork.ServerTimestamp;
        }

    }

}

[System.Serializable]
public class DegreeOffset : IDegree
{
    [SerializeField] Vector3 offset;
    public Vector3 GetDegree(Dueler_Mono owner, float x, float y, int num, int total)
    {
        return new Vector3(x + offset.x, y + offset.y, 0f);
    }
}

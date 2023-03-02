using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase_Mono : MonoBehaviour
{
    protected Vector3 origin;
    protected Vector3 targetPos;
    [SerializeField] protected ProjectileHitCheck hitCheck;
    [SerializeField, HideInInspector] protected TimestampLifeTime timestamp;

    [SerializeField] ParticleEmitter particleEmitter;
    [SerializeField] TrailFadeOut trailFadeOut;
    protected Dueler_Mono owner;
    public float NormalizedTime => timestamp.NormalizedTime;
    protected IProjectileOnHit[] onHitEffect;

    public virtual void Init(Dueler_Mono owner, int timestamp, int projectileID, Vector3 origin, Vector3 direction, float lifeTime, float shotSpeed, IProjectileOnHit[] onHitEffect)
    {
        this.onHitEffect = onHitEffect;
        this.owner = owner;
        this.timestamp.Init(timestamp, lifeTime);
        this.origin = origin;
        transform.position = origin;
        hitCheck.Init(origin);
        transform.rotation = Quaternion.LookRotation(direction);



        targetPos = origin + lifeTime * shotSpeed * direction.normalized;
    }

    protected virtual void Update()
    {
        transform.position = Vector3.Lerp(origin, targetPos, NormalizedTime);
        hitCheck.Update(transform.position, OnHit);
        if (timestamp.IsOverLifeTime)
        {
            Destroy();
        }
    }

    protected virtual void Destroy()
    {
        Destroy(this.gameObject);
    }

    protected virtual void OnHit(RaycastHit hit)
    {
        foreach(var e in onHitEffect)
        {
            e.ProjectileOnHit(owner, this, hit);
        }
        trailFadeOut.FadeStart(owner);
        particleEmitter.EmitParticle(hit);
        Destroy();
    }

}

[System.Serializable]
public class ProjectileHitCheck
{
    [SerializeField] float radius;
    [SerializeField] LayerMask mask;
    Vector3 pastPos;
    public void Init(Vector3 pastPos)
    {
        this.pastPos = pastPos;
    }
    public void Update(Vector3 currentPos, System.Action<RaycastHit> hitAction)
    {
        var direction = currentPos - pastPos;
        if (Physics.SphereCast(pastPos, radius, direction, out RaycastHit hitInfo, direction.magnitude, mask))
        {
            hitAction?.Invoke(hitInfo);
        }

        pastPos = currentPos;
    }
}

[System.Serializable]
public class ParticleEmitter
{
    [SerializeField] float lifeTime = 1f;
    [SerializeField] ParticleSystem particlePrefab;

    public void EmitParticle(RaycastHit hitInfo)
    {
        if (particlePrefab != null)
        {
            GameObject.Destroy(
                GameObject.Instantiate(particlePrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)).gameObject,
                lifeTime
                );
        }
    }
}

[System.Serializable]
public class TrailFadeOut
{
    [SerializeField] float fadeTime = 1f;
    [SerializeField] TrailRenderer trail;

    public void FadeStart(MonoBehaviour mono)
    {
        if(trail != null)
        {
            trail.transform.SetParent(null);
            mono.StartCoroutine(CoFade());
        }
    }

    IEnumerator CoFade()
    {
        TimestampLifeTime timestamp = new TimestampLifeTime(PhotonNetwork.ServerTimestamp, fadeTime);
        //Color col = trail.material.GetColor("_TintColor");
        //float a = trail.material.GetFloat("_InvFade");
        //Color col = trail.material.color;
        yield return null;
        while (timestamp.NormalizedTime < 1f)
        {
            //col.a = 1f - timestamp.NormalizedTime;
            //trail.material.SetColor("_TintColor", col);
            //trail.material.color = col;
            trail.material.SetFloat("_InvFade", timestamp.NormalizedTime);

            yield return null;
        }
        GameObject.Destroy(trail.gameObject);
    }
}
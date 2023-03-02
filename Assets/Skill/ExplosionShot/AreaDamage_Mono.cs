using UnityEngine;

public class AreaDamage_Mono : AreaDamageBase_Mono
{
    [SerializeField] DamagerSelfInclude damager;
    [SerializeField] OnceDetectOverlapSphere onceDetectOverlapSphere;

    protected override Damager Damager => damager;

    protected override DetectOverlapSphere Detect => onceDetectOverlapSphere;
}


public abstract class AreaDamageBase_Mono : MonoBehaviour
{
    Dueler_Mono owner;
    [SerializeField] TimestampLifeTime timestamp;
    [SerializeField] float checkDuration = 1f;

    public float LifeTime => timestamp.LifeTime;
    protected abstract Damager Damager { get; }
    protected abstract DetectOverlapSphere Detect { get; }

    public void Init(Dueler_Mono owner, float scale = 1f)
    {
        this.owner = owner;
        Detect.Init(OnHit, scale);
        transform.localScale = Vector3.one * scale;
        timestamp.Init();
    }


    private void FixedUpdate()
    {
        if (checkDuration >= timestamp.TimeElapsed)
        {
            Detect.Check(transform.position);
        }
        if (timestamp.IsOverLifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnHit(Vector3 pos, Collider col)
    {
        if (col.TryGetComponent(out IDamageable dmaagable))
        {
            var direction = col.transform.position - pos;
            Damager.DealDamage(owner, direction, dmaagable);
        }
    }
}
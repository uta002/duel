using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer_Mono : MonoBehaviour
{
    int sourceTeamID;
    int sourceID;
    public DamageType damageType;
    public ImpactType impactType;
    float damage;
    public Vector3 Direction { get; set; }

    public float Damage { get => damage; set => damage = value; }

    public void Init(int sourceTeamID, int sourceID, float damage)
    {
        this.sourceTeamID = sourceTeamID;
        this.sourceID = sourceID;
        this.damage = damage;
    }

    public void DealDamage(RaycastHit hitInfo)
    {
        if (hitInfo.collider.TryGetComponent(out IDamageable damageable)) {
            DealDamage(damageable);
        }
    }

    public void DealDamage(IDamageable damageable)
    {
        DealDamage(damageable, Direction);
    }

    public void DealDamage(RaycastHit hitInfo, Vector3 direction)
    {
        if (hitInfo.collider.TryGetComponent(out IDamageable damageable))
        {
            DealDamage(damageable, direction);
        }
    }

    public void DealDamage(IDamageable damageable, Vector3 direction)
    {
        if(damageable.TeamID != sourceTeamID)
        {
            damageable.TakeDamage(new DamageInfo(sourceTeamID, sourceID, damageType, impactType, direction, damage));
        }
    }
}

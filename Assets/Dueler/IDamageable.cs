using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    int TeamID { get; }
    void TakeDamage(DamageInfo damageInfo);
}

[System.Serializable]
public class DamageInfo
{
    [HideInInspector] public int sourceTeamID;
    [HideInInspector] public int sourceID;
    public DamageType damageType;
    public ImpactType impactType;
    public float damage;
    public Vector3 direction;

    public DamageInfo(int sourceTeamID, int sourceID, DamageType damageType, ImpactType impactType, Vector3 direction, float damage)
    {
        this.sourceTeamID = sourceTeamID;
        this.sourceID = sourceID;
        this.damageType = damageType;
        this.impactType = impactType;
        this.damage = damage;
        this.direction = direction;
    }
}

public enum DamageType
{
    Melee,
    Range,
    AOE,
}

public enum ImpactType
{
    None,
    Cancel,
    Ragdoll,
}

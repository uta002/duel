using UnityEngine;

public class IntervalAreaDamage_Mono : AreaDamageBase_Mono
{
    [SerializeField] DamagerSelfInclude damager;
    [SerializeField] IntervalDetectOverlapSphere intervalDetectOverlapSphere;

    protected override Damager Damager => damager;

    protected override DetectOverlapSphere Detect => intervalDetectOverlapSphere;
}
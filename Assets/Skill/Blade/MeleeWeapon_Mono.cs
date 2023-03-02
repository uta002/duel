using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeleeWeapon_Mono : MeleeWeaponBase_Mono
{
    public UnityEvent OnCheckStart;
    public UnityEvent OnCheckEnd;
    [SerializeField] LayerMask mask;
    [SerializeField] Transform damagePivot;
    [SerializeField] Vector3 centerOffset;
    [SerializeField] Vector3 halfExtents;
    List<Collider> damagedList;
    Vector3 pastPos;


    public override void CheckStart()
    {
        damagedList = new List<Collider>();
        pastPos = transform.position;
        OnCheckStart?.Invoke();
    }

    public override void CheckEnd()
    {
        OnCheckEnd?.Invoke();
    }

    public override void CheckCollisions(Action<RaycastHit> hitEvent)
    {
        var direction = transform.position - pastPos;
        var overlap = Physics.BoxCastAll(damagePivot.position + centerOffset, halfExtents, direction, damagePivot.rotation, direction.magnitude, mask);
        for(int i = 0; i < overlap.Length; ++i)
        {
            var hit = overlap[i];
            if (!damagedList.Contains(hit.collider))
            {
                hitEvent?.Invoke(hit);
                damagedList.Add(hit.collider);
            }
        }
        pastPos = transform.position;
    }

}

public abstract class MeleeWeaponBase_Mono : MonoBehaviour
{
    public abstract void CheckStart();
    public abstract void CheckEnd();
    public abstract void CheckCollisions(Action<RaycastHit> hitEvent);
}

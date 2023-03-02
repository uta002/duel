using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile_Mono : MonoBehaviour
{
    [SerializeField] RaycastHitEvent raycastHitEvent = new RaycastHitEvent();
    [SerializeField] float radius;
    [SerializeField] LayerMask mask;
    Vector3 pastPos;
    private void Start()
    {
        pastPos = transform.position;
    }
    public void Update()
    {
        var direction = transform.position - pastPos;
        if(Physics.SphereCast(pastPos, radius, direction, out RaycastHit hitInfo, direction.magnitude, mask))
        {
            raycastHitEvent?.Invoke(hitInfo);
        }

        pastPos = transform.position;

    }
}

[System.Serializable]
public class RaycastHitEvent : UnityEvent<RaycastHit> { }

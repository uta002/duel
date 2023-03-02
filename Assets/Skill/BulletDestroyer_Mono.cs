using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestroyer_Mono : MonoBehaviour
{
    public float LifeTime = 2f;
    [SerializeField] GameObject trail;
    [SerializeField] ParticleSystem particlePrefab;

    public void OnHitSomething(RaycastHit hitInfo)
    {
        trail.transform.position = hitInfo.point;
        trail.transform.SetParent(null);

        if(particlePrefab != null)
        {
            Instantiate(particlePrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal), trail.transform);
        }

        Destroy(trail, LifeTime);
        Destroy(gameObject);
    }
}

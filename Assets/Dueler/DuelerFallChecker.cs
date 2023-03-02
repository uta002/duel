using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelerFallChecker : MonoBehaviour
{
    [SerializeField] Dueler_Mono owner;
    [SerializeField] float threshold = -10f;
    private void Update()
    {
        if(transform.position.y <= threshold)
        {
            owner.HealthSystem.Dead();
            owner.Dead(owner.transform.position, Random.insideUnitSphere * 40f);
        }
    }
}

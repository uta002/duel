using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetUI_Mono : MonoBehaviour
{
    public Dueler_Mono owner;

    TargetSystem_Mono TargetSystem => owner.TargetSystem;
    RectTransform rect;
    [SerializeField] Vector3 posOffset;
    private void Awake()
    {
        rect = transform as RectTransform;
    }
    void Update()
    {
        if(owner != null)
        {
            rect.position = RectTransformUtility.WorldToScreenPoint(owner.Camera, owner.AimPos);
        }
    }
}

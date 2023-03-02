using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelerRotation_Mono : MonoBehaviour
{
    [SerializeField] float interpolationRate;
    public void Rotate(Dueler_Mono owner)
    {
        owner.Root.rotation = Quaternion.Lerp(owner.Root.rotation, Quaternion.Euler(0f, owner.CameraPiv.eulerAngles.y, 0f), interpolationRate);
    }
}

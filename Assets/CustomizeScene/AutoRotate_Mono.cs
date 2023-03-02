using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate_Mono : MonoBehaviour
{
    public Transform target;
    public float rotateSpeed = 1f;


    void Update()
    {
        target.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}

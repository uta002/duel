using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    Animator anim;
    Rigidbody[] rbs;
    Collider[] cols;

    [SerializeField] Rigidbody chestRb;
    void Awake()
    {
        anim = GetComponent<Animator>();
        rbs = GetComponentsInChildren<Rigidbody>();
        cols = GetComponentsInChildren<Collider>();
    }

    private void Start()
    {
        DisableRagdoll();
    }


    public void SetRagdoll(bool value)
    {
        foreach(Rigidbody rb in rbs)
        {
            rb.isKinematic = !value;
        }
        foreach(Collider col in cols)
        {
            col.enabled = value;
        }
        anim.enabled = !value;
    }

    public void Push(Vector3 direction)
    {
        chestRb.AddForce(direction, ForceMode.Impulse);
        //chestRb.velocity = direction;
    }

    public void EnableRagdoll()
    {
        SetRagdoll(true);
        Push(chestRb.rotation * new Vector3(0f, 10f, 200f));
    }

    public void DisableRagdoll()
    {
        SetRagdoll(false);
    }

}

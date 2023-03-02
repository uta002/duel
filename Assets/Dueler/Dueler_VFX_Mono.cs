using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dueler_VFX_Mono : MonoBehaviour
{
    public Animator Animator => anim;
    [SerializeField] Animator anim;

    public Transform Hand_L => hand_L;
    [SerializeField] Transform hand_L;

    public Transform Hand_R => hand_R;
    [SerializeField] Transform hand_R;

    public Transform Pelvis => pelvis;
    [SerializeField] Transform pelvis;

    public Transform Armature => armature;
    [SerializeField] Transform armature;

    public RagdollController RagdollController => ragdollController;
    [SerializeField] RagdollController ragdollController;

}

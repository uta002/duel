using UnityEngine;

public class DuelerAnimation_Mono : MonoBehaviour
{
    [SerializeField] Dueler_Mono owner;
    [SerializeField] float moveSpeedNormalizer;

    Rigidbody Rb => owner.Rb;
    Animator Anim => owner.Animator;
    private void Update()
    {
        Anim?.SetFloat("MoveSpeed", Rb.velocity.magnitude * moveSpeedNormalizer);
    }
}

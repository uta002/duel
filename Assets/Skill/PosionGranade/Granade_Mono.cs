using UnityEngine;

public class Granade_Mono : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    Dueler_Mono owner;
    float gravityForce;
    IGranadeOnLand[] onLands;
    bool onLand;
    public void Init(Dueler_Mono owner, Vector3 pos, Vector3 direction, float force, IGranadeOnLand onLand, float gravityForce = 1f, float mass = 1f, float drag = 0f)
    {
        Init(owner, pos, direction, force, new IGranadeOnLand[] { onLand }, gravityForce, mass, drag);
    }

    public void Init(Dueler_Mono owner, Vector3 pos, Vector3 direction, float force, IGranadeOnLand[] onLands, float gravityForce = 1f, float mass = 1f, float drag = 0f)
    {
        this.owner = owner;
        this.gravityForce = gravityForce;
        this.onLands = onLands;

        transform.position = pos;

        rb.useGravity = false;
        rb.mass = mass;
        rb.drag = drag;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.AddForce(direction.normalized * force, ForceMode.Impulse);

        onLand = false;
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * gravityForce);


    }

    private void OnCollisionStay(Collision collision)
    {
        if (!onLand && rb.velocity.magnitude < 0.1f)
        {
            foreach (var e in onLands)
            {
                e.OnLand(owner, this.gameObject);
            }
            onLand = true;
        }
    }


}

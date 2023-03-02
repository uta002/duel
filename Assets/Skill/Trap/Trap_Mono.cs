using UnityEngine;

public class Trap_Mono : MonoBehaviour
{
    Trap trap;
    [SerializeField] Animator animator;

    public Trap Trap => trap;
    public void Init(Trap trap)
    {
        this.trap = trap;
        trap.OnReady += OnReady;
        trap.OnActivate += OnActivate;
        trap.OnDestroy += MyOnDestroy;

        animator.Play("Default");
    }

    void OnReady()
    {
        animator.Play("OnReady");
    }

    void OnActivate()
    {
        animator.Play("OnActivate");
        Destroy(gameObject, 1f);
    }

    private void MyOnDestroy()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        trap.OnCollision(other);
    }

    private void Update()
    {
        trap.Update();
    }
}

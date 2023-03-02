using UnityEngine;

public class HorizontalTurret_Mono : MonoBehaviour
{
    [SerializeField] Transform muzzle;
    HorizontalTurret horiTurret;
    bool run = false;
    public bool Running => run;

    public void Init(Dueler_Mono owner, HorizontalTurret horiTurret, int timestamp)
    {
        horiTurret.Init(owner, timestamp, muzzle.eulerAngles.y, muzzle.position);
        this.horiTurret = horiTurret;
        run = true;
    }

    private void Update()
    {
        if (!run)
            return;

        horiTurret.Update();

        if (horiTurret.IsOverLifeTime)
        {
            Destroy(this.gameObject);
        }
    }
}

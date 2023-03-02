using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Trap_Mono : Skill_Base_Mono, ITrapOnHit
{
    [SerializeField] RPC_PosAngleYSyncID_Mono rpc_placeTrap;
    [SerializeField] int maxTrapNum;
    [SerializeField] Trap_Mono trapPrafab;
    [SerializeField] DuelerState_Place state_Place;
    [SerializeField] float placeDistance;
    [SerializeField] float snareDuration;
    [SerializeField] float trapLifeTime;
    [SerializeField] float trapActivateDelay;

    [SerializeField] Damager damager;


    public override bool CanUse => base.CanUse && owner.IsGround;

    List<Trap_Mono> trapList = new List<Trap_Mono>();

    public override void Init(Dueler_Mono owner, Skill_Base_SO so, int skillSlotID, IDuelerInput input)
    {
        rpc_placeTrap.Init(owner, AfterRPCAction);
        //state_Place.Init(OnEndPut);
        base.Init(owner, so, skillSlotID, input);
        trapList.Clear();
    }
    protected override void Activated()
    {
        rpc_placeTrap.ActivateRPC();
    }

    protected void AfterRPCAction(Vector3 pos, float angleY, int syncID, PhotonMessageInfo info)
    {

        state_Place.Init(d => OnEndPlace(angleY, syncID, info));
        owner.ChangeState(state_Place);
    }

    void OnEndPlace(float angleY, int syncID, PhotonMessageInfo info)
    {
        var rot = Quaternion.Euler(0f, angleY, 0f);
        var instantiatePos = owner.Root.position + rot * Vector3.forward * placeDistance;
        var trap = Instantiate(trapPrafab, instantiatePos, rot);
        int placedTime = unchecked(info.SentServerTimestamp + state_Place.Delay);
        int activateTimestamp = unchecked( placedTime + PhotonUtil3D.Sec2MilliSec(trapActivateDelay));
        trap.Init(new Trap(owner, trap.gameObject, trapLifeTime, placedTime, activateTimestamp, new ITrapOnHit[] { this }));
        trapList.Add(trap);

        RemoveOldest();
    }

    void RemoveOldest()
    {
        if(trapList.Count > maxTrapNum)
        {
            trapList.RemoveAll(t => t == null);
            //trapList.Sort((a, b) => a.Trap.PlacedTime - b.Trap.PlacedTime);
            Destroy(trapList[0].gameObject);
        }
    }

    public void OnHit(GameObject trap, Dueler_Mono target)
    {
        target.AddStatus(new StatusSnare(owner, snareDuration));
        damager.DealDamage(owner, Vector3.zero, target);
    }
}

public interface ITrapOnHit
{
    void OnHit(GameObject trap, Dueler_Mono target);
}

public class Trap
{
    int placedTime;
    int activateTimestamp;
    Dueler_Mono owner;
    bool ready;
    bool enable;
    TimestampLifeTime timestampLife;
    GameObject trap;
    ITrapOnHit[] trapOnHits;

    public int PlacedTime => placedTime;

    public System.Action OnReady;
    public System.Action OnActivate;
    public System.Action OnDestroy;
    public Trap(Dueler_Mono owner, GameObject trap, float trapLifeTIme, int placedTime, int activateTimestamp, ITrapOnHit[] trapOnHits)
    {
        this.owner = owner;
        this.trap = trap;
        this.placedTime = placedTime;
        this.activateTimestamp = activateTimestamp;
        timestampLife = new TimestampLifeTime(activateTimestamp, trapLifeTIme);
        this.trapOnHits = trapOnHits;
        ready = false;
        enable = true;

    }

    public void OnCollision(Collider other)
    {
        if (enable && ready)
        {
            if(other.TryGetComponent(out Dueler_Mono target))
            {
                if(target.TeamID != owner.TeamID)
                {
                    Activate(target);
                }
            }
        }
    }

    public void Activate(Dueler_Mono target)
    {
        if(ready)
        {
            foreach(var e in trapOnHits)
            {
                e.OnHit(trap, target);
            }

            OnActivate?.Invoke();
            enable = false;
        }
    }

    public void Update()
    {
        if(!ready && PhotonNetwork.ServerTimestamp >= activateTimestamp)
        {
            OnReady?.Invoke();
            ready = true;
        }

        if (timestampLife.IsOverLifeTime)
        {
            OnDestroy?.Invoke();
        }
    }
}

public class StatusSnare : StatusDuration
{
    public StatusSnare(object source, float duration) : base(source, duration)
    {
    }

    public override void OnAdd(Dueler_Mono owner)
    {
        base.OnAdd(owner);
        owner.Rb.velocity = Vector3.zero;
        owner.Rb.isKinematic = true;
    }

    public override void OnRemove(Dueler_Mono owner)
    {
        base.OnRemove(owner);
        owner.Rb.isKinematic = false;
    }
}

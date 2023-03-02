using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

public class Dueler_Mono : MonoBehaviourPunCallbacks, IDamageable, ITargetable
{
    public int TeamID => teamID;
    [SerializeField] int teamID;
    public string PlayerName { get; set; }

    public int ID => photonView.ViewID;
    public Rigidbody Rb => rb;
    [SerializeField] Rigidbody rb;

    [SerializeField] Dueler_VFX_Mono dueler_VFX;
    public Dueler_VFX_Mono Dueler_VFX { get => dueler_VFX; set => dueler_VFX = value; }
    public Animator Animator => dueler_VFX.Animator;
    public Transform Pelvis => dueler_VFX.Pelvis;
    public Transform HandPos_L => dueler_VFX.Hand_L;
    public Transform HandPos_R => dueler_VFX.Hand_R;
    public RagdollController RagdollController => dueler_VFX.RagdollController;
    public Transform Armature => dueler_VFX.Armature;

    List<IDuelerStatus> statusList = new List<IDuelerStatus>();


    public SphereCasterMono SphereCaster => sphereCaster;
    [SerializeField] SphereCasterMono sphereCaster;
    [SerializeField] HealthSystem_Mono healthSystem;
    public HealthSystem_Mono HealthSystem => healthSystem;
    public void SetMaxHealth(float maxHealth) => healthSystem.SetHealth(maxHealth);
    public IDuelerInput Input => input;
    IDuelerInput input;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    [SerializeField] float moveSpeed;

    public float Acceleration { get => acceleration; set => acceleration = value; }
    [SerializeField] float acceleration;

    public float WalkableDegree => walkableDegree;
    [SerializeField] float walkableDegree;

    public Vector3 GroundNormal => groundHitInfo.normal;
    public Vector3 WorldMoveDirection => cameraPivot.rotation * input.MoveInput;
    public Vector3 LoaclMoveDirection(Vector3 dir) => Quaternion.Inverse(root.rotation) * dir;
    public bool IsGround { get; set; }
    public void SetLastGroundTime() => lastGroundTime = PhotonNetwork.ServerTimestamp;
    int lastGroundTime;
    public float TimeElapsedFromLastGround => PhotonUtil3D.MilliSec2Sec(PhotonNetwork.ServerTimestamp - lastGroundTime);
    [SerializeField] float landStunTime = 1f;
    public float LandStunTime { get => landStunTime; set => landStunTime = value; }


    public float JumpVelY { get => jumpVelY; set => jumpVelY = value; }
    [SerializeField] float jumpVelY;
    public Cooldown JumpCooldown => jumpCooldown;
    [SerializeField] Cooldown jumpCooldown;
    public int MaxJumpCount { get => maxJumpCount; set => maxJumpCount = value; }
    [SerializeField] int maxJumpCount;

    public int CurrentJumpCount => currentJumpCount;
    int currentJumpCount;

    public PhotonRigidbodyView RigidbodyView => photonRigidbodyView;

    public bool JumpInput => input.JumpInput.ButtonDown;
    public bool DashInput => input.DashInput.ButtonDown;

    public float DashVel { get => dashVelocity; set => dashVelocity = value; }
    [SerializeField] float dashVelocity = 10f;
    [SerializeField] StackableSkill dashCooldown;
    public StackableSkill DashCooldown => dashCooldown;

    public float DashDuration { get => dashDuration; set => dashDuration = value; }
    [SerializeField] float dashDuration;
    public float DashStunTime { get => dashStunTime; set => dashStunTime = value; }
    [SerializeField] float dashStunTime;

    public Transform Root => root;
    [SerializeField] Transform root;

    [SerializeField] PhotonRigidbodyView photonRigidbodyView;
    public List<Skill_Base_Mono> Skills { get => skills; set => skills = value; }
    [SerializeField] List<Skill_Base_Mono> skills = new List<Skill_Base_Mono>();

    public float AngleY => CameraPiv.eulerAngles.y;
    public float AimDegreeX => cameraPivot.rotation.eulerAngles.x > 180f ? cameraPivot.rotation.eulerAngles.x - 360f : cameraPivot.rotation.eulerAngles.x;
    public Transform CameraPiv => cameraPivot;
    public Transform CameraPos => cameraPos;
    [SerializeField] Vector3 heartPosOffset;
    [SerializeField] CameraController cameraController;
    public Vector3 HeartPos => cameraPivot.position + heartPosOffset;
    public Vector3 ShotOrigin => HeartPos + (cameraPivot.forward * 0.75f);
    public Vector3 FuturePos(float time) => HeartPos + rb.velocity * time;
    public Vector3 CurrentPos => HeartPos;
    public Vector3 Velocity => velocity; 
    Vector3 velocity;
    Vector3 pastPos;
    public bool IsDead { get; set; }
    public bool IsSpotted => isSpotted;
    bool isSpotted = true;

    public TargetSystem_Mono TargetSystem => targetSystem;
    [SerializeField] TargetSystem_Mono targetSystem;

    public Vector3 AimPos => targetSystem.TargetExisting ? targetSystem.CurrentTarget.CurrentPos : DefaultAimPos;
    public Vector3 AimDirection => AimPos - HeartPos;
    public Vector3 DefaultAimPos{
        get{
            var defAimDistance = 100f;
            if(Physics.Raycast(cameraPos.position, cameraPos.forward, out RaycastHit hit, defAimDistance, layerMask))
            {
                return hit.point;
            }
            return cameraPivot.position + cameraPivot.forward * defAimDistance;
        }
    }
    public void SetRootRot() => SetRootRot(cameraPivot.eulerAngles.y);

    public void SetRootRot(float angleY) => Root.rotation = Quaternion.Euler(0f, angleY, 0f);
    public Camera Camera => camera;
    new Camera camera;

    [SerializeField] Transform cameraPivot;
    [SerializeField] Transform cameraPos;

    public UnityEvent OnDestroyEvent;

    [SerializeField] LayerMask layerMask;

    public bool GroundCheck() => sphereCaster.Check(out groundHitInfo);
    public RaycastHit GroundHitInfo => groundHitInfo;
    RaycastHit groundHitInfo;

    public float DamagedRigidityDuration = 0.5f;
    public float RagdollDuration = 2f;
    public float RagdollInvincibleDuration = 3f;

    public void SetDefaultState() => ChangeState(new DuelerStateDefault());

    //public bool DefaultOrAirState => currentState.GetType() == typeof(DuelerStateDefault) || currentState.GetType() == typeof(DuelerStateAir);
    public bool CanUseSkills => currentState.CanUseSkill;
    public bool HasSnare => HasStatus<StatusSnare>();
    public IDuelerState CurrentState => currentState;

    public int SyncId { get; internal set; }

    IDuelerState currentState;
    public void ChangeState(IDuelerState nextState)
    {
        currentState?.OnExit(this, nextState);
        var prev = currentState;
        currentState = nextState;
        currentState?.OnEnter(this, prev);
    }

    public void Init(int teamID, IDuelerInput input)
    {
        SetTeamID(teamID);
        SetInput(input);
    }

    public void SetTeamID(int teamID) => this.teamID = teamID;

    public void SetInput(IDuelerInput input)
    {
        this.input = input;
        cameraController.Init(input);
        for (int n = 0; n < skills.Count; n++)
        {
            if(skills[n] != null)
            {
                skills[n].SetInput(input, n);
            }
        }
    }

    public void SetCamera(Camera camera)
    {
        this.camera = camera;
        camera.transform.SetParent(cameraPos, false);
    }


    void Awake()
    {
        DuelerManager_Mono.AddDueler(this);
        TargetableManger.Add(this);
    }


    private void Start()
    {
        MyStart();
    }

    public void MyStart()
    {
        Rb.velocity = Vector3.zero;
        healthSystem.InitHealth();
        SetDefaultState();
        AimAnimOff();
        dashCooldown.ForceReady();
        jumpCooldown.ForceReady();
        IsDead = false;
    }


    void Update()
    {
        StatusUpdate();
        currentState?.Update(this);
        CheckSkills();

        
    }

    private void FixedUpdate()
    {
        CalcVelocity();
    }

    public void CalcVelocity()
    {
        velocity = (CurrentPos - pastPos) / Time.fixedDeltaTime;
        pastPos = CurrentPos;
    }

    public void AddStatus(IDuelerStatus newStatus)
    {
        statusList.Add(newStatus);
        newStatus.OnAdd(this);
    }

    void StatusUpdate()
    {
        for(int n=statusList.Count-1;n>=0;--n)
        {
            statusList[n]?.Update(this);
        }
    }
    public void RemoveStatus(IDuelerStatus removeStatus)
    {
        removeStatus.OnRemove(this);
        statusList.Remove(removeStatus);
    }
    public bool HasStatus<T>() where T : IDuelerStatus
    {
        foreach(var elem in statusList)
        {
            if(elem.GetType() == typeof(T))
            {
                return true;
            }
        }
        return false;
    }


    public void SetAnimatorAimDegreeXManual(float degreeX)
    {
        Animator.SetFloat("AimDegree", degreeX);
    }

    public void SetAnimatorAimDegreeX()
    {
        Animator.SetFloat("AimDegree", AimDegreeX);
    }
    public void AimAnimOn()
    {
        int aimLayer = 1;
        Animator.SetLayerWeight(aimLayer, 1f);
        Animator.SetTrigger("Aim");
    }

    public void AimAnimOff()
    {
        int aimLayer = 1;
        Animator.SetLayerWeight(aimLayer, 0f);
    }
    public void Move()
    {
        if (HasSnare)
            return;

        if (IsGround)
        {
            var moveVec = WorldMoveDirection;
            moveVec.y = 0f;
            Vector3 addForceVector = (moveVec.normalized * MoveSpeed - Rb.velocity) * Acceleration;

            addForceVector.y = 0f;
            if (GroundNormal.y >= Mathf.Cos(WalkableDegree * Mathf.Deg2Rad))
            {
                //Quaternion rotDiff = Quaternion.FromToRotation(Vector3.up, moveParams.GroundNormal);
                //addForceVector = rotDiff * addForceVector;


                addForceVector = Vector3.ProjectOnPlane(addForceVector, GroundNormal);
            }
            Rb.AddForce(addForceVector);
        }
    }

    public void CheckJump()
    {
        if (HasSnare)
            return;

        if (JumpInput && jumpCooldown.CanUse)
        {
            if (IsGround)
            {
                Jump();
                jumpCooldown.Use();
            }
            else
            {
                if (currentJumpCount > 0)
                {
                    Jump();
                    currentJumpCount--;
                    jumpCooldown.Use();
                }
            }
        }
    }
    public void Jump()
    {
        var velocity = Rb.velocity;
        velocity.y = JumpVelY;
        OnJump(transform.position, velocity);
    }

    [SerializeField] float interpolationRate;
    public void Rotate()
    {
        root.rotation = Quaternion.Lerp(root.rotation, Quaternion.Euler(0f, cameraPivot.eulerAngles.y, 0f), interpolationRate * Time.deltaTime);
    }

    public void CheckDash()
    {
        if (HasSnare)
            return;

        if (DashInput && dashCooldown.CanUse)
        {
            Dash();
            dashCooldown.Use();
        }
    }
    public void Dash()
    {
        var velocity = WorldMoveDirection;
        velocity = velocity == Vector3.zero ? CameraPiv.forward : velocity;
        velocity.y = 0f;
        velocity = velocity.normalized * DashVel;
        photonView.RPC(nameof(ShiftDashState), RpcTarget.All, transform.position, velocity);
    }

    public void CheckSkills()
    {
        if (photonView.IsMine)
        {
            for (int n = 0; n < skills.Count; ++n)
            {
                skills[n].MyUpdate();
            }
        }
    }

    [PunRPC]
    void ShiftDashState(Vector3 pos, Vector3 vel)
    {
        ChangeState(new DuelerStateDash(pos, vel, dashDuration));
    }

    public void RagdollTest()
    {
        if (photonView.IsMine)
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {
                photonView.RPC(nameof(RagdollOn), RpcTarget.All, transform.position, cameraPivot.forward * 15f);
            }
        }
    }

    public void PlayDashAnimation(Vector3 animDir)
    {
        Animator.SetTrigger("Dash");
        Animator.SetFloat("DashHorizontal", animDir.x);
        Animator.SetFloat("DashVertical", animDir.z);
    }

    public void PlayDashStopAnimation(Vector3 animDir)
    {
        animDir = animDir.normalized * -0.3f;
        Animator.SetTrigger("DashStop");
        Animator.SetFloat("DashHorizontal", animDir.x);
        Animator.SetFloat("DashVertical", animDir.z);
    }

    public void SetAnimDefault()
    {
        if (IsGround)
        {
            Animator.SetTrigger("Move");
        }
        else
        {
            Animator.SetTrigger("Air");
        }
    }

    public void SetState()
    {
        if (IsGround) 
        {
            ChangeState(new DuelerStateDefault());
        }
        else
        {
            ChangeState(new DuelerStateAir(0f));
        }
    }

    public void RecoverCurrentJumpCount() => currentJumpCount = maxJumpCount;
    public void OnLand()
    {
        RecoverCurrentJumpCount();
        photonView.RPC(nameof(ShiftLandState), RpcTarget.All, transform.position);
    }

    [PunRPC]
    void ShiftLandState(Vector3 pos, PhotonMessageInfo info)
    {
        transform.position = pos;
        ChangeState(new DuelerStateLand(landStunTime));
    }

    public void OnAirborne(Vector3 pos, Vector3 vel)
    {
        photonView.RPC(nameof(ShiftAirborneState), RpcTarget.All, pos, vel);
    }

    [PunRPC]
    void ShiftAirborneState(Vector3 pos, Vector3 vel)
    {
        transform.position = pos;
        rb.velocity = vel;
        ChangeState(new DuelerStateAir(0f));
    }

    public void OnJump(Vector3 pos, Vector3 vel)
    {
        photonView.RPC(nameof(ShiftJumpState), RpcTarget.All, pos, vel);
    }

    [PunRPC]
    void ShiftJumpState(Vector3 pos, Vector3 vel)
    {
        transform.position = pos;
        rb.velocity = vel;
        SetLastGroundTime();
        ChangeState(new DuelerStateAir(0.5f));
    }


    [PunRPC]
    public void DamagedRigidity(Vector3 pos)
    {
        rb.velocity = Vector3.zero;
        transform.position = pos;
        ChangeState(new DuelerStateDamaged(DamagedRigidityDuration));
    }


    [PunRPC]
    public void RagdollOn(Vector3 pos, Vector3 force)
    {
   
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        transform.position = pos;
        ChangeState(new DuelerStateRagdoll(force, RagdollDuration));
    }

    public void OnDead(Vector3 force)
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(Dead), RpcTarget.All, transform.position, force);
        }
    }
    [PunRPC]
    public void Dead(Vector3 pos, Vector3 force)
    {
        IsDead = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        transform.position = pos;
        ChangeState(new DuelerStateDead(force));
    }

    void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
        DuelerManager_Mono.RemoveDueler(this);
        TargetableManger.Remove(this);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (photonView.IsMine)
        {
            if (HasStatus<StatusNoDamage>())
            {
                return;
            }
            else if (HasStatus<StatusSuperArmor>())
            {
                healthSystem.TakeDamage(damageInfo);
            }
            else
            {
                healthSystem.TakeDamage(damageInfo);
                TakeImpact(damageInfo);
            }

            if(healthSystem.CurrentHealth == 0f)
            {
                OnDead(damageInfo.direction);
            }
        }

    }

    void TakeImpact(DamageInfo damageInfo)
    {
        switch (damageInfo.impactType)
        {
            case ImpactType.None:
                break;
            case ImpactType.Cancel:
                photonView.RPC(nameof(DamagedRigidity), RpcTarget.All, transform.position);
                break;
            case ImpactType.Ragdoll:
                photonView.RPC(nameof(RagdollOn), RpcTarget.All, transform.position, damageInfo.direction);
                break;
        }
    }
}

public interface IDuelerInput
{
    Vector2 LookInput { get; }
    Vector3 MoveInput { get; }
    IButtonState JumpInput { get; }
    IButtonState DashInput { get; }
    //IButtonState GetSkillInput(int num);
    IButtonState[] SkillInputs { get; }
}

public static class PhotonTimeUtil
{
    public static float GetElapsedTime(int timestamp) => Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);

}

public interface IDuelerState
{
    bool CanUseSkill { get; }
    void OnEnter(Dueler_Mono owner, IDuelerState prevState);
    void OnExit(Dueler_Mono owner, IDuelerState nextState);
    void Update(Dueler_Mono owner);
}


public abstract class DuelerStateDuration : IDuelerState
{
    [SerializeField] protected TimestampLifeTime timestamp;

    public virtual bool CanUseSkill => false;
    public int Delay => PhotonUtil3D.Sec2MilliSec(timestamp.LifeTime);
    public TimestampLifeTime TimeStamp => timestamp;
    public float TimeElapsed => timestamp.TimeElapsed;


    protected DuelerStateDuration(float duration)
    {
        timestamp = new TimestampLifeTime(PhotonNetwork.ServerTimestamp, duration);
    }

    public void OnEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        //timestamp = new TimestampLifeTime(PhotonNetwork.ServerTimestamp, duration);
        timestamp.Init(PhotonNetwork.ServerTimestamp);
        StateEnter(owner, prevState);
    }

    protected virtual void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {

    }

    public abstract void OnExit(Dueler_Mono owner, IDuelerState nextState);

    public void Update(Dueler_Mono owner)
    {
        if (timestamp.IsOverLifeTime)
        {
            TimeOver(owner);
            return;
        }
        StateUpdate(owner);
    }
    protected virtual void StateUpdate(Dueler_Mono owner) { }

    protected abstract void TimeOver(Dueler_Mono owner);
}

public abstract class DuelerStateEndAction : DuelerStateDuration
{
    protected Action<Dueler_Mono> endAction;
    public float Duration => timestamp.LifeTime;

    public void Init(Action<Dueler_Mono> endAction)
    {
        this.endAction = endAction;
    }

    public DuelerStateEndAction(float duration, Action<Dueler_Mono> endAction) : base(duration)
    {
        this.endAction = endAction;
    }


    protected override void TimeOver(Dueler_Mono owner)
    {
        endAction?.Invoke(owner);
    }
}

public class DuelerStateDefault : IDuelerState
{
    public virtual bool CanUseSkill => true;

    public virtual void OnEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.Animator.SetTrigger("Move");
        SetPhysics(owner);
        //Debug.DrawLine(owner.)
    }

    protected void SetPhysics(Dueler_Mono owner)
    {
        owner.Rb.isKinematic = false;

        owner.Rb.useGravity = true;
        owner.RigidbodyView.enabled = true;
    }

    public virtual void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
    }

    public virtual void Update(Dueler_Mono owner)
    {
        owner.Rotate();
        if (owner.photonView.IsMine)
        {
            owner.Move();
            owner.CheckJump();
            owner.CheckDash();

            if (!owner.GroundCheck())
            {
                owner.SetLastGroundTime();
                owner.OnAirborne(owner.transform.position, owner.Rb.velocity);
            }
        }
    }
}

public class DuelerStateLand : DuelerStateDuration
{
    public DuelerStateLand(float duration) : base(duration)
    {
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.IsGround = true;

        owner.Rb.isKinematic = true;
        owner.Rb.velocity = Vector3.zero;
        owner.RigidbodyView.enabled = false;
        owner.Animator.SetTrigger("Land");

    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        owner.Rb.isKinematic = false;
    }

    protected override void StateUpdate(Dueler_Mono owner)
    {
        owner.CheckDash();
    }

    protected override void TimeOver(Dueler_Mono owner)
    {
        owner.SetDefaultState();
    }
}

public class DuelerStateAir : IDuelerState
{
    Timestamp timestamp;
    float landIgnoreTime;
    public float TimeElapsed => timestamp.TimeElapsed;

    public DuelerStateAir(float landIgnoreTime)
    {
        this.landIgnoreTime = landIgnoreTime;
        timestamp = new Timestamp(PhotonNetwork.ServerTimestamp);
    }

    public bool CanUseSkill => true;

    public void OnEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.IsGround = false;
        owner.Rb.useGravity = true;
        owner.RigidbodyView.enabled = false;
        owner.Rb.isKinematic = false;
        owner.Animator.SetTrigger("Air");

        
    }

    public void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
    }

    public void Update(Dueler_Mono owner)
    {
        owner.Rotate();
        //owner.CheckDash();
        if (owner.photonView.IsMine)
        {
            owner.CheckJump();
            owner.CheckDash();

            if (timestamp.TimeElapsed >= landIgnoreTime)
            {
                if (owner.GroundCheck())
                {
                    owner.OnLand();
                }
            }
        }
    }

}

public class DuelerStateDash : DuelerStateDuration
{
    Vector3 pos;
    Vector3 vel;
    Vector3 animParam;
    IDuelerState prevState;
    public DuelerStateDash(Vector3 pos, Vector3 vel, float duration) : base(duration)
    {
        this.pos = pos;
        this.vel = vel;
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        this.prevState = prevState;
        owner.transform.position = pos;
        owner.Rb.velocity = vel;

        owner.SetRootRot();
        animParam = owner.LoaclMoveDirection(vel);
        owner.PlayDashAnimation(animParam.normalized);


        owner.Rb.useGravity = false;
        owner.RigidbodyView.enabled = false;


    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        owner.Rb.velocity = Vector3.zero;
        owner.RigidbodyView.enabled = true;
        owner.PlayDashStopAnimation(animParam);
    }

    protected override void StateUpdate(Dueler_Mono owner)
    {

    }

    protected override void TimeOver(Dueler_Mono owner)
    {
        owner.ChangeState(new DuelerStateRigidity(null, null, owner.DashStunTime));
    }
}

[System.Serializable]
public class DuelerStateRigidity : DuelerStateDuration
{
    System.Action enterAction;
    System.Action exitAction;
    public void Init(System.Action enterAction, System.Action exitAction)
    {
        this.enterAction = enterAction;
        this.exitAction = exitAction;
    }

    public DuelerStateRigidity(System.Action onEnterAction, System.Action onExitAction, float duration) : base(duration)
    {
        enterAction = onEnterAction;
        this.exitAction = onExitAction;
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        enterAction?.Invoke();
        owner.Rb.isKinematic = true;

    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        owner.Rb.isKinematic = false;

        exitAction?.Invoke();
    }

    protected override void StateUpdate(Dueler_Mono owner)
    {
        owner.CheckDash();
    }

    protected override void TimeOver(Dueler_Mono owner)
    {
        owner.SetState();
    }
}

public class DuelerStateDamaged : DuelerStateDuration
{
    public DuelerStateDamaged(float duration) : base(duration)
    {

    }
    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        owner.Rb.isKinematic = true;
        owner.Animator.SetTrigger("Damaged");

    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
    }

    protected override void TimeOver(Dueler_Mono owner)
    {
        owner.SetState();
    }
}

public class DuelerStateRagdoll : DuelerStateDuration
{
    Vector3 force;
    public DuelerStateRagdoll(Vector3 force, float duration) : base(duration)
    {
        this.force = force;
    }

    protected override void StateEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        //Debug.Log(owner.RagdollController);
        owner.RagdollController.SetRagdoll(true);
        owner.RagdollController.Push(force);
        owner.Pelvis.SetParent(null);

        owner.AddStatus(new StatusNoDamage(this, owner.RagdollInvincibleDuration));
    }

    protected override void StateUpdate(Dueler_Mono owner)
    {
        owner.transform.position = owner.Pelvis.position;
    }

    public override void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        owner.RagdollController.SetRagdoll(false);
        owner.transform.position = owner.Pelvis.position + Vector3.up * 0.8f;
        //owner.transform.position = owner.GroundHitInfo.point;

        owner.Pelvis.SetParent(owner.Armature);
    }

    protected override void TimeOver(Dueler_Mono owner)
    {
        //owner.SetDefaultState();
        owner.SetState();
    }
}

public class DuelerStateDead : IDuelerState
{
    Vector3 force;

    public bool CanUseSkill => false;

    public DuelerStateDead(Vector3 force)
    {
        this.force = force;
    }

    public void OnEnter(Dueler_Mono owner, IDuelerState prevState)
    {
        //Debug.Log(owner.RagdollController);
        owner.RagdollController.SetRagdoll(true);
        owner.RagdollController.Push(force);
        owner.Pelvis.SetParent(null);
    }

    public void Update(Dueler_Mono owner)
    {
        owner.transform.position = owner.Pelvis.position;
    }

    public void OnExit(Dueler_Mono owner, IDuelerState nextState)
    {
        owner.RagdollController.SetRagdoll(false);
        owner.transform.position = owner.Pelvis.position + Vector3.up * 0.8f;

        owner.Pelvis.SetParent(owner.Armature);
    }
}
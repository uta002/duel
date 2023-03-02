using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Dueler", menuName = "Dueler")]
public class DuelerStatus_SO : ScriptableObject
{
    public string DuelerName => name;
    public Sprite icon;
    public Dueler_VFX_Mono VFX;

    public float MaxHealth = 100f;
    public float MoveSpeed = 3f;
    public float Acceleration = 20f;
    public float JumpVelY = 10f;
    public float JumpCooldown = 1f;
    public float LandStunTime = 1f;
    public int MaxJumpCount = 0;
    public float DashVelocity = 20f;
    public int DashCount = 3;
    public float DashStackCoolDown = 6f;
    public float DashUseCoolDown = 1f;
    public float DashDuration = 0.3f;
    public float DashStunTime = 0.5f;


    public void SetDueler(Dueler_Mono target)
    {
        target.SetMaxHealth(MaxHealth);
        target.MoveSpeed = MoveSpeed;
        target.Acceleration = Acceleration;
        target.JumpVelY = JumpVelY;
        target.JumpCooldown.CoolDownTime = JumpCooldown;
        target.LandStunTime = LandStunTime;
        target.MaxJumpCount = MaxJumpCount;
        target.DashVel = DashVelocity;
        target.DashCooldown.Init(DashCount, DashUseCoolDown, DashStackCoolDown);
        target.DashDuration = DashDuration;
        target.DashStunTime = DashStunTime;
    }
}

// yUnityz—ñ‹“Œ^‚Éƒrƒbƒg‰‰Z‚ğ—p‚¢‚Ä“¯‚Éƒtƒ‰ƒO‚ğ‚½‚Ä‚ê‚é‚æ‚¤‚É‚·‚é https://www.hanachiru-blog.com/entry/2019/10/03/230000
/*
[System.Flags]
public enum SkillType
{
    Melee = 1<<0,
    Range = 1<<1,
    Trap = 1<<2,
    Block = 1<<3,
}
*/
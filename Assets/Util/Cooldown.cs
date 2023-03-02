using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cooldown : ICooldown
{
    public float CoolDownTime { get => coolDownTime; set => coolDownTime = value; }
    [SerializeField] float coolDownTime;

    float startTime;
    float Time => UnityEngine.Time.time;

    public Cooldown(float coolDownTime)
    {
        this.coolDownTime = coolDownTime;
    }

    public Cooldown()
    {
    }

    public virtual bool CanUse => (startTime + coolDownTime) < Time;
    public float ElapsedTime => Time - startTime;
    public float CoolDownTimeNormalized => Mathf.Clamp01(ElapsedTime / coolDownTime);

    public void Use()
    {
        if (CanUse)
        {
            SetStartTime();
        }
    }

    public void ForceCountStart()
    {
        SetStartTime();
    }

    protected virtual void SetStartTime() => startTime = Time;

    public virtual void ForceReady()
    {
        startTime = Time - CoolDownTime;
    }
}
public interface ICooldown
{
    bool CanUse { get; }
    void Use();
}


[System.Serializable]
public class StackableSkill
{
    [SerializeField] int maxStackCount;
    [SerializeField] Cooldown useCooldown;
    [SerializeField] Cooldown stackCooldown;
    public void Init(int maxStackCount, float useCooldown, float stackCooldown)
    {
        this.maxStackCount = maxStackCount;
        this.useCooldown.CoolDownTime = useCooldown;
        this.stackCooldown.CoolDownTime = stackCooldown;
    }

    public void ForceReady()
    {
        stackCount = maxStackCount;
        useCooldown.ForceReady();
        stackCooldown.ForceReady();
    }

    public int StackCount
    {
        get
        {
            if (stackCount < maxStackCount)
            {
                if (stackCooldown.CanUse)
                {
                    stackCount = Mathf.Min(++stackCount, maxStackCount);
                    //stackCount += 1;
                    stackCooldown.Use();
                }
            }
            return stackCount;
        }
    }
    //public float StackCoolDownNormalized => stackCount == maxStackCount ? 1f : stackCooldown.CoolDownTimeNormalized;
    public float StackCoolDownNormalized => stackCooldown.CoolDownTimeNormalized;
    public float UseCoolDownNormalized => useCooldown.CoolDownTimeNormalized;
    int stackCount;
    public void Use()
    {
        if (CanUse)
        {
            useCooldown.Use();
            if(StackCount == maxStackCount)
            {
                stackCooldown.ForceCountStart();
            }
            stackCount -= 1;
        }
    }

    public bool CanUse => StackCount > 0 && useCooldown.CanUse;
}

[System.Serializable]
public class StackableCooldown : ICooldown
{
    [SerializeField] float useCoolDownTime;
    [SerializeField] float stackCoolDownTime;
    [SerializeField] int maxStackCount;
    float startTime;
    int stackCount;
    float stackAdditionalTime;
    public int StackCount => stackCount + (int)(ElapsedTime + stackAdditionalTime / stackCoolDownTime);
    public bool CanUse => ElapsedTime >= useCoolDownTime && StackCount > 0;
    public float ElapsedTime => Time.time - startTime;
    public float CoolDownTimeNormalized => ElapsedTime / useCoolDownTime;

    public float StackCoolDownTimeNormalized => ElapsedTime / stackCoolDownTime;

    public void Use()
    {
        if (CanUse)
        {
            ForceCountStart();
        }
    }

    public void ForceCountStart()
    {
        stackAdditionalTime = ElapsedTime % stackCoolDownTime;
        stackCount = (int)(ElapsedTime / stackCoolDownTime) - 1;
        startTime = Time.time;
    }
}

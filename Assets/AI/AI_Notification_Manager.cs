using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AI_Notification_Manager
{
    // public static List<Blade_Notification> blade_Notifications = new List<Blade_Notification>();

}


public static class MySubject<Data>
{
    public static List<IMyObservable<Data>> Observers = new List<IMyObservable<Data>>();

    public static void AddObserver(IMyObservable<Data> observer)
    {
            Observers.Add(observer);
        
    }

    public static void RemoveObserver(IMyObservable<Data> observer)
    {
        Observers.Remove(observer);
    }

    internal static void Notify(Data data)
    {
        foreach (var e in Observers)
        {
            e.Notify(data);
        }
    }
}


public interface IMyObservable<Data>
{
    void Notify(Data data);
}

public class AI_Notification_Base
{
    float activatedTime;
    Dueler_Mono source;
    ITargetable target;
}

public class Blade_Notification
{
    public int SkillId;
    public int ActivatedTime;
    public Dueler_Mono Source;
    public ITargetable Target;
    public int ReachTime;
    public float Damage;
    public Vector3 StartPos;
    public Vector3 Direction;
    public float MaxDistance;
    public bool Parryable;

    public Blade_Notification(int skillId, int activatedTime, Dueler_Mono source, ITargetable target, DuelerStatePreBlade preBladeState, float damage, Vector3 startPos, Vector3 direction, bool parryable)
    {
        SkillId = skillId;
        ActivatedTime = activatedTime;
        Source = source;
        Target = target;
        ReachTime = activatedTime + PhotonUtil3D.Sec2MilliSec(direction.magnitude / preBladeState.MoveSpeed);
        Damage = damage;
        StartPos = startPos;
        Direction = direction;
        MaxDistance = preBladeState.Duration * preBladeState.MoveSpeed;
        Parryable = parryable;
    }
}
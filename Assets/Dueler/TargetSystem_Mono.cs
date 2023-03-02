using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSystem_Mono : MonoBehaviour
{
    [SerializeField] Dueler_Mono owner;
    [SerializeField] float maxAngle = 45f;
    [SerializeField] LayerMask checkMask;

    public ITargetable CurrentTarget => currentTarget;
    ITargetable currentTarget;

    public bool TargetExisting => currentTarget != null;

    void Update()
    {

        float minDegree = float.MaxValue;
        currentTarget = null;
        foreach(var elem in TargetableManger.AllTargetables)
        {
            if(elem != null)
            {
                if (elem.IsDead)
                {
                    continue;
                }

                if (elem.TeamID != owner.TeamID && elem.IsSpotted && IsLessThanAngle(elem.CurrentPos, maxAngle))
                {
                    var toTarget = elem.CurrentPos - owner.CameraPos.position;
                    var angle = Vector3.Angle(toTarget, owner.CameraPos.forward);
                    if (angle <= minDegree && elem.IsSpotted)
                    {
                        currentTarget = elem;
                        minDegree = angle;
                    }

                }
            }
        }
    }

    bool IsLessThanAngle(Vector3 target, float angle)
    {
        var toTarget = target - owner.CameraPiv.position;
        return Vector3.Angle(toTarget, owner.CameraPiv.forward) <= angle;
    }

    bool IsInsideAngle(Vector3 target)
    {
        var toTarget = target - owner.CameraPiv.position;
        return Vector3.Angle(toTarget, owner.CameraPiv.forward) <= maxAngle;
    }
}

public static class TargetableManger
{
    public static List<ITargetable> AllTargetables => allTargetables;
    static List<ITargetable> allTargetables = new List<ITargetable>();

    public static void Add(ITargetable add)
    {
        allTargetables.Add(add);
    }

    public static void Remove(ITargetable remove)
    {
        allTargetables.Remove(remove);
    }

    public static ITargetable GetTargetable(int id)
    {
        for (int i = 0; i < allTargetables.Count; i++)
        {
            if(allTargetables[i].ID == id)
            {
                return allTargetables[i];
            }
        }
        return null;
    }
}

public interface ITargetable
{
    int ID { get; }
    int TeamID { get; }
    Vector3 CurrentPos { get; }
    Vector3 Velocity { get; }
    bool IsSpotted { get; }

    bool IsDead { get; }
}

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
public interface IDischarger
{
    int DelayInMilliSec { get; }
    float ShotSpeed { get; }
    void Init(Dueler_Mono owner, IProjectileOnHit[] onHitEffect);
    void Shot(int timestamp, int syncId, Vector3 origin, Vector3 direction);
}


public interface ITargetAction
{
    void SetTarget(ITargetable target);
}

[System.Serializable]
public class PosDegModifier : IPosDegModifier
{
    [SerializeField] int shotNum;

    Dueler_Mono owner;
    System.Action<int, int, Vector3, Vector3> action;

    [SerializeReference] protected IPosition position;
    [SerializeReference] protected IDegree degree;

    public PosDegModifier(IPosition position, IDegree degree)
    {
        this.position = position;
        this.degree = degree;
    }

    public int ShotNum => shotNum;

    public void Init(Dueler_Mono owner, System.Action<int, int, Vector3, Vector3> action)
    {
        this.owner = owner;
        this.action = action;
    }

    public void Shot(int timestamp, int syncId, Vector3 origin, float x, float y)
    {
        var rot = Quaternion.Euler(x, y, 0f);
        for (int n = 0; n < ShotNum; n++)
        {
            action(timestamp, syncId + n, position.GetPosition(owner, origin, rot, n, shotNum), degree.GetDegree(owner, x, y, n, shotNum));
        }
    }
}

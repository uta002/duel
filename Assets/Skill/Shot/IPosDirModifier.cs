
using UnityEngine;

public interface IPosDegModifier
{
    void Init(Dueler_Mono owner, System.Action<int, int, Vector3, Vector3> action);
    int ShotNum { get; }
    void Shot(int timestamp, int syncId, Vector3 origin, float x, float y);
}
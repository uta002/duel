
using UnityEngine;

[System.Serializable]
public class TimestampLifeTime : Timestamp
{
    [SerializeField] protected float lifeTime;
    public float LifeTime => lifeTime;
    public TimestampLifeTime(int timestamp, float lifeTime) : base(timestamp)
    {
        this.lifeTime = lifeTime;
    }

    public void Init(int timestamp, float lifeTime)
    {
        Init(timestamp);
        this.lifeTime = lifeTime;
    }
    public float NormalizedTime => (float)TimeElapsed / lifeTime;
    public bool IsOverLifeTime => NormalizedTime >= 1f;
}

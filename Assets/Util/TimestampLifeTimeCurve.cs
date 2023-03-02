
using UnityEngine;

[System.Serializable]
public class TimestampLifeTimeCurve : TimestampLifeTime
{
    [SerializeField] AnimationCurve animationCurve;

    public TimestampLifeTimeCurve(int timestamp, float lifeTime) : base(timestamp, lifeTime)
    {
    }



    public float EvaluatedNormalizedTime => animationCurve.Evaluate(NormalizedTime);
}
using UnityEngine;

[System.Serializable]
public class RandomDegree : IDegree
{
    [SerializeField] float max_deg_spread = 20f;

    public Vector3 GetDegree(Dueler_Mono owner, float x, float y, int num, int total)
    {
        return new Vector3(x, y, 0f) + (Vector3)Random.insideUnitCircle * max_deg_spread;
    }
}

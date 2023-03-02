using UnityEngine;

public class SphereCasterMono : MonoBehaviour
{
    [SerializeField] Vector3 checkDirection;
    [SerializeField] float checkRadius = 1f;
    [SerializeField] float checkDistance = 1f;
    [SerializeField] LayerMask checkMask = 1 << 0;


    public bool Check(out RaycastHit hitInfo)
    {
        hitInfo = default;
        return Physics.SphereCast(transform.position, checkRadius, transform.rotation * checkDirection, out hitInfo, checkDistance, checkMask);
    }

}

using UnityEngine;

public class WorldToCanvas_Mono : MonoBehaviour
{
    [SerializeField] Vector3 worldPosOffset;
    [SerializeField] Vector2 screenPosOffset;
    [SerializeField] Transform target;
    [SerializeField] Camera targetCamera;
    public void Init(Transform target, Camera targetCamera)
    {
        this.target = target;
        this.targetCamera = targetCamera;
    }
    RectTransform rect;
    private void Awake()
    {
        rect = transform as RectTransform;
    }

    private void Update()
    {
        if (target != null)
        {
            var dir = target.position - targetCamera.transform.position;
            if(Vector3.Dot(targetCamera.transform.forward, dir.normalized) > 0f)
            {
                rect.position = (Vector2)targetCamera.WorldToScreenPoint(target.position + worldPosOffset) + screenPosOffset;
            }
        }
    }
}

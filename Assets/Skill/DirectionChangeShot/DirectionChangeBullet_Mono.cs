using UnityEngine;

public class DirectionChangeBullet_Mono : ProjectileBase_Mono
{

    public void ChangeDirection(int timestamp, Vector3 origin, Vector3 targetPos, float lifeTime, float shotSpeed)
    {
        this.timestamp.Init(timestamp, lifeTime);
        this.origin = origin;
        transform.position = origin;

        var direction = targetPos - origin;

        this.targetPos = origin + lifeTime * shotSpeed * direction.normalized;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}

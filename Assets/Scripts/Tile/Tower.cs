using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10.5f)]
    private float targetingRange = 1.5f;
    TargetPoint target;
    const int enemyLayerMask = 1 << 9;
    static Collider[] targetBuffer = new Collider[100];
    [SerializeField]
    private Transform turrent = default, laserBeam = default;
    Vector3 laserBeamScale;
    [SerializeField, Range(1f, 100f)]
    float damagePerSecond = 10f;

    void Awake()
    {
        laserBeamScale = laserBeam.localScale;
    }

    public override void GameUpdate()
    {
        if (TrackTarget() || AcquireTarget())
        {
            Shoot();
        }
        else
        {
            laserBeam.localScale = Vector3.zero;
        }
        if (target != null)
        {
            target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
        }
    }

    void Shoot()
    {
        Vector3 point = target.Position;
        turrent.LookAt(point);
        laserBeam.localRotation = turrent.localRotation;

        float d = Vector3.Distance(turrent.position, point);
        laserBeamScale.z = d;
        laserBeam.localScale = laserBeamScale;
        laserBeam.localPosition = turrent.localPosition + 0.5f * d * laserBeam.forward;

    }

    bool TrackTarget()
    {
        if (target == null)
        {
            return false;
        }

        Vector3 a = transform.localPosition;
        Vector3 b = target.Position;

        float x = a.x - b.x;
        float z = a.z - b.z;
        float r = targetingRange + 0.125f * target.Enemy.Scale;
        if (x * x + z * z > r * r)
        {
            target = null;
            return false;
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
        if (target != null)
        {
            Gizmos.DrawLine(position, target.Position);
        }
    }

    bool AcquireTarget()
    {
        Vector3 a = transform.localPosition;
        Vector3 b = a;
        b.y += 2f;

        int hits = Physics.OverlapCapsuleNonAlloc(a, b, targetingRange, targetBuffer, enemyLayerMask);

        if (hits > 0)
        {
            target = targetBuffer[Random.Range(0, hits)].GetComponent<TargetPoint>();
            Debug.Assert(target != null, "targeted non-enemy !", targetBuffer[0]);
            return true;
        }
        target = null;
        return false;
    }

}
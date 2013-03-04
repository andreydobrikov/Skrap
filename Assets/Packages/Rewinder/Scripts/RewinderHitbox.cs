using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class RewinderHitbox
{
    public Collider Collider;
    public RewinderHitboxType Type;

    [HideInInspector]
    public float Radius;

    [HideInInspector]
    public Vector3 Center;

    [HideInInspector]
    public Bounds Bounds;

    [HideInInspector]
    public Transform Transform;

    [HideInInspector]
    public RewinderColliderType ColliderType = RewinderColliderType.Unsupported;

    public void Initialize()
    {
        if (Collider != null)
        {
            Transform = Collider.transform;

            BoxCollider bc = Collider as BoxCollider;
            SphereCollider sc = Collider as SphereCollider;

            if (bc != null)
            {
                Bounds = new Bounds(bc.center, bc.size);
                Center = bc.center;
                ColliderType = RewinderColliderType.Box;
            }

            if (sc != null)
            {
                Radius = sc.radius;
                Center = sc.center;
                ColliderType = RewinderColliderType.Sphere;
            }
        }
    }

    public bool OverlapSphere(ref Matrix4x4 matrix, Vector3 center, float radius)
    {
        center = matrix.MultiplyPoint(center);

        switch (ColliderType)
        {
            case RewinderColliderType.Box:
                return OverlapSphereOnBox(center, radius);

            case RewinderColliderType.Sphere:
                return OverlapSphereOnSphere(center, radius);

            default:
                return false;
        }
    }

    public bool Raycast(ref Matrix4x4 matrix, Vector3 origin, Vector3 direction, out float distance)
    {
        origin = matrix.MultiplyPoint(origin);
        direction = matrix.MultiplyVector(direction);

        switch (ColliderType)
        {
            case RewinderColliderType.Box:
                return Bounds.IntersectRay(new Ray(origin, direction), out distance);

            case RewinderColliderType.Sphere:
                return RaycastSphere(origin, direction, out distance);

            default:
                distance = 0f;
                return false;
        }
    }

    bool OverlapSphereOnSphere(Vector3 center, float radius)
    {
        return Vector3.Distance(Center, center) <= Radius + radius;
    }

    /// <summary>
    /// Source: Real-Time Collision Detection by Christer Ericson
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    bool OverlapSphereOnBox(Vector3 center, float radius)
    {
        Vector3 clampedCenter;
        Vector3 min = Bounds.min;
        Vector3 max = Bounds.max;
        RewinderUtils.Clamp(ref center, ref min, ref max, out clampedCenter);

        return Vector3.Distance(center, clampedCenter) <= radius;
    }

    bool RaycastSphere(Vector3 o, Vector3 d, out float distance)
    {
        Vector3 v = o - Center;
        float b = Vector3.Dot(v, d);
        float c = Vector3.Dot(v, v) - (Radius * Radius);

        if (c > 0f && b > 0f)
        {
            distance = 0f;
            return false;
        }

        float disc = b * b - c;

        if (disc < 0f)
        {
            distance = 0f;
            return false;
        }

        distance = -b - (float)Math.Sqrt(disc);

        if (distance < 0f)
        {
            distance = 0f;
        }

        return true;
    }

    public void Draw(Matrix4x4 matrix)
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = matrix;

        switch (ColliderType)
        {
            case RewinderColliderType.Box:
                Gizmos.DrawWireCube(Bounds.center, Bounds.size);
                break;

            case RewinderColliderType.Sphere:
                Gizmos.DrawWireSphere(Center, Radius);
                break;
        }

        Gizmos.matrix = Matrix4x4.identity;
    }
}
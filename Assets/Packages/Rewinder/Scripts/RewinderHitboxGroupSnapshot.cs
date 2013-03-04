#define REWINDER_DEBUG

using UnityEngine;
using System.Collections;

public class RewinderHitboxGroupSnapshot : IPoolable<RewinderHitboxGroup>
{
    public RewinderHitboxGroup Group;
    public Vector3 Position;
    public Quaternion Rotation;
    public Matrix4x4 Matrix;
    public Matrix4x4[] HitboxMatrices;

#if REWINDER_DEBUG
    public Matrix4x4 Matrix_Debug;
    public Matrix4x4[] HitboxMatrices_Debug;
    public Quaternion[] HitboxRotations_Debug;
#endif

    public bool OverlapSphere(Vector3 center, float radius, out RewinderHitboxHit hit)
    {
        bool proximityHit = false;

        hit = new RewinderHitboxHit();
        hit.Group = Group;
        hit.Distance = -1f; // Sphere overlaps dont have a distance

        if (Group.HasProximityHitbox)
        {
            if (proximityHit = Group.ProximityHitbox.OverlapSphere(ref Matrix, center, radius))
            {
                hit.Hitbox = Group.ProximityHitbox;
            }
            else
            {
                return false;
            }
        }

        for (int i = 0; i < Group.BodyHitboxes.Length; ++i)
        {
            RewinderHitbox hitbox = Group.BodyHitboxes[i];

            if (hitbox.OverlapSphere(ref HitboxMatrices[i], center, radius))
            {
                hit.Hitbox = hitbox;
                return true;
            }
        }

        return proximityHit;
    }

    public bool Raycast(Vector3 origin, Vector3 direction, out RewinderHitboxHit hit)
    {
        float distance = 0;
        bool proximityHit = false;

        hit = new RewinderHitboxHit();
        hit.Group = Group;
        hit.Distance = -1f;

        if (Group.HasProximityHitbox)
        {
            if (proximityHit = Group.ProximityHitbox.Raycast(ref Matrix, origin, direction, out distance))
            {
                hit.Hitbox = Group.ProximityHitbox;
                hit.Distance = distance;
            }
            else
            {
                return false;
            }
        }

        for (int i = 0; i < Group.BodyHitboxes.Length; ++i)
        {
            RewinderHitbox hitbox = Group.BodyHitboxes[i];

            if (hitbox.Raycast(ref HitboxMatrices[i], origin, direction, out distance))
            {
                hit.Hitbox = hitbox;
                hit.Distance = distance;
                return true;
            }
        }

        return proximityHit;
    }

    public void DrawGizmos()
    {
#if REWINDER_DEBUG
        if (Group.HasProximityHitbox)
        {
            Group.ProximityHitbox.Draw(Matrix_Debug);
        }

        for (int i = 0; i < Group.BodyHitboxes.Length; ++i)
        {
            Group.BodyHitboxes[i].Draw(HitboxMatrices_Debug[i]);
        }
#endif
    }

    void IPoolable<RewinderHitboxGroup>.OnInitialized(RewinderHitboxGroup group)
    {
        Group = group;
        Position = group.transform.position;
        Rotation = group.transform.rotation;
        Matrix = group.transform.worldToLocalMatrix;
        HitboxMatrices = RewinderArrayPool<Matrix4x4>.Get(group.BodyHitboxes.Length);

        for (int i = 0; i < HitboxMatrices.Length; ++i)
        {
            HitboxMatrices[i] = group.BodyHitboxes[i].Transform.worldToLocalMatrix;
        }

#if REWINDER_DEBUG
        HitboxMatrices_Debug = RewinderArrayPool<Matrix4x4>.Get(group.BodyHitboxes.Length);

        for (int i = 0; i < HitboxMatrices_Debug.Length; ++i)
        {
            HitboxMatrices_Debug[i] = group.BodyHitboxes[i].Transform.localToWorldMatrix;
        }

        HitboxRotations_Debug = RewinderArrayPool<Quaternion>.Get(group.BodyHitboxes.Length);

        for (int i = 0; i < HitboxMatrices_Debug.Length; ++i)
        {
            HitboxRotations_Debug[i] = group.BodyHitboxes[i].Transform.rotation;
        }
#endif
    }

    void IPoolable<RewinderHitboxGroup>.OnPooled()
    {
        RewinderArrayPool<Matrix4x4>.Return(HitboxMatrices);

#if REWINDER_DEBUG
        RewinderArrayPool<Matrix4x4>.Return(HitboxMatrices_Debug);
        RewinderArrayPool<Quaternion>.Return(HitboxRotations_Debug);

        HitboxMatrices_Debug = null;
        HitboxRotations_Debug = null;
#endif

        Group = null;
        HitboxMatrices = null;
    }
}
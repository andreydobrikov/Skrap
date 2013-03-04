using UnityEngine;

public struct RewinderHitboxHit
{
    public float Distance;
    public RewinderHitboxGroup Group;
    public RewinderHitbox Hitbox;

    public bool Hit { get { return Hitbox != null; } }
    public Transform Transform { get { return Group.transform; } }
}
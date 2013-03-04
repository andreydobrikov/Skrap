using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public partial class RewinderSnapshot : IPoolable<IEnumerable<RewinderHitboxGroup>>
{
    HashSet<RewinderHitboxGroupSnapshot> groups = new HashSet<RewinderHitboxGroupSnapshot>();

    public float Time { get; private set; }
    public ushort SnapshotId { get; private set; }
    public RewinderSnapshot Prev { get; private set; }
    public RewinderSnapshot Next { get; private set; }
    public IEnumerable<RewinderHitboxGroupSnapshot> Groups { get { return groups.Select(x => x); } }

    public void DrawGizmos()
    {
        foreach (RewinderHitboxGroupSnapshot group in groups)
        {
            group.DrawGizmos();
        }
    }

    public void Raycast(Vector3 origin, Vector3 direction, List<RewinderHitboxHit> result)
    {
        foreach (RewinderHitboxGroupSnapshot group in groups)
        {
            RewinderHitboxHit hit;

            if (group.Raycast(origin, direction, out hit))
            {
                result.Add(hit);
            }
        }
    }

    public void OverlapSphere(Vector3 center, float radius, List<RewinderHitboxHit> result)
    {
        foreach (RewinderHitboxGroupSnapshot group in groups)
        {
            RewinderHitboxHit hit;

            if (group.OverlapSphere(center, radius, out hit))
            {
                result.Add(hit);
            }
        }
    }

    void IPoolable<IEnumerable<RewinderHitboxGroup>>.OnInitialized(IEnumerable<RewinderHitboxGroup> initGroups)
    {
        Time = GetTime();
        SnapshotId = ++snapshotIdCounter;
        snapshotIdMap[SnapshotId] = this;

        foreach (RewinderHitboxGroup group in initGroups)
        {
            group.SnapshotsCount += 1;
            groups.Add(RewinderHitboxGroupSnapshotPool.Instance.Get(group));
        }
    }

    void IPoolable<IEnumerable<RewinderHitboxGroup>>.OnPooled()
    {
        foreach (RewinderHitboxGroupSnapshot group in groups)
        {
            group.Group.SnapshotsCount -= 1;
            RewinderHitboxGroupSnapshotPool.Instance.Return(group);
        }

        snapshotIdMap.Remove(SnapshotId);
        Time = -1f;
        SnapshotId = 0;
        Prev = null;
        Next = null;
        groups.Clear();
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class RewinderSnapshot
{
    static int count = 0;
    static int maxSnapshots = 60;
    static ushort snapshotIdCounter = 0;
    static HashSet<RewinderHitboxGroup> currentGroups = new HashSet<RewinderHitboxGroup>();
    static RewinderSnapshot latest = null;
    static RewinderSnapshot oldest = null;
    static Queue<List<RewinderHitboxHit>> hitboxHitPool = new Queue<List<RewinderHitboxHit>>(64);
    static Dictionary<ushort, RewinderSnapshot> snapshotIdMap = new Dictionary<ushort, RewinderSnapshot>();

    public static System.Func<float> GetTime = (() => (float)PhotonNetwork.time);

    public static int MaxSnapshots
    {
        get { return maxSnapshots; }
        set { maxSnapshots = Mathf.Clamp(value, 16, 2048); }
    }

    public static RewinderSnapshot Latest
    {
        get
        {
            if (count > 1)
            {
                return latest;
            }

            return null;
        }
    }

    public static RewinderSnapshot Oldest
    {
        get
        {
            if (count > 1)
            {
                return oldest;
            }

            return null;
        }
    }

    public static int Count
    {
        get { return count; }
    }

    public static void RegisterGroup(RewinderHitboxGroup group)
    {
        currentGroups.Add(group);
    }

    public static void UnregisterGroup(RewinderHitboxGroup group)
    {
        currentGroups.Remove(group);
    }

    public static RewinderSnapshot Create()
    {
        RewinderSnapshot snapshot = RewinderSnapshotPool.Instance.Get(currentGroups);

        if (latest == null)
        {
            latest = oldest = snapshot;
        }
        else
        {
            snapshot.Next = latest;
            snapshot.Next.Prev = snapshot;
            latest = snapshot;
        }

        ++count;

        while (count > MaxSnapshots)
        {
            --count;

            RewinderSnapshot temp = oldest;
            oldest = temp.Prev;
            oldest.Next = null;

            RewinderSnapshotPool.Instance.Return(temp);
        }

        return snapshot;
    }

    public static void Reset()
    {
        RewinderSnapshot current = latest;

        while (current != null)
        {
            RewinderSnapshot temp = current;
            current = current.Next;
            RewinderSnapshotPool.Instance.Return(temp);
        }

        count = 0;
        oldest = null;
        latest = null;
    }

    public static RewinderSnapshot Find(ushort id)
    {
        RewinderSnapshot snapshot;

        if (snapshotIdMap.TryGetValue(id, out snapshot))
        {
            return snapshot;
        }

        return null;
    }

    public static RewinderSnapshot Find(float time)
    {
        // TODO: Currently O(n), needs to get faster

        float foundTime = float.MaxValue;
        RewinderSnapshot foundSnapshot = latest;
        RewinderSnapshot currentSnapshot = latest;

        while (currentSnapshot != null)
        {
            float length = Mathf.Abs(currentSnapshot.Time - time);

            if (length < foundTime)
            {
                foundTime = length;
                foundSnapshot = currentSnapshot;
            }

            currentSnapshot = currentSnapshot.Next;
        }

        return foundSnapshot;
    }

    public static bool OverlapSphere(float time, Vector3 origin, float radius, out List<RewinderHitboxHit> result)
    {
        result = GetResultList(2);
        return OverlapSphere(time, origin, radius, result);
    }

    public static bool OverlapSphere(float time, Vector3 origin, float radius, List<RewinderHitboxHit> result)
    {
        time = GetTime() - time;
        RewinderSnapshot snapshot = Find(time);

        if (snapshot != null)
        {
            snapshot.OverlapSphere(origin, radius, result);
        }

        return result.Count > 0;
    }

    public static bool Raycast(float time, Vector3 origin, Vector3 direction, out List<RewinderHitboxHit> result)
    {
        result = GetResultList(2);
        return Raycast(time, origin, direction, result);
    }

    public static bool Raycast(float time, Vector3 origin, Vector3 direction, List<RewinderHitboxHit> result)
    {
        time = GetTime() - time;
        RewinderSnapshot snapshot = Find(time);

        if (snapshot != null)
        {
            snapshot.Raycast(origin, direction, result);
        }

        return result.Count > 0;
    }

    public static void Recycle(List<RewinderHitboxHit> list)
    {
        list.Clear();
        hitboxHitPool.Enqueue(list);
    }

    static List<RewinderHitboxHit> GetResultList(int capacity)
    {
        if (hitboxHitPool.Count > 0)
        {
            return hitboxHitPool.Dequeue();
        }

        return new List<RewinderHitboxHit>(capacity);
    }
}
using System.Collections.Generic;

public interface IPoolable<T>
{
    void OnInitialized(T arg);
    void OnPooled();
}

public abstract class RewinderObjectPool<TObject, TArg>
    where TObject : class, IPoolable<TArg>, new()
{
    readonly Queue<TObject> queue = new Queue<TObject>(16);

    public bool Enabled = true;

    public TObject Get(TArg arg)
    {
        TObject obj;

        if (Enabled && queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            obj = new TObject();
        }

        obj.OnInitialized(arg);
        return obj;
    }

    public void Return(TObject obj)
    {
        if (obj != null && Enabled)
        {
            obj.OnPooled();
            queue.Enqueue(obj);
        }
    }
}

public class RewinderHitboxGroupSnapshotPool : RewinderObjectPool<RewinderHitboxGroupSnapshot, RewinderHitboxGroup>
{
    public static readonly RewinderHitboxGroupSnapshotPool Instance = new RewinderHitboxGroupSnapshotPool();

    RewinderHitboxGroupSnapshotPool()
    {

    }
}

public class RewinderSnapshotPool : RewinderObjectPool<RewinderSnapshot, IEnumerable<RewinderHitboxGroup>>
{
    public static readonly RewinderSnapshotPool Instance = new RewinderSnapshotPool();

    RewinderSnapshotPool()
    {

    }
}
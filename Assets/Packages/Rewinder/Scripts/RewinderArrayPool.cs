using System.Collections;
using System.Collections.Generic;

public static class RewinderArrayPool<T>
{
    static Dictionary<int, Queue<T[]>> pools = new Dictionary<int, Queue<T[]>>();

    public static T[] Get(int size)
    {
        Queue<T[]> pool;

        if (!pools.TryGetValue(size, out pool))
        {
            pools[size] = pool = new Queue<T[]>();
        }

        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }

        return new T[size];
    }

    public static void Return(T[] array)
    {
        if (array != null)
        {
            Queue<T[]> pool;

            if (pools.TryGetValue(array.Length, out pool))
            {
                pool.Enqueue(array);
            }
        }
    }
}
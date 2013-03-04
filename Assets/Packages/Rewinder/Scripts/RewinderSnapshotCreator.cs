using UnityEngine;
using System.Collections;

public class RewinderSnapshotCreator : MonoBehaviour
{
    static RewinderSnapshotCreator instance;

    public static RewinderSnapshotCreator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = MonoBehaviour.FindObjectOfType(typeof(RewinderSnapshotCreator)) as RewinderSnapshotCreator;
            }

            return instance;
        }
    }

    public enum FrameTypes
    {
        Update,
        FixedUpdate
    }

    bool running = false;

    [SerializeField]
    public FrameTypes FrameType = FrameTypes.Update;

    [SerializeField]
    public int SnapshotEveryNthFrame = 1;

    [SerializeField]
    public int RaiseEveryNthSnapshot = 1;

    [SerializeField]
    public int MaxSnapshots = 60;

    public event System.Action<RewinderSnapshot> NewSnapshot;

    void Start()
    {
        if (Application.isPlaying)
        {
            instance = this;
            StartCoroutine("CreateSnapshot");
            RewinderSnapshot.MaxSnapshots = MaxSnapshots;
        }
    }

    void OnDestroy()
    {
        running = false;
        StopCoroutine("CreateSnapshot");
    }

    void OnEnable()
    {
        Start();
    }

    void OnDisable()
    {
        OnDestroy();
    }

    IEnumerator CreateSnapshot()
    {
        if (running)
        {
            yield break;
        }

        running = true;

        int frameCounter = 0;
        int snapshotCounter = 0;

        while (running)
        {
            switch (FrameType)
            {
                case FrameTypes.Update:
                    yield return new WaitForEndOfFrame();
                    break;

                case FrameTypes.FixedUpdate:
                    yield return new WaitForFixedUpdate();
                    break;
            }

            if (running)
            {
                if (++frameCounter >= SnapshotEveryNthFrame)
                {
                    RewinderSnapshot snapshot = RewinderSnapshot.Create();

                    if (++snapshotCounter >= RaiseEveryNthSnapshot)
                    {
                        if (NewSnapshot != null)
                        {
                            NewSnapshot(snapshot);
                        }

                        snapshotCounter = 0;
                    }

                    frameCounter = 0;
                }
            }
        }
    }
}
using UnityEngine;

public class RewinderHitboxGroup : MonoBehaviour
{
    [SerializeField]
    public RewinderHitbox ProximityHitbox;

    [SerializeField]
    public RewinderHitbox[] BodyHitboxes;

    [HideInInspector]
    public int SnapshotsCount = 0;

    public bool HasProximityHitbox
    {
        get
        {
            return 
                ProximityHitbox != null && 
                ProximityHitbox.ColliderType != RewinderColliderType.Unsupported;
        }
    }

    void Start()
    {
        if (ProximityHitbox != null)
        {
            ProximityHitbox.Initialize();
        }

        if (BodyHitboxes != null)
        {
            for (int i = 0; i < BodyHitboxes.Length; ++i)
            {
                BodyHitboxes[i].Initialize();
            }
        }

        RewinderSnapshot.RegisterGroup(this);
    }

    void OnEnable()
    {
        Start();
    }

    void OnDestroy()
    {
        RewinderSnapshot.UnregisterGroup(this);
    }

    void OnDisable()
    {
        OnDestroy();
    }
}
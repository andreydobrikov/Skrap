#define ANNOTATE_TETHER
using UnityEngine;
using UnitySteer.Helpers;

/// <summary>
/// Steers a vehicle to keep within a certain range of a point
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Tether")]
public class SteerForTether : Steering
{
    #region Private properties

    [SerializeField]
    float _outerRadius = 30f;

    [SerializeField]
    float _innerRadius = 20f;

    [SerializeField]
    Vector3 _position;

    #endregion

    #region Public properties

    /// <summary>
    /// The maximum radius of the tether.  The pushback force is maximal here.
    /// </summary>
    public float OuterRadius
    {
        get
        {
            return this._outerRadius;
        }
        set
        {
            _outerRadius = Mathf.Clamp(value, 0, float.MaxValue);
        }
    }

    /// <summary>
    /// The inner radius of the tether where the pushback force starts.
    /// The pushback increases as the vehicle approaches the outer radius.
    /// </summary>
    public float InnerRadius
    {
        get
        {
            return this._innerRadius;
        }
        set
        {
            _innerRadius = Mathf.Clamp(value, 0, float.MaxValue);
        }
    }

    /// <summary>
    /// The center position of the tether.
    /// </summary>
    public Vector3 Position
    {
        get
        {
            return this._position;
        }
        set
        {
            _position = value;
        }
    }

    #endregion

    protected override Vector3 CalculateForce()
    {
        Vector3 steering = Vector3.zero;

        var difference = Position - Vehicle.Position;   // vector difference between vehicle and tether.
        var distance = difference.magnitude;

        // Check if the vehicle is outside the inner radius.  If so,
        // start applying pushback force.
        if (distance > _innerRadius)
        {
            float percent = 0;

            if (_outerRadius == _innerRadius)
            {
                percent = 1;
            }
            else
            {
                // Percent = 
                percent = (distance - _innerRadius) / (_outerRadius - _innerRadius);
            }

            steering = difference * percent;
        }

        return steering;
    }

#if ANNOTATE_TETHER
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.9f);
        Gizmos.DrawWireSphere(_position, _outerRadius);
        Gizmos.color = new Color(0f, 0f, 0.8f, 0.9f);
        Gizmos.DrawWireSphere(_position, _innerRadius);
    }
#endif
}
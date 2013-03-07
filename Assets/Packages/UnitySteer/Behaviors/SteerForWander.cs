#define ANNOTATE_WANDER
using UnityEngine;
using UnitySteer;

/// <summary>
/// Steers a vehicle to wander around
/// Following http://www.red3d.com/cwr/steer/Wander.html
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Wander")]
public class SteerForWander : Steering
{
	Vector3 _target;
	
	[SerializeField]
	float _targetDistance = 2; 	    	// Offset distance of target sphere
	
    [SerializeField]
    float _targetRadius = 1f;	       	// Target sphere radius

    [SerializeField]
	float _jitterRadius = 0.4f;			// Jitter sphere radius

    #region Public properties
	
	/// <summary>
	/// Radius of the sphere, the target point is moving on.
	/// </summary>
    public float TargetRadius
    {
        get {
            return _targetRadius;
        }
        set
        {
            _targetRadius = value;
        }
    }

	/// <summary>
	/// Offset from the vehicle to the target sphere.
	/// </summary>
    public float TargetDistance
    {
        get
        {
            return _targetDistance;
        }
        set
        {
            _targetDistance = value;
        }
    }
	
	/// <summary>
	/// Radius of the jitter sphere, which is applied to the target point every tick.
	/// </summary>
	public float JitterRadius
	{
		get
		{
			return _jitterRadius;
		}
		set
		{
			_jitterRadius = value;
		}
	}

    #endregion
	
	protected override void Start()
	{
		base.Start();
		
		ResetTarget();
	}
	
	public void ResetTarget() {
		_target = Vehicle.transform.forward;
	}
	
	private Vector3 Direction()
	{
		if (Vehicle.Speed == 0)
			return Vehicle.transform.forward.normalized;  // Default to forward vector
		else
        	return Vehicle.Velocity.normalized;          // Velocity
	}
	
    protected override Vector3 CalculateForce()
    {
		_target += new Vector3(Random.Range(-_jitterRadius, _jitterRadius), 
					Random.Range(-_jitterRadius, _jitterRadius), 
					Random.Range(-_jitterRadius, _jitterRadius));
		if (Vehicle.IsPlanar)
		{
			_target.y = 0;
		}
		_target.Normalize();
		_target *= _targetRadius;
		
		var force = Direction() * _targetDistance + _target;
		if (force.sqrMagnitude > 1)
		{
			force.Normalize();
		}
		return force;
    }

#if ANNOTATE_WANDER
    void OnDrawGizmos()
    {
        if (Vehicle != null)
        {
            var center = Vehicle.Position + Direction() * _targetDistance;

			Gizmos.color = Color.blue;
			Gizmos.DrawLine(Vehicle.Position, center);
			
			Gizmos.color = Color.grey;
			Gizmos.DrawWireSphere(center, _targetRadius);
			
            Gizmos.color = Color.red;
			Gizmos.DrawLine(center, center + _target);
        }
    }

#endif
}
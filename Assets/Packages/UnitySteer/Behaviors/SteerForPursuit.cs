#define ANNOTATE_PURSUIT
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;

/// <summary>
/// Steers a vehicle to pursuit another one
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Pursuit")]
public class SteerForPursuit : Steering
{
	#region Private fields
	MovementTracker _movementTracker;
	
	[SerializeField]
	Transform _quarry;
	
    /// <summary>
    /// A distance at which we are 'close enough' to stop pursuing
    /// </summary>
    /// <remarks>
    /// Notice that this is different from the vehicle's arrival radius,
    /// since we may want to be able to have an attack distance that is
    /// different from the radius used when moving to a point.
    /// </remarks>
    [SerializeField]
    float _acceptableDistance = 0;
    
	[SerializeField]
	public float _trackingInterval = 0.25f;
	
	[SerializeField]
	float _maxPredictionTime = 5;
	#endregion
	
	#region Public properties
	/// <summary>
	/// Maximum time to look ahead for the prediction calculation
	/// </summary>
	public float MaxPredictionTime {
		get {
			return this._maxPredictionTime;
		}
		set {
			_maxPredictionTime = value;
		}
	}
	
	/// <summary>
	/// Target being pursued
	/// </summary>
	/// <remarks>When set, it will clear the flag that indicates we've already reported that we arrived</remarks>
	public Transform Quarry {
		get {
			return this._quarry;
		}
		set {
			if (_quarry != value) 
			{
				ReportedArrival = false;
				_quarry = value;
				
				if (_movementTracker == null)
				{
					_movementTracker = new MovementTracker(_quarry, _trackingInterval);
				}
				_movementTracker.target = _quarry;
				_movementTracker.enabled = _quarry != null;
			}
		}
	}
	#endregion
	
	protected override void Awake()
	{
		base.Awake();
		_movementTracker = new MovementTracker(Quarry, _trackingInterval);
		_movementTracker.enabled = Quarry != null;
	}
	
	protected override Vector3 CalculateForce ()
	{
		if (_quarry == null) {
			return Vector3.zero;
		}
		
		var force    = Vector3.zero;
		var offset	 = _quarry.position - Vehicle.Position;
		var distance = offset.magnitude;
		// TODO missing from merge: + _quarry.ScaledRadius, because _quarry is only a transform in my version
        var radius   = Vehicle.ScaledRadius + _acceptableDistance;

		if (distance > radius)
		{
			Vector3 unitOffset = offset / distance;

			// how parallel are the paths of "this" and the quarry
			// (1 means parallel, 0 is pependicular, -1 is anti-parallel)
			float parallelness = Vector3.Dot(transform.forward, _quarry.forward);

			// how "forward" is the direction to the quarry
			// (1 means dead ahead, 0 is directly to the side, -1 is straight back)
			float forwardness = Vector3.Dot(transform.forward, unitOffset);

			float directTravelTime = distance / Vehicle.Speed;
			int f = OpenSteerUtility.intervalComparison (forwardness,  -0.707f, 0.707f);
			int p = OpenSteerUtility.intervalComparison (parallelness, -0.707f, 0.707f);

			float timeFactor = 0;		// to be filled in below

			// Break the pursuit into nine cases, the cross product of the
			// quarry being [ahead, aside, or behind] us and heading
			// [parallel, perpendicular, or anti-parallel] to us.
			switch (f)
			{
				case +1:
					switch (p)
					{
					case +1:		  // ahead, parallel
						timeFactor = 4;
						break;
					case 0:			  // ahead, perpendicular
						timeFactor = 1.8f;
						break;
					case -1:		  // ahead, anti-parallel
						timeFactor = 0.85f;
						break;
					}
					break;
				case 0:
					switch (p)
					{
					case +1:		  // aside, parallel
						timeFactor = 1;
						break;
					case 0:			  // aside, perpendicular
						timeFactor = 0.8f;
						break;
					case -1:		  // aside, anti-parallel
						timeFactor = 4;
						break;
					}
					break;
				case -1:
					switch (p)
					{
					case +1:		  // behind, parallel
						timeFactor = 0.5f;
						break;
					case 0:			  // behind, perpendicular
						timeFactor = 2;
						break;
					case -1:		  // behind, anti-parallel
						timeFactor = 2;
						break;
					}
					break;
			}

			// estimated time until intercept of quarry
			float et = directTravelTime * timeFactor;
			float etl = (et > _maxPredictionTime) ? _maxPredictionTime : et;

			// estimated position of quarry at intercept
			Vector3 target = _quarry.position + _movementTracker.velocity * etl;

			force = Vehicle.GetSeekVector(target, true, true);
			
			#if ANNOTATE_PURSUIT
			Debug.DrawLine(Vehicle.Position, force, Color.blue);
			Debug.DrawLine(Quarry.position, target, Color.cyan);
			Debug.DrawRay(target, Vector3.up * 4, Color.cyan);
			#endif
		}
		
		return force;
	}
}
#define ANNOTATE_NAVMESH
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;

/// <summary>
/// Steers a vehicle to stay on the navmesh
/// Currently only supports the Default layer
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Navmesh")]
public class SteerForNavmesh : Steering {
	#region Private fields
	[SerializeField]
	float _avoidanceForceFactor = 0.1f;

	[SerializeField]
	float _minTimeToCollision = 2;
	
	[SerializeField]
	float _whiskerLengthFactor = 0.7f;		// in %
	
	[SerializeField]
	float _minWhiskerAngle = 15f;
	
	[SerializeField]
	float _maxWhiskerAngle = 70f;
	
	[SerializeField]
	float _whiskerSpreadSpeed = 2f;
	
	[SerializeField]
	int _whiskerCooldown = 10;		// number of ticks, before whiskers start collapsing again
	
	[SerializeField]
	float _whiskerCollapseSpeed = 2f;
	
	
	[SerializeField]
	bool _offMeshCheckingEnabled = true;
	
	[SerializeField]
	Vector3 _probePositionOffset = new Vector3(0, 0.2f, 0);
	
	[SerializeField]
	float _probeRadius = 0.1f;
	
	// TODO navmesh layer selection -> CustomEditor -> GameObjectUtility.GetNavMeshLayerNames() + Popup
	#endregion
	
	#region Private properties
	private float _currentWhiskerAngle;
	private float currentWhiskerAngle
	{
		get
		{
			return _currentWhiskerAngle;
		}
		set
		{
			_currentWhiskerAngle = Mathf.Clamp(value, _minWhiskerAngle, _maxWhiskerAngle);
		}
	}
	private int _currentWhiskerHeat;		// whiskers will collapse, if this is back to 0
	private int currentWhiskerHeat
	{
		get
		{
			return _currentWhiskerHeat;
		}
		set
		{
			_currentWhiskerHeat = Mathf.Max(0, value);
		}
	}
	#endregion
	
	#region Public properties
	/// <summary>
	/// Multiplier for the force applied on avoidance
	/// </summary>
	/// <remarks>If his value is set to 1, the behavior will return an
	/// avoidance force that uses the full brunt of the vehicle's maximum
	/// force.</remarks>
	public float AvoidanceForceFactor {
		get {
			return this._avoidanceForceFactor;
		}
		set {
			_avoidanceForceFactor = value;
		}
	}

	/// <summary>
	/// Minimum time to collision to consider
	/// </summary>
	public float MinTimeToCollision {
		get {
			return this._minTimeToCollision;
		}
		set {
			_minTimeToCollision = value;
		}
	}
	
	/// <summary>
	/// Switch if off-mesh checking should be done or not.
	/// </summary>
	/// <remarks>Off-mesh chekcing, checks if the Vehicle is currently on the navmesh or not.
	/// If not, a force is calculated to bring it back on it.
	/// </remarks>
	public bool OffMeshChecking {
		get {
			return _offMeshCheckingEnabled;
		}
		set {
			_offMeshCheckingEnabled = value;
		}
	}
	
	/// <summary>
	/// Offset where to place the off-mesh checking probe, relative to the Vehicle position
	/// </summary>
	/// <remarks>This should be as close to the navmesh height as possible. Normally 
	/// it's slightly floating above the ground (0.2 with default settings on a simple plain).
	/// </remarks>
	public Vector3 ProbePositionOffset {
		get { 
			return this._probePositionOffset;
		}
		set {
			_probePositionOffset = value;
		}
	}
	
	/// <summary>
	/// Offset where to place the off-mesh checking probe, relative to the Vehicle position
	/// </summary>
	/// <remarks>The radius makes it possible to compensate slight variations in the navmesh
	/// heigh. However, this setting  affects the horizontal tolerance as well. This means,
	/// the larger the radius, the later the vehicle will be considered off mesh.
	/// </remarks>
	public float ProbeRadius {
		get {
			return this._probeRadius;
		}
		set {
			_probeRadius = value;
		}
	}
	#endregion
	
	private int _navMeshLayerMask;
	
	protected override void Start() {
		base.Start();
		_navMeshLayerMask = 1 << NavMesh.GetNavMeshLayerFromName("Default");
		_currentWhiskerAngle = _minWhiskerAngle;
	}
	
	
	public override bool IsPostProcess 
	{ 
		get { return true; }
	}
	
	private NavMeshHit NavmeshRaycast(Vector3 movement) {
		#if ANNOTATE_NAVMESH
		Debug.DrawRay(Vehicle.Position, movement, Color.cyan);
		#endif
		
		NavMeshHit hit;
		NavMesh.Raycast(Vehicle.Position, Vehicle.Position + movement, out hit, _navMeshLayerMask);		
		return hit;
	}
	
	private Vector3 CalculateAvoidanceForce(Vector3 movement, NavMeshHit hit) {
		Vector3 avoidance = Vector3.zero;
		Profiler.BeginSample("Calculate NavMesh avoidance");
		{
			Vector3 moveDirection = movement.normalized;
			avoidance = OpenSteerUtility.perpendicularComponent(hit.normal, moveDirection);
	
			avoidance.Normalize();
			
			avoidance *= Vehicle.MaxForce * _avoidanceForceFactor;
	
			#if ANNOTATE_NAVMESH
			Debug.DrawLine(Vehicle.Position, Vehicle.Position + avoidance, Color.white);
			#endif
	
			avoidance += moveDirection;
	
			#if ANNOTATE_NAVMESH
			Debug.DrawLine(Vehicle.Position, Vehicle.Position + avoidance, Color.yellow);
			#endif
		}
		Profiler.EndSample();
		return avoidance;
	}
	
	/// <summary>
	/// Calculates the force necessary to stay on the navmesh
	/// </summary>
	/// <returns>
	/// Force necessary to stay on the navmesh, or Vector3.zero
	/// </returns>
	/// <remarks>
	/// If the Vehicle is too far off the navmesh, Vector3.zero is retured.
	/// This won't lead back to the navmesh, but there's no way to determine
	/// a way back onto it.
	/// </remarks>
	protected override Vector3 CalculateForce()
	{
		
		/*
		 * While we could just calculate line as (Velocity * predictionTime) 
		 * and save ourselves the substraction, this allows other vehicles to
		 * override PredictFuturePosition for their own ends.
		 */
		Vector3 futurePosition = Vehicle.PredictFuturePosition(_minTimeToCollision);
		Vector3 movement = futurePosition - Vehicle.Position;
		
		if (_offMeshCheckingEnabled) {
			Vector3 probePosition = Vehicle.Position + _probePositionOffset;
			
			NavMeshHit hit;
			Profiler.BeginSample("Off-mesh checking");
			NavMesh.SamplePosition(probePosition, out hit, _probeRadius, _navMeshLayerMask);
			Profiler.EndSample();
			
			if (!hit.hit) {		// we're not on the navmesh
				Profiler.BeginSample("Find closest edge");
				NavMesh.FindClosestEdge(probePosition, out hit, _navMeshLayerMask);
				Profiler.EndSample();
				
				if (hit.hit) {		// closest edge found
					#if ANNOTATE_NAVMESH
					Debug.DrawLine(probePosition, hit.position, Color.red);
					#endif
					
					return (hit.position - probePosition).normalized * Vehicle.MaxForce * _avoidanceForceFactor;
				} else {			// no closest edge - too far off the mesh
					#if ANNOTATE_NAVMESH
					Debug.DrawLine(probePosition, probePosition + Vector3.up * 3, Color.red);
					#endif
					
					return Vector3.zero;
				}
			}
		}
		
		
		NavMeshHit center;
		NavMeshHit rightWhisker;
		NavMeshHit leftWhisker;
		
		Profiler.BeginSample("NavMesh raycast");
		{
			center = NavmeshRaycast(movement);
			rightWhisker = NavmeshRaycast(Quaternion.AngleAxis(_currentWhiskerAngle, Vector3.up) * movement * _whiskerLengthFactor);
			leftWhisker = NavmeshRaycast(Quaternion.AngleAxis(-_currentWhiskerAngle, Vector3.up) * movement * _whiskerLengthFactor);
		}
		Profiler.EndSample();
		
		if (!center.hit && !leftWhisker.hit && !rightWhisker.hit) {	// no collision
			currentWhiskerHeat--;								// cool down whiskers
			if (currentWhiskerHeat == 0)						// if completely cooled down
				currentWhiskerAngle -= _whiskerCollapseSpeed;	// start to collapse
			return Vector3.zero;
		}
		
		currentWhiskerHeat = _whiskerCooldown;			// max cooldown value
		currentWhiskerAngle += _whiskerSpreadSpeed;		// spread whiskers
		
		if (rightWhisker.hit)
			return CalculateAvoidanceForce(movement, rightWhisker);
		if (leftWhisker.hit)
			return CalculateAvoidanceForce(movement, leftWhisker);
		if (center.hit)
			return CalculateAvoidanceForce(movement, center);
		
		return Vector3.zero;
	}	
}

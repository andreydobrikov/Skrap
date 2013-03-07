using UnityEngine;
using UnitySteer.Helpers;

[AddComponentMenu("UnitySteer/Steer/... for Collision")]
public class SteerForCollision : Steering {
	
	#region private properties
	private bool _didCollide = false;
	private Vector3 _collisionPosition;
	
	/// <summary>
	/// GameObject to collide with.
	/// </summary>
	[SerializeField]
	private GameObject _targetObject;
	
	/// <summary>
	/// Should the vehicle's velocity be considered in the seek calculations?
	/// </summary>
	/// <remarks>
	/// If true, the vehicle will slow down as it approaches its target
	/// </remarks>
	[SerializeField]
	private bool _considerVelocity = true;
	
	/// <summary>
	/// Should collisions with colliders on child game objects be considered
	/// equally?
	/// </summary>
	/// <remarks>
	/// If true, a collision with a collider on a child object has the same
	/// effect as a collision with the target object itself.
	/// </remarks>
	[SerializeField]
	private bool _considerSubCollider = true;
	
	/// <summary>
	/// How far does the target object have to move, to start following again?
	/// </summary>
	[SerializeField]
	private float _minMoveDistance = 0.5f;
	
	#endregion
	
	#region public properties
	
	/// <summary>
	/// GameObject to collide with.
	/// </summary>
	public GameObject TargetObject { 
		get { return _targetObject; }
		set {
			_targetObject = value;
			_didCollide = false;
			_collisionPosition = Vector3.zero;
			ReportedArrival = false;
		}
	}
	
	/// <summary>
	/// Should the vehicle's velocity be considered in the seek calculations?
	/// </summary>
	/// <remarks>
	/// If true, the vehicle will slow down as it approaches its target
	/// </remarks>
 	public bool ConsiderVelocity { 
		get { return _considerVelocity; } 
		set { _considerVelocity = value; }
	}

	/// <summary>
	/// Should collisions with colliders on child game objects be considered
	/// equally?
	/// </summary>
	/// <remarks>
	/// If true, a collision with a collider on a child object has the same
	/// effect as a collision with the target object itself.
	/// </remarks>
	public bool ConsiderSubCollider { 
		get { return _considerSubCollider; }
		set { _considerSubCollider = value; }
	}

	/// <summary>
	/// How far does the target object have to move, to start following again?
	/// </summary>
	public float MinMoveDistance { 
		get { return _minMoveDistance; }
		set { _minMoveDistance = value; }
	}
	#endregion
	
	protected void Update()
	{
		if (TargetObject != null && Vector3.Distance(TargetObject.transform.position, _collisionPosition) > MinMoveDistance)
		{
			_didCollide = false;
		}
	}
	
	/// <summary>
	/// Calculates the force to apply to the vehicle to collide with the target
	/// </summary>
	protected override Vector3 CalculateForce()
	{
		if (_didCollide)
		{
			return Vector3.zero;
		}
		return Vehicle.GetSeekVector(TargetObject.transform.position, ConsiderVelocity, false);
	}
	
	void OnCollisionEnter(Collision collision)
	{
		HandleCollision(collision);
	}
	
	void OnCollisionStay(Collision collision)
	{
		HandleCollision(collision);
	}
	
	void HandleCollision(Collision collision)
	{
		if (_didCollide || TargetObject == null)
		{
			return;
		}
		
		if (collision.gameObject == _targetObject)
		{
			_didCollide = true;
		} else if (ConsiderSubCollider)	{
			Transform transform = collision.transform.parent;
			while (transform != null)
			{
				if (transform.gameObject == _targetObject)
				{
					_didCollide = true;
					break;
				}
				transform = transform.parent;
			}
		}
		if (_didCollide)
		{
			_collisionPosition = TargetObject.transform.position;
		}
	}
}

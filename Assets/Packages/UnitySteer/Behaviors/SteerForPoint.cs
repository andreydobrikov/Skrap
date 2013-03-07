using UnityEngine;
using UnitySteer.Helpers;

[AddComponentMenu("UnitySteer/Steer/... for Point")]
public class SteerForPoint : Steering
{
	
	/// <summary>
	/// Target point
	/// </summary>
	/// <remarks>
	/// Declared as a separate value so that we can inspect it on Unity in 
	/// debug mode.
	/// </remarks>
	[SerializeField]
	Vector3 _targetPoint = Vector3.zero;

	/// <summary>
	/// Should the vehicle's velocity be considered in the seek calculations?
	/// </summary>
	/// <remarks>
	/// If true, the vehicle will slow down as it approaches its target
	/// </remarks>
	[SerializeField]
	bool _considerVelocity = false;
	
	/// <summary>
	/// Should the vehicle's arrival radius be considered in the seek calculations?
	/// </summary>
	/// <remarks>
	/// If true, the vehicle will seek for the exact target point.
	/// </remarks>
	[SerializeField]
	bool _considerArrivalRadius = true;
	
	/// <summary>
	/// The target point.
	/// </summary>
 	public Vector3 TargetPoint
	{
		get { return _targetPoint; }
		set
		{
			_targetPoint = value;
			ReportedArrival = false;
		}
	}


	/// <summary>
	/// Should the vehicle's velocity be considered in the seek calculations?
	/// </summary>
	/// <remarks>
	/// If true, the vehicle will slow down as it approaches its target
	/// </remarks>
 	public bool ConsiderVelocity
	{
		get { return _considerVelocity; }
		set { _considerVelocity = value; }
	}

	/// <summary>
	/// Should the vehicle's arrival radius be considered in the seek calculations?
	/// </summary>
	/// <remarks>
	/// If true, the vehicle will seek for the exact target point.
	/// </remarks>
	public bool ConsiderArrivalRadius
	{
		get { return _considerArrivalRadius; }
		set { _considerArrivalRadius = value; }
	}

	
	public new void Start()
	{
		base.Start();
		
		if (TargetPoint == Vector3.zero)
		{
			TargetPoint = transform.position;
		}
	}
	
	/// <summary>
	/// Calculates the force to apply to the vehicle to reach a point
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	protected override Vector3 CalculateForce()
	{
		return Vehicle.GetSeekVector(TargetPoint, _considerVelocity, _considerArrivalRadius);
	}
}

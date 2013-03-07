using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TickedPriorityQueue;

public class MovementTracker {
	private TickedObject tickedObject;
	private UnityTickedQueue tickedQueue;
	
	private float lastUpdate;
	
	private Transform _target;
	public Transform target
	{
		get { return _target; }
		set {
			_target = value;
			Reset();
		}
	}
	
	private bool _enabled = false;
	public bool enabled
	{ 
		get { return _enabled; } 
		set {
			if (_enabled == value)
				return;
			
			_enabled = value; 
			Reset();
			if (_enabled)
				tickedQueue.Add(tickedObject);
			else
				tickedQueue.Remove(tickedObject);
		}
	}
	
	public Vector3 position { get; private set; }
	private Vector3 _positionPrev = Vector3.zero;
	
	public Vector3 velocity { get; private set; }
	private Vector3 _velocityPrev = Vector3.zero;
	public Vector3 velocitySmoothed { get; private set; }
	
	public bool isMoving { get { return velocity.sqrMagnitude > _sqrMovingTolerance; } }
	private float _sqrMovingTolerance = 0.0025f;
	public float movingTolerance
	{
		get { return Mathf.Sqrt(_sqrMovingTolerance); }
		set { _sqrMovingTolerance = value * value; }
	}
	
	public Vector3 acceleration { get; private set; }
	public Vector3 accelerationSmoothed { get; private set; }
	
	public Quaternion rotation { get; private set; }
	private Quaternion _rotationPrev = Quaternion.identity;
	
	public Vector3 angularVelocity { get; private set; }
	public Vector3 angularVelocitySmoothed { get; private set; }
	
	public MovementTracker(Transform target = null, float measuringSpeed = 0.25f, string queueName = "MovementTracker")
	{
		tickedObject = new TickedObject(Update);
		tickedObject.TickLength = measuringSpeed;
		tickedQueue = UnityTickedQueue.GetInstance(queueName);
		
		this.target = target;
		enabled = true;
	}
	
	~MovementTracker()
	{
		tickedQueue.Remove(tickedObject);
	}
	
	public void Reset()
	{
		position = _positionPrev = target != null ? target.position : Vector3.zero;
		rotation = _rotationPrev = target != null ? target.rotation : Quaternion.identity;
		velocity = Vector3.zero;
		_velocityPrev = Vector3.zero;
		velocitySmoothed = Vector3.zero;
		acceleration = Vector3.zero;
		accelerationSmoothed = Vector3.zero;
		angularVelocity = Vector3.zero;
		angularVelocitySmoothed = Vector3.zero;
	}
	
	private Vector3 CalculateAngularVelocity(Quaternion prev, Quaternion current, float deltaTime)
	{
		Quaternion deltaRotation = Quaternion.Inverse(prev) * current;
		float angle = 0.0f;
		Vector3 axis = Vector3.zero;
		deltaRotation.ToAngleAxis(out angle, out axis);
		if (axis == Vector3.zero || axis.x == Mathf.Infinity || axis.x == Mathf.NegativeInfinity)
			return Vector3.zero;
		if (angle>180) angle -= 360;
		angle = angle / deltaTime;
		return axis.normalized * angle;
	}
	
	void Update(object userData)
	{
		if (!enabled)
		{
			return;
		}
		if (target == null)
		{
			Debug.LogError("No target set on MovementTracker");
			return;
		}
		
		float deltaTime = Time.time - lastUpdate;
		
		Profiler.BeginSample("Calculate movement");
		position = target.position;
		rotation = target.rotation;
		
		velocity = (position - _positionPrev) / deltaTime;
		angularVelocity = CalculateAngularVelocity(_rotationPrev, rotation, deltaTime);
		
		acceleration = (velocity - _velocityPrev) / deltaTime;
		
		velocitySmoothed = Vector3.Lerp(velocitySmoothed, velocity, deltaTime * 10);		
		accelerationSmoothed = Vector3.Lerp(accelerationSmoothed, acceleration, deltaTime * 3);
		angularVelocitySmoothed = Vector3.Lerp(angularVelocitySmoothed, angularVelocity, deltaTime * 3);
		Profiler.EndSample();
		
		_positionPrev = position;
		_rotationPrev = rotation;
		_velocityPrev = velocity;
		
		lastUpdate = Time.time;
	}
}

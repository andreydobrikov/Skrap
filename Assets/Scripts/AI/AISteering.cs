using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct SteeringWeight
{
	public float seek;
	public float flee;
	public float arrive;
	public float wander;
	public float cohesion;
	public float seperation;
	public float allignment;
	public float pursuit;
	public float evade;
	public float interpose;
	public float fock;
	public float offsetPursuit;
}

public class AISteering : MonoBehaviour 
{
	[Flags]
	public enum BehaviorType
	{
	    None = 0,
	    Seek = 1,
	    Flee = 2,
	    Arrive = 4,
		Wander = 5,
		Cohesion = 6,
		Seperation = 7,
		Allignment = 8,
		Pursuit = 9,
		Evade = 10,
		Interpose = 11,
		Hide = 12,
		Flock = 13,
		OffsetPursuit = 14
	};

	public BehaviorType behaviorTypes;

	//Move these to AI class (ref them in perhaps?)
	[SerializeField]
	float _maxSpeed;
	[SerializeField]
	float _maxForce;
	Vector3 _wanderTarget;
	[SerializeField]
	float _wanderRadius;
	[SerializeField]
	float _wanderDistance;

	private const float RADIUS_EPSILON = 1.00001f;

	private Vector3 _steeringForce;

	public Transform _targetAgent1;

	public SteeringWeight steeringWeights;



	/*Fuck this and everything it stands for!

	//----------------------- Calculate --------------------------------------
	//  calculates the accumulated steering force according to the method set
	//  in m_SummingMethod
	//------------------------------------------------------------------------
	Vector3 Calculate()
	{ 
		//reset the steering force
		_steeringForce = Vector3.zero;

		//Get neightbours
		//m_pVehicle->World()->CellSpace()->CalculateNeighbors(m_pVehicle->Pos(), m_dViewDistance);

		_steeringForce = CalculateDithered();

		return _steeringForce;
	}

	//--------------------- AccumulateForce ----------------------------------
	//
	//  This function calculates how much of its max steering force the 
	//  vehicle has left to apply and then applies that amount of the
	//  force to add.
	//------------------------------------------------------------------------
	bool AccumulateForce(Vector3 RunningTot, Vector3 ForceToAdd)
	{
		//calculate how much steering force the vehicle has used so far
		float MagnitudeSoFar = RunningTot.magnitude;

		//calculate how much steering force remains to be used by this vehicle
		float MagnitudeRemaining = _maxForce - MagnitudeSoFar;

		//return false if there is no more force left to use
		if (MagnitudeRemaining <= 0.0) 
			return false;

		//calculate the magnitude of the force we want to add
		double MagnitudeToAdd = ForceToAdd.magnitude;

		//if the magnitude of the sum of ForceToAdd and the running total
		//does not exceed the maximum force available to this vehicle, just
		//add together. Otherwise add as much of the ForceToAdd vector is
		//possible without going over the max.
		if (MagnitudeToAdd < MagnitudeRemaining)
		{
			RunningTot += ForceToAdd;
		}

		else
		{
			//add it to the steering force
			RunningTot += Vector3.Normalize(ForceToAdd) * MagnitudeRemaining;
		}

		return true;
	}

	//---------------------- CalculateDithered ----------------------------
	//
	//  this method sums up the active behaviors by assigning a probabilty
	//  of being calculated to each behavior. It then tests the first priority
	//  to see if it should be calcukated this simulation-step. If so, it
	//  calculates the steering force resulting from this behavior. If it is
	//  more than zero it returns the force. If zero, or if the behavior is
	//  skipped it continues onto the next priority, and so on.
	//
	//  NOTE: Not all of the behaviors have been implemented in this method,
	//        just a few, so you get the general idea
	//------------------------------------------------------------------------
	Vector3 CalculateDithered()
	{  
		//reset the steering force
		_steeringForce = Vector3.zero;

		if (On(separation) && RandFloat() < Prm.prSeparation)
		{
			m_vSteeringForce += Separation(m_pVehicle->World()->Agents()) * 
			m_dWeightSeparation / Prm.prSeparation;

			if (!m_vSteeringForce.isZero())
			{
				m_vSteeringForce.Truncate(_maxForce); 

				return m_vSteeringForce;
			}
		}



		if (On(flee) && RandFloat() < Prm.prFlee)
		{
		m_vSteeringForce += Flee(m_pVehicle->World()->Crosshair()) * m_dWeightFlee / Prm.prFlee;

		if (!m_vSteeringForce.isZero())
		{
		m_vSteeringForce.Truncate(m_pVehicle->MaxForce()); 

		return m_vSteeringForce;
		}
		}

		if (On(evade) && RandFloat() < Prm.prEvade)
		{
		assert(m_pTargetAgent1 && "Evade target not assigned");

		m_vSteeringForce += Evade(m_pTargetAgent1) * m_dWeightEvade / Prm.prEvade;

		if (!m_vSteeringForce.isZero())
		{
		m_vSteeringForce.Truncate(m_pVehicle->MaxForce()); 

		return m_vSteeringForce;
		}
		}

		if (On(allignment) && RandFloat() < Prm.prAlignment)
		{
		m_vSteeringForce += Alignment(m_pVehicle->World()->Agents()) *
		m_dWeightAlignment / Prm.prAlignment;

		if (!m_vSteeringForce.isZero())
		{
		m_vSteeringForce.Truncate(m_pVehicle->MaxForce()); 

		return m_vSteeringForce;
		}
		}

		if (On(cohesion) && RandFloat() < Prm.prCohesion)
		{
		m_vSteeringForce += Cohesion(m_pVehicle->World()->Agents()) * 
		m_dWeightCohesion / Prm.prCohesion;

		if (!m_vSteeringForce.isZero())
		{
		m_vSteeringForce.Truncate(m_pVehicle->MaxForce()); 

		return m_vSteeringForce;
		}
		}

		if (On(wander) && RandFloat() < Prm.prWander)
		{
		m_vSteeringForce += Wander() * m_dWeightWander / Prm.prWander;

		if (!m_vSteeringForce.isZero())
		{
		m_vSteeringForce.Truncate(m_pVehicle->MaxForce()); 

		return m_vSteeringForce;
		}
		}

		if (On(seek) && RandFloat() < Prm.prSeek)
		{
		m_vSteeringForce += Seek(m_pVehicle->World()->Crosshair()) * m_dWeightSeek / Prm.prSeek;

		if (!m_vSteeringForce.isZero())
		{
		m_vSteeringForce.Truncate(m_pVehicle->MaxForce()); 

		return m_vSteeringForce;
		}
		}

		if (On(arrive) && RandFloat() < Prm.prArrive)
		{
		m_vSteeringForce += Arrive(m_pVehicle->World()->Crosshair(), m_Deceleration) * 
		m_dWeightArrive / Prm.prArrive;

		if (!m_vSteeringForce.isZero())
		{
		m_vSteeringForce.Truncate(m_pVehicle->MaxForce()); 

		return m_vSteeringForce;
		}
		}

		return m_vSteeringForce;
	}

*/



	Vector3 Seek(Vector3 targetPos)
	{
		Vector3 desiredVelocity = Vector3.Normalize(targetPos - transform.position) * _maxSpeed;

		return (desiredVelocity - rigidbody.velocity);
	}

	Vector3 Flee(Vector3 targetPos)
	{
		Vector3 desiredVelocity = Vector3.Normalize(transform.position - targetPos) * _maxSpeed;

		return (desiredVelocity - rigidbody.velocity);
	}

	Vector3 Arrive(Vector3 targetPos, int deceleration)
	{
		Vector3 toTarget = targetPos - transform.position;

		float dist = toTarget.magnitude;

		if(dist > 0.01)
		{
			//A bit more tweaking for the deceleration
			float decelerationTweaker = 0.3f;

			float speed = dist / ((float)deceleration * decelerationTweaker);

			speed = Mathf.Min(speed, this._maxSpeed);

			Vector3 desiredVelocity = toTarget * speed / dist;

			return (desiredVelocity - rigidbody.velocity);
		}

		return Vector3.zero;
	}

	Vector3 Pursuit(GameObject evader)
	{
		//If the evader is ahead, seek torwards it.
		Vector3 toEvader = evader.transform.position - transform.position;

		float relativeHeading = Vector3.Dot(transform.forward, evader.transform.forward);

		if(Vector3.Dot(toEvader, transform.forward) > 0 && (relativeHeading < -0.95f))
		{
			return Seek(evader.transform.position);
		}
		//Not considered ahead so we predict where the evader will be.

		//the lookahead time is propotional to the distance between the evader
		//and the pursuer; and is inversely proportional to the sum of the
		//agent's velocities
																	//Replace with speed.
		float lookAheadTime = toEvader.magnitude / (this._maxSpeed + evader.rigidbody.velocity.magnitude);

		return Seek(evader.transform.position + evader.rigidbody.velocity * lookAheadTime);
	}

	Vector3 Evade(GameObject pursuer)
	{
		Vector3 toPursuer = pursuer.transform.position - transform.position;


		//Move this to an overload?
		float threatRange = 20f;
		if(toPursuer.sqrMagnitude > threatRange * threatRange)
			return Vector3.zero;


		float lookAheadTime = toPursuer.magnitude / (this._maxSpeed + pursuer.rigidbody.velocity.magnitude);

		return Flee(pursuer.transform.position + pursuer.rigidbody.velocity * lookAheadTime);
	}


	Vector3 Wander(float jitterAmount)
	{
		float jitterForThisTimeSlice = jitterAmount * Time.deltaTime;

		_wanderTarget += new Vector3(UnityEngine.Random.Range(-1f, 1f) * jitterForThisTimeSlice, UnityEngine.Random.Range(-1f, 1f) * jitterForThisTimeSlice, UnityEngine.Random.Range(-1f, 1f) * jitterForThisTimeSlice);

		_wanderTarget.Normalize();

		_wanderTarget *= _wanderRadius;

		var target = Vector3.zero;

		target = _wanderTarget + new Vector3(_wanderDistance, _wanderDistance, 0);

		return target - transform.position;
	}

	Vector3 Seperation(Transform[] neighbours)
	{
		Vector3 steeringForce = Vector3.zero;

		foreach(Transform trans in neighbours)
		{
			Vector3 toAgent = transform.position - trans.position;

			//scale the force inversely proportional to the agents distance from its neighbour.
			steeringForce += Vector3.Normalize(toAgent)/toAgent.magnitude;
		}

		return steeringForce;
	}

	Vector3 Alignment(Transform[] neighbours)
	{
		Vector3 averageHeading = Vector3.zero;
		
		if(neighbours.Length > 0)
		{
			for(int i = 0; i < neighbours.Length; i++)
			{
				averageHeading += neighbours[i].forward;
			}

			averageHeading /= neighbours.Length;

			averageHeading -= transform.forward;
		}
		return averageHeading;
	}

	Vector3 Cohesion(Transform[] neighbours)
	{
		Vector3 centreOfMass = Vector3.zero;
		Vector3 steeringForce = Vector3.zero;

		if(neighbours.Length > 0)
		{
			for(int i = 0; i < neighbours.Length; i++)
			{
				centreOfMass += neighbours[i].position;
			}

			centreOfMass /= neighbours.Length;

			steeringForce = Seek(centreOfMass);
		}

		return Vector3.Normalize(steeringForce);
	}

	Vector3 Interpose(Transform agentA, Transform agentB)
	{
		//Estimate the midpoint
		Vector3 midPoint = (agentA.position + agentB.position) / 2.0f;

		float timeToReachMidPoint = Vector3.Distance(transform.position, midPoint) / this._maxSpeed;

		//Extrapolate to get the agents position at the time calculated above (futurepos = P + V * T)
		Vector3 aPos = agentA.position + agentA.rigidbody.velocity * timeToReachMidPoint;
		Vector3 bPos = agentB.position + agentB.rigidbody.velocity * timeToReachMidPoint;

		midPoint = (aPos + bPos) / 2.0f;

		return Arrive(midPoint, 3);
	}

	Vector3 Hide(Transform hunter, Transform[] obstacles)
	{
		float distToClosest = float.MaxValue;

		Vector3 bestHidingSpot = Vector3.zero;

		Transform closestObstacle;

		foreach(Transform obstacle in obstacles)
		{
			//Find the position of the hiding spot for this obstacle
			Vector3 hidingSpot = GetHidingSpot(obstacle.position, 30f, hunter.position);

			//Get the distance-squared
			var squaredDist = (hidingSpot - transform.position).sqrMagnitude;

			if(squaredDist < distToClosest)
			{
				distToClosest = squaredDist;

				bestHidingSpot = hidingSpot;

				closestObstacle = obstacle;
			}
		}

		//if no suitable obstacles found then Evade the hunter
		if (distToClosest == float.MaxValue)
		{
			return Evade(hunter.gameObject);
		}

		//else use Arrive on the hiding spot
		return Arrive(bestHidingSpot, 3);
	}

	Vector3 GetHidingSpot(Vector3 obsPos, float obsRadius, Vector3 hunterPos)
	{
		float distanceFromBoundary = 30.0f;

		float distAway = obsRadius + distanceFromBoundary;

		Vector3 toOb = Vector3.Normalize(obsPos - hunterPos);

		return (toOb * distAway) + obsPos;
	}

	Vector3 OffsetPursuit(Transform leader, Vector3 worldOffset)
	{
		Vector3 toOffset = worldOffset - transform.position;
																	//Replace with speed
		float lookAheadTime = toOffset.magnitude / (this._maxSpeed + leader.rigidbody.velocity.magnitude);

		return Arrive(worldOffset + leader.rigidbody.velocity * lookAheadTime, 3);
	}
}
	
/*


//------------------------------- FollowPath -----------------------------
//  C++ example for waypoint following:
//
//  Given a series of Vector2Ds, this method produces a force that will
//  move the agent along the waypoints in order. The agent uses the
// 'Seek' behavior to move to the next waypoint - unless it is the last
//  waypoint, in which case it 'Arrives'
//------------------------------------------------------------------------
Vector2D SteeringBehavior::FollowPath()
{ 
  //move to next target if close enough to current target (working in
  //distance squared space)
  if(Vec2DDistanceSq(m_pPath->CurrentWaypoint(), m_pVehicle->Pos()) <
     m_dWaypointSeekDistSq)
  {
    m_pPath->SetNextWaypoint();
  }

  if (!m_pPath->Finished())
  {
    return Seek(m_pPath->CurrentWaypoint());
  }

  else
  {
    return Arrive(m_pPath->CurrentWaypoint(), normal);
  }
}
*/

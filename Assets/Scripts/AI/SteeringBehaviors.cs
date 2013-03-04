using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SteeringBehaviors : MonoBehaviour 
{
	public enum SummingMethod
	{
		WeightedAverage, Prioritized, Dithered
	}


	//Move these to AI class (ref them in perhaps?)
	float _maxSpeed;
	Vector3 _wanderTarget;
	float _wanderRadius;
	float _wanderDistance;

	private const float RADIUS_EPSILON = 1.00001f;

	private Vector3 _steeringForce;
	private SummingMethod _summingMethod;

	/*
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

		switch (_summingMethod)
		{
			case SummingMethod.WeightedAverage:
			_steeringForce = CalculateWeightedSum(); 

			break;

			case SummingMethod.Prioritized:
			_steeringForce = CalculatePrioritized(); 

			break;

			case SummingMethod.Dithered:
			_steeringForce = CalculateDithered();

			break;

			default:
			_steeringForce = Vector3.zero; 

		}
		return _steeringForce;
	}
	  
	  
	 */
	/*


//---------------------- CalculatePrioritized ----------------------------
//
//  this method calls each active steering behavior in order of priority
//  and acumulates their forces until the max steering force magnitude
//  is reached, at which time the function returns the steering force 
//  accumulated to that  point
//------------------------------------------------------------------------
Vector2D SteeringBehavior::CalculatePrioritized()
{       
  Vector2D force;
  
   if (On(wall_avoidance))
  {
    force = WallAvoidance(m_pVehicle->World()->Walls()) *
            m_dWeightWallAvoidance;

    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
  }
   
  if (On(obstacle_avoidance))
  {
    force = ObstacleAvoidance(m_pVehicle->World()->Obstacles()) * 
            m_dWeightObstacleAvoidance;

    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
  }

  if (On(evade))
  {
    assert(m_pTargetAgent1 && "Evade target not assigned");
    
    force = Evade(m_pTargetAgent1) * m_dWeightEvade;

    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
  }

  
  if (On(flee))
  {
    force = Flee(m_pVehicle->World()->Crosshair()) * m_dWeightFlee;

    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
  }


 
  //these next three can be combined for flocking behavior (wander is
  //also a good behavior to add into this mix)
  if (!isSpacePartitioningOn())
  {
    if (On(separation))
    {
      force = Separation(m_pVehicle->World()->Agents()) * m_dWeightSeparation;

      if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
    }

    if (On(allignment))
    {
      force = Alignment(m_pVehicle->World()->Agents()) * m_dWeightAlignment;

      if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
    }

    if (On(cohesion))
    {
      force = Cohesion(m_pVehicle->World()->Agents()) * m_dWeightCohesion;

      if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
    }
  }

  else
  {

    if (On(separation))
    {
      force = SeparationPlus(m_pVehicle->World()->Agents()) * m_dWeightSeparation;

      if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
    }

    if (On(allignment))
    {
      force = AlignmentPlus(m_pVehicle->World()->Agents()) * m_dWeightAlignment;

      if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
    }

    if (On(cohesion))
    {
      force = CohesionPlus(m_pVehicle->World()->Agents()) * m_dWeightCohesion;

      if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
    }
  }

  if (On(seek))
  {
    force = Seek(m_pVehicle->World()->Crosshair()) * m_dWeightSeek;

    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
  }


  if (On(arrive))
  {
    force = Arrive(m_pVehicle->World()->Crosshair(), m_Deceleration) * m_dWeightArrive;

    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
  }

  if (On(wander))
  {
    force = Wander() * m_dWeightWander;

    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
  }

  if (On(pursuit))
  {
    assert(m_pTargetAgent1 && "pursuit target not assigned");

    force = Pursuit(m_pTargetAgent1) * m_dWeightPursuit;

    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
  }

  if (On(offset_pursuit))
  {
    assert (m_pTargetAgent1 && "pursuit target not assigned");
    assert (!m_vOffset.isZero() && "No offset assigned");

    force = OffsetPursuit(m_pTargetAgent1, m_vOffset);

    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
  }

  if (On(interpose))
  {
    assert (m_pTargetAgent1 && m_pTargetAgent2 && "Interpose agents not assigned");

    force = Interpose(m_pTargetAgent1, m_pTargetAgent2) * m_dWeightInterpose;

    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
  }

  if (On(hide))
  {
    assert(m_pTargetAgent1 && "Hide target not assigned");

    force = Hide(m_pTargetAgent1, m_pVehicle->World()->Obstacles()) * m_dWeightHide;

    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
  }


  if (On(follow_path))
  {
    force = FollowPath() * m_dWeightFollowPath;

    if (!AccumulateForce(m_vSteeringForce, force)) return m_vSteeringForce;
  }

  return m_vSteeringForce;
}


//---------------------- CalculateWeightedSum ----------------------------
//
//  this simply sums up all the active behaviors X their weights and 
//  truncates the result to the max available steering force before 
//  returning
//------------------------------------------------------------------------
Vector2D SteeringBehavior::CalculateWeightedSum()
{        
  if (On(wall_avoidance))
  {
    m_vSteeringForce += WallAvoidance(m_pVehicle->World()->Walls()) *
                         m_dWeightWallAvoidance;
  }
   
  if (On(obstacle_avoidance))
  {
    m_vSteeringForce += ObstacleAvoidance(m_pVehicle->World()->Obstacles()) * 
            m_dWeightObstacleAvoidance;
  }

  if (On(evade))
  {
    assert(m_pTargetAgent1 && "Evade target not assigned");
    
    m_vSteeringForce += Evade(m_pTargetAgent1) * m_dWeightEvade;
  }


  //these next three can be combined for flocking behavior (wander is
  //also a good behavior to add into this mix)
  if (!isSpacePartitioningOn())
  {
    if (On(separation))
    {
      m_vSteeringForce += Separation(m_pVehicle->World()->Agents()) * m_dWeightSeparation;
    }

    if (On(allignment))
    {
      m_vSteeringForce += Alignment(m_pVehicle->World()->Agents()) * m_dWeightAlignment;
    }

    if (On(cohesion))
    {
      m_vSteeringForce += Cohesion(m_pVehicle->World()->Agents()) * m_dWeightCohesion;
    }
  }
  else
  {
    if (On(separation))
    {
      m_vSteeringForce += SeparationPlus(m_pVehicle->World()->Agents()) * m_dWeightSeparation;
    }

    if (On(allignment))
    {
      m_vSteeringForce += AlignmentPlus(m_pVehicle->World()->Agents()) * m_dWeightAlignment;
    }

    if (On(cohesion))
    {
      m_vSteeringForce += CohesionPlus(m_pVehicle->World()->Agents()) * m_dWeightCohesion;
    }
  }


  if (On(wander))
  {
    m_vSteeringForce += Wander() * m_dWeightWander;
  }

  if (On(seek))
  {
    m_vSteeringForce += Seek(m_pVehicle->World()->Crosshair()) * m_dWeightSeek;
  }

  if (On(flee))
  {
    m_vSteeringForce += Flee(m_pVehicle->World()->Crosshair()) * m_dWeightFlee;
  }

  if (On(arrive))
  {
    m_vSteeringForce += Arrive(m_pVehicle->World()->Crosshair(), m_Deceleration) * m_dWeightArrive;
  }

  if (On(pursuit))
  {
    assert(m_pTargetAgent1 && "pursuit target not assigned");

    m_vSteeringForce += Pursuit(m_pTargetAgent1) * m_dWeightPursuit;
  }

  if (On(offset_pursuit))
  {
    assert (m_pTargetAgent1 && "pursuit target not assigned");
    assert (!m_vOffset.isZero() && "No offset assigned");

    m_vSteeringForce += OffsetPursuit(m_pTargetAgent1, m_vOffset) * m_dWeightOffsetPursuit;
  }

  if (On(interpose))
  {
    assert (m_pTargetAgent1 && m_pTargetAgent2 && "Interpose agents not assigned");

    m_vSteeringForce += Interpose(m_pTargetAgent1, m_pTargetAgent2) * m_dWeightInterpose;
  }

  if (On(hide))
  {
    assert(m_pTargetAgent1 && "Hide target not assigned");

    m_vSteeringForce += Hide(m_pTargetAgent1, m_pVehicle->World()->Obstacles()) * m_dWeightHide;
  }

  if (On(follow_path))
  {
    m_vSteeringForce += FollowPath() * m_dWeightFollowPath;
  }

  m_vSteeringForce.Truncate(m_pVehicle->MaxForce());
 
  return m_vSteeringForce;
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
Vector2D SteeringBehavior::CalculateDithered()
{  
  //reset the steering force
   m_vSteeringForce.Zero();

  if (On(wall_avoidance) && RandFloat() < Prm.prWallAvoidance)
  {
    m_vSteeringForce = WallAvoidance(m_pVehicle->World()->Walls()) *
                         m_dWeightWallAvoidance / Prm.prWallAvoidance;

    if (!m_vSteeringForce.isZero())
    {
      m_vSteeringForce.Truncate(m_pVehicle->MaxForce()); 
      
      return m_vSteeringForce;
    }
  }
   
  if (On(obstacle_avoidance) && RandFloat() < Prm.prObstacleAvoidance)
  {
    m_vSteeringForce += ObstacleAvoidance(m_pVehicle->World()->Obstacles()) * 
            m_dWeightObstacleAvoidance / Prm.prObstacleAvoidance;

    if (!m_vSteeringForce.isZero())
    {
      m_vSteeringForce.Truncate(m_pVehicle->MaxForce()); 
      
      return m_vSteeringForce;
    }
  }

  if (!isSpacePartitioningOn())
  {
    if (On(separation) && RandFloat() < Prm.prSeparation)
    {
      m_vSteeringForce += Separation(m_pVehicle->World()->Agents()) * 
                          m_dWeightSeparation / Prm.prSeparation;

      if (!m_vSteeringForce.isZero())
      {
        m_vSteeringForce.Truncate(m_pVehicle->MaxForce()); 
      
        return m_vSteeringForce;
      }
    }
  }

  else
  {
    if (On(separation) && RandFloat() < Prm.prSeparation)
    {
      m_vSteeringForce += SeparationPlus(m_pVehicle->World()->Agents()) * 
                          m_dWeightSeparation / Prm.prSeparation;

      if (!m_vSteeringForce.isZero())
      {
        m_vSteeringForce.Truncate(m_pVehicle->MaxForce()); 
      
        return m_vSteeringForce;
      }
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


  if (!isSpacePartitioningOn())
  {
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
  }
  else
  {
    if (On(allignment) && RandFloat() < Prm.prAlignment)
    {
      m_vSteeringForce += AlignmentPlus(m_pVehicle->World()->Agents()) *
                          m_dWeightAlignment / Prm.prAlignment;

      if (!m_vSteeringForce.isZero())
      {
        m_vSteeringForce.Truncate(m_pVehicle->MaxForce()); 
      
        return m_vSteeringForce;
      }
    }

    if (On(cohesion) && RandFloat() < Prm.prCohesion)
    {
      m_vSteeringForce += CohesionPlus(m_pVehicle->World()->Agents()) *
                          m_dWeightCohesion / Prm.prCohesion;

      if (!m_vSteeringForce.isZero())
      {
        m_vSteeringForce.Truncate(m_pVehicle->MaxForce()); 
      
        return m_vSteeringForce;
      }
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

using UnityEngine;
using System.Collections;

public class BuzzSawController : Enemy
{
	int directionParamID;
	int speedParamID;
	int attackParamID;

	protected override void InitEnemy()
	{
		directionParamID = Animator.StringToHash("Direction");
		speedParamID = Animator.StringToHash("Speed");
		attackParamID = Animator.StringToHash("Attack");
	}

	protected override void UpdateAnimation()
	{
		if(photonView.isMine)
		{
			//Calculating the move direction in degrees, from -180 to 180
			// the vector that we want to measure an angle from
			Vector3 referenceForward = transform.forward;
     
			// the vector perpendicular to referenceForward (90 degrees clockwise)
			// (used to determine if angle is positive or negative)
			Vector3 referenceRight= Vector3.Cross(Vector3.up, referenceForward);
     
			// Vehicles current move vector
			Vector3 newDirection = vehicle.Velocity;
     
			// Get the angle in degrees between 0 and 180
			float angle = Vector3.Angle(newDirection, referenceForward);
     
			// Determine if the degree value should be negative. Here, a positive value
			// from the dot product means that our vector is the right of the reference vector
			// whereas a negative value means we're on the left.
			float sign = (Vector3.Dot(newDirection, referenceRight) > 0.0f) ? 1.0f: -1.0f;
     
			float finalAngle = sign * angle;
			animator.SetFloat(directionParamID, finalAngle);

			animator.SetFloat(speedParamID, vehicle.Speed);
		}
	}

	void Update()
	{
		//Make sure the network and ai has been setup before doing anything
		if (photonView != null)
			UpdateAnimation();
	}

	public override void Attack()
	{
		animator.SetBool(attackParamID, true);
	}
}

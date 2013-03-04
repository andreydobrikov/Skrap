using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
	public float followSpeed = 5.0f;
	public Vector3 rotationMask = new Vector3(0,1,0);
	public float orbitSpeed = 5f;
	public float distanceToKeep = 10.0f;
	
	Transform targetGO;
	Transform playerGO;
	
	
	void Start()
	{
		targetGO = transform.parent;
		playerGO = GameObject.FindWithTag("Player").transform;
	}
	
	void LateUpdate () 
	{
		float leftAndRightInput = Input.GetAxis("CameraHorizontal");
		if(leftAndRightInput != 0f)
		{
			RotateAroundPlayer(leftAndRightInput);
		}
		
		var distanceFromPlayer = Vector3.Distance(targetGO.position, playerGO.position);
		if(distanceFromPlayer > distanceToKeep)
		{
			FollowPlayer();
		}
	}
	
	void FollowPlayer()
	{
		var newPosition = Vector3.MoveTowards(targetGO.position, playerGO.position, followSpeed * Time.deltaTime);
		newPosition.y = targetGO.position.y;
		targetGO.position = newPosition;
	}
	
	void RotateAroundPlayer(float input)
	{
		targetGO.Rotate(rotationMask * input * orbitSpeed * Time.deltaTime);
	}
}
using UnityEngine;
using System.Collections;
 
[RequireComponent (typeof (Collider))]
 
public class CharacterControls : MonoBehaviour 
{
	public float speed = 10.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	private bool grounded = false;
	private Camera cam;
	private GameObject graphicsObject;

	void Start ()
	{
		cam = GameObject.Find("Camera").camera;
	    rigidbody.freezeRotation = true;
	    rigidbody.useGravity = false;
		graphicsObject = transform.Find("Graphics").gameObject;
	}
 
	void FixedUpdate () 
	{
		LookAtMouse();
	    if (grounded) 
		{
			var directionVector = CalculateDirectionFromInput();
			
			// Rotate the input vector into camera space so up is camera's up and right is camera's right
			directionVector = Camera.main.transform.rotation * directionVector;
			
			// Rotate input vector to be perpendicular to character's up vector
			var camToCharacterSpace = Quaternion.FromToRotation(-Camera.main.transform.forward, transform.up);
			directionVector = (camToCharacterSpace * directionVector);


	        Vector3 velocity = rigidbody.velocity;
	        Vector3 velocityChange = (directionVector - velocity);

	        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
	        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
	        velocityChange.y = 0;

	        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
 
	        // Jump
	        if (canJump && Input.GetButton("Jump"))
			{
	            rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
	        }
	    }
 
	    // We apply gravity manually for more tuning control
	    rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
 
	    grounded = false;
	}

	Vector3 CalculateDirectionFromInput()
	{
		// Get the input vector from keyboard or analog stick
		var directionVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
		
		if (directionVector != Vector3.zero) 
		{
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			var directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);

			//Make it a bit more sensitive for analogue gamepads
			directionLength *= directionLength;
			
			// Multiply the normalized direction vector by the modified length
			directionVector *= directionLength;

			//Add our movement speed
			directionVector *= speed;

			return directionVector;
		}
		return Vector3.zero;
	}
 
	void OnCollisionStay () 
	{
		 grounded = true;    
	}
 
	float CalculateJumpVerticalSpeed ()
	{
	    // From the jump height and gravity we deduce the upwards speed 
	    // for the character to reach at the apex.
	    return Mathf.Sqrt(2 * jumpHeight * gravity);
	}
	
	void LookAtMouse()
	{
		// Generate a plane that intersects the transform's position with an upwards normal.
		var playerPlane = new Plane(Vector3.up, graphicsObject.transform.position);
		
		// Generate a ray from the cursor position
		var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		
		// Determine the point where the cursor ray intersects the plane.
		// This will be the point that the object must look towards to be looking at the mouse.
		// Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
		//   then find the point along that ray that meets that distance.  This will be the point
		//   to look at.
		var hitdist = 0.0f;
		// If the ray is parallel to the plane, Raycast will return false.
		if (playerPlane.Raycast (ray, out hitdist)) 
		{
			// Get the point along the ray that hits the calculated distance.
			var targetPoint = ray.GetPoint(hitdist);
			
			// Determine the target rotation.  This is the rotation if the transform looks at the target point.
			var targetRotation = Quaternion.LookRotation(targetPoint - graphicsObject.transform.position);
			
			// Smoothly rotate towards the target point.
			graphicsObject.transform.rotation = Quaternion.Slerp(graphicsObject.transform.rotation, targetRotation, speed * Time.deltaTime);
		}
	}
}
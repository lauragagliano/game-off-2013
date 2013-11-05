using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	float worldZClamp;
	public float movespeed = 20.0f;
	
	void Awake ()
	{
		// Remember their initial Z position and keep them there forever.
		worldZClamp = transform.position.z;
	}
	
	void Update ()
	{
		TryMove ();
		ClampToWorldZ (worldZClamp);
	}
	
	/*
	 * Sets the character's position to the specified worldZ, preventing
	 * him from shifting.
	 */
	void ClampToWorldZ (float worldZ)
	{
		transform.position = new Vector3 (transform.position.x, transform.position.y, worldZ);
	}
	
	/*
	 * Polls input and moves the character accordingly
	 */
	void TryMove ()
	{
		float direction = Input.GetAxis ("Horizontal");
		Move (new Vector3 (direction, 0.0f, 0.0f), movespeed);
	}

	void Move (Vector3 direction, float speed)
	{
		Vector3 movement = (direction.normalized * speed);
		movement *= Time.deltaTime;

		// Apply movement vector
		CharacterController biped = GetComponent<CharacterController> ();
		biped.Move (movement);
	}
}

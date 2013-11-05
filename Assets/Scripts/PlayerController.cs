using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	
	public float movespeed = 20.0f;
	
	void Update ()
	{
		TryMove ();
	}
	
	/*
	 * Polls input and moves the character accordingly
	 */
	void TryMove ()
	{
		float direction = Input.GetAxis ("Horizontal");
		Move (new Vector3 (direction, 0.0f, 0.0f), movespeed);
		if (direction == 0) {
			transform.Translate (0, 0, 0);
		}
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

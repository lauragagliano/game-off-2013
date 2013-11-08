using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerControllerRGB : MonoBehaviour
{
	Player player;
	float worldZClamp;
	ColorLogic colorLogic;
	ColorWheel[] myColors;
	public float movespeed = 20.0f;
	public Transform[] lanes;
	int laneIndex = 1; // 0, 1, 2
	
	#region #1 Awake and Update
	void Awake ()
	{
		player = (Player)GetComponent<Player> ();
		// Remember their initial Z position and keep them there forever.
		worldZClamp = transform.position.z;
		
		// Initialize the colors according to the level rules
		colorLogic = (ColorLogic)GetComponent<ColorLogic> ();
		int NUM_COLORS = 3;
		myColors = new ColorWheel[NUM_COLORS];
		myColors [0] = ColorWheel.red;
		myColors [1] = ColorWheel.green;
		myColors [2] = ColorWheel.blue;
		
		// Cycle and render colors
		//CycleColor(true);
		RenderCurrentColor ();
	}

	void Update ()
	{
		TryMove ();
		TrySwapColor ();
		ClampToWorldZ (worldZClamp);
		colorLogic.Refresh ();
	}
	#endregion
	
	#region #2 Input Tries
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
		//if (Input.GetButtonDown ("LaneShiftLeft")) {
		//	ShiftLeft ();
		//} else if (Input.GetButtonDown ("LaneShiftRight")) {
		//	ShiftRight ();
		//}
	}
	
	/*
	 * Logic for switching colors or using abilities. If player is
	 * already the color of the button they are pushing, try to use
	 * that color's special power.
	 */
	void TrySwapColor ()
	{
		if (Input.GetKeyDown ("j")) {
			if (colorLogic.color == myColors [0]) {
				player.RegenHealth ();
			} else {
				colorLogic.color = myColors [0];
			}
		} else if (Input.GetKeyDown ("k")) {
			if (colorLogic.color == myColors [1]) {
				player.RegenHealth ();
			} else {
				colorLogic.color = myColors [1];
			}
		} else if (Input.GetKeyDown ("l")) {
			if (colorLogic.color == myColors [2]) {
				player.SlowDown ();
			} else {
				colorLogic.color = myColors [2];
			}
		}
	}
	#endregion
	
	#region #3 Character Movement
	/*
	 * Move the character in the specified direction with the specfied speed.
	 */
	void Move (Vector3 direction, float speed)
	{
		Vector3 movement = (direction.normalized * speed);
		movement *= Time.deltaTime;

		// Apply movement vector
		CharacterController biped = GetComponent<CharacterController> ();
		biped.Move (movement);
	}
	
	void ShiftLeft ()
	{
		if (laneIndex >= 1) {
			laneIndex--;
		}
		transform.position = (lanes [laneIndex].position);
	}
	
	void ShiftRight ()
	{
		if (laneIndex <= 1) {
			laneIndex++;
		}
		transform.position = (lanes [laneIndex].position);
	}
	
	/*
	 * Sets the character's position to the specified worldZ, preventing
	 * him from shifting.
	 */
	void ClampToWorldZ (float worldZ)
	{
		transform.position = new Vector3 (transform.position.x, transform.position.y, worldZ);
	}
	#endregion

	/*
	 * Refresh the current color on the character
	 */
	void RenderCurrentColor ()
	{
		colorLogic.Refresh ();
	}
	
}

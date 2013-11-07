using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	float worldZClamp;
	ColorLogic colorLogic;
	ColorWheel[] myColors;
	int currentColorIndex;
	public float movespeed = 20.0f;
	public GameObject leftCube;
	public GameObject rightCube;
	
	#region #1 Awake and Update
	void Awake ()
	{
		// Remember their initial Z position and keep them there forever.
		worldZClamp = transform.position.z;
		
		// Initialize the colors according to the level rules
		colorLogic = (ColorLogic)GetComponent<ColorLogic> ();
		int NUM_COLORS = 3;
		myColors = new ColorWheel[NUM_COLORS];
		myColors [0] = ColorWheel.blue;
		myColors [1] = ColorWheel.yellow;
		myColors [2] = ColorWheel.red;
		currentColorIndex = 0;
		
		// Cycle and render colors
		CycleColor(true);
		RenderCurrentColor ();
	}

	void Update ()
	{
		TryMove ();
		TrySwapColor ();
		ClampToWorldZ (worldZClamp);
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
	}
	
	void TrySwapColor ()
	{
		bool isSwapClockwise = Input.GetButtonDown ("Fire2");
		bool isSwapCounterClockwise = Input.GetButtonDown ("Fire1");
		if (isSwapClockwise || isSwapCounterClockwise) {
			CycleColor (isSwapClockwise);
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
	
	/*
	 * Sets the character's position to the specified worldZ, preventing
	 * him from shifting.
	 */
	void ClampToWorldZ (float worldZ)
	{
		transform.position = new Vector3 (transform.position.x, transform.position.y, worldZ);
	}
	#endregion
	
	void CycleColor (bool isCycleClockwise)
	{
		// Wrap around the array
		int increment = isCycleClockwise ? 1 : -1;
		int nextIndex = WrapColorIndex (currentColorIndex + increment);
		colorLogic.color = myColors [nextIndex];
		currentColorIndex = nextIndex;
		
		// Assign new colors to left and right cubes
		ColorLogic leftCubeColor = (ColorLogic)leftCube.gameObject.GetComponent<ColorLogic> ();
		ColorLogic rightCubeColor = (ColorLogic)rightCube.gameObject.GetComponent<ColorLogic> ();
		colorLogic.color = myColors [currentColorIndex];
		leftCubeColor.color = myColors [WrapColorIndex (currentColorIndex - 1)];
		rightCubeColor.color = myColors [WrapColorIndex (currentColorIndex + 1)];
		
		RenderCurrentColor ();
	}
	
	/*
	 * Wraps the specified index into the colors according to our color switching rules
	 */
	int WrapColorIndex (int newIndex)
	{
		int nextIndex;
		if (newIndex < 0) {
			nextIndex = myColors.Length - 1;
		} else if (newIndex >= myColors.Length) {
			nextIndex = 0;
		} else {
			nextIndex = newIndex;
		}
		
		return nextIndex;
	}
	
	/*
	 * Refresh the current color on the character
	 */
	void RenderCurrentColor ()
	{
		ColorLogic leftCubeColor = (ColorLogic)leftCube.gameObject.GetComponent<ColorLogic> ();
		ColorLogic rightCubeColor = (ColorLogic)rightCube.gameObject.GetComponent<ColorLogic> ();
		colorLogic.Refresh ();
		leftCubeColor.Refresh ();
		rightCubeColor.Refresh ();
	}
	
}

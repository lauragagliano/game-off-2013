using UnityEngine;
using System.Collections;

public class Treadmill : MonoBehaviour {
	
	public float scrollspeed = 0.3f;
	public float accelerationPerFrame = 0.0005f;
	public float maxspeed = 30.0f;
	
	void FixedUpdate ()
	{
		scrollspeed = Mathf.Min((scrollspeed + accelerationPerFrame), maxspeed);
		transform.Translate (new Vector3(0,0,-scrollspeed * Time.deltaTime));
	}
}

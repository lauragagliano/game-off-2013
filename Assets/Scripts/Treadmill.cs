using UnityEngine;
using System.Collections;

public class Treadmill : MonoBehaviour {
	
	public float startingSpeed = 10.0f;
	public float scrollspeed;
	public float accelerationPerFrame = 0.0005f;
	public float maxspeed = 30.0f;
	
	void Awake ()
	{
		scrollspeed = startingSpeed;
	}
	
	void FixedUpdate ()
	{
		scrollspeed = Mathf.Min((scrollspeed + accelerationPerFrame), maxspeed);
		transform.Translate (new Vector3(0,0,-scrollspeed * Time.deltaTime));
	}
	
	public void SlowDown ()
	{
		scrollspeed = (scrollspeed + startingSpeed)/2;
	}
}

using UnityEngine;
using System.Collections;

public class Treadmill : MonoBehaviour {
	
	public float scrollspeed = 0.3f;
	public float accelerationPerFrame = 0.0005f;
	public float maxspeed = 1.2f;
	
	void Update ()
	{
		scrollspeed = Mathf.Min((scrollspeed + accelerationPerFrame), maxspeed);
		transform.Translate (new Vector3(0,0,-scrollspeed));
	}
}

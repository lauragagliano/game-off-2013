using UnityEngine;
using System.Collections;

public class Treadmill : MonoBehaviour {
	
	public float scrollspeed = 0.1f;
	
	void Update () {
	
		transform.Translate (new Vector3(0,0,-scrollspeed));
	}
}

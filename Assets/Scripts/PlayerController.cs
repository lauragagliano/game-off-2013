using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	
	public float movespeed = 20.0f;
	
	void Update ()
	{
	
		float vertical = Input.GetAxis ("Vertical");
		if (Mathf.Abs (vertical) > 0) {
			transform.Translate (new Vector3 (-(vertical * movespeed * Time.deltaTime), 0, 0));
		}
	}
}

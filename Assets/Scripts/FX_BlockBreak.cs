using UnityEngine;
using System.Collections;

public class FX_BlockBreak : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/*
	 * Apply an explosive force to all the bits in this block
	 */
	public void Explode(Vector3 position, float force)
	{
		int i = 0;
		while (i < transform.childCount)
		{
			Transform child = transform.GetChild (i).transform;
			child.rigidbody.AddExplosionForce(force, position, 8.0f, 0.25f, ForceMode.Impulse);
			i++;
		}	
	}
}

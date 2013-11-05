using UnityEngine;
using System.Collections;

public class BlockLogic : MonoBehaviour
{
	ColorLogic blockColor;
	
	void Awake ()
	{
		blockColor = GetComponent<ColorLogic> ();
	}
	
	// Update is called once per frame
	void OnTriggerStay (Collider other)
	{
		ColorLogic otherColor = other.gameObject.GetComponent<ColorLogic> ();
		if (otherColor == null) {
			Debug.LogWarning ("Had a collision between objects that don't both have color logic.");
			return;
		}
		if (otherColor.isCompatible (blockColor)) {
			Debug.Log ("Colliding with compatible color.");
		} else {
			Debug.Log ("DEAD");
			other.gameObject.SetActive(false);
		}
	}
}

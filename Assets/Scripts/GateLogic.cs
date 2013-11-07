using UnityEngine;
using System.Collections;

public class GateLogic : MonoBehaviour
{
	ColorLogic gateColor;
	
	void Awake ()
	{
		gateColor = GetComponent<ColorLogic> ();
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider other)
	{
		ColorLogic otherColor = other.gameObject.GetComponent<ColorLogic> ();
		if (otherColor == null) {
			Debug.LogWarning ("Had a collision between objects that don't both have color logic.");
			return;
		}
		if (other.tag != Tags.PLAYER) {
			Debug.LogWarning ("Had a collision with a non-player object.");
			return;
		}
		if (otherColor.isCompatible (gateColor)) {
			ScoreKeeper.Instance.ScorePoint (gateColor.color);
		} else {
			Debug.Log ("DEAD");
			other.gameObject.SetActive(false);
		}
	}
}
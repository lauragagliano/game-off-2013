using UnityEngine;
using System.Collections;

public class BlockLogic : MonoBehaviour
{
	bool suckedUp;
	ColorLogic blockColor;
	
	void Awake ()
	{
		blockColor = GetComponent<ColorLogic> ();
	}
	
	void Update ()
	{
		if (suckedUp) {
			SuckUpBlock ();
		}	
	}
	
	// Update is called once per frame
	void OnTriggerStay (Collider other)
	{
		Debug.Log ("triggered");
		ColorLogic otherColor = other.gameObject.GetComponent<ColorLogic> ();
		if (otherColor == null) {
			Debug.LogWarning ("Had a collision between objects that don't both have color logic.");
			return;
		}
		if (otherColor.isCompatible (blockColor)) {
			ScoreKeeper.Instance.ScorePoint();
			SuckUpBlock ();
		} else {
			other.gameObject.SetActive(false);
		}
	}
	
	/*
	 * Play a transition to scale to nothing and then destroy the block.
	 */
	void SuckUpBlock()
	{
		suckedUp = true;
		transform.localScale = transform.localScale * 0.9f;
		if (transform.localScale.magnitude < 0.05f) {
			Destroy (gameObject);
		}
	}
	
	/*
	 * 
	 */
}

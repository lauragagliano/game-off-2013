using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
	bool collecting;
	
	void Awake ()
	{
	}
	
	void Update ()
	{
		if (collecting) {
			AnimateCollect ();
		} else {
			AnimateIdle ();
		}
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider other)
	{
		if (!other.CompareTag (Tags.PLAYER)) {
			Debug.LogWarning ("Had a block collision with something that's not the player. Exiting method.");
			return;
		}
		
		Player player = other.GetComponent<Player> ();
		player.AwardWildcard ();
		
		StartCollecting ();
	}
	
	void AnimateIdle ()
	{
		transform.Rotate (new Vector3 (-30, -60, -45) * Time.deltaTime);
	}
	
	/*
	 * Play a transition to scale to nothing and then destroy the pickup.
	 */
	void AnimateCollect ()
	{
		transform.localScale = transform.localScale * 0.9f;
		if (transform.localScale.magnitude < 0.05f) {
			Destroy (gameObject);
		}
	}

	/*
	 * Begin the collection animation and disable the collider
	 */
	void StartCollecting ()
	{
		collecting = true;
		collider.enabled = false;
	}
}

using UnityEngine;
using System.Collections;

public class BlockLogic : MonoBehaviour
{
	bool suckedUp;
	RGB blockRGB;
	public GameObject destroyFX;
	
	GameObject sucker;
	float originalDistance;
	
	float distanceScaleup = 0.1f;
	
	void Start ()
	{
		// TODO Refactor when pickups is its own class
		blockRGB = GetComponent<RGB> ();
		if (CompareTag (Tags.PICKUP)) {
			blockRGB.color = GameManager.Instance.player.playerRGB.color;
			blockRGB.Refresh ();
			GameManager.Instance.player.RememberPickup (gameObject);
		}
	}
	
	void LateUpdate ()
	{
		if (suckedUp) {
			AnimateSuckUp ();
		}	
	}
	
	/*
	 * When a player enters the trigger for a block, it means a collision has
	 * occurred with a black block. Play the animation and perform player collision
	 * logic.
	 */
	void OnTriggerEnter (Collider other)
	{
		if (!other.CompareTag (Tags.PLAYER)) {
			Debug.LogWarning ("Block collided with non-player object. Fix this.");
			return;
		}
		Player player = other.GetComponent<Player> ();
		BlowUp (other.transform.position);
		player.CollideWithBlock ();
	}

	/*
	 * Play a transition to scale to nothing and then destroy the block.
	 */
	void AnimateSuckUp ()
	{
		distanceScaleup *= 1.1f;
		float maxDistance = distanceScaleup * originalDistance;
		transform.position = Vector3.MoveTowards(transform.position, sucker.transform.position, maxDistance);
		
		transform.localScale = transform.localScale * 0.90f;
		if (Mathf.Abs (transform.position.x - sucker.transform.position.x) < 0.25f) {
			Destroy (gameObject);
		}
	}
	
	/*
	 * Verify we award the pickup when destroyed, if we had a valid sucker.
	 */
	void OnDestroy()
	{
		if(suckedUp && sucker != null) {
			GameManager.Instance.player.CollectPickup (gameObject);
		}
		GameObject playerObj =  GameObject.FindGameObjectWithTag (Tags.PLAYER);
		if (playerObj != null) {
			playerObj.GetComponent<Player> ().ForgetPickup (gameObject);
		}
	}

	/*
	 * Set the state of our block to sucked up and prevent further collisions.
	 */
	public void SuckUpBlock(GameObject suckerObject)
	{
		sucker = suckerObject;
		originalDistance = Vector3.Distance (sucker.transform.position, transform.position);
		suckedUp = true;
		transform.parent = null;
	}

	public void BlowUp(Vector3 position)
	{
		GameObject fx = (GameObject)Instantiate (destroyFX, transform.position,
				Quaternion.LookRotation (Vector3.forward, Vector3.up));
		FX_BlockBreak fxScript = (FX_BlockBreak)fx.GetComponent<FX_BlockBreak>();
		if(fxScript != null) {
			fxScript.Explode(position, 40);
			Destroy (fx, 3.0f);
			// Parent the explosion to the treadmill
			fx.transform.parent = transform.parent;
		}
		else {
			Destroy (fx, 0.8f);
		}
		Destroy (gameObject);
	}
}

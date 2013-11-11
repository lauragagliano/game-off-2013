using UnityEngine;
using System.Collections;

public class BlockLogic : MonoBehaviour
{
	bool suckedUp;
	RGB blockRGB;
	
	void Awake ()
	{
		blockRGB = GetComponent<RGB> ();
	}
	
	void Update ()
	{
		if (suckedUp) {
			AnimateSuckUp ();
		}	
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider other)
	{
		if (!other.CompareTag(Tags.PLAYER)) {
			Debug.LogWarning ("Had a block collision with something that's not the player. Exiting method.");
			return;
		}
		Player player = other.GetComponent<Player> ();
		player.HandleBlockCollision (blockRGB);
		if (player.playerRGB.isCompatible (blockRGB)) {
			SuckUpBlock ();
		} else {
			// BLOW UP BIG
			// TODO Spawn bricks with rigid bodies and DESTROY the block object
			if (particleEmitter != null) {
				particleEmitter.Emit ();
			}
			renderer.enabled = false;
		}
	}
	
	/*
	 * Play a transition to scale to nothing and then destroy the block.
	 */
	void AnimateSuckUp ()
	{
		transform.localScale = transform.localScale * 0.9f;
		if (transform.localScale.magnitude < 0.05f) {
			Destroy (gameObject);
		}
	}
	
	/*
	 * Set the state of our block to sucked up and prevent further collisions.
	 */
	void SuckUpBlock()
	{
		suckedUp = true;
		collider.enabled = false;
	}
}

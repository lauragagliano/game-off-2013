using UnityEngine;
using System.Collections;

public class BlockLogic : MonoBehaviour
{
	bool usedUp;
	ColorLogic blockColor;
	
	void Awake ()
	{
		blockColor = GetComponent<ColorLogic> ();
	}
	
	void Update ()
	{
		if (usedUp) {
			SuckUpBlock ();
		}	
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider other)
	{
		if (!other.CompareTag(Tags.PLAYER)) {
			Debug.LogWarning ("Had a block collision with something that's not the player. Exiting method.");
			return;
		}
		if (!usedUp) {
			Player player = other.GetComponent<Player> ();
			player.HandleBlockCollision (blockColor);
			if (player.playerColor.isCompatible (blockColor)) {
				SuckUpBlock ();
			} else {
				// BLOW UP BIG
				if (particleEmitter != null) {
					particleEmitter.Emit ();
				}
				renderer.enabled = false;
			}
		}
	}
	
	/*
	 * Play a transition to scale to nothing and then destroy the block.
	 */
	void SuckUpBlock()
	{
		usedUp = true;
		transform.localScale = transform.localScale * 0.9f;
		if (transform.localScale.magnitude < 0.05f) {
			Destroy (gameObject);
		}
	}
}

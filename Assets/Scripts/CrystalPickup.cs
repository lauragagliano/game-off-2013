using UnityEngine;
using System.Collections;

public class CrystalPickup : Pickup
{
	RGB crystalRGB;
	
	protected override void Start ()
	{
		base.Start ();
		// Get the appropriate color for the crystal
		crystalRGB = GetComponent<RGB> ();
		crystalRGB.color = GameManager.Instance.player.playerRGB.color;
		crystalRGB.Refresh ();
	}
	
	protected override void AnimateIdle ()
	{
		// Do not animate crystal pickups yet.
	}
	
	/*
	 * Start collecting the crystal
	 */
	public override void StartCollecting(GameObject pickingUpGameObject)
	{
		base.StartCollecting (pickingUpGameObject);
		
		// CrystalPickups need to detach from the treadmill
		transform.parent = null;
	}
	
}

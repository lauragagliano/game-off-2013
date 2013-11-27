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
	}
	
	protected override void AnimateIdle ()
	{
		transform.Rotate (new Vector3 (0, -60, 0u) * Time.deltaTime);
	}
	
	/*
	 * Start collecting the crystal
	 */
	public override void StartCollecting (GameObject pickingUpGameObject)
	{
		base.StartCollecting (pickingUpGameObject);
		
		// CrystalPickups need to detach from the treadmill
		transform.parent = null;
	}
	
}

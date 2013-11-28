using UnityEngine;
using System.Collections;

public class FX_Boost : MonoBehaviour
{
	
	public FX_BoostLine redLine;
	public FX_BoostLine greenLine;
	public FX_BoostLine blueLine;
	
	/*
	 * Stop all the BoostLines from emitting.
	 */
	public void StopEmitting ()
	{
		redLine.StopEmitting ();
		greenLine.StopEmitting ();
		blueLine.StopEmitting ();
	}
	
	/*
	 * Called by the boost lines when they are done emitting
	 */
	public void OnStoppedEmitting ()
	{
		GameManager.Instance.player.DoBlockClearExplosion ();
		Destroy (gameObject);
	}
}

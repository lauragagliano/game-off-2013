using UnityEngine;
using System.Collections;

/*
 * This singleton class will keep references to global objects
 * like the player and his/her statistics.
 */
public class GameManager : Singleton<GameManager>
{
	public Player player;
	
	public int numPickupsPassed;
	
	void Awake ()
	{
		player = GameObject.FindGameObjectWithTag (Tags.PLAYER).GetComponent<Player> ();
	}
	
	/*
	 * Check whether the player is in play. Returns false if the
	 * player has been destroyed or deactivated.
	 */
	public bool IsPlayerAlive ()
	{
		return player!= null && player.gameObject != null && player.gameObject.activeSelf;
	}
}

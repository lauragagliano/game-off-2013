using UnityEngine;
using System.Collections;

/*
 * This singleton class will keep references to global objects
 * like the player and his/her statistics.
 */
public class GameManager : Singleton<GameManager>
{
	public Player player;
	public RGB playerRGB;
	
	public Difficulty difficulty = Difficulty.Easy;
	public int numPickupsPassed;
	public int redPoints;
	public int greenPoints;
	public int bluePoints;
	
	Transform playerSpawn;
	Treadmill treadmill;
	
	const int MEDIUM_THRESHOLD = 300; // Number of pickups passed
	const int HARD_THRESHOLD = 2000;
	
	public enum Difficulty {
		Easy,
		Medium,
		Hard
	}
	
	void Awake ()
	{
		player = GameObject.FindGameObjectWithTag (Tags.PLAYER).GetComponent<Player> ();
		playerRGB = player.GetComponentInChildren<RGB> ();
		playerSpawn = (Transform)GameObject.Find (ObjectNames.PLAYER_SPAWN).transform;
		treadmill = (Treadmill)GameObject.Find(ObjectNames.TREADMILL).GetComponent<Treadmill> ();
	}
	
	void Update ()
	{
		UpdateDifficulty ();
	}
	
	/*
	 * Check whether the player is in play. Returns false if the
	 * player has been destroyed or deactivated.
	 */
	public bool IsPlayerAlive ()
	{
		return player!= null && player.gameObject != null && player.gameObject.activeSelf;
	}
	
		
	/*
	 * Check how many pickups we've passed. Once we've passed the threshold for medium
	 * and hard, update the difficulty respectively.
	 */
	void UpdateDifficulty ()
	{
		if (numPickupsPassed > MEDIUM_THRESHOLD) {
			difficulty = Difficulty.Medium;
		} else if (numPickupsPassed > HARD_THRESHOLD) {
			difficulty = Difficulty.Hard;
		}
	}
	
	/*
	 * Return true if difficulty is set to easy.
	 */
	public bool isEasy ()
	{
		return difficulty == Difficulty.Easy;
	}
	
	/*
	 * Return true if difficulty is set to medium.
	 */
	public bool isMedium ()
	{
		return difficulty == Difficulty.Medium;
	}
	
	/*
	 * Return true if difficulty is set to hard.
	 */
	public bool isHard ()
	{
		return difficulty == Difficulty.Hard;
	}
	
	/*
	 * Add a point to the respective color currency. Accepts a ColorWheel color, which
	 * should be Red, Green, or Blue and adds to that color's points.
	 */
	public void AddPoint (ColorWheel colorToAddTo)
	{
		switch (colorToAddTo) {
		case ColorWheel.red:
			redPoints++;
			break;
		case ColorWheel.green:
			greenPoints++;
			break;
		case ColorWheel.blue:
			bluePoints++;
			break;
		}
	}
	
	/*
	 * Restart necessary data, respawn player, and restart the treadmill.
	 */
	public void StartGame ()
	{
		numPickupsPassed = 0;
		difficulty = Difficulty.Easy;
		player.gameObject.SetActive (true);
		player.transform.position = playerSpawn.position;
		treadmill.ResetTreadmill ();
	}
}

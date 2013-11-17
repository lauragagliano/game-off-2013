using UnityEngine;
using System.Collections;

/*
 * This singleton class will keep references to global objects
 * like the player and his/her statistics.
 */
public class GameManager : Singleton<GameManager>
{
	public Player player;
	
	Difficulty difficulty = Difficulty.Easy;
	GameScreen gameScreen = GameScreen.Game;
	
	public int numPickupsPassed;
	public int redPoints;
	public int greenPoints;
	public int bluePoints;
	
	Transform playerSpawn;
	Treadmill treadmill;
	Store store;
	
	const int MEDIUM_THRESHOLD = 300; // Number of pickups passed
	const int HARD_THRESHOLD = 2000;
	
	enum Difficulty {
		Easy,
		Medium,
		Hard
	}
	
	enum GameScreen {
		Menu,
		Game,
		Dead,
		Inventory,
		Store
	}
	
	void Awake ()
	{
		player = GameObject.FindGameObjectWithTag (Tags.PLAYER).GetComponent<Player> ();
		playerSpawn = (Transform)GameObject.Find (ObjectNames.PLAYER_SPAWN).transform;
		treadmill = (Treadmill)GameObject.Find (ObjectNames.TREADMILL).GetComponent<Treadmill> ();
		store = (Store)GameObject.Find (ObjectNames.STORE).GetComponent<Store> ();
	}
	
	void Update ()
	{
		UpdateDifficulty ();
	}
	
	/*
	 * Check whether the player is in play. Returns false if the
	 * player has been destroyed or deactivated.
	 */
	public bool CheckIfPlayerLiving ()
	{
		if (player == null || player.gameObject == null || !player.gameObject.activeSelf) {
			gameScreen = GameScreen.Dead;
			return false;
		}
		return true;
	}
	
	/*
	 * Return true if the game should be on the Dead screen.
	 */
	public bool IsDead ()
	{
		return gameScreen == GameScreen.Dead;
	}
	
	/*
	 * Return true if the game should be on the Store screen.
	 */
	public bool IsShopping ()
	{
		return gameScreen == GameScreen.Store;
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
	public bool IsEasy ()
	{
		return difficulty == Difficulty.Easy;
	}
	
	/*
	 * Return true if difficulty is set to medium.
	 */
	public bool IsMedium ()
	{
		return difficulty == Difficulty.Medium;
	}
	
	/*
	 * Return true if difficulty is set to hard.
	 */
	public bool IsHard ()
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
		gameScreen = GameScreen.Game;
		difficulty = Difficulty.Easy;
		player.gameObject.SetActive (true);
		player.InitializeStats ();
		player.transform.position = playerSpawn.position;
		treadmill.ResetTreadmill ();
	}
	
	public void EnterStore ()
	{
		gameScreen = GameScreen.Store;
		store.EnterStore ();
	}
	
	public void ExitStore ()
	{
		store.ExitStore ();
		gameScreen = GameScreen.Menu;
	}
}

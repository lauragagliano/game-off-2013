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
	GameState gameState = GameState.Running;
	public int numPickupsPassed;
	public int redPoints;
	public int greenPoints;
	public int bluePoints;
	Transform playerSpawn;
	Treadmill treadmill;
	Store store;
	public GUI_WildcardReveal wildcardGUI;
	public GameObject WildcardRevealGO;
	const int MEDIUM_THRESHOLD = 300; // Number of pickups passed
	const int HARD_THRESHOLD = 2000;
	
	enum Difficulty
	{
		Easy,
		Medium,
		Hard
	}
	
	enum GameState
	{
		Menu,
		Running,
		Dead,
		WildcardReveal,
		Store
	}
	
	void Awake ()
	{
		LinkSceneObjects();
	}
	
	/* Search for and assign references to scene objects the GameManager needs to know about.
	 */
	void LinkSceneObjects()
	{
		player = GameObject.FindGameObjectWithTag (Tags.PLAYER).GetComponent<Player> ();
		
		// Link Player Spawn
		GameObject foundObject = GameObject.Find (ObjectNames.PLAYER_SPAWN);
		if (foundObject != null) {
			playerSpawn = (Transform)foundObject.transform;
		}
		
		// Link Treadmill
		foundObject = GameObject.Find (ObjectNames.TREADMILL);
		if(foundObject == null)
		{
			Debug.LogError("Cannot find treadmill. This map will not work without a Treadmill object.");
			return;
		}
		treadmill = (Treadmill)foundObject.GetComponent<Treadmill> ();
		
		// Link Store
		foundObject = GameObject.Find (ObjectNames.STORE);
		if(foundObject != null )
		{
			store = (Store)foundObject.GetComponent<Store> ();
		}
		
		// Link Wildcard Reveal UI
		foundObject = GameObject.Find (ObjectNames.GUI_WILDCARD_REVEAL);
		if(foundObject != null ) {
			wildcardGUI = (GUI_WildcardReveal)foundObject.GetComponent<GUI_WildcardReveal> ();
		}
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
			gameState = GameState.Dead;
			return false;
		}
		return true;
	}
	
	/*
	 * Return true if the game should be on the Dead screen.
	 */
	public bool IsDead ()
	{
		return gameState == GameState.Dead;
	}
	
	/*
	 * Return true if the game is running.
	 */
	public bool IsRunning ()
	{
		return gameState == GameState.Running;
	}
	
	/*
	 * Return true if the game should be on the Store screen.
	 */
	public bool IsShopping ()
	{
		return gameState == GameState.Store;
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
		gameState = GameState.Running;
		difficulty = Difficulty.Easy;
		player.gameObject.SetActive (true);
		player.InitializeStats ();
		player.transform.position = playerSpawn.position;
		treadmill.ResetTreadmill ();
	}
	
	/*
	 * Ends the current run for the player.
	 */
	public void EndRun ()
	{
		if (player.WildcardCount > 0) {
			GoToWildCardState ();
		} else {
			gameState = GameState.Dead;
		}
	}
	
	void GoToWildCardState ()
	{
		gameState = GameState.WildcardReveal;
		wildcardGUI.ShowCards (player.WildcardCount);
	}
	
	public void EnterStore ()
	{
		gameState = GameState.Store;
		store.EnterStore ();
	}
	
	public void ExitStore ()
	{
		store.ExitStore ();
		gameState = GameState.Menu;
	}
}

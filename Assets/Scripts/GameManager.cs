using UnityEngine;
using System.Collections;

/*
 * This singleton class will keep references to global objects
 * like the player and his/her statistics.
 */
public class GameManager : Singleton<GameManager>
{
	public Player player;
	// TODO make this private when we don't need to show difficulty on screen
	public Difficulty difficulty = Difficulty.Easy;
	GameState gameState = GameState.Running;
	public int numPickupsPassed;
	public int numPointsThisRound;
	Transform playerSpawn;
	public Treadmill treadmill;
	Store store;
	GameObject wildcardRevealerPrefab;
	GUI_WildcardReveal wildcardRevealer;
	public GameObject WildcardRevealGO;
	const int MEDIUM_THRESHOLD = 300; // Number of pickups passed
	const int HARD_THRESHOLD = 2000;
	float timeDeathDelayStarted;
	float deathDelayTime = 1.0f;
	
	// TODO make this private when we don't need to show difficulty on screen
	public enum Difficulty
	{
		Easy,
		Medium,
		Hard
	}
	
	enum GameState
	{
		Menu,
		Tutorial,
		Running,
		DeathDelay,
		WildcardReveal,
		Reviving,
		GameOver,
		Store
	}
	
	void Awake ()
	{
		LoadReferencedObjects ();
		LinkSceneObjects ();
		
		GoToMainMenu ();
	}
	
	/*
	 * Loads any prefabs that need to be referenced by the GameManager.
	 */
	void LoadReferencedObjects ()
	{
		wildcardRevealerPrefab = (GameObject)Resources.Load (ObjectNames.GUI_WILDCARD_REVEAL, typeof(GameObject));
	}
	
	/* Search for and assign references to scene objects the GameManager needs to know about.
	 */
	void LinkSceneObjects ()
	{
		player = GameObject.FindGameObjectWithTag (Tags.PLAYER).GetComponent<Player> ();
		
		// Link Player Spawn
		GameObject foundObject = GameObject.Find (ObjectNames.PLAYER_SPAWN);
		if (foundObject != null) {
			playerSpawn = (Transform)foundObject.transform;
		}
		
		// Link Treadmill
		foundObject = GameObject.Find (ObjectNames.TREADMILL);
		if (foundObject == null) {
			Debug.LogError ("Cannot find treadmill. This map will not work without a Treadmill object.");
			return;
		}
		treadmill = (Treadmill)foundObject.GetComponent<Treadmill> ();
		
		// Link Store
		foundObject = GameObject.Find (ObjectNames.STORE);
		if (foundObject != null) {
			store = (Store)foundObject.GetComponent<Store> ();
		}
	}
	
	void Update ()
	{
		if (gameState == GameState.Tutorial) {
			if (Input.anyKeyDown) {
				GoToRunning(false);
			}
		} else if (gameState == GameState.Running) {
			UpdateDifficulty ();
		} else if (gameState == GameState.DeathDelay) {
			if (Time.time > timeDeathDelayStarted + deathDelayTime) {
				if (player.WildcardCount > 0) {
					GoToWildCardState ();
				} else {
					GoToGameOver ();
				}
			}
		}
			
	}
	
	/*
	 * Return true if game is in Menu state.
	 */
	public bool IsOnMenu ()
	{
		return gameState == GameState.Menu;
	}
	
	/*
	 * Return true if the game and all subsequent states are over and we are ready to retry to go to main.
	 */
	public bool IsGameOver ()
	{
		return gameState == GameState.GameOver;
	}
	
	/*
	 * Return true if the game is running.
	 */
	public bool IsPlaying ()
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
	 * Add a point to our score, to be displayed at the end of each round.
	 */
	public void AddPoint ()
	{
		numPointsThisRound++;
	}
	
	/*
	 * Starts the infinite running game. Begins with a tutorial if requested.
	 */
	public void StartGame (bool showTutorial)
	{
		InitializeGame ();
		
		if (showTutorial) {
			GoToTutorial ();
		} else {
			GoToRunning (false);
		}
	}
	
	/*
	 * Does all required data reseting to begin a game.
	 */
	private void InitializeGame ()
	{
		// TODO We could improve performance by turning off objects as well as cameras
		Camera menuCamera = GameObject.Find (ObjectNames.MENU_CAMERA).camera;
		Camera gameCamera = GameObject.Find (ObjectNames.GAME_CAMERA).camera;
		Camera storeCamera = GameObject.Find (ObjectNames.STORE_CAMERA).camera;
		menuCamera.enabled = false;
		gameCamera.enabled = true;
		storeCamera.enabled = false;
		
		numPickupsPassed = 0;
		numPointsThisRound  = 0;
		difficulty = Difficulty.Easy;
		
		SpawnPlayer ();
		
		treadmill.ResetTreadmill ();
	}
	
	/*
	 * Revives the game in the last configuration.
	 */
	public void RevivePlayer ()
	{
		SpawnPlayer ();
		GoToReviving();
	}
	
	/*
	 * Spawn the player entity or respawn him if he's dead.
	 */
	private void SpawnPlayer ()
	{
		player.Spawn (playerSpawn.position);
	}
	
	/*
	 * Ends the current run for the player.
	 */
	public void EndRun ()
	{
		GoToDeathDelay ();
	}
	
	/*
	 * Puts the game manager into the state where it waits for the player's death animation.
	 */
	void GoToDeathDelay ()
	{
		gameState = GameState.DeathDelay;
		timeDeathDelayStarted = Time.time;
		treadmill.PauseScrolling ();
	}
	
	/*
	 * Puts the game manager in the state where it awaits player's retry or main menu command.
	 */
	public void GoToGameOver ()
	{
		gameState = GameState.GameOver;
	}
	
	/*
	 * Puts the game manager in the state where it awaits player's retry or main menu command.
	 */
	public void GoToReviving ()
	{
		gameState = GameState.Reviving;
	}
	
	/*
	 * Called by the Player when he's done reviving.
	 */
	public void OnReviveDone()
	{
		GoToRunning(true);
	}
	
	/*
	 * Puts the game manager in the state where it reveals the wildcards.
	 */
	void GoToWildCardState ()
	{
		gameState = GameState.WildcardReveal;
		
		// Spawn wildcard revealer
		GameObject spawnedObject = (GameObject)Instantiate (wildcardRevealerPrefab, wildcardRevealerPrefab.transform.position,
				Quaternion.LookRotation (Vector3.forward, Vector3.up));
		wildcardRevealer = spawnedObject.GetComponent<GUI_WildcardReveal> ();
		wildcardRevealer.StartShowing (player.WildcardCount);
	}
	
	/*
	 * Go to the store section of our scene.
	 */
	public void GoToStore ()
	{
		gameState = GameState.Store;
		// TODO We could improve performance by turning off objects as well as cameras
		Camera menuCamera = GameObject.Find (ObjectNames.MENU_CAMERA).camera;
		Camera gameCamera = GameObject.Find (ObjectNames.GAME_CAMERA).camera;
		Camera storeCamera = GameObject.Find (ObjectNames.STORE_CAMERA).camera;
		menuCamera.enabled = false;
		gameCamera.enabled = false;
		storeCamera.enabled = true;
	}
	
	/*
	 * Go to the running state.
	 */
	private void GoToRunning (bool isResuming)
	{
		gameState = GameState.Running;
		if(isResuming) {
			treadmill.ResumeTreadmill();
		} else {
			treadmill.StartScrolling ();
		}
	}
	
	/*
	 * Return to the main menu.
	 */
	public void GoToMainMenu ()
	{
		// TODO This is a good place to deactivate assets as well
		gameState = GameState.Menu;
		Camera menuCamera = GameObject.Find (ObjectNames.MENU_CAMERA).camera;
		Camera gameCamera = GameObject.Find (ObjectNames.GAME_CAMERA).camera;
		Camera storeCamera = GameObject.Find (ObjectNames.STORE_CAMERA).camera;
		menuCamera.enabled = true;
		storeCamera.enabled = false;
		gameCamera.enabled = false;
		
		treadmill.PauseScrolling ();
	}
	
	/*
	 * Put the game in the tutorial state
	 */
	void GoToTutorial ()
	{
		gameState = GameState.Tutorial;
		
		treadmill.ShowTutorial ();
	}
}

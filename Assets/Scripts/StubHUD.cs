using UnityEngine;
using System.Collections;

public class StubHUD : MonoBehaviour
{
	public Player player;
	public GUIText startEndText;
	public GUIText helpText;
	public GUIText scoreText;

	void Awake ()
	{
		helpText.text = "A: LEFT\nD: RIGHT\n\nJ: RED\nK: GREEN\nL: BLUE\n\n(Tap Twice for POAWAHH";
		SetItemTexts ();
	}
	
	void Update ()
	{

	}
	
	void OnGUI ()
	{
		if (GameManager.Instance.IsOnMenu ()) {
			DisplayMainMenu ();
		} else if (GameManager.Instance.IsGameOver ()) {
			DisplayDeadMenu ();
		} else if (GameManager.Instance.IsShopping ()) {
			DisplayStoreMenu ();
		} else {
			startEndText.text = string.Empty;
		}
		scoreText.text = string.Format ("Passed Pigments: {0}\nHealth: {1}\nWildcards: {2}\nMoney: {3}\nDifficulty: {4}",
			GameManager.Instance.numPickupsPassed, player.curHealth, player.WildcardCount, player.money,
			GameManager.Instance.difficulty);
	}
	
	/*
	 * Display the menu for when the player is dead. Receive inputs and call
	 * the appropriate GameManager implemented method.
	 */
	void DisplayDeadMenu ()
	{
		startEndText.text = "Game Over!";
		GUILayout.BeginArea (new Rect (Screen.width / 2 - 50.0f, Screen.height / 2, 200.0f, 70.0f));
		if (GUILayout.Button ("Click to Retry")) {
			//Application.LoadLevel (Application.loadedLevel);
			GameManager.Instance.StartGame (false);
		}
		if (GUILayout.Button ("Go to Store")) {
			GameManager.Instance.GoToStore ();
		}
		GUILayout.EndArea ();
	}
	
	/*
	 * Display the menu for when the player is at the store. Receive inputs and call
	 * the appropriate GameManager implemented methods.
	 */
	void DisplayStoreMenu ()
	{
		startEndText.text = string.Empty;
		Store store = (Store)GameObject.Find (ObjectNames.STORE).GetComponent<Store> ();
		
		// TODO Let's at least make the Buy/AlreadyOwned a 3d button on the item mesh
		GUILayout.BeginArea (new Rect (Screen.width - 220.0f, Screen.height - 70.0f, 200.0f, 70.0f));
		if (store.IsAlreadyPurchased ()) {
			GUILayout.Button ("Already Owned");
		}
		else if (!store.HasEnoughMoney ()) {
			GUILayout.Button ("Not Enough Money");
		} else {
			if (GUILayout.Button ("Buy")) {
				store.BuyItem ();
			}
		}
		if (GUILayout.Button ("Start Game")) {
			GameManager.Instance.GoToGame ();
			GameManager.Instance.StartGame (false);
		}
		GUILayout.EndArea ();
	}
	
	void SetItemTexts ()
	{
		GameObject[] itemObjs = GameObject.FindGameObjectsWithTag (Tags.ITEM);
		foreach (GameObject obj in itemObjs) {
			Item item = obj.GetComponent<Item> ();
			TextMesh itemText = obj.GetComponentInChildren<TextMesh> ();
			itemText.text = string.Format ("{0}\n\nCost: {1}", item.itemName, item.cost);
		}
	}
	
	/*
	 * Show the buttons for the main menu and include logic when buttons
	 * are pressed.
	 */
	void DisplayMainMenu ()
	{
		GUILayout.BeginArea (new Rect (Screen.width - 220.0f, Screen.height - 70.0f, 200.0f, 70.0f));
		if (GUILayout.Button ("Start Game [ENTER]")) {
			GameManager.Instance.GoToGame ();
			GameManager.Instance.StartGame (true);
		}
		if (GUILayout.Button ("Shop")) {
			GameManager.Instance.GoToStore ();
		}
		GUILayout.EndArea ();
	}
}

using UnityEngine;
using System.Collections;

public class StubHUD : MonoBehaviour
{
	public Player player;
	public GUIText finalDistanceLabelText;
	public GUIText finalDistanceText;
	public GUIText crystalsCollectedLabelText;
	public GUIText crystalsCollectedText;
	public GUIText moneyText;
	public GUIText distanceText;
	public GUIText debugText;
	
	Treadmill treadmill;

	void Awake ()
	{
		SetItemTexts ();
		treadmill = GameObject.Find (ObjectNames.TREADMILL).GetComponent<Treadmill> ();
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
			DisplayInGameText ();
		}
	}
	
	/*
	 * Display the distance and any other in game text we need to show the player.
	 */
	void DisplayInGameText ()
	{
		EnableInGameText (true);
		EnableGameOverText (false);
		distanceText.text = "Distance: " + Mathf.RoundToInt (treadmill.distanceTraveled);
		PrintMoneyToScreen ();
		debugText.text = string.Format ("Passed Pigments: {0}\nHealth: {1}\nWildcards: {2}\nDifficulty: {3}",
			GameManager.Instance.numPickupsPassed, player.curHealth, player.WildcardCount,
			GameManager.Instance.difficulty);
	}
	
	/*
	 * Display the menu for when the player is dead. Receive inputs and call
	 * the appropriate GameManager implemented method.
	 */
	void DisplayDeadMenu ()
	{
		EnableInGameText (false);
		EnableGameOverText (true);
		finalDistanceLabelText.text = ("You had\n a colorful run of");
		finalDistanceText.text = Mathf.RoundToInt (treadmill.distanceTraveled) + "m";
		crystalsCollectedLabelText.text = "Crystals Collected";
		crystalsCollectedText.text = GameManager.Instance.numPointsThisRound.ToString ();
		GUILayout.BeginArea (new Rect (Screen.width / 2 - 50.0f, Screen.height - 70.0f, 200.0f, 70.0f));
		if (GUILayout.Button ("Click to Retry")) {
			//Application.LoadLevel (Application.loadedLevel);
			GameManager.Instance.StartGame (false);
			EnableInGameText (true);
		}
		if (GUILayout.Button ("Go to Store")) {
			GameManager.Instance.GoToStore ();
		}
		GUILayout.EndArea ();
	}
	
	/*
	 * Helper method to display money on GUIText object.
	 */
	void PrintMoneyToScreen ()
	{
		moneyText.enabled = true;
		moneyText.text = "Money: " + player.money;
	}
	
	/*
	 * Display the menu for when the player is at the store. Receive inputs and call
	 * the appropriate GameManager implemented methods.
	 */
	void DisplayStoreMenu ()
	{
		EnableGameOverText (false);
		EnableInGameText (false);
		PrintMoneyToScreen ();
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
		EnableInGameText (false);
		EnableGameOverText (false);
		if (GUILayout.Button ("Start Game [ENTER]")) {
			GameManager.Instance.StartGame (true);
		}
		if (GUILayout.Button ("Shop")) {
			GameManager.Instance.GoToStore ();
		}
		GUILayout.EndArea ();
	}
	
	/*
	 * Helper method to enable or disable in game text.
	 */
	void EnableInGameText (bool enable)
	{
		moneyText.enabled = enable;
		distanceText.enabled = enable;
		debugText.enabled = enable;
	}
	
	/*
	 * Helper method to enable or disable game over text elements.
	 */
	void EnableGameOverText (bool enable)
	{
		finalDistanceLabelText.enabled = enable;
		finalDistanceText.enabled = enable;
		crystalsCollectedText.enabled = enable;
		crystalsCollectedLabelText.enabled = enable;
	}
}

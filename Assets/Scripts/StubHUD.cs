using UnityEngine;
using System.Collections;

public class StubHUD : MonoBehaviour
{
	public Player player;
	public GUIText startEndText;
	public GUIText helpText;
	public GUIText scoreText;
	public GameObject redMeterGO;
	GUIMeter redMeter;
	public GameObject blueMeterGO;
	GUIMeter blueMeter;
	public GameObject greenMeterGO;
	GUIMeter greenMeter;
	
	void Awake ()
	{
		helpText.text = "A: LEFT\nD: RIGHT\n\nJ: RED\nK: GREEN\nL: BLUE\n\n(Tap Twice for POAWAHH";
		redMeter = redMeterGO.GetComponent<GUIMeter> ();
		blueMeter = blueMeterGO.GetComponent<GUIMeter> ();
		greenMeter = greenMeterGO.GetComponent<GUIMeter> ();
	}
	
	void Update ()
	{

	}
	
	void OnGUI ()
	{
		if (GameManager.Instance.IsGameOver ()) {
			DisplayDeadMenu ();
		} else if (GameManager.Instance.IsShopping ()){
			DisplayStoreMenu ();
		} else {
			startEndText.text = string.Empty;
		}
		scoreText.text = string.Format ("Passed Pigments: {0}\nHealth: {1}\nWildcards: {2}",
			GameManager.Instance.numPickupsPassed, player.curHealth, player.WildcardCount);
		
		redMeter.CurrentFillPercent = player.redPower.GetFillPercentage ();
		greenMeter.CurrentFillPercent = player.greenPower.GetFillPercentage ();
		blueMeter.CurrentFillPercent = player.bluePower.GetFillPercentage ();
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
			GameManager.Instance.StartGame ();
		}
		if (GUILayout.Button ("Go to Store")) {
			GameManager.Instance.EnterStore ();
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
		Store store = (Store) GameObject.Find (ObjectNames.STORE).GetComponent<Store> ();
		string buyOrPurchased = "Already Owned";
		if (store.DisplayBuyForSelectedItem ()) {
			buyOrPurchased = "Buy";
		}
		
		// TODO Let's at least make the Buy/AlreadyOwned a 3d button on the item mesh
		GUILayout.BeginArea (new Rect (Screen.width - 220.0f, Screen.height - 70.0f, 200.0f, 70.0f));
		if (GUILayout.Button (buyOrPurchased) && buyOrPurchased == "Buy") {
			store.BuyItem ();
		}
		if (GUILayout.Button ("Start Game")) {
			GameManager.Instance.ExitStore ();
			GameManager.Instance.StartGame ();
		}
		GUILayout.EndArea ();
	}
}

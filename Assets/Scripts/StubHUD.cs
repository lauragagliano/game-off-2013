using UnityEngine;
using System.Collections;

public class StubHUD : MonoBehaviour
{
	public Player player;
	public GUIText finalDistanceLabelText;
	public GUIText finalDistanceLabelTextShadow;
	public GUIText finalDistanceText;
	public GUIText finalDistanceTextShadow;
	public GUIText crystalsCollectedLabelText;
	public GUIText crystalsCollectedLabelTextShadow;
	public GUIText crystalsCollectedText;
	public GUIText crystalsCollectedTextShadow;
	public GUIText moneyText;
	public GUIText distanceText;
	public GUIText debugText;
	public GUIText pressSpaceText;
	public GUIText pressSpaceTextShadow;
	
	public Texture leftArrowTexture;
	public Texture rightArrowTexture;
	
	public GUIStyle areaStyle;
	public GUIStyle redButtonStyle;
	public GUIStyle blueButtonStyle;
	public GUIStyle greenButtonStyle;
	public GUIStyle greyButtonStyle;
	
	const float AREA_WIDTH = 400.0f;
	const float AREA_HEIGHT = 350.0f;
	
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
		// Display the tutorial text
		if (!treadmill.IsShowingTutorial ()) {
			SetShadowGUIText (string.Empty, pressSpaceText, pressSpaceTextShadow);
			pressSpaceText.enabled = false;
			pressSpaceTextShadow.enabled = false;
		} else {
			SetShadowGUIText ("Press [Space] to Start", pressSpaceText, pressSpaceTextShadow);
			pressSpaceText.enabled = true;
			pressSpaceTextShadow.enabled = true;
		}
		EnableGameOverText (false);
		distanceText.text = "Distance: " + Mathf.RoundToInt (treadmill.distanceTraveled);
		PrintMoneyToScreen ();
		if (GameManager.Instance.DEBUG_MODE) {
			debugText.text = string.Format ("Passed Pigments: {0}\nHealth: {1}\nWildcards: {2}\nDifficulty: {3}",
				GameManager.Instance.numPickupsPassed, player.curHealth, player.WildcardCount,
				GameManager.Instance.difficulty);
		}
	}
	
	/*
	 * Display the menu for when the player is dead. Receive inputs and call
	 * the appropriate GameManager implemented method.
	 */
	void DisplayDeadMenu ()
	{
		EnableInGameText (false);
		EnableGameOverText (true);
		SetShadowGUIText ("You Survived For", finalDistanceLabelText, finalDistanceLabelTextShadow);
		int finalDistance = Mathf.RoundToInt (treadmill.distanceTraveled);
		SetShadowGUIText (finalDistance + "m", finalDistanceText, finalDistanceTextShadow);
		SetShadowGUIText ("Crystals Collected", crystalsCollectedLabelText, crystalsCollectedLabelTextShadow);
		SetShadowGUIText (GameManager.Instance.numPointsThisRound.ToString (), crystalsCollectedText,
			crystalsCollectedTextShadow);

		GUILayout.BeginArea (new Rect ((Screen.width - AREA_WIDTH)/2, (Screen.height - AREA_HEIGHT), AREA_WIDTH, AREA_HEIGHT), areaStyle);
		// Push our buttons down to bottom of screen
		GUILayout.BeginVertical ();
		GUILayout.FlexibleSpace ();
		// Center our buttons with space on both sides
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("Go to Store", greenButtonStyle)) {
			GameManager.Instance.GoToStore ();
		}
		if (GUILayout.Button ("Retry [Enter]", blueButtonStyle)) {
			//Application.LoadLevel (Application.loadedLevel);
			GameManager.Instance.StartGame (true);
			EnableInGameText (true);
		}
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
		GUILayout.EndArea ();
		
		if (Input.GetKeyDown (KeyCode.KeypadEnter) || Input.GetKeyDown (KeyCode.Return)) {
			GameManager.Instance.StartGame (true);
			EnableInGameText (true);
		}
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

		// Add our left and right arrows
		GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
		GUILayout.BeginVertical ();
		// Push our arrows to center
		GUILayout.FlexibleSpace ();
		GUILayout.BeginHorizontal ();
		if (store.selectedItem != 0) {
			if (GUILayout.Button (leftArrowTexture, GUIStyle.none)) {
				store.ScrollToPrevious ();
			}
		}
		GUILayout.FlexibleSpace ();
		if (store.selectedItem != store.allItems.Length -1) {
			if (GUILayout.Button (rightArrowTexture, GUIStyle.none)) {
				store.ScrollToNext ();
			}
		}
		GUILayout.EndHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.EndVertical ();
		GUILayout.EndArea ();
		
		// TODO Let's at least make the Buy/AlreadyOwned a 3d button on the item mesh
		GUILayout.BeginArea (new Rect (Screen.width - AREA_WIDTH, (Screen.height - AREA_HEIGHT), AREA_WIDTH, AREA_HEIGHT), areaStyle);
		// Push our buttons to bottom of screen
		GUILayout.BeginVertical ();
		GUILayout.FlexibleSpace ();
		// Push our buttons to right of screen.
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		if (store.IsAlreadyPurchased ()) {
			GUILayout.Button ("Already Owned", greyButtonStyle);
		}
		else if (!store.HasEnoughMoney ()) {
			GUILayout.Button ("Not Enough Money", redButtonStyle);
		} else {
			if (GUILayout.Button ("Buy (" + store.GetSelectedItem ().cost + ")", greenButtonStyle)) {
				store.BuyItem ();
			}
		}
		GUILayout.EndHorizontal ();
		// Push more buttons to the right of the screen
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("Play", blueButtonStyle)) {
			GameManager.Instance.StartGame (true);
		}
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
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
		EnableInGameText (false);
		EnableGameOverText (false);
		GUILayout.BeginArea (new Rect ((Screen.width - AREA_WIDTH)/2, (Screen.height - AREA_HEIGHT), AREA_WIDTH, AREA_HEIGHT), areaStyle);
		// Push buttons to the bottom of the screen
		GUILayout.BeginVertical ();
		GUILayout.FlexibleSpace ();
		GUILayout.BeginHorizontal ();
		// Push buttons to the center of the screen
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("Go to Store", greenButtonStyle)) {
			GameManager.Instance.GoToStore ();
		}
		if (GUILayout.Button ("Start Game [ENTER]", blueButtonStyle)) {
			GameManager.Instance.StartGame (true);
		}
		// Push more buttons to the center of the screen
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
		GUILayout.EndArea ();
		
		if (Input.GetKeyDown (KeyCode.KeypadEnter) || Input.GetKeyDown (KeyCode.Return)) {
			GameManager.Instance.StartGame (true);
		}
	}
	
	/*
	 * Helper method to enable or disable in game text.
	 */
	void EnableInGameText (bool enable)
	{
		pressSpaceText.enabled = enable;
		pressSpaceTextShadow.enabled = enable;
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
		finalDistanceLabelTextShadow.enabled = enable;
		finalDistanceText.enabled = enable;
		finalDistanceTextShadow.enabled = enable;
		crystalsCollectedText.enabled = enable;
		crystalsCollectedTextShadow.enabled = enable;
		crystalsCollectedLabelText.enabled = enable;
		crystalsCollectedLabelTextShadow.enabled = enable;
	}
	
	/*
	 * Helper method to set provided GUITText and its shadow to a string.
	 */
	void SetShadowGUIText (string newText, GUIText guiText, GUIText guiTextShadow)
	{
		guiText.text = newText;
		guiTextShadow.text = newText;
	}
}

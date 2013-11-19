using UnityEngine;
using System.Collections;

public class GUI_WildcardReveal : MonoBehaviour
{
	const float CARD_WIDTH = 6.0f;
	public GameObject wildcardPrefab;
	public float padding = 2f;
	public int numCardsPerRow = 30;
	float timeBetweenShows = 0.2f;
	float idleTimeHidden = 1.0f;
	float idleTimeRevealed = 3.0f;
	int nextCardToAnimate = 0;
	float timeStateEntered;
	RevealState state;
	bool allCardAreRevealed;
	public Item headstart;
	public Item money;
	public Item revive;
	public Item bigMoney;
	
	enum RevealState
	{
		Hidden,
		Showing,
		Idle,
		Revealing,
		Hiding,
		Finish
	}
	
	void Awake ()
	{
		state = RevealState.Hidden;
	}
	
	void Update ()
	{
		if (state == RevealState.Showing) {
			if (Time.time > timeStateEntered + (timeBetweenShows * nextCardToAnimate)) {
				ShowCard (nextCardToAnimate);
				nextCardToAnimate ++;
				if (nextCardToAnimate >= transform.childCount) {
					GoToIdle ();
				}
			}
		} else if (state == RevealState.Idle) {
			if (allCardAreRevealed) {
				if (Time.time > timeStateEntered + idleTimeRevealed) {
					GoToHiding ();
				}
			} else {
				if (Time.time > timeStateEntered + idleTimeHidden) {
					GoToRevealing ();
				}
			}
		} else if (state == RevealState.Revealing) {
			if (Time.time > timeStateEntered + (timeBetweenShows * nextCardToAnimate)) {
				RevealCard (nextCardToAnimate);
				nextCardToAnimate ++;
				if (nextCardToAnimate >= transform.childCount) {
					allCardAreRevealed = true;
					GoToIdle ();
				}
			}
		} else if (state == RevealState.Hiding) {
			float hideTime = 1.0f;
			if (Time.time > timeStateEntered + hideTime) {
				End ();
			}
		}
	}
	
	/*
	 * Tells the WildcardRevealer to begin the showing process.
	 */
	public void StartShowing (int numWildcards)
	{
		GoToShowing (numWildcards);
	}
	
	/*
	 * Plays the specified card's show animation
	 */
	void ShowCard (int index)
	{
		Transform child = transform.GetChild (index);
		child.gameObject.GetComponent<GUI_Wildcard> ().Show ();
	}
	
	/*
	 * Plays the specified card's reveal animation
	 */
	void RevealCard (int index)
	{
		Transform child = transform.GetChild (index);
		child.gameObject.GetComponent<GUI_Wildcard> ().Reveal ();
	}
	
	/*
	 * Plays the specified card's hide animation
	 */
	void HideCard (int index)
	{
		Transform child = transform.GetChild (index);
		child.gameObject.GetComponent<GUI_Wildcard> ().Hide ();
	}
	
	/* Shows the number of specified cards in a simple row / column box pattern
	 */
	void GoToShowing (int numWildcards)
	{
		int numCardsToDeal = numWildcards;
		int rowCount = 0;
		float numRows = Mathf.Ceil ((float)numWildcards / numCardsPerRow);
		float initialZOffset = ((numRows - 1) / 2.0f) * (CARD_WIDTH + padding);
		while (numCardsToDeal > 0) {
			int cardsThisRow = Mathf.Min (numCardsToDeal, numCardsPerRow);
			// Get the initial xOffset for the row based on how many we will have to deal
			float initialXOffset = -((cardsThisRow - 1) / 2.0f) * (CARD_WIDTH + padding);
			int colCount = 0;
			while (colCount < cardsThisRow) {
				float xOffset = colCount * (CARD_WIDTH + padding);
				float zOffset = -rowCount * (CARD_WIDTH + padding);
				// Spawn the card
				SpawnWildcard (initialXOffset + xOffset, initialZOffset + zOffset);
				numCardsToDeal--;
				colCount++;
			}
			rowCount++;
		}
		
		state = RevealState.Showing;
		allCardAreRevealed = false;
		nextCardToAnimate = 0;
		timeStateEntered = Time.time;
		
		AssignItems ();
	}
	
	/*
	 * Puts the Revealer into the state where it reveals cards one by one.
	 */
	void GoToRevealing ()
	{
		state = RevealState.Revealing;
		nextCardToAnimate = 0;
		timeStateEntered = Time.time;
	}
	
	/*
	 * Puts the Revealer into the state where it shows the card backs
	 */
	void GoToIdle ()
	{
		state = RevealState.Idle;
		nextCardToAnimate = 0;
		timeStateEntered = Time.time;
	}
	
	/*
	 * Puts the Revealer into the state where it hides cards one by one.
	 */
	void GoToHiding ()
	{
		state = RevealState.Hiding;
		nextCardToAnimate = 0;
		timeStateEntered = Time.time;
		
		// Hide all the cards at once
		while (nextCardToAnimate < transform.childCount) {
			HideCard (nextCardToAnimate);
			nextCardToAnimate++;
		}
	}
	
	/*
	 * Ends the Reveal
	 */
	void End ()
	{
		GameManager.Instance.GoToGameOver ();
		state = RevealState.Finish;
		Destroy (gameObject);
	}
	
	void SpawnWildcard (float xOffset, float zOffset)
	{
		Vector3 position = transform.position + new Vector3 (xOffset, 0, zOffset);
		GameObject wildcard = (GameObject)Instantiate (wildcardPrefab, position,
				Quaternion.LookRotation (Vector3.back, Vector3.up));
		
		// Parent the card to the revealer
		wildcard.transform.parent = transform;
	}
	
	/*
	 * Assigns items to all of the wildcards based on the drop tables.
	 */
	void AssignItems ()
	{
		Item chosenItem;
		int i = 0;
		foreach (Transform child in transform) {
			switch (i) {
			case 0 :
				chosenItem = money;
				break;
			case 1 :
				chosenItem = headstart;
				break;
			case 2 :
				chosenItem = bigMoney;
				break;
			case 3 :
				chosenItem = revive;
				break;
			default :
				chosenItem = headstart;
				break;
			}
			
			child.gameObject.GetComponent<GUI_Wildcard> ().SetItem (chosenItem);
			i++;
		}
	}
}

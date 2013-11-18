using UnityEngine;
using System.Collections;

public class GUI_WildcardReveal : MonoBehaviour
{
	const float CARD_WIDTH = 6.0f;
	public GameObject wildcardPrefab;
	public float padding = 2f;
	public int numCardsPerRow = 30;
	float timeBetweenShows = 0.2f;
	float idleTime = 1.0f;
	int nextCardToAnimate = 0;
	float timeStateEntered;
	RevealState state;
	
	enum RevealState
	{
		Hidden,
		Showing,
		Idle,
		Revealing,
		Hiding
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
					StartIdling ();
				}
			}
		} else if (state == RevealState.Idle) {
			if (Time.time > timeStateEntered + idleTime) {
				StartRevealing ();
			}
		} else if (state == RevealState.Revealing) {
			if (Time.time > timeStateEntered + (timeBetweenShows * nextCardToAnimate)) {
				RevealCard (nextCardToAnimate);
				nextCardToAnimate ++;
				if (nextCardToAnimate >= transform.childCount) {
					StartHiding ();
				}
			}
		}
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
	
	
	/* Shows the number of specified cards in a simple row / column box pattern
	 */
	public void StartShowing (int numWildcards)
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
		nextCardToAnimate = 0;
		timeStateEntered = Time.time;
	}
	
	/*
	 * Puts the Revealer into the state where it reveals cards one by one.
	 */
	void StartRevealing ()
	{
		state = RevealState.Revealing;
		nextCardToAnimate = 0;
		timeStateEntered = Time.time;
	}
	
	/*
	 * Puts the Revealer into the state where it shows the card backs
	 */
	void StartIdling ()
	{
		state = RevealState.Idle;
		nextCardToAnimate = 0;
		timeStateEntered = Time.time;
	}
	
	/*
	 * Puts the Revealer into the state where it hides cards one by one.
	 */
	void StartHiding ()
	{
		state = RevealState.Hiding;
		nextCardToAnimate = 0;
		timeStateEntered = Time.time;
	}
	
	void SpawnWildcard (float xOffset, float zOffset)
	{
		Vector3 position = transform.position + new Vector3 (xOffset, 0, zOffset);
		GameObject wildcard = (GameObject)Instantiate (wildcardPrefab, position,
				Quaternion.LookRotation (Vector3.forward, Vector3.up));
		
		// Parent the card to the revealer
		wildcard.transform.parent = transform;
	}
}

using UnityEngine;
using System.Collections;

public class GUI_WildcardReveal : MonoBehaviour
{
	const float CARD_WIDTH = 6.0f;
	public GameObject wildcardPrefab;
	public float padding = 2f;
	public int numCardsPerRow = 30;
	
	void Update ()
	{
	}
	
	/* Shows the number of specified cards in a simple row / column box pattern
	 */
	public void ShowCards (int numWildcards)
	{
		int numCardsToDeal = numWildcards;
		int rowCount = 0;
		float numRows = Mathf.Ceil ((float) numWildcards / numCardsPerRow);
		float initialZOffset = ((numRows -1) / 2.0f) * (CARD_WIDTH + padding);
		while (numCardsToDeal > 0) {
			int cardsThisRow = Mathf.Min (numCardsToDeal, numCardsPerRow);
			// Get the initial xOffset for the row based on how many we will have to deal
			float initialXOffset = -((cardsThisRow -1) / 2.0f) * (CARD_WIDTH + padding);
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
	}
	
	void SpawnWildcard (float xOffset, float zOffset)
	{
		Vector3 position = transform.position + new Vector3 (xOffset, 0, zOffset);
		GameObject wildcard = (GameObject)Instantiate (wildcardPrefab, position,
				Quaternion.LookRotation (Vector3.forward, Vector3.up));
		
		// Play the card's show animation
		wildcard.GetComponent<GUI_Wildcard> ().Show ();
		
		// Parent the card to the revealer
		wildcard.transform.parent = transform;
	}
}

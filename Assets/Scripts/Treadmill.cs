using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Treadmill : MonoBehaviour
{
	Difficulty difficulty = Difficulty.Easy;
	public float startingSpeed = 10.0f;
	public float scrollspeed;
	public float accelerationPerFrame = 0.0005f;
	public float maxspeed = 30.0f;
	public GameObject emptySection;
	public List<GameObject> easySections;
	public List<GameObject> mediumSections;
	public List<GameObject> hardSections;
	
	List<GameObject> sectionsInPlay;
	Transform sectionSpawnZone;
	Transform sectionKillZone;
	
	const int MEDIUM_THRESHOLD = 300; // Number of pickups passed
	const int HARD_THRESHOLD = 2000;
	
	enum Difficulty {
		Easy,
		Medium,
		Hard
	}
	
	void Awake ()
	{
		sectionsInPlay = new List<GameObject> ();
		scrollspeed = startingSpeed;
		sectionSpawnZone = (Transform)GameObject.Find (ObjectNames.SECTION_SPAWN).transform;
		sectionKillZone = (Transform)GameObject.Find (ObjectNames.SECTION_KILLZONE).transform;
		SpawnNextSection ();
	}
	
	void FixedUpdate ()
	{
		scrollspeed = Mathf.Min ((scrollspeed + accelerationPerFrame), maxspeed);
		transform.Translate (new Vector3 (0, 0, -scrollspeed * Time.deltaTime));
		// Check if our last section is on screen. If so, spawn another.
		if (isSectionOnScreen (GetLastSectionInPlay ())) {
			SpawnNextSection ();
		}
		// Check if the first section is past the kill line. If so, kill it!
		if (isSectionPastKillZone (sectionsInPlay [0])) {
			KillSection (sectionsInPlay [0]);
		}
		if (!GameManager.Instance.IsPlayerAlive ()) {
			scrollspeed = 0;
		}
		UpdateDifficulty ();
	}
	
	public void SlowDown ()
	{
		scrollspeed = (scrollspeed + startingSpeed) / 2;
	}
	
	/*
	 * Check if the provided (but should be the most recently generated) section has past the spawn zone.
	 * If it has past, return true.
	 */
	bool isSectionOnScreen (GameObject backSection)
	{
		Transform backEdge = (Transform)backSection.transform.FindChild (ObjectNames.BACK_EDGE);
		return backEdge.position.z <= sectionSpawnZone.position.z;
	}
	
	/*
	 * Check if the provided (but should be the least recently generated) section has past the kill zone.
	 * Return true if it has. We can compare z since it's locked for the player.
	 */
	bool isSectionPastKillZone (GameObject frontSection)
	{
		Transform backEdge = (Transform)frontSection.transform.FindChild (ObjectNames.BACK_EDGE);
		return backEdge.position.z <= sectionKillZone.position.z;
	}
	
	/*
	 * Using our level generation algorithm, select and spawn a new section. Destory the
	 * stale section (the section past the player) and assign the section on top of the player
	 * now as stale, to be destroyed next.
	 */
	void SpawnNextSection ()
	{
		// Determine which bucket of sections to draw from
		List<GameObject> sectionBucket = easySections;
		if (difficulty == Difficulty.Medium) {
			// Only give an X% change of medium tiles when we're in it.
			bool coinflip = RBRandom.PercentageChance (75f);
			if (coinflip) {
				sectionBucket = mediumSections;
			}
		} else if (difficulty == Difficulty.Hard){
			sectionBucket = hardSections;
		}
		Vector3 rowSpacing = new Vector3 (0, 0, 1);
		GameObject newSection = (GameObject)Instantiate (GetRandomSectionFromBucket (sectionBucket),
				sectionSpawnZone.position + rowSpacing, Quaternion.identity);
		sectionsInPlay.Add (newSection);
	}
	
	/*
	 * Using the provided list of sections, find one that is compatible and return it.
	 * If none is found, return an empty section.
	 */
	GameObject GetRandomSectionFromBucket (List<GameObject> sectionsToPickFrom)
	{
		// Set up a bag of indexes to draw from for the provided sectionsToPickFrom
		List<int> bagOfIndexes = new List<int> (sectionsToPickFrom.Count);
		for (int i = 0; i < sectionsToPickFrom.Count; i++) {
			bagOfIndexes.Add (i);
		}
		RBRandom.Shuffle<int> (bagOfIndexes);
		// Just take the first you get if it's the first one drawn
		if (GetLastSectionInPlay () == null) {
			return sectionsToPickFrom[bagOfIndexes[0]];
		}
		
		// Iterate through our random bag until we pick a section that is compatible.
		Section sectionToCheck;
		Section lastSection = (Section) GetLastSectionInPlay ().GetComponent<Section> ();
		int compatibleSectionIndex = -1;
		foreach (int index in bagOfIndexes) {
			sectionToCheck = sectionsToPickFrom[index].GetComponent<Section> ();
			if (lastSection.CanBeFollowedBy (sectionToCheck)) {
				compatibleSectionIndex = index;
				break;
			}
		}

		if (compatibleSectionIndex == -1) {
			Debug.LogWarning ("Couldn't find a compatible section in the bucket. This can " +
				"be fixed by adding a section prefab with open exits and entrances.");
			return emptySection;
		}
		return sectionsToPickFrom [compatibleSectionIndex];
	}
	
	/*
	 * Helper method to return the last section on the treadmill.
	 */
	GameObject GetLastSectionInPlay ()
	{
		if (sectionsInPlay.Count == 0) {
			return null;
		}
		return sectionsInPlay [Mathf.Max (sectionsInPlay.Count - 1, 0)];
	}
	
	/*
	 * Remove any references to the provided section and destroy it.
	 */
	void KillSection (GameObject sectionToKill)
	{
		sectionsInPlay.Remove (sectionToKill);
		Destroy (sectionToKill);
	}
	
	/*
	 * Check how many pickups we've passed. Once we've passed the threshold for medium
	 * and hard, update the difficulty respectively.
	 */
	void UpdateDifficulty ()
	{
		if (GameManager.Instance.numPickupsPassed > MEDIUM_THRESHOLD) {
			difficulty = Difficulty.Medium;
		} else if (GameManager.Instance.numPickupsPassed > HARD_THRESHOLD) {
			difficulty = Difficulty.Hard;
		}
	}
}

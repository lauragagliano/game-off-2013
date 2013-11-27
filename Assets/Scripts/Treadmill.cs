using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Treadmill : MonoBehaviour
{
	// Section Algorithm Configurations
	const int MIN_WILDCARD_DISTANCE = 400;
	const int MAX_WILDCARD_DISTANCE = 1000;
	int nextWildcardMarker;
	bool needsWildcard;
	const int MIN_FREEBIE_DISTANCE = 150; // ~5 sections of 70m
	const int MAX_FREEBIE_DISTANCE = 300;
	int nextFreebieMarker;
	
	public const int HARD_THRESHOLD = 2000;
	
	const float STARTING_SPEED = 20.0f;
	const float STARTING_ACCEL = 0.005f;
	public float distanceTraveled;
	public float scrollspeed;
	float previousScrollspeed;
	float accelerationPerFrame = 0.005f;
	float prevAccelerationPerFrame;
	public float maxspeed = 50.0f;
	public GameObject emptySection;
	public List<GameObject> normalSections;
	public List<GameObject> challengeSections;
	public List<GameObject> freebieSections;
	
	List<GameObject> sectionsInPlay;
	Transform sectionSpawnZone;
	Transform sectionKillZone;
	
	float lerpToSpeed;
	public bool lerping;
	const int lerpSpeed = 5;

	Status status;
	
	enum Status {
		Tutorial,
		Stopped,
		Started
	}
	
	void Awake ()
	{
		sectionsInPlay = new List<GameObject> ();
		sectionSpawnZone = (Transform)GameObject.Find (ObjectNames.SECTION_SPAWN).transform;
		sectionKillZone = (Transform)GameObject.Find (ObjectNames.SECTION_KILLZONE).transform;
		nextWildcardMarker = GenerateNextWildcardMarker ();
		nextFreebieMarker = GenerateNextFreebieMarker ();
	}
	
	void Update ()
	{
		if (status == Status.Started) {
			if (!needsWildcard && distanceTraveled >= nextWildcardMarker) {
				needsWildcard = true;
			}
			
			if (lerping) {
				UpdateLerping ();
			} else {
				scrollspeed = Mathf.Min ((scrollspeed + accelerationPerFrame), maxspeed);
			}
			// Move our treadmill and add up the distance
			float distance = scrollspeed * Time.deltaTime;
			transform.Translate (new Vector3 (0, 0, -distance));
			distanceTraveled += distance;
			// Check if our last section is on screen. If so, spawn another.
			if (GetLastSectionInPlay () == null) {
				SpawnNextSection ();
			} else if (isSectionOnScreen (GetLastSectionInPlay ())) {
				SpawnNextSection ();
			}
			// Check if the first section is past the kill line. If so, kill it!
			if (isSectionPastKillZone (sectionsInPlay [0])) {
				KillSection (sectionsInPlay [0]);
			}
		}
	}
	
	/*
	 * Return true if tutorial is being shown.
	 */
	public bool IsShowingTutorial ()
	{
		return status == Status.Tutorial;
	}
	
	#region #1 Treadmill Manipulation (Start/Stop/Reset/Slowdown)
	public void ShowTutorial ()
	{
		// Reset the tutorial image
		GameObject.Find (ObjectNames.TUTORIAL_IMAGE).transform.position = new Vector3 (0, 0.1f, 9.0f);
		scrollspeed = 0;
		status = Status.Tutorial;
	}
	
	public void StartScrolling ()
	{
		scrollspeed = STARTING_SPEED;
		status = Status.Started;
	}
	
	public void PauseScrolling ()
	{
		PauseSpeed();
		PauseAcceleration();
		status = Status.Stopped;
	}
	
	/*
	 * Temporarily turn off speed of the treadmill. Call ResumeTreadmill to restore its speed.
	 */
	void PauseSpeed ()
	{
		if (scrollspeed == 0) {
			Debug.LogWarning ("Tried to pause speed when it was already at 0!");
			return;
		}
		previousScrollspeed = scrollspeed;
		scrollspeed = 0;
	}

	/*
	 * Temporarily turn off acceleration of treadmill. Make sure to call
	 * ResumeAcceleration when done!
	 */
	public void PauseAcceleration ()
	{
		if (accelerationPerFrame == 0) {
			Debug.LogWarning ("Tried to pause acceleration when it was already at 0!");
			return;
		}
		prevAccelerationPerFrame = accelerationPerFrame;
		accelerationPerFrame = 0;
	}
	
	/*
	 * Restore the acceleration back to previous value.
	 */
	public void ResumeAcceleration ()
	{
		accelerationPerFrame = prevAccelerationPerFrame;
	}
	
	/*
	 * Cause the treadmill to stop until further notice
	 */
	public void TemporarySlowDown (float amount)
	{
		previousScrollspeed = scrollspeed;
		PauseAcceleration ();
		LerpToSpeed (Mathf.Max (STARTING_SPEED, scrollspeed - amount));
	}
	
	/*
	 * Bring the treadmill back up to it's previous speed and acceleration.
	 */
	public void ResumeTreadmill ()
	{
		LerpToSpeed (previousScrollspeed);
		ResumeAcceleration ();
		status = Status.Started;
	}
	
	/*
	 * Reset treadmill to prepare for a new game. Values should be restored to default
	 * and any sections in play should be destroyed.
	 */
	public void ResetTreadmill ()
	{
		for (int i = sectionsInPlay.Count-1; i >= 0; i--) {
			KillSection (sectionsInPlay[i]);
		}
		accelerationPerFrame = STARTING_ACCEL;
		lerping = false;
		lerpToSpeed = STARTING_SPEED;
		distanceTraveled = 0.0f;
		nextWildcardMarker = GenerateNextWildcardMarker ();
	}
	
	/*
	 * Handle the lerping process and turn off lerping when done.
	 */
	void UpdateLerping () {
		scrollspeed = Mathf.Lerp (scrollspeed, lerpToSpeed, lerpSpeed * Time.deltaTime);
		if (Mathf.RoundToInt(scrollspeed) == Mathf.RoundToInt (lerpToSpeed)) {
			lerping = false;
		}
	}
	
	/*
	 * Use this to change the speed of the treadmill with an acceleration or deceleration.
	 */
	void LerpToSpeed (float newSpeed)
	{
		if (!lerping) {
			lerpToSpeed = newSpeed;
			lerping = true;
		}
	}
	
	/*
	 * Return true if the treadmill has passed the distance for hard mode.
	 */
	public bool IsPastHardThreshold ()
	{
		return distanceTraveled > HARD_THRESHOLD;
	}
	#endregion
	
	#region #2 Section Logic
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
		List<GameObject> sectionBucket = normalSections;
		if (GameManager.Instance.IsEasy ()) {
		} /*else if (GameManager.Instance.IsMedium ()) {
			// Only give an X% change of medium tiles when we're in it.
			bool coinflip = RBRandom.PercentageChance (75f);
			if (coinflip) {
				sectionBucket = challengeSections;
			}
		}*/ else if (GameManager.Instance.IsHard ()) {
			sectionBucket = challengeSections;
		}
		// Pull from the freebie bucket when nextFreebieMarker has been passed
		if (distanceTraveled >= nextFreebieMarker) {
			sectionBucket = freebieSections;
			nextFreebieMarker = GenerateNextFreebieMarker ();
			Debug.Log ("Pulling a new freebie section. Next section at: " + nextFreebieMarker);
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
	 * Calculate where the next distance marker should be.
	 */
	int GenerateNextWildcardMarker ()
	{
		return (int) distanceTraveled + Random.Range (MIN_WILDCARD_DISTANCE, MAX_WILDCARD_DISTANCE);
	}
	
	/*
	 * Calculate where the next freebie should be spawned.
	 */
	int GenerateNextFreebieMarker ()
	{
		return (int) distanceTraveled + Random.Range (MIN_FREEBIE_DISTANCE, MAX_FREEBIE_DISTANCE);
	}
	
	/*
	 * Return whether the treadmill needs a wilcard to be spawned.
	 */
	public bool NeedsWildcard ()
	{
		return needsWildcard;
	}
	
	/*
	 * Called when a wildcard is successfully spawned.
	 */
	public void OnWildcardSpawn ()
	{
		needsWildcard = false;
		nextWildcardMarker = GenerateNextWildcardMarker ();
		Debug.Log ("Spawned a new wildcard. Next wildcard at: " + nextWildcardMarker);
	}
	#endregion
}

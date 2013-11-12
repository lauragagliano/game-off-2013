using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Treadmill : MonoBehaviour {
	
	public float startingSpeed = 10.0f;
	public float scrollspeed;
	public float accelerationPerFrame = 0.0005f;
	public float maxspeed = 30.0f;
	public List<GameObject> easySections;
	public List<GameObject> freebieSections;
	public float chanceOfFreebie;
	float lastFreebieTime;
	float delayWeight = 60.0f; // Higher means good things happen less often
	
	List<GameObject> sectionsInPlay;
	GameObject player;

	Transform sectionSpawnZone;
	Transform sectionKillZone;
	
	void Awake ()
	{
		sectionsInPlay = new List<GameObject> ();
		scrollspeed = startingSpeed;
		player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
		sectionSpawnZone = (Transform) GameObject.Find (ObjectNames.SECTION_SPAWN).transform;
		sectionKillZone = (Transform) GameObject.Find (ObjectNames.SECTION_KILLZONE).transform;
		SpawnNextSection ();
		lastFreebieTime = Time.time;
	}
	
	void FixedUpdate ()
	{
		scrollspeed = Mathf.Min((scrollspeed + accelerationPerFrame), maxspeed);
		transform.Translate (new Vector3(0,0,-scrollspeed * Time.deltaTime));
		// Check if our last section is on screen. If so, spawn another.
		if (isSectionOnScreen (sectionsInPlay[(sectionsInPlay.Count-1)])) {
			SpawnNextSection ();
		}
		// Check if the first section is past the kill line. If so, kill it!
		if (isSectionPastKillZone (sectionsInPlay[0])) {
			KillSection (sectionsInPlay[0]);
		}
	}
	
	public void SlowDown ()
	{
		scrollspeed = (scrollspeed + startingSpeed)/2;
	}
	
	/*
	 * Check if the provided (but should be the most recently generated) section has past the spawn zone.
	 * If it has past, return true.
	 */
	bool isSectionOnScreen (GameObject backSection)
	{
		Transform backEdge = (Transform) backSection.transform.FindChild (ObjectNames.BACK_EDGE);
		return backEdge.position.z <= sectionSpawnZone.position.z;
	}
	
	/*
	 * Check if the provided (but should be the least recently generated) section has past the kill zone.
	 * Return true if it has. We can compare z since it's locked for the player.
	 */
	bool isSectionPastKillZone (GameObject frontSection)
	{
		Transform backEdge = (Transform) frontSection.transform.FindChild (ObjectNames.BACK_EDGE);
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
		List<GameObject> sectionBucket;
		float timeSinceLastFreebie = Time.time - lastFreebieTime;
		chanceOfFreebie = (timeSinceLastFreebie * timeSinceLastFreebie) / (delayWeight * delayWeight) * 100;
		if (RBRandom.PercentageChance (chanceOfFreebie)) {
			sectionBucket = freebieSections;
			lastFreebieTime = Time.time;
		} else {
			sectionBucket = easySections;
		}
		int sectionIndex = Random.Range (0, sectionBucket.Count);
		Vector3 rowSpacing = new Vector3 (0, 0, 1);
		GameObject newSection = (GameObject)Instantiate (sectionBucket[sectionIndex], sectionSpawnZone.position + rowSpacing, Quaternion.identity);
		sectionsInPlay.Add (newSection);
		// Parent the random new section with the treadmill
		newSection.transform.parent = gameObject.transform;
		Debug.Log (string.Format ("Section {0} Spawned at {1}", newSection.name, sectionSpawnZone.position));
		// TODO This could be done more elegantly with objects more aware of the pickups and player
		// i.e. one that doesn't have to do a lookup every time.
		// Set the pickups in the scene to match the player's current color
		GameObject[] pickups = GameObject.FindGameObjectsWithTag (Tags.PICKUP);
		foreach (GameObject pickup in pickups) {
			RGB pickupRGB = pickup.GetComponent<RGB> ();
			Player playerscript = (Player) player.GetComponent<Player> ();
			pickupRGB.color = playerscript.playerRGB.color;
			pickupRGB.Refresh ();
		}
	}
	
	/*
	 * Remove any references to the provided section and destroy it.
	 */
	void KillSection (GameObject sectionToKill)
	{
		sectionsInPlay.Remove (sectionToKill);
		Destroy (sectionToKill);
	}
}

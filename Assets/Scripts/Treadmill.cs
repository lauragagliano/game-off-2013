using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Treadmill : MonoBehaviour {
	
	public float startingSpeed = 10.0f;
	public float scrollspeed;
	public float accelerationPerFrame = 0.0005f;
	public float maxspeed = 30.0f;
	public Transform sectionSpawnZone;
	public List<GameObject> easySections;
	public List<GameObject> freebieSections;
	public float chanceOfFreebie;
	float lastFreebieTime;
	float delayWeight = 60.0f; // Higher means good things happen less often
	
	GameObject player;
	GameObject mostRecentSection;
	GameObject staleSection;
	
	void Awake ()
	{
		scrollspeed = startingSpeed;
		player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
		SpawnNextSection ();
		lastFreebieTime = Time.time;
	}
	
	void FixedUpdate ()
	{
		scrollspeed = Mathf.Min((scrollspeed + accelerationPerFrame), maxspeed);
		transform.Translate (new Vector3(0,0,-scrollspeed * Time.deltaTime));
		if (isSectionPastPlayer ()) {
			SpawnNextSection ();
		}
	}
	
	public void SlowDown ()
	{
		scrollspeed = (scrollspeed + startingSpeed)/2;
	}
	
	/*
	 * Check if the most recently generated section has past the player.
	 * Return true if it has. We can compare z since it's locked for the player.
	 */
	bool isSectionPastPlayer ()
	{
		return mostRecentSection.transform.position.z <= player.transform.position.z;
	}
	
	/*
	 * Using our level generation algorithm, select and spawn a new section. Destory the
	 * stale section (the section past the player) and assign the section on top of the player
	 * now as stale, to be destroyed next.
	 */
	void SpawnNextSection ()
	{
		if (staleSection != null) {
			Destroy (staleSection);
		}
		staleSection = mostRecentSection;
		
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
		mostRecentSection = (GameObject)Instantiate (sectionBucket[sectionIndex], sectionSpawnZone.position, Quaternion.identity);
		// Parent the random new section with the treadmill
		mostRecentSection.transform.parent = gameObject.transform;
		// TODO This could be done more elegantly with objects more aware of the pickups and player
		// i.e. one that doesn't have to do a lookup every time.
		// Set the pickups in the scene to match the player's current color
		GameObject[] pickups = GameObject.FindGameObjectsWithTag (Tags.PICKUP);
		foreach (GameObject pickup in pickups) {
			RGB pickupRGB = pickup.GetComponent<RGB> ();
			Player playerscript = (Player) player.GetComponent<Player> ();
			pickupRGB.color = playerscript.playerRGB.color;
			pickupRGB.Refresh ();
		}}
}

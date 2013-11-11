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
	
	Transform playerTransform;
	GameObject mostRecentSection;
	GameObject staleSection;
	
	void Awake ()
	{
		scrollspeed = startingSpeed;
		playerTransform = (Transform) GameObject.Find (ObjectNames.PLAYER).transform;
		SpawnNextSection ();
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
		return mostRecentSection.transform.position.z <= playerTransform.position.z;
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
		int sectionIndex = Random.Range (0, easySections.Count);
		mostRecentSection = (GameObject)Instantiate (easySections[sectionIndex], sectionSpawnZone.position, Quaternion.identity);
		// Parent the random new section with the treadmill
		mostRecentSection.transform.parent = gameObject.transform;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Section : MonoBehaviour
{
	public int numberOfPickups;
	
	public bool[] entranceOpenings;
	public bool[] exitOpenings;

	public byte entranceBitmap;
	public byte exitBitmap;
	
	public GameObject redCrystalPrefab;
	public GameObject greenCrystalPrefab;
	public GameObject blueCrystalPrefab;
	GameObject blockPrefab;
	GameObject wildcardPrefab;

	GameObject tempPrefabHolder;
	Treadmill treadmill;
	
	void Awake ()
	{
		// Start by moving the new Section onto the Treadmill (as a child of the object)
		treadmill = GameManager.Instance.treadmill;
		transform.parent = treadmill.transform;
		ReferencePrefabs ();
	}
	
		
	/*
	 * Reference all the prefabs that GameManager stores which sections can spawn.
	 */
	void ReferencePrefabs ()
	{
		redCrystalPrefab = GameManager.Instance.redCrystalPrefab;
		greenCrystalPrefab = GameManager.Instance.greenCrystalPrefab;
		blueCrystalPrefab = GameManager.Instance.blueCrystalPrefab;
		blockPrefab = GameManager.Instance.blockPrefab;
		wildcardPrefab = GameManager.Instance.wildcardPrefab;
	}
	
	void Start ()
	{
		// Create an empty object to parent prefabs to (for some reason, the children prefabs can't
		// be attached to this section itself.
		tempPrefabHolder = new GameObject("Prefabs");
		// Move our prefabs now that they've been created
		tempPrefabHolder.transform.parent = transform;
		
		// Shuffle the three crystal prefabs
		List<GameObject> threeCrystals = new List<GameObject> {redCrystalPrefab, greenCrystalPrefab, blueCrystalPrefab};
		RBRandom.Shuffle<GameObject> (threeCrystals);
		GameObject randomCrystalForPickupA = threeCrystals[0];
		GameObject randomCrystalForPickupB = threeCrystals[1];
		GameObject randomCrystalForPickupC = threeCrystals[2];

		foreach (Transform child in transform) {
			// Replace placeholders with BlackBlock prefab
			if (child.CompareTag (Tags.BLOCK)) {
				InstantiatePrefabAtPlaceholder (blockPrefab, child, tempPrefabHolder.transform);
			}
			// Replace pickups with Pickup prefab.
			else if (child.CompareTag (Tags.PICKUP_GROUP_A)) {
				InstantiatePrefabAtPlaceholder (randomCrystalForPickupA, child, tempPrefabHolder.transform);
			} else if (child.CompareTag (Tags.PICKUP_GROUP_B)) {
				InstantiatePrefabAtPlaceholder (randomCrystalForPickupB, child, tempPrefabHolder.transform);
			} else if (child.CompareTag (Tags.PICKUP_GROUP_C)) {
				InstantiatePrefabAtPlaceholder (randomCrystalForPickupC, child, tempPrefabHolder.transform);
			} else if (child.CompareTag (Tags.RED_PICKUP)) {
				InstantiatePrefabAtPlaceholder (redCrystalPrefab, child, tempPrefabHolder.transform);
			} else if (child.CompareTag (Tags.GREEN_PICKUP)) {
				InstantiatePrefabAtPlaceholder (greenCrystalPrefab, child, tempPrefabHolder.transform);
			} else if (child.CompareTag (Tags.BLUE_PICKUP)) {
				InstantiatePrefabAtPlaceholder (blueCrystalPrefab, child, tempPrefabHolder.transform);
			} else if (child.CompareTag (Tags.WILDCARD)) {
				if (treadmill.NeedsWildcard ()) {
					InstantiatePrefabAtPlaceholder (wildcardPrefab, child, tempPrefabHolder.transform);
					treadmill.OnWildcardSpawn ();
				} else {
					child.gameObject.SetActive (false);
				}
			}
		}
		GameManager.Instance.numPickupsPassed += numberOfPickups;
	}

	/*
	 * Create an instance of a prefab in resources at the same location as the placeholder. Also,
	 * parent the prefab to any specified Transform. Then finally, kill the prefab.
	 */
	GameObject InstantiatePrefabAtPlaceholder (GameObject prefab, Transform placeholder, Transform prefabParent)
	{
		GameObject clonedPrefab = (GameObject)Instantiate(prefab, placeholder.position, Quaternion.identity);

		clonedPrefab.transform.parent = prefabParent;
		Destroy (placeholder.gameObject);
		return clonedPrefab;
	}

	/*
	 * Ensure our pickup count is set correctly. This should be called in the
	 * Editor so that we can make calculations against prefabs before instantiating them.
	 */
	public void SetPickupCount ()
	{
		numberOfPickups = 0;
		foreach (Transform child in transform) {
			if (child.CompareTag (Tags.PICKUP_GROUP_A) ||
				child.CompareTag (Tags.PICKUP_GROUP_B) ||
				child.CompareTag (Tags.PICKUP_GROUP_C)) {
				numberOfPickups++;
			}
		}
	}
	
	/*
	 * Take our boolean blockage values for entrance and exit and set the
	 * bitmaps that will be used to match up sequences.
	 */
	public void SetEntranceAndExitBitmaps ()
	{
		entranceBitmap = CalculateDecimalValue (entranceOpenings);
		exitBitmap = CalculateDecimalValue (exitOpenings);
	}
	
	/*
	 * Take an array of booleans and calculate what they are as a bitmap if transposed
	 * to the lowest bits. For example the values TFTT would be 0000 1011 as a byte.
	 * The values TFFFT would be 0001 0001. Then with that bitmap, return the decimal
	 * that it equates to.
	 * 
	 * Examples
	 * TTTTT = 11111 = 31 (all columns are open)
	 * TFTFF = 10100 = 20
	 * FFFTT = 00011 = 3
	 */
	byte CalculateDecimalValue (bool[] openings)
	{
		if (openings == null || openings.Length == 0) {
			Debug.LogWarning (string.Format("Cannot CalculateDecimalValue for {0} until openings are set.", 
				gameObject.name));
			return 0;
		}
		byte highestBitVal = (byte) (Mathf.Pow (2, openings.Length-1));
		byte curBitVal = 0;
		for (int i = 0; i < openings.Length; i++) {
			if (openings[i]) {
				curBitVal += (byte) (highestBitVal / (byte) Mathf.Pow (2, i));
			}
		}
		return curBitVal;
	}
	
	/*
	 * Test whether the Section can be followed by a provided Section. This compares
	 * this.Section's exit with the passed in Section's entrance. If there are any
	 * openings in line, it will return true.
	 */
	public bool CanBeFollowedBy (Section nextSection)
	{
		return (exitBitmap & nextSection.entranceBitmap) > 0;
	}
}

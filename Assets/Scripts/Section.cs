using UnityEngine;
using System.Collections;

public class Section : MonoBehaviour
{
	public int numberOfPickups;
	
	public bool[] entranceOpenings;
	public bool[] exitOpenings;

	public byte entranceBitmap;
	public byte exitBitmap;
	
	GameObject tempPrefabHolder;
	
	void Awake ()
	{
		// Start by moving the new Section onto the Treadmill (as a child of the object)
		Transform treadmill = GameObject.Find(ObjectNames.TREADMILL).transform;
		transform.parent = treadmill;

	}
	
	void Start ()
	{
		// Create an empty object to parent prefabs to (for some reason, the children prefabs can't
		// be attached to this section itself.
		tempPrefabHolder = new GameObject("Prefabs");
		// Move our prefabs now that they've been created
		tempPrefabHolder.transform.parent = transform;

		foreach (Transform child in transform) {
			// Replace placeholders with BlackBlock prefab
			if (child.CompareTag (Tags.BLOCK)) {
				InstantiatePrefabAtPlaceholder (ObjectNames.BLOCK_PREFAB, child, tempPrefabHolder.transform);
			}
			// Replace pickups with Pickup prefab.
			//TODO Serious FPS slowdown when pickups are involved.
			else if (child.CompareTag (Tags.PICKUP)) {
				GameObject pickup = InstantiatePrefabAtPlaceholder (ObjectNames.PICKUP_PREFAB, child, tempPrefabHolder.transform);
			}
			else if (child.CompareTag (Tags.WILDCARD)) {
				GameObject pickup = InstantiatePrefabAtPlaceholder (ObjectNames.WILDCARD_PREFAB, child, tempPrefabHolder.transform);
			}
		}
		GameManager.Instance.numPickupsPassed += numberOfPickups;
	}
	
	/*
	 * Create an instance of a prefab in resources at the same location as the placeholder. Also,
	 * parent the prefab to any specified Transform. Then finally, kill the prefab.
	 */
	GameObject InstantiatePrefabAtPlaceholder (string resourceName, Transform placeholder, Transform prefabParent)
	{
		GameObject prefab = (GameObject)Instantiate(Resources.Load(resourceName, typeof(GameObject)), 
			placeholder.position, Quaternion.identity);
		prefab.transform.parent = prefabParent;
		Destroy (placeholder.gameObject);
		return prefab;
	}
	
	/*
	 * Ensure our pickup count is set correctly. This should be called in the
	 * Editor so that we can make calculations against prefabs before instantiating them.
	 */
	public void SetPickupCount ()
	{
		numberOfPickups = 0;
		foreach (Transform child in transform) {
			if (child.CompareTag (Tags.PICKUP)) {
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

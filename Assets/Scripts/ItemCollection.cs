using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemCollection : MonoBehaviour
{
	Hashtable itemCollection = new Hashtable ();
	const int NOT_FOUND = -1;

	public int GetItemCount (string itemName)
	{
		if (!itemCollection.ContainsKey (itemName)) {
			return NOT_FOUND;
		}
		return (int)itemCollection [itemName];
	}
	
	public void AddItem (string itemName, int amountToAdd)
	{
		if (itemCollection.ContainsKey (itemName)) {
			int curCount = (int)itemCollection[itemName];
			itemCollection[itemName] = curCount + amountToAdd;
		} else {
			itemCollection.Add (itemName, amountToAdd);
		}
	}
	
	public void RemoveItem (string itemName, int amountToRemove)
	{
		if (itemCollection.ContainsKey (itemName)) {
			int curCount = (int)itemCollection[itemName];
			if (curCount - amountToRemove >= 0) {
				itemCollection[itemName] = curCount - amountToRemove;
			} else {
				Debug.LogWarning (string.Format ("Attempted to remove more " +
					"of an item than was available. Item: {0}, Available Count: {1}, Remove: {2}",
					itemName, curCount, amountToRemove));
			}
		} else {
			Debug.LogWarning (string.Format ("Attempted to remove an item ({0}) " +
				"that did not exist in collection ({1}).", itemName, name));
		}
	}
	
	/*
	 * This method returns the contents of the collection as a JSON string.
	 * It will most likely be used for debugging and may be handy when saving player
	 * data.
	 */
	public string GetContentsAsJSON ()
	{
		string str = "{";
		foreach (string itemKey in itemCollection.Keys) {
			str += string.Format("\"{0}\":{1}, ", itemKey, itemCollection[itemKey]);
		}
		// Clean up trailing comma and space
		str = str.TrimEnd (',', ' ');
		str += "}";
		return str;
	}
}
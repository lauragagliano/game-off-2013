using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
	Hashtable inventory = new Hashtable ();
	const int NOT_FOUND = -1;
	
	/*
	 * Return the count of any item, using item name as
	 * the identifier.
	 */
	public int GetItemCount (string itemName)
	{
		if (!inventory.ContainsKey (itemName)) {
			return NOT_FOUND;
		}
		return (int)inventory [itemName];
	}
	
	/*
	 * Add a quantity of any item, using the itemName to identity
	 * which item.
	 */
	public void AddItem (string itemName, int amountToAdd)
	{
		if (inventory.ContainsKey (itemName)) {
			int curCount = (int)inventory[itemName];
			inventory[itemName] = curCount + amountToAdd;
		} else {
			inventory.Add (itemName, amountToAdd);
		}
	}
	
	/*
	 * Remove a given quantity of any item. Logs warnings if quantity
	 * to remove exceeds the quantity in the inventory. The caller of
	 * this method should perform a GetItemCount to check that removal
	 * can be made.
	 */
	public void RemoveItem (string itemName, int amountToRemove)
	{
		if (inventory.ContainsKey (itemName)) {
			int curCount = (int)inventory[itemName];
			if (curCount - amountToRemove >= 0) {
				inventory[itemName] = curCount - amountToRemove;
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
		foreach (string itemKey in inventory.Keys) {
			str += string.Format("\"{0}\":{1}, ", itemKey, inventory[itemKey]);
		}
		// Clean up trailing comma and space
		str = str.TrimEnd (',', ' ');
		str += "}";
		return str;
	}
}
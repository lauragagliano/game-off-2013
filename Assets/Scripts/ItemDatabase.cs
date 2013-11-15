using UnityEngine;
using System.Collections;

public class ItemDatabase : Singleton<ItemDatabase>
{
	Hashtable itemDatabase = new Hashtable ();
	
	void Awake ()
	{
		Item item1 = (Item)ScriptableObject.CreateInstance (typeof (Item));
		item1.itemName = ItemNames.ITEM1;
		item1.cost = 100;
		item1.type = Item.Type.consumable;
		itemDatabase.Add (item1.itemName, item1);

		Item item2 = (Item)ScriptableObject.CreateInstance (typeof (Item));
		item2.itemName = ItemNames.ITEM2;
		item2.cost = 100;
		item2.type = Item.Type.consumable;
		itemDatabase.Add (item2.itemName, item2);

		Item item3 = (Item)ScriptableObject.CreateInstance (typeof (Item));
		item3.itemName = ItemNames.ITEM3;
		item3.cost = 100;
		item3.type = Item.Type.consumable;
		itemDatabase.Add (item3.itemName, item3);
	}
	
	public Item[] GetAllItems ()
	{
		Item[] allItems = new Item[itemDatabase.Count];
		itemDatabase.Values.CopyTo (allItems, 0);
		return allItems;
	}
}

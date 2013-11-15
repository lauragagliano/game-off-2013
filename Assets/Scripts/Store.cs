using UnityEngine;
using System.Collections;

public class Store : MonoBehaviour
{	
	public ItemCollection inventory;
	
	void Awake ()
	{
		inventory = (ItemCollection)GetComponent<ItemCollection> ();
		// For each item in the database, add it here.
		foreach (Item item in ItemDatabase.Instance.GetAllItems ()) {
			Debug.Log (item.itemName);
			inventory.AddItem (item.itemName, int.MaxValue);
		}
		Debug.Log (inventory.GetContentsAsJSON ());
	}
	
	public void EnterStore ()
	{
		Camera storeCamera = GameObject.Find (ObjectNames.STORE_CAMERA).camera;
		Camera mainCamera = GameObject.Find (ObjectNames.MAIN_CAMERA).camera;
		storeCamera.enabled = true;
		mainCamera.enabled = false;
	}
	
	public void ExitStore ()
	{
		Camera storeCamera = GameObject.Find (ObjectNames.STORE_CAMERA).camera;
		Camera mainCamera = GameObject.Find (ObjectNames.MAIN_CAMERA).camera;
		storeCamera.enabled = false;
		mainCamera.enabled = true;
	}
}

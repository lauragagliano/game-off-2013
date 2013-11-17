using UnityEngine;
using System.Collections;

public class Store : MonoBehaviour
{	
	//public ItemCollection inventory;
	public GameObject[] allItems;
	public int selectedItem = 0;
	Inventory playerInventory;
	
	Transform scroller;
	
	void Start ()
	{
		scroller = (Transform) GameObject.Find (ObjectNames.STORE_SCROLLER).transform;
		playerInventory = GameManager.Instance.player.GetComponent<Inventory> ();
	}
	
	void Update () 
	{
		//TODO Get some real inputs in here. Also, don't do this when the
		// player isn't at the store.
		if (Input.GetKeyDown ("a")) {
			ScrollToPrevious ();
		} else if (Input.GetKeyDown ("d")) {
			ScrollToNext ();
		}
		ScrollToItem (selectedItem);
	}
	
	/*
	 * Adjust our selected item index to the previous one in the array.
	 * Prevent going out of bounds.
	 */
	void ScrollToPrevious ()
	{
		selectedItem--;
		if (selectedItem < 0) {
			selectedItem = 0;
		}
	}
	
	/*
	 * Adjust our selected item index to the next one in the array. Cap
	 * it out at the end of the array.
	 */
	void ScrollToNext ()
	{
		selectedItem++;
		if (selectedItem >= allItems.Length) {
			selectedItem = allItems.Length -1;
		}
	}
	
	/*
	 * (Slowly) Scroll to a provided index in the item list.
	 */
	void ScrollToItem (int index)
	{
		int adjustment = index * 8;
		float speed = 8.0f;
		// New position relative to shop
		Vector3 newLocation = transform.position - new Vector3(adjustment, 0, 0);
		scroller.position = Vector3.Lerp (scroller.position, newLocation, speed * Time.deltaTime);
	}
	
	/*
	 * Turn on the store cam and turn off the main camera.
	 */
	public void EnterStore ()
	{
		// TODO We could improve performance by turning off objects as well as cameras
		Camera storeCamera = GameObject.Find (ObjectNames.STORE_CAMERA).camera;
		Camera mainCamera = GameObject.Find (ObjectNames.MAIN_CAMERA).camera;
		storeCamera.enabled = true;
		mainCamera.enabled = false;
	}
	
	/*
	 * Turn off the store cam and turn on the main camera.
	 */
	public void ExitStore ()
	{
		// TODO We could improve performance by turning off objects as well as cameras
		Camera storeCamera = GameObject.Find (ObjectNames.STORE_CAMERA).camera;
		Camera mainCamera = GameObject.Find (ObjectNames.MAIN_CAMERA).camera;
		storeCamera.enabled = false;
		mainCamera.enabled = true;
	}
	
	/*
	 * Check if the player can purchase the item. If so, return true. This checks
	 * whether the player has the item already or not.
	 */
	public bool DisplayBuyForSelectedItem ()
	{
		Item itemToBuy = allItems[selectedItem].GetComponent<Item> ();
		return !playerInventory.HasItem (itemToBuy.itemName);
	}
	
	/*
	 * Add the currently selected item to the player's inventory.
	 */
	public void BuyItem ()
	{
		//TODO Growing my program, no validation
		Item itemToBuy = allItems[selectedItem].GetComponent<Item> ();
		playerInventory.AddItem (itemToBuy.itemName);
		Debug.Log (playerInventory.GetContentsAsJSON ());
	}
}

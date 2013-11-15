using UnityEngine;
using System.Collections;

public class Store : MonoBehaviour
{	
	
	
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

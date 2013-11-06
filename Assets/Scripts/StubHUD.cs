using UnityEngine;
using System.Collections;

public class StubHUD : MonoBehaviour
{
	public GUIText startEndText;
	
	void Update ()
	{
		if (GameObject.FindGameObjectWithTag(Tags.PLAYER) == null) {
			startEndText.text = "Game Over!";
		}
	}
}

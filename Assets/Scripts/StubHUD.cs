using UnityEngine;
using System.Collections;

public class StubHUD : MonoBehaviour
{
	public GUIText startEndText;
	public GUIText helpText;

	void Awake ()
	{
		helpText.text = "A: LEFT\nD: RIGHT\n\nQ: BLUE\nW: YELLOW\nE: RED\n\n" +
			"1. EASY LEVEL\n2. MEDIUM LEVEL\n3. HARD LEVEL";
	}
	
	void Update ()
	{
		if (GameObject.FindGameObjectWithTag(Tags.PLAYER) == null) {
			startEndText.text = "Game Over!";
		}
	}
}

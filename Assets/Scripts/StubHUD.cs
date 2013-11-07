using UnityEngine;
using System.Collections;

public class StubHUD : MonoBehaviour
{
	public GUIText startEndText;
	public GUIText helpText;
	public GUIText scoreText;
	
	void Awake ()
	{
		helpText.text = "A: LEFT\nD: RIGHT\n\nQ: BLUE\nW: YELLOW\nE: RED\n\n" +
			"1. EASY LEVEL\n2. MEDIUM LEVEL\n3. HARD LEVEL";
	}
	
	void Update ()
	{

	}
	
	void OnGUI ()
	{
		if (GameObject.FindGameObjectWithTag (Tags.PLAYER) == null) {
			startEndText.text = "Game Over!";
			GUILayout.BeginArea (new Rect (Screen.width / 2 - 50.0f, Screen.height / 2, 200.0f, 70.0f));
			if (GUILayout.Button ("Click to Retry")) {
				Application.LoadLevel (Application.loadedLevel);
			}
			GUILayout.EndArea ();
		}
		scoreText.text = string.Format("Score:\nRed: {0}\nBlue: {1}\nYellow: {2}", 
			ScoreKeeper.Instance.redScore,ScoreKeeper.Instance.blueScore,ScoreKeeper.Instance.yellowScore);
	}
}

using UnityEngine;
using System.Collections;

public class StubHUD : MonoBehaviour
{
	public Player player;
	public GUIText startEndText;
	public GUIText helpText;
	public GUIText scoreText;
	public GameObject redMeterGO;
	GUIMeter redMeter;
	
	void Awake ()
	{
		helpText.text = "A: LEFT\nD: RIGHT\n\nJ: RED\nK: GREEN\nL: BLUE\n\n(Tap Twice for POAWAHH";
		redMeter = redMeterGO.GetComponent<GUIMeter> ();
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
		scoreText.text = string.Format ("Power:\nRed: {0}\nBlue: {1}\nGreen: {2}\n\nHealth: {3}",
			player.redPower.curValue, player.bluePower.curValue, player.greenPower.curValue, player.curHealth);
		
		redMeter.CurrentFillPercent = ((float) player.redPower.curValue / player.redPower.MaxValue);
	}
}

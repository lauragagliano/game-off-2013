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
	public GameObject blueMeterGO;
	GUIMeter blueMeter;
	public GameObject greenMeterGO;
	GUIMeter greenMeter;
	
	void Awake ()
	{
		helpText.text = "A: LEFT\nD: RIGHT\n\nJ: RED\nK: GREEN\nL: BLUE\n\n(Tap Twice for POAWAHH";
		redMeter = redMeterGO.GetComponent<GUIMeter> ();
		blueMeter = blueMeterGO.GetComponent<GUIMeter> ();
		greenMeter = greenMeterGO.GetComponent<GUIMeter> ();
	}
	
	void Update ()
	{

	}
	
	void OnGUI ()
	{
		if (!GameManager.Instance.IsPlayerAlive ()) {
			startEndText.text = "Game Over!";
			GUILayout.BeginArea (new Rect (Screen.width / 2 - 50.0f, Screen.height / 2, 200.0f, 70.0f));
			if (GUILayout.Button ("Click to Retry")) {
				//Application.LoadLevel (Application.loadedLevel);
				GameManager.Instance.StartGame ();
			}
			GUILayout.EndArea ();
		}
		scoreText.text = string.Format ("Power:\nRed: {0}\nGreen: {1}\nBlue: {2}\n\nHealth: {3}\nPassed Pigments: {4}",
			GameManager.Instance.redPoints, GameManager.Instance.greenPoints, 
			GameManager.Instance.bluePoints, player.curHealth,
			GameManager.Instance.numPickupsPassed);
		
		redMeter.CurrentFillPercent = ((float) player.redPower.curValue / player.redPower.MaxValue);
		blueMeter.CurrentFillPercent = ((float) player.bluePower.curValue / player.bluePower.MaxValue);
		greenMeter.CurrentFillPercent = ((float) player.greenPower.curValue / player.greenPower.MaxValue);
	}
}

using UnityEngine;
using System.Collections;

public class TutorialChallengeExit : MonoBehaviour
{
	// Update is called once per frame
	void OnTriggerEnter () {
		GameManager.Instance.MarkTutorialComplete ();
	}
}

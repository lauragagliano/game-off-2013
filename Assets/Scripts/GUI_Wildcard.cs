using UnityEngine;
using System.Collections;

public class GUI_Wildcard : MonoBehaviour {
	
	public AnimationClip onAnim;
	public AnimationClip revealAnim;
	public AnimationClip hideAnim;
	
	public void Show () {
		animation.Play(onAnim.name);
	}
	
	public void Reveal () {
		animation.Play(revealAnim.name);
	}
	
	public void Hide () {
		animation.Play(hideAnim.name);
	}
}

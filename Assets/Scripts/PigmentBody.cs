using UnityEngine;
using System.Collections;
using System;

public class PigmentBody : MonoBehaviour
{
	ColorWheel currentColor;
	public Material[] bodyMaterials = new Material[4];
	bool isReviving;
	
	public GameObject[] limbs = new GameObject[Enum.GetNames(typeof(Limb)).Length];
	public GameObject[] fxLimbs = new GameObject[Enum.GetNames(typeof(Limb)).Length];
	public GameObject[] fxLimbPrefabs = new GameObject[Enum.GetNames(typeof(Limb)).Length];
	
	enum Limb
	{
		Body,
		ArmL,
		ArmR,
		LegL,
		LegR
	}
	
	public void OnLimbDoneLerping()
	{
		foreach(GameObject limb in fxLimbs)
		{
			if(limb.GetComponent<FX_PigmentLimb> ().IsLerping) {
				return;
			}
		}
		
		isReviving = false;
			
		GameManager.Instance.player.RagdollRestored();
		
		foreach(GameObject limb in fxLimbs)
		{
			Destroy (limb);
		}
	}
	
	public void SetColor (ColorWheel color, bool colorFX)
	{
		GameObject bodyToColor = colorFX ? fxLimbs[(int)Limb.Body] : limbs[(int)Limb.Body];
		
		// Get materials to color limbs with
		Material armLegMat = ColorManager.Instance.red;
		Material bodyMat = ColorManager.Instance.red;
		if (color == ColorWheel.red) {
			armLegMat = ColorManager.Instance.red;
			bodyMat = bodyMaterials [0];
		} else if (color == ColorWheel.green) {
			armLegMat = ColorManager.Instance.green;
			bodyMat = bodyMaterials [1];
		} else if (color == ColorWheel.blue) {
			armLegMat = ColorManager.Instance.blue;
			bodyMat = bodyMaterials [2];
		} else if (color == ColorWheel.neutral) {
			armLegMat = ColorManager.Instance.neutral;
			bodyMat = bodyMaterials [3];
		}
		
		ColorLimbs (armLegMat, colorFX);
		bodyToColor.renderer.material = bodyMat;
		currentColor = color;
	}
	
	public void SetColor (ColorWheel color)
	{
		SetColor (color, false);
	}
	
	void ColorLimbs (Material mat, bool colorFX)
	{
		GameObject[] limbsToColor = colorFX ? fxLimbs : limbs;
		foreach(GameObject limb in limbsToColor)
		{
			limb.renderer.material = mat;
		}
	}
	
	/*
	 * Replaces the character with ragdoll pieces and gives them impulse to match
	 * the character's velocity.
	 */
	public void ReplaceWithRagdoll ()
	{
		float blockImpediment = 0.25f;
		Vector3 force = GameManager.Instance.player.perceivedVelocity * blockImpediment;
		for (int i = 0; i < Enum.GetNames(typeof(Limb)).Length; i++)
		{
			fxLimbs[i] = ReplaceLimb (limbs[i], fxLimbPrefabs[i], force);
		}
		
		SetColor (currentColor, true);
		
	}
	
	public void RestoreFromRagdoll ()
	{
		isReviving = true;
		
		foreach (GameObject limb in fxLimbs)
		{
			limb.GetComponent<FX_PigmentLimb> ().SetLerping (true);
		}
	}
	
	/*
	 * Replaces the specified GameObject with the specified Prefab and applies the specified force.
	 */
	GameObject ReplaceLimb (GameObject limb, GameObject limbFX, Vector3 force)
	{
		GameObject fx = (GameObject)Instantiate (limbFX, limb.transform.position,
			limb.transform.rotation);
		fx.rigidbody.AddForce (force, ForceMode.Impulse);
		fx.GetComponent<FX_PigmentLimb> ().SetOriginalLimb (limb, transform.gameObject);
		return fx;
	}
}

using UnityEngine;
using System.Collections;

public class PigmentBody : MonoBehaviour
{
	ColorWheel currentColor;
	public GameObject armL;
	public GameObject armR;
	public GameObject legL;
	public GameObject legR;
	public GameObject body;
	GameObject fxArmL;
	GameObject fxArmR;
	GameObject fxLegL;
	GameObject fxLegR;
	GameObject fxBody;
	public GameObject fxArmLPrefab;
	public GameObject fxArmRPrefab;
	public GameObject fxLegLPrefab;
	public GameObject fxLegRPrefab;
	public GameObject fxBodyPrefab;
	public Material[] bodyMaterials = new Material[4];
	bool isReviving;
	
	public void LimbIsDoneLerping()
	{
		if (fxArmL.GetComponent<FX_PigmentLimb> ().IsLerping) {
			return;
		}
		if (fxArmR.GetComponent<FX_PigmentLimb> ().IsLerping) {
			return;
		}
		if (fxLegL.GetComponent<FX_PigmentLimb> ().IsLerping) {
			return;
		}
		if (fxLegR.GetComponent<FX_PigmentLimb> ().IsLerping) {
			return;
		}
		if (fxBody.GetComponent<FX_PigmentLimb> ().IsLerping) {
			return;
		}
		isReviving = false;
			
		GameManager.Instance.player.RagdollRestored();
		
		Destroy (fxArmL);
		Destroy (fxArmR);
		Destroy (fxLegL);
		Destroy (fxLegR);
		Destroy (fxBody);
	}
	
	public void SetColor (ColorWheel color, bool colorFX)
	{
		GameObject bodyToColor = colorFX ? fxBody : body;
		Material armLegMat = ColorManager.Instance.red;
		if (color == ColorWheel.red) {
			armLegMat = ColorManager.Instance.red;
			bodyToColor.renderer.material = bodyMaterials [0];
		} else if (color == ColorWheel.green) {
			armLegMat = ColorManager.Instance.green;
			bodyToColor.renderer.material = bodyMaterials [1];
		} else if (color == ColorWheel.blue) {
			armLegMat = ColorManager.Instance.blue;
			bodyToColor.renderer.material = bodyMaterials [2];
		} else if (color == ColorWheel.neutral) {
			armLegMat = ColorManager.Instance.neutral;
			bodyToColor.renderer.material = bodyMaterials [3];
		}
		ColorArmsAndLegs (armLegMat, colorFX);
		
		currentColor = color;
	}
	
	public void SetColor (ColorWheel color)
	{
		SetColor (color, false);
	}
	
	void ColorArmsAndLegs (Material mat, bool colorFX)
	{
		if (colorFX) {
			fxArmL.renderer.material = mat;
			fxArmR.renderer.material = mat;
			fxLegL.renderer.material = mat;
			fxLegR.renderer.material = mat;
		} else {
			armL.renderer.material = mat;
			armR.renderer.material = mat;
			legL.renderer.material = mat;
			legR.renderer.material = mat;
		}
	}
	
	/*
	 * Replaces the character with ragdoll pieces and gives them impulse to match
	 * the character's velocity.
	 */
	public void ReplaceWIthRagdoll ()
	{
		float blockImpediment = 0.25f;
		Vector3 force = GameManager.Instance.player.perceivedVelocity * blockImpediment;
		fxBody = ReplaceLimb (body, fxBodyPrefab, force);
		fxArmL = ReplaceLimb (armL, fxArmLPrefab, force);
		fxArmR = ReplaceLimb (armR, fxArmRPrefab, force);
		fxLegL = ReplaceLimb (legL, fxLegLPrefab, force);
		fxLegR = ReplaceLimb (legR, fxLegRPrefab, force);
		
		SetColor (currentColor, true);
	}
	
	public void RestoreFromRagdoll ()
	{
		isReviving = true;
		
		fxArmL.GetComponent<FX_PigmentLimb> ().SetLerping (true);
		fxArmR.GetComponent<FX_PigmentLimb> ().SetLerping (true);
		fxLegL.GetComponent<FX_PigmentLimb> ().SetLerping (true);
		fxLegR.GetComponent<FX_PigmentLimb> ().SetLerping (true);
		fxBody.GetComponent<FX_PigmentLimb> ().SetLerping (true);
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

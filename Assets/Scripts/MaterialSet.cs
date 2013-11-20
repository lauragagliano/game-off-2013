using UnityEngine;
using System.Collections;

public class MaterialSet : MonoBehaviour {

	public GameObject armL;
	public GameObject armR;
	public GameObject legL;
	public GameObject legR;
	public GameObject body;
	
	public Material[] bodyMaterials = new Material[4];
	
	public void SetColor(ColorWheel color)
	{
		Material armLegMat = ColorManager.Instance.red;
		if(color == ColorWheel.red)
		{
			armLegMat = ColorManager.Instance.red;
			body.renderer.material = bodyMaterials[0];
		}
		else if(color == ColorWheel.green)
		{
			armLegMat = ColorManager.Instance.green;
			body.renderer.material = bodyMaterials[1];
		}
		else if(color == ColorWheel.blue)
		{
			armLegMat = ColorManager.Instance.blue;
			body.renderer.material = bodyMaterials[2];
		} else if(color == ColorWheel.neutral)
		{
			armLegMat = ColorManager.Instance.neutral;
			body.renderer.material = bodyMaterials[3];
		}
		ColorArmsAndLegs(armLegMat);
	}
	
	void ColorArmsAndLegs(Material mat)
	{
		armL.renderer.material = mat;
		armR.renderer.material = mat;
		legL.renderer.material = mat;
		legR.renderer.material = mat;
	}
}

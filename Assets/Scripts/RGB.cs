using UnityEngine;
using System.Collections;

/*
 * Attach this script to objects that the player will interact with
 * and to the player.
 */
public class RGB : MonoBehaviour
{
	public ColorWheel color;
	
	void Awake () {
		SetMaterialToCurrentColor ();
	}
	
	/*
	 * Check whether the color of the object this script is attached to is
	 * compatible with the passed in color. Compatible colors are those that
	 * either match or match one of the component colors in the color wheel.
	 */
	public bool isCompatible (RGB theirRGB) {
		if (theirRGB.color == color) {
			return true;
		}
		return false;
	}
	
	/*
	 * Set the renderer's color to whatever color this script is set to. This will
	 * keep our display color in sync with our logic. We can update these colors
	 * to custom colors later.
	 */
	void SetMaterialToCurrentColor ()
	{
		switch (color) {
		case ColorWheel.red:
			renderer.material = ColorManager.Instance.red;
			break;
		case ColorWheel.green:
			renderer.material = ColorManager.Instance.green;
			break;
		case ColorWheel.blue:
			renderer.material = ColorManager.Instance.blue;
			break;
		case ColorWheel.black:
			renderer.material = ColorManager.Instance.black;
			break;
		}
	}
	
	public void Refresh()
	{
		SetMaterialToCurrentColor();
	}
}
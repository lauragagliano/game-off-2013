using UnityEngine;
using System.Collections;

/*
 * Attach this script to objects that the player will interact with
 * and to the player.
 */
public class ColorLogic : MonoBehaviour
{
	public ColorWheel color;
	
	public enum ColorWheel
	{
		red,
		blue,
		yellow,
		purple,
		green,
		orange
	}
	
	/*
	 * Check whether the color of the object this script is attached to is
	 * compatible with the passed in color. Compatible colors are those that
	 * either match or match one of the component colors in the color wheel.
	 */
	public bool isCompatible (ColorWheel theirColor) {
		// Short-circuit if colors are the exact same
		if (theirColor == color) {
			return true;
		}
		switch (color) {
		case ColorWheel.red:
			if (theirColor == ColorWheel.purple || theirColor == ColorWheel.orange) {
				return true;
			}
			break;
		case ColorWheel.blue:
			if (theirColor == ColorWheel.green || theirColor == ColorWheel.purple) {
				return true;
			}
			break;
		case ColorWheel.yellow:
			if (theirColor == ColorWheel.orange || theirColor == ColorWheel.green) {
				return true;
			}
			break;
		case ColorWheel.purple:
			if (theirColor == ColorWheel.red || theirColor == ColorWheel.blue) {
				return true;
			}
			break;
		case ColorWheel.green:
			if (theirColor == ColorWheel.yellow || theirColor == ColorWheel.blue) {
				return true;
			}
			break;
		case ColorWheel.orange:
			if (theirColor == ColorWheel.red || theirColor == ColorWheel.yellow) {
				return true;
			}
			break;
		}
		return false;
	}
}

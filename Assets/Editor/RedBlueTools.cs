using UnityEngine;
using UnityEditor;
using System.Collections;

public class RedBlueTools : EditorWindow 
{
	[MenuItem("RedBlue Tools/RedBlue Tools")]
	static void Init ()
	{
		RedBlueTools window = (RedBlueTools)EditorWindow.CreateInstance (typeof(RedBlueTools));
		window.Show ();
	}
	
	void OnGUI ()
	{
		if (GUILayout.Button ("Refresh Colors")) {
			RefreshColors ();
		}
		if (GUILayout.Button ("Calculate Section Stats")) {
			CalculateSectionBitmapsAndPickups ();
		}
	}
	
	/*
	 * Read in a CSV Attack file generated from our Google Spreadsheet of attacks. Each
	 * line will be an Attack, stored as a comma delimited value for each field in our Attack class.
	 * Once complete, write the name of each attack in a lookup file of strings.
	 */
	void RefreshColors ()
	{
		RGB[] allBlocks = (RGB[])GameObject.FindObjectsOfType(typeof(RGB));
		foreach (RGB colorLogic in allBlocks) {
			colorLogic.Refresh();
		}
	}
	
	/*
	 * Iterate through all our sections and set the bitmap value and pickup count.
	 * This is meant to help out whoever is building sections in the editor add
	 * meaningful stats on each section without manual calculation.
	 */
	void CalculateSectionBitmapsAndPickups ()
	{
		Section[] allSections = (Section[])GameObject.FindObjectsOfType(typeof(Section));
		foreach (Section section in allSections) {
			section.SetEntranceAndExitBitmaps ();
			Debug.Log (string.Format ("Set {0} entrance to {1} and exit to {2}.", section.name, 
				section.entranceBitmap, section.exitBitmap));
			section.SetPickupCount ();
			Debug.Log (string.Format ("Set {0} pickup count to {1}", section.name, 
				section.numberOfPickups));
		}
	}
}

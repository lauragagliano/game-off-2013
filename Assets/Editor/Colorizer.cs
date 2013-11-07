using UnityEngine;
using UnityEditor;
using System.Collections;

public class Colorizer : EditorWindow 
{
	[MenuItem("RedBlue Tools/Colorizer")]
	static void Init ()
	{
		Colorizer window = (Colorizer)EditorWindow.CreateInstance (typeof(Colorizer));
		window.Show ();
	}
	
	void OnGUI ()
	{
		if (GUILayout.Button ("Refresh Colors")) {
			RefreshColors ();
		}
	}
	
	/*
	 * Read in a CSV Attack file generated from our Google Spreadsheet of attacks. Each
	 * line will be an Attack, stored as a comma delimited value for each field in our Attack class.
	 * Once complete, write the name of each attack in a lookup file of strings.
	 */
	void RefreshColors ()
	{
		ColorLogic[] allBlocks = (ColorLogic[])GameObject.FindObjectsOfType(typeof(ColorLogic));
		foreach (ColorLogic colorLogic in allBlocks) {
			colorLogic.Refresh();
		}
	}
}

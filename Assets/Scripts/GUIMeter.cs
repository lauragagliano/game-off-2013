using UnityEngine;
using System.Collections;

public class GUIMeter : MonoBehaviour
{
	float maxScale;
	float maxSize;
	public Power power;
	public GameObject fill;
	public AnimationClip glowAnimation;
	public Material fillMaterial;
	
	void Start ()
	{
		if (power == null) {
			Debug.LogError ("Meter [" + name + "] not tied to a power.");
		}
		// Cache the scale of the fill meter.
		maxScale = fill.transform.localScale.x;
		
		// Cache the size of the fillbar to be used to adjust the anchoring
		maxSize = transform.renderer.bounds.extents.x;
	}
	
	void Update ()
	{
		float currentFillPercentage = power.GetFillPercentage ();
		// Set scale to the current fill percentage
		fill.transform.localScale = new Vector3 (currentFillPercentage * maxScale, fill.transform.localScale.y, 
			fill.transform.localScale.z);
		
		// Adjust "anchoring" of the fill to always be to the left of the parent object
		float missingX = maxSize * (1 - currentFillPercentage) * maxScale;
		fill.transform.localPosition = new Vector3 (-missingX, fill.transform.localPosition.y,
			fill.transform.localPosition.z);
		
		// Glow an active meter
		if (power.IsPowerActive ()) {
			Animation animation = GetComponent<Animation>();
			animation.Play (glowAnimation.name);
		} else {
			Animation animation = GetComponent<Animation>();
			animation.Stop();
			fill.renderer.material = fillMaterial;
		}
	}
}

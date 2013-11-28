using UnityEngine;
using System.Collections;

public class FX_BoostLine : MonoBehaviour
{
	int vertexCount = 0;
	static int MAX_VERTS = 15;
	LineRenderer lineRenderer;
	Vector3[] vertexSet = new Vector3[MAX_VERTS];
	
	void Awake ()
	{
		LinkLineRenderers ();
	}
	
	void Update ()
	{
		ShiftVerteces ();
		SpoofMoving ();
	}
	
	/*
	 * Cache references to the lineRenderer componenet
	 */
	void LinkLineRenderers ()
	{
		lineRenderer = GetComponent<LineRenderer> ();
	}
	
	/*
	 * Shifts the verteces down to make room for a new position at the front
	 * of the line.
	 */
	void ShiftVerteces ()
	{
		// Add an extra vertex if not maxed
		if (vertexCount < MAX_VERTS) {
			lineRenderer.SetVertexCount (++vertexCount);
		}
		
		// Shift verteces down
		int i = vertexCount - 1;
		while (i > 0) {
			lineRenderer.SetPosition (i, vertexSet [i - 1]);
			vertexSet [i] = vertexSet [i - 1];
			i--;
		}
		lineRenderer.SetPosition (0, transform.position);
		vertexSet [0] = transform.position;
	}
	
	/*
	 * Moves all the segments in the line down (except the first) as if the player moved.
	 */
	void SpoofMoving ()
	{
		int j = 1;
		while (j < vertexCount) {
			vertexSet [j] = vertexSet [j] + new Vector3 (0.0f, 0.0f, 
				-GameManager.Instance.treadmill.scrollspeed * Time.deltaTime);
			j++;
		}
		int i = 1;
		while (i < vertexCount) {
			lineRenderer.SetPosition (i, vertexSet [i]);
			i++;
		}
	}
}

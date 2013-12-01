using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
	
	public AudioClip[] musicTracks;
	AudioSource audio;
	int currentTrack = -1;
	public bool startOnRandomTrack = false;
	
	void Awake ()
	{
		audio = GetComponent<AudioSource> ();
		if ( startOnRandomTrack ) {
			currentTrack = Random.Range(0, musicTracks.Length);
		}
	}
	
	void Update ()
	{
		if (audio.isPlaying == false) {
			audio.clip = GetNextTrack ();
			audio.Play ();
		}
	}
	
	/* Returns the next track to play
	 */
	AudioClip GetNextTrack ()
	{
		currentTrack++;
		if (currentTrack >= musicTracks.Length) {
			currentTrack = 0;
		}
		return musicTracks [currentTrack];
	}
}

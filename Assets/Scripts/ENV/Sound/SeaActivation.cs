using UnityEngine;
using System.Collections;

public class SeaActivation : MonoBehaviour {

	public AudioHandler audioHandler;

	public string seaSound = "Mare";
	public string seagullsSound = "Gabbiani";

	void Start () {
	
	}

	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			activeSound();
		}
	}

	void activeSound()
	{
		if (audioHandler != null)
		{
			audioHandler.playClipByName("Mare");
			audioHandler.playClipByName("Gabbiani");
		}
	}
}

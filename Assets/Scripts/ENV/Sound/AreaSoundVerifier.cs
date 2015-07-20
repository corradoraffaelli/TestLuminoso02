using UnityEngine;
using System.Collections;

public class AreaSoundVerifier : MonoBehaviour {

	bool collidingPlayer = false;
	bool wasCollidingPlayer = false;

	public GameObject soundObject;
	AudioHandler audioHandler;

	public string audioName;

	void Start () {
		if (soundObject != null)
			audioHandler = soundObject.GetComponent<AudioHandler>();

		if (audioHandler != null)
			audioHandler.playClipByName(audioName);
	}

	void Update () {
		if (collidingPlayer != wasCollidingPlayer)
		{
			if (collidingPlayer)
			{
				setPlayerPosition();
				setSoundObjectParent(GeneralFinder.player.transform);
			}
			else
				setSoundObjectParent(null);
		}
		wasCollidingPlayer = collidingPlayer;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			collidingPlayer = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			collidingPlayer = false;
		}
	}

	void setSoundObjectParent(Transform parentTransform)
	{
		if (soundObject != null)
		{
			soundObject.transform.SetParent(parentTransform);
		}
	}

	void setPlayerPosition()
	{
		if (soundObject != null)
		{
			soundObject.transform.position = GeneralFinder.player.transform.position;
		}
	}
}

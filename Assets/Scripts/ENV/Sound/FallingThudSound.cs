using UnityEngine;
using System.Collections;

public class FallingThudSound : MonoBehaviour {

	Rigidbody2D rigidbody;
	AudioHandler audioHandler;

	float lastYSpeed;

	public string soundName = "Tonfo";

	public bool isPlayer = false;

	float[] lastYSpeeds = new float[4];

	[Range(0.0f, 20.0f)]
	public float minSpeedSound = 10.0f;
	[Range(0.0f, 5.0f)]
	public float minInterval = 2.0f;

	void Start () {
		rigidbody = GetComponent<Rigidbody2D>();
		audioHandler = GetComponent<AudioHandler>();
		if (rigidbody != null)
			lastYSpeed = rigidbody.velocity.y;

		for (int i = 0; i < lastYSpeeds.Length; i++)
		{
			lastYSpeeds[i] = 0.0f;
		}


	}

	void Update () {

	}

	void handleSound()
	{
		float actualSpeed = rigidbody.velocity.y;
		if (actualSpeed > -minInterval && actualSpeed < minInterval && lastYSpeed < -minSpeedSound)
		{
			Debug.Log ("asdads");
			if (isPlayer)
			{
				if(GeneralFinder.playerMovements.onGround)
					audioHandler.playClipByName("Tonfo");
			}
			else
				audioHandler.playClipByName("Tonfo");
		}

		lastYSpeed = actualSpeed;
	}

	void handleSound02()
	{
		float actualSpeed = rigidbody.velocity.y;

		/*
		Debug.Log ("inizio");
		Debug.Log (lastYSpeeds[0]);
		Debug.Log (actualSpeed);
		*/

		if (actualSpeed > -minInterval && actualSpeed < minInterval && lastYSpeeds[0] < -minSpeedSound)
		{
			//Debug.Log ("asdads");
			audioHandler.playClipByName(soundName);
		}

		for (int i = 0; i < lastYSpeeds.Length - 1; i++)
		{
			lastYSpeeds[i] = lastYSpeeds[i+1];
		}
		lastYSpeeds[lastYSpeeds.Length - 1] = actualSpeed;
	}

	void FixedUpdate()
	{
		handleSound02();
	}
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce il suono degli oggetti che vengono trascinati (cassa)
/// </summary>

// Corrado
public class ShiftingSound : MonoBehaviour {

	Rigidbody2D rigidbody;
	AudioHandler audioHandler;
	
	public string soundName = "Spinta";

	public float minMovement = 2.0f;
	public float yThreshold = 0.02f;

	bool wasActive = false;

	void Start () {
		rigidbody = GetComponent<Rigidbody2D>();
		audioHandler = GetComponent<AudioHandler>();
	}

	void Update () {
		soundHandler();
	}

	void soundHandler()
	{
		bool active = false;
		if (rigidbody != null && audioHandler != null)
		{
			if (rigidbody.velocity.y < yThreshold && rigidbody.velocity.y > -yThreshold && 
			    (rigidbody.velocity.x > minMovement || rigidbody.velocity.x < -minMovement))
				active = true;

			if (active != wasActive)
			{
				//Debug.Log ("ininin");
				if (active)
					audioHandler.playClipByName(soundName);
				else
					audioHandler.stopClipByName(soundName);
			}

			wasActive = active;
		}
	}
}

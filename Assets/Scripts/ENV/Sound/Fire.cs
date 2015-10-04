using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce il suono del fuoco
/// </summary>

// Corrado
public class Fire : MonoBehaviour {

	public bool useAudioHandler = false;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnBecameVisible ()
	{
		if (!useAudioHandler) {
			if (audioSource != null)
				audioSource.enabled = true;
				audioSource.Play();
		}
	}

	void OnBecameInvisible()
	{
		if (!useAudioHandler) {
			if (audioSource != null)
				audioSource.Stop ();
		}
	}
}

using UnityEngine;
using System.Collections;

public class AudioHandler : MonoBehaviour {

	[System.Serializable]
	public class AudioClipGeneral{
		public string clipName;
		public AudioClip clip;
		public bool loop;
		[Range(0.0f,1.0f)]
		public float volume = 1.0f;
		[Range(-3.0f, 3.0f)]
		public float pitch = 1.0f;
		//[HideInInspector]
		public AudioSource audioSource;
	}

	public bool updateAudioClips = false;

	[SerializeField]
	AudioClipGeneral[] clips;

	//public AudioSource audioSource;

	void updateAudioSources()
	{
		AudioSource[] sources = GetComponents<AudioSource> ();
		for (int i = 0; i<sources.Length; i++)
			Destroy (sources [i]);

		for (int i = 0; i< clips.Length; i++)
		{
			if (clips[i] != null)
			{
				if (clips[i].audioSource == null)
					clips[i].audioSource = gameObject.AddComponent<AudioSource>();
				clips[i].audioSource.playOnAwake = false;
				clips[i].audioSource.clip = clips[i].clip;
				clips[i].audioSource.volume = clips[i].volume;
				clips[i].audioSource.loop = clips[i].loop;
				clips[i].audioSource.pitch = clips[i].pitch;
			}
		}
	}

	public void playClipByIndex(int clipIndex)
	{
		if (clips [clipIndex] != null && clips[clipIndex].audioSource!=null && !clips[clipIndex].audioSource.isPlaying) {
			clips [clipIndex].audioSource.Play();
		}
	}

	public void playClipByName(string clipNameInput)
	{
		if (clips.Length != 0) {
			for (int i = 0; i< clips.Length; i++) {
				if (clips[i] != null && clips[i].clipName == clipNameInput && clips[i].audioSource!=null && !clips[i].audioSource.isPlaying){
					clips[i].audioSource.Play();
					break;
				}
			}
		}
	}

	public void stopClipByIndex(int clipIndex)
	{
		if (clips [clipIndex] != null && clips[clipIndex].audioSource!=null) {
			clips [clipIndex].audioSource.Stop();
		}
	}

	public void stopClipByName(string clipNameInput)
	{
		for (int i = 0; i< clips.Length; i++) {
			if (clips [i] != null && clips [i].clipName == clipNameInput && clips[i].audioSource!=null) {
				clips [i].audioSource.Stop();
				break;
			}
		}
	}

	// Use this for initialization
	void Start () {
		updateAudioSources ();
		/*
		if (audioSource == null)
			audioSource = GetComponent<AudioSource> ();

		if (audioSource == null) {
			audioSource = gameObject.AddComponent<AudioSource> ();
		}

		if (audioSource == null)
			Debug.Log ("Attenzione!! L'oggetto " + gameObject.name + " non è riuscito a trovare un Audio Source valido!!");

		if (audioSource != null)
			audioSource.playOnAwake = false;
			*/
	}
	
	// Update is called once per frame
	void Update () {
		cleanNotPlaying ();
		if (updateAudioClips) {
			updateAudioSources();
			updateAudioClips = false;
		}
	}

	void cleanNotPlaying()
	{
		/*
		for (int i = 0; i< clips.Length; i++)
		{
			if (clips[i] != null)
			{
				if (clips[i].needSecondaryAudioSource)
				{
					if (clips[i].secondaryAudioSourceObject != null && !clips[i].secondaryAudioSource.isPlaying)
					{
						clips[i].secondaryAudioSource.Stop();
						Destroy(clips[i].secondaryAudioSourceObject);
					}
				}
			}
		}
		*/
	}
}

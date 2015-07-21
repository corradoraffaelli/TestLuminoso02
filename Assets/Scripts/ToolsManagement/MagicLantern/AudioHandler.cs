using UnityEngine;
using System.Collections;

public class AudioHandler : MonoBehaviour {

	[System.Serializable]
	public class AudioClipGeneral{
		public string clipName;
		public AudioClip clip;
		public bool loop;
		public bool playOnAwake = false;
		[Range(0.0f,1.0f)]
		public float volume = 1.0f;
		[Range(-3.0f, 3.0f)]
		public float pitch = 1.0f;

		public bool distanceDependency = false;
		[Range(0.0f, 30.0f)]
		public float maxVolumeDistance = 5.0f;
		[Range(0.0f, 50.0f)]
		public float zeroVolumeDistance = 10.0f;

		//[HideInInspector]
		public float volumeMultiplier = 1.0f;
		public void setVolumeMultiplier(float inputMultiplier)
		{
			volumeMultiplier = inputMultiplier;
		}

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

	public AudioClipGeneral getAudioClipByName(string clipNameInput)
	{
		if (clips.Length != 0) {
			for (int i = 0; i< clips.Length; i++) {
				if (clips[i] != null && clips[i].clipName == clipNameInput){
					return clips[i];
				}
			}
		}
		return null;
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

	public void playForcedClipByName(string clipNameInput)
	{
		if (clips.Length != 0) {
			for (int i = 0; i< clips.Length; i++) {
				if (clips[i] != null && clips[i].clipName == clipNameInput && clips[i].audioSource!=null){
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

	void Awake()
	{
		setStandardMultipliers();
	}

	void Start () {
		updateAudioSources ();

		startIfNecessary();
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
		setVolumeByDistance();
		setVolumeByMultiplier();
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

	void setVolumeByDistance()
	{
		float playerDistance = Vector3.Distance(transform.position, GeneralFinder.player.transform.position);

		for (int i = 0; i< clips.Length; i++) {
			if (clips [i] != null && clips[i].audioSource!=null) {
				if (clips[i].distanceDependency)
				{
					float maxVolume = clips[i].volume;

					float b = maxVolume/(clips[i].maxVolumeDistance - clips[i].zeroVolumeDistance);
					float a = -b * clips[i].zeroVolumeDistance;

					float volume = a + b * playerDistance;
					if (volume < 0.0f)
						volume = 0.0f;
					if (volume > maxVolume)
						volume = maxVolume;

					clips[i].audioSource.volume = volume;
				}
			}
		} 
	}

	void setVolumeByMultiplier()
	{
		for (int i = 0; i< clips.Length; i++) {
			if (clips [i] != null && clips[i].audioSource!=null) {

				float maxVolume = clips[i].audioSource.volume;
				
				float volume = maxVolume * clips[i].volumeMultiplier;
				if (volume < 0.0f)
					volume = 0.0f;
				if (volume > 1.0f)
					volume = 1.0f;
				
				clips[i].audioSource.volume = volume;

			}
		} 
	}

	void startIfNecessary()
	{
		for (int i = 0; i< clips.Length; i++) {
			if (clips [i] != null && clips[i].audioSource!=null && clips[i].playOnAwake) 
			{
				clips[i].audioSource.playOnAwake = true;
				clips[i].audioSource.Play();
			}
		} 
	}

	void setStandardMultipliers()
	{
		for (int i = 0; i< clips.Length; i++) {
			if (clips [i] != null) 
			{
				clips[i].volumeMultiplier = 1.0f;
			}
		} 
	}
}

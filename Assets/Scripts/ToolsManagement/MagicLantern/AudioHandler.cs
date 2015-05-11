using UnityEngine;
using System.Collections;

public class AudioHandler : MonoBehaviour {

	[System.Serializable]
	public class AudioClipGeneral{
		public AudioClip clip;
		public string clipName;
	}

	[SerializeField]
	AudioClipGeneral[] clips;

	public AudioSource audioSource;

	public void playClipByIndex(int clipIndex)
	{
		if (clips [clipIndex] != null) {
			audioSource.clip = clips[clipIndex].clip;
			audioSource.Play();
		}
	}

	public void playClipByName(string clipNameInput)
	{
		if (clips.Length != 0) {
			for (int i = 0; i< clips.Length; i++)
			{
				if (clips[i] != null && clips[i].clipName == clipNameInput)
				{
					audioSource.clip = clips[i].clip;
					audioSource.Play();
					break;
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
		if (audioSource == null)
			audioSource = GetComponent<AudioSource> ();

		if (audioSource == null) {
			audioSource = gameObject.AddComponent<AudioSource> ();
		}

		if (audioSource == null)
			Debug.Log ("Attenzione!! L'oggetto " + gameObject.name + " non è riuscito a trovare un Audio Source valido!!");

		if (audioSource != null)
			audioSource.playOnAwake = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

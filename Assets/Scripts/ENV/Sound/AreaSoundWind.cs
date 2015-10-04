using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce il suono ad area del vento.
/// </summary>

// Corrado
public class AreaSoundWind : MonoBehaviour {

	GameObject referenceWind;
	AreaEffector2D effector;
	bool savedGO = false;

	bool windActive = false;
	bool wasWindActive = false;

	AreaSoundVerifier soundVerifier;
	GameObject soundObject;
	AudioHandler audioHandler;
	AudioHandler.AudioClipGeneral windClip;
	string soundName;

	float standardMultiplier = 1.0f;
	public float changingSpeed = 2.0f;
	bool settedMaxOrMin = false;

	void Start () {
		soundVerifier = GetComponent<AreaSoundVerifier>();
		if (soundVerifier != null)
		{
			soundName = soundVerifier.audioName;

			soundObject = soundVerifier.soundObject;
			if (soundObject!=null)
			{
				audioHandler = soundObject.GetComponent<AudioHandler>();
				if (audioHandler != null)
				{
					windClip = audioHandler.getAudioClipByName(soundName);
				}
			}
		}
	}

	void Update () {
		windActive = isWindActive();

		/*
		if (windActive != wasWindActive)
		{
			Debug.Log ("ajdinfakdjgnajgnp");
		}
		*/

		changeMultiplier();

		//wasWindActive = windActive;
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (!savedGO) 
		{
			AreaEffector2D areaEffector = other.gameObject.GetComponent<AreaEffector2D>();
			if (areaEffector != null)
			{
				referenceWind = other.gameObject;
				savedGO = true;
				effector = areaEffector;
			}
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (savedGO) 
		{
			if (other.gameObject == referenceWind)
			{
				referenceWind = null;
				effector = null;
			}
		}
	}

	bool isWindActive()
	{
		if (referenceWind != null && effector != null)
		{
			if (referenceWind.activeInHierarchy == true && effector.enabled == true)
				return true;
			else
			{
				referenceWind = null;
				effector = null;
			}
		}
		savedGO = false;
		return false;
	}

	void changeMultiplier()
	{
		if (windActive)
			standardMultiplier = Mathf.MoveTowards(standardMultiplier, 1.0f, Time.deltaTime * changingSpeed);
		else
			standardMultiplier = Mathf.MoveTowards(standardMultiplier, 0.0f, Time.deltaTime * changingSpeed);

		if (!settedMaxOrMin)
		{
			windClip.setVolumeMultiplier(standardMultiplier);

			if (standardMultiplier == 1.0f || standardMultiplier == 0.0f)
			{
				settedMaxOrMin = true;
			}
		}

		if (standardMultiplier != 1.0f && standardMultiplier != 0.0f)
		{
			settedMaxOrMin = false;
		}

	}
}

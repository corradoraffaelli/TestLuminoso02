using UnityEngine;
using System.Collections;

/// <summary>
/// Specifica del livello 1. Gestisce l'audio della piattaforma killer gigante (non più presente)
/// </summary>

// Corrado
public class BigPlatformAudio : MonoBehaviour {

	public movingPlatform movingPlat;

	AreaSoundVerifier areaSound;
	AudioHandler audioHandler;

	public string soundUpName = "Up";
	public string soundDownName = "Down";

	bool _switch;
	bool musicSwitchActivated = false;
	bool musicNoSwitchActivated = false;


	void Start () {
		areaSound = GetComponent<AreaSoundVerifier>();
		if (areaSound != null)
		{
			if (areaSound.soundObject != null)
			{
				audioHandler = areaSound.soundObject.GetComponent<AudioHandler>();
			}
		}
	}

	void Update () {
		if (movingPlat != null && audioHandler != null)
		{
			_switch = movingPlat.Switch;

			if (!_switch && !musicNoSwitchActivated && audioHandler != null)
			{
				audioHandler.playClipByName(soundUpName);
				audioHandler.stopClipByName(soundDownName);
			}
			
			if (_switch && !musicSwitchActivated && audioHandler != null)
			{
				audioHandler.playClipByName(soundDownName);
				audioHandler.stopClipByName(soundUpName);
			}
		}
	}
}

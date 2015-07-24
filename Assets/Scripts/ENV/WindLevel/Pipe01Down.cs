using UnityEngine;
using System.Collections;

public class Pipe01Down : MonoBehaviour {

	SimpleLinearMovement linearMovement;
	public GameObject[] extWindToActive;
	public GameObject[] extWindToDeactive;

	AudioHandler audioHandler;

	bool activated = false;
	public string pipeDownSound = "TuboGiu";
	bool soundPlayed = false;

	void Start()
	{
		audioHandler = GetComponent<AudioHandler>();
		linearMovement = GetComponent<SimpleLinearMovement>();
	}

	public void InteractingMethod()
	{
		activated = true;

		if (linearMovement != null)
		{
			linearMovement.active = true;
		}

		for (int i = 0; i<extWindToActive.Length; i++)
		{
			if(extWindToActive[i] != null)
				extWindToActive[i].SetActive(true);
		}

		for (int i = 0; i<extWindToDeactive.Length; i++)
		{
			if(extWindToDeactive[i] != null)
				extWindToDeactive[i].SetActive(false);
		}
	}

	void Update()
	{
		if (activated && !soundPlayed && linearMovement != null && linearMovement.DestReached && audioHandler != null)
		{
			audioHandler.playClipByName(pipeDownSound);
			soundPlayed = true;
		}
			
	}
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Specifica del livello del vento. Gestisce il comportamento del secondo pezzo di tubo.
/// </summary>

// Corrado
public class Pipe02Down : MonoBehaviour {

	public GameObject firstPipe;
	public GameObject firstDest;
	public GameObject secondDest;
	public GameObject leva;

	public bool canBeActivated = false;

	SimpleLinearMovement linearMovementPipe01;
	SimpleLinearMovement linearMovement;
	InteragibileObject interagibileMethod;

	public int turningNum = 8;
	int actualTurningNum = 0;

	public float uberSpeed = 20.0f;
	public float normalSpeed = 4.0f;

	bool suEGiu = false;

	AudioHandler audioHandler;
	public string soundName;
	bool firstPassed = false;

	public string soundPipeDown = "TuboGiu";
	bool soundPlayed = false;

	void Start () {
		linearMovement = GetComponent<SimpleLinearMovement>();
		if (firstPipe != null)
			linearMovementPipe01 = firstPipe.GetComponent<SimpleLinearMovement>();
		if (leva != null)
			interagibileMethod = leva.GetComponent<InteragibileObject>();

		audioHandler = GetComponent<AudioHandler>();
	}
	

	void Update () {
		if (linearMovementPipe01.isOnDestination())
			canBeActivated = true;

		if (suEGiu)
			setLinearVariables();

		handleSound();
	}

	public void InteractingMethod02()
	{
		if (canBeActivated)
		{
			firstPassed = true;

			if (interagibileMethod != null)
			{
				interagibileMethod.oneTimeInteraction = true;
			}

			if (linearMovement != null)
			{
				linearMovement.setDestinationPosition(secondDest.transform.position);
				linearMovement.canReturnBack = false;
				linearMovement.speed = normalSpeed;
				linearMovement.active = true;
			}

		}else{

			if (linearMovement != null)
			{
				suEGiu = true;
			}
		}
	}

	void setLinearVariables()
	{
		linearMovement.setDestinationPosition(firstDest.transform.position);
		linearMovement.canReturnBack = true;
		linearMovement.pauseToSwitch = 0.0f;
		
		//if (actualTurningNum < turningNum)
		//{
			linearMovement.active = true;
			linearMovement.speed = uberSpeed;
			if (linearMovement.isOnDestination() || linearMovement.isOnOrigin())
				actualTurningNum ++;
		//}
		if (actualTurningNum > turningNum && linearMovement.isOnOrigin())
		{
			//Debug.Log ("entrato");
			actualTurningNum = 0;
			linearMovement.active = false;
			suEGiu = false;
		}
			
	}

	void handleSound()
	{
		if (!firstPassed && linearMovement != null && audioHandler != null)
		{
			if (linearMovement.DestReached)
				audioHandler.playForcedClipByName(soundName);
		}

		if (firstPassed && !soundPlayed && linearMovement != null && audioHandler != null)
		{
			if (linearMovement.DestReached)
			{
				soundPlayed = true;
				audioHandler.playForcedClipByName(soundPipeDown);
			}
				
		}
	}
}

﻿using UnityEngine;
using System.Collections;

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

	void Start () {
		linearMovement = GetComponent<SimpleLinearMovement>();
		if (firstPipe != null)
			linearMovementPipe01 = firstPipe.GetComponent<SimpleLinearMovement>();
		if (leva != null)
			interagibileMethod = leva.GetComponent<InteragibileObject>();
	}
	

	void Update () {
		if (linearMovementPipe01.isOnDestination())
			canBeActivated = true;

		if (suEGiu)
			setLinearVariables();
	}

	public void InteractingMethod02()
	{
		if (canBeActivated)
		{
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
}
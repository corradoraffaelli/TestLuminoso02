using UnityEngine;
using System.Collections;

[System.Serializable]
public class PatrolParameters {

	//Gestione patrol----------------------------------------------------------------------------------
	public GameObject []patrolPoints;
	//verso di default dove puntare lo sguardo nel caso di un singolo punto di patrol
	public bool DefaultVerseRight = true;

	[Range(0.1f,10.0f)]
	public float patrolSpeed = 4.0f;

	[Range(0.1f,10.0f)]
	public float patrolSuspiciousSpeed = 2.0f;

	//nuova gestione suspicious
	bool firstCheckDone_Suspicious = false;
	[Range(0.1f,10.0f)]
	public float tSearchLenght = 2.5f;
	bool standingSusp = false;


	bool exitSuspicious = false;

	bool ExitSuspicious {
		get{ return exitSuspicious;}
		set { exitSuspicious = value;}

	}
	
	//variabili da resettare ad inizio stato
	bool patrollingTowardAPoint = false;
	Transform patrolledTarget;//utile dichiararlo momentaneamente public per vedere che valore ha
	
	[Range(0.1f,10.0f)]
	public float DEFAULT_DUMB_SPEED = 2.0f;
	
	//Gestione raycast target------------------------------------
	//public LayerMask targetLayers; dichiarata su
	float frontalDistanceOfView = 5.0f;
	float scale_FrontalDistanceOfView_ToBeFar = 1.5f;
	float backDistanceOfView = 2.0f;
}

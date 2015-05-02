using UnityEngine;
using System.Collections;

public class PatrolFSM : StateFSM {

	PatrolParameters pa;

	//patrol parameters
	//Gestione patrol----------------------------------------------------------------------------------
	public GameObject []patrolPoints;
	//verso di default dove puntare lo sguardo nel caso di un singolo punto di patrol
	public bool DefaultVerseRight = true;
	
	//nuova gestione suspicious
	bool firstCheckDone_Suspicious = false;
	public float tSearchLenght = 2.5f;
	bool standingSusp = false;
	bool exitSuspicious = false;
	
	//variabili da resettare ad inizio stato
	bool patrollingTowardAPoint = false;
	Transform patrolledTarget;//utile dichiararlo momentaneamente public per vedere che valore ha
	
	//public float DEFAULT_DUMB_SPEED = 2.0f;
	
	//Gestione raycast target------------------------------------
	//public LayerMask targetLayers; dichiarata su
	float frontalDistanceOfView = 5.0f;
	float scale_FrontalDistanceOfView_ToBeFar = 1.5f;
	float backDistanceOfView = 2.0f;



	public PatrolFSM(GameObject go) : base(go) {

		state = myStateName.Patrol;

		if(parametersScript!=null)
			setPatrolPar(ref parametersScript.patrolParameters);

		myInitialize += initializePatrol;
		
		myUpdate += updatePatrol;
		
		myFinalize += finalizePatrol;

		myTransition += patrolToChaseTransition;

		myTransition += patrolToFleeTransition;

	}

	private void setPatrolPar(ref PatrolParameters p) {

		pa = p;

	}

	private void initializePatrol(){
		Debug.Log ("inizio PATROL --------------------------------");
	}
	
	private void updatePatrol(){
		Debug.Log ("ciao da PATROL");
	}
	
	private void finalizePatrol(){
		Debug.Log ("finisco PATROL --------------------------------");
	}

	private void patrolToChaseTransition(ref myStateName st){

		if (st != myStateName.Patrol)
			return;

		if (Input.GetKeyUp(KeyCode.C)) {
			st = myStateName.Chase;
		} else {
			return;
		}
	}

	private void patrolToFleeTransition(ref myStateName st){

		if (st != myStateName.Patrol)
			return;

		if (Input.GetKeyUp(KeyCode.F)) {
			st = myStateName.Flee;
		} else {
			return;
		}
	}
}

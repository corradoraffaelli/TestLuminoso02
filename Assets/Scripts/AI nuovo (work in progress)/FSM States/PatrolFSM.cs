using UnityEngine;
using System.Collections;

public class PatrolFSM : StateFSM {
	
	public enum patrolSubState {
		Walk,
		Stand,
		Area,
		AreaSuspicious,
	}
	public patrolSubState paSS;
	patrolSubState defaultPaType;
	//PatrolParameters pa;

	public PatrolParameters patrolPar;

	float RangeOfView {
		get{ 
			if(patrolPar!=null) return patrolPar.RangeOfView;
			else return 0.0f;}
		set{ 
			if(patrolPar!=null) patrolPar.RangeOfView = value;}

	}

	GameObject patrolTarget {
		get{ 
			if(patrolPar!=null) return patrolPar.patrolTarget;
			else return null;}
		set{ if(patrolPar!=null) patrolPar.patrolTarget = value;}

	}
	
	GameObject foundTarget {
		get{ 
			if(patrolPar!=null) return patrolPar.foundTarget;
			else return null;}
		set{ if(patrolPar!=null) patrolPar.foundTarget = value;}
		
	}

	float patrolSpeed {
		get{ 
			if(patrolPar!=null) return patrolPar.patrolSpeed;
			else return 0.0f;}
		set{ if(patrolPar!=null) patrolPar.patrolSpeed = value;}

	}
	/*
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
	Transform patrolTarget;//utile dichiararlo momentaneamente public per vedere che valore ha
	
	//public float DEFAULT_DUMB_SPEED = 2.0f;
	
	//Gestione raycast target------------------------------------
	//public LayerMask targetLayers; dichiarata su
	float frontalDistanceOfView = 5.0f;
	float scale_FrontalDistanceOfView_ToBeFar = 1.5f;
	float backDistanceOfView = 2.0f;

	*/

	//CONSTRUCTOR----------------------------------------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------------------------------------------------

	public PatrolFSM(GameObject gameo, patrolSubState patrolT) : base(gameo) {

		//GameObject gameo = this.gameObject;

		//myGameObject = gameo;

		state = myStateName.Patrol;
		stateName = "Patrol";



		//paSS = patrolSubState.Walk;

		//if(par!=null)
		//	setPatrolPar(ref par.patrolParameters);

		//myInitialize += initializePatrol;
		
		myUpdate -= updateState;

		switch (patrolT) {

		case patrolSubState.Walk :
			myUpdate += updatePatrolWalk;
			myHandleCollisionEnter += wanderHandleCollisionEnter;
			break;

		case patrolSubState.Area :
			myUpdate += updatePatrolArea;
			break;

		case patrolSubState.Stand :
			//myUpdate += updatePatrolPoint;
			break;
		}
		
		//myFinalize += finalizePatrol;

		myTransitions = new myStateTransition[2];

		myTransitions[0] += P2FcheckDanger;

		myTransitions[1] += P2CcheckChaseTarget;

		//IEnumerator

		initializePatrolParameters ();
		//myHandleCollision;

	}

	void initializePatrolParameters(){

		//Debug.Log ("mio game object" + myGameObject.name);
		//Debug.Log ("ecco " + myGameObject.GetComponent<AIParameters> ()._AIwalkSpeed);

		//patrolPar = new PatrolParameters ();

		patrolPar = myGameObject.GetComponent<AIParameters> ().patrolParameters;

		if (patrolPar != null) {
			if(!getRangeOfView()) {
				//arbitrario valore di default 
				RangeOfView = 3.0f;
			}
		} 
		else {
			Debug.Log ("ATTENZIONE - PatrolParameters non trovato perché non è stato trovato AIParameters");
		}

	}

	bool getRangeOfView(){
		
		GameObject range = null;

		foreach (Transform child in transform) {
			if(child.gameObject.name=="RangeOfView") {
				range = child.gameObject;
			}
			
		}
		
		if (range != null) {
			
			if(range.transform.localPosition.x < 0) 
				Debug.Log("ATTENZIONE - L'empty 'RangeOfView' è in una posizione negativa");
			
			RangeOfView = Mathf.Abs( range.transform.localPosition.x ) * Mathf.Abs( transform.localScale.x );
			
			return true;
			
		}
		else {
			
			return false;
			
		}
		
	}

	private void setPatrolPar(ref PatrolParameters p) {

		//pa = p;

	}

	//END CONSTRUCTOR------------------------------------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------------------------------------------------


	//INITIALIZE---------------------------------------------------------------------------------------------------------------------------------

	protected override void initializeState(ref object ob) {

		patrolTarget = null;

	}

	//END INITIALIZE---------------------------------------------------------------------------------------------------------------------------------

	
	//UPDATES---------------------------------------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------------------------

	protected override void updateState() {

	}

	void updatePatrolWalk(){

		i_move (patrolSpeed);

	}

	void updatePatrolArea(){

		patrolBetweenPoints ();

	}

	private void patrolBetweenPoints() {

		if (patrolTarget == null) {
			if(patrolPar.patrolPoints.Length > 1) {
				if(patrolPar.patrolPoints[0] != null && patrolPar.patrolPoints[1]!= null) {
					int randomIndex = Random.Range(0,2);
					patrolTarget = patrolPar.patrolPoints[randomIndex];
				}
				else {
					Debug.Log ("ATTENZIONE - problemi con i patrol points");
					return;
				}
			}
			else{
				Debug.Log ("ATTENZIONE - problemi con i patrol points");
				return;
			}
		}

		//TODO: gestire meglio l'arrivo?
		if (Vector3.Distance (transform.position, patrolTarget.transform.position) < 1.0f) {
			//Debug.Log ("cambio target");
			if(patrolTarget != patrolPar.patrolPoints[0])
				patrolTarget = patrolPar.patrolPoints[0];
			else
				patrolTarget = patrolPar.patrolPoints[1];

		} 
		else {

			moveTowardTarget (patrolTarget, patrolSpeed);

		}




	}


	void updatePatrolPoint() {
		//TODO: fare caso stand, unico punto

	}

	//END UPDATES---------------------------------------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------------------------------------------------

	//FINALIZE---------------------------------------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------------------------------------------------
	
	protected override object finalizeState() {

		object ob;
		Debug.Log (" PATROL - ritorno foundtarget : " + foundTarget.name);

		ob = (object)foundTarget;
		return ob;

	}

	//TRANSITIONS---------------------------------------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------------------------------------------------


	private void patrolToChaseTransition(ref myStateName nextState){

		//if (nextState != state)
		//	return;

		if (Input.GetKeyUp(KeyCode.C)) {
			nextState = myStateName.Chase;
		} else {
			return;
		}
	}
	
	private void P2FcheckDanger(ref myStateName st){

		if (st != state)
			return;

		RaycastHit2D hit;


		Debug.DrawLine (new Vector2(transform.position.x, transform.position.y + 0.8f), i_facingRight () ? new Vector2 (transform.position.x + RangeOfView, transform.position.y + 0.8f) : new Vector2 (transform.position.x - RangeOfView, transform.position.y + 0.8f), Color.yellow);
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.8f) , i_facingRight()? Vector2.right : -Vector2.right, RangeOfView, fleeLayer);
		
		if (hit.collider != null) {
			
			st = myStateName.Flee;
			_fleeTarget = hit.transform.gameObject;
			return;
		}

	}


	private void P2CcheckChaseTarget(ref myStateName st){

		//if (st != state)
		//	return;

		RaycastHit2D hit;
		float obstacleDist = -1.0f;

		//controllo se ho degli OSTACOLI in mezzo...
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.0f) , i_facingRight()? Vector2.right : -Vector2.right, RangeOfView, obstacleLayers);
		if (hit.collider != null) {
			obstacleDist = Vector2.Distance( hit.point, transform.position);
			if(par.DEBUG_FSM_TRANSITION[1])
				Debug.Log ("PA -> CH - Raycast trova ostacolo : " + hit.transform.gameObject.name);
		}
		
		Debug.DrawLine (new Vector2(transform.position.x, transform.position.y + 1.0f), i_facingRight () ? new Vector2 (transform.position.x + RangeOfView, transform.position.y + 1.0f) : new Vector2 (transform.position.x - RangeOfView, transform.position.y + 1.0f), Color.red);
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.0f) , i_facingRight()? Vector2.right : -Vector2.right, RangeOfView, targetLayers);
		if (hit.collider != null) {
			
			foundTarget = hit.transform.gameObject;

			if(par.DEBUG_FSM_TRANSITION[1])
				Debug.Log ("PA -> CH - Raycast trova target : " + foundTarget.name);

			if(obstacleDist == -1.0f) {

				st = myStateName.Chase;
				return;
			}
			else {

				float targetDist = Vector2.Distance( hit.point, transform.position);
				if(targetDist < obstacleDist) {
					st = myStateName.Chase;
					return;
				}
			}
		}

		foundTarget = null;
		//non è stato rilevato nulla
		return;
		
	}

	//END TRANSITIONS---------------------------------------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------------------------------------------------
	


}


















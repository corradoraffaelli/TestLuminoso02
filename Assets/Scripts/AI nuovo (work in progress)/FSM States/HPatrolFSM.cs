//#define _DEBUG
using UnityEngine;
using System.Collections;



public class HPatrolFSM : HStateFSM {

	public enum patrolSubState {
		Walk,
		Stand,
		Area,
		AreaSuspicious,
	}
	public patrolSubState paSS;
	patrolSubState defaultPaType;
	//PatrolParameters pa;

	#region QUICKPATROLPARAMETERS

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

	#endregion QUICKPATROLPARAMETERS
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
	public HPatrolFSM(int _stateId, GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent, patrolSubState patrolT) 
	: base("Patrol", _stateId, _gameo, _hLevel, true, _fatherState, _scriptAIAgent) {

		myInitialize += patrolInitialize;

		switch (patrolT) {
			
		case patrolSubState.Walk :
			myUpdate += updatePatrolWalk;
			myHandleCollisionEnter += checkFlipNeedForCollision;
			break;
			
		case patrolSubState.Area :
			myUpdate += updatePatrolArea;
			break;
			
		case patrolSubState.Stand :
			myUpdate += updatePatrolStand;
			break;
		}
		
		myFinalize += patrolFinalize;

		myHandleCollisionEnter += checkKillPlayerCollision;

		initializePatrolParameters ();

	}

	public void setDefaultTransitions(HStunnedFSM stunState, HChaseFSM chaseState) {

			
		//addTransition (P2FcheckDanger, "Flee");
			
		//addTransition (P2CcheckChaseTarget, "Chase");
			
		//addTransition (P2ScheckStunned, "Stunned");
		addTransition (P2ScheckStunned, stunState);

		//addTransition (P2CcheckChaseTarget, "Chase");
		addTransition (P2CcheckChaseTarget, chaseState);
	}

	public void setDefaultTransitions(HStunnedFSM stunState, HChase1FSM chaseState) {

		addTransition (P2ScheckStunned, stunState);
		
		addTransition (P2CcheckChaseTarget, chaseState);
	}

	#region INITIALIZEPATROLPARAMETERS

	void initializePatrolParameters(){
		
		patrolPar = gameObject.GetComponent<AIParameters> ().patrolParameters;
		
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

	#endregion INITIALIZEPATROLPARAMETERS
	
	#region MYINITIALIZE

	protected void patrolInitialize(ref object ob) {
	#if _DEBUG
			Debug.Log ("inizio patrol--------------------------------");
	#endif

		if(ob!=null) {

			//patrolSubState patrolType = (patrolSubState

		}

		patrolTarget = null;


	}
	
	#endregion MYINITIALIZE
	
	#region MYUPDATE

	void updatePatrolWalk(){
		
		i_move (patrolSpeed);
		
	}
	
	void updatePatrolArea(){
		
		patrolBetweenPoints ();
		
	}

	void updatePatrolStand() {

		patrolOnePoint ();

	}

	void patrolOnePoint() {

		if(patrolPar.patrolPoints.Length > 0) {
			if (Vector3.Distance (transform.position, patrolPar.patrolPoints[0].transform.position) < 0.5f) {

				if((!i_facingRight() && patrolPar.DefaultVerseRight == true) ||
				   (i_facingRight() && patrolPar.DefaultVerseRight == false) )
					i_flip();

			} 
			else {
				
				moveTowardTarget (patrolPar.patrolPoints[0], patrolSpeed);
				
			}
		}
		else {

			Debug.Log("ATTENZIONE - single patrol point NOT assigned");

		}
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
		if (Vector3.Distance (transform.position, patrolTarget.transform.position) < 0.5f) {

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
	
	#endregion MYUPDATE

	#region MYFINALIZE

	protected object patrolFinalize() {
		#if _DEBUG
			Debug.Log ("finisco patrol--------------------------------");
		#endif

		object ob;

		if (foundTarget != null) {
			#if _DEBUG
				Debug.Log (" PATROL - ritorno foundtarget : " + foundTarget.name);
			#endif

		} 
		else {
			#if _DEBUG
				Debug.Log ("PATROL - ritorno NULL come foundtarget");
			#endif

		}

		ob = (object)foundTarget;
		return ob;
		
	}

	#endregion MYFINALIZE
	
	#region MYTRANSITIONS

	private bool P2FcheckDanger(){

		RaycastHit2D hit;
		
		Debug.DrawLine (new Vector2(transform.position.x, transform.position.y + 0.8f), i_facingRight () ? new Vector2 (transform.position.x + RangeOfView, transform.position.y + 0.8f) : new Vector2 (transform.position.x - RangeOfView, transform.position.y + 0.8f), Color.yellow);
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.8f) , i_facingRight()? Vector2.right : -Vector2.right, RangeOfView, fleeLayer);
		
		if (hit.collider != null) {
			
			_fleeTarget = hit.transform.gameObject;
			return true;
		}

		return false;
		
	}



	private bool P2CcheckChaseTarget(){

		RaycastHit2D hit;
		float obstacleDist = -1.0f;
		
		//controllo se ho degli OSTACOLI in mezzo...
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.0f) , i_facingRight()? Vector2.right : -Vector2.right, RangeOfView, obstacleLayers);
		if (hit.collider != null) {
			obstacleDist = Vector2.Distance( hit.point, transform.position);
			#if _DEBUG
				Debug.Log ("PA -> CH - Raycast trova ostacolo : " + hit.transform.gameObject.name);
			#endif

		}
		
		Debug.DrawLine (new Vector2(transform.position.x, transform.position.y + 1.0f), i_facingRight () ? new Vector2 (transform.position.x + RangeOfView, transform.position.y + 1.0f) : new Vector2 (transform.position.x - RangeOfView, transform.position.y + 1.0f), Color.red);
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.0f) , i_facingRight()? Vector2.right : -Vector2.right, RangeOfView, targetLayers);
		if (hit.collider != null) {
			
			foundTarget = hit.transform.gameObject;

			#if _DEBUG
				Debug.Log ("PA -> CH - Raycast trova target : " + foundTarget.name);
			#endif
			
			if(obstacleDist == -1.0f) {

				return true;
			}
			else {
				
				float targetDist = Vector2.Distance( hit.point, transform.position);
				if(targetDist < obstacleDist) {

					return true;
				}
			}
		}
		
		foundTarget = null;

		return false;
		
	}

	private bool P2ScheckStunned(){

		if (par.stunnedReceived) {

			par.stunnedReceived = false;
			return true;
		} 
		else {
			return false;
		}
	}

	#endregion MYTRANSITIONS
	

}



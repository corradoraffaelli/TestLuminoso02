using UnityEngine;
using System.Collections;
using System;


/*
 *	PATTERN USATO PER FSM E TRANSIZIONI :
 *
 *	UPDATE : switch nell'update invoca in base a stato attuale (in eMS) una funziona con _NomeStato
 *	
 *		_NOMESTATO : questo invoca la la funzione involucro, tipicamente chiamata nomestaton (n è un numero)
 *	
 *			NOMESTATON : qui c'è un pattern fisso, si dichiara un enemystatemachine, si fa un if su una funzione che testa se continuare o meno
 *			in questo stato (isStoppedNomeStato)
 *
 *				ISSTOPPEDNOMESTATO : fa dei test ad hoc per lo stato e ha come tipo di ritorno un enemystatemachine
 *
 *			ritornando alla funzione, in base all'esito della funzione appena citata si entrerà nel primo ramo dell'if che farà continuare
 *			l'azione di questo stato (continueNomeStato), se invece si dovesse entrare nell'else si invocherà la funzione makestatetransiotion
 *
 *				MAKESTATETRANSITION : questa ha 2 serie di switch che indirizzerà verso la funzione che porterà dallo stato attuale al prossimo
 *
 *					MAKESTATE1STATE2TRANSITION : qui si settano certe variabili e si ritorna, si devono richiamare due funzioni in particolare,
 *					una per l'uscita da uno stato e una per l'entrata nel nuovo stato, per il reset o set delle variabili
 *
 *	
 *	COME AGGIUNGERE UNO STATO ALLA FSM :
 *
 *	1) aggiungerlo alla enemyMachineState enum
 *	2) creare e aggiungere la relativa funzione _NomeStato() allo switch dentro l'update
 *	3) la funzione _NomeStato dovrà avere lo schema if(isStoppedNomeStato) continueNomeStato else makeStateTransition() ...
 *	4) le funzioni del punto 4 dovranno supportare il template con lo switch dell'enemyType
 *	5) aggiungere i casi agli switch della funzione makeStateTransition e quindi creare le nuove funzioni necessarie per entrare ed uscire da questo stato
 *
 * 
 *  TIPOLOGIE NEMICO :
 * 
 * 
 */

public class basicAIEnemyV4 : MonoBehaviour {

	//DEBUG VARIABLES
	bool DEBUG_PHASE_BASIC = false;
	bool DEBUG_PHASE_WEIRD = false;
	
	//l'indice dell'array indica la profondità con cui si vuole scavare
	public bool []DEBUG_FSM_TRANSITION = new bool[3];
	public bool []DEBUG_RECOGNITION = new bool[2];
	public bool []DEBUG_ASTAR = new bool[2];
	public bool DEBUG_FLIP = false;
	public bool DEBUG_COLLISION = false;

	//Gestione macchina a stati-----------------------------------------------------------------------
	public enum enemyMachineState {
		Wander,
		Patrol,
		Chase,
		Attack,
		Flee,
		Stunned,
	}
	public enemyMachineState eMS;
	
	//Gestione tipologia nemico------------------------------------------------------------------------
	public enum enemyType {
		Dumb,
		Guard,
		Heavy,
	}
	public enemyType eType;

	public enum patrolType {
		Walk,
		Stand,
		Area,
	}
	patrolType paType;

	public enum chaseType {
		ChargedCh,
		Constant,
	}
	chaseType chType;

	public enum attackType {
		Immediate,
		ChargedAt,
	}
	attackType atType;

	bool killable = false;
	public GameObject spawner;
	//LAYER MASK da usare
	//public LayerMask wallLayers;
	//public LayerMask groundBasic;
	public LayerMask targetLayers;
	public LayerMask fleeLayer;
	public LayerMask hidingLayer;
	public LayerMask cloneLayer;
	public LayerMask obstacleLayers;



	//è la distanza al di sotto della quale vuol dire che sono arrivato al punto desiderato
	//questo perché le tiles dell'astar coprono range di 0.5f ciascuna, quindi mettere
	//una distanza troppo vicina a quel valore mi farebbe avere comportamente strani

	//TODO: ricollegare in qualche modo con le tile size dello script pathfinder2D?
	float approachPatrolPoint_Distance = 1.5f;

	//bool DEBUG_FSM_TRANSITION_deepth0 = false;
	//bool DEBUG_FSM_TRANSITION_deepth1 = false;

	//Gestione status visivo
	public Sprite []statusSprites;
	GameObject statusImg;
	public GameObject pointPrefab;

	//Gestione import di oggetti vari------------------------------------------------------------------
	//controller2DV2 c2d;
	PlayerMovements pm;
	Transform groundCheckTransf;
	
	//Gestione basic ground raycast--------------------------------------------------------------------
	//public LayerMask groundBasic; dichiarato su
	
	//Gestione patrol----------------------------------------------------------------------------------
	public GameObject []patrolPoints;
	//verso di default dove puntare lo sguardo nel caso di un singolo punto di patrol
	public bool DefaultVerseRight = true;

	public GameObject[]patrolSuspiciousPoints;

	float suspiciousStartTime = 0.0f;
	float countDown_Suspicious = 7.0f;
	float suspPoint_ReachedTime = 0.0f;
	float countDown_SingleSuspPoint = 2.0f;
	float thresholdNearSuspPoint = 1.0f;
	float offset_SuspPoint = 1.5f;
	bool suspicious = false;
	
	//variabili da resettare ad inizio stato
	bool patrollingTowardAPoint = false;
	public Transform patrolledTarget;//utile dichiararlo momentaneamente public per vedere che valore ha
	bool reacheadOneSuspPoint = false;

	public float DEFAULT_DUMB_SPEED = 2.0f;
	public float WalkSpeed = 4.0f;

	//Gestione raycast target------------------------------------
	//public LayerMask targetLayers; dichiarata su
	bool backVision = true;
	float frontalDistanceOfView = 5.0f;
	float scale_FrontalDistanceOfView_ToBeFar = 1.5f;
	float backDistanceOfView = 2.0f;
	
	//Gestione chase-----------------------------------------------------------------------------------
	//Transform chasedTarget;
	//bool avoidingObstacles = false;
	Vector3 ignoreZ;//da privatizzare

	public float RunSpeed = 5.0f;

	//enemytype nojumpsoftchase
	float offset_MaxDistanceReachable_FromChase = 5.0f;
	bool chaseCharged = false;
	bool chaseCharging = false;
	public float tChargingChase = 1.0f;
	float tStartCrash = -1.0f;
	public float tToMaxVelocity = 0.5f;

	//Gestione attack----------------------------------------------------------------------------------

	//usati per basic attack... non usati con nuova implementazione
	float tLastAttack = 0.0f;
	float tBetweenAttacks = 1.0f;

	bool targetNearCheck = false;

	//non usata...
	float marginAtChThreshold = 0.06f;//ex. 0.3f

	//non usata per ora...
	bool isTargetStunned = false;

	bool attackCharged = false;
	bool attackCharging = false;
	bool attackStuck = false;

	public float tChargingAttack = 0.5f;
	public float tStuckLenght = 2.0f;
	
	//Gestione fleeing---------------------------------------------------------------------------------
	//public LayerMask fleeLayer; dichiarato su
	Transform fleeingBy;
	Transform fleeingTargetPoint;
	bool fleeingTowardAPoint = false;
	float securityDistanceFleeing = 8.0f;
	
	//Gestione stunned---------------------------------------------------------------------------------
	
	bool stunnedReceived = false;
	float tLastStunnedAttackReceived = -10.0f;
	public float tStunnedLenght = 4.0f;
	//bool autoDestroy = false;
	//sotto caso freezed----------------------

	PhysicsMaterial2D myPhysicsMat;
	GameObject myToStun;
	GameObject myWeakPoint;
	bool involveEType = false;

	//Gestione jump------------------------------------------------------------------------------------
	
	bool canJump = true;
	float thresholdHeightDifference = 1f;//ex 3... ora la rendo 5
	//float originalHDifferenceWithTarget = 0.0f;
	float tLastJump = -3.0f;
	float tBetweenJumps = 0.3f;
	//Gestione flip------------------------------------------------------------------------------------
	float tLastFlip = -0.5f;
	float tBetweenFlips = 0.2f;
	
	//GESTIONE A*
	SimpleAI2D myAstar;

	float thresholdApproachingNextTile = 0.4f;

	bool freezedByGun = false;
	
	//INIZIO FUNZIONI START---------------------------------------------------------------------------------------------------------------------
	//------------------------------------------------------------------------------------------------------------------------------------------
	
	
	// Use this for initialization
	void Start () {

		pm = GetComponent<PlayerMovements> ();
		myAstar = GetComponent<SimpleAI2D> ();

		getGroundCheck ();

		checkSuspPoints ();
		
		normalizeSpatialVariables ();
		
		takeStatusImg ();
		

		setupEnemy ();

		getMyToStun ();

		getMyWeakPoint ();

	}

	private patrolType setupPatrolPoints() {

		bool pointNotAssigned = false;

		if(patrolPoints.Length>0) {
			//serve almeno 1 punto
			
			foreach(GameObject pp in patrolPoints) {
				
				if(pp == null) {
					Debug.Log("Attento, non hai assegnato correttamente i patrol point/s");
					pointNotAssigned = true;
				}
			}
			
			if(pointNotAssigned) {
				Debug.Log ("L'AI quindi ha come default il patrol di tipo walk (wander like)");
				return patrolType.Walk;
			}

			if(patrolPoints.Length>2) {
				Debug.Log("Hai assegnato più di 2 punti di patrol, non è un caso previsto per ora");
				Debug.Log ("L'AI quindi ha come default il patrol di tipo walk (wander like)");
				return patrolType.Walk;
			}
				 

			if(patrolPoints.Length==1) {
				return patrolType.Stand;
			}
			else { 
				if(patrolPoints.Length==2) {
					return patrolType.Area;
				} 
				else {
					Debug.Log ("CASO ASSURDO NEL SETUP DI PATROL");
					Debug.Log ("L'AI quindi ha come default il patrol di tipo walk (wander like)");
					return patrolType.Walk;
				}
			}

		}
		else {

			return patrolType.Walk;

		}


	}

	private void setupEnemy(bool involution = false) {

		switch(eType) {

			case enemyType.Dumb :
				canJump = false;
				eMS = enemyMachineState.Wander;
				//pm.speedFactor = 2.0f;
				if(involution) {
					this.WalkSpeed = DEFAULT_DUMB_SPEED;
					pm.WalkSpeed = DEFAULT_DUMB_SPEED;
				}
				else {
					pm.WalkSpeed = this.WalkSpeed;
					pm.RunSpeed = this.RunSpeed;
					
				}
				
				break;
			
			case enemyType.Guard :
				canJump = false;
				eMS = enemyMachineState.Patrol;
				//pm.speedFactor = 5.0f;
				pm.WalkSpeed = this.WalkSpeed;
				pm.RunSpeed = this.RunSpeed;
				paType = setupPatrolPoints ();
				chType = chaseType.ChargedCh;
				atType = attackType.Immediate;
				break;

			case enemyType.Heavy :
				canJump = false;
				eMS = enemyMachineState.Patrol;
				//pm.speedFactor = 2.0f;
				pm.WalkSpeed = this.WalkSpeed;
				pm.RunSpeed = this.RunSpeed;
				paType = setupPatrolPoints ();
				chType = chaseType.Constant;
				atType = attackType.ChargedAt;
				break;

		}

	}
	
	
	private void getMyToStun() {

		bool founded = false;

		if (eType != enemyType.Heavy)
			return;

		foreach (Transform child in transform) {

			if(child.tag == "Stunning") {

				myToStun = child.gameObject;
				founded = true;
				break;

			}

		}

		if (!founded) {
			Debug.Log ("Oggetto ToStun mancante, attenzione");
		}

	}

	private void getMyWeakPoint() {

		bool founded = false;

		foreach (Transform child in transform) {
			
			if(child.tag == "Weakness") {
				
				myWeakPoint = child.gameObject;
				founded = true;
				break;
				
			}
			
		}

		if (!founded) {
			Debug.Log ("Oggetto WeakPoint mancante, attenzione");
		}

	}


	
	private void getGroundCheck(){
		
		foreach (Transform child in transform) {
			
			if(child.tag=="GroundCheck") {
				groundCheckTransf = child;
				break;
			}
			
		}
		
		if (groundCheckTransf == null)
			Debug.Log ("groundCheck non trovato");
		
	}
	
	private void checkSuspPoints() {
		
		if (patrolSuspiciousPoints.Length > 1) {
			
			return;
		}
		else {
			
			GameObject p = GameObject.Find ("AutoGenSuspPoints");
			
			if (p == null) {
				//Debug.Log ("creo susp points container");
				p = (GameObject) GameObject.Instantiate(pointPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
				p.name = "AutoGenSuspPoints";
			}
			
			patrolSuspiciousPoints = new GameObject[2];
			
			GameObject ob1 = (GameObject) GameObject.Instantiate(pointPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			ob1.name = "Point(clone) " + this.name + " left";
			ob1.transform.parent = p.transform;
			patrolSuspiciousPoints[0] = ob1;
			
			GameObject ob2 = (GameObject) GameObject.Instantiate(pointPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
			ob2.name = "Point(clone) " + this.name + " right";
			ob2.transform.parent = p.transform;
			patrolSuspiciousPoints[1] = ob2;
			
		}
		
	}
	

	private bool getRangeOfView(){

		GameObject range = null;

		foreach (Transform child in transform) {
			if(child.gameObject.name=="RangeOfView") {
				range = child.gameObject;
			}
		
		}
		
		if (range != null) {

			if(range.transform.localPosition.x < 0) 
				Debug.Log("Attenzione! L'empty 'RangeOfView' è in una posizione negativa");

			frontalDistanceOfView = Mathf.Abs( range.transform.localPosition.x ) * Mathf.Abs( transform.localScale.x );

			return true;

		}
		else {

			return false;

		}

	}
	
	private void normalizeSpatialVariables(){
		
		//Gestione raycast target------------------------------------


		 if(!getRangeOfView()) {
			Debug.Log ("L'empty 'RangeOfView' non è assegnato!!");
			frontalDistanceOfView = frontalDistanceOfView * Mathf.Abs(transform.localScale.x);

		}


		backDistanceOfView = backDistanceOfView * Mathf.Abs(transform.localScale.x);
		
		//Gestione jump------------------------------------------------------------------------------------
		
		thresholdHeightDifference = thresholdHeightDifference * Mathf.Abs(transform.localScale.y);
		
		//Gestione attack----------------------------------------------------------------------------------
		
		//thresholdNear = thresholdNear * Mathf.Abs(transform.localScale.x) * 0.5f;
		marginAtChThreshold = marginAtChThreshold * Mathf.Abs(transform.localScale.x);

		//Gestione fleeing---------------------------------------------------------------------------------
		
		securityDistanceFleeing = securityDistanceFleeing * Mathf.Abs(transform.localScale.x);
		
		//Gestione patrolling------------------------------------------------------------------------------
		
		thresholdNearSuspPoint = thresholdNearSuspPoint * Mathf.Abs(transform.localScale.x);
	}
	
	void takeStatusImg(){
		foreach (Transform child in transform) {
			
			if(child.name=="statusImg") {
				
				statusImg = child.gameObject;
				break;
			}
			
		}
		
	}


	//FINE FUNZIONI START-----------------------------------------------------------------------------------------------------------------------
	//------------------------------------------------------------------------------------------------------------------------------------------
	
	//INIZIO FUNZIONI UPDATE--------------------------------------------------------------------------------------------------------------------
	//------------------------------------------------------------------------------------------------------------------------------------------
	
	
	// Update is called once per frame
	void Update () {
		
		//used to be sure to not fall

		checkPriorities ();
		
		//used to evaluate what to do in case of low health
		
		switch(eMS) {

			case enemyMachineState.Wander :

				_Wandering(Time.deltaTime);
				if(DEBUG_PHASE_BASIC)
					Debug.Log("wander " + Time.time);

				break;

			case enemyMachineState.Patrol :
				
				_Patrolling(Time.deltaTime);
				//check davanti
				if(DEBUG_PHASE_BASIC)
					Debug.Log("patrol " + Time.time);
				break;
				
			case enemyMachineState.Chase :
				_Chasing(Time.deltaTime);
				if(DEBUG_PHASE_BASIC)
					Debug.Log("chasing " + Time.time);
				break;
				
			case enemyMachineState.Attack :
				
				_Attacking(Time.deltaTime);
				if(DEBUG_PHASE_BASIC)
					Debug.Log("attack " + Time.time);
				break;
				
			case enemyMachineState.Flee :
				_Fleeing(Time.deltaTime);
				break;
				
			case enemyMachineState.Stunned :
				if(DEBUG_PHASE_BASIC)
					Debug.Log("Stunned" + Time.time);
				_Stunned(Time.deltaTime);
				break;
				
				
			default :
				if(DEBUG_PHASE_BASIC)
					Debug.Log("caso strano in switch +++++++++++++++++++");
				break;
		}
	}

	void checkPriorities() {
		
		if(eType!=enemyType.Dumb) 
			checkFleeingNeed ();

		checkStunned ();
	}
	
	//METODI GESTIONE COMPORTAMENTO--------------------------------------------------------------------------------------------------------
	
	private void suspiciousUp(){
		
		suspicious = true;
	}
	
	private void suspiciousDown(){
		
		suspicious = false;
	}
	
	//METODI PRINCIPALI GESTIONE MACHINE STATE---------------------------------------------------------------------------------------------
	
	
	
	/*
	//TODO: possibile improvement
	//si potrebbero usare due array di delegati per snellire un pizzico il codice... ma ne vale la pena?
	//così mi risparmierei le funzioni _State () .... non è gran risparmio

	//se invece generalizzassi le isStoppedState e continueState??
	
	private void provaGenericUpdate(){

		GameObject nextTarget = null;
		
		enemyMachineState next = enemyMachineState.Patrol;
		
		if ((next = isStoppedState[(int)eMS] (ref nextTarget)) == eMS) {
			
			continueState[(int)eMS]();
			
		} 
		else {
			
			makeStateTransition(eMS,next,nextTarget);
			
		}
		
	}
	*/



	private void _Wandering(float deltaTime) {

		enemyMachineState next = enemyMachineState.Patrol;
		
		if ((next = isStoppedWander ()) == enemyMachineState.Wander) {
			
			continueWander();
			
		} 
		else {
			
			makeStateTransition(eMS,next);
			
		}

	}
	
	private void _Patrolling(float deltaTime) {
		
		//come regola non dovrebbe servire
		//sarebbe spreco di cpu
		
		GameObject nextTarget = null;
		
		enemyMachineState next = enemyMachineState.Patrol;
		
		if ((next = isStoppedPatrol (ref nextTarget)) == enemyMachineState.Patrol) {
			
			continuePatrol();
			
		} 
		else {
			
			makeStateTransition(eMS,next,nextTarget);
			
		}
		
		
		
	}
	
	
	private void _Chasing(float deltaTime) {
		
		enemyMachineState next = enemyMachineState.Chase;
		
		if ((next = isStoppedChase ()) == enemyMachineState.Chase ) {
			//se arrivo fin qui, allora continuo nel chase
			
			continueChase();
			
		} 
		else {
			
			makeStateTransition(eMS, next);
			
		}
		
	}
	
	
	private void _Attacking(float deltaTime) {
		
		enemyMachineState next = 0;
		
		if ((next = isStoppedAttack ()) == enemyMachineState.Attack) {
			//continuo ad attaccare
			
			continueAttack();
			
		} 
		else {
			
			makeStateTransition(eMS, next);
			
		}
		
	}
	
	
	private void _Fleeing(float deltaTime) {
		
		enemyMachineState next = enemyMachineState.Flee;
		
		if ((next = isStoppedFlee ()) == enemyMachineState.Flee) {
			//basicFleeOppositeDirection ();
			
			continueFlee ();
			
		}
		else {
			
			makeStateTransition(eMS, next);
			
		}
		
	}
	
	private void _Stunned(float deltaTime) {
		
		enemyMachineState next = enemyMachineState.Stunned;
		
		if ((next = isStoppedStunned ()) == enemyMachineState.Stunned) {
			//basicFleeOppositeDirection ();
			
			continueStunned ();
			
		}
		else {
			
			makeStateTransition(eMS, next);
			
		}
		
	}
	
	//---------------------------------------------------------------------------------------------
	
	
	private void setStatusSprite(enemyMachineState e) {
		
		if (statusImg == null)
			return;
		
		switch (e) {
			
		case enemyMachineState.Chase :
			statusImg.GetComponent<SpriteRenderer>().sprite = statusSprites[0];
			break;
			
		case enemyMachineState.Patrol :
			
			if(suspicious) {
				statusImg.GetComponent<SpriteRenderer>().sprite = statusSprites[1];
				
			}
			else {
				statusImg.GetComponent<SpriteRenderer>().sprite = null;
				
			}
			
			break;
			
		}
		
	}
	
	//GESTIONE TRANSIZIONI FSM--------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------
	
	
	private void makePaChTransition(GameObject myTarget) {
		
		eMS = enemyMachineState.Chase;
		
		initializeChase (myTarget);
		
		setStatusSprite (eMS);
		
	}
	
	private void makeChPaTransition() {
		
		eMS = enemyMachineState.Patrol;
		
		initializePatrol ();
		
		setStatusSprite (eMS);
		//Debug.Log ("ehiehi");
	}
	
	private void makeChAtTransition() {

		initializeAttack ();

		eMS = enemyMachineState.Attack;
		
		//initialize attack non serve, perché veniamo dal chase
		//quindi il target è già settato
		
	}

	private void finalizeAttack() {

		targetNearCheck = false;

	}

	private void makeAtPaTransition() {

		finalizeAttack ();

		eMS = enemyMachineState.Patrol;
		
		initializePatrol (true);
		
		setStatusSprite (eMS);
	}
	
	private void makeAtChTransition() {

		finalizeAttack ();

		eMS = enemyMachineState.Chase;
		
		//initialize chase non serve, perché veniamo dall'attack
		//quindi il target è già settato
		
	}
	
	private void finalizeAnyState() {

		finalizeAttack ();
		setTargetAStar(null);
		
	}
	
	private void makeAnyFlTransition(GameObject myTarget) {
		
		//fare finalize e switch?
		
		finalizeAnyState ();
		
		eMS = enemyMachineState.Flee;
		
		initializeFlee (myTarget);
		
	}
	
	private void makeFlPaTransition() {
		
		finalizeFlee();
		
		eMS = enemyMachineState.Patrol;
		
		initializePatrol ();
		
		setStatusSprite (eMS);
		
	}
	
	private void makeStPaTransition() {

		finalizeStunned ();

		eMS = enemyMachineState.Patrol;
		
		initializePatrol ();
		
		setStatusSprite (eMS);
	}
	
	private void makeSubPatrolStateTransition(string s) {
		
		switch (s) {
			
		case "normal" :
			suspiciousDown();
			setStatusSprite(eMS);
			break;
			
		case "suspicious" :
			suspiciousUp();
			setStatusSprite(eMS);
			break;
		default :
			break;
			
		}
		
	}
	
	public void makeAnyStTransition() {
		
		//TODO: PRIOR posibile fonte di errori,da provare
		finalizeAnyState ();
		finalizeFlee ();
		
		eMS = enemyMachineState.Stunned;
		
		initializeStunned ();

		//Debug.Log ("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
		
	}

	private void makeStWaTransition() {

		finalizeStunned ();
		
		eMS = enemyMachineState.Wander;
	
	}
	

	private void makeStateTransition(enemyMachineState actual, enemyMachineState next, GameObject target=null) {

		if (DEBUG_FSM_TRANSITION[0])
			Debug.Log (Enum.GetName(typeof(enemyMachineState),actual) + " -> " + Enum.GetName(typeof(enemyMachineState), next) + (target==null ? "." : ". Target : " + target.name) );

		switch (actual) {

			case enemyMachineState.Wander :
				switch(next) {
					
					case enemyMachineState.Chase :
						break;

					case enemyMachineState.Stunned :
						makeAnyStTransition();
						break;
						
					default :
						break;
				}

				break;

			case enemyMachineState.Patrol :
				
				switch(next) {
					
				case enemyMachineState.Chase :
					
					makePaChTransition(target);
					
					break;
					
				case enemyMachineState.Flee :
					
					makeAnyFlTransition(target);		
					
					break;
					
				case enemyMachineState.Stunned :
					
					makeAnyStTransition();
					
					break;
					
				default :
					break;
					
				}
				
				break;
				
			case enemyMachineState.Chase :
				switch(next) {
					
				case enemyMachineState.Patrol :
					
					makeChPaTransition();
					
					break;
					
				case enemyMachineState.Flee :
					
					makeAnyFlTransition(target);		
					
					break;
					
				case enemyMachineState.Attack :
					
					makeChAtTransition();
					
					break;
					
				case enemyMachineState.Stunned :
					
					makeAnyStTransition();
					
					break;
					
				default :
					break;
					
				}
				
				break;
				
			case enemyMachineState.Flee :
				
				switch(next) {
					
				case enemyMachineState.Patrol :
					
					makeFlPaTransition();
					
					break;
					
				case enemyMachineState.Stunned :
					
					makeAnyStTransition();
					
					break;
					
				default :
					break;
					
				}
				
				break;
				
			case enemyMachineState.Attack :
				switch(next) {
					
				case enemyMachineState.Patrol :
					
					makeAtPaTransition();
					
					break;
					
				case enemyMachineState.Flee :
					
					makeAnyFlTransition(target);		
					
					break;
					
				case enemyMachineState.Chase :
					
					makeAtChTransition();
					
					break;
					
				case enemyMachineState.Stunned :
					
					makeAnyStTransition();
					
					break;
					
				default :
					break;
					
				}
				
				break;
				
			case enemyMachineState.Stunned :
				switch(next) {
					
				case enemyMachineState.Patrol :
					
					makeStPaTransition();
					
					break;

				case enemyMachineState.Wander :
					
					makeStWaTransition();
					
					break;

				case enemyMachineState.Stunned :

					makeAnyStTransition();
					
					break;
						
				default :
					break;
					
				}
				
				break;
				
				
		}
		
	}
	
	
	//METODI GESTIONE STATI-----------------------------------------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------------------------------------------------

	private bool isTargetLost(){
		
		if (myAstar.Target == null ) {
			//TODO: PRIORRR perché è NULL?????
			if(DEBUG_FSM_TRANSITION[2])
				Debug.Log ("CH -> PA - target perso perché NULL");
			suspiciousUp();
			return true;
		}

		if ((targetLayers.value & 1 << myAstar.Target.layer) == 0) {
			if(DEBUG_FSM_TRANSITION[2])
				Debug.Log ("CH -> PA - target perso perché LAYER non di interesse");
			suspiciousUp();
			return true;
		}


		/*
		if (myAstar.Target.layer != 12 && myAstar.Target.layer != 14 ) {
			//Debug.Log ("SCOMPARSO");
			suspiciousUp();
			return true;
		}
		*/
		return false;
	}
	
	
	//METODI GESTIONE STATO WANDER -------------------------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------------------------------------------------
	
	private enemyMachineState isStoppedWander() {
		
		return enemyMachineState.Wander;
		
	}
	
	private void continueWander() {
		
		Debug.DrawLine (new Vector2(transform.position.x, transform.position.y + 1.0f), i_facingRight () ? new Vector2 (transform.position.x + 1.0f, transform.position.y + 1.0f) : new Vector2 (transform.position.x - 1.0f, transform.position.y + 1.0f), Color.red);
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.0f), i_facingRight()? Vector2.right : -Vector2.right, 1.0f, obstacleLayers);
		if (hit.collider != null) {
			if(DEBUG_FLIP)
				Debug.Log ("flippooooo per ostacolo in prossimità " + hit.collider.gameObject.name);
			i_flip();
			
		}
		
		i_move (true);
		
	}
	
	
	//METODI GESTIONE STATO FLEEING-----------------------------------------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------------------------------------------------
	
	
	private void initializeFlee(GameObject fleeT) {
		
		fleeingBy = fleeT.transform;
		
	}
	
	private void finalizeFlee(){
		
		fleeingTowardAPoint = false;
		setTargetAStar (null);
		fleeingTargetPoint = null;
		fleeingBy = null;
		
	}
	
	
	private enemyMachineState isStoppedFlee() {
		
		//controlli ad hoc per ogni enemyType
		switch (eType) {
		
			case enemyType.Guard :

			case enemyType.Heavy :

				
			default :
				
				break;
			
		}
		
		//controlli in comune a tutti gli enemyType
		
		if (isTargetFleeLostORFarEnough ()) {
			return enemyMachineState.Patrol;
		}
		
		return enemyMachineState.Flee;
	}
	
	private void continueFlee() {
		
		switch (eType) {

			case enemyType.Guard :

			case enemyType.Heavy :

			default :
				
				fleeTowardFarthestEscapePoint ();
				
				break;
			
		}
		
		
	}
	
	private bool isTargetFleeLostORFarEnough (){
		
		//se il target è null o il suo layer è 14
		//lo perdo
		if(fleeingBy == null) {
			return true;
		}
		
		//se target layer è diverso da 15 (toflee) o da quello del personaggio (12)
		//allora posso tornare tranquillo

		if ((fleeLayer.value & 1 << fleeingBy.gameObject.layer) == 0) {			
			return true;
		}

		/*
		if(fleeingBy.gameObject.layer != 15) {
			return true;
		}
		*/
		float dist = Vector2.Distance (groundCheckTransf.position, new Vector2 (fleeingBy.position.x, fleeingBy.position.y));
		
		if (Mathf.Abs (dist) > frontalDistanceOfView * 1.5f) {
			return true;
		}
		
		return false;
		
	}
	
	
	private void checkFleeingNeed(){
		
		//basic method to detect enemies
		
		RaycastHit2D hit = Physics2D.Raycast(transform.position, i_facingRight()? Vector2.right : -Vector2.right, frontalDistanceOfView, fleeLayer);
		if (hit.collider != null) {
			
			makeStateTransition(eMS,enemyMachineState.Flee, hit.transform.gameObject);
			
			return;
		}
		
		if (backVision) {
			//OPZIONALE PER NEMICI FORTI??
			
			hit = Physics2D.Raycast (transform.position, i_facingRight() ? -Vector2.right : Vector2.right, backDistanceOfView, fleeLayer);
			if (hit.collider != null) {
				makeStateTransition(eMS,enemyMachineState.Flee, hit.transform.gameObject);
				
				return;
			}
		}
		
		return;
		
	}
	
	private bool getEscapePoints(ref ArrayList points){
		GameObject gogc = GameObject.Find ("GameController");
		GameObject ep = null;
		//ArrayList points = new ArrayList ();
		
		foreach (Transform child in gogc.transform)
		{
			//child is your child transform
			if(child.name=="ESCAPE POINTS") {
				ep = child.gameObject;
				break;
			}
		}
		
		if(ep==null)
			return false;
		
		foreach (Transform child in ep.transform)
		{
			//child is your child transform
			if(child.tag=="escapePoint") {
				//ep = child;
				points.Add(child);
			}
		}
		
		if(points.Count>0)
			return true;
		else
			return false;
		
	}
	
	private Transform getBestEscapePoint(ref ArrayList points){
		
		float maxDist = 0;
		Transform bestPoint = null;
		
		for (int i=0; i<points.Count; i++) {
			
			Transform t = (Transform)points[i];
			
			float dist = Vector2.Distance( new Vector2(fleeingBy.position.x, fleeingBy.position.y) , new Vector2(t.position.x, t.position.y));
			
			if(dist > maxDist) {
				bestPoint = t;
				maxDist = dist;
				
			}
			
		}
		return bestPoint;
	}
	
	private void setNextFleeTargetPoint(GameObject t) {
		
		setTargetAStar (t);
		
	}
	
	private void fleeTowardFarthestEscapePoint(){
		
		if (!fleeingTowardAPoint) {
			ArrayList points = new ArrayList();
			if(getEscapePoints(ref points)) {
				fleeingTargetPoint = getBestEscapePoint(ref points);
				fleeingTowardAPoint = true;
				setNextFleeTargetPoint(fleeingTargetPoint.gameObject);
			}
			else {
				//gestire il caso in cui non si abbiano punti di escape di default
				
			}
			
		}
		else {
			
			if(Vector2.Distance(groundCheckTransf.position, fleeingBy.position) > securityDistanceFleeing) {
				
				//mi sono allontanato abbastanza da fleeingBy
				//ritorno al patrol
				
				makeStateTransition(eMS, enemyMachineState.Patrol);
				
			}
			else {
				
				float distMeAndEnd = Vector2.Distance(groundCheckTransf.position, fleeingTargetPoint.position);
				float distMeAndFleeingBy = Vector2.Distance(groundCheckTransf.position, fleeingBy.position);
				
				if(distMeAndEnd < 5.0f && distMeAndFleeingBy < securityDistanceFleeing) {
					//caso in cui sono arrivato alla fine del path e il nemico è ancora troppo vicino
					//devo quindi procedere verso un'altra direzione
					ArrayList points = new ArrayList();
					if(getEscapePoints(ref points)) {
						fleeingTargetPoint = getBestEscapePoint(ref points);
						fleeingTowardAPoint = true;
						setNextFleeTargetPoint(fleeingTargetPoint.gameObject);
					}
					
				}
				else {
					//caso in cui sono in viaggio verso il punto di fuga
					
					moveAlongAStarPathNoLimits();
				}
			}
			
			
			
		}
		
	}
	
	
	
	
	//METODI GESTIONE STATO PATROL-----------------------------------------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------------------------------------------------
	
	private void initializePatrol(bool changeDir = false){
		patrollingTowardAPoint = false;
		patrolledTarget = null;
		reacheadOneSuspPoint = false;
		setTargetAStar (null);
		
		if (suspicious) {
			reacheadOneSuspPoint = false;
		}
		/*
		if (Mathf.Abs (patrolPoints [0].transform.position.y - groundCheckTransf.transform.position.y) > thresholdHeightDifference) {

			checkPatrolPoints(true);

		}
		*/

		if (changeDir) {
			//Debug.Log("ci sono riuscito");
			if(patrolPoints.Length>0) {
				if(pm.FacingRight) {
					patrolledTarget = patrolPoints[0].transform;
					//Debug.Log("cerco sx");
				}
				else {
					//dovrei impostare il patrol point destro, ma se
					//non è assegnato, punto al primo

					if(patrolPoints.Length>1)
						patrolledTarget = patrolPoints[1].transform;
					else
						patrolledTarget = patrolPoints[0].transform;

					//Debug.Log("cerco dx");
				}

				patrollingTowardAPoint = true;
				setTargetAStar (patrolledTarget.gameObject);

			}

			i_flip();
		}
	}
	

	//basic method to detect enemies
	private enemyMachineState isStoppedPatrol(ref GameObject go){

		RaycastHit2D hit;

		//switch fra eType oppure paType inutile

		//controlli in comune a tutti gli enemyType e paType

		Debug.DrawLine (new Vector2(transform.position.x, transform.position.y + 1.0f), i_facingRight () ? new Vector2 (transform.position.x + frontalDistanceOfView, transform.position.y + 1.0f) : new Vector2 (transform.position.x - frontalDistanceOfView, transform.position.y + 1.0f), Color.red);
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.0f) , i_facingRight()? Vector2.right : -Vector2.right, frontalDistanceOfView, targetLayers);
		if (hit.collider != null) {
			go = hit.transform.gameObject;
			if(DEBUG_FSM_TRANSITION[1])
				Debug.Log ("PA -> CH - Raycast trova target : " + go.name);

			return enemyMachineState.Chase;
			
		}
		

		
		return enemyMachineState.Patrol;
		
	}

	private void continuePatrol() {

		switch (paType) {

			case patrolType.Walk :
				continueWander();
				break;
			case patrolType.Stand :
				if(suspicious) {
					patrollingSuspicious();
				}
				else {
					patrollingBetweenPoints (0, 0.5f);
				}
				break;
			case patrolType.Area :
				if(suspicious) {
					patrollingSuspicious();
				}
				else {
					patrollingBetweenPoints (0, 0.5f);
				}
				break;
			default :
				break;

		}


	}


	private void setNextPatrolTargetPoint(GameObject t) {
		
		setTargetAStar (patrolledTarget.gameObject);
		
	}
	
	//TODO : PRIOR il parametro è toglibile... c'è già il parametro globale
	private void searchNextPatrollingPoint(bool suspic = false) {
		
		//definisco prossimo patrol point
		
		if (suspic) {
			
			//CASO SOSPETTOSO
			
			if (patrolledTarget == null) {
				
				if(i_facingRight()) {
					patrolledTarget = patrolSuspiciousPoints [1].transform;
					//Debug.Log("destra");
				}
				else {
					patrolledTarget = patrolSuspiciousPoints [0].transform;
					//Debug.Log("sinistra");
				}
				
				setNextPatrolTargetPoint(patrolledTarget.gameObject);
				
			} 
			else {
				
				//scelgo alternatamente il primo e il secondo
				int firstTry = UnityEngine.Random.Range (0, patrolSuspiciousPoints.Length);
				
				
				while (true) {
					firstTry = firstTry % patrolSuspiciousPoints.Length;
					if (patrolSuspiciousPoints [firstTry].transform != patrolledTarget) {
						patrolledTarget = patrolSuspiciousPoints [firstTry].transform;
						break;
					}
					firstTry++;
					
				}


				setNextPatrolTargetPoint(patrolledTarget.gameObject);
			}
			
			
		} 
		else {
			
			//CASO NORMALE

			if (patrolledTarget == null) {

				//TODO: PRIOR rinnovare all'occorrenza punti di patrol

				patrolledTarget = patrolPoints [0].transform;
				setNextPatrolTargetPoint(patrolledTarget.gameObject);

			} else {
				
				//scelta randomica
				int firstTry = UnityEngine.Random.Range (0, patrolPoints.Length);
				
				
				while (true) {
					firstTry = firstTry % patrolPoints.Length;
					if (patrolPoints [firstTry].transform != patrolledTarget) {
						patrolledTarget = patrolPoints [firstTry].transform;
						break;
					}
					firstTry++;
					
				}
				
				setNextPatrolTargetPoint(patrolledTarget.gameObject);
			}
			
		}
		
	}
	
	private void patrollingBetweenPoints(int mode, float myScaleFactor=1) {
		
		//Debug.Log ("ciao1");


		if (patrolledTarget != null) {

			if (Mathf.Abs (patrolledTarget.transform.position.y - groundCheckTransf.position.y) > thresholdHeightDifference) {

				paType = patrolType.Walk;
			
			}
		}
		
		if (!patrollingTowardAPoint) {
			//search next point
			searchNextPatrollingPoint();
			
			patrollingTowardAPoint = true;
		}
		else {
			//go toward next point
			
			if(Vector2.Distance(groundCheckTransf.position, patrolledTarget.position) < approachPatrolPoint_Distance) {
				//if we are really near, we'll set next point to patrol
				//TODO: PRIOR one patrol point
				if(patrolPoints.Length==1) {

					if( (DefaultVerseRight && !pm.FacingRight) || (!DefaultVerseRight && pm.FacingRight) ) {

						//quando sto rallentando mi flippo nella direzione giusta, non sempre quindi
						//if( Mathf.Abs( GetComponent<Rigidbody2D>().velocity.x) < 1.0f)
							i_flip();
					}

				}
				else {

					searchNextPatrollingPoint();

				}
				//Debug.Log ("cambio dir" + Time.time);
			}
			else {
				//go toward the patrol point
				if(mode==0) {
					moveAlongAStarPathNoJump(true, myScaleFactor);
				}
				else {

					//moveAlongAStarPathNoLimits();
				}
			}
			
			
		}
		
		
	}


	private void setSuspPoints() {
		
		//Debug.Log ("SETTO SUSP POINTS!");
		patrolSuspiciousPoints [0].transform.position = new Vector3 (groundCheckTransf.position.x - (offset_SuspPoint * Mathf.Abs(transform.localScale.x)), groundCheckTransf.position.y, 0f);
		patrolSuspiciousPoints [1].transform.position = new Vector3 (groundCheckTransf.position.x + (offset_SuspPoint * Mathf.Abs(transform.localScale.x)), groundCheckTransf.position.y, 0f);
		
	}
	
	
	private void patrollingSuspicious () {
		
		if (!patrollingTowardAPoint) {
			//search next point
			setSuspPoints();
			searchNextPatrollingPoint(true);
			patrollingTowardAPoint = true;
			suspiciousStartTime = Time.time;
		}
		else {
			//go toward next point
			
			if(Time.time > suspiciousStartTime + countDown_Suspicious) {
				//se è passato abbastanza tempo, torno al normale patrol
				
				makeSubPatrolStateTransition("normal");
				
				initializePatrol();
				
			}
			else {
				//patrol attorno i punti di sospetto
				
				
				if(Vector2.Distance(groundCheckTransf.position, patrolledTarget.position) < thresholdNearSuspPoint) {
					//if we are really near, we'll set next point to patrol
					
					if(reacheadOneSuspPoint) {
						if(Time.time > suspPoint_ReachedTime + countDown_SingleSuspPoint) {
							//se sto abbastanza tempo, cambio punto
							searchNextPatrollingPoint(true);
							reacheadOneSuspPoint = false;
						}
						else {
							//fermo a fissare il vuoto
							//Debug.Log ("fermoooooo");
						}
						
					}
					else {
						reacheadOneSuspPoint = true;
						suspPoint_ReachedTime = Time.time;
					}
					
				}
				else {
					//go toward the patrol point
					moveAlongAStarPathNoLimits();
				}
				
			}
		}
		
	}
	
	
	//METODI GESTIONE STATO CHASING---------------------------------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------------------------------------------------
	
	private void initializeChase(GameObject t) {
		
		setTargetAStar (t);

		switch (chType) {

			case chaseType.ChargedCh :

				chaseCharged = false;
				chaseCharging = false;

				break;

			default :
				break;

		}
		
	}

	//TODO: PRIOR!!! implementare fuori uscita da area di patrol
	private bool isTargetOutOfPatrolArea() {
		
		if (transform.position.x + offset_MaxDistanceReachable_FromChase < patrolPoints [0].transform.position.x || transform.position.x - offset_MaxDistanceReachable_FromChase > patrolPoints [1].transform.position.x) {
			i_flip();
			return true;
		}
		else
			return false;
		
	}
	
	//basic lost of target by the abs(distance)
	private bool isTargetFar(float dist){
		if (dist > frontalDistanceOfView * scale_FrontalDistanceOfView_ToBeFar) {
			suspiciousUp();
			return true;
			
		}
		else {
			return false;
		}
		
	}
	
	//per capire quando iniziare ad attaccare
	private bool isTargetNear(float dist) {

		return targetNearCheck;
		/*
		if (dist < thresholdNear - 0.3f) {
			//Debug.Log ("PASSO ALL'ATTACCO");
			return true;
		}
		else {
			return false;
			
		}
		*/
		
	}

	private bool isTargetAtDifferentHeight() {

		//float dist = Vector2.Distance (groundCheckTransf.position, myAstar.Target.transform.position);
		float hDiff = Mathf.Abs (groundCheckTransf.position.y - myAstar.Target.transform.position.y );

		if (hDiff > 2.5f)
			return true;
		else
			return false;

	}

	private enemyMachineState isStoppedChase() {

		float dist;

		switch (chType) {

			case chaseType.ChargedCh :
				if(isTargetAtDifferentHeight()) {
					
					if(DEBUG_FSM_TRANSITION[1])
						Debug.Log ("CH -> PA - target va ad una altezza diversa dalla mia");
					
					return enemyMachineState.Patrol;
				}
				
				if(chaseCharged) {
					RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.0f), i_facingRight()? Vector2.right : -Vector2.right, 1.0f, obstacleLayers);
					if (hit.collider != null) {
						if(DEBUG_FSM_TRANSITION[1])
							Debug.Log ("urtato ostacolo!" + hit.collider.gameObject.name);
						return enemyMachineState.Stunned;
						
					}
				}

				break;

			case chaseType.Constant :
				if(isTargetAtDifferentHeight()) {
					
					if(DEBUG_FSM_TRANSITION[1])
						Debug.Log ("CH -> PA - target va ad una altezza diversa dalla mia");
					
					return enemyMachineState.Patrol;
				}
				break;

			default :
				break;

		}

		//controlli in comune a tutti
		
		//controllo se è null o ha cambiato layer...
		if (isTargetLost ()) {
			
			if(DEBUG_FSM_TRANSITION[1])
				Debug.Log ("CH -> PA - target perso");
			
			return enemyMachineState.Patrol;
		}

		dist = Vector2.Distance (groundCheckTransf.position, myAstar.Target.transform.position);
		
		//se è troppo lontano torno a patrol...
		if (isTargetFar (dist)) {
			
			if(DEBUG_FSM_TRANSITION[1])
				Debug.Log ("CH -> PA - target troppo lontano");
			
			return enemyMachineState.Patrol;
		}
		
		
		//se è vicino passo ad attack
		if (isTargetNear (dist)) {
			
			if(DEBUG_FSM_TRANSITION[1])
				Debug.Log ("CH -> AT - target è nella mia zona di attacco");
			
			return enemyMachineState.Attack;
			
		}
		
		return enemyMachineState.Chase;

	}



	//TODO: da generalizzare.. per ora è fatta ad hoc... per dare priorità a qualcosa altro che non sia il player
	private void checkChangeChaseTarget(){
		//Debug.Log ("check");
		RaycastHit2D hit;
		GameObject go = null;

		Debug.DrawLine (new Vector2(transform.position.x, transform.position.y + 1.0f), i_facingRight () ? new Vector2 (transform.position.x + frontalDistanceOfView, transform.position.y + 1.0f) : new Vector2 (transform.position.x - frontalDistanceOfView, transform.position.y + 1.0f), Color.red);
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.0f) , i_facingRight()? Vector2.right : -Vector2.right, frontalDistanceOfView, targetLayers);
		if (hit.collider != null) {
			//Debug.Log ("check2");
			go = hit.transform.gameObject;

			/*
			if(myAstar.Target.gameObject.transform.parent==null){
				Debug.Log ("if1" + myAstar.Target.gameObject.name);
				return;
			}
			*/
			//if(myAstar.Target.transform.parent.gameObject.layer
			//if ((targetLayers.value & 1 << myAstar.Target.layer) == 0) {
			if(myAstar.Target.gameObject==go) {
				//Debug.Log ("if2");
				return;
			}

			if(go.tag == "Player")
				return;

			//if(myAstar.Target.gameObject.transform.parent.gameObject.tag == "Player")
			//	return;

			setTargetAStar(go);
			//Debug.Log ("cambio fattoùùùùùùùùùùùùùùùù");

			return;
			
		}
		
	}

	private IEnumerator chargeChase() {

		chaseCharging = true;

		yield return new WaitForSeconds(tChargingChase);

		chaseCharging = false;
		chaseCharged = true;
		tStartCrash = Time.time;
	}

	//smusso anche qui l'accellerazione, più velocemente però
	private void constantCatchTarget() {

		float mydelta;
		mydelta = Time.time - tStartCrash;
		
		if (mydelta != 0) {

			//per raggiungere la massima velocità di carica
			//uso una tempistica inferiore a quella usata per il tipo di chase con la carica

			mydelta = mydelta / (tToMaxVelocity * 0.7f );
			if(mydelta>1.0f)
				mydelta = 1.0f;
			
		}
		
		moveAlongAStarPathNoJump(false, Mathf.Lerp (0.5f, 1.0f, mydelta));

	}

	private void crashTowardTarget() {

		float mydelta;
		mydelta = Time.time - tStartCrash;

		if (mydelta != 0) {

			mydelta = mydelta / tToMaxVelocity;
			if(mydelta>1.0f)
				mydelta = 1.0f;

		}

		if (mydelta < 1)
			moveAlongAStarPathNoJump (false, Mathf.Lerp (0.5f, 1.0f, mydelta));
		else
			i_move (false);
	}

	private void continueChase(){

		switch (chType) {

			case chaseType.ChargedCh :
				if(chaseCharged) {

					crashTowardTarget();
					
				}
				else {
					if(!chaseCharging) {
						StartCoroutine(chargeChase());
					}
				}

				break;

			case chaseType.Constant :
				constantCatchTarget();

				break;

			default : 

				break;

		}

	}
	
	//METODI GESTIONE STATO ATTACKING -------------------------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------------------------------------------------
	
	
	//se si allontana un po dobbiamo inseguirlo di nuovo e quindi ritornare
	//allo stato chasing
	private bool isTargetNotNear(float dist) {

		return !targetNearCheck;
		/*
		if (dist > thresholdNear) {
			return true;
			
		}
		else {
			return false;
			
		}
		*/
	}

	private void initializeAttack() {

		Rigidbody2D r = GetComponent<Rigidbody2D> ();
		r.velocity = new Vector2 (r.velocity.x * 0.2f, r.velocity.y);
		
		attackCharged = false;
		attackCharging = false;
		attackStuck = false;
	}
	
	private enemyMachineState isStoppedAttack() {

		float dist;
		//controlli ad hoc per ogni enemyType

		switch (atType) {

			case attackType.ChargedAt :

				break;

			case attackType.Immediate :

				break;

			default :

				break;

		}

		//controlli in comune a tutti

		//non esco da modalità attack se sono stuck
		if (attackStuck || attackCharging || attackCharged) {

			if(DEBUG_FSM_TRANSITION[1])
				Debug.Log("AT -> AT - variabili che non mi fanno uscire da attack");

			return enemyMachineState.Attack;

		}

		//se per qualche motivo il target scompare o viene distrutto...
		if (isTargetLost()) {
			if(DEBUG_FSM_TRANSITION[1])
				Debug.Log("AT -> PA - target perso");
			return enemyMachineState.Patrol;
		}
		
		dist = Vector2.Distance (groundCheckTransf.position, new Vector2 (myAstar.Target.transform.position.x, myAstar.Target.transform.position.y));
		
		if (isTargetNotNear(dist)) {

			if(DEBUG_FSM_TRANSITION[1])
				Debug.Log("AT -> CH - target non più vicino");

			return enemyMachineState.Chase;
			
		}
		
		return enemyMachineState.Attack;
	}

	private IEnumerator chargeAttack() {
		
		attackCharging = true;
		if (DEBUG_FSM_TRANSITION [0])
			Debug.Log ("AT start -> AT CHARGE");
		
		yield return new WaitForSeconds(tChargingAttack);

		if (DEBUG_FSM_TRANSITION [0])
			Debug.Log ("AT CHARGE -> AT charged");
		attackCharging = false;
		attackCharged = true;
	}

	private IEnumerator handleStuck() {
		
		attackStuck = true;
		if (DEBUG_FSM_TRANSITION [0])
			Debug.Log ("AT charged -> AT STUCK");
		
		yield return new WaitForSeconds(tStuckLenght);
		
		attackStuck = false;

		if (DEBUG_FSM_TRANSITION [0])
			Debug.Log ("AT STUCK -> AT end");
	}
	
	private void continueAttack(){

		switch (atType) {
			
			case attackType.ChargedAt :
				if(attackCharged || attackStuck) {
					
					if(!attackStuck) {
						heavyAttack();
					}
					else {
						if(attackStuck) {
							if(DEBUG_FSM_TRANSITION[2])
								Debug.Log ("sono in stuck");
							
						}

					}
					
					
				}
				else {
					if(!attackCharging || !attackStuck) {
						StartCoroutine(chargeAttack());
					}
				}
				break;
				
			case attackType.Immediate :
				
				break;
				
			default :
				
				break;
			
		}


	}

	private void heavyAttack() {

		if(DEBUG_FSM_TRANSITION[2])
			Debug.Log ("ATTACCO PESANTE");

		myToStun.SendMessage ("c_chargedAttack");

		attackCharged = false;
		attackCharging = false;

		StartCoroutine (handleStuck ());
	}
	//basic attack
	private void basicAttack(){
		
		if (Time.time - tLastAttack > tBetweenAttacks) {
			i_attack();
			tLastAttack = Time.time;
		}
		
	}
	
	//METODI GESTIONE STATO STUNNED -------------------------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------------------------------------------------
	
	private void checkStunned() {
		
		if (stunnedReceived) {
			stunnedReceived = false;
			makeStateTransition(eMS,enemyMachineState.Stunned);
			
		}
		
	}
	
	private void initializeStunned() {
		
		tLastStunnedAttackReceived = Time.time;

		if (freezedByGun) {
			//handleFreezing(true);
			//GetComponent<Animator>().enabled = false;
		} else {
			i_stunned (true);

			switch(eType) {

				case enemyType.Dumb :
					StartCoroutine(handleKill());
					break;
				case enemyType.Guard :
					if(involveEType) {
						eType = enemyType.Dumb;
						setupEnemy(true);
						eMS = enemyMachineState.Stunned;
					}
					break;
				case enemyType.Heavy :
					break;
				default :
					break;
			}



		}

		if (myToStun != null)
			myToStun.GetComponent<BoxCollider2D> ().enabled = false;

	}

	private void finalizeStunned() {
		
		i_stunned (false);
		//TODO: PRIOR resta così? sarebbe responsabilità del photogun
		if (freezedByGun) {
			handleFreezing(false);
			//GetComponent<Rigidbody2D> ().isKinematic = false;
			//GetComponent<Animator> ().enabled = true;
			//freezedByGun = false;
		}

		if (myToStun != null)
			myToStun.GetComponent<BoxCollider2D> ().enabled = true;

	}

	private bool isStunnedCountDownFinished() {
		
		if (Time.time - tLastStunnedAttackReceived > tStunnedLenght) {
			if(DEBUG_FSM_TRANSITION[2])
				Debug.Log ("ST -> PA/WA - countDown finito");
			return true;
		}
		else {
			if(DEBUG_FSM_TRANSITION[2])
				Debug.Log ("Stunned countdown : " + (Time.time - tLastStunnedAttackReceived));
			return false;
		}
	}
	
	private enemyMachineState isStoppedStunned(){
		
		//controlli ad hoc per ogni enemyType
		switch (eType) {

			case enemyType.Dumb :
				if (isStunnedCountDownFinished ()) {
					
					if(DEBUG_FSM_TRANSITION[1])
						Debug.Log("ST -> WA - fine countdown stun");
					
					return enemyMachineState.Wander;
					
				}
				else {
					return enemyMachineState.Stunned;
				}
				break;

			case enemyType.Guard :
				if (isStunnedCountDownFinished ()) {
					
					if(DEBUG_FSM_TRANSITION[1])
						Debug.Log("ST -> PA - fine countdown stun");
					
					return enemyMachineState.Patrol;
					
				}
				break;

			case enemyType.Heavy :
				if (isStunnedCountDownFinished ()) {
					
					if(DEBUG_FSM_TRANSITION[1])
						Debug.Log("ST -> PA - fine countdown stun");
					
					return enemyMachineState.Patrol;
					
				}

				break;

			default :
				
				break;
			
		}

		return enemyMachineState.Stunned;
		
	}
	
	private void continueStunned(){
		
		//controlli ad hoc per ogni enemyType
		switch (eType) {

			case enemyType.Dumb :

			case enemyType.Guard :

			case enemyType.Heavy :

			default :
				
				break;
			
		}
		
		//controlli in comune a tutti gli enemyType
		
		//non faccio nulla per ora...
	}

	//gestione KILL - sotto caso di stunned

	private IEnumerator handleKill() {
		
		yield return new WaitForSeconds(0.1f);
		
		Rigidbody2D r = GetComponent<Rigidbody2D> ();
		BoxCollider2D b2d = GetComponent<BoxCollider2D> ();
		CircleCollider2D c2d = GetComponent<CircleCollider2D> ();
		
		r.AddForce(new Vector2(100.0f,300.0f));
		b2d.isTrigger = true;
		c2d.isTrigger = true;
		
	}
	
	private void lastWill() {
		
		if (spawner != null) {

			spawner.SendMessage("letsSpawn");

		}
	}
	
	private IEnumerator handleDestroy(float timer) {
		
		yield return new WaitForSeconds(timer);
		Destroy (this.gameObject);
		
	}

	
	//GESTIONE INTERFACCIA CON PLAYERMOVEMENTS-------------------------------------------------------------------------------------------------------
	//GESTIONE INTERFACCIA CON CONTROLLER2D-------------------------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------------------------------------------------
	
	private void i_jump(bool onlyDouble = false){
		
		if (canJump) {
			if (Time.time - tLastJump > tBetweenJumps) {
				//c2d.AiApplyJump (onlyDouble);
				pm.c_jump();
				tLastJump = Time.time;
			}
		}
		
	}
	
	private void i_attack() {
		//c2d.giveNearAttack (0);
		//pm.c_attack();
	}

	private void i_flip() {

		if (DEBUG_FLIP) {

			Debug.Log ("Flippo - ");

		}

		if(Time.time - tLastFlip > tBetweenFlips ) {
			//c2d.Flip();
			//Debug.Log("flippo : " + Time.time);
			pm.c_flip();
			tLastFlip = Time.time;
			
			//flippo pure lo sprite per lo status
			if(statusImg!=null)
				statusImg.transform.localScale = new Vector3(-statusImg.transform.localScale.x, statusImg.transform.localScale.y, statusImg.transform.localScale.z);
			
		}
		
	}

	private void i_stunned(bool isStun) {

		pm.c_stunned (isStun);

		if (isStun == false) {

			myWeakPoint.SendMessage("c_setBouncy", true);
		}
		//gameObject.SendMessage ("c_setBouncy", true);



	}
	
	private bool i_facingRight(){
		
		//return c2d.FacingRight;
		return pm.FacingRight;
	}
	
	private void i_move(bool isPatrolSpeed = true, float scaleFactor = 1){
		
		//c2d.moveCharacterHorizontal (!i_facingRight(), i_facingRight(), scaleFactor);
		pm.c_runningManagement(!i_facingRight(), i_facingRight(), isPatrolSpeed, scaleFactor);
	}


	
	//COMUNICAZIONE CON A*-SIMPLEAI2D-PATHFINDING2D-----------
	//--------------------------------------------------------
	
	
	//setto il target del path A*
	private void setTargetAStar(GameObject myT) {
		
		if (myT == null) {
			if(DEBUG_ASTAR[0])
				Debug.Log ("ASTAR - metto il target astar a null");

			myAstar.Target = myT;
			return;
		}
		
		foreach (Transform child in myT.transform) {
			if(child.tag=="GroundCheck") {
				if(DEBUG_ASTAR[0])
					Debug.Log ("ASTAR - trovato e assegnato il groundcheck");
				myAstar.Target = child.gameObject;
				//Debug.Log ("contenuto target : " + myAstar.Target.name + " ++++++++++++++++++");
				return;
			}
			
		}
		if(DEBUG_ASTAR[0])
			Debug.Log ("ASTAR - NON trovato il groundcheck, assegno lui stesso");

		myAstar.Target = myT;
		
	}
	
	//funzione di medio livello per muoversi lungo il path dell'A*
	//VECCHIO USO : uso un numero superiore di step per quando sono in una differente altezza
	//rispetto le prossime tiles dell'A*

	//ATTUALE USO : uso un numero di step uguale a 2 per evitare comportamenti strani, cioè
	//che si flippi prima del dovuto, così lui va avanti guardando nel "futuro"
	//TODO: è una cosa da gestire meglio
	private void followPath(int step , bool isPatrolSpeed = true, float scaleFactor=1){
		
		if (myAstar.Path.Count > step) {
			
			if((myAstar.Path[0].x < myAstar.Path[step].x) && !i_facingRight()) {
				i_flip ();
			}
			
			if((myAstar.Path[0].x > myAstar.Path[step].x) && i_facingRight()) {
				i_flip ();
			}
		}
		
		
		i_move (isPatrolSpeed, scaleFactor);
		
	}
	
	//funzione di alto livello per muoversi lungo il path dell'A*
	private void moveAlongAStarPath1() {
		
		if (myAstar.Path.Count > 0)
		{
			//if we get close enough or we are closer then the indexed position, then remove the position from our path list,
			//primo OR : se le x sono più vicine di 1.5 (caso in cui devo solo salire)
			//secondo OR : se la distanza punto punto è meno di 0.2 (più restrittiva)
			//terzo OR : distanza inizio-fine è maggiore di tempDistance, dove tempDistance è la distanza fra l'AI e il target
			if ( /*(Mathf.Abs(transform.position.x - myAstar.Path[0].x)<1.5F)||*/ Vector3.Distance(transform.position, myAstar.Path[0]) < thresholdApproachingNextTile || myAstar.tempDistance < Vector3.Distance(myAstar.Path[0], myAstar.Target.transform.position)) 
			{
				myAstar.Path.RemoveAt(0);
				/*
				while(myAstar.Path.Count>0) {
					if((Mathf.Abs(transform.position.y - myAstar.Path[0].y)<5 && Mathf.Abs(transform.position.x - myAstar.Path[0].x)<1.5F))
						myAstar.Path.RemoveAt(0);
					else
						break;
				}
				*/
			}
			
			
			if(myAstar.Path.Count < 1)
				return;
			
			//First we will create a new vector ignoreing the depth (z-axiz).
			ignoreZ = new Vector3(myAstar.Path[0].x, myAstar.Path[0].y, transform.position.z);
			
			//now move towards the newly created position
			
			//transform.position = Vector3.MoveTowards(transform.position, ignoreZ, Time.deltaTime * Speed);
			
			
		}
		else {
			//path finito
			return;
		}
		
		//può dare problemi toglierlo??
		/*
		if(ignoreZ == new Vector3(0,0,0))
			return;
		*/
		
		
		if (Mathf.Abs (ignoreZ.y - transform.position.y) > 1f) {
			
			//target ad un livello diverso
			
			
			if(myAstar.Path.Count>1) {
				
				//livello superiore
				if(myAstar.Path[1].y > transform.position.y + 0.5f) {
					Debug.Log ("+++^^^^^^^^^^^^");
					i_jump(true);
					return;
				}
				
				//livello inferiore
				if(myAstar.Path[1].y < transform.position.y) {
					//Debug.Log ("+++____________");
					followPath(5);
					return;
				}
			}
			
			//handleChasingAtDifferentHeightLevel1(ignoreZ);
		} 
		else {
			
			//target allo stesso livello
			
			//TODO: giocare con questo parametro qui sotto..
			
			
			if(myAstar.Path.Count>1) {
				if(myAstar.Path[1].y > transform.position.y + 1.5f) {
					i_jump(true);
					Debug.Log ("----^^^^^^^^^^");
					return;
				}
			}
			
			//fase di discesa
			if(myAstar.Path.Count>1) {
				if(myAstar.Path[1].y  < transform.position.y - 0.5f) {
					//i_jump(true);
					//Debug.Log ("----___________");
					return;
				}
			}
			
			//Debug.Log ("----------");
			followPath(1);
			
		}
		
	}
	
	//funzione di alto livello per muoversi lungo il path dell'A*
	private void moveAlongAStarPathNoLimits() {
		
		if (myAstar.Path.Count > 0)
		{
			//if we get close enough or we are closer then the indexed position, then remove the position from our path list,
			//primo OR : se le x sono più vicine di 1.5 (caso in cui devo solo salire)
			//secondo OR : se la distanza punto punto è meno di 0.2 (più restrittiva)
			//terzo OR : distanza inizio-fine è maggiore di tempDistance, dove tempDistance è la distanza fra l'AI e il target
			if ( /*(Mathf.Abs(  groundCheckTransf.position.x - myAstar.Path[0].x)<1.5F)|| */ Vector3.Distance(groundCheckTransf.position, myAstar.Path[0]) < thresholdApproachingNextTile || myAstar.tempDistance < Vector3.Distance(myAstar.Path[0], myAstar.Target.transform.position)) 
			{
				myAstar.Path.RemoveAt(0);
				/*
				while(myAstar.Path.Count>1) {
					if((Mathf.Abs(transform.position.y - myAstar.Path[0].y)<5 && Mathf.Abs(transform.position.x - myAstar.Path[0].x)<1.5F))
						myAstar.Path.RemoveAt(0);
					else
						break;
				}
				*/
			}
			
			
			if(myAstar.Path.Count < 1)
				return;
			
			//First we will create a new vector ignoreing the depth (z-axiz).
			ignoreZ = new Vector3(myAstar.Path[0].x, myAstar.Path[0].y, groundCheckTransf.position.z);
			
			//now move towards the newly created position
			
			//transform.position = Vector3.MoveTowards(transform.position, ignoreZ, Time.deltaTime * Speed);
			
			
		}
		else {
			//path finito
			return;
		}
		
		//può dare problemi toglierlo??
		/*
		if(ignoreZ == new Vector3(0,0,0))
			return;
		*/
		
		
		if (Mathf.Abs (ignoreZ.y - groundCheckTransf.position.y) > 1f) {
			
			//target ad un livello diverso
			
			
			if(myAstar.Path.Count>1) {
				
				//livello superiore
				if(myAstar.Path[1].y > groundCheckTransf.position.y + 0.5f) {
					//Debug.Log ("+++^^^^^^^^^^^^");
					i_jump(true);
					return;
				}
				
				//livello inferiore
				if(myAstar.Path[1].y < groundCheckTransf.position.y) {
					//Debug.Log ("+++____________");
					followPath(5);
					return;
				}
			}
			
			//handleChasingAtDifferentHeightLevel1(ignoreZ);
		} 
		else {
			
			//target allo stesso livello
			
			//TODO: giocare con questo parametro qui sotto..
			
			
			if(myAstar.Path.Count>1) {
				if(myAstar.Path[1].y > groundCheckTransf.position.y + 1.5f) {
					i_jump(true);
					//Debug.Log ("----^^^^^^^^^^");
					return;
				}
			}
			
			//fase di discesa
			if(myAstar.Path.Count>1) {
				if(myAstar.Path[1].y  < groundCheckTransf.position.y - 0.5f) {
					//i_jump(true);
					//Debug.Log ("----___________");
					return;
				}
			}
			
			//Debug.Log ("----------");
			followPath(1);
			
		}
		
	}
	
	
	private void moveAlongAStarPathNoJump(bool isPatrolSpeed = true, float m_scaleFactor=1) {
		
		if (myAstar.Path.Count > 0)
		{
			//TODO: verificare condizioni OR dell'if, quali siano quelle essenziali
			
			//if we get close enough or we are closer then the indexed position, then remove the position from our path list,
			//primo OR : se le x sono più vicine di 1.5 (caso in cui devo solo salire)
			//secondo OR : se la distanza punto punto è meno di 0.2 (più restrittiva)
			//terzo OR : distanza inizio-fine è maggiore di tempDistance, dove tempDistance è la distanza fra l'AI e il target
			if ( /*(Mathf.Abs(  groundCheckTransf.position.x - myAstar.Path[0].x)<1.5F)|| */Vector3.Distance(groundCheckTransf.position, myAstar.Path[0]) < thresholdApproachingNextTile || myAstar.tempDistance < Vector3.Distance(myAstar.Path[0], myAstar.Target.transform.position)) 
			{
				myAstar.Path.RemoveAt(0);
				
				//se i prossimi passi sono in verticale, li salto, perché mi posso muovere solo in orizzontale
				while(myAstar.Path.Count>1) {
					if(myAstar.Path[0].x == myAstar.Path[1].x)
						myAstar.Path.RemoveAt(0);
					else
						break;
				}
				
			}
			
			
			if(myAstar.Path.Count < 1)
				return;
			
		}
		else {
			//path finito
			return;
		}
		
		if (Mathf.Abs (myAstar.Target.transform.position.y - groundCheckTransf.position.y) > 1.0f) {
			
			if(Mathf.Abs (myAstar.Target.transform.position.x - groundCheckTransf.position.x) > 1.0f) {

				if(DEBUG_ASTAR[1])
					Debug.Log ("ASTAR - target più alto di me e distante");

				i_move(isPatrolSpeed, m_scaleFactor* 0.85f);
			}
			else {
				
				if(DEBUG_ASTAR[1])
					Debug.Log ("ASTAR - target più alto di me e sopra di me");

				i_move(isPatrolSpeed, m_scaleFactor * 0.7f);
			}
			
		}
		else {

			if(DEBUG_ASTAR[1])
				Debug.Log ("ASTAR - target alla mia stessa altezza");

			followPath(2, isPatrolSpeed, m_scaleFactor);
		}
		
		
		
	}

	//FUNZIONI VARIE--------------------------------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------


	private int convertBinToDec(int binintval) {
		
		switch (binintval) {
			
		case 256 :
			return 8;
			break;
			
		case 512 :
			return 9;
			break;
			
		case 1024 :
			return 10;
			break;
			
		case 2048 :
			return 11;
			break;
			
		case 4096 :
			return 12;
			break;
			
		case 8192 :
			return 13;
			break;
			
		case 16384 :
			return 14;
			break;
			
		case 32768 :
			return 15;				
			break;
			
		case 65536 :
			return 16;
			break;
			
		case 131072 :
			return 17;
			break;

		case 262144 :
			return 18;
			break;

		case 524288 :
			return 19;
			break;

		case 1048576 :
			return 20;
			break;

		default :
				break;
				
		}
		return 0;
	}
	
	
	//FUNZIONI PUBBLICHE INVOCATE DALL'ESTERNO------------------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------

	public void setStunned(bool st) {
		//Debug.Log ("ahi");
		this.stunnedReceived = st;

		if (eType == enemyType.Guard)
			this.involveEType = true;
	}
	/*
	public void setAutoDestroy(bool a) {
		Debug.Log ("autoDestroy set");
		if (a) {
			Debug.Log ("aaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
			makeAnyStTransition();
			this.autoDestroy = a;
		} 
		else {
			this.autoDestroy = a;
		}
	}
	*/

	private void handleFreezing(bool start){

		if (start) {
			GetComponent<Animator> ().enabled = false;
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (0.0f, 0.0f);
			gameObject.GetComponent<Rigidbody2D> ().isKinematic = true;
			myPhysicsMat = GetComponent<BoxCollider2D>().sharedMaterial;
			GetComponent<BoxCollider2D>().sharedMaterial = GetComponent<CircleCollider2D>().sharedMaterial;
			GetComponent<BoxCollider2D>().enabled = false;
			GetComponent<BoxCollider2D>().enabled = true;
			freezedByGun = true;
		}
		else {
			GetComponent<Rigidbody2D> ().isKinematic = false;
			GetComponent<Animator> ().enabled = true;
			GetComponent<BoxCollider2D>().sharedMaterial = myPhysicsMat;
			GetComponent<BoxCollider2D>().enabled = false;
			GetComponent<BoxCollider2D>().enabled = true;
			freezedByGun = false;
		}

	}



	private void handleFreezingClone() {

		gameObject.layer = convertBinToDec(cloneLayer.value);

		//Debug.Log ("valore " + cloneLayer.value + " " + convertBinToDec(cloneLayer.value));
		
		foreach(Transform child in transform) {
			
			child.gameObject.SetActive(false);
		}
		
		GetComponent<Animator>().enabled = false;
		
		GetComponent<Rigidbody2D> ().velocity = new Vector2 (0.0f, 0.0f);
		GetComponent<Rigidbody2D> ().isKinematic = true;

		//gameObject.AddComponent<autoDestroy> ();

		StartCoroutine(handleDestroy(4.0f));

		//GetComponent<autoDestroy>().enabled = true;

		GetComponent<BoxCollider2D>().sharedMaterial = GetComponent<CircleCollider2D>().sharedMaterial;
		GetComponent<BoxCollider2D>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = true;
		
		GetComponent<basicAIEnemyV4>().enabled = false;

	}
	
	public void c_freeze_ai(bool clone = false) {
		
		if (!clone) {
			//Debug.Log ("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");

			//freezedByGun = true;
			handleFreezing(true);
			makeAnyStTransition ();

			//TODO: PRIOR
			//makeStateTransition(eMS,enemyMachineState.Stunned);

		} else {

			handleFreezingClone();
		
		}


		
	}

	public void c_playerStunned(bool a) {

		if (a) {
			//Debug.Log ("colpito");

			isTargetStunned = true;

		} else {
			//non usato
			isTargetStunned = false;

		}

	}

	public void c_targetNear(bool n) {

		if (DEBUG_FSM_TRANSITION [2]) {
			if(n)
				Debug.Log ("Ricevuto segnale per ENTRARE ad AT");
			else
				Debug.Log ("Ricevuto segnale per USCIRE da AT");
		}
		this.targetNearCheck = n;

	}

	public void c_startAutoDestroy(float timer) {

		lastWill ();

		StartCoroutine(handleDestroy(timer));

	}

	public void OnCollisionEnter2D(Collision2D c) {

		//se l'AI è stunnata non reca danno al player, per ora...
		if (eMS == enemyMachineState.Stunned)
			return;

		if(DEBUG_COLLISION)
			Debug.Log ("TOCCATO " + c.gameObject.name);

		if (c.gameObject.tag == "Player") {
			c.gameObject.transform.SendMessage ("c_stunned", true);
			Vector2 dist = c.gameObject.transform.position - transform.position;
			
			Rigidbody2D r = c.gameObject.GetComponent<Rigidbody2D>();
			r.velocity = new Vector2(0.0f, 0.0f);
			r.AddForce(300.0f*dist.normalized);
			//c_playerStunned(true);
		}


	}
	
}

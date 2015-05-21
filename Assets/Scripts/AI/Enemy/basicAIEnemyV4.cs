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
	public bool []DEBUG_FLIP = new bool[2];
	public bool []DEBUG_COLLISION = new bool[3];

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

	public enum patrolSubState {
		Walk,
		Stand,
		Area,
		AreaSuspicious,
	}
	patrolSubState paSS;
	patrolSubState defaultPaType;

	public enum chaseSubState {
		ChargingCh,//TODO : now impostare suo uso - cambiare setup e continuechase e stopchase...
		ChargedCh,
		Constant,
	}
	chaseSubState chSS;

	public enum attackSubState {
		Immediate,
		ChargingAt,
		Stuck,
	}
	attackSubState atSS;
	attackSubState defaultAtType;

	public enum fleeSubState {
		Surprise,
		Escape,
	}
	fleeSubState flSS;

	bool killable = false;

	public GameObject Spawner;

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
	Rigidbody2D myRigidBody;
	
	//Gestione basic ground raycast--------------------------------------------------------------------
	//public LayerMask groundBasic; dichiarato su
	
	//Gestione patrol----------------------------------------------------------------------------------
	public GameObject []patrolPoints;
	//verso di default dove puntare lo sguardo nel caso di un singolo punto di patrol
	public bool DefaultVerseRight = true;

	//nuova gestione suspicious
	bool firstCheckDone_Suspicious = false;
	[Range(0.1f,10.0f)]
	public float tSearchLenght = 2.5f;
	bool standingSusp = false;
	bool exitSuspicious = false;

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
	
	//Gestione chase-----------------------------------------------------------------------------------
	//Transform chasedTarget;
	//bool avoidingObstacles = false;
	Vector3 ignoreZ;//da privatizzare
	[Range(0.1f,5.0f)]
	public float fTargetFar = 2.0f;
	
	//enemytype nojumpsoftchase
	float offset_MaxDistanceReachable_FromChase = 5.0f;
	bool chaseCharged = false;
	bool chaseCharging = false;
	[Range(0.1f,5.0f)]
	public float tChargingChase = 1.0f;
	float tStartCrash = -1.0f;
	[Range(0.1f,5.0f)]
	public float tToMaxVelocity = 0.5f;
	[Range(0.1f,5.0f)]
	public float tLosingTargerLenght = 5.5f;
	bool losingTarget = false;
	float tStartLosingTarget = -3.0f;

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

	[Range(0.1f,5.0f)]
	public float tChargingAttack = 0.5f;
	[Range(0.1f,5.0f)]
	public float tStuckLenght = 2.0f;
	
	//Gestione fleeing---------------------------------------------------------------------------------
	//public LayerMask fleeLayer; dichiarato su
	Transform fleeingBy;
	SpriteRenderer fleeingBySR;
	Transform fleeingTargetPoint;
	float securityDistanceFleeing = 8.0f;
	bool surprising = false;
	bool readyToEscape = false;
	[Range(0.1f,5.0f)]
	public float tSurpriceLenght = 2.5f;
	[Range(0.1f,10.0f)]
	public float tEscapeLenght = 4.0f;
	float tStartEscape = -10.0f;

	bool notImpressionedByFleeingByTarget = false;
	GameObject notImpressioningFleeingBy;
	
	//Gestione stunned---------------------------------------------------------------------------------
	
	bool stunnedReceived = false;
	float tLastStunnedAttackReceived = -10.0f;
	[Range(0.1f,5.0f)]
	public float tStunnedLenght = 4.0f;
	//bool autoDestroy = false;
	//sotto caso freezed----------------------

	PhysicsMaterial2D myPhysicsMat;
	GameObject myToStun;
	GameObject myWeakPoint;
	GameObject myHelmet;
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

	//Gestione collision
	bool handlingCollision = false;
	GameObject actualGround;

	//Gestione target (indipendente da Astar)
	GameObject myTarget;

	//INIZIO FUNZIONI START---------------------------------------------------------------------------------------------------------------------
	//------------------------------------------------------------------------------------------------------------------------------------------
	
	
	// Use this for initialization
	void Start () {

		pm = GetComponent<PlayerMovements> ();
		myAstar = GetComponent<SimpleAI2D> ();
		myRigidBody = GetComponent<Rigidbody2D> ();

		getGroundCheck ();

		normalizeSpatialVariables ();
		
		takeStatusImg ();

		setupEnemy ();

		getMyToStun ();

		getMyWeakPoint ();

		if(eType == enemyType.Heavy)
			getMyHelmet ();

	}



	private patrolSubState setupPatrolPoints() {

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
				return patrolSubState.Walk;
			}

			if(patrolPoints.Length>2) {
				Debug.Log("Hai assegnato più di 2 punti di patrol, non è un caso previsto per ora");
				Debug.Log ("L'AI quindi ha come default il patrol di tipo walk (wander like)");
				return patrolSubState.Walk;
			}
				 

			if(patrolPoints.Length==1) {
				return patrolSubState.Stand;
			}
			else { 
				if(patrolPoints.Length==2) {
					return patrolSubState.Area;
				} 
				else {
					Debug.Log ("CASO ASSURDO NEL SETUP DI PATROL");
					Debug.Log ("L'AI quindi ha come default il patrol di tipo walk (wander like)");
					return patrolSubState.Walk;
				}
			}

		}
		else {

			return patrolSubState.Walk;

		}


	}

	private void setupEnemy(bool involution = false) {

		switch(eType) {

			case enemyType.Dumb :
				canJump = false;
				eMS = enemyMachineState.Wander;
				//pm.speedFactor = 2.0f;
				if(involution) {
					pm.AI_walkSpeed = DEFAULT_DUMB_SPEED;
				}
				
				break;
			
			case enemyType.Guard :
				canJump = false;
				eMS = enemyMachineState.Patrol;
				//pm.speedFactor = 5.0f;
				paSS = setupPatrolPoints ();
				defaultPaType = paSS;
				chSS = chaseSubState.ChargedCh;
				atSS = attackSubState.Immediate;
				flSS = fleeSubState.Surprise;
				break;

			case enemyType.Heavy :
				canJump = false;
				eMS = enemyMachineState.Patrol;
				//pm.speedFactor = 2.0f;
				paSS = setupPatrolPoints ();
				defaultPaType = paSS;
				chSS = chaseSubState.Constant;
				atSS = attackSubState.ChargingAt;
				flSS = fleeSubState.Surprise;
				break;

		}

	}
	


	private void getMyHelmet () {
		
		bool founded = false;
		
		if (eType != enemyType.Heavy)
			return;
		
		foreach (Transform child in transform) {
			
			if(child.name == "Helmet") {
				
				myHelmet = child.gameObject;
				founded = true;
				break;
				
			}
			
		}
		
		if (!founded) {
			Debug.Log ("Oggetto elmetto mancante, attenzione");
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
			if(eType!=enemyType.Dumb)
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
		
	}
	
	void takeStatusImg(){

		bool founded = false;

		foreach (Transform child in transform) {
			
			if(child.name=="StatusImg") {
				
				statusImg = child.gameObject;
				if(transform.localScale.x < 0.0f)
					statusImg.transform.localScale = new Vector3(-statusImg.transform.localScale.x, statusImg.transform.localScale.y, statusImg.transform.localScale.z);
				founded = true;
				break;
			}

		}

		if (!founded) {
			if(eType != enemyType.Dumb)
				Debug.Log ("ATTENZIONE - game object figlio per status sprite assente");
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
		
		if(eType!=enemyType.Dumb && eMS != enemyMachineState.Flee) 
			checkFleeingNeed ();

		checkStunned ();
	}
	
	//METODI GESTIONE COMPORTAMENTO--------------------------------------------------------------------------------------------------------
	/*
	private void suspiciousUp(){
		//Debug.Log ("chiamato sono stato");
		paSS = patrolSubState.AreaSuspicious;

	}
	
	private void suspiciousDown(){
		
		paSS = setupPatrolPoints ();
	}
	*/
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


	private void setStatusSprite(string statusType = " ") {

		if (statusImg == null)
			return;

		switch (statusType) {

			case "allert" :
				
				statusImg.GetComponent<SpriteRenderer>().sprite = statusSprites[0];
				
				break;

			case "suspicious" :
				
				statusImg.GetComponent<SpriteRenderer>().sprite = statusSprites[1];

				break;



			case "stuck" :

				statusImg.GetComponent<SpriteRenderer>().sprite = statusSprites[2];

				break;

			case " " :

				statusImg.GetComponent<SpriteRenderer>().sprite = null;
				
				break;

			default :

				Debug.Log ("STRINGA INCORRETTA PASSATA A SETSTATUSSPRITE");

				break;

		}

	}
	
	//GESTIONE TRANSIZIONI FSM--------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------
	
	
	private void makePaChTransition(GameObject myTarget) {

		finalizePatrol ();

		eMS = enemyMachineState.Chase;
		
		initializeChase (myTarget);
		
		setStatusSprite ("allert");
		
	}
	
	private void makeChPaTransition() {

		finalizeChase ();

		eMS = enemyMachineState.Patrol;
		
		initializePatrol ();

		if (paSS == patrolSubState.AreaSuspicious)
			setStatusSprite ("suspicious");
		else
			setStatusSprite ();
	}
	
	private void makeChAtTransition() {

		finalizeChase ();

		eMS = enemyMachineState.Attack;

		initializeAttack ();


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
		
		if (paSS == patrolSubState.AreaSuspicious)
			setStatusSprite ("suspicious");
		else
			setStatusSprite ();
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
		finalizePatrol ();
		finalizeChase ();
		
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
		
		if (paSS == patrolSubState.AreaSuspicious)
			setStatusSprite ("suspicious");
		else
			setStatusSprite ();
		
	}

	private void makeFlChTransition(GameObject target) {

		finalizeFlee();

		eMS = enemyMachineState.Chase;

		initializeChase (target);

		setStatusSprite ("allert");

	}
	
	private void makeStPaTransition() {

		finalizeStunned ();

		eMS = enemyMachineState.Patrol;
		
		initializePatrol ();

		if (eType == enemyType.Heavy)
			paSS = patrolSubState.AreaSuspicious;

		if (paSS == patrolSubState.AreaSuspicious)
			setStatusSprite ("suspicious");
		else
			setStatusSprite ();
	}
	
	private void makeSubStateTransition(patrolSubState pt) {

		paSS = pt;

		if (pt == patrolSubState.AreaSuspicious)
			setStatusSprite ("suspicious");
		else
			setStatusSprite ();
		
	}

	private void makeSubStateTransition(fleeSubState fs) {

		flSS = fs;

		switch (flSS) {

			case fleeSubState.Surprise :
				break;

			case fleeSubState.Escape :
				setStatusSprite ();
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

				case enemyMachineState.Chase :
					
					makeFlChTransition(target);
					
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
		
		if (myTarget == null ) {
			//TODO: PRIORRR perché è NULL?????
			if(DEBUG_FSM_TRANSITION[2])
				Debug.Log ("CH -> PA - target perso perché NULL");
			//suspiciousUp();
			makeSubStateTransition(patrolSubState.AreaSuspicious);
			return true;
		}

		//TODO: serve fare il check sul cambio di layer?? ormai il layer non cambia più...
		//al massimo posso fare check su spegnimento della proiezione
		if ((targetLayers.value & 1 << myTarget.layer) == 0 && !notImpressionedByFleeingByTarget) {
			if(DEBUG_FSM_TRANSITION[2])
				Debug.Log ("CH -> PA - target perso perché LAYER non di interesse");
			//suspiciousUp();
			makeSubStateTransition(patrolSubState.AreaSuspicious);
			return true;
		}

		return false;
	}
	
	
	//METODI GESTIONE STATO WANDER -------------------------------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------------------------------------------------------
	
	private enemyMachineState isStoppedWander() {
		
		return enemyMachineState.Wander;
		
	}

	private void checkForObstacle() {

		Debug.DrawLine (new Vector2(transform.position.x, transform.position.y + 1.0f), i_facingRight () ? new Vector2 (transform.position.x + 1.0f, transform.position.y + 1.0f) : new Vector2 (transform.position.x - 1.0f, transform.position.y + 1.0f), Color.red);
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.0f), i_facingRight()? Vector2.right : -Vector2.right, 1.0f, obstacleLayers);
		if (hit.collider != null) {
			if(DEBUG_FLIP[1])
				Debug.Log ("flippooooo per ostacolo in prossimità " + hit.collider.gameObject.name);
			i_flip();
			
		}

	}
	
	private void continueWander() {

		//checkForObstacle ();
		
		i_move (true);
		
	}
	
	
	//METODI GESTIONE STATO FLEEING-----------------------------------------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------------------------------------------------
	
	
	private void initializeFlee(GameObject fleeT) {
		
		fleeingBy = fleeT.transform;
		fleeingBySR = fleeingBy.GetComponent<SpriteRenderer> ();
		readyToEscape = false;
		surprising = false;
	}
	
	private void finalizeFlee(bool resetEverything = true){

		if (resetEverything) {
			fleeingBy = null;
			fleeingBySR = null;

		}
		fleeingTargetPoint = null;
		flSS = fleeSubState.Surprise;
		
	}

	private bool checkEscapeCountDownAndFarEnough() {

		if (Time.time < tStartEscape + tEscapeLenght)
			return false;

		if (fleeingBy == null) {
			if(DEBUG_FSM_TRANSITION[2])
				Debug.Log ("FL -> PA - tempo di fuga esaurito e proiezione è null");
			return true;
		}

		if (!fleeingBySR.isVisible) {
			if(DEBUG_FSM_TRANSITION[2])
				Debug.Log ("FL -> PA - tempo di fuga esaurito e proiezione è sparita");
			return true;
		}

		float dist = Vector2.Distance (groundCheckTransf.position, new Vector2 (fleeingBy.position.x, fleeingBy.position.y));
		
		if (Mathf.Abs (dist) > frontalDistanceOfView * 2.0f) {
			if(DEBUG_FSM_TRANSITION[2])
				Debug.Log ("FL -> PA - tempo di fuga esaurito e abbastanza lontano dalla proiezione");
			
			return true;
		}
		else {
			return false;
		}
				
	}
	
	private enemyMachineState isStoppedFlee() {
		
		//controlli ad hoc per ogni enemyType
		switch (flSS) {
		
			case fleeSubState.Surprise :
				if (isTargetFleeLostORFarEnough ()) {
					if(DEBUG_FSM_TRANSITION[1])
					Debug.Log ("FL -> PA - scomparso prima che io fugga via");
					makeSubStateTransition(patrolSubState.AreaSuspicious);
					return enemyMachineState.Patrol;
				}
				break;
			case fleeSubState.Escape :
				if(checkEscapeCountDownAndFarEnough()){
					if(DEBUG_FSM_TRANSITION[1])
						Debug.Log ("FL -> PA - finito il tempo di escape");

					return enemyMachineState.Patrol;
				}
				break;
			default :
				
				break;
			
		}
		
		return enemyMachineState.Flee;
	}

	private IEnumerator destroyHelmet() {
		
		yield return new WaitForSeconds(5.0f);

		Destroy (myHelmet);
	}

	private IEnumerator chargeToEscape() {

		float factor = 1.0f;

		if (transform.position.x <= fleeingBy.transform.position.x)
			factor = -1.0f;
		
		myRigidBody.AddForce (new Vector2 (120.0f*factor, 150.0f));

		if (myHelmet != null && eType == enemyType.Heavy) {
			myHelmet.transform.parent = null;
			Rigidbody2D rh = myHelmet.GetComponent<Rigidbody2D> ();
			rh.isKinematic = false;
			rh.mass = 1;
			rh.AddForce (new Vector2 (150.0f * factor, 200.0f));
			killable = true;
			StartCoroutine (destroyHelmet ());
		}
		yield return new WaitForSeconds(tSurpriceLenght);

		readyToEscape = true;
	}

	private void continueFlee() {

		switch (flSS) {
			
			case fleeSubState.Surprise :

				if(!surprising) {
					StartCoroutine( chargeToEscape());
					surprising = true;
				}
				else {
					if(readyToEscape) {
						tStartEscape = Time.time;
						if(eType == enemyType.Heavy) {

							notImpressionedByFleeingByTarget = true;
							makeStateTransition(eMS,enemyMachineState.Chase,fleeingBy.gameObject);
							return;
							
						}
						else {

							makeSubStateTransition(fleeSubState.Escape);
							return;

						}
						
					}
				}

				break;
			case fleeSubState.Escape :

				goOppositeDirectionOfTargetFlee ();

				break;
			default :
				break;
			
		}

		
	}

	private void continueFlee1() {
		
		switch (eType) {

			case enemyType.Guard :

			case enemyType.Heavy :

			default :
				
				//fleeTowardFarthestEscapePoint ();
				
				break;
			
		}


		goOppositeDirectionOfTargetFlee ();

		
	}

	private void goOppositeDirectionOfTargetFlee() {

		if (fleeingBy == null) {
			i_move (false);
			return;
		}

		if (!fleeingBySR.isVisible ) {
			//se la proiezione è scomparsa, io corro e basta, non faccio caso se devo flipparmi o meno
			//perché 'ricordo' la sua ultima posizione
			i_move (false);
			return;
		}

		if (transform.position.x >= fleeingBy.transform.position.x) {
			if(!i_facingRight()) {
				if(DEBUG_FLIP[1])
					Debug.Log("flippo perché io sono a destra di " + fleeingBy.name);

				i_flip();
			}

		}
		else {

			if(i_facingRight()) {

				if(DEBUG_FLIP[1])
					Debug.Log("flippo perché io sono a destra di " + fleeingBy.name);
				
				i_flip();
			}

		}

		i_move (false);

	}

	private bool isTargetFleeLostORFarEnough (){
		
		//se il target è null o il suo layer è 14
		//lo perdo
		if(fleeingBy == null) {
			if(DEBUG_FSM_TRANSITION[2])
				Debug.Log ("FL -> PA - perché ciò che mi spaventava è null");

			return true;
		}
		
		//se target layer è diverso da 15 (toflee) o da quello del personaggio (12)
		//allora posso tornare tranquillo

		if ((fleeLayer.value & 1 << fleeingBy.gameObject.layer) == 0) {
			if(DEBUG_FSM_TRANSITION[2])
				Debug.Log ("FL -> PA - perché ciò che mi spaventava ha cambiato layer");

			return true;
		}

		/*
		if(fleeingBy.gameObject.layer != 15) {
			return true;
		}
		*/
		float dist = Vector2.Distance (groundCheckTransf.position, new Vector2 (fleeingBy.position.x, fleeingBy.position.y));
		
		if (Mathf.Abs (dist) > frontalDistanceOfView * 2.0f) {
			if(DEBUG_FSM_TRANSITION[2])
				Debug.Log ("FL -> PA - perché ciò che mi spaventava è lontano adesso");

			return true;
		}
		
		return false;
		
	}
	
	
	private void checkFleeingNeed(){

		if (notImpressionedByFleeingByTarget)
			return;

		//basic method to detect enemies
		RaycastHit2D hit;

		Debug.DrawLine (new Vector2(transform.position.x, transform.position.y + 0.8f), i_facingRight () ? new Vector2 (transform.position.x + frontalDistanceOfView, transform.position.y + 0.8f) : new Vector2 (transform.position.x - frontalDistanceOfView, transform.position.y + 0.8f), Color.yellow);
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.8f) , i_facingRight()? Vector2.right : -Vector2.right, frontalDistanceOfView, fleeLayer);

		//RaycastHit2D hit = Physics2D.Raycast(transform.position, i_facingRight()? Vector2.right : -Vector2.right, frontalDistanceOfView, fleeLayer);
		if (hit.collider != null) {
			
			makeStateTransition(eMS,enemyMachineState.Flee, hit.transform.gameObject);
			
			return;
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

	
	//METODI GESTIONE STATO PATROL-----------------------------------------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------------------------------------------------

	private void finalizePatrol(){

		paSS = defaultPaType;

	}

	private void initializePatrolSuspiciousVariables() {

		firstCheckDone_Suspicious = false;
		bool standingSusp = false;
		bool exitSuspicious = false;
	}

	private void initializePatrolBasicVariables() {

		switch (paSS) {

			case patrolSubState.Walk :
				//non dovrei inizializzare nulla nel caso walk, per ora...
				break;

			default :
				patrollingTowardAPoint = false;
				patrolledTarget = null;
				setTargetAStar (null);

				break;

		}

	}

	private void initializePatrol(bool changeDir = false){

		initializePatrolBasicVariables ();

		initializePatrolSuspiciousVariables ();

		//variabile che serve a flee-chase più che a patrol...
		//TODO: capire se è il caso di gestirla meglio
		notImpressionedByFleeingByTarget = false;

		//TODO: now da eliminare...
		/*
		switch (paSS) {

			case patrolSubState.AreaSuspicious :
				reacheadOneSuspPoint = false;
				break;

			default :
				break;
				
		}
		*/

		//TODO : da gestire meglio...??
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

		//controllo se ho degli OSTACOLI in mezzo...
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.0f) , i_facingRight()? Vector2.right : -Vector2.right, frontalDistanceOfView, obstacleLayers);
		if (hit.collider != null) {
			return enemyMachineState.Patrol;
		}

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

		switch (paSS) {

			case patrolSubState.Walk :
				continueWander();
				
				break;
			case patrolSubState.Stand :
				patrollingBetweenPoints(0);
				
				break;
			case patrolSubState.Area :
				patrollingBetweenPoints(0);
				
				break;

			case patrolSubState.AreaSuspicious :
				//patrollingSuspicious();
				standSuspicious();
				break;
				
			default :
				break;

		}


	}


	private void setNextPatrolTargetPoint(GameObject t) {
		
		setTargetAStar (patrolledTarget.gameObject);
		
	}
	
	//TODO : PRIOR il parametro è toglibile... c'è già il parametro globale
	private void searchNextPatrollingPoint() {
		

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
	
	private void patrollingBetweenPoints(int mode, float myScaleFactor=1) {
		
		//Debug.Log ("ciao1");


		if (patrolledTarget != null) {

			if (Mathf.Abs (patrolledTarget.transform.position.y - groundCheckTransf.position.y) > thresholdHeightDifference) {

				paSS = patrolSubState.Walk;
			
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
					moveTowardTarget(true, myScaleFactor);
				}
				else {

					//moveAlongAStarPathNoLimits();
				}
			}
			
			
		}
		
		
	}
	

	private IEnumerator standSuspiciousTimer() {

		if (eMS == enemyMachineState.Patrol) {
			standingSusp = true;

			yield return new WaitForSeconds (tSearchLenght * 0.5f);

			if (eMS == enemyMachineState.Patrol) {
				
				if (!firstCheckDone_Suspicious) {
					firstCheckDone_Suspicious = true;
					standingSusp = false;
					i_flip ();
				} else {
					
					standingSusp = false;
					firstCheckDone_Suspicious = false;
					exitSuspicious = true;
					
				}
			} else {

				if(DEBUG_FSM_TRANSITION[1])
					Debug.Log ("CAMBIO DI STATO CON ROUTINE IN CORSO");

				standingSusp = false;
				firstCheckDone_Suspicious = false;
			}

		} 
		else {

			if(DEBUG_FSM_TRANSITION[1])
				Debug.Log ("CAMBIO DI STATO CON ROUTINE IN CORSO");

			standingSusp = false;
			firstCheckDone_Suspicious = false;
		}
	}

	private void standSuspicious(){

		if(exitSuspicious) {
			i_flip();
			exitSuspicious = false;
			makeSubStateTransition(defaultPaType);
			return;
		}

		if (!firstCheckDone_Suspicious) {
			if(!standingSusp) {
				StartCoroutine( standSuspiciousTimer());
			}

		}
		else {
			if(!standingSusp) {
				StartCoroutine( standSuspiciousTimer());
			}
		}

	}
	
	
	//METODI GESTIONE STATO CHASING---------------------------------------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------------------------------------------------------------------

	private void finalizeChase() {

		setStatusSprite();

	}

	private void initializeChase(GameObject t) {
		
		setTargetAStar (t);

		switch (chSS) {

			case chaseSubState.ChargedCh :

				chaseCharged = false;
				chaseCharging = false;
				losingTarget = false;

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

	private bool isTargetOutOfMyView(bool timeCheck) {

		if (myTarget == null)
			return false;
		
		if (timeCheck ) {

			if (	((transform.position.x > myTarget.transform.position.x) && (pm.FacingRight)	)	|| 
			    	((transform.position.x < myTarget.transform.position.x) && (!pm.FacingRight)	)		) {

				if(DEBUG_FSM_TRANSITION[2])
					Debug.Log ("CH -> PA - target perso perché io guardo a destra e lui è a sinistra o viceversa");
				//suspiciousUp();

				if(!losingTarget) {
					losingTarget = true;
					tStartLosingTarget = Time.time;
				}
				else {
					if(Time.time > tStartLosingTarget + tLosingTargerLenght) {
						Debug.Log ("ritorno");
						makeSubStateTransition(patrolSubState.AreaSuspicious);
						return true;

					}

				}

			}
			else {

				losingTarget = false;

			}

			
		} 
		else {
			
			if ( ( (transform.position.x > myTarget.transform.position.x) && pm.FacingRight  )	||
			     ( (transform.position.x < myTarget.transform.position.x) && !pm.FacingRight )		) {

				if(DEBUG_FSM_TRANSITION[2])
					Debug.Log ("CH -> PA - target perso perché io guardo a destra e lui è a sinistra o viceversa");
				//suspiciousUp();
				makeSubStateTransition(patrolSubState.AreaSuspicious);
				return true;
			}
			
		}

		
		return false;

	}

	private bool isTargetOutOfMyView1(bool distanceCheck) {

		if (myTarget == null)
			return false;

		if (distanceCheck ) {

			float dist = Mathf.Abs(Vector2.Distance (groundCheckTransf.position, myTarget.transform.position));
			
			if ((transform.position.x > myTarget.transform.position.x) && (pm.FacingRight) && (dist > frontalDistanceOfView * fTargetFar) ) {
				if(DEBUG_FSM_TRANSITION[2])
					Debug.Log ("CH -> PA - target perso perché io guardo a destra e lui è a sinistra");
				//suspiciousUp();
				makeSubStateTransition(patrolSubState.AreaSuspicious);
				return true;
			}
			
			
			if ((transform.position.x < myTarget.transform.position.x) && (!pm.FacingRight) && (dist > frontalDistanceOfView * fTargetFar) ) {
				if(DEBUG_FSM_TRANSITION[2])
					Debug.Log ("CH -> PA - target perso perché io guardo a sinistra e lui è a destra");
				//suspiciousUp();
				makeSubStateTransition(patrolSubState.AreaSuspicious);
				return true;
			}

		} 
		else {

			if ((transform.position.x > myTarget.transform.position.x) && pm.FacingRight) {
				if(DEBUG_FSM_TRANSITION[2])
					Debug.Log ("CH -> PA - target perso perché io guardo a destra e lui è a sinistra");
				//suspiciousUp();
				makeSubStateTransition(patrolSubState.AreaSuspicious);
				return true;
			}
			
			
			if ((transform.position.x < myTarget.transform.position.x) && !pm.FacingRight) {
				if(DEBUG_FSM_TRANSITION[2])
					Debug.Log ("CH -> PA - target perso perché io guardo a sinistra e lui è a destra");
				//suspiciousUp();
				makeSubStateTransition(patrolSubState.AreaSuspicious);
				return true;
			}

		}



		
		return false;
	}
	
	//NON PIU USATA
	//TODO : NOW da cancellare?
	private bool isTargetFar(float dist){
		if (dist > frontalDistanceOfView * scale_FrontalDistanceOfView_ToBeFar) {
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

	private bool isTargetAtDifferentHeight(bool linearDistanceCheck) {

		if (myTarget == null)
			return false;

		float hDiff = Mathf.Abs (groundCheckTransf.position.y - myTarget.transform.position.y );
			
		if (linearDistanceCheck) {

			float dist = Vector2.Distance (groundCheckTransf.position, myTarget.transform.position);

			if ((hDiff > 2.5f) && (dist > frontalDistanceOfView * 2.0f))
				return true;
			else
				return false;


		} else {

			if (hDiff > 2.5f)
				return true;
			else
				return false;

		}






	}

	private bool isObstacleHit(){

		if(chaseCharged && myTarget!=null) {


			RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.0f), i_facingRight()? Vector2.right : -Vector2.right, 1.0f, obstacleLayers);
			if (hit.collider != null) {
				if(DEBUG_FSM_TRANSITION[2])
					Debug.Log ("CH -> PA - urtato ostacolo!" + hit.collider.gameObject.name);
				return true;
				
			}

		}

		return false;
		
	}

	private enemyMachineState isStoppedChase() {

		float dist;

		switch (chSS) {

			case chaseSubState.ChargedCh :
				if(isTargetAtDifferentHeight(true)) {
					
					if(DEBUG_FSM_TRANSITION[1])
						Debug.Log ("CH -> PA - target va ad una altezza diversa dalla mia");
					
					return enemyMachineState.Patrol;
				}
				
				

				if(isTargetOutOfMyView(true)) {
					
					if(DEBUG_FSM_TRANSITION[1])
						Debug.Log ("CH -> PA - perso di vista dopo la corsa");
					
					return enemyMachineState.Patrol;
				}
				
				break;

			case chaseSubState.Constant :
				if(isTargetAtDifferentHeight(false)) {
					
					if(DEBUG_FSM_TRANSITION[1])
						Debug.Log ("CH -> PA - target va ad una altezza diversa dalla mia");
					
					return enemyMachineState.Patrol;
				}
				if(isTargetOutOfMyView(false)) {
					
					if(DEBUG_FSM_TRANSITION[1])
						Debug.Log ("CH -> PA - target perso di vista, non più davanti a me");
					
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

		dist = Vector2.Distance (groundCheckTransf.position, myTarget.transform.position);
		
		//se è troppo lontano torno a patrol...
		
		//se è vicino passo ad attack
		if (isTargetNear (dist)) {
			
			if(DEBUG_FSM_TRANSITION[1])
				Debug.Log ("CH -> AT - target è nella mia zona di attacco");
			
			return enemyMachineState.Attack;
			
		}
		
		return enemyMachineState.Chase;

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
		
		moveTowardTarget(false, Mathf.Lerp (0.5f, 1.0f, mydelta));

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
			moveTowardTarget (false, Mathf.Lerp (0.5f, 1.0f, mydelta));
		else
			i_move (false);
	}

	private void continueChase(){

		switch (chSS) {

			case chaseSubState.ChargedCh :
				if(chaseCharged) {

					crashTowardTarget();
					
				}
				else {
					if(!chaseCharging) {
						StartCoroutine(chargeChase());
					}
				}

				break;

			case chaseSubState.Constant :
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

		myRigidBody.velocity = new Vector2 (myRigidBody.velocity.x * 0.2f, myRigidBody.velocity.y);
		
		attackCharged = false;
		attackCharging = false;
		attackStuck = false;
	}
	
	private enemyMachineState isStoppedAttack() {

		float dist;
		//controlli ad hoc per ogni enemyType

		switch (atSS) {

			case attackSubState.ChargingAt :

				break;

			case attackSubState.Immediate :

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
		
		dist = Vector2.Distance (groundCheckTransf.position, new Vector2 (myTarget.transform.position.x, myTarget.transform.position.y));
		
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
		setStatusSprite ("stuck");

		if (DEBUG_FSM_TRANSITION [0])
			Debug.Log ("AT charged -> AT STUCK");
		
		yield return new WaitForSeconds(tStuckLenght);
		
		attackStuck = false;
		setStatusSprite ();

		if (DEBUG_FSM_TRANSITION [0])
			Debug.Log ("AT STUCK -> AT end");
	}
	
	private void continueAttack(){

		switch (atSS) {
			
			case attackSubState.ChargingAt :
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
					if(!attackCharging && !attackStuck) {
						StartCoroutine(chargeAttack());
					}
				}
				break;
				
			case attackSubState.Immediate :
				
				break;
				
			default :
				
				break;
			
		}


	}

	private void heavyAttack() {

		if(DEBUG_FSM_TRANSITION[2])
			Debug.Log ("ATTACCO PESANTE");

		myToStun.SendMessage ("c_chargedAttack");

		turnOffProjector ();

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
					if(killable) {
						StartCoroutine(handleKill());
					}
					else {
						if(involveEType) {
							eType = enemyType.Dumb;
							setupEnemy(true);
							eMS = enemyMachineState.Stunned;
							SpriteRenderer s = GetComponent<SpriteRenderer>();
							s.color = Color.blue;
						}
					}
					setStatusSprite();
					break;
				case enemyType.Heavy :
					
					if(killable) {
						StartCoroutine(handleKill());
					}
					setStatusSprite();
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
			//if(DEBUG_FSM_TRANSITION[2])
				//Debug.Log ("Stunned countdown : " + (Time.time - tLastStunnedAttackReceived));
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
		
		BoxCollider2D b2d = GetComponent<BoxCollider2D> ();
		CircleCollider2D c2d = GetComponent<CircleCollider2D> ();
		
		myRigidBody.AddForce(new Vector2(100.0f,300.0f));
		b2d.isTrigger = true;
		c2d.isTrigger = true;
		
	}
	
	private void lastWill() {
		
		if (Spawner != null) {

			Spawner.SendMessage("letsSpawn");
			//Debug.Log("spawn - enemy");

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

		if (DEBUG_FLIP[0]) {

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

			myTarget = myT;
			//myAstar.Target = myT;
			return;
		}
		
		foreach (Transform child in myT.transform) {
			if(child.tag=="GroundCheck") {
				if(DEBUG_ASTAR[0])
					Debug.Log ("ASTAR - trovato e assegnato il groundcheck");
				//myAstar.Target = child.gameObject;
				myTarget = child.gameObject;
				return;
			}
			
		}
		if(DEBUG_ASTAR[0])
			Debug.Log ("ASTAR - NON trovato il groundcheck, assegno lui stesso");

		//myAstar.Target = myT;
		myTarget = myT;
		
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
			if ( Vector3.Distance(groundCheckTransf.position, myAstar.Path[0]) < thresholdApproachingNextTile || myAstar.tempDistance < Vector3.Distance(myAstar.Path[0], myAstar.Target.transform.position)) 
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


	//FUNZIONI MOVIMENTO BASE (indipendente da Astar)-----------------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------------------------------------------------------------

	private void moveTowardTarget(bool isPatrolSpeed = true, float m_scaleFactor=1) {


		if (Mathf.Abs (myTarget.transform.position.y - groundCheckTransf.position.y) > 1.0f) {
			
			if(Mathf.Abs (myTarget.transform.position.x - groundCheckTransf.position.x) > 1.0f) {
				
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

			moveRightVerse(isPatrolSpeed, m_scaleFactor);
			//followPath(2, isPatrolSpeed, m_scaleFactor);

		}


	}

	private void moveRightVerse(bool isPatrolSpeed = true, float m_scaleFactor=1) {

	
		if( (myTarget.transform.position.x > groundCheckTransf.transform.position.x ) && !i_facingRight()) {
			i_flip ();
		}
		
		if( (myTarget.transform.position.x < groundCheckTransf.transform.position.x ) && i_facingRight()) {
			i_flip ();
		}

		
		
		i_move (isPatrolSpeed, m_scaleFactor);

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


	private void handleFreezing(bool start){

		if (start) {
			GetComponent<Animator> ().enabled = false;
			myRigidBody.velocity = new Vector2 (0.0f, 0.0f);
			myRigidBody.isKinematic = true;
			myPhysicsMat = GetComponent<BoxCollider2D>().sharedMaterial;
			GetComponent<BoxCollider2D>().sharedMaterial = GetComponent<CircleCollider2D>().sharedMaterial;
			GetComponent<BoxCollider2D>().enabled = false;
			GetComponent<BoxCollider2D>().enabled = true;
			freezedByGun = true;
		}
		else {
			myRigidBody.isKinematic = false;
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
		
		myRigidBody.velocity = new Vector2 (0.0f, 0.0f);
		myRigidBody.isKinematic = true;

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

			handleFreezing(true);
			makeAnyStTransition ();

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

	//INIZIO GESTIONE COLLISIONI--------------------------------------------------------------------------------------------------------------

	private bool checkObstacleCollision(Collision2D co) {

		if ((obstacleLayers.value & 1 << co.gameObject.layer) > 0) {

			BoxCollider2D bo = co.gameObject.GetComponent<BoxCollider2D>();
			if(bo==null){
				Debug.Log ("GESTIRE QUESTO CASO-----------------------------------");
			}
			else {

				if(bo.bounds.max.y < groundCheckTransf.position.y) {
					if (DEBUG_COLLISION [2])
						Debug.Log ("ostacolo sotto di me, è ground..." + bo.bounds.max.y);
					actualGround = co.gameObject;
					return false;
				}
				else {
					if (DEBUG_COLLISION [2])
						Debug.Log ("ostacolo al mio livello! lo considero" + bo.bounds.max.y);

					return true;
				}
			}

			/*
			if (co.transform.position.y < transform.position.y) {

				if (DEBUG_COLLISION [2])
					Debug.Log ("ostacolo sotto di me, è ground...");

				return false;

			} else {

				if (DEBUG_COLLISION [2])
					Debug.Log ("ostacolo al mio livello! lo considero");

				return true;

			}

			*/
		}

		return false;

	}

	private bool checkPlayerCollision(Collision2D co) {

		if (co.gameObject.tag == "Player") {
			//co.gameObject.transform.SendMessage ("c_stunned", true);
			co.gameObject.transform.SendMessage ("c_instantKill");
			Vector2 dist = co.gameObject.transform.position - transform.position;
			
			Rigidbody2D r = co.gameObject.GetComponent<Rigidbody2D>();
			r.velocity = new Vector2(0.0f, 0.0f);
			r.AddForce(300.0f*dist.normalized);
			//c_playerStunned(true);
			return true;
		}
		else {

			return false;

		}
	}

	private void recoil() {

		if (pm.FacingRight) {

			myRigidBody.velocity = new Vector2(0.0f, 0.0f);
			myRigidBody.AddForce(new Vector2(-200.0f, 0.0f));

		}
		else {

			myRigidBody.velocity = new Vector2(0.0f, 0.0f);
			myRigidBody.AddForce(new Vector2(200.0f, 0.0f));

		}

	}

	private bool checkRecoilOrFlip(Collision2D coo) {

		switch (eMS) {

			case enemyMachineState.Wander :
				if(DEBUG_COLLISION[2])
					Debug.Log ("WA - flippo e basta");
				i_flip();
				break;

			case enemyMachineState.Patrol :

				if(paSS== patrolSubState.Walk) {
					
					if(DEBUG_COLLISION[2])
						Debug.Log ("PA wa - flippo e basta");

					i_flip();
				}
				else {
					if(paSS==patrolSubState.AreaSuspicious) {
						Debug.Log ("ATTENZIONE - caso da gestire meglio");
						i_flip();
						return true;
					}
					else {
						Debug.Log ("ATTENZIONE - caso da gestire meglio");
						i_flip();
						return true;
					}
					   
				}
				break;

			case enemyMachineState.Chase :

				if(chSS== chaseSubState.ChargedCh) {
					if(DEBUG_COLLISION[2])
						Debug.Log ("CH CH - cambio a stun");
					
					//verifico la DIREZIONE dello sbattimento, per evitare casi strani...
					//esempio, lui è al muro, si gira e mi trova, entra in chase e sta per iniziare ad inseguirmi, ma poiché
					//è ancora in collisione con l'ostacolo viene steso a terra... invece con questo controllo evito questo strano comportamento
					if( 	((coo.transform.position.x > transform.position.x) && (pm.FacingRight) 	) 	||
					   		((coo.transform.position.x < transform.position.x) && (!pm.FacingRight) )		){
					
						makeStateTransition(eMS, enemyMachineState.Stunned);
						recoil();
					}
					else {
						//Debug.Log ("nome oggetto" + coo.gameObject.name + " x oggetto " + coo.transform.position.x + " mia x " + transform.position.x + " mia velocità orizz " + myRigidBody.velocity.x);
					}
					
				}
				else {
					if(DEBUG_COLLISION[2])
						Debug.Log ("CH CO - flippo e cambio a patrol");

					i_flip();
					return true;

				}
				break;

			case enemyMachineState.Flee :
				if(DEBUG_COLLISION[2])
						Debug.Log ("FL CO - flippo e cambio a patrol");

				i_flip();
				return true;
				break;

			default :
				Debug.Log ("CASO DI COLLISIONE IN UNO STATO NON PREVISTO");
				break;

		}

		return false;

	}

	private bool handleObstacleCollision(Collision2D co) {

		switch (eType) {

			case enemyType.Dumb :

				i_flip();

				break;

			case enemyType.Guard :
				
				return( checkRecoilOrFlip(co));
				
				break;

			case enemyType.Heavy :
				
				return( checkRecoilOrFlip(co));
				
				break;

		}

		return false;

	}

	public void OnTriggerEnter2D(Collider2D c) {

		if (c.name == "Proiettore") {

			if(c.gameObject.transform.parent.tag == "Lantern") {

				turnOffProjector();
			}
			//mia reazione?

		}

	}

	private void turnOffProjector() {

		GameObject o = GameObject.FindGameObjectWithTag("MagicLanternLogic");
		if(o != null) {
			
			o.SendMessage("c_touchedByEnemy");
		}

	}

	public void OnCollisionStay2D(Collision2D c) {

		if (c.gameObject == actualGround)
			return;

		if(DEBUG_COLLISION[0])
			Debug.Log("HO COLLISO CON " + c.gameObject.name);

		if (!handlingCollision)
			StartCoroutine (coroutineCollEnter (c));

	}

	public IEnumerator coroutineCollEnter(Collision2D c) {
		//Debug.Log ("dai cazzo");
		if(DEBUG_COLLISION[1])
			Debug.Log("COROUTINE -> HO COLLISO CON " + c.gameObject.name);

		handlingCollision = true;

		bool playerColl = false;

		//se l'AI è stunnata non reca danno al player, per ora...
		if (eMS == enemyMachineState.Stunned) {
			handlingCollision = false;
			return true;
		}


		playerColl = checkPlayerCollision (c);

		if(checkObstacleCollision (c)) {
			//colliso con un ostacolo

			if(playerColl) {
				//mi comporto in qualche modo se collido pure con il player?
				if(DEBUG_COLLISION[1])
					Debug.Log("HO COLLISO SIA COL PLAYER CHE CON UN OSTACOLO " + c.gameObject.name);
			}
			else {
				if(DEBUG_COLLISION[1])
					Debug.Log("HO COLLISO CON UN OSTACOLO " + c.gameObject.name);
				if(handleObstacleCollision(c)) {


					makeSubStateTransition (patrolSubState.AreaSuspicious);
					makeStateTransition (eMS, enemyMachineState.Patrol);

					//TODO: soluzione momentanea per allontanarmi dall'ostacolo
					for (int i= 0; i<10; i++) {
						i_move ();
						yield return new WaitForSeconds (0.1f);
					}
					


				}
				else {

					//Debug.Log ("non serve tornare indietro");

				}

			}



		}
		else {

			if(DEBUG_COLLISION[1])
				Debug.Log("HA COLLISO SOLO CON IL PLAYER");
		}


		if(DEBUG_COLLISION[0])
			Debug.Log ("TOCCATO " + c.gameObject.name);

		//yield return new WaitForSeconds (1.0f);

		handlingCollision = false;


	}

	public void c_instantKill(){

		killable = true;
		makeStateTransition(eMS, enemyMachineState.Stunned);

	}

	//FINE GESTIONE COLLISIONI----------------------------------------------------------------------------------------------
	
}

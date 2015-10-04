using UnityEngine;
using System.Collections;

/// <summary>
/// Classe che si occupa di gestire i movimenti del player in base agli input dell'utente
/// </summary>
public class PlayerMovements : MonoBehaviour {

	public bool jumpSLIDEMAN;
	public PhysicsMaterial2D matFrictionZero; 

	bool respawningFromDeath;

	public float timeToRespawn = 1.0f;


	//COMPONENT DEL PLAYER
	Rigidbody2D rigBody;
	Animator anim;
	AudioHandler audioHandler;

	//ALTRI OGGETTI E COMPONENT
	GameObject controller;
	CursorHandler cursorHandler;
	GameObject mainCamera;
	InputKeeper inputKeeper;
	MagicLantern magicLanternLogic;

	/*
	 * CORRADO
	 * VARIABILI DI STATO DEL PLAYER
	 */
	[System.Serializable]
	public class StateVar
	{
		[HideInInspector]
		public bool facingRight = true;

		public bool onGround = false;
		[HideInInspector]
		public bool wasGround = false;
		[Range(1.0f,5.0f)]

		public float gravityMultiplier = 1.0f;
		[HideInInspector]
		public float standardGravity;

		[HideInInspector]
		public bool freezedByTool = false;
	}

	[SerializeField]
	StateVar stateVar;

	//utile per vecchi script che usano la proprietà OnGround
	public bool onGround {
		get{ return stateVar.onGround;}
	}

	//utile per vecchi script che usano la proprietà FacingRight
	public bool FacingRight {
		get{ return stateVar.facingRight;}
		set{ stateVar.facingRight = value;}
	}

	/*
	 * CORRADO
	 * VARIABILI PER LA CORSA
	 */

	[System.Serializable]
	public class RunningVars
	{
		public float speedFactor = 5.0f;
		[HideInInspector]
		public bool running;
	}

	[SerializeField]
	RunningVars runningVars;

	//utile per vecchi script che usano il metodo isRunning()
	public bool isRunning(){
		return runningVars.running;
	}

	/*
	 * CORRADO
	 * VARIABILI PER IL SALTO
	 */

	[System.Serializable]
	public class JumpingVars
	{
		public float jumpFactor = 3.5f;
		[HideInInspector]
		public bool jumping = false;
		[HideInInspector]
		public float lastJump = 0.0f;
		[HideInInspector]
		public float diffBetweenJump = 0.3f;
	}
	
	[SerializeField]
	JumpingVars jumpingVars;

	/*
	 * CORRADO
	 * VARIABILI PER IL CONTROLLO IN ARIA
	 */

	[System.Serializable]
	public class OnAirVars
	{
		[HideInInspector]
		public bool addFallingForceRight = false;
		[HideInInspector]
		public bool addFallingForceLeft = false;
		[HideInInspector]
		public bool removeFallingNegForce = false;
		[HideInInspector]
		public bool removeFallingPosForce = false;
		[HideInInspector]
		public bool removeFallingForce = false;
		public float forceOnAirFactor = 16.0f;
		public float airXDeceleration = 8.0f;
		public float maxFallingSpeed = 14.0f;
	}

	//utile per vecchi script che usano la variabile maxFallingSpeed
	public float maxFallingSpeed{
		get{ return onAirVars.maxFallingSpeed;}
		set{ onAirVars.maxFallingSpeed = value;}
	}

	[SerializeField]
	OnAirVars onAirVars;

	/*
	 * CORRADO
	 * VARIABILI PER LA SCALA
	 */

	[System.Serializable]
	public class LadderVars
	{
		[HideInInspector]
		public bool onLadder = false;
		[HideInInspector]
		public bool jumpFromLadderRight = false;
		[HideInInspector]
		public bool jumpFromLadderLeft = false;
		[HideInInspector]
		public bool collidingLadder = false;
		public float ladderLateralLimit = 0.3f;
		public float onLadderMovement = 0.1f;
		public float fromLadderForce = 100.0f;
		[HideInInspector]
		public GameObject actualLadder;
	}

	//utile per vecchi script che usano la variabile onLadder
	public bool onLadder{
		get{ return ladderVar.onLadder;}
		set{ ladderVar.onLadder = value;}
	}

	[SerializeField]
	LadderVars ladderVar;

	/*
	 * CORRADO
	 * VARIABILI PER LA SCALA
	 */

	[System.Serializable]
	public class BounceVars
	{
		public bool fallingVelocityDependence = false;
		public float baseAscForce = 525.0f;
		public float baseAscForceSpring = 450.0f;
		public float addEnemyAscForce = 250.0f;
		public float addHeightAscForce = 200.0f;
		public int numEnemyJump = 0;
		public int maxNumEnemyCount = 2;
		[HideInInspector]
		public bool settedNumHeightLevel = false;
		public float diffHeight = 0.0f;
		public float maxDiffHeight = 9.0f;
		//[HideInInspector]
		public float addedForceDebug = 0.0f;
		[HideInInspector]
		public bool savedFallingPositions = false;
		[HideInInspector]
		public float lastYPositivePosition = 0.0f;
		[HideInInspector]
		public float firstYNegativePosition = 0.0f;
		[HideInInspector]
		public float bounceTime = 0.0f;
		public float diffBounceTime = 0.5f;
	}

	[SerializeField]
	BounceVars bounceVars;



	/*
	 * 
	 * DARIO
	 * 
	 */

	public bool underWater = false;
	//public bool UnderWater { get; set; }


	//solo per AI-------------------------

	[Range(0.1f,10.0f)]
	public float AI_walkSpeed = 4.0f;
	[Range(0.1f,10.0f)]
	public float AI_runSpeed = 5.0f;


	//HERE...
	//TODO: da valutare se lasciare o meno il set
	/*
	public AIParameters ai_par;

	public float AI_walkSpeed {
		get{return ai_par._AIwalkSpeed;}
		set{ ai_par._AIwalkSpeed = value;}
	}

	public float AI_runSpeed {
		get{ return ai_par._AIrunSpeed;}
		set{ ai_par._AIrunSpeed = value;}
	}
	*/
	
	
	//------------------------------------









	public bool AIControl = false;

	//onGround Test Objects
	public Transform GroundCheckUpperLeft;
	public Transform GroundCheckBottomRight;
	public LayerMask GroundLayers;
	public LayerMask hardGroundLayers;
	public LayerMask PlayerLayer;
	public LayerMask PlayerStunnedLayer;



	bool alert;

	bool charged;





	/*
	public GameObject MainCamera {
		get{ return mainCamera;}
		set{ mainCamera = value;}
	}
	*/



	bool stunned = false;
	float tStartStunned = -5.0f;
	float tToReturnFromStunned = 1.5f;

	Vector2 vec2Null = new Vector2(0.0f,0.0f);

	public GameObject respawnPoint;
	gameSet gs;
	//private GameObject myToStun;






	bool cameraOscura = false;

	public PlayerFeatures miniPlayerFeatures;

	public PlayerFeatures normalePlayerFeatures;

	public PlayerFeatures hugePlayerFeatures;

	PlayerFeatures activePlayerFeatures;

	public GameObject zoneAnalyzerParent;

	public teleportHandler tempTeleport;

	//public GameObject []zoneAnalyzers;




	void Start () {
		//HERE...


		if (AIControl) {
			/*
			ai_par = GetComponent<AIParameters>();

			if(ai_par==null)
				Debug.Log ("ATTENZIONE - AIParameters non trovato da playermovements");
			*/
		}


		takePlayerComponents ();
		
		takeComponents ();


		if (!AIControl){
			getRespawnPoint ();

			gs = controller.GetComponent<gameSet>();

			if(gs!=null) {
				if(gs.starterPoint>0)
					bringMeToRespawnPosition ();
			}
		}

		checkStartFacing ();

		stateVar.standardGravity = rigBody.gravityScale;


		initNormalPlayerFeatures ();

		initZoneAnalyzer ();

		/*
		foreach (Transform child in transform) {

			if(child.tag=="Stunning") {
				myToStun = child.gameObject;
				warning = false;
				break;
			}
		}

		if (warning)
			Debug.Log ("attenzione, manca un oggetto stunning sotto il player");
		*/
	
	}

	//CORRADO
	//prende i component del player
	void takePlayerComponents()
	{
		rigBody = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator> ();
		audioHandler = GetComponent<AudioHandler> ();
	}

	//CORRADO
	//prende component ed oggetti
	void takeComponents()
	{
		mainCamera = Camera.main.gameObject;
		magicLanternLogic = GeneralFinder.magicLanternLogic;
		controller = GeneralFinder.controller;
		cursorHandler = GeneralFinder.cursorHandler;
		inputKeeper = GeneralFinder.inputKeeper;
	}

	void initZoneAnalyzer() {

		if (zoneAnalyzerParent == null) {

			GameObject childAnalyzer = GameObject.FindGameObjectWithTag("ZoneAnalyzer");

			if(childAnalyzer!=null)
				zoneAnalyzerParent = childAnalyzer.transform.parent.gameObject;

		}


	}


	void initNormalPlayerFeatures() {

		activePlayerFeatures = normalePlayerFeatures;
		
		normalePlayerFeatures.jumpFactor = jumpingVars.jumpFactor;
		
		normalePlayerFeatures.speedFactor = runningVars.speedFactor;
		
		normalePlayerFeatures.scaleFactor = Mathf.Abs(transform.localScale.x);

	}







	private void getRespawnPoint () {

		bool found = false;

		foreach (Transform child in controller.transform) {

			if (child.name == "RespawnPoint"){
				respawnPoint = child.gameObject;
				found = true; 
				break;
			}

		}

		if(!found)
			Debug.Log ("ATTENZIONE - oggetto RespawnPoint non trovato");
	}



	private void checkStartFacing(){

		if (transform.localScale.x < 0) {
			if(transform.localScale.x == -1) {
				
				stateVar.facingRight = false;
				
			}
			else {
				
				Debug.Log ("ATTENZIONE : Assegnato uno scale strano");
				
			}
			
		}

	}

	private void bringMeToRespawnPosition(){

		if (respawnPoint != null) {

			transform.position = respawnPoint.transform.position;
			if(	(respawnPoint.transform.localScale.x > 0 && !FacingRight) ||
				(respawnPoint.transform.localScale.x < 0 && FacingRight)	) {
				Flip();
			}

		}

	}

	//CORRADO E DARIO
	void Update () {
	
		if (!PlayStatusTracker.inPlay)
			return;

		if (respawningFromDeath) {
			setAnimations();
			return;
		}

		if (!ladderVar.onLadder) {
			stateVar.onGround = groundCheck ();

			if (!stateVar.freezedByTool && !AIControl && !stunned) {

				//gestione del flipping del player
				flipHandling();
	
				//se tocco terra
				if (stateVar.onGround) {
					//resetto le variabili di movimento in aria
					resetAscForceVariables ();
					
					//gestione della corsa
					runningManagement ();
					
					//gestione del salto (viene preso l'input, la forza verrà gestita nel FixedUpdate)
					jumpingManagement ();

				} else {
					
					//salvo le posizioni utili a calcolare le forze di rimbalzo

					if(underWater) {

						underWaterManagement();

					}

					saveFallingPositionsManagement ();
					
				}

				
				//gestione del movimento in aria
				notGroundManagement ();
				
				//gestione della collisione con la scala
				CollidingLadderManagement ();

				//gestione delle movingPlatform
				movingPlatformManagement();

				slideManagement();

			} else {
				
				if (stunned) {
					
					//riga che avevo provato a mettere per controbilanciare le piattaforme con friction zero...
					//ma per ora meglio di no
					//rigBody.velocity = vec2Null;
					
					if (stunned && !AIControl) {
						//player stunned
						handleStunned ();
					}
				}
				//AI stunned è gestito da esterno
				
				//personaggio freezato
				if (stateVar.freezedByTool) {
					rigBody.velocity = vec2Null;
				}
				
				//personaggio AI
				
				//per ora solo check per vedere se AI è fermo
				if (AIControl) {
					
					if (rigBody.velocity.x == 0.0f) {
						//anim.SetBool ("Running", false);
						runningVars.running = false;
					}
				}
			}
		
		}
		//sono sulla scala
		else {
			//gestione del movimento lungo la scala
			OnLadderManagement ();
		}

		setAnimations ();

	}

	void slideManagement() {

		if (jumpSLIDEMAN)
			return;

		if (matFrictionZero == null)
			return;

		CircleCollider2D cc = GetComponent<CircleCollider2D> ();

		//Debug.Log ("cc ha " + cc.sharedMaterial.ToString ());

		if (!stateVar.onGround && cc.sharedMaterial == null) {
			//Debug.Log ("attivo slide");
			cc.sharedMaterial = matFrictionZero;
			//cc.enabled = false;
			//cc.enabled = true;

		}

		if (stateVar.onGround && cc.sharedMaterial != null) {
			//Debug.Log ("DISattivo slide");
			cc.sharedMaterial = null;
			//cc.enabled = false;
			//cc.enabled = true;

		}

	}

	//CORRADO
	//gestisce il flipping del player
	void flipHandling()
	{
		//il player si gira nella direzione in cui sta puntando la lanterna
		if (magicLanternLogic!= null && magicLanternLogic.active && !magicLanternLogic.leftLantern)
		{
			if (((cursorHandler.getCursorWorldPosition ().x > transform.position.x) && !FacingRight) ||
			    ((cursorHandler.getCursorWorldPosition ().x < transform.position.x) && FacingRight))
			{
				Flip ();
			}	
		}

		//il player si gira nella direzione del cursore
		if (cursorHandler!=null && cursorHandler.isCursorMoving())
		{
			if (((cursorHandler.getCursorWorldPosition ().x > transform.position.x) && !FacingRight) ||
			    ((cursorHandler.getCursorWorldPosition ().x < transform.position.x) && FacingRight))
			{
				Flip ();
			}	
		}

		//se la lanterna è attiva ma l'ho lasciata, oppure non è attiva, il player gira in base al suo movimento solo se non sto muovendo il cursore
		if (inputKeeper != null && ((magicLanternLogic.active && magicLanternLogic.leftLantern) || (!magicLanternLogic.active)) && !cursorHandler.isCursorMoving ()) {
			if ((inputKeeper.getAxis("Horizontal") < -0.2f && stateVar.facingRight) || (inputKeeper.getAxis("Horizontal") > 0.2f && !stateVar.facingRight))
			{
				Flip ();
			}
			   
		}
	}

	//CORRADO
	//funzione chiamata ogni volta che l'animazione del player, raggiunge un frame in cui il piede tocca terra
	public void animationEventTest()
	{
		audioHandler.playForcedClipByName ("TonfoCaduta");
		//Debug.Log ("passo");
	}

	//CORRADO E DARIO
	//setta le variabili dell'animator, in base alle funzioni precedenti dell'update
	void setAnimations()
	{
		if(!anim.GetBool("Stunned") && stunned)
			anim.SetTrigger("StartStunned");

		if (AIControl) {

			if(!anim.GetBool("Alerted") && alert) {

				anim.SetTrigger("StartAlert");

			}

			anim.SetBool ("Charged", charged);
			anim.SetBool ("Alerted", alert);

		}

		anim.SetBool ("Stunned", stunned);

		if (stateVar.onGround && ((ladderVar.onLadder && rigBody.velocity.y <= 0.0f) || (!ladderVar.onLadder)))
			anim.SetBool ("onGround", true);
		else
			anim.SetBool ("onGround", false);

		anim.SetBool ("Running", runningVars.running);

		

		//velocità animazione run proporzionale all'input orizzontale
		if (!AIControl) {


			if(Mathf.Abs( rigBody.velocity.x ) > 0.0f ) {

				if(stateVar.onGround) {
					anim.speed = Mathf.Clamp( Mathf.Abs(GeneralFinder.inputManager.getAxis ("Horizontal") ) / 1.5f, 0.2f, 0.75f);
				}
				else {
					anim.speed = 1.0f;
				}
			}
			else {
				
				//anim.enabled = true;
				anim.speed = 1.0f;

			}



		}

		if (!AIControl) {

			if (runningVars.running) {
				if (((rigBody.velocity.x > 0.0f) && !stateVar.facingRight) || ((rigBody.velocity.x < 0.0f) && stateVar.facingRight))
					anim.SetBool ("Backwards", true);
				else
					anim.SetBool ("Backwards", false);
			}



			anim.SetBool ("OnLadder", ladderVar.onLadder);
			if (ladderVar.onLadder && rigBody.velocity.y != 0.0f)
				anim.SetBool ("Climbing", true);
			else
				anim.SetBool ("Climbing", false);

		}

	}

	//CORRADO
	//se sono sulla scala, ignoro le collisioni con i softGround
	void setCollisionsOnLadder(bool toIgnore = true)
	{
		GameObject[] softGrounds = GameObject.FindGameObjectsWithTag("SoftGround");

		for (int i = 0; i < softGrounds.Length; i++) {
			if (softGrounds[i] != null)
			{
				Physics2D.IgnoreCollision(softGrounds[i].transform.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>(), toIgnore);
				Physics2D.IgnoreCollision(softGrounds[i].transform.GetComponent<BoxCollider2D>(), GetComponent<CircleCollider2D>(), toIgnore);
			}
		}
	}

	//CORRADO
	void FixedUpdate()
	{
		if (!stateVar.freezedByTool && !AIControl) {
			//fa un salto
			jumpingManagementFU ();

			//aggiunge forza durante lo spostamento in aria
			notGroundManagementFU ();

			//scala
			OnLadderManagemetFU ();
		}
	}

	//CORRADO
	//in aria, salva varibili utili per calcolare la forza di rimbalzo
	void saveFallingPositionsManagement()
	{
		if (!bounceVars.savedFallingPositions)
		{
			if (rigBody.velocity.y >= 0.0f){
				//salva la posizione Y più elevata raggiunta durante il salto
				bounceVars.lastYPositivePosition = transform.position.y;
			}else{
				bounceVars.firstYNegativePosition = transform.position.y;
				bounceVars.savedFallingPositions = true;
			}
		}
	}


	//CORRADO
	public void setAscForce(bool isSpring = false)
	{
		//richiamo la funzione solo se è passato il tempo necessario
		if (Mathf.Abs (Time.time - bounceVars.bounceTime) > bounceVars.diffBounceTime) {
			bounceVars.bounceTime = Time.time;

			//richiamato al primo rimbalzo, la variabile settedNumHeightLevel deve essere rimessa a false ogni volta che si tocca terra
			if (!bounceVars.settedNumHeightLevel && !bounceVars.fallingVelocityDependence) {
				
				//faccio una media tra l'ultima posizione con velocità positiva registrata e la prima con velocità negativa,
				//per approssimare la massima posizione raggiunta
				float mediumMaxHeight = (bounceVars.lastYPositivePosition + bounceVars.firstYNegativePosition) / 2;
				
				if (mediumMaxHeight>transform.position.y)
				{
					bounceVars.diffHeight = Mathf.Abs (mediumMaxHeight - transform.position.y);
					if (bounceVars.diffHeight > bounceVars.maxDiffHeight)
						bounceVars.diffHeight = bounceVars.maxDiffHeight;
				}else{
					bounceVars.diffHeight = 0.0f;
				}
				
				bounceVars.settedNumHeightLevel = true;
			}
			
			float velocity = 0.0f;
			if (bounceVars.fallingVelocityDependence) {
				velocity = Mathf.Abs (rigBody.velocity.y);
			}
			
			rigBody.velocity = new Vector3(rigBody.velocity.x,0.0f,0.0f);
			
			//dipende dalla velocità di caduta
			if (bounceVars.fallingVelocityDependence) {
				bounceVars.addedForceDebug = bounceVars.baseAscForce + (bounceVars.numEnemyJump*bounceVars.addEnemyAscForce) + (velocity*0.1f*bounceVars.addHeightAscForce);
			} else {
				
				//dipende dall'altezza di caduta, non dalla velocità
				if (isSpring)
					bounceVars.addedForceDebug = bounceVars.baseAscForceSpring + (bounceVars.numEnemyJump*bounceVars.addEnemyAscForce) + (bounceVars.diffHeight/4*bounceVars.addHeightAscForce);
				else
					bounceVars.addedForceDebug = bounceVars.baseAscForce + (bounceVars.numEnemyJump*bounceVars.addEnemyAscForce) + (bounceVars.diffHeight/4*bounceVars.addHeightAscForce);
			}

			rigBody.AddForce(new Vector2 (0.0f,bounceVars.addedForceDebug));

		}
		if (audioHandler != null)
			audioHandler.playClipByName ("RimbalzoNemico");
	}

	//CORRADO
	//chiamata dal nemico ad ogni salto in testa, incrementa una variabile
	public void addEnemyCount()
	{
		if (bounceVars.numEnemyJump<bounceVars.maxNumEnemyCount)
			bounceVars.numEnemyJump++;
	}
		
	//CORRADO
	//resetta le variabili che si usano in aria, viene chiamata quando il player è a terra
	void resetAscForceVariables()
	{
		bounceVars.numEnemyJump = 0;
		bounceVars.settedNumHeightLevel = false;
		bounceVars.diffHeight = 0.0f;
		bounceVars.lastYPositivePosition = transform.position.y;
		bounceVars.savedFallingPositions = false;
	}

	//CORRADO
	//gestione del salto nell'update, imposta una variabile usata nel fixedUpdate
	void jumpingManagement()
	{
		if (inputKeeper!=null && inputKeeper.isButtonDown ("Jump") && ((Time.time - jumpingVars.lastJump)> jumpingVars.diffBetweenJump)) {
			jumpingVars.lastJump = Time.time;
			jumpingVars.jumping = true;
		}
	}

	//CORRADO
	//gestione del salto nel FixedUpdate, assegna una forza ascendente al player
	void jumpingManagementFU()
	{
		if (jumpingVars.jumping == true) {
			if (audioHandler != null)
				audioHandler.playClipByName("Salto");

			if (!cameraOscura)
				rigBody.AddForce (new Vector2 (0.0f, 150.0f * jumpingVars.jumpFactor));
			else
				rigBody.AddForce (new Vector2 (0.0f, (transform.localScale.y) * 150.0f * jumpingVars.jumpFactor));
			
			jumpingVars.jumping = false;
		}
	}

	//CORRADO
	void resetFallingForcesVariables()
	{
		onAirVars.removeFallingForce = false;
		onAirVars.removeFallingNegForce = false;
		onAirVars.removeFallingPosForce = false;
	}

	//CORRADO
	//gestisce l'input quando il player è in aria. Setta alcune variabili usate poi nel FixedUpdate per assegnare forze
	void notGroundManagement()
	{
		if (inputKeeper!=null && !stateVar.onGround && !ladderVar.onLadder) {
			if (inputKeeper.getAxis ("Horizontal") == 0.0f)
			{
				onAirVars.addFallingForceRight = false;
				onAirVars.addFallingForceLeft = false;

				//rallentamento del personaggio
				if (rigBody.velocity.x != 0.0f && !onAirVars.removeFallingForce)
					onAirVars.removeFallingForce = true;
			}

			else{
				if (inputKeeper.getAxis ("Horizontal") > 0.0f){
					//se muovo il player verso destra, imposto la variabile usata poi nel FU per assegnare la forza
					onAirVars.addFallingForceRight = true;
					//se supero la velocità massima non aggiungo forza
					if ((rigBody.velocity.x > 0.0f) && (Mathf.Abs (rigBody.velocity.x) > runningVars.speedFactor)){
						onAirVars.addFallingForceRight = false;
					}
				}else if (inputKeeper.getAxis ("Horizontal") < 0.0f){
					//se muovo il player verso sinistra, imposto la variabile usata poi nel FU per assegnare la forza
					onAirVars.addFallingForceLeft = true;
					//se supero la velocità massima non aggiungo forza
					if ((rigBody.velocity.x < 0.0f) && (Mathf.Abs (rigBody.velocity.x) > runningVars.speedFactor)){
						onAirVars.addFallingForceLeft = false;
					}
				}

				//-----nuova aggiunta------
				resetFallingForcesVariables();
			}

			//setto la massima velocità di caduta
			if (rigBody.velocity.y < -onAirVars.maxFallingSpeed)
			{
				rigBody.velocity = new Vector2(rigBody.velocity.x, -onAirVars.maxFallingSpeed);
			}

		}
	}

	//CORRADO
	//movimenti in aria, da usare nel FixedUpdate (forze in gioco)
	void notGroundManagementFU()
	{
		if (!stateVar.onGround && !ladderVar.onLadder) {

			//se le variabili sono state settate nell'update, aggiungo forza
			if (onAirVars.addFallingForceRight)
			{
				rigBody.AddForce (new Vector2(onAirVars.forceOnAirFactor,0.0f));
				onAirVars.addFallingForceRight = false;
			}

			if (onAirVars.addFallingForceLeft)
			{
				rigBody.AddForce (new Vector2(-onAirVars.forceOnAirFactor,0.0f));
				onAirVars.addFallingForceLeft = false;
			}

			//-----nuova aggiunta------
			if (onAirVars.removeFallingForce)
			{
				//ho superato il limite
				if ((rigBody.velocity.x < 0.0f && onAirVars.removeFallingPosForce) || (rigBody.velocity.x > 0.0f && onAirVars.removeFallingNegForce))
				{
					rigBody.velocity = new Vector2(0.0f, rigBody.velocity.y);
					onAirVars.removeFallingForce = false;
					onAirVars.removeFallingNegForce = false;
					onAirVars.removeFallingPosForce = false;
					return;
				}

				if (rigBody.velocity.x < 0.0f)
				{
					onAirVars.removeFallingNegForce = true;
					rigBody.AddForce (new Vector2(onAirVars.airXDeceleration,0.0f));
				}else if(rigBody.velocity.x > 0.0f)
				{
					onAirVars.removeFallingPosForce = true;
					rigBody.AddForce (new Vector2(-onAirVars.airXDeceleration,0.0f));
				}

			}

		}
	}

	//CORRADO
	//agisce sui collider dei softGround, gli elementi che possono essere attraversati dal basso verso l'alto
	void softGroundCollManagement ()
	{

		GameObject[] softGrounds = GameObject.FindGameObjectsWithTag ("SoftGround");

		for (int i = 0; i < softGrounds.Length; i++) {
			if (softGrounds[i] != null)
			{
				BoxCollider2D objCollider = softGrounds[i].transform.GetComponent<BoxCollider2D> ();
				SpriteRenderer objRend = softGrounds[i].transform.GetComponent<SpriteRenderer> ();
				
				if (!ladderVar.onLadder) {
					
					if (objRend.bounds.max.y > GroundCheckUpperLeft.transform.position.y) {
						objCollider.enabled = false;
					} else {
						objCollider.enabled = true;
					}
				}else
					objCollider.enabled = false;
			}
		}
	}

	//CORRADO
	//assegna le giuste variabili se decido si salire su una scala con cui sto collidendo
	void CollidingLadderManagement()
	{
		//se decido di salire sulla scala
		if (inputKeeper!=null && ladderVar.collidingLadder && !ladderVar.onLadder) {

			if (((inputKeeper.getAxisRaw ("Vertical") > 0.5f) || (inputKeeper.getAxisRaw ("Vertical") < -0.5f && !groundCheckOnLadder()))) 
			{
				//controllo sulla distanza dalla scala, si può sostituire diminuendo la dimensione del collider, ma così ho più controllo
				if (Mathf.Abs (ladderVar.actualLadder.transform.position.x - transform.position.x) < ladderVar.ladderLateralLimit) {
					ladderVar.onLadder = true;
					stateVar.onGround = false;
					//anim.SetBool ("OnLadder", true);
					rigBody.velocity = vec2Null;
					transform.position = new Vector3 (ladderVar.actualLadder.transform.position.x, transform.position.y, transform.position.z);

					setCollisionsOnLadder();
				}
			}
		}
	}

	//CORRADO
	//gestione dei movimenti lungo la scala
	void OnLadderManagement ()
	{
		if (inputKeeper!=null && ladderVar.onLadder == true) {

			//metto la gravità a 0
			rigBody.gravityScale = 0.0f;

			//faccio in modo che il personaggio sia sempre "centrato"
			//ERA COMMENTATA NELLA BUILD (?!?!?!)
			transform.position = new Vector3 (ladderVar.actualLadder.transform.position.x, transform.position.y, transform.position.z);

			//se non premo più verso il basso o l'alto, il personaggio si ferma
			if (inputKeeper.getAxisRaw("Vertical") == 0.0f)
			{
				rigBody.velocity = vec2Null;
			}else if(inputKeeper.getAxisRaw("Vertical") > 0.5f && isUpperOnLadder())
			{
				rigBody.velocity = new Vector2(0.0f, ladderVar.onLadderMovement*40.0f);
			}else if(inputKeeper.getAxisRaw("Vertical") > 0.5f && !isUpperOnLadder())
			{
				rigBody.velocity = vec2Null;
			}else if(inputKeeper.getAxisRaw("Vertical") < -0.5f && isMiddleOnLadder())
			{
				if (!groundCheckOnLadder())
					rigBody.velocity = new Vector2(0.0f, -ladderVar.onLadderMovement*40.0f);
				//sto toccando terra e premendo il tasto giù, cioè sto lasciando la scala
				else
				{
					leaveLadder();
				}
					

			}else if(inputKeeper.getAxisRaw("Vertical") < -0.5f && !isMiddleOnLadder())
			{
				//dovrebbe cadere
				rigBody.velocity = vec2Null;

				leaveLadder();
			}



			float HorInput = inputKeeper.getAxisRaw("Horizontal");

			//se mi muovo lateralmente dalla scala
			if (HorInput != 0.0f && inputKeeper.getAxisRaw("Vertical") > -0.4f && inputKeeper.getAxisRaw("Vertical") < 0.4f)
			{
				leaveLadder();
				rigBody.velocity = vec2Null;

				if (HorInput > 0.0f)
					ladderVar.jumpFromLadderRight = true;
				else
					ladderVar.jumpFromLadderLeft = true;
			}else{
				ladderVar.jumpFromLadderRight = false;
				ladderVar.jumpFromLadderLeft = false;
			}

			//bisogna resettare le variabili, perché se il player è sulla scala non è onGround, quindi le variabili rimangono settate
			resetAscForceVariables();
		}
	}

	//CORRADO
	//imposta alcune variabili nel momento in cui bisogna lasciare la scala
	void leaveLadder()
	{
		setCollisionsOnLadder(false);
		ladderVar.onLadder = false;
		rigBody.gravityScale = stateVar.standardGravity;
	}

	//CORRADO
	//gestione delle forze da applicare quando si lascia la scala
	void OnLadderManagemetFU()
	{
		if (ladderVar.onLadder) {
			if (ladderVar.jumpFromLadderRight)
			{
				rigBody.AddForce(new Vector2(ladderVar.fromLadderForce,0.0f));
				ladderVar.jumpFromLadderRight = false;
			}else if(ladderVar.jumpFromLadderLeft)
			{
				rigBody.AddForce(new Vector2(-ladderVar.fromLadderForce,0.0f));
				ladderVar.jumpFromLadderLeft = false;
			}
		}	
	}

	//gestione della corsa
	//HERE...
	void runningManagement(bool facingLeftAI = false, bool facingRightAI = false, float speedAI = 1.0f)
	{
		if (inputKeeper!=null && !AIControl) {
			rigBody.velocity = new Vector2 (inputKeeper.getAxis ("Horizontal") * runningVars.speedFactor, rigBody.velocity.y);

			//stoppo immediatamente il movimento non appena si lascia il tasto per la corsa
			if (inputKeeper.getAxisRaw ("Horizontal") == 0)
				rigBody.velocity = new Vector2 (0.0f, rigBody.velocity.y);
		}
		else {
			int directionAI = 1;
			if (facingLeftAI && !facingRightAI)
			{
				directionAI = -1;
			
			}else if (!facingLeftAI && facingRightAI)
				directionAI = 1;

			rigBody.velocity = new Vector2 (directionAI * speedAI, rigBody.velocity.y);

		}
		

		
		if (rigBody.velocity.x != 0) {
			if (!AIControl)
			{
				//if (((rigBody.velocity.x > 0.0f) && !stateVar.facingRight) || ((rigBody.velocity.x < 0.0f) && stateVar.facingRight))
					//anim.SetBool ("Backwards", true);
				//else
					//anim.SetBool ("Backwards", false);

			}
			runningVars.running = true;
			//anim.SetBool ("Running", true);
		} else {
			runningVars.running = false;
			//anim.SetBool ("Running", false);
		}
	}

	//????
	void runningManagement1(bool facingLeftAI = false, bool facingRightAI = false, bool isWalkSpeed = true, float scaleFactorAI = 1)
	{
		if (inputKeeper!=null && !AIControl) {
			rigBody.velocity = new Vector2 (inputKeeper.getAxis ("Horizontal") * runningVars.speedFactor, rigBody.velocity.y);
			
			//stoppo immediatamente il movimento non appena si lascia il tasto per la corsa
			if (inputKeeper.getAxisRaw ("Horizontal") == 0)
				rigBody.velocity = new Vector2 (0.0f, rigBody.velocity.y);
		}
		else {
			int directionAI = 1;
			if (facingLeftAI && !facingRightAI)
			{
				directionAI = -1;
				
			}else if (!facingLeftAI && facingRightAI)
				directionAI = 1;
			
			if(isWalkSpeed) {
				rigBody.velocity = new Vector2 (scaleFactorAI * AI_walkSpeed * directionAI, rigBody.velocity.y);
			}
			else {
				rigBody.velocity = new Vector2 (scaleFactorAI * AI_runSpeed * directionAI, rigBody.velocity.y);
			}
		}
		
		
		
		if (rigBody.velocity.x != 0) {
			if (!AIControl)
			{
				//if (((rigBody.velocity.x > 0.0f) && !stateVar.facingRight) || ((rigBody.velocity.x < 0.0f) && stateVar.facingRight))
				//anim.SetBool ("Backwards", true);
				//else
				//anim.SetBool ("Backwards", false);
				
			}
			runningVars.running = true;
			//anim.SetBool ("Running", true);
		} else {
			runningVars.running = false;
			//anim.SetBool ("Running", false);
		}
	}

	void underWaterManagement () {
		if (inputKeeper!=null)
			rigBody.AddForce (new Vector2 (inputKeeper.getAxis ("Horizontal") * 10.0f, inputKeeper.getAxis ("Vertical") * 20.0f));

	}

	//CORRADO
	//se sono sopra una piattaforma mobile, imposto il player come figlio
	void movingPlatformManagement()
	{
		if (stateVar.onGround) {
			GameObject GO = getGroundGameObject ();
			if (GO.tag.ToString () == "MovingPlatform") {
				//parenting
				setPlayerParentTo(GO);
			}else{
				setPlayerParentTo(null);
			}
		} else {
			setPlayerParentTo(null);
		}
		stateVar.wasGround = stateVar.onGround;
	}

	//CORRADO
	void setPlayerParentTo(GameObject parentGO)
	{
		if (parentGO != null)
			transform.parent = parentGO.transform;
		else
			transform.parent = null;
	}

	//CORRADO
	//metodi usati per capire in che parte della scala sono
	bool isUpperOnLadder()
	{
		GameObject[] Ladders = GameObject.FindGameObjectsWithTag ("Ladder");
		Bounds playerBounds = GetComponent<Collider2D> ().bounds;
		float yPlayerPos = playerBounds.max.y;
		float xPlayerPos = playerBounds.center.x;
		foreach (GameObject ladder in Ladders) {
			Bounds ladderBounds = ladder.GetComponent<Collider2D> ().bounds;
			float yMaxLadderPos = ladderBounds.max.y;
			float yMinLadderPos = ladderBounds.min.y;
			float xCenterLadder = ladderBounds.center.x;
			if (xPlayerPos <= (xCenterLadder+0.1f) && xPlayerPos >= (xCenterLadder-0.1f))
			{
				if (yPlayerPos <= yMaxLadderPos && yPlayerPos >= yMinLadderPos)
					return true;
			}
		}
		return false;
	}

	//CORRADO
	bool isMiddleOnLadder()
	{
		GameObject[] Ladders = GameObject.FindGameObjectsWithTag ("Ladder");
		Bounds playerBounds = GetComponent<Collider2D> ().bounds;
		float yPlayerPos = playerBounds.center.y;
		float xPlayerPos = playerBounds.center.x;
		foreach (GameObject ladder in Ladders) {
			Bounds ladderBounds = ladder.GetComponent<Collider2D> ().bounds;
			float yMaxLadderPos = ladderBounds.max.y;
			float yMinLadderPos = ladderBounds.min.y;
			float xCenterLadder = ladderBounds.center.x;
			//sono sulla stessa scala
			if (xPlayerPos <= (xCenterLadder+0.1f) && xPlayerPos >= (xCenterLadder-0.1f))
			{
				if (yPlayerPos <= yMaxLadderPos && yPlayerPos >= yMinLadderPos)
					return true;
			}
		}
		return false;
	}

	//CORRADO
	//se attivo il trigger della scala, setto le relative variabili
	void OnTriggerStay2D(Collider2D coll)
	{
		if (coll.tag == "Ladder") {
			ladderVar.collidingLadder = true;
			ladderVar.actualLadder = coll.gameObject;
		}
	}

	//CORRADO
	//esco dal trigger della scala
	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.tag == "Ladder") {
			ladderVar.collidingLadder = false;
		}
	}

	//CORRADO
	//giro il player
	void Flip()
	{
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		stateVar.facingRight = ! stateVar.facingRight;
	}

	//CORRADO
	//controllo che il player tocchi terra
	bool groundCheck()
	{
		return Physics2D.OverlapArea (new Vector2(GroundCheckUpperLeft.position.x,GroundCheckUpperLeft.position.y), 
		                              new Vector2(GroundCheckBottomRight.position.x,GroundCheckBottomRight.position.y), GroundLayers);
	}

	//CORRADO
	//groundCheck, ma in cui si evitano i layer softGround.
	bool groundCheckOnLadder()
	{
		return Physics2D.OverlapArea (new Vector2(GroundCheckUpperLeft.position.x,GroundCheckUpperLeft.position.y), 
		                              new Vector2(GroundCheckBottomRight.position.x,GroundCheckBottomRight.position.y), hardGroundLayers);
	}

	//CORRADO
	GameObject getGroundGameObject()
	{
		return Physics2D.OverlapArea (new Vector2(GroundCheckUpperLeft.position.x,GroundCheckUpperLeft.position.y), 
		                              new Vector2(GroundCheckBottomRight.position.x,GroundCheckBottomRight.position.y), GroundLayers).gameObject;
	}

	//CORRADO
	public void isUsingTool(bool useOrNot)
	{
		stateVar.freezedByTool = useOrNot;
	}

	/*
	 * 
	 * DARIO
	 * 
	 */

	//funzione temporanea per conversione del layer

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

	//funzione per gestire lo stato stunned del player, diversa gestione per l'AI

	private void handleStunned() {

		//Debug.Log ("stunnato");

		if (Time.time > tStartStunned + tToReturnFromStunned) {
			stunned = false;

			if(!AIControl)
				gameObject.layer = LayerMask.NameToLayer("Player"); //convertBinToDec(PlayerLayer.value);
			//myToStun.layer = convertBinToDec(PlayerLayer.value);
			//anim.SetBool ("Stunned", false);
			//Debug.Log ("fin (e STUNNNNNNN");
		}
		
	}

	//------------------------FUNZIONI WRAPPER-------------------------------

	public void c_jump()
	{
		jumpingVars.jumping = true;
		jumpingManagementFU ();
	}

	
	//HERE...
	public void c_runningManagement(bool AIfacingLeft, bool AIfacingRight, bool isWalkSpeed = true, float AIscaleFactor = 1)
	{
		runningManagement1 (AIfacingLeft, AIfacingRight, isWalkSpeed, AIscaleFactor);
		
	}


	//HERE...
	public void c_moveManagement(bool AIfacingLeft, bool AIfacingRight, float speed) {

		runningManagement (AIfacingLeft, AIfacingRight, speed);

	}

	public void c_flip()
	{
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		stateVar.facingRight = ! stateVar.facingRight;
	}

	//gestione stunned, usato sia per player che per AI
	public void c_stunned(bool isStun) {
		
		if (isStun) {
			if(!stunned) {
				runningVars.running = false;
				//anim.SetBool ("Running", false);
				//altri controlli? devo mettere a false altre variabili?
				//anim.SetBool ("Stunned", true);
				//anim.SetTrigger("StartStunned");
				stunned = true;
				if(!AIControl) {
					tStartStunned = Time.time;

					if(!AIControl)
						gameObject.layer = LayerMask.NameToLayer("PlayerStunned");// convertBinToDec(PlayerStunnedLayer.value);
					//myToStun.layer = convertBinToDec(PlayerStunnedLayer.value);
					
				}
			}
			
		}
		else {
			
			//parte di codice chiamata ESCLUSIVAMENTE da AI quando si esce da stunned

			if(!AIControl)
				gameObject.layer = LayerMask.NameToLayer("Player");//convertBinToDec(PlayerLayer.value);

			stunned = false;

		}
		
	}

	public void c_alert(bool _alert) {

		alert = _alert;

	}

	public void c_charged(bool _charged) {

		charged = _charged;

	}

	public void c_instantKill(string tagSource){

		if (!AIControl) {
			if(zoneAnalyzerParent!=null) {
				zoneAnalyzerParent.BroadcastMessage("c_playerKilled", tagSource);
			}

			StartCoroutine (handlePlayerKill ());
			c_stunned (true);

		} else {
			//se ne occupa lo script dell'AI, riceve anche lui il messaggio

		}

	}

	public void c_crushKill(string tagSource){
		//Debug.Log ("AAAAAAAAAAAAAAAA");
		if (!AIControl) {
			//Debug.Log ("AAAAAAAAAAAAAAAA1");
			if(zoneAnalyzerParent!=null) {
				zoneAnalyzerParent.BroadcastMessage("c_playerKilled", tagSource);
			}

			if(onGround) {
				//Debug.Log ("AAAAAAAAAAAAAAAA2");
				StartCoroutine (handlePlayerKill ());
				c_stunned (true);
			}
		} 
		else {
			//se ne occupa lo script dell'AI, riceve anche lui il messaggio
			//Debug.Log ("OOOOOOOOOOOOO");
			SendMessage("c_instantKill", "");
		}
		
	}

	private IEnumerator handlePlayerKill() {

		//MOD CORRADO!!!!!!!!!!
		leaveLadder();

		stateVar.onGround = false;
		runningVars.running = false;

		respawningFromDeath = true;

		if (audioHandler != null)
			audioHandler.playClipByName ("Morte");

		yield return new WaitForSeconds(0.1f);
		
		BoxCollider2D b2d = GetComponent<BoxCollider2D> ();
		CircleCollider2D c2d = GetComponent<CircleCollider2D> ();
		
		rigBody.AddForce(new Vector2(100.0f,300.0f));
		b2d.enabled = false;
		c2d.enabled = false;

		yield return new WaitForSeconds(0.5f);

		//transizione scura...

		if (tempTeleport!=null) {
			
			//GeneralFinder.camera.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
			
			tempTeleport.noEffectsTeleportPlayer();

			tempTeleport = null;

		}
		else {

			GameObject canv = GameObject.FindGameObjectWithTag ("CanvasPlayingUI");

			if (canv != null) {
				//Debug.Log ("yeah0.0");
				PlayingUIGameOver puigo = canv.GetComponent<PlayingUIGameOver>();

				if(puigo!=null) {
					puigo.c_GameOver(timeToRespawn);
					//Debug.Log ("yeah0.1");
					yield return new WaitForSeconds(timeToRespawn);
				}
				else {
					//Debug.Log ("yeah0.2");
					yield return new WaitForSeconds(timeToRespawn);
				}
			} 
			else {
				//Debug.Log ("yeah1");
				if (mainCamera != null)
					mainCamera.SendMessage ("GameOver");

				yield return new WaitForSeconds(timeToRespawn);

			}

			//riposizionamento all'ultimo checkpoint
			b2d.enabled = true;
			c2d.enabled = true;

			rigBody.velocity = new Vector2 (0.0f, 0.0f);


			bringMeToRespawnPosition ();
		}

		respawningFromDeath = false;
	
	}

	public void c_jumpEnemy(){

		setAscForce ();
		addEnemyCount ();

	}

	public void c_setUnderWater(bool uw) {

		underWater = uw;

	}

	public void c_setPlayerMovementsFeatures(bool greater ) {

		if (greater) {

			if(activePlayerFeatures== normalePlayerFeatures) {

				changePlayerMovementsFeatures(hugePlayerFeatures);

			}
			else if(activePlayerFeatures== miniPlayerFeatures) {

				changePlayerMovementsFeatures(normalePlayerFeatures);

			}

		} else {

			if(activePlayerFeatures== normalePlayerFeatures) {

				changePlayerMovementsFeatures(miniPlayerFeatures);

			}
			else if(activePlayerFeatures== hugePlayerFeatures) {

				changePlayerMovementsFeatures(normalePlayerFeatures);
				
			}


		}

	}

	void changePlayerMovementsFeatures(PlayerFeatures features) {

		float playerVerse = 1.0f;

		if (gameObject.transform.localScale.x < 0.0f) {

			playerVerse = -1.0f;

		}

		if(features.scaleFactor!=0.0f)
			gameObject.transform.localScale = new Vector3 (features.scaleFactor * playerVerse, features.scaleFactor, features.scaleFactor);

		if(features.jumpFactor!=0.0f)
			jumpingVars.jumpFactor = features.jumpFactor;

		if(features.speedFactor!=0.0f)
			runningVars.speedFactor = features.speedFactor;


		activePlayerFeatures = features;

	}

}

[System.Serializable]
public class PlayerFeatures {

	public float scaleFactor;
	public float jumpFactor;
	public float speedFactor;

	public PlayerFeatures(float _scaleFactor, float _jumpFactor, float _speedFactor) {

		scaleFactor = _scaleFactor;
		jumpFactor = _jumpFactor;
		speedFactor = _speedFactor;

	}

}

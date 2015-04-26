using UnityEngine;
using System.Collections;

public class PlayerMovements : MonoBehaviour {



	Rigidbody2D RigBody;
	Animator anim;
	//Collider coll;

	bool facingRight = true;
	bool Jumping = false;
	public bool onGround = false;
	public bool onLadder = false;
	public bool collidingLadder = false;

	public bool addFallingForceRight = false;
	public bool addFallingForceLeft = false;
	bool removeFallingNegForce = false;
	bool removeFallingPosForce = false;
	bool removeFallingForce = false;
	bool climbingUp = false;
	bool climbingDown = false;
	bool jumpFromLadderRight = false;
	bool jumpFromLadderLeft = false;

	[Range(1.0f,5.0f)]
	public float gravityMultiplier = 1.0f;

	public float speedFactor = 4.0f;

	//solo per AI-------------------------
	[Range(0.1f,10.0f)]
	public float AI_walkSpeed = 4.0f;
	[Range(0.1f,10.0f)]
	public float AI_runSpeed = 5.0f;
	
	//------------------------------------
	public float jumpFactor = 2.0f;
	public float forceOnAirFactor = 10.0f;
	public float LadderLateralLimit = 0.1f;
	public float onLadderMovement = 0.1f;
	public float fromLadderForce = 100.0f;
	public float airXDeceleration = 5.0f;
	public float maxFallingSpeed = 12.0f;

	//gestione rimbalzi
	public float baseAscForce = 300.0f;
	public float baseAscForceSpring = 500.0f;
	public float addEnemyAscForce = 100.0f;
	public float addHeightAscForce = 50.0f;
	//public float HeightLevels = 5.0f;
	public int numEnemyJump = 0;
	public int maxNumEnemyCount = 2;
	//public int numHeightLevel = 0;
	//public float lastTouchedHeight;
	bool settedNumHeightLevel = false;
	public float diffHeight = 0.0f;
	public float maxDiffHeight = 5.0f;
	public float addedForceDebug = 0.0f;
	//bool leavedGround = false;
	bool savedFallingPositions = false;
	float lastYPositivePosition = 0.0f;
	float firstYNegativePosition = 0.0f;
	//public float yPos = 0.0f;

	float standardGravity;
	bool freezedByTool = false;
	public bool AIControl = false;

	//onGround Test Objects
	public Transform GroundCheckUpperLeft;
	public Transform GroundCheckBottomRight;
	public LayerMask GroundLayers;
	public LayerMask hardGroundLayers;
	public LayerMask PlayerLayer;
	public LayerMask PlayerStunnedLayer;

	public bool running;

	GameObject actualLadder;

	GameObject controller;
	CursorHandler cursorHandler;
	GameObject mainCamera;
	public GameObject MainCamera {
		get{ return mainCamera;}
		set{ mainCamera = value;}
	}

	public bool FacingRight {
		get{ return facingRight;}
		set{ facingRight = value;}
	}

	public bool OnGround {
		get{ return onGround;}
	}

	bool stunned = false;
	float tStartStunned = -5.0f;
	float tToReturnFromStunned = 1.5f;

	Vector2 vec2Null = new Vector2(0.0f,0.0f);
	Vector3 movingPlatformPosition;
	bool goneOnMovingPlatform = false;

	public GameObject respawnPoint;
	gameSet gs;
	//private GameObject myToStun;

	void Start () {

		RigBody = transform.GetComponent<Rigidbody2D>();
		anim = transform.GetComponent<Animator> ();

		getGameController ();

		if (!AIControl){
			getRespawnPoint ();
			getCursorHandler ();

			gs = controller.GetComponent<gameSet>();

			if(gs!=null) {
				if(gs.starterPoint>0)
					bringMeToRespawnPosition ();
			}
		}

		checkStartFacing ();

		standardGravity = RigBody.gravityScale;



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

	private void getGameController(){

		controller = GameObject.FindGameObjectWithTag ("Controller");

		if (controller == null)
			Debug.Log ("ATTENZIONE - oggetto GameController non trovato");

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

	private void getCursorHandler(){

		cursorHandler = controller.GetComponent<CursorHandler> ();
		if (cursorHandler == null)
			Debug.Log ("ATTENZIONE - oggetto cursorHandler non trovato");
	}

	private void checkStartFacing(){

		if (transform.localScale.x < 0) {
			if(transform.localScale.x == -1) {
				
				facingRight = false;
				
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

	void Update () {
		//verifico se il player è a terra ed aggiorno l'animator

		if (!onLadder) {
			onGround = groundCheck ();

			if (!freezedByTool && !AIControl && !stunned) {
				
				//gestione del girarsi o meno
				//if ((facingRight == true && Input.GetAxis ("Horizontal") < 0) || (facingRight == false && Input.GetAxis ("Horizontal") > 0))
				//	Flip ();
				
				if (((cursorHandler.getCursorWorldPosition ().x > transform.position.x) && !FacingRight) ||
					((cursorHandler.getCursorWorldPosition ().x < transform.position.x) && FacingRight))
					Flip ();
				
				//se tocco terra
				if (onGround) {
					resetAscForceVariables ();
					
					//corsa
					runningManagement ();
					
					//salto (verrà gestito nel FixedUpdate)
					jumpingManagement ();


					
				} else {
					
					//salvo le posizioni utili a calcolare le forze di rimbalzo
					saveFallingPositionsManagement ();
					
				}
				
				//-------------debug-------------
				/*
				if (Input.GetKeyUp (KeyCode.Q)) {
					addEnemyCount ();
				}
				*/
				
				//gestione del movimento in aria
				notGroundManagement ();
				
				//gestione della collisione con la scala
				CollidingLadderManagement ();

				//gestione delle piattaforme che si muovono. importante che non sia nell'if dell'onground, fa un controllo all'interno
				movingPlatformManagement();
				//gestione collisioni con oggetti "softGround" (quelli in corrispondenza o meno di scale)
				//softGroundCollManagement ();
				//softGroundCollManagement_02();
			} else {
				
				if (stunned) {
					
					//riga che avevo provato a mettere per controbilanciare le piattaforme con friction zero...
					//ma per ora meglio di no
					//RigBody.velocity = vec2Null;
					
					if (stunned && !AIControl) {
						//player stunned
						handleStunned ();
					}
				}
				//AI stunned è gestito da esterno
				
				//personaggio freezato
				if (freezedByTool) {
					RigBody.velocity = vec2Null;
				}
				
				//personaggio AI
				
				//per ora solo check per vedere se AI è fermo
				if (AIControl) {
					
					if (RigBody.velocity.x == 0.0f) {
						//anim.SetBool ("Running", false);
						running = false;
					}
				}
			}

			gravityManagement();
		} 
		//sono sulla scala
		else {
			//gestione del movimento lungo la scala
			OnLadderManagement ();
		}
		
		setAnimations ();

	}

	void setAnimations()
	{
		if(!anim.GetBool("Stunned") && stunned)
			anim.SetTrigger("StartStunned");
		
		anim.SetBool ("Stunned", stunned);
		if (onGround && RigBody.velocity.y <= 0.0f)
			anim.SetBool ("onGround", true);
		else
			anim.SetBool ("onGround", false);
		anim.SetBool ("Running", running);
		if (running) {
			if (((RigBody.velocity.x > 0.0f) && !facingRight) || ((RigBody.velocity.x < 0.0f) && facingRight))
				anim.SetBool ("Backwards", true);
			else
				anim.SetBool ("Backwards", false);
		}

		anim.SetBool ("OnLadder", onLadder);
		if (onLadder && RigBody.velocity.y != 0.0f)
			anim.SetBool ("Climbing", true);
		else
			anim.SetBool ("Climbing", false);

	}

	void gravityManagement()
	{
		if (RigBody.velocity.y < 0.0f)
			RigBody.gravityScale = gravityMultiplier * standardGravity;
		else
			RigBody.gravityScale = standardGravity;

		//Debug.Log (RigBody.gravityScale);
	}

	void setCollisionsOnLadder(bool toIgnore = true)
	{
		GameObject[] softGrounds = GameObject.FindGameObjectsWithTag("SoftGround");
		
		foreach (GameObject softGround in softGrounds)
		{
			Physics2D.IgnoreCollision(softGround.transform.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>(), toIgnore);
			Physics2D.IgnoreCollision(softGround.transform.GetComponent<BoxCollider2D>(), GetComponent<CircleCollider2D>(), toIgnore);
		}
	}

	void FixedUpdate()
	{
		if (!freezedByTool && !AIControl) {
			//fa un salto
			jumpingManagementFU ();

			//aggiunge forza durante lo spostamento in aria
			notGroundManagementFU ();

			//scala
			OnLadderManagemetFU ();
		}
	}

	void saveFallingPositionsManagement()
	{
		if (!savedFallingPositions)
		{
			if (RigBody.velocity.y >= 0.0f){
				lastYPositivePosition = transform.position.y;
			}else{
				firstYNegativePosition = transform.position.y;
				savedFallingPositions = true;
			}
		}
	}

	void softGroundCollManagement_02()
	{
		//if (onLadder)
		//	onGround = groundCheckOnLadder ();
	}

	public void setAscForce(bool isSpring = false)
	{
		//richiamato al primo rimbalzo, la variabile settedNumHeightLevel deve essere rimessa a false ogni volta che si tocca terra
		if (!settedNumHeightLevel) {

			//faccio una media tra l'ultima posizione con velocità positiva registrata e la prima con velocità negativa,
			//per approssimare la massima posizione raggiunta
			float mediumMaxHeight = (lastYPositivePosition + firstYNegativePosition) / 2;

			if (mediumMaxHeight>transform.position.y)
			{
				diffHeight = Mathf.Abs (mediumMaxHeight - transform.position.y);
				if (diffHeight > maxDiffHeight)
					diffHeight = maxDiffHeight;
			}else{
				diffHeight = 0.0f;
			}

			settedNumHeightLevel = true;
		}

		RigBody.velocity = new Vector3(RigBody.velocity.x,0.0f,0.0f);

		if (isSpring)
			addedForceDebug = baseAscForceSpring + (numEnemyJump*addEnemyAscForce) + (diffHeight/4*addHeightAscForce);
		else
			addedForceDebug = baseAscForce + (numEnemyJump*addEnemyAscForce) + (diffHeight/4*addHeightAscForce);

		RigBody.AddForce(new Vector2 (0.0f,addedForceDebug));
	}

	public void addEnemyCount()
	{
		if (numEnemyJump<maxNumEnemyCount)
			numEnemyJump = numEnemyJump +1;
	}
		

	void resetAscForceVariables()
	{
		numEnemyJump = 0;
		settedNumHeightLevel = false;
		diffHeight = 0.0f;
		lastYPositivePosition = transform.position.y;
		savedFallingPositions = false;
	}

	void jumpingManagement()
	{
		if (Input.GetButtonDown ("Jump")) {
			Jumping = true;
		}
	}

	void jumpingManagementFU()
	{
		if (Jumping == true) {
			RigBody.AddForce (new Vector2 (0.0f, 150.0f * jumpFactor));
			Jumping = false;
		}
	}

	void resetFallingForcesVariables()
	{
		removeFallingForce = false;
		removeFallingNegForce = false;
		removeFallingPosForce = false;
	}

	void notGroundManagement()
	{
		if (!onGround && !onLadder) {
			if (Input.GetAxis ("Horizontal") == 0.0f)
			{
				addFallingForceRight = false;
				addFallingForceLeft = false;

				//rallentamento del personaggio
				if (RigBody.velocity.x != 0.0f && !removeFallingForce)
					removeFallingForce = true;
			}

			else{
				if (Input.GetAxis ("Horizontal") > 0.0f){
					addFallingForceRight = true;
					//se supero la velocità massima non aggiungo forza
					if ((RigBody.velocity.x > 0.0f) && (Mathf.Abs (RigBody.velocity.x) > speedFactor)){
						addFallingForceRight = false;
					}
				}else if (Input.GetAxis ("Horizontal") < 0.0f){
					addFallingForceLeft = true;
					//se supero la velocità massima non aggiungo forza
					if ((RigBody.velocity.x < 0.0f) && (Mathf.Abs (RigBody.velocity.x) > speedFactor)){
						addFallingForceLeft = false;
					}
				}



				/*
				//se sto muovendo nella stessa direzione in cui sto guardando, devo controllare che la velocità non sia già massima
				if (((RigBody.velocity.x > 0.0f) && (Input.GetAxis ("Horizontal") > 0)) || ((RigBody.velocity.x < 0.0f) && (Input.GetAxis ("Horizontal") < 0)))
				{
					if (Mathf.Abs (RigBody.velocity.x) < speedFactor)
					{
						addFallingForce = true;
					}
				}else{
					addFallingForce = true;
				}

				*/
				//-----nuova aggiunta------
				resetFallingForcesVariables();
			}

			//setto la massima velocità di caduta
			if (RigBody.velocity.y < -maxFallingSpeed)
			{
				RigBody.velocity = new Vector2(RigBody.velocity.x, -maxFallingSpeed);
			}
		}
	}

	//movimenti in aria, da usare nel FixedUpdate (forze in gioco)
	void notGroundManagementFU()
	{
		if (!onGround && !onLadder) {

			if (addFallingForceRight)
			{
				RigBody.AddForce (new Vector2(forceOnAirFactor,0.0f));
				addFallingForceRight = false;
			}

			if (addFallingForceLeft)
			{
				RigBody.AddForce (new Vector2(-forceOnAirFactor,0.0f));
				addFallingForceLeft = false;
			}

			//-----nuova aggiunta------
			if (removeFallingForce)
			{
				//ho superato il limite
				if ((RigBody.velocity.x < 0.0f && removeFallingPosForce) || (RigBody.velocity.x > 0.0f && removeFallingNegForce))
				{
					RigBody.velocity = new Vector2(0.0f, RigBody.velocity.y);
					removeFallingForce = false;
					removeFallingNegForce = false;
					removeFallingPosForce = false;
					return;
				}

				if (RigBody.velocity.x < 0.0f)
				{
					removeFallingNegForce = true;
					RigBody.AddForce (new Vector2(airXDeceleration,0.0f));
				}else if(RigBody.velocity.x > 0.0f)
				{
					removeFallingPosForce = true;
					RigBody.AddForce (new Vector2(-airXDeceleration,0.0f));
				}

			}

		}
	}

	void softGroundCollManagement ()
	{
		foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("SoftGround")) {
			
			BoxCollider2D objCollider = fooObj.transform.GetComponent<BoxCollider2D> ();
			SpriteRenderer objRend = fooObj.transform.GetComponent<SpriteRenderer> ();
			
			if (!onLadder) {
				
				if (objRend.bounds.max.y > GroundCheckUpperLeft.transform.position.y) {
					objCollider.enabled = false;
				} else {
					objCollider.enabled = true;
				}
			}else
				objCollider.enabled = false;
		}
	}
	
	void CollidingLadderManagement()
	{
		//se decido di salire sulla scala
		if (collidingLadder && !onLadder) {

			if (((Input.GetAxisRaw ("Vertical") > 0.5f) || (Input.GetAxisRaw ("Vertical") < -0.5f && !groundCheckOnLadder()))) 
			{
				//controllo sulla distanza dalla scala, si può sostituire diminuendo la dimensione del collider, ma così ho più controllo
				if (Mathf.Abs (actualLadder.transform.position.x - transform.position.x) < LadderLateralLimit) {
					onLadder = true;
					onGround = false;
					//anim.SetBool ("OnLadder", true);
					RigBody.velocity = vec2Null;
					transform.position = new Vector3 (actualLadder.transform.position.x, transform.position.y, transform.position.z);

					setCollisionsOnLadder();
				}
			}
		}
	}

	void OnLadderManagement ()
	{

		if (onLadder == true) {

			//onGround = false;

			//onGround = groundCheckOnLadder();

			RigBody.gravityScale = 0.0f;

			//faccio in modo che il personaggio sia sempre "centrato"
			//transform.position = new Vector3 (actualLadder.transform.position.x, transform.position.y, transform.position.z);

			//se non premo più verso il basso o l'alto, il personaggio si ferma
			if (Input.GetAxisRaw("Vertical") == 0.0f)
			{
				//if (onGround)
				//	onLadder = false;
				RigBody.velocity = vec2Null;
			}else if(Input.GetAxisRaw("Vertical") > 0.5f && isUpperOnLadder())
			{
				RigBody.velocity = new Vector2(0.0f, onLadderMovement*40.0f);
			}else if(Input.GetAxisRaw("Vertical") > 0.5f && !isUpperOnLadder())
			{
				RigBody.velocity = vec2Null;
			}else if(Input.GetAxisRaw("Vertical") < -0.5f && isMiddleOnLadder())
			{
				if (!groundCheckOnLadder())
					RigBody.velocity = new Vector2(0.0f, -onLadderMovement*40.0f);
				//sto toccando terra e premendo il tasto giù, cioè sto lasciando la scala
				else
				{
					leaveLadder();
				}
					

			}else if(Input.GetAxisRaw("Vertical") < -0.5f && !isMiddleOnLadder())
			{
				//dovrebbe cadere
				RigBody.velocity = vec2Null;

				leaveLadder();
			}



			float HorInput = Input.GetAxisRaw("Horizontal");
			//se mi muovo lateralmente dalla scala
			if (HorInput != 0.0f && Input.GetAxisRaw("Vertical") > -0.4f && Input.GetAxisRaw("Vertical") < 0.4f)
			{
				leaveLadder();
				RigBody.velocity = vec2Null;

				if (HorInput > 0.0f)
					jumpFromLadderRight = true;
				else
					jumpFromLadderLeft = true;
			}else{
				jumpFromLadderRight = false;
				jumpFromLadderLeft = false;
			}

			//bisogna resettare le variabili, perché se il player è sulla scala non è onGround, quindi le variabili rimangono settate
			resetAscForceVariables();
		}
	}

	void leaveLadder()
	{
		setCollisionsOnLadder(false);
		onLadder = false;
		RigBody.gravityScale = standardGravity;
	}

	void OnLadderManagemetFU()
	{
		if (onLadder) {
			if (jumpFromLadderRight)
			{
				RigBody.AddForce(new Vector2(fromLadderForce,0.0f));
				jumpFromLadderRight = false;
			}else if(jumpFromLadderLeft)
			{
				RigBody.AddForce(new Vector2(-fromLadderForce,0.0f));
				jumpFromLadderLeft = false;
			}
		}	
	}

	//gestione della corsa
	//i
	void runningManagement(bool facingLeftAI = false, bool facingRightAI = false, bool isWalkSpeed = true, float scaleFactorAI = 1)
	{
		if (!AIControl) {
			RigBody.velocity = new Vector2 (Input.GetAxis ("Horizontal") * speedFactor, RigBody.velocity.y);

			//stoppo immediatamente il movimento non appena si lascia il tasto per la corsa
			if (Input.GetAxisRaw ("Horizontal") == 0)
				RigBody.velocity = new Vector2 (0.0f, RigBody.velocity.y);
		}
		else {
			int directionAI = 1;
			if (facingLeftAI && !facingRightAI)
			{
				directionAI = -1;

			}else if (!facingLeftAI && facingRightAI)
				directionAI = 1;

			if(isWalkSpeed) {
				RigBody.velocity = new Vector2 (scaleFactorAI * AI_walkSpeed * directionAI, RigBody.velocity.y);
			}
			else {
				RigBody.velocity = new Vector2 (scaleFactorAI * AI_runSpeed * directionAI, RigBody.velocity.y);
			}
		}
		

		
		if (RigBody.velocity.x != 0) {
			if (!AIControl)
			{
				//if (((RigBody.velocity.x > 0.0f) && !facingRight) || ((RigBody.velocity.x < 0.0f) && facingRight))
					//anim.SetBool ("Backwards", true);
				//else
					//anim.SetBool ("Backwards", false);

			}
			running = true;
			//anim.SetBool ("Running", true);
		} else {
			running = false;
			//anim.SetBool ("Running", false);
		}
	}

	void movingPlatformManagement()
	{
		if (onGround) {
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
	}

	void setPlayerParentTo(GameObject parentGO)
	{
		if (parentGO != null)
			transform.parent = parentGO.transform;
		else
			transform.parent = null;
	}

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

	//se attivo il trigger della scala, setto le relative variabili
	void OnTriggerStay2D(Collider2D coll)
	{
		if (coll.tag == "Ladder") {
			collidingLadder = true;
			actualLadder = coll.gameObject;
		}
	}

	//esco dal trigger della scala
	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.tag == "Ladder") {
			collidingLadder = false;
		}
	}


	//giro il player
	void Flip()
	{
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		facingRight = ! facingRight;
	}
	
	//controllo che il player tocchi terra
	bool groundCheck()
	{
		return Physics2D.OverlapArea (new Vector2(GroundCheckUpperLeft.position.x,GroundCheckUpperLeft.position.y), 
		                              new Vector2(GroundCheckBottomRight.position.x,GroundCheckBottomRight.position.y), GroundLayers);
	}

	bool groundCheckOnLadder()
	{
		return Physics2D.OverlapArea (new Vector2(GroundCheckUpperLeft.position.x,GroundCheckUpperLeft.position.y), 
		                              new Vector2(GroundCheckBottomRight.position.x,GroundCheckBottomRight.position.y), hardGroundLayers);
	}

	GameObject getGroundGameObject()
	{
		return Physics2D.OverlapArea (new Vector2(GroundCheckUpperLeft.position.x,GroundCheckUpperLeft.position.y), 
		                              new Vector2(GroundCheckBottomRight.position.x,GroundCheckBottomRight.position.y), GroundLayers).gameObject;
	}

	public void isUsingTool(bool UseOrNot)
	{
		freezedByTool = UseOrNot;
		//anim.SetBool("usingTool",UseOrNot);
	}

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
			gameObject.layer = convertBinToDec(PlayerLayer.value);
			//myToStun.layer = convertBinToDec(PlayerLayer.value);
			//anim.SetBool ("Stunned", false);
			//Debug.Log ("fin (e STUNNNNNNN");
		}
		
	}

	//------------------------FUNZIONI WRAPPER-------------------------------

	public void c_jump()
	{
		Jumping = true;
		jumpingManagementFU ();
	}

	public void c_runningManagement(bool AIfacingLeft, bool AIfacingRight, bool isWalkSpeed = true, float AIscaleFactor = 1)
	{
		runningManagement (AIfacingLeft, AIfacingRight, isWalkSpeed, AIscaleFactor);
		
	}

	public void c_flip()
	{
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		facingRight = ! facingRight;
	}

	//gestione stunned, usato sia per player che per AI
	public void c_stunned(bool isStun) {
		
		if (isStun) {
			if(!stunned) {
				running = false;
				//anim.SetBool ("Running", false);
				//altri controlli? devo mettere a false altre variabili?
				//anim.SetBool ("Stunned", true);
				//anim.SetTrigger("StartStunned");
				stunned = true;
				if(!AIControl) {
					tStartStunned = Time.time;
					gameObject.layer = convertBinToDec(PlayerStunnedLayer.value);
					//myToStun.layer = convertBinToDec(PlayerStunnedLayer.value);
					
				}
			}
			
		}
		else {
			
			//parte di codice chiamata ESCLUSIVAMENTE da AI quando si esce da stunned
			
			//anim.SetBool ("Stunned", false);
			gameObject.layer = convertBinToDec(PlayerLayer.value);
			stunned = false;
			//myToStun.layer = convertBinToDec(PlayerLayer.value);
			//qualcosa per riporlarlo allo stato idle
		}
		
	}

	public void c_instantKill(){

		if (!AIControl) {
			StartCoroutine (handlePlayerKill ());
			c_stunned (true);
		} else {
			//se ne occupa lo script dell'AI, riceve anche lui il messaggio

		}

	}

	private IEnumerator handlePlayerKill() {
		
		yield return new WaitForSeconds(0.1f);
		
		BoxCollider2D b2d = GetComponent<BoxCollider2D> ();
		CircleCollider2D c2d = GetComponent<CircleCollider2D> ();
		
		RigBody.AddForce(new Vector2(100.0f,300.0f));
		b2d.isTrigger = true;
		c2d.isTrigger = true;

		yield return new WaitForSeconds(0.5f);

		//transizione scura...
		if(mainCamera!=null)
			mainCamera.SendMessage ("GameOver");

		yield return new WaitForSeconds(1.0f);

		//riposizionamento all'ultimo checkpoint
		b2d.isTrigger = false;
		c2d.isTrigger = false;
		bringMeToRespawnPosition ();

	}

	public void c_jumpEnemy(){

		setAscForce ();
		addEnemyCount ();

	}

}

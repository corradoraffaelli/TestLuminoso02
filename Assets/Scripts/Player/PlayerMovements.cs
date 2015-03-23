using UnityEngine;
using System.Collections;

public class PlayerMovements : MonoBehaviour {

	Rigidbody2D RigBody;
	Animator anim;

	bool facingRight = true;
	bool Jumping = false;
	bool onGround = false;
	bool onLadder = false;
	bool collidingLadder = false;

	bool addFallingForce = false;
	bool climbingUp = false;
	bool climbingDown = false;
	bool jumpFromLadderRight = false;
	bool jumpFromLadderLeft = false;

	public float speedFactor = 4.0f;
	public float jumpFactor = 2.0f;
	public float forceOnAirFactor = 10.0f;
	public float LadderLateralLimit = 0.1f;
	public float onLadderMovement = 0.1f;
	public float fromLadderForce = 100.0f;
	
	float standardGravity;
	bool freezedByTool = false;
	public bool AIControl = false;

	//onGround Test Objects
	public Transform GroundCheckUpperLeft;
	public Transform GroundCheckBottomRight;
	public LayerMask GroundLayers;

	GameObject actualLadder;

	public bool FacingRight {
		get{ return facingRight;}
	}

	bool stunnedState = false;
	float tStartStunned = -5.0f;
	float tToReturnFromStunned = 1.5f;

	void Start () {
		RigBody = transform.GetComponent<Rigidbody2D>();
		anim = transform.GetComponent<Animator> ();
		standardGravity = RigBody.gravityScale;
	}

	void Update () {

		//verifico se il player è a terra ed aggiorno l'animator
		onGround = groundCheck ();
		anim.SetBool ("onGround", onGround);
	
		if (!freezedByTool && !AIControl && !stunnedState) {
			//gestione del girarsi o meno
			if ((facingRight == true && Input.GetAxis ("Horizontal") < 0) || (facingRight == false && Input.GetAxis ("Horizontal") > 0))
				Flip ();

			//se tocco terra
			if (onGround == true) {

				//corsa
				runningManagement ();

				//salto (verrà gestito nel FixedUpdate
				jumpingManagement ();
			}

			notGroundManagement ();

			//gestione della collisione con la scala
			CollidingLadderManagement ();

			//gestione del movimento lungo la scala
			OnLadderManagement ();

			//gestione collisioni con oggetti "softGround" (quelli in corrispondenza o meno di scale)
			softGroundCollManagement ();
		} else {

			//personaggio stunned
			if(stunnedState) {
				
				handleStunned();
			}

			//personaggio freezato
			if(freezedByTool) {
				RigBody.velocity = new Vector2(0.0f,0.0f);
			}

			//personaggio AI

			//per ora solo check per vedere se AI è fermo
			if(AIControl) {

				if(RigBody.velocity.x == 0.0f)
					anim.SetBool ("Running", false);

			}


			
		}
	}

	private void handleStunned() {

		if (Time.time > tStartStunned + tToReturnFromStunned) {
			stunnedState = false;
			anim.SetBool ("Stunned", false);
		}

	}

	public void c_stunned(bool isStun) {

		if (isStun) {
			if(!stunnedState) {
				anim.SetBool ("Running", false);
				//altri controlli? devo mettere a false altre variabili?
				anim.SetBool ("Stunned", true);
				anim.SetTrigger("StartStunned");
				if(!AIControl) {
					tStartStunned = Time.time;
					stunnedState = true;
				}
			}

		}
		else {
			anim.SetBool ("Stunned", false);
			//qualcosa per riporlarlo allo stato idle
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
	
	void notGroundManagement()
	{
		if (!onGround && !onLadder) {
			if (Input.GetAxis ("Horizontal") == 0.0f)
				addFallingForce = false;
			else{
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
			}
		}
	}

	//movimenti in aria, da usare nel FixedUpdate (forze in gioco)
	void notGroundManagementFU()
	{
		if (!onGround && !onLadder) {

			if (addFallingForce)
			{
				if (facingRight)
					RigBody.AddForce (new Vector2(forceOnAirFactor,0.0f));
				else
					RigBody.AddForce (new Vector2(-forceOnAirFactor,0.0f));
				addFallingForce = false;
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
			if (Input.GetAxisRaw ("Vertical") != 0.0f) 
			{
				//controllo sulla distanza dalla scala, si può sostituire diminuendo la dimensione del collider, ma così ho più controllo
				if (Mathf.Abs (actualLadder.transform.position.x - transform.position.x) < LadderLateralLimit) {
					onLadder = true;
					anim.SetBool ("OnLadder", true);
					RigBody.velocity = new Vector2 (0.0f,0.0f);
				}
			}
		}
	}

	void OnLadderManagement ()
	{
		if (onLadder == true) {

			RigBody.gravityScale = 0.0f;

			//faccio in modo che il personaggio sia sempre "centrato"
			transform.position = new Vector3 (actualLadder.transform.position.x, transform.position.y, transform.position.z);

			//se non premo più verso il basso o l'alto, il personaggio si ferma
			if (Input.GetAxisRaw("Vertical") == 0.0f)
			{
				RigBody.velocity = new Vector2 (0.0f,0.0f);
				anim.SetBool("Climbing", false);
			}else if(Input.GetAxisRaw("Vertical") > 0.0f)
			{
				climbingUp = true;
				climbingDown = false;
				anim.SetBool("Climbing", true);
			}else if(Input.GetAxisRaw("Vertical") < 0.0f)
			{
				climbingUp = false;
				climbingDown = true;
				anim.SetBool("Climbing", true);
			}

			float HorInput = Input.GetAxisRaw("Horizontal");
			//se mi muovo lateralmente dalla scala
			if (HorInput != 0.0f)
			{
				onLadder = false;
				anim.SetBool("OnLadder", false);
				anim.SetBool("Climbing", false);

				RigBody.gravityScale = standardGravity;

				if (HorInput > 0.0f)
					jumpFromLadderRight = true;
				else
					jumpFromLadderLeft = true;
			}

		}
	}

	void OnLadderManagemetFU()
	{
		if (onLadder) {

			if (climbingUp)
			{
				transform.position = new Vector3 (transform.position.x, transform.position.y+onLadderMovement,transform.position.z);
				climbingUp = false;
			}else if(climbingDown)
			{
				transform.position = new Vector3 (transform.position.x, transform.position.y-onLadderMovement,transform.position.z);
				climbingDown = false;
			}

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
	void runningManagement(bool facingLeftAI = false, bool facingRightAI = false, float scaleFactorAI = 1)
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

			RigBody.velocity = new Vector2 (scaleFactorAI * speedFactor * directionAI, RigBody.velocity.y);
		}
		

		
		if (RigBody.velocity.x != 0)
			anim.SetBool ("Running", true);
		else 
			anim.SetBool ("Running", false);
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
	void OntriggerExit2D(Collider2D coll)
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

	public void isUsingTool(bool UseOrNot)
	{
		freezedByTool = UseOrNot;
		anim.SetBool("usingTool",UseOrNot);
	}

	//------------------------FUNZIONI WRAPPER-------------------------------

	public void c_jump()
	{
		Jumping = true;
		jumpingManagementFU ();
	}

	public void c_runningManagement(bool AIfacingLeft, bool AIfacingRight, float AIscaleFactor)
	{
		runningManagement (AIfacingLeft, AIfacingRight, AIscaleFactor);
		
	}

	public void c_flip()
	{
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		facingRight = ! facingRight;
	}
	
}

﻿using UnityEngine;
using System.Collections;

public class PlayerMovements : MonoBehaviour {

	Rigidbody2D RigBody;
	Animator anim;

	bool facingRight = true;
	bool Jumping = false;
	public bool onGround = false;
	bool onLadder = false;
	bool collidingLadder = false;

	bool addFallingForce = false;
	bool removeFallingNegForce = false;
	bool removeFallingPosForce = false;
	bool removeFallingForce = false;
	bool climbingUp = false;
	bool climbingDown = false;
	bool jumpFromLadderRight = false;
	bool jumpFromLadderLeft = false;

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
	
	float standardGravity;
	bool freezedByTool = false;
	public bool AIControl = false;

	//onGround Test Objects
	public Transform GroundCheckUpperLeft;
	public Transform GroundCheckBottomRight;
	public LayerMask GroundLayers;
	public LayerMask PlayerLayer;
	public LayerMask PlayerStunnedLayer;

	public bool running;

	GameObject actualLadder;

	public bool FacingRight {
		get{ return facingRight;}
		set{ facingRight = value;}
	}

	bool stunnedState = false;
	float tStartStunned = -5.0f;
	float tToReturnFromStunned = 1.5f;

	//private GameObject myToStun;

	void Start () {
		checkStartFacing ();
		bool warning = true;
		RigBody = transform.GetComponent<Rigidbody2D>();
		anim = transform.GetComponent<Animator> ();
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


			if(stunnedState && !AIControl) {
				//player stunned
				handleStunned();
			}
	
			//AI stunned è gestito da esterno

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
			{
				addFallingForce = false;

				//-----nuova aggiunta------
				if (RigBody.velocity.x != 0.0f && !removeFallingForce)
					removeFallingForce = true;
			}
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

				//-----nuova aggiunta------
				removeFallingForce = false;
				removeFallingNegForce = false;
				removeFallingPosForce = false;
			}

			if (RigBody.velocity.y < -maxFallingSpeed)
			{
				RigBody.velocity = new Vector2(RigBody.velocity.x, -maxFallingSpeed);
			}
			Debug.Log(RigBody.velocity.y);
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


			//-----nuova aggiunta------
			if (removeFallingForce)
			{
				//ho superato il limite
				if ((RigBody.velocity.x < 0.0f && removeFallingPosForce) || (RigBody.velocity.x > 0.0f && removeFallingNegForce))
				{
					Debug.Log ("I'm in");
					RigBody.velocity = new Vector2(0.0f, RigBody.velocity.y);
					removeFallingForce = false;
					removeFallingNegForce = false;
					removeFallingPosForce = false;
					return;
				}

				if (RigBody.velocity.x < 0.0f)
				{
					//Debug.Log ("rallenting");
					removeFallingNegForce = true;
					RigBody.AddForce (new Vector2(airXDeceleration,0.0f));
				}else if(RigBody.velocity.x > 0.0f)
				{
					//Debug.Log ("rallenting");
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
			running = true;
			anim.SetBool ("Running", true);
		} else {
			running = false;
			anim.SetBool ("Running", false);
		}
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
			stunnedState = false;
			gameObject.layer = convertBinToDec(PlayerLayer.value);
			//myToStun.layer = convertBinToDec(PlayerLayer.value);
			anim.SetBool ("Stunned", false);
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
			if(!stunnedState) {
				anim.SetBool ("Running", false);
				//altri controlli? devo mettere a false altre variabili?
				anim.SetBool ("Stunned", true);
				anim.SetTrigger("StartStunned");
				if(!AIControl) {
					tStartStunned = Time.time;
					stunnedState = true;
					gameObject.layer = convertBinToDec(PlayerStunnedLayer.value);
					//myToStun.layer = convertBinToDec(PlayerStunnedLayer.value);
					
				}
			}
			
		}
		else {
			anim.SetBool ("Stunned", false);
			gameObject.layer = convertBinToDec(PlayerLayer.value);
			//myToStun.layer = convertBinToDec(PlayerLayer.value);
			//qualcosa per riporlarlo allo stato idle
		}
		
	}

}

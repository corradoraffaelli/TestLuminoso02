using UnityEngine;
using System.Collections;

[System.Serializable]
public class StateFSM : MonoBehaviour {

	protected GameObject myGameObject;

	public enum myStateName {
		Wander,
		Patrol,
		Chase,
		Attack,
		Flee,
		Stunned,
	}
	public myStateName state;

	protected string stateName;

	protected AIAgent agentScript;
	protected PlayerMovements playerScript;
	protected AIParameters par;
	
	public delegate void myStateInitialize(ref object ob);
	public myStateInitialize myInitialize;

	public delegate void myStateUpdate();
	public myStateUpdate myUpdate;

	public delegate object myStateFinalize();
	public myStateFinalize myFinalize;

	public delegate void myStateTransition(ref myStateName s);
	public myStateTransition []myTransitions;

	public delegate int myStateTransition1(ref object t);
	public myStateTransition []myTransitions1;

	public delegate void myStateHandleCollisionEnter(Collision2D c);
	public myStateHandleCollisionEnter myHandleCollisionEnter;

	public delegate void myStateHandleTriggerEnter(Collider2D c);
	public myStateHandleTriggerEnter myHandleTriggerEnter;




	//AIParameters properties

	Transform myTransform;
	
	protected Transform transform {
		
		get{ 
			if(myTransform==null)
				myTransform = myGameObject.transform;
			
			return myTransform;}
		
	}

	protected Rigidbody2D _rigidbody {
		get{ return par._rigidbody;}
		set{ par._rigidbody = value;}

	}

	protected BoxCollider2D _boxCollider {
		get{ return par._boxCollider;}
		set{ par._boxCollider = value;}

	}

	protected CircleCollider2D _circleCollider {
		get{ return par._circleCollider;}
		set{ par._circleCollider = value;}

	}

	protected GameObject _target {
		get{ return par._target;}
		set{ par._target = value;}

	}

	
	protected GameObject _fleeTarget {
		get{ return par._fleeTarget;}
		set{ par._fleeTarget = value;}
		
	}

	protected LayerMask targetLayers {
		get{ return par.targetLayers;}
		set{ par.targetLayers = value;}
	}
	protected LayerMask fleeLayer{
		get{ return par.fleeLayer;}
		set{ par.fleeLayer = value;}
	}
	protected LayerMask hidingLayer{
		get{ return par.hidingLayer;}
		set{ par.hidingLayer = value;}
	}
	protected LayerMask cloneLayer{
		get{ return par.cloneLayer;}
		set{ par.cloneLayer = value;}
	}
	protected LayerMask obstacleLayers{
		get{ return par.obstacleLayers;}
		set{ par.obstacleLayers = value;}
	}



	public StateFSM(GameObject gameo){

		//GameObject gameo = this.gameObject;

		myGameObject = gameo;

		agentScript = gameo.GetComponent<AIAgent> ();

		if (agentScript == null) {
			Debug.Log ("ATTENZIONE - script AIAgent non trovato");
		}

		playerScript = gameo.GetComponent<PlayerMovements> ();

		if (playerScript == null) {
			Debug.Log ("ATTENZIONE - script PlayerMovements non trovato");
		}

		par = gameo.GetComponent<AIParameters> ();
		
		if (par == null) {
			Debug.Log ("ATTENZIONE - script AIParameters non trovato");
		}

		
		myInitialize += initializeState;
		
		myUpdate += updateState;
		
		myFinalize += finalizeState;


	}


	protected virtual void initializeState(ref object ob) {
		Debug.Log ("entro in stato generico");
	}

	protected virtual void updateState() {
		Debug.Log ("stato generico");
	}

	protected virtual object finalizeState() {
		Debug.Log ("esco da stato generico");
		return null;
	}

	//-------------------------------------

	protected void i_move(float speed){

		//Debug.Log ("mi muovo" + (i_facingRight()? "destra" : "sinistra") );
		//TODO: da cambiare dentro playermovement il nome dei parametri...in particolare maxspeed
		playerScript.c_moveManagement(!i_facingRight(), i_facingRight(), speed);
	}

	protected bool i_facingRight() {

		return playerScript.FacingRight;

	}

	protected void i_flip() {

		playerScript.c_flip ();
	}

	protected void i_stunned(bool isStun) {
		
		playerScript.c_stunned (isStun);
		
		if (isStun == false) {
			
			par.myWeakPoint.SendMessage("c_setBouncy", true);
		}
		//gameObject.SendMessage ("c_setBouncy", true);
		
		
		
	}

	protected void wanderHandleCollisionEnter(Collision2D c) {
		
		if(par.DEBUG_COLLISION[0])
			Debug.Log ("COLL - entrato in collisione con " + c.gameObject.name);
		
		if (c.gameObject.tag != "Player") {
			Debug.Log("collisione! mi flippo!");
			i_flip();
			
		}
		
	}

	protected void moveTowardTarget(GameObject myTarget, float speed) {
		
		
		if (Mathf.Abs (myTarget.transform.position.y - transform.position.y) > 1.0f) {
			//diversa dalla mia altezza

			if (Mathf.Abs (myTarget.transform.position.x - transform.position.x) > 1.0f) {
				
				if (par.DEBUG_ASTAR [1])
					Debug.Log ("TARGET - target più alto di me e distante");
				
				i_move (speed * 0.85f);
			} else {
				
				if (par.DEBUG_ASTAR [1])
					Debug.Log ("TARGET - target più alto di me e sopra di me");
				
				i_move (speed * 0.7f);
			}
			
		} 
		else {
			//stessa mia altezza

			if (par.DEBUG_ASTAR [1])
				Debug.Log ("TARGET - target alla mia stessa altezza");
			
			moveCorrectVerse (myTarget, speed);

		}
	}

	protected void moveCorrectVerse(GameObject myTarget, float speed) {
		
		
		if( (myTarget.transform.position.x > transform.position.x ) && !i_facingRight()) {
			i_flip ();
		}
		
		if( (myTarget.transform.position.x < transform.position.x ) && i_facingRight()) {
			i_flip ();
		}

		i_move (speed);
		
	}
}

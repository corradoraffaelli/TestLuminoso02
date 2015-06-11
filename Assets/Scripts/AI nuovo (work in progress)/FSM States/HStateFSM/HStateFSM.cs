using UnityEngine;
using System.Collections;

public class HStateFSM : MonoBehaviour {

	#region VARIABLES
	protected GameObject myGameObject;
	
	protected string stateName;
	public string StateName {
		get{ return stateName;}
	}

	protected int stateId;
	public int StateID {
		get{ return stateId;}
		set{ stateId = value;}
	}

	protected int myHLevel = 0;
	public int HLevel {
		get{ return myHLevel;}

	}

	protected bool finalHLevel = false;
	public bool FinalHLevel {
		get{ return finalHLevel;}
		
	}

	protected HStateFSM fatherState;
	public HStateFSM FatherState {
		get{ return fatherState;}

	}

	//DEBUG
	public bool debugPlay = false;

	//sotto stati
	public HStateFSM []states;

	public int activeStateIndex = 0;
	public HStateFSM activeState;

	//OTHER SCRIPTS
	
	protected AIAgent1 agentScript;
	protected PlayerMovements playerScript;
	protected AIParameters par;

	#region QUICKOWNREF

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

	#endregion QUICKOWNREF

	protected GameObject _target {
		get{ return par._target;}
		set{ par._target = value;}
		
	}

	protected GameObject _fleeTarget {
		get{ return par._fleeTarget;}
		set{ par._fleeTarget = value;}
		
	}

	#region LAYERMASKS

	
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


	#endregion LAYERMASKS



	#endregion VARIABLES

	//DELEGATES
	#region DELEGATES
	//----

	public delegate void myHStateInitialize(ref object ob);
	private myHStateInitialize myHInitialize;
	public myHStateInitialize MyHInitialize {
		get{ return myHInitialize;}
	}

	public delegate void myStateInitialize(ref object ob);
	public myStateInitialize myInitialize;

	//----

	public delegate void myHStateUpdate();
	private myHStateUpdate myHUpdate;
	public myHStateUpdate MyHUpdate {
		get{ return myHUpdate;}
	}

	public delegate void myStateUpdate();
	public myStateUpdate myUpdate;

	//----

	public delegate object myHStateFinalize();
	private myStateFinalize myHFinalize;
	public myStateFinalize MyHFinalize {
		get{ return myHFinalize;}
	}

	public delegate object myStateFinalize();
	public myStateFinalize myFinalize;

	//-----

	//se ho stati sotto di me devo invocare questo
	public delegate int myHStateTransition(ref int id);
	private myHStateTransition myHTransition;
	public myHStateTransition MyHTransition {
		get{ return myHTransition;}
	}

	//se sono uno stato finale, invoco queste
	public delegate int myStateTransition(ref int id);
	public myStateTransition []myTransitions;

	private int indexTransitions = 0;
	public string[]targetStateNameTransitions;
	public int []targetStateIndexTransitions;

	//-----

	public delegate void myHStateHandleCollisionEnter(Collision2D c);
	public myHStateHandleCollisionEnter myHHandleCollisionEnter;

	public delegate void myStateHandleCollisionEnter(Collision2D c);
	public myStateHandleCollisionEnter myHandleCollisionEnter;


	//-----

	public delegate void myHStateHandleTriggerEnter(Collider2D c);
	public myHStateHandleTriggerEnter myHHandleTriggerEnter;

	public delegate void myStateHandleTriggerEnter(Collider2D c);
	public myStateHandleTriggerEnter myHandleTriggerEnter;

	#endregion DELEGATES
	//-----

	public HStateFSM() {
		stateName = "stato vuoto";
	}

	public HStateFSM(string _stateName, int _stateId, GameObject _gameo, int _hLevel, bool _finalHLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent){
		
		//GameObject gameo = this.gameObject;

		stateName = _stateName;

		stateId = _stateId;

		myGameObject = _gameo;

		myHLevel = _hLevel;

		finalHLevel = _finalHLevel;

		if (_fatherState == null)
			fatherState = _fatherState;

		//states = _states;

		agentScript = myGameObject.GetComponent<AIAgent1> ();



		//TODO : NOW
		agentScript = _scriptAIAgent;

		//if (agentScript == null) {
		//	Debug.Log ("ATTENZIONE - script AIAgent non trovato");
		//}
		
		playerScript = myGameObject.GetComponent<PlayerMovements> ();
		
		if (playerScript == null) {
			Debug.Log ("ATTENZIONE - script PlayerMovements non trovato");
		}
		
		par = myGameObject.GetComponent<AIParameters> ();
		
		if (par == null) {
			Debug.Log ("ATTENZIONE - script AIParameters non trovato");
		}
		
		
		myHInitialize += initializeHState;
		
		myHUpdate += updateHState;
		
		myHFinalize += finalizeHState;

		myHTransition += handleHTransition;

		myHHandleCollisionEnter += handleHEnCollision;

		myHHandleTriggerEnter += handleHEnTrigger;

		agentScript.statesMap.addState (this);

		/*
		myTransitions = new myStateTransition[1];
		myTransitions [0] += nomefunctrans;
		*/


		
	}

	public void setSubStates(HStateFSM []_states) {

		states = _states;

	}

	public int getSubStateIndex(HStateFSM state) {

		for(int i=0; i<states.Length; i++) {

			if(states[i]==state)
				return i;

		}

		return -1;

	}

	public void setActiveState(int i) {

		activeStateIndex = i;
		activeState = states [activeStateIndex];

	}

	//INITIALIZE
	//-----------------------------------------------------------------------------------------------------------------------------------------------

	protected virtual void initializeHState(ref object ob) {
		Debug.Log ("HFSM - entro in stato generico");

		if (finalHLevel = true) {
			
			if(myInitialize!=null)
				myInitialize(ref ob);
			else
				Debug.Log ("ATTENZIONE - HFSM - initialize dello stato " + StateName  + " è nulla ");
		} 
		else {
			
			if(states [activeStateIndex].myHInitialize != null)
				states [activeStateIndex].myHInitialize (ref ob);
			else
				Debug.Log ("ATTENZIONE - HFSM - initialize del sotto stato numero" + activeStateIndex + " : " + states[activeStateIndex].StateName + " è nulla ");
			
		}

	}

	//UPDATE
	//-----------------------------------------------------------------------------------------------------------------------------------------------

	protected virtual void updateHState() {
		Debug.Log ("HFSM - stato generico");

		if (finalHLevel = true) {
			
			if(myUpdate!=null)
				myUpdate();
			else
				Debug.Log ("ATTENZIONE - HFSM - update dello stato " + StateName  + " è nulla ");
		} 
		else {
			
			if(states [activeStateIndex].myHUpdate != null)
				states [activeStateIndex].myHUpdate ();
			else
				Debug.Log ("ATTENZIONE - HFSM - update del sotto stato numero" + activeStateIndex + " : " + states[activeStateIndex].StateName + " è nulla ");
			
		}


	}


	//FINALIZE
	//-----------------------------------------------------------------------------------------------------------------------------------------------

	protected virtual object finalizeHState() {
		Debug.Log ("HFSM - esco da stato generico");

		object ob = null;

		if (finalHLevel = true) {

			if(myFinalize!=null)
				ob = myFinalize();
			else
				Debug.Log ("ATTENZIONE - HFSM - finalize dello stato " + StateName  + " è nulla ");
		} 
		else {

			if(states [activeStateIndex].myHFinalize != null)
				ob = states [activeStateIndex].myHFinalize ();
			else
				Debug.Log ("ATTENZIONE - HFSM - finalize del sotto stato numero" + activeStateIndex + " : " + states[activeStateIndex].StateName + " è nulla ");

		}



		return ob;
	}

	//TRANSITIONS
	//-----------------------------------------------------------------------------------------------------------------------------------------------

	protected virtual int handleHTransition(ref int _id) {

		int result = -1;

		if (finalHLevel) {
			//siamo all'ultimo livello

			result = handleLastHLevel(ref _id);

			if(debugPlay)
				Debug.Log("-> result " + result + " e il ref id è " + _id);

		}
		else {
			//dobbiamo scavare ancora
			if(states [activeStateIndex].myHTransition!=null)
				result = states [activeStateIndex].myHTransition(ref _id);
			else
				Debug.Log ("ATTENZIONE - HFSM - finalize del sotto stato numero" + activeStateIndex + " : " + states[activeStateIndex].StateName + " è nulla ");

		}

		return result;

	}


	protected virtual int handleLastHLevel(ref int _id) {

		bool transitionDone = false;
		int result = -1;

		if (myTransitions == null) {
			//TODO: mettere debug
			Debug.Log ("ATTENZIONE - livello gerarchia " + myHLevel + ", NESSUNA TRANSIZIONE presente nello stato " + stateName);
			return result;
		}

		for (int tn=0; tn<myTransitions.Length; tn++) {

			if(myTransitions[tn] != null) {
				result = myTransitions[tn](ref _id);
				if(debugPlay)
					Debug.Log("-> -> result è " + result + " e ref id " + _id);

				if(result != -1) {
					Debug.Log("ciao1");
					//TODO: NEWVERSION
					if(result == -2) {
						Debug.Log("ciao2");
						if(targetStateIndexTransitions[tn]==-1) {
							targetStateIndexTransitions[tn] = getState(targetStateNameTransitions[tn]);
						}
						_id = targetStateIndexTransitions[tn];
						return targetStateIndexTransitions[tn];

					}

					return result;
				}

			}
			else {

				Debug.Log ("ATTENZIONE - livello gerarchia " + myHLevel + ", la func transition numero " + tn + " dello stato " + StateName + " è null");
			}
		}

		//dovrebbe essere -1, cioè nessuna transizione
		return result;

	}

	//COLLISION
	//-----------------------------------------------------------------------------------------------------------------------------------------------

	protected virtual void handleHEnCollision(Collision2D c) {

		if (finalHLevel = true) {

			if(myHandleCollisionEnter!=null)
				myHandleCollisionEnter(c);
			else
				Debug.Log ("ATTENZIONE - HFSM - enter collision dello stato " + StateName  + " è nulla ");
		} 
		else {
			
			if(states [activeStateIndex].myHHandleCollisionEnter != null)
				states [activeStateIndex].myHHandleCollisionEnter (c);
			else
				Debug.Log ("ATTENZIONE - HFSM - enter collision del sotto stato numero" + activeStateIndex + " : " + states[activeStateIndex].StateName + " è nulla ");
			
		}

	}

	//TRIGGER
	//-----------------------------------------------------------------------------------------------------------------------------------------------

	protected virtual void handleHEnTrigger(Collider2D c) {

		if (finalHLevel = true) {

			if(myHandleTriggerEnter!=null)
				myHandleTriggerEnter(c);
			else
				Debug.Log ("ATTENZIONE - HFSM - enter trigger dello stato " + StateName  + " è nulla ");
		} 
		else {
			
			if(states [activeStateIndex].myHHandleTriggerEnter != null)
				states [activeStateIndex].myHHandleTriggerEnter (c);
			else
				Debug.Log ("ATTENZIONE - HFSM - enter trigger del sotto stato numero" + activeStateIndex + " : " + states[activeStateIndex].StateName + " è nulla ");
			
		}

	}

	//USEFUL METHODS-------------------------------------
	
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
		
		if(debugPlay)
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

	private int getIndexState (string _stateName) {

		int ind = agentScript.statesMap.getStateIDByName (_stateName);
		if(debugPlay)
			Debug.Log ("stato " + _stateName + " preso");

		return ind;
		
	}

	protected int getState (string _stateName) {

		int output = getIndexState (_stateName);

		if (output == -1) {

			Debug.Log ("ATTENZIONE - stato " + _stateName + " rimasto a -1");
			
		}

		return output;

	}

	public void addTransition(myStateTransition _method, string _stateName) {


		allocateMyTransitions (indexTransitions + 1);

		myTransitions [indexTransitions] += _method;

		targetStateNameTransitions [indexTransitions] = _stateName;

		targetStateIndexTransitions [indexTransitions] = -1;

		indexTransitions++;

	}

	void allocateMyTransitions(int len) {

		if (len==1) {
			//first allocation
			myTransitions = new myStateTransition[len];
			targetStateIndexTransitions = new int[len];
			targetStateNameTransitions = new string[len];
			//firsta allocation index
		} 
		else {

			//reallocate
			myStateTransition []tempTrans = new myStateTransition[len];
			int []tempIndex = new int[len];
			string []tempString = new string[len];

			//reallocate transition
			int i = 0;
			foreach(myStateTransition tr in myTransitions) {

				tempTrans[i] = tr;
				i++;
			}

			i = 0;

			foreach(int tr in targetStateIndexTransitions) {
				
				tempIndex[i] = tr;
				i++;
			}

			i = 0;

			foreach(string tr in targetStateNameTransitions) {
				
				tempString[i] = tr;
				i++;
			}

			myTransitions = tempTrans;
			targetStateIndexTransitions = tempIndex;
			targetStateNameTransitions = tempString;


		}

	}

	public IEnumerator ciaosequenza() {
		Debug.Log ("ciao 1 CHASE --------------------------------");
		
		yield return new WaitForSeconds(1.0f);
		Debug.Log ("ciao 2 CHASE --------------------------------");
		
	}

}


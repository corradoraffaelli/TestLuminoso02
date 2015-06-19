﻿//#define _DEBUG

using UnityEngine;
using System.Collections;



public class HStateFSM {

	#region VARIABLES
	protected GameObject gameObject;

	protected int defaultLayer;
	protected int deadLayer;

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
	//public bool _DEBUG_PLAY = false;

	//sotto stati
	public HStateFSM []states;
	private int statesIndex = 0;

	//public int activeStateIndex = 0;
	public HStateFSM activeState;

	//OTHER SCRIPTS
	
	protected AIAgent1 agentScript;
	protected PlayerMovements playerScript;
	protected AIParameters par;
	protected StatusParameters statusPar;

	#region QUICKOWNREF

	Transform myTransform;
	
	protected Transform transform {
		
		get{ 
			if(myTransform==null)
				myTransform = gameObject.transform;
			
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

	//protected SpriteRenderer statusSpriteRenderer;

	//spriteRenderStatus

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
	public delegate bool myHStateTransition(ref HStateFSM _nextState);
	private myHStateTransition myHTransition;
	public myHStateTransition MyHTransition {
		get{ return myHTransition;}
	}

	//se sono uno stato finale, invoco queste
	public delegate bool myStateTransition();
	public myStateTransition []myTransitions;


	private int indexTransitions = 0;

	public HStateFSM []targetStateTransitions;
	//public string[]targetStateNameTransitions;
	//public int []targetStateIndexTransitions;

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

		gameObject = _gameo;

		myHLevel = _hLevel;

		finalHLevel = _finalHLevel;

		if (_fatherState == null)
			fatherState = _fatherState;

		//states = _states;

		agentScript = gameObject.GetComponent<AIAgent1> ();




		agentScript = _scriptAIAgent;

		defaultLayer = gameObject.layer;
		deadLayer = LayerMask.NameToLayer("");
		
		playerScript = gameObject.GetComponent<PlayerMovements> ();
		
		if (playerScript == null) {
			Debug.Log ("ATTENZIONE - script PlayerMovements non trovato");
		}
		
		par = gameObject.GetComponent<AIParameters> ();
		
		if (par == null) {
			Debug.Log ("ATTENZIONE - script AIParameters non trovato");
		}

		statusPar = par.statusParameters;
		
		myHInitialize += initializeHState;
		
		myHUpdate += updateHState;
		
		myHFinalize += finalizeHState;

		myHTransition += checkHierarchyTransitions;

		myHHandleCollisionEnter += handleHEnCollision;

		myHHandleTriggerEnter += handleHEnTrigger;

		//agentScript.statesMap.addState (this);

	}

	public void setSubStates(HStateFSM []_states) {

		states = _states;

	}

	public void addState(HStateFSM _hstate) {
		
		if (statesIndex == 0) {
			
			states = new HStateFSM[1];
			
		}
		else {
			reallocateHStates();
		}

		states [statesIndex] = _hstate;
		
		if (activeState == null) {
			Debug.Log("Dentro " + stateName + " setto come active state " + _hstate.StateName);
			
			activeState = _hstate;
		}
		
		statesIndex++;
		
	}
	
	void reallocateHStates() {
		
		HStateFSM [] tempStates = new HStateFSM[statesIndex + 1];
		
		int i = 0;
		foreach (HStateFSM hst in states) {
			
			tempStates[i] = hst;
			i++;
			
		}
		
		states = tempStates;
		
	}

	public int getSubStateIndex(HStateFSM state) {

		for(int i=0; i<states.Length; i++) {

			if(states[i]==state)
				return i;

		}

		return -1;

	}

	public void setActiveState(HStateFSM _state) {

		activeState = _state;

	}

	//INITIALIZE
	//-----------------------------------------------------------------------------------------------------------------------------------------------

	protected virtual void initializeHState(ref object ob) {
		Debug.Log ("HFSM - entro in stato generico");

		if (finalHLevel == true) {
			
			if(myInitialize!=null)
				myInitialize(ref ob);
			else
				Debug.Log ("ATTENZIONE - HFSM - initialize dello stato " + StateName  + " è nulla ");
		} 
		else {

			if(myInitialize!=null)
				myInitialize(ref ob);
			else
				Debug.Log ("N.B. - HFSM - initialize dello stato " + StateName  + " è nulla ");


			if(activeState.myHInitialize != null)
				activeState.myHInitialize (ref ob);
			else
				Debug.Log ("ATTENZIONE - HFSM - initialize del sotto stato " + activeState.StateName + " è nulla ");
			
		}

	}

	//UPDATE
	//-----------------------------------------------------------------------------------------------------------------------------------------------

	protected virtual void updateHState() {


		if (finalHLevel == true) {

			#if _DEBUG
				Debug.Log ("Update di " + stateName + " stato finale");
			#endif
				

			if(myUpdate!=null)
				myUpdate();
			else
				Debug.Log ("ATTENZIONE - HFSM - update dello stato " + StateName  + " è nulla ");
		} 
		else {

			#if _DEBUG
				Debug.Log ("Update di " + stateName + " stato NON finale");
			#endif
				

			if(myUpdate!=null)
				myUpdate();
			else {

				#if _DEBUG
					Debug.Log ("N.B. - HFSM - update dello stato " + StateName  + " è nulla ");
				#endif

			}

			if(activeState.myHUpdate != null) {
				activeState.myHUpdate ();
			}
			else {
				Debug.Log ("ATTENZIONE - HFSM - update del sotto stato " + activeState.StateName + " è nulla ");
			}
		}


	}


	//FINALIZE
	//-----------------------------------------------------------------------------------------------------------------------------------------------

	protected virtual object finalizeHState() {
		Debug.Log ("HFSM - esco da stato generico");

		object ob = null;

		if (finalHLevel == true) {

			if(myFinalize!=null)
				ob = myFinalize();
			else
				Debug.Log ("ATTENZIONE - HFSM - finalize dello stato " + StateName  + " è nulla ");
		} 
		else {

			if(myFinalize!=null)
				ob = myFinalize();
			else
				Debug.Log ("ATTENZIONE - HFSM - finalize dello stato " + StateName  + " è nulla ");

			if(activeState.myHFinalize != null)
				ob = activeState.myHFinalize ();
			else
				Debug.Log ("ATTENZIONE - HFSM - finalize del sotto stato " + activeState.StateName + " è nulla ");

		}



		return ob;
	}

	//TRANSITIONS
	//-----------------------------------------------------------------------------------------------------------------------------------------------

	protected virtual bool checkHierarchyTransitions(ref HStateFSM _nextState) {

		bool result = false;

		result = checkMyTransitions(ref _nextState);
		
		if(result != false)
			Debug.Log("-> result " + result + ", nextstate : " + _nextState + " post transition of " + StateName);

		if (!finalHLevel && result==false) {

			//se al mio livello non è stata rilevata nessuna transizione
			//dobbiamo scavare ancora e scoprire se nei livelli sottostanti serve fare una transizione

			if(activeState.myHTransition!=null)
				result = activeState.myHTransition(ref _nextState);
			else
				Debug.Log ("ATTENZIONE - HFSM - finalize del sotto stato " + activeState.StateName + " è nulla ");

		}

		//se ho rilevato una transizione, devo capire se devo occuparmene io o meno
		//in caso positivo, cioè lo stato finale è sotto uno dei miei, faccio la transizione e rimetto result a -1,
		//così che sopra di me non si accorgano di nulla
		//altrimenti, se non è di mia competenza, lascio result != -1 e se ne occuperà qualcuno sopra di me

		if (/*HLevel == 0 &&*/ !finalHLevel && result!=false  ) {

			//TODO: come fare per cambiare macro stato
			Debug.Log ("devo faare qualcosa? sono " + StateName + " e devo passare a " + _nextState.StateName);

			foreach(HStateFSM st in states) {

				if(st==_nextState) {
					//se appartiene a me, 
					makeHTransition(_nextState);
					
					result = false;

					break;

				}

			}



		}

		return result;

	}

	protected virtual bool checkMyTransitions(ref HStateFSM _nextState) {
		
		bool transitionDone = false;
		bool result = false;
		
		if (myTransitions == null) {
			//TODO: mettere debug
			Debug.Log ("ATTENZIONE - livello gerarchia " + myHLevel + ", NESSUNA TRANSIZIONE presente nello stato " + stateName);
			return result;
		}
		
		for (int tn=0; tn<myTransitions.Length; tn++) {
			
			if(myTransitions[tn] != null) {
				result = myTransitions[tn]();

				#if _DEBUG
					Debug.Log("-> -> result è " + result + " e ref id " + _nextState);
				#endif
					
				
				if(result != false) {

					_nextState = targetStateTransitions[tn];
					return true;
					/*
					bool isMyTransition = false;

					foreach(HStateFSM targetS in targetStateTransitions) {

						if(targetS==_nextState) {
							isMyTransition = true;
							break;
						}


					}

					if(isMyTransition) {

						makeHTransition( targetStateTransitions[tn] );
						return -1;

					}
					else {

						_nextState = targetStateTransitions[tn];
						return -2;

					}


					*/

				}
				
			}
			else {
				
				Debug.Log ("ATTENZIONE - livello gerarchia " + myHLevel + ", la func transition numero " + tn + " dello stato " + StateName + " è null");
			}
		}
		
		//dovrebbe essere -1, cioè nessuna transizione
		return result;
		
	}

	protected void makeHTransition(HStateFSM _nextState) {

		Debug.Log ("TRANSIZIONE INTERNA - sono " + stateName + " e passo da " + activeState.StateName + " a " + _nextState.StateName + "++++++++++++++++++++++++++");

		object ob = null;

		if (activeState == _nextState) {
			Debug.Log ("ATTENZIONE - stato destinazione uguale a stato attuale");
			return;
		}

		if(activeState.myFinalize!=null)
			ob = activeState.myFinalize ();

		activeState = _nextState;

		Debug.Log ("sto per inizializzare " + activeState.stateName);

		if(activeState.myInitialize!=null)
			activeState.myInitialize (ref ob);

	}

	/*

	protected virtual int handleLastHLevel(ref HStateFSM _nextState) {

		bool transitionDone = false;
		int result = -1;

		if (myTransitions == null) {
			//TODO: mettere debug
			Debug.Log ("ATTENZIONE - livello gerarchia " + myHLevel + ", NESSUNA TRANSIZIONE presente nello stato " + stateName);
			return result;
		}

		for (int tn=0; tn<myTransitions.Length; tn++) {

			if(myTransitions[tn] != null) {
				result = myTransitions[tn](ref _nextState);
				if(_DEBUG_PLAY)
					Debug.Log("-> -> result è " + result + " e ref id " + _nextState);

				if(result != -1) {
					Debug.Log("ciao1");
					//TODO: NEWVERSION
					if(result == -2) {
						Debug.Log("ciao2");
						if(targetStateIndexTransitions[tn]==-1) {
							targetStateIndexTransitions[tn] = getState(targetStateNameTransitions[tn]);
						}
						_nextState = targetStateIndexTransitions[tn];
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
	*/

	//COLLISION
	//-----------------------------------------------------------------------------------------------------------------------------------------------

	protected virtual void handleHEnCollision(Collision2D c) {

		if (finalHLevel == true) {

			if(myHandleCollisionEnter!=null)
				myHandleCollisionEnter(c);
			else
				Debug.Log ("ATTENZIONE - HFSM - enter collision dello stato " + StateName  + " è nulla ");
		} 
		else {

			if(myHandleCollisionEnter!=null)
				myHandleCollisionEnter(c);
			else
				Debug.Log ("N.B. - HFSM - enter collision dello stato " + StateName  + " è nulla ");

			if(activeState.myHHandleCollisionEnter != null)
				activeState.myHHandleCollisionEnter (c);
			else
				Debug.Log ("ATTENZIONE - HFSM - enter collision del sotto stato " + activeState.StateName + " è nulla ");
			
		}

	}

	//TRIGGER
	//-----------------------------------------------------------------------------------------------------------------------------------------------

	protected virtual void handleHEnTrigger(Collider2D c) {

		if (finalHLevel == true) {

			if(myHandleTriggerEnter!=null)
				myHandleTriggerEnter(c);
			else {
				#if _DEBUG
					Debug.Log ("ATTENZIONE - HFSM - enter trigger dello stato " + StateName  + " è nulla, triggato da " + c.name);
				#endif
			}
		} 
		else {

			if(myHandleTriggerEnter!=null)
				myHandleTriggerEnter(c);
			else {
				#if _DEBUG
					Debug.Log ("N.B. - HFSM - enter trigger dello stato " + StateName  + " è nulla, triggato da " + c.name);
				#endif
			}
			if(activeState.myHHandleTriggerEnter != null)
				activeState.myHHandleTriggerEnter (c);
			else {
				#if _DEBUG
					Debug.Log ("ATTENZIONE - HFSM - enter trigger del sotto stato " + activeState.StateName + " è nulla, triggato da " + c.name);
				#endif
			}
		}

	}

	//USEFUL METHODS-------------------------------------
	#region USEFULMETHODS
	
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

	protected void checkFlipNeedForCollision(Collision2D c) {
		
		#if _DEBUG
			Debug.Log ("COLL - entrato in collisione con " + c.gameObject.name);
		#endif
			
		
		if (c.gameObject.tag != "Player") {

			if(!isUnderMyFeet(c)) {

				#if _DEBUG
				Debug.Log("Collisione! mi flippo!");ame);
				#endif

				i_flip();
			
			}
		}
		
	}

	bool isUnderMyFeet(Collision2D c) {

		GameObject collisionObj = c.gameObject;

		BoxCollider2D bc = collisionObj.GetComponent<BoxCollider2D> ();

		ContactPoint2D []contactPoints =  c.contacts;

		bool underMyFeet = false;

		foreach (ContactPoint2D cp in contactPoints) {

			Debug.Log ("I'm at : x " + transform.position.x + " y " + transform.position.y + " and the contact point is at : x " + cp.point.x + " y " + cp.point.y);

			//TODO: dovrei prendere meglio le misure, in base alla larghezza del player, o meglio, del suo collider
			if( (cp.point.x - transform.position.x) > 0.3f)
				underMyFeet = false;
			else
				underMyFeet = true;

		}

		return underMyFeet;

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

	/*
	private int getIndexState (string _stateName) {

		int ind = agentScript.statesMap.getStateIDByName (_stateName);
		if(_DEBUG_PLAY)
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
	*/

	public void addTransition(myStateTransition _method, HStateFSM _state) {
		
		
		allocateMyTransitions (indexTransitions + 1);
		
		myTransitions [indexTransitions] += _method;
		
		targetStateTransitions [indexTransitions] = _state;

		indexTransitions++;
		
	}

	/*
	public void addTransition(myStateTransition _method, string _stateName) {


		allocateMyTransitions (indexTransitions + 1);

		myTransitions [indexTransitions] += _method;

		targetStateTransitions [indexTransitions] = _stateName;

		targetStateIndexTransitions [indexTransitions] = -1;

		indexTransitions++;

	}
	*/

	void allocateMyTransitions(int len) {
		
		if (len==1) {
			//first allocation
			myTransitions = new myStateTransition[len];
			targetStateTransitions = new HStateFSM[len];

		} 
		else {
			
			//reallocate
			myStateTransition []tempTrans = new myStateTransition[len];
			HStateFSM []tempStates = new HStateFSM[len];

			//reallocate transition
			int i = 0;
			foreach(myStateTransition tr in myTransitions) {
				
				tempTrans[i] = tr;
				i++;
			}
			
			i = 0;
			
			foreach(HStateFSM tr in targetStateTransitions) {
				
				tempStates[i] = tr;
				i++;
			}

			
			myTransitions = tempTrans;
			targetStateTransitions = tempStates;
			//targetStateNameTransitions = tempString;
			
			
		}
		
	}

	/*
	void allocateMyTransitions1(int len) {

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
	*/

	public IEnumerator ciaosequenza() {
		Debug.Log ("ciao 1 CHASE --------------------------------");
		
		yield return new WaitForSeconds(1.0f);
		Debug.Log ("ciao 2 CHASE --------------------------------");
		
	}

	protected Coroutine _StartCoroutine(IEnumerator method) {

		Coroutine co = agentScript.StartCoroutine (method);

		return co;

	}

	protected void _StopCoroutine(IEnumerator method) {

		agentScript.StopCoroutine (method);

	}

	protected void checkPlayerCollision(Collision2D co) {
		
		if (co.gameObject.tag == "Player") {
			//co.gameObject.transform.SendMessage ("c_stunned", true);
			co.gameObject.transform.SendMessage ("c_instantKill");
			Vector2 dist = co.gameObject.transform.position - transform.position;
			
			Rigidbody2D r = co.gameObject.GetComponent<Rigidbody2D>();
			r.velocity = new Vector2(0.0f, 0.0f);
			r.AddForce(300.0f*dist.normalized);
			//c_playerStunned(true);
			//return true;
		}
		else {
			
			//return false;
			
		}
	}

	protected bool getStatusSpriteRenderer(ref SpriteRenderer _statusSR) {

		GameObject statusImgObj = null;

		foreach (Transform child in transform) {

			if(child.gameObject.name=="StatusImg") {
				statusImgObj = child.gameObject;
			}

		}

		if (statusImgObj != null) {

			_statusSR = statusImgObj.GetComponent<SpriteRenderer>();
			return true;
		}

		return false;

	}

	protected void changeLayer(string layerName) {


	}

	#endregion USEFULMETHODS

}


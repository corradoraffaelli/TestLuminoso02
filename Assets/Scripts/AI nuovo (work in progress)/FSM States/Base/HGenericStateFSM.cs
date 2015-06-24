//#define _DEBUG

using UnityEngine;
using System.Collections;



public abstract class HGenericStateFSM {
	
	protected AIAgent1 agentScript;
	
	#region VARIABLES
	
	protected GameObject gameObject;

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
	
	protected HGenericStateFSM fatherState;
	public HGenericStateFSM FatherState {
		get{ return fatherState;}
		
	}

	protected ArrayList StackFinalizeMessages {
		
		get {
			if(agentScript!=null) {
				
				if(agentScript.stackFinalizeMessages!=null)
					return agentScript.stackFinalizeMessages;
				
			}
			
			return null;
			
		}
		
		set {
			if(agentScript!=null) {
				agentScript.stackFinalizeMessages = value;
			}
			
		}
		
	}
	
	//DEBUG
	//public bool _DEBUG_PLAY = false;
	
	//sotto stati
	public HGenericStateFSM []states;
	private int statesIndex = 0;
	
	//public int activeStateIndex = 0;
	public HGenericStateFSM activeState;
	
	//OTHER SCRIPTS
	
	
	
	#region QUICKOWNREF
	
	Transform myTransform;
	
	protected Transform transform {
		
		get{ 
			if(myTransform==null)
				myTransform = gameObject.transform;
			
			return myTransform;}
		
	}

	
	#endregion QUICKOWNREF
	
	//protected SpriteRenderer statusSpriteRenderer;
	
	//spriteRenderStatus

	
	
	
	#endregion VARIABLES
	
	//DELEGATES
	#region DELEGATES
	//----
	
	public delegate void myHStateInitialize();
	private myHStateInitialize myHInitialize;
	public myHStateInitialize MyHInitialize {
		get{ return myHInitialize;}
	}
	
	public delegate void myStateInitialize();
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
	
	public delegate void myHStateFinalize();
	private myStateFinalize myHFinalize;
	public myStateFinalize MyHFinalize {
		get{ return myHFinalize;}
	}
	
	public delegate void myStateFinalize();
	public myStateFinalize myFinalize;
	
	//-----
	
	//se ho stati sotto di me devo invocare questo
	public delegate bool myHStateTransition(ref HGenericStateFSM _nextState);
	private myHStateTransition myHTransition;
	public myHStateTransition MyHTransition {
		get{ return myHTransition;}
	}
	
	//se sono uno stato finale, invoco queste
	public delegate bool myStateTransition();
	public myStateTransition []myTransitions;
	
	
	private int indexTransitions = 0;
	
	public HGenericStateFSM []targetStateTransitions;
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
	
	public HGenericStateFSM(string _stateName, int _stateId, GameObject _gameo, int _hLevel, bool _finalHLevel, HGenericStateFSM _fatherState, AIAgent1 _scriptAIAgent){

		stateName = _stateName;
		
		stateId = _stateId;
		
		gameObject = _gameo;
		
		myHLevel = _hLevel;
		
		finalHLevel = _finalHLevel;
		
		if (_fatherState == null)
			fatherState = _fatherState;
		
		agentScript = _scriptAIAgent;
		
		myHInitialize += initializeHState;
		
		myHUpdate += updateHState;
		
		myHFinalize += finalizeHState;
		
		myHTransition += checkHierarchyTransitions;
		
		myHHandleCollisionEnter += handleHEnCollision;
		
		myHHandleTriggerEnter += handleHEnTrigger;

	}

	public HGenericStateFSM(string _stateName, GameObject _gameo, bool _finalHLevel, HGenericStateFSM _fatherState, AIAgent1 _scriptAIAgent){
		
		stateName = _stateName;
		
		//stateId = _stateId;
		
		gameObject = _gameo;
		
		//myHLevel = _hLevel;
		
		finalHLevel = _finalHLevel;
		
		if (_fatherState == null) {
			fatherState = null;
			myHLevel = 0;
		} 
		else {
			fatherState = _fatherState;
			myHLevel = _fatherState.HLevel + 1;
		}

		agentScript = _scriptAIAgent;
		
		myHInitialize += initializeHState;
		
		myHUpdate += updateHState;
		
		myHFinalize += finalizeHState;
		
		myHTransition += checkHierarchyTransitions;
		
		myHHandleCollisionEnter += handleHEnCollision;
		
		myHHandleTriggerEnter += handleHEnTrigger;
		
	}
	
	public void setSubStates(HGenericStateFSM []_states) {
		
		states = _states;
		
	}
	
	public void addState(HGenericStateFSM _hstate) {
		
		if (statesIndex == 0) {
			
			states = new HGenericStateFSM[1];
			
		}
		else {
			reallocateHStates();
		}
		
		states [statesIndex] = _hstate;
		
		if (activeState == null) {
			#if _DEBUG
			Debug.Log("Dentro " + stateName + " setto come active state " + _hstate.StateName);
			#endif			
			activeState = _hstate;
		}
		
		statesIndex++;
		
	}
	
	void reallocateHStates() {
		
		HGenericStateFSM [] tempStates = new HGenericStateFSM[statesIndex + 1];
		
		int i = 0;
		foreach (HGenericStateFSM hst in states) {
			
			tempStates[i] = hst;
			i++;
			
		}
		
		states = tempStates;
		
	}
	
	public int getSubStateIndex(HGenericStateFSM state) {
		
		for(int i=0; i<states.Length; i++) {
			
			if(states[i]==state)
				return i;
			
		}
		
		return -1;
		
	}
	
	public void setActiveState(HGenericStateFSM _state) {
		
		bool found = false;
		
		foreach (HGenericStateFSM st in states) {
			
			if(_state==st)
				found = true;
		}
		
		if (found)
			activeState = _state;
		else {
			#if _WARNING_DEBUG
			Debug.Log("ATTENZIONE - tentativo di settare come stato attivo uno stato non figlio");
			#endif
		}
	}
	
	//INITIALIZE
	//-----------------------------------------------------------------------------------------------------------------------------------------------
	
	protected virtual void initializeHState() {
		#if _DEBUG
		Debug.Log ("HFSM - entro in stato generico");
		#endif
		
		
		if (finalHLevel == true) {
			
			if(myInitialize!=null) {
				myInitialize();
			}
			else{
				#if _WARNING_DEBUG
				Debug.Log ("ATTENZIONE - HFSM - initialize dello stato " + StateName  + " è nulla ");
				#endif
				
			}
			
			emptyFinalizeMessages();
			
		} 
		else {
			
			//first initialize father state, because it could need to set which child will be the active one
			if(myInitialize!=null){
				myInitialize();
			}
			else{
				Debug.Log ("N.B. - HFSM - initialize dello stato " + StateName  + " è nulla ");
			}
			
			if(activeState.myHInitialize != null) {
				activeState.myHInitialize ();
			}
			else{
				#if _WARNING_DEBUG
				Debug.Log ("ATTENZIONE - HFSM - initialize del sotto stato " + activeState.StateName + " è nulla ");
				#endif
				
			}
			
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
			else{
				#if _DEBUG
				Debug.Log ("ATTENZIONE - HFSM - update dello stato " + StateName  + " è nulla ");
				#endif
			}
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
				#if _WARNING_DEBUG
				Debug.Log ("ATTENZIONE - HFSM - update del sotto stato " + activeState.StateName + " è nulla ");
				#endif
				
			}
		}
		
		
	}
	
	
	//FINALIZE
	//-----------------------------------------------------------------------------------------------------------------------------------------------
	
	protected virtual void finalizeHState() {
		#if _DEBUG
		Debug.Log ("HFSM - esco da stato generico");
		#endif
		
		if (finalHLevel == true) {
			
			if(myFinalize!=null) {

				myFinalize();
			}
			else {
				#if _WARNING_DEBUG
				Debug.Log ("ATTENZIONE - HFSM - finalize dello stato " + StateName  + " è nulla ");
				#endif

			}
			
		} 
		else {

			if(activeState.myHFinalize != null) {

				activeState.myHFinalize ();
			}
			else {
				#if _WARNING_DEBUG
				Debug.Log ("ATTENZIONE - HFSM - finalize del sotto stato " + activeState.StateName + " è nulla ");
				#endif
				
			}

			if(myFinalize!=null) {

				myFinalize();

			}
			else {
				#if _WARNING_DEBUG
				Debug.Log ("ATTENZIONE - HFSM - finalize dello stato " + StateName  + " è nulla ");
				#endif
				
			}
			

		}
		
		
	}
	
	//TRANSITIONS
	//-----------------------------------------------------------------------------------------------------------------------------------------------
	
	protected virtual bool checkHierarchyTransitions(ref HGenericStateFSM _nextState) {
		
		bool result = false;
		
		result = checkMyTransitions(ref _nextState);
		
		//TODO : e qui?
		//if(result != false)
		//Debug.Log("-> result " + result + ", nextstate : " + _nextState + " post transition of " + StateName);
		
		if (!finalHLevel && result==false) {
			
			//se al mio livello non è stata rilevata nessuna transizione
			//dobbiamo scavare ancora e scoprire se nei livelli sottostanti serve fare una transizione
			
			if(activeState.myHTransition!=null)
				result = activeState.myHTransition(ref _nextState);
			else {
				#if _WARNING_DEBUG
				Debug.Log ("ATTENZIONE - HFSM - finalize del sotto stato " + activeState.StateName + " è nulla ");
				#endif
				
			}
		}
		
		//se ho rilevato una transizione, devo capire se devo occuparmene io o meno
		//in caso positivo, cioè lo stato finale è sotto uno dei miei, faccio la transizione e rimetto result a -1,
		//così che sopra di me non si accorgano di nulla
		//altrimenti, se non è di mia competenza, lascio result != -1 e se ne occuperà qualcuno sopra di me
		
		if (/*HLevel == 0 &&*/ !finalHLevel && result!=false  ) {
			
			//TODO: come fare per cambiare macro stato
			//Debug.Log ("devo faare qualcosa? sono " + StateName + " e devo passare a " + _nextState.StateName);
			
			foreach(HGenericStateFSM st in states) {
				
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
	
	protected virtual bool checkMyTransitions(ref HGenericStateFSM _nextState) {
		
		bool transitionDone = false;
		bool result = false;
		
		if (myTransitions == null) {
			#if _DEBUG
			Debug.Log ("ATTENZIONE - livello gerarchia " + myHLevel + ", NESSUNA TRANSIZIONE presente nello stato " + stateName);
			#endif
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

					foreach(HGenericStateFSM targetS in targetStateTransitions) {

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
				#if _WARNING_DEBUG
				Debug.Log ("ATTENZIONE - livello gerarchia " + myHLevel + ", la func transition numero " + tn + " dello stato " + StateName + " è null");
				#endif
				
			}
		}
		
		//dovrebbe essere -1, cioè nessuna transizione
		return result;
		
	}
	
	protected void makeHTransition(HGenericStateFSM _nextState) {
		
		#if _DEBUG
		Debug.Log ("TRANSIZIONE INTERNA - sono " + stateName + " e passo da " + activeState.StateName + " a " + _nextState.StateName + "++++++++++++++++++++++++++");
		#endif
		
		if (activeState == _nextState) {
			
			#if _WARNING_DEBUG
			Debug.Log ("ATTENZIONE - stato destinazione uguale a stato attuale");
			#endif
			
			return;
		}
		
		if(activeState.myFinalize!=null)
			activeState.myFinalize ();
		
		activeState = _nextState;
		
		#if _DEBUG
		Debug.Log ("sto per inizializzare " + activeState.stateName);
		#endif
		
		
		if(activeState.myInitialize!=null)
			activeState.myInitialize ();
		
	}

	
	//COLLISION
	//-----------------------------------------------------------------------------------------------------------------------------------------------
	
	protected virtual void handleHEnCollision(Collision2D c) {
		
		if (finalHLevel == true) {

			//Debug.Log("sono " + gameObject +" e ho colliso con " + c.gameObject.name);

			if(myHandleCollisionEnter!=null) {
				myHandleCollisionEnter(c);
			}
			else {
				#if _WARNING_DEBUG
				Debug.Log ("ATTENZIONE - HFSM - enter collision dello stato " + StateName  + " è nulla ");
				#endif
			}
		} 
		else {
			
			if(myHandleCollisionEnter!=null)
				myHandleCollisionEnter(c);
			else
				Debug.Log ("N.B. - HFSM - enter collision dello stato " + StateName  + " è nulla ");
			
			if(activeState.myHHandleCollisionEnter != null)
				activeState.myHHandleCollisionEnter (c);
			else {
				#if _WARNING_DEBUG
				Debug.Log ("ATTENZIONE - HFSM - enter collision del sotto stato " + activeState.StateName + " è nulla ");
				#endif
				
			}
		}
		
	}
	
	//TRIGGER
	//-----------------------------------------------------------------------------------------------------------------------------------------------
	
	protected virtual void handleHEnTrigger(Collider2D c) {
		
		if (finalHLevel == true) {
			
			if(myHandleTriggerEnter!=null)
				myHandleTriggerEnter(c);
			else {
				#if _WARNING_DEBUG
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
				#if _WARNING_DEBUG
				Debug.Log ("ATTENZIONE - HFSM - enter trigger del sotto stato " + activeState.StateName + " è nulla, triggato da " + c.name);
				#endif
			}
		}
		
	}
	
	//USEFUL METHODS-------------------------------------
	#region USEFULMETHODS





	
	public void addTransition(myStateTransition _method, HGenericStateFSM _state) {
		
		
		allocateMyTransitions (indexTransitions + 1);
		
		myTransitions [indexTransitions] += _method;
		
		targetStateTransitions [indexTransitions] = _state;
		
		indexTransitions++;
		
	}
	
	void allocateMyTransitions(int len) {
		
		if (len==1) {
			//first allocation
			myTransitions = new myStateTransition[len];
			targetStateTransitions = new HGenericStateFSM[len];
			
		} 
		else {
			
			//reallocate
			myStateTransition []tempTrans = new myStateTransition[len];
			HGenericStateFSM []tempStates = new HGenericStateFSM[len];
			
			//reallocate transition
			int i = 0;
			foreach(myStateTransition tr in myTransitions) {
				
				tempTrans[i] = tr;
				i++;
			}
			
			i = 0;
			
			foreach(HGenericStateFSM tr in targetStateTransitions) {
				
				tempStates[i] = tr;
				i++;
			}
			
			
			myTransitions = tempTrans;
			targetStateTransitions = tempStates;
			//targetStateNameTransitions = tempString;
			
			
		}
		
	}

	
	protected Coroutine _StartCoroutine(IEnumerator method) {
		
		Coroutine co = agentScript.StartCoroutine (method);
		
		return co;
		
	}
	
	protected void _StopCoroutine(IEnumerator method) {
		#if _DEBUG
		Debug.Log ("stop coroutine");
		#endif
		agentScript.StopCoroutine (method);
		
	}

	protected void addFinalizeMessage(MessageFSM message) {
		if (StackFinalizeMessages == null)
			Debug.Log ("La coda dei messaggi è null");
		
		if(message==null)
			Debug.Log ("Il messaggio da inserire nella coda è null");
		
		if(message!=null && StackFinalizeMessages!=null)
			StackFinalizeMessages.Add (message);
		
	}
	
	protected ArrayList takeFinalizeMessages() {
		
		ArrayList tempArray = new ArrayList ();
		
		foreach (object ob in StackFinalizeMessages) {
			
			tempArray.Add(ob);
		}
		//Debug.Log ("presi i messaggi");
		return tempArray;
		
	}
	
	protected T[] takeFinalizeMessages <T> () {
		
		if (StackFinalizeMessages != null) {
			if(StackFinalizeMessages.Count>0) {
				
				return (T[]) StackFinalizeMessages.ToArray(typeof(T));
				
			}
			
		}
		
		return null;
	}
	
	protected void emptyFinalizeMessages() {
		
		StackFinalizeMessages.RemoveRange(0, StackFinalizeMessages.Count);
		//Debug.Log ("svuotati i messaggi");
	}
	
	
	
	#endregion USEFULMETHODS
	
}


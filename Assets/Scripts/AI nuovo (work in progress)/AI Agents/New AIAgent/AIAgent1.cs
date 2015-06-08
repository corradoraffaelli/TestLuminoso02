using UnityEngine;
using System.Collections;

public class AIAgent1 : MonoBehaviour {
	
	//da eliminare

	//[HideInInspector]
	//public StateFSM.myStateName []stati;
	
	//protected string []nomiStati;

	//public StateFSM.myStateName activeStateName;
	//[HideInInspector]

	//da tenere...

	public provandosi PROVA = new provandosi ("cacca");

	public HStateFSM statoprova = new HStateFSM();

	public HStateFSM []hstates;

	public HStateRecords statesMap;
	
	public HStateFSM activeState;
	
	protected int activeHStateIndex = 0;
	protected int prevActiveStateIndex = 0;
	
	protected AIParameters par;
	
	//DEBUG

	bool debugPlay = false;


	//AI Parameters
	
	Transform myTransform;
	
	protected Transform transform {
		
		get{ 
			if(myTransform==null)
				myTransform = transform;
			
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
	
	// Use this for initialization
	protected virtual void Start () {
		
		
		par = GetComponent<AIParameters> ();
		
		if(par == null)
			Debug.Log ("ATTENZIONE - non trovato par da aiagent");

		initializeHStates ();
		setStartState ();

		initializeConditions ();
		
	}
	
	protected virtual void setStartState() {
		
		//activeStateName = StateFSM.myStateName.Patrol;
		
	}
	
	protected virtual void initializeConditions() {
		
	}

	protected virtual void initializeHStates() {

		//hstates = new HStateFSM[stati.Length];
		
		//int count = 0;
		/*
		foreach (StateFSM.myStateName st in stati) {
			
			switch(st) {
				
			case StateFSM.myStateName.Patrol :

				//HStateFSM pHFSM = new HStateFSM
				PatrolFSM pFSM = new PatrolFSM(this.gameObject, PatrolFSM.patrolSubState.Walk);
				states[count] = pFSM;
				
				break;
				
			case StateFSM.myStateName.Chase :
				
				ChaseFSM cFSM = new ChaseFSM(this.gameObject);
				states[count] = cFSM;
				
				break;
				
			case StateFSM.myStateName.Wander :
				
				WanderFSM wFSM = new WanderFSM(this.gameObject);
				states[count] = wFSM;
				
				break;
				
			case StateFSM.myStateName.Stunned :
				StunnedFSM sFSM = new StunnedFSM(this.gameObject);
				states[count] = sFSM;
				
				break;
				
			}
			
			if(activeStateName == st) {
				activeHStateIndex = count;
			}
			
			count++;
			
		}
*/
	}

	protected virtual void initializeStates(){
		
		//esempio di initialize...
		//if(stati.Length==0)
		/*
		states = new StateFSM[stati.Length];
		
		int count = 0;
		
		foreach (StateFSM.myStateName st in stati) {
			
			switch(st) {
				
			case StateFSM.myStateName.Patrol :
				
				PatrolFSM pFSM = new PatrolFSM(this.gameObject, PatrolFSM.patrolSubState.Walk);
				states[count] = pFSM;
				
				break;
				
			case StateFSM.myStateName.Chase :
				
				ChaseFSM cFSM = new ChaseFSM(this.gameObject);
				states[count] = cFSM;
				
				break;
				
			case StateFSM.myStateName.Wander :
				
				WanderFSM wFSM = new WanderFSM(this.gameObject);
				states[count] = wFSM;
				
				break;
				
			case StateFSM.myStateName.Stunned :
				StunnedFSM sFSM = new StunnedFSM(this.gameObject);
				states[count] = sFSM;
				
				break;
				
			}
			
			if(activeStateName == st) {
				activeHStateIndex = count;
			}
			
			count++;
			
		}
		*/
	}
	
	// Update is called once per frame
	protected virtual void Update () {

		if (!PlayStatusTracker.inPlay)
			return;

		int nextState = activeState.StateID;
		
		if (hstates [activeHStateIndex].myHTransition != null) {

			int result = hstates [activeHStateIndex].myHTransition(ref nextState);

			if(debugPlay)
				Debug.Log(" > result " + result + " ref id " + nextState);

			if(result != -1 ) {
				if(nextState != activeState.StateID) {

					if(nextState != -1) {
						if(debugPlay)
							Debug.Log ("TRANSITION - transizione a ref id " + nextState);
						makeTransition(nextState);

					}
					else {
						Debug.Log ("ATTENZIONE - CASO NON PREVISTO, NEXTSTATE = -1");
					}
				}
				else {

					Debug.Log ("ATTENZIONE - lo stato destinazione è uguale a quello attuale" + activeState.StateName);

				}
			}

		}

		hstates[activeHStateIndex].myUpdate ();
		
	}

	protected virtual void makeTransition(int _targetStateID) {

		HStateFSM targetState = statesMap.getStateByID (_targetStateID);

		/*
		if (targetState == null) {
			Debug.Log ("ATTENZIONE - target state non trovato fra i records");
			return;
		}
		*/

		object ob = null;

		int [] hierarchy;

		if(activeState.myFinalize != null) {
			ob = activeState.myFinalize();
		}

		hierarchy = getHierarchyIndexes (targetState);
		activeHStateIndex = hierarchy [hierarchy.Length - 1];



		if (activeHStateIndex != -1) {

			setHStatesActiveIndexes(hierarchy);

			activeState = hstates[activeHStateIndex];

			if (targetState.myInitialize != null) {

				targetState.myInitialize (ref ob);

			}


		} 
		else {

			Debug.Log ("ATTENZIONE - GRAVE ERRORE NELL'INDICIZZAZIONE DEGLI HSTATE FATHER");
		
		}



	}

	public void setHStatesActiveIndexes(int[] hierarchy) {

		for(int i=0; i<hierarchy.Length; i++) {

			HStateFSM tempState = statesMap.getStateByID(hierarchy[i]);

			if(!tempState.FinalHLevel) {

				HStateFSM tempChildState = statesMap.getStateByID(hierarchy[i+1]);

				int index = tempState.getSubStateIndex(tempChildState);

				if(index != -1) {

					tempState.setActiveState(index);

				}
				else {

					Debug.Log("ATTENZIONE - SOTTO STATO NON TROVATO");

				}

			}

		}

	}

	public int[] getHierarchyIndexes(HStateFSM _state) {

		HStateFSM father;
		HStateFSM temp = _state;
		ArrayList alTemp = new ArrayList();
		int[] hierarchy;

		alTemp.Add (_state.StateID);

		while (true) {

			if(temp.HLevel == 0) {
				father = temp;
				alTemp.Add (temp.StateID);
				break;
			}
			else {
				temp = temp.FatherState;
				alTemp.Add (temp.StateID);
			}

		}

		hierarchy = new int[alTemp.Count];

		for (int t=0; t< alTemp.Count; t++) {

			//scrivo la gerarchia al contrario, dal più alto al più basso
			hierarchy[t] = (int) alTemp[alTemp.Count - t - 1];

		}

		return hierarchy;

		/*
		for (int t=0; t<hstates.Length; t++) {

			if(hstates[t].StateID==father.StateID) {

				return t;

			}

		}
		return -1;
		*/


	}

	

	
	public void setStunned(bool st) {

		par.stunnedReceived = st;
		
		//if (eType == enemyType.Guard)
		//	this.involveEType = true;
	}
	
	public void handleClean() {
		
		if (par.Spawner != null) {
			
			par.Spawner.SendMessage("letsSpawn");
			//Debug.Log("spawn - enemy");
			
		}
		
	}

	protected virtual void OnCollisionEnter2D(Collision2D c) {
		
		if(hstates [activeHStateIndex].myHHandleCollisionEnter!=null)
			hstates [activeHStateIndex].myHHandleCollisionEnter (c);
		
	}

	public virtual void OnTriggerEnter2D(Collider2D c) {
		
		if (c.tag == "Cleaner") {
			handleClean();
			Destroy (this.gameObject);
			return;
		}
		
		if(hstates [activeHStateIndex].myHHandleTriggerEnter!=null)
			hstates [activeHStateIndex].myHHandleTriggerEnter (c);
	}


}

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------

public class HStateRecords {

	public HStateFSM []mappedStates;
	int index = 0;

	public HStateRecords() {
		//TODO: da ottimizzare
		mappedStates = new HStateFSM[20];
	}

	public void addState(HStateFSM state) {

		if (index >= mappedStates.Length) {

			reallocateMappedStates();

		}
		Debug.Log ("aggiungo lo stato " + state.StateName + " all'indice " + index);
		mappedStates [index] = state;

		Debug.Log ("ho aggiunto lo stato " + mappedStates [index].StateName + " all'indice " + index);

		index++;

	}

	void reallocateMappedStates(){

	}

	public HStateFSM getStateByID(int _id) {

		if (_id == -1 || _id > index) {
			Debug.Log ("ATTENZIONE - tentativo di accesso errato alla mappa degli stati");
			return null;
		}

		//per ora non posso fare questo controllo
		/*
		if (mappedStates [_id] != null)
			return mappedStates [_id];
		else {
			Debug.Log ("ATTENZIONE - mappedStates ha valore nullo all'indice " + _id);
			return null;
		}
		*/

		
		return mappedStates [_id];

	}

	public HStateFSM getStateByName(string _stateName) {

		for(int i=0; i<mappedStates.Length; i++) {
			//TODO: da ottimizzare
			if(i>=index)
				break;

			if(_stateName == mappedStates[i].StateName) {
				
				return mappedStates[i];
				
			}
			
		}

		return null;

	}

	public int getStateIDByName(string _stateName) {

		for(int i=0; i<mappedStates.Length; i++) {
			//TODO: da ottimizzare
			if(i>=index)
				break;

			if(_stateName == mappedStates[i].StateName) {

				return mappedStates[i].StateID;

			}

		}

		return -1;

	}

}

public class provandosi {

	public string ciao;

	public provandosi (string ci) {
		ciao = ci;

	}
}

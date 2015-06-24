using UnityEngine;
using System.Collections;

public abstract class AIAgent1 : MonoBehaviour {

	#region LAYERMASKS

	private HGenericStateFSM []hstates;

	private int hstatesIndex = 0;
	
	public HGenericStateFSM activeState;
	public HGenericStateFSM previousState;
	
	protected AIParameters par;

	public ArrayList stackFinalizeMessages = new ArrayList();

	public GameObject Spawner {
		get{
			if (par != null) {
				if (par.Spawner != null) {
					return par.Spawner;
				}
			}

			return null;
		}
		set {
			if (par != null) {
				par.Spawner = value;

			}

		}
	}
	//DEBUG

	bool debugPlay = false;


	//AI Parameters

	#region QUICKOWNREF

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

	#endregion QUICKOWNREF

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
	protected LayerMask obstacleLayers{
		get{ return par.obstacleLayers;}
		set{ par.obstacleLayers = value;}
	}

	#endregion LAYERMASKS

	#endregion VARIABLES
	
	// Use this for initialization
	protected virtual void Start () {
		
		
		par = GetComponent<AIParameters> ();
		
		if(par == null)
			Debug.Log ("ATTENZIONE - non trovato par da aiagent");

		initializeHStates ();
		/*
		setStartState ();
		initializeConditions ();
		*/
		
		activeState.MyHInitialize ();
		
	}

	protected abstract void initializeHStates ();

	protected virtual void Update () {

		//Debug.Log ("stato attivo : " + activeState.StateName);

		if (!PlayStatusTracker.inPlay)
			return;
		
		HGenericStateFSM _nextState = activeState;
		
		if (activeState.MyHTransition != null) {
			
			bool result = false;
			
			result = activeState.MyHTransition(ref _nextState);
			
			if(debugPlay)
				Debug.Log(" > result " + result + " ref id " + _nextState);
			
			if(result != false ) {

				bool found = false;

				foreach(HGenericStateFSM st in hstates) {

					if(st == _nextState) {
						found = true;
						break;
					}
				}


				makeTransition(_nextState);
					
					

			}
			
		}
		
		activeState.MyHUpdate ();
		
	}
	
	/*
	protected virtual void Update2 () {

		if (!PlayStatusTracker.inPlay)
			return;

		int nextState = activeState.StateID;
		
		if (activeState.MyHTransition != null) {

			int result = -1;

			nextState = activeState.MyHTransition(ref result);

			if(debugPlay)
				Debug.Log(" > result " + result + " ref id " + nextState);

			if(nextState != -1 ) {

				if(nextState != activeState.StateID) {


					if(debugPlay)
						Debug.Log ("TRANSITION - transizione a ref id " + nextState);

					makeTransition(nextState);

					
				}
				else {

					Debug.Log ("ATTENZIONE - lo stato destinazione (indice " + nextState + ") è uguale a quello attuale" + activeState.StateName);

				}
			}

		}

		activeState.myUpdate ();
		
	}
	*/

	/*
	protected virtual void Update1 () {
		
		if (!PlayStatusTracker.inPlay)
			return;
		
		int nextState = activeState.StateID;
		
		if (hstates [activeHStateIndex].MyHTransition != null) {
			
			int result = -1;
			
			nextState = hstates [activeHStateIndex].MyHTransition(ref result);
			
			if(debugPlay)
				Debug.Log(" > result " + result + " ref id " + nextState);
			
			if(nextState != -1 ) {
				
				if(nextState != activeState.StateID) {
					
					
					if(debugPlay)
						Debug.Log ("TRANSITION - transizione a ref id " + nextState);
					
					makeTransition(nextState);
					
					
				}
				else {
					
					Debug.Log ("ATTENZIONE - lo stato destinazione (indice " + nextState + ") è uguale a quello attuale" + activeState.StateName);
					
				}
			}
			
		}
		
		hstates[activeHStateIndex].myUpdate ();
		
	}
	*/

	protected virtual void makeTransition(HGenericStateFSM _targetState) {
		
		object ob = null;

		if (activeState == _targetState) {

			Debug.Log ("ATTENZIONE - stato destinazione uguale a stato attuale");
			return;

		}

		if(activeState.MyHFinalize != null) {
			//ob = activeState.MyHFinalize();
			activeState.MyHFinalize();
		}

		activeState = _targetState;

		if (activeState.MyHInitialize != null) {
			
			activeState.MyHInitialize ();
			
		}

		
	}

	/*
	protected virtual void makeTransition1(int _targetStateID) {

		HGenericStateFSM targetState = statesMap.getStateByID (_targetStateID);



		object ob = null;

		int [] hierarchy;
		int nextStateInd = -1;
		if(activeState.myFinalize != null) {
			ob = activeState.myFinalize();
		}

		hierarchy = getHierarchyIndexes (targetState);
		nextStateInd = hierarchy [hierarchy.Length - 1];



		if (nextStateInd != -1) {

			setHStatesActiveIndexes(hierarchy);

			activeState = hstates[nextStateInd];

			if (targetState.myInitialize != null) {

				targetState.myInitialize (ref ob);

			}


		} 
		else {

			Debug.Log ("ATTENZIONE - GRAVE ERRORE NELL'INDICIZZAZIONE DEGLI HSTATE FATHER");
		
		}



	}
	*/

	public void addState(HGenericStateFSM _hstate) {

		if (hstatesIndex == 0) {

			hstates = new HGenericStateFSM[1];

		}
		else {
			reallocateHStates();
		}

		hstates [hstatesIndex] = _hstate;

		if (activeState == null) {
			#if _DEBUG
				Debug.Log("Dentro AIAgent setto come active state " + _hstate.StateName);
			#endif
			activeState = _hstate;
		}

		hstatesIndex++;

	}

	void reallocateHStates() {

		HGenericStateFSM [] tempStates = new HGenericStateFSM[hstatesIndex + 1];

		int i = 0;
		foreach (HGenericStateFSM hst in hstates) {

			tempStates[i] = hst;
			i++;

		}

		hstates = tempStates;

	}

	public bool setActiveState(string _stateName) {

		for (int i=0; i<hstates.Length; i++) {
			
			if(hstates[i].StateName == _stateName) {
				
				activeState = hstates[i];
				
				//activeHStateIndex = i;
				
				return true;
			}
			
		}
		
		return false;

	}

	public bool setActiveState(HGenericStateFSM _state) {

		for (int i=0; i<hstates.Length; i++) {
			
			if(hstates[i] == _state) {
				
				activeState = hstates[i];
				
				return true;
			}
			
		}

		return false;

	}

	/*
	public void setHStatesActiveIndexes(int[] hierarchy) {

		for(int i=0; i<hierarchy.Length; i++) {

			HGenericStateFSM tempState = statesMap.getStateByID(hierarchy[i]);

			if(!tempState.FinalHLevel) {

				HGenericStateFSM tempChildState = statesMap.getStateByID(hierarchy[i+1]);

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
	*/

	/*
	public int[] getHierarchyIndexes(HGenericStateFSM _state) {

		HGenericStateFSM father;
		HGenericStateFSM temp = _state;
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




	}
	*/
	

	
	public void setStunned(bool st) {

		par.stunnedReceived = st;

	}

	private void checkForSpawner() {
		
		if (Spawner != null) {
			
			Spawner.SendMessage("letsSpawn");

		}
	}

	protected virtual void OnCollisionEnter2D(Collision2D c) {
		
		if(activeState.myHHandleCollisionEnter!=null)
			activeState.myHHandleCollisionEnter (c);
		
	}

	public virtual void OnTriggerEnter2D(Collider2D c) {

		//TODO : valutare se meglio mettere qui l'iniziativa di spawn o meno
		/*
		if (c.tag == "Cleaner") {
			checkForSpawner();
			Destroy (this.gameObject);
			return;
		}
		*/
		if(activeState.myHHandleTriggerEnter!=null)
			activeState.myHHandleTriggerEnter (c);
	}

	public void c_autoDestroy() {
		
		checkForSpawner ();

		Destroy (this.gameObject);

		//StartCoroutine(handleDestroy(timer));
		
	}

	
	private IEnumerator handleLateDestroy(float timer) {
		
		yield return new WaitForSeconds(timer);
		Destroy (this.gameObject);
		
	}

	protected void addFinalizeMessage(MessageFSM message) {
		
		stackFinalizeMessages.Add (message);
		
	}
	
	protected ArrayList takeFinalizeMessages() {
		
		ArrayList tempArray = new ArrayList ();
		
		foreach (object ob in stackFinalizeMessages) {
			
			tempArray.Add(ob);
		}
		
		return tempArray;
		
	}
	
	protected void emptyFinalizeMessages() {
		
		stackFinalizeMessages.RemoveRange(0, stackFinalizeMessages.Count);
		Debug.Log ("svuotati i messaggi");
	}



}

//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------

public class HStateRecords {

	public HGenericStateFSM []mappedStates;
	int index = 0;

	public HStateRecords() {
		//TODO: da ottimizzare
		mappedStates = new HGenericStateFSM[20];
	}

	public void addState(HGenericStateFSM state) {

		if (index >= mappedStates.Length) {

			reallocateMappedStates();

		}
		//Debug.Log ("aggiungo lo stato " + state.StateName + " all'indice " + index);
		mappedStates [index] = state;

		//Debug.Log ("ho aggiunto lo stato " + mappedStates [index].StateName + " all'indice " + index);

		index++;

	}

	void reallocateMappedStates(){

	}

	public HGenericStateFSM getStateByID(int _id) {

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

	public HGenericStateFSM getStateByName(string _stateName) {

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

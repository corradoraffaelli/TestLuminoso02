using UnityEngine;
using System.Collections;

public class AIAgent : MonoBehaviour {

	/*
	public enum patrolSubState {
		Walk,
		Stand,
		Area,
		AreaSuspicious,
	}
	public patrolSubState paSS;
	patrolSubState defaultPaType;



	public enum chaseSubState {
		ChargingCh,//TODO : now impostare suo uso - cambiare setup e continuechase e stopchase...
		ChargedCh,
		Constant,
	}
	chaseSubState chSS;
	
	public enum attackSubState {
		Immediate,
		ChargingAt,
		Stuck,
	}
	public attackSubState atSS;
	attackSubState defaultAtType;
	
	public enum fleeSubState {
		Surprise,
		Escape,
	}
	fleeSubState flSS;
	*/
	//----------

	//[HideInInspector]
	public StateFSM []states;

	//[HideInInspector]
	public StateFSM.myStateName []stati;

	protected string []nomiStati;

	public StateFSM.myStateName activeStateName;
	protected int activeStateIndex = 0;
	protected int prevActiveStateIndex = 0;

	protected AIParameters par;


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

		setStartState ();
		initializeStates ();
		initializeConditions ();

	}

	protected virtual void setStartState() {

		//activeStateName = StateFSM.myStateName.Patrol;

	}

	protected virtual void initializeConditions() {

	}

	protected virtual void initializeStates(){

		//esempio di initialize...
		//if(stati.Length==0)

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
				activeStateIndex = count;
			}

			count++;

		}

	}
	
	// Update is called once per frame
	protected virtual void Update () {

		StateFSM.myStateName nextStateName = activeStateName;

		if (states [activeStateIndex].myTransitions != null) {

			for (int t=0; t< states[activeStateIndex].myTransitions.Length; t++) {

				states [activeStateIndex].myTransitions [t] (ref nextStateName);

				if (states [activeStateIndex].state != nextStateName) {
					makeTransition (nextStateName);
					break;
				}
			}
		}
		/*
		if(states[activeStateIndex].myTransition != null)
			states[activeStateIndex].myTransition (ref nextStateName);

		if (states[activeStateIndex].state != nextStateName) {

			makeTransition (nextStateName);
		
		}
		*/
		states[activeStateIndex].myUpdate ();
		
	}



	protected virtual void makeTransition(StateFSM.myStateName targetState) {

		for (int i=0;i<states.Length;i++) {

			if(states[i].state == targetState) {

				object ob = null;

				if(states[activeStateIndex].myFinalize != null) {
					ob = states[activeStateIndex].myFinalize();
					//Debug.Log ("stam");
					//Debug.Log ("stampo l'object " + ((GameObject)ob).name);
				}
				activeStateName = states[i].state;
				activeStateIndex = i;

				if(states[i].myInitialize != null)
					states[i].myInitialize(ref ob);

				break;

			}

		}

	}

	protected virtual void OnCollisionEnter2D(Collision2D c) {

		if(states [activeStateIndex].myHandleCollisionEnter!=null)
			states [activeStateIndex].myHandleCollisionEnter (c);

	}

	public void setStunned(bool st) {
		Debug.Log ("ahi!!!!!!!!!!!!!!!!!!");
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

	public virtual void OnTriggerEnter2D(Collider2D c) {

		if (c.tag == "Cleaner") {
			handleClean();
			Destroy (this.gameObject);
			return;
		}

		if(states [activeStateIndex].myHandleTriggerEnter!=null)
			states [activeStateIndex].myHandleTriggerEnter (c);
	}
}

[System.Serializable]
public class piango {

	[SerializeField]
	string lacrima;

}
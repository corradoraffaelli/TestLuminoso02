using UnityEngine;
using System.Collections;

public class HStunnedFSM : HStateFSM {

	float startStunnedTime = 0.0f;
	float timeToStayStunned = 3.0f;

	bool killingState = false;

	int stateAfterStunnedID = -1;

	public HStunnedFSM(int _stateId, GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent, bool _killingState=true) 
	: base("Stunned", _stateId, _gameo, _hLevel, true, _fatherState, _scriptAIAgent) {

		killingState = _killingState;

		if (!killingState) {
			myInitialize += normalStunnedInitialize;
		} 
		else {
			myInitialize += killingStunnedInitialize;
		}

		myUpdate += stunnedUpdate;

		myFinalize += stunnedFinalize;


		
	}

	public void setDefaultTransitions(HPatrolFSM patrolState) {


		//addTransition (S2PcountDownStunned, "Patrol");

		addTransition (S2PcountDownStunned, patrolState);

		/*
		if (!killingState) {
			
			addTransition (S2PcountDownStunned, "Patrol");
			
			Debug.Log ("trans messa");
		} 
		else {
			

			
		}
		*/
		
	}

	public void setDefaultTransitions(HPatrol1FSM patrolState) {
		
		
		//addTransition (S2PcountDownStunned, "Patrol");
		
		addTransition (S2PcountDownStunned, patrolState);
		
		/*
		if (!killingState) {
			
			addTransition (S2PcountDownStunned, "Patrol");
			
			Debug.Log ("trans messa");
		} 
		else {
			

			
		}
		*/
		
	}

	public void setDefaultTransitions(HWanderFSM wanderState) {
		
		//addTransition (S2PcountDownStunned, "Wander");
		addTransition (S2PcountDownStunned, wanderState);

		/*
		if (!killingState) {
			
			addTransition (S2PcountDownStunned, "Patrol");
			
			Debug.Log ("trans messa");
		} 
		else {
			
			addTransition (S2PcountDownStunned, "Wander");
			
		}
		*/
		
	}

	protected void normalStunnedInitialize(ref object ob){

		#if _DEBUG
				Debug.Log ("inizio stunn --------------------------------");
		#endif

		i_stunned (true);

		startStunnedTime = Time.time;

		setEnemiesStunnedLayer ();

	}

	protected void killingStunnedInitialize(ref object ob){

		#if _DEBUG
		Debug.Log ("inizio stunn --------------------------------");
		#endif

		i_stunned (true);
		
		handleKillo ();

		setDeadLayer ();
	}

	protected void handleKillo() {

		_rigidbody.AddForce(new Vector2(100.0f,300.0f));
		_boxCollider.isTrigger = true;
		_circleCollider.isTrigger = true;



		
	}
	
	protected IEnumerator handleKill() {
		
		yield return new WaitForSeconds(0.1f);
		
		par._rigidbody.AddForce(new Vector2(100.0f,300.0f));
		par._boxCollider.isTrigger = true;
		par._circleCollider.isTrigger = true;
		
	}
	
	protected void stunnedUpdate(){

	}
	
	protected object stunnedFinalize(){

		#if _DEBUG
			Debug.Log ("finisco stunn --------------------------------");
		#endif

		//finishStunned = false;
		i_stunned (false);
		//TODO: inserire altra roba per cui serve riattivare altro...
		setDefaultLayer ();

		return null;
	}

	public bool S2PcountDownStunned(){

		if(Time.time - startStunnedTime > timeToStayStunned) {
			par.stunnedReceived = false;
			return true;
		}
		else {
			return false;
		}

	}


}

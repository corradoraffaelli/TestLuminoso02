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


		addTransition (S2PcountDownStunned, "Patrol");

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
		
		addTransition (S2PcountDownStunned, "Wander");
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
		
		Debug.Log ("inizio stunn --------------------------------");

		i_stunned (true);

		startStunnedTime = Time.time;

	}

	protected void killingStunnedInitialize(ref object ob){
		
		Debug.Log ("inizio stunn --------------------------------");

		i_stunned (true);
		
		handleKillo ();
	}

	protected void handleKillo() {

		par._rigidbody.AddForce(new Vector2(100.0f,300.0f));
		par._boxCollider.isTrigger = true;
		par._circleCollider.isTrigger = true;
		
	}
	
	protected IEnumerator handleKill() {
		
		yield return new WaitForSeconds(0.1f);
		
		par._rigidbody.AddForce(new Vector2(100.0f,300.0f));
		par._boxCollider.isTrigger = true;
		par._circleCollider.isTrigger = true;
		
	}
	
	protected void stunnedUpdate(){
		Debug.Log ("ciao da stunn");
	}
	
	protected object stunnedFinalize(){
		Debug.Log ("finisco stunn --------------------------------");
		//finishStunned = false;
		i_stunned (false);
		//TODO: inserire altra roba per cui serve riattivare altro...
		return null;
	}

	public int S2PcountDownStunned(ref int _id){
		Debug.Log ("controll");
		if(Time.time - startStunnedTime > timeToStayStunned) {
			par.stunnedReceived = false;
			return -2;
		}
		else {
			return -1;
		}

	}


}

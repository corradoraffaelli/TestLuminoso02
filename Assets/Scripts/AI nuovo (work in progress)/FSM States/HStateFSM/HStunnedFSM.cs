using UnityEngine;
using System.Collections;

public class HStunnedFSM : HStateFSM {

	float startStunnedTime = 0.0f;
	float timeToStayStunned = 3.0f;

	int stateAfterStunnedID = -1;

	public HStunnedFSM(int _stateId, GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent, bool killingState=true) 
	: base("Stunned", _stateId, _gameo, _hLevel, true, _fatherState, _scriptAIAgent) {

		if (!killingState) {
			myInitialize += normalStunnedInitialize;
		} 
		else {
			myInitialize += killingStunnedInitialize;
		}

		myUpdate += stunnedUpdate;

		myFinalize += stunnedFinalize;

		if (!killingState) {
			myTransitions = new myStateTransition[1];
			myTransitions[0] += S2PcountDownStunned;
			Debug.Log ("trans messa");
		}
		else {


		}

		
	}

	protected void normalStunnedInitialize(ref object ob){
		
		Debug.Log ("inizio stunn --------------------------------");

		if (stateAfterStunnedID == -1) {

			stateAfterStunnedID = getIndexState ("Patrol");

			if(stateAfterStunnedID == -1) {

				stateAfterStunnedID = getIndexState ("Wander");

			}
		}
		i_stunned (true);


		startStunnedTime = Time.time;
		//Invoke ("countDown", countDownStunned);

		//StartCoroutine (countDownn());

	}

	protected void killingStunnedInitialize(ref object ob){
		
		Debug.Log ("inizio stunn --------------------------------");

		if(stateAfterStunnedID!=-1)
			stateAfterStunnedID = getIndexState ("Patrol");

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

	protected int S2PcountDownStunned(ref int _id){
		Debug.Log ("controll");
		if(Time.time - startStunnedTime > timeToStayStunned) {

			_id = stateAfterStunnedID;
			Debug.Log ("_id ritornato " + _id);
			return 0;
		}
		else {
			return -1;
		}
		/*
		if (finishStunned) {
			_id = stateAfterStunnedID;
			finishStunned = false;
			return 0;
		}
		else {
			return -1;
		}
		*/

	}


}

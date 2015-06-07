using UnityEngine;
using System.Collections;

public class HWanderFSM : HStateFSM {

	public int stateStunnedID = -1;

	public HWanderFSM(int _stateId, GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base("Wander", _stateId, _gameo, _hLevel, true, _fatherState, _scriptAIAgent) {

		//myUpdate += updateState;

		myInitialize += wanderInitialize;

		myUpdate += wanderUpdate;

		myTransitions = new myStateTransition[1];
		
		myTransitions[0] += W2ScheckStunned;
		
		myHandleCollisionEnter += wanderHandleCollisionEnter;
		
		//myTransition += patrolToFleeTransition;
		
	}
	
	
	protected void wanderUpdate(){
		//Debug.Log ("stato wander");
		//TODO : da cambiare
		i_move (4.0f);
		
	}

	protected void wanderInitialize (ref object ob) {

		if (stateStunnedID == -1) {
			stateStunnedID = agentScript.statesMap.getStateIDByName ("Stunned");
			Debug.Log ("stato stunned " + stateStunnedID);
		}

	}
	
	private int W2ScheckStunned(ref int _id){

		//Debug.Log("check pre stun");
		if (par.stunnedReceived) {
			Debug.Log("-> -> -> result 0 ref id" + stateStunnedID);
			_id = stateStunnedID;
			par.stunnedReceived = false;
			return 0;
		} else {
			return -1;
		}
	}


}

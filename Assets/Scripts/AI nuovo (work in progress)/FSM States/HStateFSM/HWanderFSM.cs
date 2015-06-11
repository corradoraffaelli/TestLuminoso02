using UnityEngine;
using System.Collections;

public class HWanderFSM : HStateFSM {

	public int stateStunnedID = -1;

	public HWanderFSM(int _stateId, GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base("Wander", _stateId, _gameo, _hLevel, true, _fatherState, _scriptAIAgent) {

		myInitialize += wanderInitialize;

		myUpdate += wanderUpdate;

		myHandleCollisionEnter += wanderHandleCollisionEnter;

	}
	
	public void setDefaultTransitions(HStunnedFSM stunState) {
		
		addTransition (W2ScheckStunned, "Stunned");
		
	}


	protected void wanderUpdate(){
		//Debug.Log ("stato wander");
		//TODO : da cambiare
		i_move (4.0f);
		
	}

	protected void wanderInitialize (ref object ob) {


	}
	
	private int W2ScheckStunned(ref int _id){

		//Debug.Log("check pre stun");
		if (par.stunnedReceived) {
			Debug.Log("STUNNNNNNNNNNNNNNNNNNNNNNNNNNNNN");

			par.stunnedReceived = false;
			return -2;
		} else {
			return -1;
		}
	}


}

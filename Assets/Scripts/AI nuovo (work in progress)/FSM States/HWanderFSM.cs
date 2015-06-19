using UnityEngine;
using System.Collections;

public class HWanderFSM : HStateFSM {

	public int stateStunnedID = -1;

	public HWanderFSM(int _stateId, GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base("Wander", _stateId, _gameo, _hLevel, true, _fatherState, _scriptAIAgent) {

		myInitialize += wanderInitialize;

		myUpdate += wanderUpdate;

		myHandleCollisionEnter += checkFlipNeedForCollision;

	}
	
	public void setDefaultTransitions(HStunnedFSM stunState) {
		
		//addTransition (W2ScheckStunned, "Stunned");
		addTransition (W2ScheckStunned, stunState);
	}


	protected void wanderUpdate(){
		//Debug.Log ("stato wander");
		//TODO : da cambiare
		i_move (4.0f);
		
	}

	protected void wanderInitialize (ref object ob) {


	}
	
	private bool W2ScheckStunned(){

		if (par.stunnedReceived) {
			#if _DEBUG
				Debug.Log("STUNNNNNNNNNNNNNNNNNNNNNNNNNNNNN");
			#endif


			par.stunnedReceived = false;
			return true;
		} else {
			return false;
		}
	}


}

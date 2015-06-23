using UnityEngine;
using System.Collections;

public class HFleeFSM : HEnemyStateFSM {
	
	public int stateStunnedID = -1;
	FleeParameters fleePar;

	GameObject fleeTarget {
		get{ 
			if(fleePar!=null) return fleePar.fleeTarget;
			else return null;}
		set{ if(fleePar!=null) fleePar.fleeTarget = value;}
		
	}
	

	
	float fleeSpeed {
		get{ 
			if(fleePar!=null) return fleePar.fleeSpeed;
			else return 0.0f;}
		set{ if(fleePar!=null) fleePar.fleeSpeed = value;}
		
	}
	
	public HFleeFSM(int _stateId, GameObject _gameo, int _hLevel, HEnemyStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base("Flee", _stateId, _gameo, _hLevel, true, _fatherState, _scriptAIAgent) {
		
		myInitialize += fleeInitialize;
		
		myUpdate += fleeUpdate;
		
		myHandleCollisionEnter += checkFlipNeedForCollision;
		
	}

	void initializePatrolParameters(){
		
		fleePar = gameObject.GetComponent<AIParameters> ().fleeParameters;

	}

	public void setDefaultTransitions(HStunnedFSM stunState) {
		
		//addTransition (F2ScheckStunned, "Stunned");
		addTransition (F2ScheckStunned, stunState);
	}
	
	
	protected void fleeUpdate(){
		//Debug.Log ("stato wander");
		//TODO : da cambiare
		i_move (4.0f);
		
	}

	protected void fleeAwayATarget(GameObject _fleeTarget) {



	}

	protected void fleeInitialize () {

		//settare target flee
		
	}
	
	private bool F2ScheckStunned(){
		
		//Debug.Log("check pre stun");
		if (par.stunnedReceived) {
			
			par.stunnedReceived = false;
			return true;
		} else {
			return false;
		}
	}
	
	
}

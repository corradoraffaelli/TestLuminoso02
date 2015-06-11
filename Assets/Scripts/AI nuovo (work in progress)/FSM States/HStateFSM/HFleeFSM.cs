﻿using UnityEngine;
using System.Collections;

public class HFleeFSM : HStateFSM {
	
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
	
	public HFleeFSM(int _stateId, GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base("Flee", _stateId, _gameo, _hLevel, true, _fatherState, _scriptAIAgent) {
		
		myInitialize += fleeInitialize;
		
		myUpdate += fleeUpdate;
		
		myHandleCollisionEnter += wanderHandleCollisionEnter;
		
	}

	void initializePatrolParameters(){
		
		fleePar = myGameObject.GetComponent<AIParameters> ().fleeParameters;

	}

	public void setDefaultTransitions(HStunnedFSM stunState) {
		
		addTransition (F2ScheckStunned, "Stunned");
		
	}
	
	
	protected void fleeUpdate(){
		//Debug.Log ("stato wander");
		//TODO : da cambiare
		i_move (4.0f);
		
	}

	protected void fleeAwayATarget(GameObject _fleeTarget) {



	}

	protected void fleeInitialize (ref object ob) {

		//settare target flee
		
	}
	
	private int F2ScheckStunned(ref int _id){
		
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
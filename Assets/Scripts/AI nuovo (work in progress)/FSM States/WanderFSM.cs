using UnityEngine;
using System.Collections;

public class WanderFSM : StateFSM {

	/*

	public myStateName state;
	protected string stateName;
	
	protected AIAgent agentScript;
	protected PlayerMovements playerScript;
	protected AIParameters parametersScript;
	
	public delegate void myStateInitialize();
	public myStateInitialize myInitialize;
	
	public delegate void myStateUpdate();
	public myStateUpdate myUpdate;
	
	public delegate void myStateFinalize();
	public myStateUpdate myFinalize;
	
	public delegate void myStateTransition(ref myStateName s);
	public myStateTransition myTransition;
	*/

	public WanderFSM(GameObject gameo) : base(gameo) {

		//GameObject gameo = this.gameObject;

		state = myStateName.Wander;
		stateName = "Wander";

		//myUpdate += updateState;

		myTransitions = new myStateTransition[1];

		myTransitions[0] += W2ScheckStunned;

		myHandleCollisionEnter += wanderHandleCollisionEnter;

		//myTransition += patrolToFleeTransition;

	}


	protected override void updateState(){
		//Debug.Log ("stato wander");
		//TODO : da cambiare
		i_move (4.0f);

	}


	private void W2ScheckStunned(ref myStateName st){
		
		//if (st != state)
		//	return;

		//Debug.Log("check pre stun");
		if (par.stunnedReceived) {
			Debug.Log("stunnato");
			st = myStateName.Stunned;
			par.stunnedReceived = false;
		} else {
			return;
		}
	}



}

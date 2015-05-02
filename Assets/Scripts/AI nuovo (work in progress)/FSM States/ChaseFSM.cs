using UnityEngine;
using System.Collections;

public class ChaseFSM : StateFSM {
	
	public ChaseFSM(GameObject go) : base(go) {

		//AIGameObject = ai;

		state = myStateName.Chase;

		myInitialize += initializeChase;
		
		myUpdate += updateChase;
		
		myFinalize += finalizeChase;
		
		myTransition += chaseToPatrolTransition;
		myTransition += chaseToFleeTransition;
	}
	
	private void initializeChase(){
		Debug.Log ("inizio CHASE --------------------------------");
	}
	
	private void updateChase(){
		Debug.Log ("ciao da CHASE");
	}
	
	private void finalizeChase(){
		Debug.Log ("finisco CHASE --------------------------------");
	}

	private void chaseToPatrolTransition(ref myStateName st){
		
		if (st != myStateName.Chase)
			return;
		
		if (Input.GetKeyUp(KeyCode.P)) {
			st = myStateName.Chase;
		} else {
			return;
		}
	}
	
	private void chaseToFleeTransition(ref myStateName st){
		
		if (st != myStateName.Chase)
			return;
		
		if (Input.GetKeyUp(KeyCode.F)) {
			st = myStateName.Flee;
		} else {
			return;
		}
	}
}

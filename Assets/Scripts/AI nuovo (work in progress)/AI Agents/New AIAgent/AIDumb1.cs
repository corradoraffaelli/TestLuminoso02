using UnityEngine;
using System.Collections;

public class AIDumb1 : AIAgent1 {

	//initializeStates ();
	//setStartState ();
	//initializeConditions ();

	protected override void setStartState(){

		object ob = null;

		activeState = hstates [0];
		activeHStateIndex = 0;

		activeState.myHInitialize (ref ob);


	}

	protected override void initializeConditions() {



	}
	
	protected override void initializeHStates() {

		Debug.Log ("àààààààààààààààààààààààààààààààààààààààà");

		hstates = new HStateFSM[2];
		statesMap = new HStateRecords ();

		hstates [0] = new HWanderFSM (0, this.gameObject, 0, null, this);
		//statesMap.addState (hstates [0]);

		hstates [1] = new HStunnedFSM (1, this.gameObject, 0, null, this,false);
		//statesMap.addState (hstates [1]);

		//hstates = new HStateFSM[stati.Length];
		
		//int count = 0;
		/*
		foreach (StateFSM.myStateName st in stati) {
			
			switch(st) {
				
			case StateFSM.myStateName.Patrol :

				//HStateFSM pHFSM = new HStateFSM
				PatrolFSM pFSM = new PatrolFSM(this.gameObject, PatrolFSM.patrolSubState.Walk);
				states[count] = pFSM;
				
				break;
				
			case StateFSM.myStateName.Chase :
				
				ChaseFSM cFSM = new ChaseFSM(this.gameObject);
				states[count] = cFSM;
				
				break;
				
			case StateFSM.myStateName.Wander :
				
				WanderFSM wFSM = new WanderFSM(this.gameObject);
				states[count] = wFSM;
				
				break;
				
			case StateFSM.myStateName.Stunned :
				StunnedFSM sFSM = new StunnedFSM(this.gameObject);
				states[count] = sFSM;
				
				break;
				
			}
			
			if(activeStateName == st) {
				activeHStateIndex = count;
			}
			
			count++;
			
		}
*/
	}
	
	protected override void initializeStates(){


		//esempio di initialize...
		//if(stati.Length==0)
		/*
		states = new StateFSM[stati.Length];
		
		int count = 0;
		
		foreach (StateFSM.myStateName st in stati) {
			
			switch(st) {
				
			case StateFSM.myStateName.Patrol :
				
				PatrolFSM pFSM = new PatrolFSM(this.gameObject, PatrolFSM.patrolSubState.Walk);
				states[count] = pFSM;
				
				break;
				
			case StateFSM.myStateName.Chase :
				
				ChaseFSM cFSM = new ChaseFSM(this.gameObject);
				states[count] = cFSM;
				
				break;
				
			case StateFSM.myStateName.Wander :
				
				WanderFSM wFSM = new WanderFSM(this.gameObject);
				states[count] = wFSM;
				
				break;
				
			case StateFSM.myStateName.Stunned :
				StunnedFSM sFSM = new StunnedFSM(this.gameObject);
				states[count] = sFSM;
				
				break;
				
			}
			
			if(activeStateName == st) {
				activeHStateIndex = count;
			}
			
			count++;
			
		}
		*/
	}
}

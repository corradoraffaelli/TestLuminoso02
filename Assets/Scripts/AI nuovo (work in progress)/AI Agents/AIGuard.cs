using UnityEngine;
using System.Collections;

public class AIGuard : AIAgent {

	protected virtual void setStartState() {

		activeStateName = StateFSM.myStateName.Patrol;
		activeStateIndex = 0;
	}

	protected override void initializeStates(){
		
		//esempio di initialize...
		//if(stati.Length==0)
		stati = new StateFSM.myStateName[4];

		stati [0] = StateFSM.myStateName.Patrol;
		stati [1] = StateFSM.myStateName.Chase;
		stati [2] = StateFSM.myStateName.Wander;
		stati [3] = StateFSM.myStateName.Flee;


		states = new StateFSM[4];
		
		int count = 0;
		
		foreach (StateFSM.myStateName st in stati) {
			
			switch(st) {
				
			case StateFSM.myStateName.Patrol :
				
				PatrolFSM pFSM = new PatrolFSM(this.gameObject, PatrolFSM.patrolSubState.Area);
				states[count] = pFSM;
				
				break;
				
			case StateFSM.myStateName.Chase :
				
				ChaseFSM cFSM = new ChaseFSM(this.gameObject);
				states[count] = cFSM;
				
				break;
				
			case StateFSM.myStateName.Wander :
				
				//WanderFSM wFSM = new WanderFSM(this.gameObject);
				//states[count] = wFSM;
				
				break;
				
			case StateFSM.myStateName.Stunned :
				StunnedFSM sFSM = new StunnedFSM(this.gameObject);
				states[count] = sFSM;
				
				break;
				
			}
			
			if(activeStateName == st) {
				activeStateIndex = count;
			}
			
			count++;
			
		}
		
	}

}

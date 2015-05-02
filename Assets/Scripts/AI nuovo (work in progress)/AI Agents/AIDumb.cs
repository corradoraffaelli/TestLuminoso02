using UnityEngine;
using System.Collections;

public class AIDumb : AIAgent {

	protected override void initializeStates(){
		states = new StateFSM[2];

		//WanderFSM wFSM = new WanderFSM(gameObject);
		//states[0] = wFSM;

		//StunnedFSM sFSM = new StunnedFSM(gameObject);
		//states[1] = sFSM;

	}

	protected void initializeConditions() {
		activeStateName = StateFSM.myStateName.Wander;
		activeStateIndex = 0;
	}

}

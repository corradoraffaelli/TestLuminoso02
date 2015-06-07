using UnityEngine;
using System.Collections;

public class AIDumb : AIAgent {

	protected virtual void setStartState() {
		
		activeStateName = StateFSM.myStateName.Wander;
		
	}

	protected override void initializeConditions() {

		//activeStateName = StateFSM.myStateName.Wander;
		//activeStateIndex = 0;

	}


}

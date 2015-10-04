using UnityEngine;
using System.Collections;

/// <summary>
/// AI del guard, versione. (DEPRECATO)
/// </summary>

//Dario

public class AIGuard1 : AIAgent1 {
	
	//initializeHStates ();
	//setStartState ();
	//initializeConditions ();
	

	
	protected override void initializeHStates() {
		
		HPatrolFSM hp = new HPatrolFSM (0, this.gameObject, 0, null, this, HPatrolFSM.patrolSubState.Walk);

		HStunnedFSM hs =  new HStunnedFSM (1, this.gameObject, 0, null, this, false);

		HChaseFSM hc = new HChaseFSM (2, this.gameObject, 0, null, this);

		addState (hp);

		addState (hs);

		addState (hc);

		//------

		setActiveState (hp);

		hp.setDefaultTransitions (hs, hc);

		hs.setDefaultTransitions (hp);

		hc.setDefaultTransitions (hs, hp);

	}
	
	
}

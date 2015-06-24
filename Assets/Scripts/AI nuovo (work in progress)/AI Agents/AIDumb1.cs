using UnityEngine;
using System.Collections;

public class AIDumb1 : AIAgent1 {

	//initializeStates ();
	//setStartState ();
	//initializeConditions ();

	AIParameters aiParam;
	
	protected override void initializeHStates() {

		aiParam = GetComponent<AIParameters> ();

		HWanderFSM hw = new HWanderFSM (0, this.gameObject, 0, null, this);

		HStunnedFSM hs = new HStunnedFSM (1, this.gameObject, 0, null, this);

		addState (hw);

		addState (hs);


		//-------

		setActiveState (hw);

		hw.setDefaultTransitions (hs);

		hs.setDefaultTransitions (hw);

	}

	public void c_instantKill(){
		
		//TODO:
		//makeStateTransition(eMS, enemyMachineState.Stunned);
		aiParam.instantKill = true;
		
	}

}

using UnityEngine;
using System.Collections;

public class AIDumb1 : AIAgent1 {

	//initializeStates ();
	//setStartState ();
	//initializeConditions ();


	
	protected override void initializeHStates() {

		HWanderFSM hw = new HWanderFSM (0, this.gameObject, 0, null, this);

		HStunnedFSM hs = new HStunnedFSM (1, this.gameObject, 0, null, this);

		addState (hw);

		addState (hs);


		//-------

		setActiveState (hw);

		hw.setDefaultTransitions (hs);

		hs.setDefaultTransitions (hw);

	}
	

}

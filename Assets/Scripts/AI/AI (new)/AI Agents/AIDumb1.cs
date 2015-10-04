using UnityEngine;
using System.Collections;

/// <summary>
/// AI dumb.
/// </summary>

//Dario

public class AIDumb1 : AIAgent1 {

	//initializeStates ();
	//setStartState ();
	//initializeConditions ();

	AIParameters aiParam;

	public bool flipControl = false;

	protected override void initializeHStates() {

		aiParam = GetComponent<AIParameters> ();

		HWanderFSM hw = new HWanderFSM (0, this.gameObject, 0, null, this, flipControl);

		HStunnedFSM hs = new HStunnedFSM (1, this.gameObject, 0, null, this);

		addState (hw);

		addState (hs);

		//-------

		setActiveState (hw);

		hw.setDefaultTransitions (hs);

		hs.setDefaultTransitions (hw);

	}
	
}

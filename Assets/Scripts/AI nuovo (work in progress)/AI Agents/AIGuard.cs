using UnityEngine;
using System.Collections;

public class AIGuard : AIAgent1 {

	public HPatrolFSM.patrolSubState patrolType;

	protected override void initializeHStates() {
		//Time.timeScale = 0.3f;

		HPatrolFSM hp = new HPatrolFSM (0, this.gameObject, 0, null, this, patrolType);
		
		HStunnedFSM hs =  new HStunnedFSM (1, this.gameObject, 0, null, this, false);
		//(string _stateName, GameObject _gameo, int _hLevel, AIAgent1 _scriptAIAgent)
		HChase1FSM hc = new HChase1FSM ("Chase", this.gameObject, 0, this);

		HChargeChaseFSM hcc1 = new HChargeChaseFSM (this.gameObject, 1, hc, this);
		HCrashChaseFSM hcc2 = new HCrashChaseFSM (this.gameObject, 1, hc, this);

		addState (hp);
		
		addState (hs);
		
		addState (hc);

		hc.setDefaultStates (hcc1, hcc2);
		//hc.addState (hcc1);
		//hc.addState (hcc2);

		//hpadre.addState (hfiglio1);
		//hpadre.addState (hfiglio2);

		//------
		
		setActiveState (hp);
		
		hp.setDefaultTransitions (hs, hc);
		
		hs.setDefaultTransitions (hp);

		hc.setDefaultInitialize ();
		hc.setDefaultTransitions (hs, hp);
		hc.setDefaultCollision ();

		hcc1.setDefaultTransitions (hcc2);
		hcc2.setDefaultTransitions (hs);

	}
}

using UnityEngine;
using System.Collections;

//Dario

public class AIProva : AIAgent1 {
	
	//initializeStates ();
	//setStartState ();
	//initializeConditions ();
	
	
	
	protected override void initializeHStates() {
		
		//HWanderFSM hw = new HWanderFSM (0, this.gameObject, 0, null, this);
		
		//HStunnedFSM hs = new HStunnedFSM (1, this.gameObject, 0, null, this);

		HPadreFSM hpadre = new HPadreFSM ("Padre", this.gameObject, 0, this);

		HPadreFSM hmadre = new HPadreFSM ("Madre", this.gameObject, 0, this);

		HFiglio1FSM hfiglio1 = new HFiglio1FSM ("Figlio1", this.gameObject, 1, hpadre, this);
		HFiglio1FSM hfiglio2 = new HFiglio1FSM ("Figlio2", this.gameObject, 1, hpadre, this);

		HFiglio1FSM hfiglia1 = new HFiglio1FSM ("Figlia1", this.gameObject, 1, hmadre, this);
		HFiglio1FSM hfiglia2 = new HFiglio1FSM ("Figlia2", this.gameObject, 1, hmadre, this);

		addState (hpadre);

		addState (hmadre);

		hpadre.addState (hfiglio1);
		hpadre.addState (hfiglio2);

		hmadre.addState (hfiglia1);
		hmadre.addState (hfiglia2);
		//addState (hw);
		
		//addState (hs);
		
		//-------
		
		//setActiveState (hw);

		setActiveState (hpadre);

		//hw.setDefaultTransitions (hs);
		hfiglio1.setTransition1 (hfiglio2, hmadre);
		hfiglio2.setTransition2 (hfiglio1, hmadre);

		hfiglia1.setTransition1 (hfiglia2, hpadre);
		hfiglia2.setTransition2 (hfiglia1, hpadre);


		//hs.setDefaultTransitions (hw);
		
	}
	
	
}
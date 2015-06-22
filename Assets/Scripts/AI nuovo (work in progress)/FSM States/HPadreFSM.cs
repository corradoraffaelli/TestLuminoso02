using UnityEngine;
using System.Collections;

public class HPadreFSM : HStateFSM {

	public HPadreFSM(string _stateName, GameObject _gameo, int _hLevel, AIAgent1 _scriptAIAgent)
		: base(_stateName, 0, _gameo, _hLevel, false, null, _scriptAIAgent) {

		finalHLevel = false;
		myInitialize += initPadre;

	}

	protected void initPadre() {
		Debug.Log ("ciao da " + stateName);
	}



}

public class HFiglio1FSM : HPadreFSM {

	public HFiglio1FSM(string _stateName, GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base (_stateName, _gameo, _hLevel, _scriptAIAgent) {

		finalHLevel = true;
		fatherState = _fatherState;
		myInitialize += initFiglio;

		myUpdate += updateFiglio;

	}

	void updateFiglio() {
		Debug.Log ("up da " + stateName);
	}

	protected void initFiglio() {
		Debug.Log ("ciao da " + stateName);
	}

	public void setTransition1(HFiglio1FSM figlio, HPadreFSM padre) {

		addTransition(check1, figlio);
		addTransition(check3, padre);

	}

	public void setTransition2(HFiglio1FSM figlio, HPadreFSM padre) {
		
		addTransition(check2, figlio);
		addTransition(check3, padre);
		
	}

	private bool check1() {

		if (Input.GetKeyDown (KeyCode.A))
			return true;
		else
			return false;

	}

	private bool check2() {

		if (Input.GetKeyDown (KeyCode.D))
			return true;
		else
			return false;
		
	}

	private bool check3() {
		
		if (Input.GetKeyDown (KeyCode.W))
			return true;
		else
			return false;
		
	}

}


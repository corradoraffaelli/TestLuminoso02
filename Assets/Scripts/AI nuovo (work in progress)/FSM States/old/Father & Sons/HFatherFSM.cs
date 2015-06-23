using UnityEngine;
using System.Collections;

public class HFatherFSM : HStateFSM {
	
	public HFatherFSM(string _stateName, GameObject _gameo, int _hLevel, AIAgent1 _scriptAIAgent)
	: base(_stateName, 0, _gameo, _hLevel, false, null, _scriptAIAgent) {
		
		finalHLevel = false;
		myInitialize += initChaseFather;
		
	}
	
	protected void initChaseFather() {
		Debug.Log ("ciao da " + stateName);
	}
	
}

public class HFinalChildFSM : HFatherFSM {
	public HFinalChildFSM(string _stateName, GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base (_stateName, _gameo, _hLevel, _scriptAIAgent) {
		
		
		
	}

}
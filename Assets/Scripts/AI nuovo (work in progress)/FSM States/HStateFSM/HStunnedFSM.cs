using UnityEngine;
using System.Collections;

public class HStunnedFSM : HStateFSM {

	
	public HStunnedFSM(int _stateId, GameObject _gameo, int _hLevel, HStateFSM _fatherState, AIAgent1 _scriptAIAgent) 
	: base("Stunned", _stateId, _gameo, _hLevel, true, _fatherState, _scriptAIAgent) {

		myInitialize += stunnedInitialize;

		myUpdate += stunnedUpdate;

		myFinalize += stunnedFinalize;

		
	}
	
	protected void stunnedInitialize(ref object ob){
		
		Debug.Log ("inizio stunn --------------------------------");
		
		//StartCoroutine (handleKill ());
		i_stunned (true);
		
		handleKillo ();
	}
	
	protected void handleKillo() {

		par._rigidbody.AddForce(new Vector2(100.0f,300.0f));
		par._boxCollider.isTrigger = true;
		par._circleCollider.isTrigger = true;
		
	}
	
	protected IEnumerator handleKill() {
		
		yield return new WaitForSeconds(0.1f);
		
		par._rigidbody.AddForce(new Vector2(100.0f,300.0f));
		par._boxCollider.isTrigger = true;
		par._circleCollider.isTrigger = true;
		
	}
	
	protected void stunnedUpdate(){
		Debug.Log ("ciao da stunn");
	}
	
	protected object stunnedFinalize(){
		Debug.Log ("finisco stunn --------------------------------");
		return null;
	}

}

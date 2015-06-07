using UnityEngine;
using System.Collections;

public class StunnedFSM : StateFSM {


	
	public StunnedFSM(GameObject gameo) : base(gameo) {

		//GameObject gameo = this.gameObject;

		state = myStateName.Stunned;
		stateName = "Stunned";

		
		//myTransition += patrolToChaseTransition;
		
		//myTransition += patrolToFleeTransition;
		
		//myHandleCollision;
		
	}

	protected override void initializeState(ref object ob){

		Debug.Log ("inizio stunn --------------------------------");

		//StartCoroutine (handleKill ());
		i_stunned (true);

		handleKillo ();
	}

	protected void handleKillo() {
		
		//yield return new WaitForSeconds(0.1f);
		
		//BoxCollider2D b2d = GetComponent<BoxCollider2D> ();
		//CircleCollider2D c2d = GetComponent<CircleCollider2D> ();
		
		par._rigidbody.AddForce(new Vector2(100.0f,300.0f));
		par._boxCollider.isTrigger = true;
		par._circleCollider.isTrigger = true;
		
	}
	
	protected IEnumerator handleKill() {
		
		yield return new WaitForSeconds(0.1f);
		
		//BoxCollider2D b2d = GetComponent<BoxCollider2D> ();
		//CircleCollider2D c2d = GetComponent<CircleCollider2D> ();
		
		par._rigidbody.AddForce(new Vector2(100.0f,300.0f));
		par._boxCollider.isTrigger = true;
		par._circleCollider.isTrigger = true;
		
	}
	
	protected override void updateState(){
		//Debug.Log ("ciao da stunn");
	}
	
	protected override object finalizeState(){
		Debug.Log ("finisco stunn --------------------------------");
		return null;
	}


}

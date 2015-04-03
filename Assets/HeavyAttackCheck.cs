using UnityEngine;
using System.Collections;

public class HeavyAttackCheck : MonoBehaviour {
	
	public bool DEBUG_CHECKHA = false;
	
	float tLastStun = -2.0f;
	float tBetweenAdvise = 2.0f;
	public LayerMask targetLayers;
	basicAIEnemyV4 bai;
	public GameObject actualTarget;

	
	void Start() {
		bai = transform.parent.gameObject.GetComponent<basicAIEnemyV4> ();
	}
	
	public void OnTriggerEnter2D(Collider2D c) {
		
		//Debug.Log ("schiacciato");
		
		if ((targetLayers.value & 1 << c.gameObject.layer) > 0) {
			
			if (c.gameObject.tag == "Player") {
				
				if(DEBUG_CHECKHA) {
					
					Debug.Log("Io sono " + transform.gameObject.name + " e mi preparo a colpire il player " + c.gameObject.name);
					
				}
				actualTarget = c.gameObject;
				i_targetNear (true);
				

			}
			else {
				
				if(DEBUG_CHECKHA) {
					
					Debug.Log("Io sono " + transform.gameObject.name + " e mi preparo a colpire la proiezione " + c.gameObject.name);
					
				}
				actualTarget = c.gameObject;
				i_targetNear (true);
				
			}
		}
		
	}

	
	public void OnTriggerExit2D(Collider2D c) {
		
		if ((targetLayers.value & 1 << c.gameObject.layer) > 0) {
			actualTarget = null;
			i_targetNear (false);
			
		}
		
	}
	
	public void i_targetNear(bool n) {
		
		//bai.c_targetNear (n);
		transform.parent.gameObject.SendMessage ("c_targetNear", n);
		
	}

	public void c_chargedAttack() {

		if (actualTarget == null) {
			i_targetNear (false);
			return;
		}

		if (actualTarget.tag == "Player") {
			if(DEBUG_CHECKHA) {
				
				Debug.Log("Io sono " + transform.gameObject.name + " e ho colpito il player " + actualTarget.name);
				
			}
			actualTarget.transform.SendMessage ("c_stunned", true);
			Vector2 dist = actualTarget.transform.position - transform.position;
			Rigidbody2D r = actualTarget.GetComponent<Rigidbody2D>();

			r.velocity = new Vector2(0.0f, 0.0f);
			r.AddForce(300.0f*dist.normalized);



		}
		else {
			
			if(DEBUG_CHECKHA) {
				
				Debug.Log("Io sono " + transform.gameObject.name + " e ho colpito la proiezione " + actualTarget.name);
				
			}

			//comportamento da implementare per le proiezioni
			

		}

		actualTarget = null;
		i_targetNear (false);
		
	}
	
}

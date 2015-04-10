using UnityEngine;
using System.Collections;

public class handleStunAI : MonoBehaviour {
	
	public bool DEBUG_STUNNED = false;
	public bool bouncy = true;
	public float sprintForce = 350.0f;
	
	public enum stunType {
		Player,
		AI,
	}
	
	public stunType IAM;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void OnTriggerEnter2D(Collider2D c) {

		switch (IAM) {
			//questo script è di ciò che riceve lo stun
			
			case stunType.AI :
				
				if (c.gameObject.tag=="Player") {

					if(DEBUG_STUNNED) {
						
						Debug.Log("Io sono " + transform.gameObject.name + " e sono stato colpito dal figlio di " + c.gameObject.name);
						
					}
					
					transform.parent.SendMessage ("setStunned", true);
					
					//MODIFICA GESTIONE BOUNCE
					if(bouncy) {
						Rigidbody2D r = c.gameObject.GetComponent<Rigidbody2D>();
						r.velocity = new Vector2(0.0f, 0.0f);
					r.AddForce(new Vector2(0.0f, sprintForce));
						//bouncy = false;
					}
				} 
				else {
					//Debug.Log ("nome oggetto " + c.gameObject.name);
				}
				
				break;
				
			}
		
		
		
	}
	
	public void c_setBouncy(bool b) {
		
		bouncy = b;
		
	}
	
}

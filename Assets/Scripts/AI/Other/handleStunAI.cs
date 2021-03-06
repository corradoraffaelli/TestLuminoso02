﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce lo stun dell'AI.
/// </summary>

public class handleStunAI : MonoBehaviour {
	
	public bool DEBUG_STUNNED = false;
	public bool bouncy = true;
	public float sprintForce = 350.0f;
	public bool newImplementation = true;

	int defaultParentLayer = 0;

	private GameObject helmet;
	
	public enum stunType {
		Player,
		AI,
	}
	
	public stunType IAM;
	// Use this for initialization
	void Start () {
		defaultParentLayer = transform.parent.gameObject.layer;

	}
	

	// Update is called once per frame
	void Update () {
		
	}


	
	public void OnTriggerEnter2D(Collider2D c) {

		switch (IAM) {
			//questo script è di ciò che riceve lo stun
			
			case stunType.AI :
				
				if (c.gameObject.tag=="Player") {
					
					//TODO: caso da vedere meglio
					if(c.gameObject.transform.position.y < transform.parent.transform.position.y + 1.0f) {
						return;
					}
					
					if(DEBUG_STUNNED) {
						
						Debug.Log("Io sono " + transform.gameObject.name + " e sono stato colpito dal figlio di " + c.gameObject.name);
						
					}
					
					transform.parent.SendMessage ("setStunned", true);
					
					//MODIFICA GESTIONE BOUNCE
					
					if(bouncy && transform.parent.gameObject.layer==defaultParentLayer) {
						if(newImplementation) {
							c.gameObject.SendMessage("c_jumpEnemy");
						}
						else {
							Rigidbody2D r = c.gameObject.GetComponent<Rigidbody2D>();
							r.velocity = new Vector2(0.0f, 0.0f);
							r.AddForce(new Vector2(0.0f, sprintForce));

						}
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

[System.Serializable]
public class proviamo {
	[SerializeField]
	public int ciao = 1;
	[SerializeField]
	public float deo = 2;
	[SerializeField]
	private float io = 5.0f;
}
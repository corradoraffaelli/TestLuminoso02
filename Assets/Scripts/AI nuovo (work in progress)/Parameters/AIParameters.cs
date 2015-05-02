using UnityEngine;
using System.Collections;

public class AIParameters : MonoBehaviour {

	PlayerMovements pm;

	//TODO: capire se inglobare playermovements... derivandola?
	//perché vorrei avere i parametri di velocità di movimento assieme agli altri parametri dell'AI

	//l'indice dell'array indica la profondità con cui si vuole scavare
	public bool []DEBUG_FSM_TRANSITION = new bool[3];
	public bool []DEBUG_RECOGNITION = new bool[2];
	public bool []DEBUG_ASTAR = new bool[2];
	public bool []DEBUG_FLIP = new bool[2];
	public bool []DEBUG_COLLISION = new bool[3];

	[Range(0.1f,10.0f)]
	public float AI_walkSpeed = 4.0f;
	[Range(0.1f,10.0f)]
	public float AI_runSpeed = 5.0f;



	bool killable = false;
	
	public GameObject Spawner;
	
	//LAYER MASK da usare
	//public LayerMask wallLayers;
	//public LayerMask groundBasic;
	public LayerMask targetLayers;
	public LayerMask fleeLayer;
	public LayerMask hidingLayer;
	public LayerMask cloneLayer;
	public LayerMask obstacleLayers;

	[SerializeField]
	public PatrolParameters patrolParameters;


	void Start(){

		pm = GetComponent<PlayerMovements> ();

		if(pm==null) {
			Debug.Log ("ATTENZIONE - script PlayerMovements non trovato da AIParameters");
		}


	}

	void Update(){

		pm.AI_walkSpeed = this.AI_walkSpeed;
		pm.AI_runSpeed = this.AI_runSpeed;

	}



}

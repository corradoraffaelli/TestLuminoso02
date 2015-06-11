using UnityEngine;
using System.Collections;

public class AIParameters : MonoBehaviour {

	PlayerMovements pm;

	//TODO: capire se inglobare playermovements... derivandola?
	//perché vorrei avere i parametri di velocità di movimento assieme agli altri parametri dell'AI

	//DEBUG----------------------

	//l'indice dell'array indica la profondità con cui si vuole scavare
	public bool []DEBUG_FSM_TRANSITION = new bool[3];
	public bool []DEBUG_RECOGNITION = new bool[2];
	public bool []DEBUG_ASTAR = new bool[2];
	public bool []DEBUG_FLIP = new bool[2];
	public bool []DEBUG_COLLISION = new bool[3];

	//GENERAL--------------------

	[Range(0.1f,10.0f)]
	public float _AIwalkSpeed = 4.0f;
	[Range(0.1f,10.0f)]
	public float _AIrunSpeed = 5.0f;

	public GameObject _target;
	public GameObject _fleeTarget;

	public bool stunnedReceived = false;

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
	
	[SerializeField]
	public ChaseParameters chaseParameters;

	[SerializeField]
	public FleeParameters fleeParameters;

	//NASCOSTI------------------
	[HideInInspector]
	public Rigidbody2D _rigidbody;
	[HideInInspector]
	public BoxCollider2D _boxCollider;
	[HideInInspector]
	public CircleCollider2D _circleCollider;
	[HideInInspector]
	public GameObject myWeakPoint;

	void Start(){

		pm = GetComponent<PlayerMovements> ();

		if(pm==null) {
			Debug.Log ("ATTENZIONE - script PlayerMovements non trovato da AIParameters");
		}

		_rigidbody = GetComponent<Rigidbody2D> ();

		if(_rigidbody == null)
			Debug.Log ("ATTENZIONE - rigidbody2D non trovato da AIParameters");

		_boxCollider = GetComponent<BoxCollider2D> ();

		if(_boxCollider == null)
			Debug.Log ("ATTENZIONE - boxcollider non trovato da AIParameters");

		_circleCollider = GetComponent<CircleCollider2D> ();

		if(_circleCollider == null)
			Debug.Log ("ATTENZIONE - circlecollider non trovato da AIParameters");

		getMyWeakPoint ();

	}

	private void getMyWeakPoint() {
		
		bool founded = false;
		
		foreach (Transform child in transform) {
			
			if(child.tag == "Weakness") {
				
				myWeakPoint = child.gameObject;
				founded = true;
				break;
				
			}
			
		}
		
		if (!founded) {
			Debug.Log ("Oggetto WeakPoint mancante, attenzione");
		}
		
	}

	void Update(){

		//pm.AI_walkSpeed = this.AI_walkSpeed;
		//pm.AI_runSpeed = this.AI_runSpeed;

	}



}

//-----------------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------------------

[System.Serializable]
public class PatrolParameters {
	
	//nuovi parametri
	
	[Range(0.1f,10.0f)]
	public float patrolSpeed = 4.0f;
	
	[HideInInspector]
	public GameObject patrolTarget;

	[HideInInspector]
	public GameObject foundTarget;
	
	//TODO: si dovrebbe eliminare...
	[HideInInspector]
	public float thresholdHeightDifference = 1f;
	
	public GameObject []patrolPoints;
	
	[HideInInspector]
	public float RangeOfView;
	//vecchi parametri
	
	//Gestione patrol----------------------------------------------------------------------------------
	
	//verso di default dove puntare lo sguardo nel caso di un singolo punto di patrol
	public bool DefaultVerseRight = true;
	
	
	
	[Range(0.1f,10.0f)]
	public float patrolSuspiciousSpeed = 2.0f;
	
	//nuova gestione suspicious
	bool firstCheckDone_Suspicious = false;
	[Range(0.1f,10.0f)]
	public float tSearchLenght = 2.5f;
	bool standingSusp = false;
	
	
	bool exitSuspicious = false;
	
	bool ExitSuspicious {
		get{ return exitSuspicious;}
		set { exitSuspicious = value;}
		
	}
	
	//variabili da resettare ad inizio stato
	bool patrollingTowardAPoint = false;
	Transform patrolledTarget;//utile dichiararlo momentaneamente public per vedere che valore ha
	
	[Range(0.1f,10.0f)]
	public float DEFAULT_DUMB_SPEED = 2.0f;
	
	//Gestione raycast target------------------------------------
	//public LayerMask targetLayers; dichiarata su
	float frontalDistanceOfView = 5.0f;
	float scale_FrontalDistanceOfView_ToBeFar = 1.5f;
	float backDistanceOfView = 2.0f;
}

//-----------------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------------------

[System.Serializable]
public class ChaseParameters {

	//nuovi parametri

	public float chaseSpeed = 6.0f;

	//[HideInInspector]
	public GameObject chaseTarget;
	//vecchi parametri

	[HideInInspector]
	public float RangeOfView;

	public float AdditionalROV = 2.0f;

	//Gestione chase-----------------------------------------------------------------------------------
	//Transform chasedTarget;
	//bool avoidingObstacles = false;
	[Range(0.1f,5.0f)]
	public float fTargetFar = 2.0f;
	
	//enemytype nojumpsoftchase
	float offset_MaxDistanceReachable_FromChase = 5.0f;
	bool chaseCharged = false;
	bool chaseCharging = false;
	[Range(0.1f,5.0f)]
	public float tChargingChase = 1.0f;
	float tStartCrash = -1.0f;
	[Range(0.1f,5.0f)]
	public float tToMaxVelocity = 0.5f;
	[Range(0.1f,5.0f)]
	public float tLosingTargerLenght = 5.5f;
	bool losingTarget = false;
	float tStartLosingTarget = -3.0f;
}

[System.Serializable]
public class FleeParameters {
	
	//nuovi parametri
	public float fleeSpeed = 6.0f;
	
	//[HideInInspector]
	public GameObject fleeTarget;

}
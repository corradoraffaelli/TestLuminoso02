using UnityEngine;
using System.Collections;

public class AIParameters : MonoBehaviour {

	PlayerMovements pm;

	[HideInInspector]
	public GameObject Spawner;

	public LayerMask targetLayers;
	public LayerMask fleeLayer;
	public LayerMask hidingLayer;
	//public LayerMask cloneLayer;
	public LayerMask obstacleLayers;

	[SerializeField]
	public StatusParameters statusParameters;

	[SerializeField]
	public StunnedParameters stunnedParameters;

	[SerializeField]
	public WanderParameters wanderParameters;

	[SerializeField]
	public PatrolParameters patrolParameters;
	
	[SerializeField]
	public ChaseParameters chaseParameters;

	[SerializeField]
	public FleeParameters fleeParameters;

	public bool canKillPlayer = true;
	
	//NASCOSTI------------------

	[HideInInspector]
	public bool stunnedReceived = false;

	[HideInInspector]
	public GameObject _target;
	
	[HideInInspector]
	public GameObject _fleeTarget;

	[HideInInspector]
	public int defaultLayer;
	
	[HideInInspector]
	public int deadLayer;

	[HideInInspector]
	public Rigidbody2D _rigidbody;

	[HideInInspector]
	public BoxCollider2D _boxCollider;

	[HideInInspector]
	public CircleCollider2D _circleCollider;

	[HideInInspector]
	public GameObject myWeakPoint;

	[HideInInspector]
	public BoxCollider2D myWeakPointCollider;

	[HideInInspector]
	public bool instantKill = false;

	[HideInInspector]
	public AudioHandler audioHandler;

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

		audioHandler = GetComponent<AudioHandler> ();

		getMyWeakPoint ();

		setLayers ();

	}

	void setLayers() {

		defaultLayer = gameObject.layer;
		deadLayer = LayerMask.NameToLayer("Dead");

	}

	private void getMyWeakPoint() {
		
		bool founded = false;
		
		foreach (Transform child in transform) {
			
			if(child.tag == "Weakness") {
				
				myWeakPoint = child.gameObject;

				myWeakPointCollider = myWeakPoint.GetComponent<BoxCollider2D>();

				founded = true;
				break;
				
			}
			
		}
		
		if (!founded) {
			Debug.Log ("Oggetto WeakPoint mancante, attenzione");
		}
		
	}

	void Update(){


	}

	public void c_setCanKillPlayer(bool can) {

		canKillPlayer = can;

	}


}

[System.Serializable]
public class StunnedParameters {
	
	[Range(0.1f,10.0f)]
	public float tStunnedLength = 3.0f;
	
}

[System.Serializable]
public class WanderParameters {
	
	[Range(0.1f,10.0f)]
	public float wanderSpeed = 4.0f;

}

[System.Serializable]
public class PatrolParameters {

	[Range(0.1f,10.0f)]
	public float patrolSpeed = 4.0f;
	
	[HideInInspector]
	public GameObject patrolTarget;

	[HideInInspector]
	public GameObject foundTarget;
	
	public GameObject []patrolPoints;
	
	[HideInInspector]
	public float RangeOfView;

	public bool DefaultVerseRight = true;

	[Range(0.1f,10.0f)]
	public float tSuspiciousLenght = 2.5f;

	[Range(0.1f,10.0f)]
	public float tSuspiciousIntervalFlip = 1.0f;


}

[System.Serializable]
public class ChaseParameters {
	
	public float chaseSpeed = 6.0f;

	[HideInInspector]
	public GameObject chaseTarget;

	public float RangeOfView;

	public float timeToLoseTarget = 1.0f;
	public float timeToCharge = 1.0f;
		
	//[Range(0.1f,5.0f)]
	//public float fTargetFar = 2.0f;

	float offset_MaxDistanceReachable_FromChase = 5.0f;

}

[System.Serializable]
public class FleeParameters {
	
	public float fleeSpeed = 6.0f;
	
	[HideInInspector]
	public GameObject fleeTarget;

}

[System.Serializable]
public class StatusParameters {

	public Sprite alarmed;

	public Sprite confused;

}
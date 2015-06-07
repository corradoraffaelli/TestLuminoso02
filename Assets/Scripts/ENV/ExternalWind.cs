using UnityEngine;
using System.Collections;

public class ExternalWind : MonoBehaviour {

	[Range(0.0f,15.0f)]
	public float offTime = 3.0f;
	[Range(0.0f,15.0f)]
	public float onTime = 6.0f;

	float beginOnTime = 0.0f;
	float beginOffTime = 0.0f;

	public bool turnedOn = true;

	AreaEffector2D areaEffector;

	public bool intermittance = true;

	public bool subWind = false;
	public GameObject masterWind;
	ExternalWind masterExtWind;

	//usato per evitare problemi lungo la scala
	GameObject player;
	PlayerMovements playerMovements;
	bool activeOnPlayer = true;

	// Use this for initialization
	void Start () {
		areaEffector = GetComponent<AreaEffector2D>();
		if (subWind && masterWind != null)
			masterExtWind = masterWind.GetComponent<ExternalWind>();

		player = GameObject.FindGameObjectWithTag ("Player");
		if (player != null)
			playerMovements = player.GetComponent<PlayerMovements> ();
	}
	
	// Update is called once per frame
	void Update () {
		playerLadderManagement();

		if (!subWind)
		{
			if (intermittance)
			{
				if (turnedOn)
				{
					if (Mathf.Abs(Time.time - beginOnTime) > onTime)
					{
						turnedOn = false;
						areaEffector.enabled = false;
					}
					beginOffTime = Time.time;
				}else{
					if (Mathf.Abs(Time.time - beginOffTime) > offTime)
					{
						turnedOn = true;
						areaEffector.enabled = true;
					}
					beginOnTime = Time.time;
				}
			}
		}else{
			if (masterExtWind != null)
			{
				if (!masterExtWind.turnedOn)
				{
					areaEffector.enabled = false;
					turnedOn = false;
				}else{
					areaEffector.enabled = true;
					turnedOn = true;
				}
			}
				
					
		}

	}

	void playerLadderManagement()
	{
		if (playerMovements.onLadder && activeOnPlayer) {
			activeOnPlayer = false;
			
			// Bit shift dell'indice del player per avere una layer mask con solo il bit del player attivo
			int layerMask = 1 << (LayerMask.NameToLayer ("Player"));
			
			//L'operatore ~ inverte una layerMask (perciò si ottiene la layerMask con tutto, escluso il player
			layerMask = ~layerMask;
			
			//cambio quindi la layermask su cui ha effetto il vento
			areaEffector.colliderMask = layerMask;
		}
		
		if (!playerMovements.onLadder && !activeOnPlayer) {
			//la layer mask -1 indica tutti i layer
			areaEffector.colliderMask = -1;
			
			activeOnPlayer = true;
		}
	}

	void flipDirection()
	{

	}

	void setIntermittance()
	{

	}
}

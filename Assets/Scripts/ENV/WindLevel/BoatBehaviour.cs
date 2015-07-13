using UnityEngine;
using System.Collections;

public class BoatBehaviour : MonoBehaviour {

	public GameObject noExitLimit;
	public GameObject rightExitLimit;
	public GameObject anchor;
	public GameObject sails;
	public GameObject destination;
	public GameObject door;

	SailsManagement sailsManagement;
	LanternOnBoat lanternOnBoat;

	bool playerOnBoard = false;
	bool sailsMoving = false;
	bool destinationReached = false;

	GameObject lantern;
	GameObject player;

	Animator animator;

	public float boatMaxSpeed = 1.0f;
	float actualBoatSpeed = 0.0f;
	public float lerpSpeed = 1.0f;

	float oldXPosition = 0.0f;

	bool oldLanternParentControl = false;

	// Use this for initialization
	void Start () {
		//lantern = GameObject.FindGameObjectWithTag("Lantern");
		//player = GameObject.FindGameObjectWithTag("Player");
		lantern = GeneralFinder.magicLantern;
		player = GeneralFinder.player;
		if (sails != null)
			sailsManagement = sails.GetComponent<SailsManagement>();
		animator = GetComponent<Animator>();

		lanternOnBoat = GetComponentInChildren<LanternOnBoat>();

		oldXPosition = transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		//DEBUG TESTTT
		/*
		if (Input.GetButtonUp("Jump"))
			door.SendMessage("buttonPushed", true);
		if (Input.GetButtonUp("Mira"))
			door.SendMessage("buttonPushed", false);
			*/


		if (playerOnBoard)
		{
			lantern = GameObject.FindGameObjectWithTag("Lantern");

			bool actualLanternParentControl = lantern!= null && lantern.activeInHierarchy && lantern.transform.parent == null && lanternOnBoat.IsLanternOnBoat;

			//if (lantern!= null && lantern.activeInHierarchy && lantern.transform.parent == null && lanternOnBoat.IsLanternOnBoat)
			if (actualLanternParentControl && !oldLanternParentControl)
			{
				lantern.transform.parent = transform;
				Debug.Log ("dentro");
			}
			oldLanternParentControl = actualLanternParentControl;
				
		}

		controlDestination();

		setBoatAnimations();

		updateBoatPosition();
	}

	//metodo richiamato quando viene tolta l'ancora
	public void InteractingMethod(){
		Debug.Log ("tolta ancora");

		//se si toglie l'ancora il player non può più scendere
		if (noExitLimit != null)
			noExitLimit.SetActive(true);

		//disattivo la sprite dell'ancora
		if (anchor != null){
			SpriteRenderer spriteRenderer = anchor.GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
				spriteRenderer.enabled = false;
		}

		//setto un bool che mi dice che il player è a bordo
		playerOnBoard = true;

		//player.transform.parent = transform;

		//riprendo in mano la lanterna per evitare che venga lasciata a terra, nonostante sia figlia della barca
		/*
		GameObject magicLanternLogicOBJ = GameObject.FindGameObjectWithTag("MagicLanternLogic");
		//MagicLantern magicLanternLogic;
		if (magicLanternLogicOBJ != null){
			MagicLantern magicLanternLogic = magicLanternLogicOBJ.GetComponent<MagicLantern>();
			if (magicLanternLogic != null)
				magicLanternLogic.actualState = MagicLantern.lanternState.NotUsed;
		}
		*/
	}

	void setBoatAnimations()
	{
		if (sailsManagement != null)
		{
			bool activeSails = sailsManagement.active;

			if (activeSails)
			{
				animator.SetBool("starting", true);
				animator.SetBool("stopping", false);
				sailsMoving = true;
			}else{
				animator.SetBool("starting", false);
				animator.SetBool("stopping", true);
				sailsMoving = false;
			}
		}
	}

	void updateBoatPosition()
	{
		if (sailsMoving && playerOnBoard && !destinationReached)
		{
			actualBoatSpeed = Mathf.Lerp(actualBoatSpeed, boatMaxSpeed /30.0f, lerpSpeed*Time.deltaTime);
			transform.position = new Vector3(transform.position.x + actualBoatSpeed, transform.position.y, transform.position.z);
		}else{
			actualBoatSpeed = Mathf.Lerp(actualBoatSpeed, 0.0f, lerpSpeed*Time.deltaTime);
			transform.position = new Vector3(transform.position.x + actualBoatSpeed, transform.position.y, transform.position.z);
		}
	}

	void controlDestination()
	{
		if (destination != null)
		{
			if (transform.position.x > destination.transform.position.x)
			{
				destinationReached = true;
				if (rightExitLimit != null)
					rightExitLimit.SetActive(false);
			}
				
		}
	}
}

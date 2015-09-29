using UnityEngine;
using System.Collections;

public class LanternFanfareController : MonoBehaviour {
	
	[System.Serializable]
	public class GroundRisingVar
	{
		public GameObject groundToRise;
		public Transform destination;
		[Range(0.1f, 2.0f)]
		public float risingSpeed = 4.0f;
		[HideInInspector]
		public Vector3 beginPosition;
	}

	[SerializeField]
	GroundRisingVar groundRisingVar;

	[System.Serializable]
	public class LanternImageVar
	{
		public GameObject lanternImage;
		[HideInInspector]
		public SpriteRenderer spriteRenderer;
		public Transform destination;
		public float timeToWait = 1.5f;
		public float risingSpeed = 3.0f;
		public float maxAlphaSpeed = 3.0f;
		[HideInInspector]
		public float actAlpha = 0.0f;
		public Vector3 maxScale = new Vector3 (0.1f, 0.1f, 0.1f);
	}
	
	[SerializeField]
	LanternImageVar lanternImageVar;

	[System.Serializable]
	public class CameraVar
	{
		public GameObject cameraDestination;
		[Range(5.0f, 25.0f)]
		public float risingSpeed = 4.0f;
	}
	
	[SerializeField]
	CameraVar cameraVar;

	[System.Serializable]
	public class DarkScreenVar
	{
		public GameObject darkScreenObject;
		[HideInInspector]
		public SpriteRenderer spriteRenderer;
		[Range(1.0f, 25.0f)]
		public float alphaSpeed = 4.0f;
		[HideInInspector]
		public float actAlpha = 0.0f;
		public float maxAlpha = 0.5f;
	}
	
	[SerializeField]
	DarkScreenVar darkScreenVar;

	[System.Serializable]
	public class SpotlightVar
	{
		public GameObject spotlightFather;
		public GameObject spotlightObject;
		public GameObject rayObject;
		public GameObject circleObject;

		public Vector3 circleScale = new Vector3(3.0f, 3.0f, 3.0f);
		public bool pointOnPlayer = false;

		[HideInInspector]
		public float xSizeRay;

		[HideInInspector]
		public float rundomNumber;
		//il tempo beginTime indica quando inizia a lampeggiare
		[HideInInspector]
		public float beginTime;
		//il tempo turnOn indica quando deve accendersi una volta che ha lampeggiato per un po'
		[HideInInspector]
		public float turnOnTime;

		[HideInInspector]
		public bool turnedOn = false;

		[HideInInspector]
		public Vector3 finalPosition;
	}
	
	[SerializeField]
	SpotlightVar[] spotlightVar;

	[System.Serializable]
	public class SpotlightTimeVar
	{
		public float beginTime = 2.8f;
		public float intermittanceTime = 0.3f;
		public float turnOnTime = 4.5f;
		public float maxRandomBeginTime = 1.0f;
		public float appearTime = 2.0f;
		public float appearSpeed = 10.0f;
	}
	
	[SerializeField]
	SpotlightTimeVar spotlightTimeVar;

	[System.Serializable]
	public class LanternTextVar
	{
		public GameObject textObject01;
		[HideInInspector]
		public Vector3 begin01Position;
		[HideInInspector]
		public Vector3 begin01Scale;
		[HideInInspector]
		public SpriteRenderer spriteRenderer01;

		public GameObject textObject02;
		[HideInInspector]
		public Vector3 begin02Position;
		[HideInInspector]
		public Vector3 begin02Scale;
		[HideInInspector]
		public SpriteRenderer spriteRenderer02;

		public float appear01Time = 4.0f;
		public float appear01Speed = 10.0f;
		public float appear02Time = 5.2f;
		public float appear02Speed = 10.0f;
	}
	
	[SerializeField]
	LanternTextVar lanternTextVar;

	[System.Serializable]
	public class CoriandoliVar
	{
		public GameObject coriandoliObject;
		[HideInInspector]
		public ParticleRenderer particleRenderer;
		[HideInInspector]
		public ParticleEmitter particleEmitter;
		[HideInInspector]
		public float standardEmission = 4.0f;
	}
	
	[SerializeField]
	CoriandoliVar[] coriandoliVar;

	[System.Serializable]
	public class ProseguireVar
	{
		public GameObject textObject;
		public float timeToAppear = 13.0f;
		public string sortingLayer = "SceneUI";
		public int sortingNumber = 0;
		public Sprite controllerSprite;
		public Sprite keyboardSprite;
	}
	
	[SerializeField]
	ProseguireVar proseguireVar;

	public string particleSortingLayer = "Lanterna";
	public int particleSortingNumber = 1;

	bool effectConsumed = false;
	public bool effectStarted = false;
	public bool effectInverted = false;
	bool firstStart = false;
	AudioHandler audioHandler;

	Animator playerAnimator;
	Rigidbody2D playerRigidbody;

	float beginTime;

	bool coriandoliEmitting = false;
	bool coriandoliEmittingOLD = false;

	bool proseguireCreated = false;
	bool proseguireCreatedOLD = false;

	bool firstDisabling = false;
	bool firstDisablingOLD = false;
	
	void Start () {
		playerAnimator = GeneralFinder.player.GetComponent<Animator> ();
		playerRigidbody = GeneralFinder.player.GetComponent<Rigidbody2D> ();

		audioHandler = GetComponent<AudioHandler> ();

		if (darkScreenVar.darkScreenObject != null)
			darkScreenVar.spriteRenderer = darkScreenVar.darkScreenObject.GetComponent<SpriteRenderer> ();

		if (lanternImageVar.lanternImage != null)
			lanternImageVar.spriteRenderer = lanternImageVar.lanternImage.GetComponent<SpriteRenderer> ();

		if (groundRisingVar.groundToRise != null)
			groundRisingVar.beginPosition = groundRisingVar.groundToRise.transform.position;

		takeSpotlightElements();
		lanternTextStart ();
		coriandoliStart ();
		proseguireStart ();
	}

	void Update () {
		//DA TOGLIERE
		//if (!effectConsumed && Input.GetKeyUp (KeyCode.T))
		//	startEffect ();



		if (firstStart) {
			firstStart = false;
			if (audioHandler != null)
			{
				audioHandler.playClipByName("Fanfare");
				cameraDefaultEnable(false);
				enablePlayerMovements(false);
			}
		}

		if (effectStarted) {
			if (GeneralFinder.inputKeeper.isButtonUp("Interaction"))
			{
				if ((Time.time - beginTime) > proseguireVar.timeToAppear)
				{
					effectStarted = false;
					effectInverted = true;

					firstDisabling = true;

					cameraDefaultEnable(true);
					enablePlayerMovements(true);
				}
			}
		}

		groundRisingHandler ();
		cameraMovementsHandler ();
		darkScreenHandler ();
		lanternBehaveHandler ();
		spotlightsMovements ();
		spotlightsGraphic ();
		spotlightAppear ();
		lanternTextUpdate ();
		coriandoliUpdate ();
		proseguireUpdate ();

		firstDisablingOLD = firstDisabling;
	}

	public void startEffect()
	{
		beginTime = Time.time;
		effectConsumed = true;
		effectStarted = true;
		firstStart = true;
		//Debug.Log ("Avviati effetti lanterna");
		//chooseProseguireSprite ();
	}

	public void InteractingMethod(){
		startEffect();
	}

	void enablePlayerMovements(bool enable)
	{
		GeneralFinder.playerMovements.enabled = enable;
		if (!enable) {
			playerAnimator.SetBool ("onGround", true);
			playerAnimator.SetBool ("Running", false);
			playerRigidbody.velocity = Vector2.zero;
		}
	}

	void groundRisingHandler()
	{
		if (effectStarted) {
			if (groundRisingVar.destination != null && groundRisingVar.groundToRise != null) {
				Vector3 actPos = groundRisingVar.groundToRise.transform.position;
				Vector3 newPos = Vector3.Lerp (actPos, groundRisingVar.destination.transform.position, Time.deltaTime * groundRisingVar.risingSpeed);
				groundRisingVar.groundToRise.transform.position = newPos;
			}
		} 
		else if (effectInverted) 
		{
			if (groundRisingVar.beginPosition != null && groundRisingVar.groundToRise != null) {
				Vector3 actPos = groundRisingVar.groundToRise.transform.position;
				Vector3 newPos = Vector3.MoveTowards (actPos, groundRisingVar.beginPosition, Time.deltaTime * groundRisingVar.risingSpeed * 4.0f);
				groundRisingVar.groundToRise.transform.position = newPos;
			}
		}
	}

	void cameraDefaultEnable(bool enable)
	{
		GeneralFinder.cameraMovements.enabled = enable;
	}

	void cameraMovementsHandler()
	{
		if (effectStarted) {
			if (cameraVar.cameraDestination != null)
			{
				Vector3 actPos = GeneralFinder.camera.transform.position;
				Vector3 destPos = new Vector3(cameraVar.cameraDestination.transform.position.x, cameraVar.cameraDestination.transform.position.y,
				                              actPos.z);
				Vector3 newPos = Vector3.MoveTowards(actPos, destPos, Time.deltaTime * cameraVar.risingSpeed / 10.0f);
				GeneralFinder.camera.transform.position = newPos;
			}
		}
	}

	void darkScreenHandler()
	{
		if (effectStarted) {
			if (darkScreenVar.darkScreenObject != null) {
				darkScreenVar.actAlpha = Mathf.MoveTowards (darkScreenVar.actAlpha, darkScreenVar.maxAlpha, Time.deltaTime * darkScreenVar.alphaSpeed / 10.0f);

				if (darkScreenVar.spriteRenderer != null) {
					Color oldColor = darkScreenVar.spriteRenderer.color;
					Color newColor = new Color (oldColor.r, oldColor.g, oldColor.b, darkScreenVar.actAlpha);
					darkScreenVar.spriteRenderer.color = newColor;
				}
			}
		} 
		else if (effectInverted) 
		{
			if (darkScreenVar.darkScreenObject != null) {
				darkScreenVar.actAlpha = Mathf.MoveTowards (darkScreenVar.actAlpha, 0.0f, Time.deltaTime * darkScreenVar.alphaSpeed / 2.0f);
				
				if (darkScreenVar.spriteRenderer != null) {
					Color oldColor = darkScreenVar.spriteRenderer.color;
					Color newColor = new Color (oldColor.r, oldColor.g, oldColor.b, darkScreenVar.actAlpha);
					darkScreenVar.spriteRenderer.color = newColor;
				}
			}
		}
	}

	void lanternBehaveHandler()
	{
		if (effectStarted) {
			if (lanternImageVar.lanternImage != null) {
				//POSITION
				Vector3 actPos = lanternImageVar.lanternImage.transform.position;
				Vector3 newPos = Vector3.Lerp (actPos, lanternImageVar.destination.position, Time.deltaTime * lanternImageVar.risingSpeed / 10.0f);
				lanternImageVar.lanternImage.transform.position = newPos;

				//SCALE
				Vector3 newScale = lanternImageVar.lanternImage.transform.localScale;
				newScale = Vector3.MoveTowards (newScale, lanternImageVar.maxScale, Time.deltaTime * lanternImageVar.maxAlphaSpeed / 10.0f);
				lanternImageVar.lanternImage.transform.localScale = newScale;

				//ALPHA
				if (lanternImageVar.spriteRenderer != null) {
					lanternImageVar.actAlpha = Mathf.MoveTowards (lanternImageVar.actAlpha, 1.0f, Time.deltaTime * lanternImageVar.maxAlphaSpeed / 10.0f);

					Color oldColor = lanternImageVar.spriteRenderer.color;
					Color newColor = new Color (oldColor.r, oldColor.g, oldColor.b, lanternImageVar.actAlpha);
					lanternImageVar.spriteRenderer.color = newColor;
				}
			}
		} 
		else if (effectInverted) 
		{
			if (lanternImageVar.lanternImage != null) {
				lanternImageVar.lanternImage.SetActive(false);
			}
		}
	}

	//setta le variabili standard dei faretti e li spegne e li sposta
	void takeSpotlightElements()
	{
		for (int i = 0; i < spotlightVar.Length; i++) {
			if (spotlightVar[i] != null && spotlightVar [i].spotlightFather != null && spotlightVar [i].rayObject != null && spotlightVar[i].circleObject != null) 
			{
				SpriteRenderer renderer = spotlightVar [i].rayObject.GetComponent<SpriteRenderer>();
				if (renderer != null)
				{
					Bounds boundsRay = renderer.bounds;
					spotlightVar[i].xSizeRay = boundsRay.size.x;
				}

				//tempi random
				spotlightVar[i].rundomNumber = Random.Range(0.0f, spotlightTimeVar.maxRandomBeginTime);
				spotlightVar[i].beginTime = spotlightVar[i].rundomNumber + spotlightTimeVar.beginTime;
				spotlightVar[i].turnOnTime = spotlightVar[i].rundomNumber + spotlightTimeVar.turnOnTime;

				//spengo le luci e disattivo l'oggetto
				spotlightVar[i].rayObject.SetActive(false);
				spotlightVar[i].circleObject.SetActive(false);
				spotlightVar[i].spotlightObject.SetActive(false);

				//sposto gli oggetti
				spotlightVar[i].finalPosition = spotlightVar[i].spotlightFather.transform.position;
				if (spotlightVar[i].spotlightFather.transform.localPosition.x < 0.0f)
					spotlightVar[i].spotlightFather.transform.position = new Vector3(spotlightVar[i].finalPosition.x - 5.0f, spotlightVar[i].finalPosition.y, spotlightVar[i].finalPosition.z);
				else
					spotlightVar[i].spotlightFather.transform.position = new Vector3(spotlightVar[i].finalPosition.x + 5.0f, spotlightVar[i].finalPosition.y, spotlightVar[i].finalPosition.z);
			}
		}
	}

	//movimenti del raggio e dei faretti
	void spotlightsMovements(){
			
		if (effectStarted) {
			for (int i = 0; i < spotlightVar.Length; i++) {
				if (spotlightVar[i] != null && spotlightVar[i].spotlightFather != null && spotlightVar[i].spotlightObject != null
				    && spotlightVar[i].rayObject != null && spotlightVar[i].circleObject != null)
				{
					//salvo la posizione dell'obiettivo del faretto
					Vector3 objPosition;
					if (spotlightVar[i].pointOnPlayer)
					{
						Vector3 playerPos = GeneralFinder.player.transform.position;
						objPosition = new Vector3 (playerPos.x, playerPos.y + 0.6f, playerPos.z);
					}
						
					else
						objPosition = lanternImageVar.lanternImage.transform.position;
					
					//prendo la direzione tra il fulcro del faretto e il punto obiettivo
					Vector3 _direction = (objPosition - spotlightVar[i].spotlightFather.transform.position).normalized;
					
					//ruoto di conseguenza
					spotlightVar[i].spotlightFather.transform.right = _direction;

					//piazzo il cerchio nell'obiettivo del faretto
					spotlightVar[i].circleObject.transform.position = objPosition;

					//cambio la scala del cerchio secondo quanto stabilito
					spotlightVar[i].circleObject.transform.localScale = spotlightVar[i].circleScale;

					//cambio la scala del raggio in base alla distanza tra faretto e obiettivo
					float distance = Vector3.Distance (objPosition, spotlightVar[i].rayObject.transform.position);			
					spotlightVar[i].rayObject.transform.localScale = new Vector3(distance * 2.5f / spotlightVar[i].xSizeRay,spotlightVar[i].circleScale.x,1);
				}
			}
		}
	}

	//controllo tutti i tempi e dico se un faretto deve essere acceso o meno
	void spotlightsGraphic(){
		if (effectStarted) {
			for (int i = 0; i < spotlightVar.Length; i++) {
				if (spotlightVar [i] != null) {
					bool mustBeActive = false;

					//se è passato il tempo dovuto si accendono e basta
					if ((Time.time - beginTime) > spotlightVar [i].turnOnTime) {
						mustBeActive = true;
					} else {
						mustBeActive = false;
						if ((Time.time - beginTime) > spotlightVar [i].beginTime) {
							//se il resto della divisione bla bla è pari, pure si devono accendere
							if ((int)((Time.time - beginTime) / spotlightTimeVar.intermittanceTime) % 2 == 0)
								mustBeActive = true;
						}

					}

					if (mustBeActive != spotlightVar [i].turnedOn) {
						spotlightVar [i].rayObject.SetActive (mustBeActive);
						spotlightVar [i].circleObject.SetActive (mustBeActive);
					}

					spotlightVar [i].turnedOn = mustBeActive;
				}
			}
		} 
		else if (effectInverted && firstDisabling != firstDisablingOLD) 
		{
			for (int i = 0; i < spotlightVar.Length; i++) {
				if (spotlightVar [i] != null) {
					spotlightVar [i].rayObject.SetActive (false);
					spotlightVar [i].circleObject.SetActive (false);
				}
			}
		}
	}

	void spotlightAppear()
	{
		if (effectStarted) {
			for (int i = 0; i < spotlightVar.Length; i++) {
				if (spotlightVar [i] != null) {
					if ((Time.time - beginTime) > spotlightTimeVar.appearTime) {
						spotlightVar [i].spotlightFather.transform.position = Vector3.MoveTowards (
							spotlightVar [i].spotlightFather.transform.position, spotlightVar [i].finalPosition, Time.deltaTime * spotlightTimeVar.appearSpeed);

						spotlightVar [i].spotlightObject.SetActive (true);
					}

				}
			}
		} 
		else if (effectInverted) 
		{
			for (int i = 0; i < spotlightVar.Length; i++) {
				if (spotlightVar [i] != null) {
					if ((spotlightVar[i].spotlightFather.transform.localPosition.x < 0.0f &&
					    spotlightVar [i].spotlightFather.transform.position.x == spotlightVar[i].finalPosition.x - 5.0f) ||
					    (spotlightVar[i].spotlightFather.transform.localPosition.x > 0.0f &&
					 	spotlightVar [i].spotlightFather.transform.position.x == spotlightVar[i].finalPosition.x + 5.0f))
						spotlightVar [i].spotlightObject.SetActive (false);
					else
					{
						Vector3 objPosition;
						if (spotlightVar[i].spotlightFather.transform.localPosition.x < 0.0f)
							objPosition = new Vector3(spotlightVar[i].finalPosition.x - 5.0f, spotlightVar[i].finalPosition.y, spotlightVar[i].finalPosition.z);
						else
							objPosition = new Vector3(spotlightVar[i].finalPosition.x + 5.0f, spotlightVar[i].finalPosition.y, spotlightVar[i].finalPosition.z);
						
						spotlightVar [i].spotlightFather.transform.position = Vector3.MoveTowards (
							spotlightVar [i].spotlightFather.transform.position, objPosition, Time.deltaTime * spotlightTimeVar.appearSpeed);
						
						spotlightVar [i].spotlightObject.SetActive (true);
					}
				}
			}
		}
	}

	void lanternTextStart()
	{
		if (lanternTextVar.textObject01 != null && lanternTextVar.textObject02 != null) {
			lanternTextVar.begin01Position = lanternTextVar.textObject01.transform.position;
			lanternTextVar.begin01Scale = lanternTextVar.textObject01.transform.localScale;
			lanternTextVar.begin02Position = lanternTextVar.textObject02.transform.position;
			lanternTextVar.begin02Scale = lanternTextVar.textObject02.transform.localScale;
			lanternTextVar.spriteRenderer01 = lanternTextVar.textObject01.GetComponent<SpriteRenderer>();
			lanternTextVar.spriteRenderer02 = lanternTextVar.textObject02.GetComponent<SpriteRenderer>();
			Color color01 = lanternTextVar.spriteRenderer01.color;
			color01 = new Color(color01.r, color01.g, color01.b, 0.0f);
			lanternTextVar.spriteRenderer01.color = color01;
			Color color02 = lanternTextVar.spriteRenderer02.color;
			color02 = new Color(color02.r, color02.g, color02.b, 0.0f);
			lanternTextVar.spriteRenderer02.color = color02;

			lanternTextVar.textObject01.transform.localScale = lanternTextVar.begin01Scale / 3.0f;
			lanternTextVar.textObject02.transform.localScale = lanternTextVar.begin02Scale * 5.0f;
		}
	}

	void lanternTextUpdate()
	{
		if (effectStarted) {
			if ((Time.time - beginTime) > lanternTextVar.appear01Time) {
				lanternTextVar.textObject01.transform.localScale = Vector3.MoveTowards (
					lanternTextVar.textObject01.transform.localScale, lanternTextVar.begin01Scale, Time.deltaTime * lanternTextVar.appear01Speed / 5.0f);

				Color color01 = lanternTextVar.spriteRenderer01.color;
				float alpha = Mathf.MoveTowards (color01.a, 10.0f, Time.deltaTime * lanternTextVar.appear01Speed);
				color01 = new Color (color01.r, color01.g, color01.b, alpha);
				lanternTextVar.spriteRenderer01.color = color01;
			}

			if ((Time.time - beginTime) > lanternTextVar.appear02Time) {
				lanternTextVar.textObject02.transform.localScale = Vector3.MoveTowards (
					lanternTextVar.textObject02.transform.localScale, lanternTextVar.begin02Scale, Time.deltaTime * lanternTextVar.appear02Speed / 4.0f);
				
				Color colo02 = lanternTextVar.spriteRenderer02.color;
				float alpha = Mathf.MoveTowards (colo02.a, 10.0f, Time.deltaTime * lanternTextVar.appear01Speed);
				colo02 = new Color (colo02.r, colo02.g, colo02.b, alpha);
				lanternTextVar.spriteRenderer02.color = colo02;
			}
		} else if (effectInverted && firstDisabling != firstDisablingOLD) {
			if (lanternTextVar.textObject01 != null && lanternTextVar.textObject02 != null)
			{
				lanternTextVar.textObject01.SetActive(false);
				lanternTextVar.textObject02.SetActive(false);
			}

		}
	}

	void coriandoliStart()
	{
		for (int i = 0; i < coriandoliVar.Length; i++)
		{
			if (coriandoliVar[i] != null && coriandoliVar[i].coriandoliObject != null)
			{
				coriandoliVar[i].particleEmitter = coriandoliVar[i].coriandoliObject.GetComponent<ParticleEmitter>();
				coriandoliVar[i].standardEmission = coriandoliVar[i].particleEmitter.maxEmission;
				coriandoliVar[i].particleEmitter.emit = false;

				coriandoliVar[i].particleRenderer = coriandoliVar[i].coriandoliObject.GetComponent<ParticleRenderer>();
				coriandoliVar[i].particleRenderer.sortingLayerName = particleSortingLayer;
				coriandoliVar[i].particleRenderer.sortingOrder = particleSortingNumber;
			}
		}
	}

	void coriandoliUpdate()
	{
		if (effectStarted) {
			coriandoliEmitting = true;
		} else {
			coriandoliEmitting = false;
		}

		if (coriandoliEmitting != coriandoliEmittingOLD) {
			for (int i = 0; i < coriandoliVar.Length; i++)
			{
				if (coriandoliVar[i] != null && coriandoliVar[i].particleEmitter != null)
				{
					coriandoliVar[i].particleEmitter.emit = coriandoliEmitting;
				}
			}
		}

		coriandoliEmittingOLD = coriandoliEmitting;
	}

	void proseguireStart()
	{
		if (proseguireVar.textObject != null) {
			Renderer[] renderers = proseguireVar.textObject.GetComponentsInChildren<Renderer>();
			//Debug.Log (renderers.Length);
			for (int i = 0; i < renderers.Length; i++)
			{
				if (renderers[i] != null)
				{
					renderers[i].sortingLayerName = proseguireVar.sortingLayer;
					renderers[i].sortingOrder = proseguireVar.sortingNumber;
				}
			}

			proseguireVar.textObject.SetActive(false);
		}
	}

	void proseguireUpdate()
	{
		if (effectStarted) {
			if ((Time.time - beginTime) > proseguireVar.timeToAppear) {
				proseguireCreated = true;
			}

			if (proseguireCreated != proseguireCreatedOLD && proseguireVar.textObject != null) {
				proseguireVar.textObject.SetActive (true);
				SpriteRenderer spriteRenderer = proseguireVar.textObject.GetComponentInChildren<SpriteRenderer> ();
				if (spriteRenderer != null) {
					/*
					if (GeneralFinder.cursorHandler.useController)
						spriteRenderer.sprite = proseguireVar.controllerSprite;
					else
						spriteRenderer.sprite = proseguireVar.keyboardSprite;
						*/
					spriteRenderer.sprite = GeneralFinder.inputManager.getSprite("Interaction");
				}
			}
		
			proseguireCreatedOLD = proseguireCreated;
		} 
		else if (effectInverted && firstDisabling != firstDisablingOLD) 
		{
			if (proseguireVar.textObject != null)
				proseguireVar.textObject.SetActive (false);
		}
	}
}

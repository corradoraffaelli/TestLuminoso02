using UnityEngine;
using System.Collections;

public class LanternTutorialHandler : MonoBehaviour {

	//bool utile per settare in che situazioni l'aiuto deve comparire
	public bool active = true;
	bool wasActive = true;

	public ComicBalloonManager.Type type;

	GameObject player;
	MagicLantern magicLantern;

	[Range (0.0f, 180.0f)]
	public float timeToTrigger = 0.0f;
	float totalTime = 0.0f;
	[Range (0.0f, 180.0f)]
	public float balloonTimeToDisable = 6.0f;
	float balloonLastAppearTime;
	[Range (0.0f, 180.0f)]
	public float balloonReappearTime = 10.0f;
	float balloonLastDisappearTime = 0.0f;

	public GameObject[] gameObjectsToEnable;
	public GameObject[] gameObjectsToEnableController;
	SpriteRenderer[] spriteRenderers;

	public string balloonString;
	public string balloonStringController;
	public GameObject balloonPrefab;
	public bool balloonOnTheRight = true;
	bool addedBalloon = false;
	bool removedBalloon = true;
	bool removeBalloon = false;
	GameObject balloon;
	ComicBalloonManager balloonManagerScript;
	
	bool playerColliding;
	bool enabling = false;
	bool wasEnabling = false;

	public LanternTutorialHandler[] tutToEnable;
	public LanternTutorialHandler[] tutToEnableOnDisable;
	public LanternTutorialHandler[] tutToDisable;
	public LanternTutorialHandler[] tutToDisableOnDisable;

	public MagicLantern.lanternState[] lanternStates;

	public float changingAlphaSpeed = 1.0f;

	bool wasUseController = false;
	
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		magicLantern = UtilFinder._GetComponentOfGameObjectWithTag<MagicLantern>("MagicLanternLogic");

		fillSpriteRenderers();
		setRenderersZeroAlpha();

		wasActive = active;
	}
	
	void Update () {
		if (active)
		{
			if (playerColliding && !enabling)
				totalTime = totalTime + Time.deltaTime;
			//abilito gli oggetti solo se il player è nel trigger e lo stato della lanterna è quello specificato
			enabling = (playerColliding && controlIfState() && controlIfTime());
			if (enabling)
				enableDisableTut();
			enablingObjects (enabling);

			setBalloonDisappear();
			balloonManager();

			if (!enabling && wasEnabling)
			{
				enableDisableTutOnDisable();
			}

			wasEnabling = enabling;
		}
		else
		{
			if (wasActive)
			{
				if (balloon != null && balloonManagerScript != null)
					balloonManagerScript.startDisappear();
				enableDisableTutOnDisable();
				totalTime = 0.0f;
			}
		}


		wasActive = active;

		bool useController = GeneralFinder.cursorHandler.useController;
		if (wasUseController != useController)
			fillSpriteRenderers();
		wasUseController = useController;
	}

	void fillSpriteRenderers()
	{
		if (!GeneralFinder.cursorHandler.useController)
			spriteRenderers = new SpriteRenderer[gameObjectsToEnable.Length];
		else
			spriteRenderers = new SpriteRenderer[gameObjectsToEnableController.Length];

		for (int i = 0; i< spriteRenderers.Length; i++)
		{
			if (gameObjectsToEnable[i] != null && !GeneralFinder.cursorHandler.useController)
			{
					spriteRenderers[i] = gameObjectsToEnable[i].GetComponent<SpriteRenderer>();
			}
			else if (gameObjectsToEnableController[i] != null && GeneralFinder.cursorHandler.useController)
			{
				spriteRenderers[i] = gameObjectsToEnableController[i].GetComponent<SpriteRenderer>();
			}
				
			if (spriteRenderers[i] != null)
			{
				Color tempColor = spriteRenderers[i].color;
				tempColor = new Color(tempColor.r, tempColor.g, tempColor.b, 0.0f);
				spriteRenderers[i].color = tempColor;
			}
		}
	}

	void setRenderersZeroAlpha()
	{
		if (gameObjectsToEnable != null)
		{
			for (int i = 0; i < gameObjectsToEnable.Length; i++)
			{
				if (gameObjectsToEnable[i] != null)
				{
					SpriteRenderer tempSpriteRenderer = gameObjectsToEnable[i].GetComponent<SpriteRenderer>();
					if (tempSpriteRenderer != null)
					{
						Color tempColor = tempSpriteRenderer.color;
						tempSpriteRenderer.color = new Color(tempColor.r, tempColor.g, tempColor.b, 0.0f);
					}
				}
			}
		}
		
		if (gameObjectsToEnableController != null)
		{
			for (int i = 0; i < gameObjectsToEnableController.Length; i++)
			{
				if (gameObjectsToEnableController[i] != null)
				{
					SpriteRenderer tempSpriteRenderer = gameObjectsToEnableController[i].GetComponent<SpriteRenderer>();
					if (tempSpriteRenderer != null)
					{
						Color tempColor = tempSpriteRenderer.color;
						tempSpriteRenderer.color = new Color(tempColor.r, tempColor.g, tempColor.b, 0.0f);
					}
				}
			}
		}
	}

	bool controlIfState()
	{
		//ritorna vero anche se non è specificato nessun lanternState
		if (lanternStates.Length == 0)
			return true;

		for (int i = 0; i < lanternStates.Length; i++)
		{
			if (lanternStates[i] != null && lanternStates[i] == magicLantern.actualState)
			{
				return true;
			}
		}
		return false;
	}

	bool controlIfTime()
	{
		if (totalTime > timeToTrigger)
			return true;
		else
			return false;
	}

	void enablingObjects(bool enabling)
	{
		for (int i = 0; i< spriteRenderers.Length; i++)
		{
			if (spriteRenderers[i] != null)
			{
				if (!spriteRenderers[i].enabled)
					spriteRenderers[i].enabled = true;
				float destAlpha = 0.0f;
				float speedMultiplier = 4.0f;
				if (enabling)
				{
					destAlpha = 1.0f;
					speedMultiplier = 1.0f;
				}
				Color tempColor = spriteRenderers[i].color;
				float tempAlpha = tempColor.a;
				tempAlpha = Mathf.Lerp (tempAlpha, destAlpha, changingAlphaSpeed * Time.deltaTime * speedMultiplier);
				spriteRenderers[i].color = new Color(tempColor.r, tempColor.g, tempColor.b, tempAlpha);
			}
			
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
			playerColliding = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
			playerColliding = false;
	}

	void balloonManager()
	{
		if ((balloonString != null && balloonString !="") )
		{
			if (enabling && !addedBalloon && (balloonLastDisappearTime == 0.0 || (Time.time - balloonLastDisappearTime) > balloonReappearTime))
			{
				//Debug.Log ("creato balloon");
				addedBalloon = true;
				balloon = Instantiate(balloonPrefab);
				balloonManagerScript = balloon.GetComponent<ComicBalloonManager>();
				if (!GeneralFinder.cursorHandler.useController)
					balloonManagerScript.setText(balloonString);
				else
					balloonManagerScript.setText(balloonStringController);
				balloonManagerScript.setType(type);
				balloonManagerScript.setCirclesPosition(balloonOnTheRight);
				removedBalloon = false;

				balloonLastAppearTime = Time.time;
				//removeBalloon = false;
			}
			if (removeBalloon)
			{
				if (balloonManagerScript != null)
				{
					//Debug.Log ("togliendo balloon");
					balloonManagerScript.startDisappear();
					addedBalloon = false;
					removedBalloon = true;
				}
				balloonLastDisappearTime = Time.time;
				removeBalloon = false;
			}
		}
	}

	void setBalloonDisappear()
	{
		//if (!removedBalloon && !enabling )
		if (!removedBalloon && (!enabling ||(balloonLastAppearTime != 0.0f && (Time.time - balloonLastAppearTime) > balloonTimeToDisable)))
		{
			removeBalloon = true;
		}
	}

	void enableDisableTut()
	{
		for (int i = 0; i < tutToEnable.Length; i++)
		{
			if (tutToEnable[i] != null)
			{
				tutToEnable[i].enableMe(true);
			}
		}
		for (int i = 0; i < tutToDisable.Length; i++)
		{
			if (tutToDisable[i] != null)
			{
				tutToDisable[i].enableMe(false);
			}
		}
	}

	void enableDisableTutOnDisable()
	{
		for (int i = 0; i < tutToEnableOnDisable.Length; i++)
		{
			if (tutToEnableOnDisable[i] != null)
			{
				tutToEnableOnDisable[i].enableMe(true);
			}
		}
		for (int i = 0; i < tutToDisableOnDisable.Length; i++)
		{
			if (tutToDisableOnDisable[i] != null)
			{
				tutToDisableOnDisable[i].enableMe(false);
			}
		}
	}

	public void enableMe(bool enable = true)
	{
		active = enable;
	}
}

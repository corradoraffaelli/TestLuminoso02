using UnityEngine;
using System.Collections;

public class LanternTutorialHandler : MonoBehaviour {

	//bool utile per settare in che situazioni l'aiuto deve comparire
	public bool active = true;

	GameObject player;
	MagicLantern magicLantern;

	public GameObject[] toEnable;
	SpriteRenderer[] spriteRenderers;
	
	bool playerColliding;
	bool enabling = false;

	public LanternTutorialHandler[] tutToEnable;
	public LanternTutorialHandler[] tutToDisable;

	public MagicLantern.lanternState[] triggerStates;

	public float changingAlphaSpeed = 1.0f;
	
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		magicLantern = UtilFinder._GetComponentOfGameObjectWithTag<MagicLantern>("MagicLanternLogic");

		fillSpriteRenderers();
	}
	
	void Update () {
		if (active)
		{
			//abilito gli oggetti solo se il player è nel trigger e lo stato della lanterna è quello specificato
			enabling = (playerColliding && controlIfState());
			if (enabling)
				enableDisableTut();
			enablingObjects (enabling);
		}
	}

	void fillSpriteRenderers()
	{
		spriteRenderers = new SpriteRenderer[toEnable.Length];
		for (int i = 0; i< spriteRenderers.Length; i++)
		{
			if (toEnable[i] != null)
				spriteRenderers[i] = toEnable[i].GetComponent<SpriteRenderer>();
			if (spriteRenderers[i] != null)
			{
				Color tempColor = spriteRenderers[i].color;
				tempColor = new Color(tempColor.r, tempColor.g, tempColor.b, 0.0f);
				spriteRenderers[i].color = tempColor;
			}
				
		}
	}

	bool controlIfState()
	{
		for (int i = 0; i < triggerStates.Length; i++)
		{
			if (triggerStates[i] != null && triggerStates[i] == magicLantern.actualState)
			{
				return true;
			}
		}
		return false;
	}

	void enablingObjects(bool enabling)
	{
		for (int i = 0; i< spriteRenderers.Length; i++)
		{
			if (spriteRenderers[i] != null)
			{
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
		if (other.gameObject == player)
			playerColliding = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject == player)
			playerColliding = false;
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

	public void enableMe(bool enable = true)
	{
		active = enable;
	}
}

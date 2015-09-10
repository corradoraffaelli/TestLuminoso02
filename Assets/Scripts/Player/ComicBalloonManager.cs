using UnityEngine;
using System.Collections;

public class ComicBalloonManager : MonoBehaviour {

	public enum Type{
		hint,
		tutorial,
		dialogue
	}

	Type type = Type.hint;

	public float positionY = 3.0f;
	public float lerpSpeed = 15.0f;
	public float limitBalloonThreshold = 0.3f;

	public float piecesScale = 0.13f;
	
	public bool circlesOnRight = true;

	public string sortingLayerName = "SceneUI";
	public int sortingLayerInt = 10;

	GameObject objectToFollow;

	[System.Serializable]
	public class BalloonSprites{
		public Sprite bigCircleSprite;
		public Sprite smallCircleSprite;

		public Sprite cornerSprite;
		public Sprite borderSprite;
		public Sprite centerSprite;
	}

	[SerializeField]
	BalloonSprites balloonSprites;

	[System.Serializable]
	public class CirclesVariables{
		public float bigCircleScale = 0.15f;
		public float smallCircleScale = 0.08f;
		
		public float bigCircleXPosition = 1.2f;
		public float bigCircleYPosition = 1.78f;
		public float smallCircleXPosition = 0.7f;
		public float smallCircleYPosition = 1.18f;
	}

	[SerializeField]
	CirclesVariables circlesVariables;

	[System.Serializable]
	public class TutorialSprites{
		public Sprite cornerSprite;
		public Sprite borderSprite;
		public Sprite centerSprite;
	}
	
	[SerializeField]
	TutorialSprites tutorialSprites;

	[System.Serializable]
	public class DialogueSprites{
		public Sprite dialogueSprite;
		public Sprite keyboardContinue;
		public Sprite controllerContinue;
	}

	[SerializeField]
	DialogueSprites dialogueSrites;

	GameObject player;
	BoxCollider2D boxCollider;
	TextMesh textMesh;

	GameObject[] spriteOBJGroup;
	SpriteRenderer[] renderersGroup;

	SpriteRenderer dialogueRenderer;

	GameObject bigCircleOBJ;
	SpriteRenderer bigCircleRenderer;
	GameObject smallCircleOBJ;
	SpriteRenderer smallCircleRenderer;

	public bool standardAppear = true;
	public float appearSpeed = 0.3f;
	float lastAppear = 0.0f;
	float lastDisappear = 0.0f;
	public bool appearing = true;
	public bool disappearing = false;

	string inputText;
	float inputTextSize = 0.0f;

	void Start () {

		if (objectToFollow == null)
			objectToFollow = GeneralFinder.player;

		textMesh = GetComponent<TextMesh>();
		GetComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
		GetComponent<MeshRenderer>().sortingOrder = sortingLayerInt + 2;

		if (inputText != null && inputText != "")
		{
			inputText = inputText.Replace("NEWLINE", "\n");
			textMesh.text = inputText;
		}
			
		if (inputTextSize != 0.0f)
			textMesh.characterSize = inputTextSize;

		boxCollider = gameObject.AddComponent<BoxCollider2D>();
		boxCollider.isTrigger = true;
		player = GeneralFinder.player;
		
		setInitialPosition();
		
		createSprites();
		createCirclesSprites();

		if (standardAppear)
			setGeneralAlpha(0.0f);


	}

	void Update () {
		setPosition();
		if (standardAppear)
		{
			appear();
			disappear();
		}
	}

	//allo start, setta la giusta posizione del fumetto
	void setInitialPosition()
	{
		transform.position = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y + positionY, objectToFollow.transform.position.z -1.0f);
	}

	//chiamato nell'update, permette al fumetto di seguire il player, con un lerp
	void setPosition()
	{
		Vector3 objPosition = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y + positionY, objectToFollow.transform.position.z -1.0f);
		transform.position = Vector3.Lerp(transform.position, objPosition, Time.deltaTime * lerpSpeed);
	}

	//crea tutti gli oggetti che contengono le sprites del fumetto
	void createSprites()
	{
		if (verifyIfNull())
		{
			//cambiamento momentaneo, dovuto al fatto che almeno per la prima parte,
			//il balloon del dialogo è uguale a quello dlel'hint
			bool wasDialogue = (type == Type.dialogue);
			if (wasDialogue)
				type = Type.hint;

			//creo un oggetto e lo piazzo al centro, mi serve per calcolare le coordinate necessarie.
			GameObject centralPiece = new GameObject();
			centralPiece.transform.localScale = new Vector3(piecesScale, piecesScale, 1.0f);
			centralPiece.transform.position = boxCollider.bounds.center;

			SpriteRenderer centralRenderer = centralPiece.AddComponent<SpriteRenderer>();
			centralRenderer.sprite = balloonSprites.centerSprite;

			float yCollBound = boxCollider.bounds.size.y;
			float xCollBound = boxCollider.bounds.size.x;
			float spriteSize = centralRenderer.bounds.size.x;

			Destroy(centralPiece);

			int totXNumber = Mathf.FloorToInt((xCollBound + limitBalloonThreshold) / spriteSize) + 1;
			int totYNumber = Mathf.FloorToInt((yCollBound + limitBalloonThreshold) / spriteSize) + 1;

			//calcolo le posizioni
			float startingX;
			float startingY;
			if (totXNumber % 2 == 0)
			{
				startingX = boxCollider.bounds.center.x - (totXNumber/2 * spriteSize) + spriteSize/2;
			}
			else
			{
				startingX = boxCollider.bounds.center.x - (totXNumber/2 * spriteSize);
			}

			if (totYNumber % 2 == 0)
			{
				startingY = boxCollider.bounds.center.y - (totYNumber/2 * spriteSize) + spriteSize/2;
			}
			else
			{
				startingY = boxCollider.bounds.center.y - (totYNumber/2 * spriteSize);
			}
			Vector3 startingPosition = new Vector3(startingX, startingY, transform.position.z);

			int totalNumber = totXNumber * totYNumber;
			spriteOBJGroup = new GameObject[totalNumber];
			renderersGroup = new SpriteRenderer[totalNumber];

			for (int i = 0; i < spriteOBJGroup.Length; i++)
			{
				spriteOBJGroup[i] = new GameObject();
				spriteOBJGroup[i].transform.parent = transform;
				spriteOBJGroup[i].transform.localScale = new Vector3(piecesScale, piecesScale, 1.0f);

				renderersGroup[i] = spriteOBJGroup[i].AddComponent<SpriteRenderer>();
				if (type == Type.hint)
					renderersGroup[i].sprite = balloonSprites.centerSprite;
				else if (type == Type.tutorial)
					renderersGroup[i].sprite = tutorialSprites.centerSprite;
				renderersGroup[i].sortingLayerName = sortingLayerName;
				renderersGroup[i].sortingOrder = sortingLayerInt;
			}

			//piazzo gli oggetti nelle giuste posizioni e cambio in maniera opportuna le sprites
			for (int i = 0; i < totXNumber; i++)
			{
				for (int j = 0; j < totYNumber; j++)
				{
					Vector3 tempPosition = new Vector3(startingPosition.x + i* spriteSize, startingPosition.y + j* spriteSize, startingPosition.z + 0.5f);
					spriteOBJGroup[totYNumber*i + j].transform.position = tempPosition;

					//cambio la sprite e ruoto opportunamente
					if (i == 0)
					{
						if (j==0)
						{
							if (type == Type.hint)
								renderersGroup[totYNumber*i + j].sprite = balloonSprites.cornerSprite;
							else if (type == Type.tutorial)
								renderersGroup[totYNumber*i + j].sprite = tutorialSprites.cornerSprite;
							spriteOBJGroup[totYNumber*i + j].transform.eulerAngles = new Vector3(0.0f,0.0f,90.0f);
						}
						else if (j == totYNumber - 1)
						{
							if (type == Type.hint)
								renderersGroup[totYNumber*i + j].sprite = balloonSprites.cornerSprite;
							else if (type == Type.tutorial)
								renderersGroup[totYNumber*i + j].sprite = tutorialSprites.cornerSprite;
							//spriteOBJGroup[totYNumber*i + j].transform.eulerAngles = new Vector3(0.0f,0.0f,180.0f);
						}
						else
						{
							if (type == Type.hint)
								renderersGroup[totYNumber*i + j].sprite = balloonSprites.borderSprite;
							else if (type == Type.tutorial)
								renderersGroup[totYNumber*i + j].sprite = tutorialSprites.borderSprite;
							spriteOBJGroup[totYNumber*i + j].transform.eulerAngles = new Vector3(0.0f,0.0f,90.0f);
						}
					}
					else if (i == totXNumber - 1)
					{
						if (j==0)
						{
							if (type == Type.hint)
								renderersGroup[totYNumber*i + j].sprite = balloonSprites.cornerSprite;
							else if (type == Type.tutorial)
								renderersGroup[totYNumber*i + j].sprite = tutorialSprites.cornerSprite;
							spriteOBJGroup[totYNumber*i + j].transform.eulerAngles = new Vector3(0.0f,0.0f,180.0f);
						}
						else if (j == totYNumber - 1)
						{
							if (type == Type.hint)
								renderersGroup[totYNumber*i + j].sprite = balloonSprites.cornerSprite;
							else if (type == Type.tutorial)
								renderersGroup[totYNumber*i + j].sprite = tutorialSprites.cornerSprite;
							spriteOBJGroup[totYNumber*i + j].transform.eulerAngles = new Vector3(0.0f,0.0f,-90.0f);
						}
						else
						{
							if (type == Type.hint)
								renderersGroup[totYNumber*i + j].sprite = balloonSprites.borderSprite;
							else if (type == Type.tutorial)
								renderersGroup[totYNumber*i + j].sprite = tutorialSprites.borderSprite;
							spriteOBJGroup[totYNumber*i + j].transform.eulerAngles = new Vector3(0.0f,0.0f,-90.0f);
						}
					}
					else if (j == totYNumber - 1)
					{
						if (type == Type.hint)
							renderersGroup[totYNumber*i + j].sprite = balloonSprites.borderSprite;
						else if (type == Type.tutorial)
							renderersGroup[totYNumber*i + j].sprite = tutorialSprites.borderSprite;
						//spriteOBJGroup[totYNumber*i + j].transform.eulerAngles = new Vector3(0.0f,0.0f,-90.0f);
					}
					else if (j == 0)
					{
						if (type == Type.hint)
							renderersGroup[totYNumber*i + j].sprite = balloonSprites.borderSprite;
						else if (type == Type.tutorial)
							renderersGroup[totYNumber*i + j].sprite = tutorialSprites.borderSprite;
						spriteOBJGroup[totYNumber*i + j].transform.eulerAngles = new Vector3(0.0f,0.0f,180.0f);
					}
				}
			}

			if (wasDialogue)
				type = Type.dialogue;

			if (type == Type.dialogue)
			{
				//pezzo del fumetto che punta verso chi sta parlando
				GameObject dialoguePiece = new GameObject();
				dialoguePiece.transform.parent = transform;
				dialoguePiece.name = "DialoguePiece";

				dialoguePiece.transform.localScale = new Vector3(piecesScale, piecesScale, 1.0f);

				SpriteRenderer dialogueRenderer = dialoguePiece.AddComponent<SpriteRenderer>();
				dialogueRenderer.sprite = dialogueSrites.dialogueSprite;

				dialogueRenderer.sortingLayerName = sortingLayerName;
				dialogueRenderer.sortingOrder = sortingLayerInt + 1;

				Vector3 tempPosition = new Vector3(objectToFollow.transform.position.x + 0.4f, startingPosition.y, startingPosition.z);
				dialoguePiece.transform.position = tempPosition;

				//indicazione per proseguire
				GameObject continuePiece = new GameObject();
				continuePiece.transform.parent = transform;
				continuePiece.name = "ContinuePiece";
				
				continuePiece.transform.localScale = new Vector3(piecesScale, piecesScale, 1.0f);
				
				SpriteRenderer continueRenderer = continuePiece.AddComponent<SpriteRenderer>();
				if (!GeneralFinder.cursorHandler.useController)
					continueRenderer.sprite = dialogueSrites.keyboardContinue;
				else
					continueRenderer.sprite = dialogueSrites.controllerContinue;
				
				continueRenderer.sortingLayerName = sortingLayerName;
				continueRenderer.sortingOrder = sortingLayerInt + 2;
				
				Vector3 tempPosition02 = new Vector3(objectToFollow.transform.position.x, startingPosition.y - 0.6f, startingPosition.z);
				continuePiece.transform.position = tempPosition02;
			}
		}
	}

	void createCirclesSprites()
	{
		if (type == Type.hint)
		{
			bigCircleOBJ = new GameObject();
			bigCircleOBJ.transform.parent = transform;
			bigCircleOBJ.transform.localScale = new Vector3(circlesVariables.bigCircleScale, circlesVariables.bigCircleScale, 1.0f);
			
			bigCircleRenderer = bigCircleOBJ.AddComponent<SpriteRenderer>();
			bigCircleRenderer.sprite = balloonSprites.bigCircleSprite;
			
			smallCircleOBJ = new GameObject();
			smallCircleOBJ.transform.parent = transform;
			smallCircleOBJ.transform.localScale = new Vector3(circlesVariables.smallCircleScale, circlesVariables.smallCircleScale, 1.0f);
			
			smallCircleRenderer = smallCircleOBJ.AddComponent<SpriteRenderer>();
			smallCircleRenderer.sprite = balloonSprites.smallCircleSprite;
			
			bigCircleRenderer.sortingLayerName = sortingLayerName;
			bigCircleRenderer.sortingOrder = sortingLayerInt;
			smallCircleRenderer.sortingLayerName = sortingLayerName;
			smallCircleRenderer.sortingOrder = sortingLayerInt;
			
			if (circlesOnRight)
			{
				bigCircleOBJ.transform.position = new Vector3(objectToFollow.transform.position.x + circlesVariables.bigCircleXPosition, 
				                                              objectToFollow.transform.position.y + circlesVariables.bigCircleYPosition, objectToFollow.transform.position.z);
				smallCircleOBJ.transform.position = new Vector3(objectToFollow.transform.position.x + circlesVariables.smallCircleXPosition, 
				                                                objectToFollow.transform.position.y + circlesVariables.smallCircleYPosition, objectToFollow.transform.position.z);
			}
			else
			{
				bigCircleOBJ.transform.position = new Vector3(objectToFollow.transform.position.x - circlesVariables.bigCircleXPosition, 
				                                              objectToFollow.transform.position.y + circlesVariables.bigCircleYPosition, objectToFollow.transform.position.z);
				smallCircleOBJ.transform.position = new Vector3(objectToFollow.transform.position.x - circlesVariables.smallCircleXPosition, 
				                                                objectToFollow.transform.position.y + circlesVariables.smallCircleYPosition, objectToFollow.transform.position.z);
			}
		}

	}

	//ritorna vero se tutte le sprites del fumetto non sono nulle
	bool verifyIfNull()
	{
		if (balloonSprites.centerSprite != null
		    && balloonSprites.borderSprite != null && balloonSprites.cornerSprite != null
		    && balloonSprites.smallCircleSprite != null && balloonSprites.bigCircleSprite != null
		    && tutorialSprites.borderSprite != null && tutorialSprites.cornerSprite != null
		    &&tutorialSprites.centerSprite != null)
			return true;
		else
			return false;
	}
	
	//setta l'alpha del balloon e del testo
	void setBalloonAlpha(float newAlpha)
	{
		if (newAlpha >= 0.0f && newAlpha <= 1.0f)
		{
			textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, newAlpha);
			if (renderersGroup != null && renderersGroup.Length != 0)
			{
				for (int i = 0; i<renderersGroup.Length; i++)
				{
					if (renderersGroup[i] != null)
					{
						renderersGroup[i].color = new Color(renderersGroup[i].color.r, renderersGroup[i].color.g, renderersGroup[i].color.b, newAlpha);
					}
				}
			}

			if (type == Type.dialogue && dialogueRenderer != null)
			{
				dialogueRenderer.color = new Color(dialogueRenderer.color.r, dialogueRenderer.color.g, dialogueRenderer.color.b, newAlpha);
			}
		}
	}
	
	void setBigCircleAlpha(float newAlpha)
	{
		if (newAlpha >= 0.0f && newAlpha <= 1.0f)
		{
			if (bigCircleOBJ != null && bigCircleRenderer != null)
				bigCircleRenderer.color = new Color(bigCircleRenderer.color.r, bigCircleRenderer.color.g, bigCircleRenderer.color.b, newAlpha);
		}
	}
	
	void setSmallCircleAlpha(float newAlpha)
	{
		if (newAlpha >= 0.0f && newAlpha <= 1.0f)
		{
			if (smallCircleOBJ != null && smallCircleRenderer != null)
				smallCircleRenderer.color = new Color(smallCircleRenderer.color.r, smallCircleRenderer.color.g, smallCircleRenderer.color.b, newAlpha);
		}
	}

	void appear()
	{
		if (appearing)
		{
			if (type == Type.hint)
			{
				if ((Time.time - lastAppear) > appearSpeed)
				{
					//Debug.Log ("dentro");
					if (smallCircleRenderer.color.a == 0.0f)
						setSmallCircleAlpha(1.0f);
					else if (bigCircleRenderer.color.a == 0.0)
						setBigCircleAlpha(1.0f);
					else
					{
						setBalloonAlpha(1.0f);
						appearing = false;
					}
					
					lastAppear = Time.time;
				}
			}
			else if (type == Type.tutorial)
			{
				if ((Time.time - lastAppear) > appearSpeed)
				{
					setBalloonAlpha(1.0f);
					appearing = false;

					lastAppear = Time.time;
				}
			}
			else if (type == Type.dialogue)
			{
				if ((Time.time - lastAppear) > appearSpeed)
				{
					setBalloonAlpha(1.0f);
					appearing = false;
					
					lastAppear = Time.time;
				}
			}
		}
	}

	void disappear()
	{
		if (disappearing)
		{
			if (type == Type.hint)
			{
				appearing = false;
				if ((Time.time - lastDisappear) > appearSpeed)
				{
					//Debug.Log ("dentro");
					if (textMesh.color.a == 1.0)
						setBalloonAlpha(0.0f);
					else if (bigCircleRenderer.color.a == 1.0)
					{
						Debug.Log ("cancellando coso grosso");
						setBigCircleAlpha(0.0f);
					}
					else
					{
						Debug.Log ("cancellando coso piccolo");
						setSmallCircleAlpha(0.0f);
						disappearing = false;
						Destroy(gameObject);
					}
					
					lastDisappear = Time.time;
				}
			}
			else if (type == Type.tutorial)
			{
				appearing = false;
				if ((Time.time - lastDisappear) > appearSpeed)
				{
					setBalloonAlpha(0.0f);

					disappearing = false;
					Destroy(gameObject);

					lastDisappear = Time.time;
				}
			}
			else if (type == Type.dialogue)
			{
				appearing = false;
				if ((Time.time - lastDisappear) > appearSpeed)
				{
					setBalloonAlpha(0.0f);
					
					disappearing = false;
					Destroy(gameObject);
					
					lastDisappear = Time.time;
				}
			}
		}
	}

	public void startDisappear()
	{
		disappearing = true;
		lastDisappear = Time.time;
	}

	//ELENCO DELLE FUNZIONI PUBBLICHE

	public void setStandardAppear(bool standard)
	{
		standardAppear = standard;
	}

	//setta il testo
	public void setText(string text)
	{
		inputText = text;
		//textMesh.text = text;
	}

	//setta la grandezza del testo
	public void setFontSize(float size)
	{
		inputTextSize = size;
		//textMesh.characterSize = size;
	}

	public void setCirclesPosition (bool right)
	{
		circlesOnRight = right;
	}

	public void setGeneralAlpha(float newAlpha)
	{
		setSmallCircleAlpha(newAlpha);
		setBigCircleAlpha(newAlpha);
		setBalloonAlpha(newAlpha);
	}

	//setta la grandezza e la posizione dei cerchi
	public void setCirclesVariables(float bigSize, float smallSize, Vector2 bigPosition, Vector2 smallPosition)
	{
		circlesVariables.bigCircleScale = bigSize;
		circlesVariables.smallCircleScale = smallSize;
		circlesVariables.bigCircleXPosition = bigPosition.x;
		circlesVariables.bigCircleYPosition = bigPosition.y;
		circlesVariables.smallCircleXPosition = smallPosition.x;
		circlesVariables.smallCircleYPosition = smallPosition.y;
	}

	public void setType (ComicBalloonManager.Type inputType)
	{
		type = inputType;
	}

	public void setObjectToFollow(GameObject inputObject)
	{
		objectToFollow = inputObject;
	}
}

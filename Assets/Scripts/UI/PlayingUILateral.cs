using UnityEngine;
using System.Collections;

public class PlayingUILateral : MonoBehaviour {

	[System.Serializable]
	public class SpritesGroupElement{
		public Sprite sprite;
		public GameObject imageObject;
		public GameObject exclamationObject;
	}

	[System.Serializable]
	public class SpritesGroup{
		public UIPosition position;
		public Sprite backgroundSprite;
		public UISize size = UISize.Normal;
		//[HideInInspector]
		//public Sprite[] sprites;
		//[HideInInspector]
		//public GameObject[] imagesObject;
		[HideInInspector]
		public GameObject fatherGO;
		[HideInInspector]
		public RectTransform fatherRectTransform;

		[HideInInspector]
		public bool showing = false;
		[HideInInspector]
		public bool hiding = true;

		//[HideInInspector]
		public SpritesGroupElement[] spritesGroupElement;

		//[HideInInspector]
		//public GameObject[] exclamationGO;
	}
	
	[System.Serializable]
	public class PositionVariables{
		[Range(10,200)]
		public int YPadding = 60;
		[Range(10,200)]
		public int XShowPosition = 60;
		[Range(10,200)]
		public int XHidePosition = 60;
		[Range(0.1f,1000.0f)]
		public float speedChangingPosition = 300.0f;
	}
	
	[System.Serializable]
	public class SizeVariables{
		[Range(20, 130)]
		public int smallSprites = 40;
		[Range(20, 130)]
		public int normalSprites = 60;
		[Range(20, 130)]
		public int bigSprites = 80;
		[Range(20, 800)]
		public int backgroundSize = 400;
		[Range(10, 130)]
		public int exclamationSize = 20;
	}
	
	public enum UIPosition
	{
		Right,
		Left
	};
	
	public enum UISize
	{
		Small,
		Normal,
		Big
	};

	public Sprite exclamationMark;
	[SerializeField]
	SpritesGroup[] spritesGroup = new SpritesGroup[2];
	[SerializeField]
	PositionVariables positionVariables;
	[SerializeField]
	SizeVariables sizeVariables;

	//creo i padri che contengono la sprite contenitore
	void initializeFather(UIPosition pos)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				spritesGroup[i].fatherGO = new GameObject();
				spritesGroup[i].fatherGO.transform.parent = transform;
				spritesGroup[i].fatherGO.name = pos.ToString();
				
				//creo i component dell'oggetto, solo se è stata indicata una sprite contenitore, da mettere sempre, al massimo trasparente
				if (spritesGroup[i].backgroundSprite != null)
				{
					//creo i component image e rect
					UnityEngine.UI.Image tempImage = spritesGroup[i].fatherGO.AddComponent<UnityEngine.UI.Image>();
					RectTransform rectTransform = spritesGroup[i].fatherGO.GetComponent<RectTransform>();
					spritesGroup[i].fatherRectTransform = spritesGroup[i].fatherGO.GetComponent<RectTransform>();

					//setto le variabili del rect
					rectTransform.pivot = new Vector2(0.5f,0.5f);			
					rectTransform.sizeDelta = new Vector2(sizeVariables.backgroundSize, sizeVariables.backgroundSize);

					if (pos == UIPosition.Left)
					{
						rectTransform.anchorMin = new Vector2(0.0f,0.5f);
						rectTransform.anchorMax = new Vector2(0.0f,0.5f);
						rectTransform.anchoredPosition = new Vector2(-positionVariables.XHidePosition, 0.0f);
					}
					else if (pos == UIPosition.Right)
					{
						rectTransform.anchorMin = new Vector2(1.0f,0.5f);
						rectTransform.anchorMax = new Vector2(1.0f,0.5f);
						rectTransform.anchoredPosition = new Vector2(positionVariables.XHidePosition, 0.0f);
					}

					//setto le sprites
					tempImage.sprite = spritesGroup[i].backgroundSprite;
					tempImage.preserveAspect = true;

					rectTransform.localScale = new Vector3(1.0f,1.0f,1.0f);
				}

				break;
			}
		}
	}

	public void showIcons(UIPosition pos, bool show = true)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				spritesGroup[i].showing = show;
				spritesGroup[i].hiding = !show;
				break;
			}
		}
	}

	void moveIcons()
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].fatherRectTransform != null)
			{
				float actualXPosition = spritesGroup[i].fatherRectTransform.anchoredPosition.x;
				float objPosition = 0.0f;
				if (spritesGroup[i].showing)
				{
					if (spritesGroup[i].position == UIPosition.Right)
					{
						objPosition = -positionVariables.XShowPosition;
					}
					else if(spritesGroup[i].position == UIPosition.Left)
					{
						objPosition = positionVariables.XShowPosition;
					}
				}
				else if (spritesGroup[i].hiding)
				{
					if (spritesGroup[i].position == UIPosition.Right)
					{
						objPosition = positionVariables.XHidePosition;
					}
					else if(spritesGroup[i].position == UIPosition.Left)
					{
						objPosition = -positionVariables.XHidePosition;
					}
				}
				actualXPosition = Mathf.MoveTowards(actualXPosition, objPosition, positionVariables.speedChangingPosition * Time.deltaTime);
				spritesGroup[i].fatherRectTransform.anchoredPosition = new Vector2(actualXPosition, spritesGroup[i].fatherRectTransform.anchoredPosition.y);
			}

		}
	}

	public void setSprites(Sprite[] sprites, UIPosition pos)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				//distruggo i vecchi gameObject
				if (spritesGroup[i].spritesGroupElement!= null)
				{
					for (int j = 0; j<spritesGroup[i].spritesGroupElement.Length; j++)
					{
						if (spritesGroup[i].spritesGroupElement[j]!=null && spritesGroup[i].spritesGroupElement[j].imageObject != null)
							Destroy(spritesGroup[i].spritesGroupElement[j].imageObject);
					}
				}

				spritesGroup[i].spritesGroupElement = new SpritesGroupElement[sprites.Length];
				for (int j = 0; j < spritesGroup[i].spritesGroupElement.Length; j++)
				{
					spritesGroup[i].spritesGroupElement[j] = new SpritesGroupElement();
					//if (spritesGroup[i].spritesGroupElement[j] != null && sprites[j]!=null)
					spritesGroup[i].spritesGroupElement[j].sprite = sprites[j];
				}
				//spritesGroup[i].sprites = sprites;
				break;
			}
		}
	}

	//serve per settare la grandezza delle sprites
	public void setSpritesSize(UIPosition pos, UISize size)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				spritesGroup[i].size = size;
				break;
			}
		}
	}

	//metodo da chiamare per aggiornare le sprites a schermo
	//potrebbe essere chiamata automaticamente ad ogni aggiornamento, ma preferisco lasciare la scelta al programmatore
	public void updateSpritesOnScreen(UIPosition pos)
	{
		createObject (pos);
	}

	//metodo che crea gli oggetti UI di tipo image e setta le relative sprites
	void createObject(UIPosition pos)
	{

		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				
				if (spritesGroup[i].spritesGroupElement!= null && spritesGroup[i].spritesGroupElement.Length != 0)
				{
					//se esiste già gli oggetti, li tolgo
					//non dovrebbe servire, fatto in setSprites
					for (int j = 0; j<spritesGroup[i].spritesGroupElement.Length; j++)
					{
						if (spritesGroup[i].spritesGroupElement[j].imageObject != null)
							Destroy(spritesGroup[i].spritesGroupElement[j].imageObject);
					}

					float firstYPosition = returnSpriteFirstPosition(spritesGroup[i].spritesGroupElement.Length);
					//Debug.Log (firstYPosition);

					//per ogni oggetto presente nella posizione di schermo considerata
					for (int j = 0; j<spritesGroup[i].spritesGroupElement.Length; j++)
					{
						//creo l'oggetto
						spritesGroup[i].spritesGroupElement[j].imageObject = new GameObject();
						if (spritesGroup[i].fatherGO != null)
							spritesGroup[i].spritesGroupElement[j].imageObject.transform.parent = spritesGroup[i].fatherGO.transform;

						spritesGroup[i].spritesGroupElement[j].imageObject.name = pos.ToString() + "_" +j;
						
						//creo i component dell'oggetto
						UnityEngine.UI.Image tempImage = spritesGroup[i].spritesGroupElement[j].imageObject.AddComponent<UnityEngine.UI.Image>();
						RectTransform rectTransform = spritesGroup[i].spritesGroupElement[j].imageObject.GetComponent<RectTransform>();

						rectTransform.anchorMin = new Vector2(0.5f,0.5f);
						rectTransform.anchorMax = new Vector2(0.5f,0.5f);
						rectTransform.pivot = new Vector2(0.5f,0.5f);
			
						//setto la dimensione
						int tempSize = getSpritesGroupSize(pos);
						rectTransform.sizeDelta = new Vector2(tempSize, tempSize);
						
						//setto le sprites
						tempImage.sprite = spritesGroup[i].spritesGroupElement[j].sprite;
						
						tempImage.preserveAspect = true;
						
						rectTransform.localScale = new Vector3(1.0f,1.0f,1.0f);

						float YPosition = returnSpritePosition(firstYPosition, spritesGroup[i].spritesGroupElement.Length - j - 1);
						rectTransform.anchoredPosition = new Vector2(0.0f,YPosition);
					}
					
				}
			}
		}

	}

	public int getSpritesGroupSize(UIPosition pos)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				if (spritesGroup[i].size == UISize.Big)
					return sizeVariables.bigSprites;
				else if (spritesGroup[i].size == UISize.Normal)
					return sizeVariables.normalSprites;
				else if (spritesGroup[i].size == UISize.Small)
					return sizeVariables.smallSprites;
			}
		}
		return 0;
	}

	float returnSpriteFirstPosition(int spritesTotalNumber)
	{
		bool pari = (spritesTotalNumber % 2 == 0);
		float firstPosition = 0.0f;
		int halfNumber = spritesTotalNumber/2;
		if (pari)
		{
			firstPosition = firstPosition + positionVariables.YPadding/2;
		}
		firstPosition = firstPosition - (halfNumber*positionVariables.YPadding);
		return firstPosition;
	}

	float returnSpritePosition(float firstPosition, int index)
	{
		return firstPosition + index*positionVariables.YPadding;
	}

	//funzione che può essere richiamata per assicurarsi che il blocco sia completamente nascosto
	public bool isCompletelyHidden(UIPosition pos)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				if (spritesGroup[i].fatherGO != null && spritesGroup[i].fatherRectTransform != null)
				{
					float comparePosition = 0.0f;

					if (pos == UIPosition.Right)
						comparePosition = positionVariables.XHidePosition;
					else if (pos == UIPosition.Left)
						comparePosition = -positionVariables.XHidePosition;

					if (spritesGroup[i].fatherRectTransform.anchoredPosition.x == comparePosition)
						return true;
					else
						return false;
				}
			}
		}
		return false;
	}

	//funzione che può essere richiamata per assicurarsi che il blocco sia completamente visibile
	public bool isCompletelyVisible(UIPosition pos)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				if (spritesGroup[i].fatherGO != null && spritesGroup[i].fatherRectTransform != null)
				{
					float comparePosition = 0.0f;
					
					if (pos == UIPosition.Right)
						comparePosition = -positionVariables.XShowPosition;
					else if (pos == UIPosition.Left)
						comparePosition = positionVariables.XShowPosition;
					
					if (spritesGroup[i].fatherRectTransform.anchoredPosition.x == comparePosition)
						return true;
					else
						return false;
				}
				break;
			}
		}
		return false;
	}

	public void setExclamation(UIPosition pos, int index, bool set = true)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				if (spritesGroup[i].spritesGroupElement.Length > index && spritesGroup[i].spritesGroupElement[index] != null)
				{
					if (spritesGroup[i].spritesGroupElement[index].exclamationObject != null)
						Destroy (spritesGroup[i].spritesGroupElement[index].exclamationObject);
					if (set)
					{
						spritesGroup[i].spritesGroupElement[index].exclamationObject = new GameObject();
						spritesGroup[i].spritesGroupElement[index].exclamationObject.transform.parent = spritesGroup[i].spritesGroupElement[index].imageObject.transform;
						string tempName = pos.ToString() + "_" + spritesGroup[i].spritesGroupElement[index].ToString() + "_exclamation";
						spritesGroup[i].spritesGroupElement[index].exclamationObject.name = tempName;

						UnityEngine.UI.Image tempImage = spritesGroup[i].spritesGroupElement[index].exclamationObject.AddComponent<UnityEngine.UI.Image>();
						RectTransform rectTransform = spritesGroup[i].spritesGroupElement[index].exclamationObject.GetComponent<RectTransform>();

						rectTransform.anchorMin = new Vector2(1.0f,0.0f);
						rectTransform.anchorMax = new Vector2(1.0f,0.0f);
						rectTransform.pivot = new Vector2(1.0f,0.0f);
						
						//setto la dimensione
						int tempSize = sizeVariables.exclamationSize;
						rectTransform.sizeDelta = new Vector2(tempSize, tempSize);
						
						//setto le sprites
						tempImage.sprite = exclamationMark;
						
						tempImage.preserveAspect = true;
						
						rectTransform.localScale = new Vector3(1.0f,1.0f,1.0f);
						
						//float YPosition = returnSpritePosition(firstYPosition, spritesGroup[i].spritesGroupElement.Length - j - 1);
						rectTransform.anchoredPosition = new Vector2(0.0f,0.0f);
					}
				}
				break;
			}
		}

	}

	public void removeAllExclamations()
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i] != null && spritesGroup[i].spritesGroupElement != null)
			{
				for (int j = 0; j < spritesGroup[i].spritesGroupElement.Length; j++)
				{
					if (spritesGroup[i].spritesGroupElement[j].exclamationObject != null)
						Destroy(spritesGroup[i].spritesGroupElement[j].exclamationObject);
				}
			}
		}
	}

	public GameObject getImageObject(UIPosition pos, int index)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				if (spritesGroup[i].spritesGroupElement != null && spritesGroup[i].spritesGroupElement.Length > index && spritesGroup[i].spritesGroupElement[index] != null && spritesGroup[i].spritesGroupElement[index].imageObject != null)
					return spritesGroup[i].spritesGroupElement[index].imageObject;
				break;
			}
		}
		return null;
	}

	void Start () {
		initializeFather(UIPosition.Right);
		initializeFather(UIPosition.Left);
	}

	void Update () {
		moveIcons();
	}

	public int getXWhenVisible()
	{
		return positionVariables.XShowPosition;
	}
}

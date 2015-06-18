using UnityEngine;
using System.Collections;

public class PlayingUI : MonoBehaviour {

	//UnityEngine.UI.Image

	[System.Serializable]
	public class SpritesGroup{
		public UIPosition position;
		public UISize size = UISize.Normal;
		public UIDistance distance = UIDistance.Fixed;
		public bool horizontal = true;
		public bool buttonHorizontal = true;
		[HideInInspector]
		public Sprite[] sprites;
		[HideInInspector]
		public GameObject[] imagesObject;
		[HideInInspector]
		public Sprite buttonSprite;
		[HideInInspector]
		public GameObject imageButtonObject;
	}

	[System.Serializable]
	public class DistanceVariables{
		[Range(10,50)]
		public int XPadding = 30;
		[Range(10,50)]
		public int YPadding = 30;
		[Range(10,50)]
		public int XDistance = 20;
		[Range(10,50)]
		public int YDistance = 20;
	}

	[System.Serializable]
	public class SizeVariables{
		[Range(20, 130)]
		public int smallSprites = 40;
		[Range(20, 130)]
		public int normalSprites = 60;
		[Range(20, 130)]
		public int bigSprites = 80;
		[Range(20, 130)]
		public int buttonSprite = 30;
	}

	public enum UIPosition
	{
		UpperRight,
		UpperLeft,
		BottomRight,
		BottomLeft
	};

	public enum UISize
	{
		Small,
		Normal,
		Big
	};

	public enum UIDistance
	{
		Fixed,
		Proportional
	};

	[SerializeField]
	SpritesGroup[] spritesGroup = new SpritesGroup[4];
	[SerializeField]
	DistanceVariables distanceVariables;
	[SerializeField]
	SizeVariables sizeVariables;

	public void cleanPositionGameObjects(UIPosition pos)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				if (spritesGroup[i].sprites != null)
				{
					for (int j=0; j<spritesGroup[i].sprites.Length; j++)
					{
						spritesGroup[i].sprites[j] = null;
					}

					spritesGroup[i].sprites = null;
				}

				if (spritesGroup[i].imagesObject != null)
				{
					for (int j=0; j<spritesGroup[i].imagesObject.Length; j++)
					{
						if (spritesGroup[i].imagesObject != null)
							Destroy(spritesGroup[i].imagesObject[j]);
					}

					spritesGroup[i].imagesObject = null;
				}

				break;
			}
		}
	}

	public void cleanPositionButtonObject(UIPosition pos)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				spritesGroup[i].buttonSprite = null;

				Destroy(spritesGroup[i].imageButtonObject);
				
				break;
			}
		}
	}

	/*
	 * assegna le sprites da mostrare nella posizione specificata, 
	 * come parametro viene passato un array di sprites, 
	 * chiaramente, se l’array contiene una sola sprite, ne verrà mostrata solo una
	*/
	public void setSprites(Sprite[] sprites, UIPosition pos)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				spritesGroup[i].sprites = sprites;
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

	//per ora inutilizzata
	public void setSpriteSize(UIPosition pos, int index, UISize size)
	{
		/*
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				if (spritesGroup[i].sprites[index] != null)
				{


				}

			}
		}
		*/
	}

	//serve per settare la distanza delle sprites
	public void setDistance(UIPosition pos, UIDistance distance)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				spritesGroup[i].distance = distance;
				break;
			}
		}
	}

	//serve per settare il verso delle sprites
	public void setVertical(UIPosition pos, bool vertical = true)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				spritesGroup[i].horizontal = !vertical;
				break;
			}
		}
	}

	//serve per settare la posizione del bottone
	public void setVerticalButton(UIPosition pos, bool vertical = true)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				spritesGroup[i].buttonHorizontal = !vertical;
				break;
			}
		}
	}

	//metodo che può essere chiamato dall'esterno per avere diretto accesso agli oggetti col component Image
	//di default non dovrebbe essere usato
	public GameObject getImageObject(UIPosition pos, int index)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				if (spritesGroup[i].imagesObject[index] != null)
					return spritesGroup[i].imagesObject[index];
			}
		}
		return null;
	}

	//metodo da chiamare per aggiornare le sprites a schermo
	//potrebbe essere chiamata automaticamente ad ogni aggiornamento, ma preferisco lasciare la scelta al programmatore
	public void updateSpritesOnScreen(UIPosition pos)
	{
		createObject (pos);
		changePositions (pos);
		buttonHandle (pos);
	}


	//metodo che crea gli oggetti UI di tipo image e setta le relative sprites
	void createObject(UIPosition pos)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{

				if (spritesGroup[i].sprites!= null && spritesGroup[i].sprites.Length != 0)
				{
					//Debug.Log ("sono dentro" + spritesGroup[i].position.ToString());

					//se esiste già un array di oggetti, lo pulisco
					if (spritesGroup[i].imagesObject != null && spritesGroup[i].imagesObject.Length != 0)
					{
						for (int j = 0; j<spritesGroup[i].imagesObject.Length; j++)
						{
							if (spritesGroup[i].imagesObject[j] != null)
								Destroy(spritesGroup[i].imagesObject[j]);
						}
					}
					//System.Array.Clear(spritesGroup[i].imagesObject, 0, spritesGroup[i].imagesObject.Length);

					//creo un array di oggetti con dimensione pari al numero di sprites
					spritesGroup[i].imagesObject = new GameObject[spritesGroup[i].sprites.Length];
					
					//per ogni oggetto presente nella posizione di schermo considerata
					for (int j = 0; j<spritesGroup[i].imagesObject.Length; j++)
					{
						//creo l'oggetto
						spritesGroup[i].imagesObject[j] = new GameObject();
						spritesGroup[i].imagesObject[j].transform.parent = transform;

						//creo i component dell'oggetto
						UnityEngine.UI.Image tempImage = spritesGroup[i].imagesObject[j].AddComponent<UnityEngine.UI.Image>();
						RectTransform rectTransform = spritesGroup[i].imagesObject[j].GetComponent<RectTransform>();

						spritesGroup[i].imagesObject[j].name = pos.ToString() + "_" + j;

						//a seconda della posizione setto parametri particolari
						if (spritesGroup[i].position == UIPosition.UpperRight)
						{
							rectTransform.anchorMin = new Vector2(1.0f,1.0f);
							rectTransform.anchorMax = new Vector2(1.0f,1.0f);
							rectTransform.pivot = new Vector2(1.0f,1.0f);
						}else if(spritesGroup[i].position == UIPosition.UpperLeft)
						{
							rectTransform.anchorMin = new Vector2(0.0f,1.0f);
							rectTransform.anchorMax = new Vector2(0.0f,1.0f);
							rectTransform.pivot = new Vector2(0.0f,1.0f);
						}else if(spritesGroup[i].position == UIPosition.BottomRight)
						{
							rectTransform.anchorMin = new Vector2(1.0f,0.0f);
							rectTransform.anchorMax = new Vector2(1.0f,0.0f);
							rectTransform.pivot = new Vector2(1.0f,0.0f);
						}else if(spritesGroup[i].position == UIPosition.BottomLeft)
						{
							rectTransform.anchorMin = new Vector2(0.0f,0.0f);
							rectTransform.anchorMax = new Vector2(0.0f,0.0f);
							rectTransform.pivot = new Vector2(0.0f,0.0f);
						}

						//setto la dimensione
						int tempSize = getSpritesGroupSize(pos);
						rectTransform.sizeDelta = new Vector2(tempSize, tempSize);

						//setto le sprites
						tempImage.sprite = spritesGroup[i].sprites[j];

						tempImage.preserveAspect = true;

						rectTransform.localScale = new Vector3(1.0f,1.0f,1.0f);
					}
					
				}


			}
		}
	}

	//metodo che setta le posizioni dei component RectTransform degli oggetti UI con component Image
	void changePositions(UIPosition pos)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				if (spritesGroup[i].imagesObject != null)
				{
					//controllo sul posizionamento Fixed, per ora non è gestito quello proporzionale
					if (spritesGroup[i].distance == UIDistance.Fixed)
					{
						if (spritesGroup[i].horizontal)
						{
							int tempSize = getSpritesGroupSize(pos);
							
							switch(spritesGroup[i].position)
							{
							case UIPosition.UpperRight:
								for (int j=0; j<spritesGroup[i].imagesObject.Length;j++)
								{
									//ordine inverso delle sprite passate in ingresso
									int k = spritesGroup[i].imagesObject.Length-j-1;
									
									RectTransform rectTransform = spritesGroup[i].imagesObject[k].GetComponent<RectTransform>();
									
									int XPosition = distanceVariables.XPadding + (j*tempSize) + (j*distanceVariables.XDistance);
									rectTransform.anchoredPosition = new Vector2(-XPosition, -distanceVariables.YPadding);
								}
								break;
							case UIPosition.UpperLeft:
								for (int j=0; j<spritesGroup[i].imagesObject.Length;j++)
								{
									RectTransform rectTransform = spritesGroup[i].imagesObject[j].GetComponent<RectTransform>();
									
									int XPosition = distanceVariables.XPadding + (j*tempSize) + (j*distanceVariables.XDistance);
									rectTransform.anchoredPosition = new Vector2(XPosition,- distanceVariables.YPadding);
								}
								break;
							case UIPosition.BottomRight:
								for (int j=0; j<spritesGroup[i].imagesObject.Length;j++)
								{
									//ordine inverso delle sprite passate in ingresso
									int k = spritesGroup[i].imagesObject.Length-j-1;
									
									RectTransform rectTransform = spritesGroup[i].imagesObject[k].GetComponent<RectTransform>();
									
									int XPosition = distanceVariables.XPadding + (j*tempSize) + (j*distanceVariables.XDistance);
									rectTransform.anchoredPosition = new Vector2(-XPosition, distanceVariables.YPadding);
								}
								break;
							case UIPosition.BottomLeft:
								for (int j=0; j<spritesGroup[i].imagesObject.Length;j++)
								{
									RectTransform rectTransform = spritesGroup[i].imagesObject[j].GetComponent<RectTransform>();
									
									int XPosition = distanceVariables.XPadding + (j*tempSize) + (j*distanceVariables.XDistance);
									rectTransform.anchoredPosition = new Vector2(XPosition,distanceVariables.YPadding);
								}
								break;
							}
							
							//VERTICALE
						}else{
							
							int tempSize = getSpritesGroupSize(pos);
							
							switch(spritesGroup[i].position)
							{
							case UIPosition.UpperRight:
								for (int j=0; j<spritesGroup[i].imagesObject.Length;j++)
								{
									RectTransform rectTransform = spritesGroup[i].imagesObject[j].GetComponent<RectTransform>();
									
									int YPosition = distanceVariables.YPadding + (j*tempSize) + (j*distanceVariables.YPadding);
									rectTransform.anchoredPosition = new Vector2(-distanceVariables.XPadding,- YPosition);
								}
								break;
							case UIPosition.UpperLeft:
								for (int j=0; j<spritesGroup[i].imagesObject.Length;j++)
								{
									RectTransform rectTransform = spritesGroup[i].imagesObject[j].GetComponent<RectTransform>();
									
									int YPosition = distanceVariables.YPadding + (j*tempSize) + (j*distanceVariables.YPadding);
									rectTransform.anchoredPosition = new Vector2( distanceVariables.XPadding,- YPosition);
								}
								break;
							case UIPosition.BottomRight:
								for (int j=0; j<spritesGroup[i].imagesObject.Length;j++)
								{
									RectTransform rectTransform = spritesGroup[i].imagesObject[j].GetComponent<RectTransform>();
									
									int YPosition = distanceVariables.YPadding + (j*tempSize) + (j*distanceVariables.YPadding);
									rectTransform.anchoredPosition = new Vector2(-distanceVariables.XPadding, YPosition);
								}
								break;
							case UIPosition.BottomLeft:
								for (int j=0; j<spritesGroup[i].imagesObject.Length;j++)
								{
									RectTransform rectTransform = spritesGroup[i].imagesObject[j].GetComponent<RectTransform>();
									
									int YPosition = distanceVariables.YPadding + (j*tempSize) + (j*distanceVariables.YPadding);
									rectTransform.anchoredPosition = new Vector2(distanceVariables.XPadding, YPosition);
								}
								break;
							}
							
						}
						
					}
				}

			}
		}
	}

	void Start()
	{
		//prima inizializzazione, nel caso i vari spritesGroup siano già settati da inspector
		for (int i = 0; i<spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null)
				updateSpritesOnScreen(spritesGroup[i].position);
		}

		//Invoke ("testMethods", 2);
		//testMethods ();
	}

	//metodi chiamati, per test delle funzionalità, nello start
	/*
	void testMethods()
	{
		setSprites (spritesTest, UIPosition.UpperRight);
		setButtonSprite (UIPosition.UpperRight, buttonSprite);
		updateSpritesOnScreen(UIPosition.UpperRight);

		setSprites (spritesTest, UIPosition.BottomLeft);
		setButtonSprite (UIPosition.BottomLeft, buttonSprite);
		updateSpritesOnScreen(UIPosition.BottomLeft);

		setSprites (spritesTest, UIPosition.UpperLeft);
		setButtonSprite (UIPosition.UpperLeft, buttonSprite);
		updateSpritesOnScreen(UIPosition.UpperLeft);

		setSprites (spritesTest, UIPosition.BottomRight);
		setButtonSprite (UIPosition.BottomRight, buttonSprite);
		updateSpritesOnScreen(UIPosition.BottomRight);
	}
	*/

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

	public float getYPadding()
	{
		return distanceVariables.YPadding;
	}

	public float getXPadding()
	{
		return distanceVariables.XPadding;
	}

	//metodo per settare la sprite del bottone
	public void setButtonSprite(UIPosition pos, Sprite sprite)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				//Debug.Log ("BELLA ZIO! Sono in "+spritesGroup[i].position.ToString());
				spritesGroup[i].buttonSprite = sprite;
			}
		}
	}

	//metodo che si occupa di piazzare a schermo la sprite del bottone da premere.
	//se il bottone non deve essere messo, si occupa di cancellare eventuali riferimenti.
	//si dà per presupposto che il bottone si trovi nella parte verso il centro dello schermo
	void buttonHandle(UIPosition pos)
	{
		for (int i = 0; i < spritesGroup.Length; i++) {
			if (spritesGroup[i].position != null && spritesGroup[i].position == pos)
			{
				if (spritesGroup[i].imageButtonObject != null)
					Destroy(spritesGroup[i].imageButtonObject);

				//faccio tutto il casino, solo se è settata un'immagine per un bottone
				if (spritesGroup[i].buttonSprite != null)
				{
					//setto le variabili di posizione del bottone, in base al bool buttonHorizontal
					int XPosition;
					int YPosition;
					int tempSize = getSpritesGroupSize(pos);

					if (spritesGroup[i].buttonHorizontal)
					{
						int maxX = 0;

						if (spritesGroup[i].imagesObject != null)
						{
							//prendo la sprite con la posizione più centrale (prendo quella col modulo della posizione maggiore)
							for (int j = 0; j<spritesGroup[i].imagesObject.Length; j++)
							{
								if (spritesGroup[i].imagesObject[j] != null)
								{
									int spritePos = System.Convert.ToInt16(spritesGroup[i].imagesObject[j].GetComponent<RectTransform>().anchoredPosition.x);
									if (Mathf.Abs (spritePos) > maxX)
										maxX = Mathf.Abs(spritePos);
								}
							}
						}

						XPosition = maxX + tempSize + distanceVariables.XDistance;
						YPosition = distanceVariables.YPadding;
					}else{
						int maxY = 0;

						if (spritesGroup[i].imagesObject != null)
						{
							//prendo la sprite con la posizione più centrale (prendo quella col modulo della posizione maggiore)
							for (int j = 0; j<spritesGroup[i].imagesObject.Length; j++)
							{
								if (spritesGroup[i].imagesObject[j] != null)
								{
									int spritePos = System.Convert.ToInt16(spritesGroup[i].imagesObject[j].GetComponent<RectTransform>().anchoredPosition.y);
									if (Mathf.Abs (spritePos) > maxY)
										maxY = Mathf.Abs(spritePos);
								}
							}
						}

						XPosition = distanceVariables.XPadding;
						YPosition = maxY + tempSize + distanceVariables.YDistance;
					}

					/*
					int tempSize = getSpritesGroupSize(pos);
					int XPosition = maxX + tempSize + distanceVariables.XDistance;
					int YPosition = distanceVariables.YPadding;
					*/
					
					//creo l'oggetto che contiene il component di tipo image
					spritesGroup[i].imageButtonObject = new GameObject();
					spritesGroup[i].imageButtonObject.transform.parent = transform;

					spritesGroup[i].imageButtonObject.name = pos.ToString() + "_Button";
					
					//prendo i riferimenti ai component
					UnityEngine.UI.Image tempImage = spritesGroup[i].imageButtonObject.AddComponent<UnityEngine.UI.Image>();
					RectTransform rectTransform = spritesGroup[i].imageButtonObject.GetComponent<RectTransform>();
					
					//a seconda della posizione setto parametri particolari, anchor e pivot delle rectTransform
					if (pos == UIPosition.UpperRight)
					{
						rectTransform.anchorMin = new Vector2(1.0f,1.0f);
						rectTransform.anchorMax = new Vector2(1.0f,1.0f);
						rectTransform.pivot = new Vector2(1.0f,1.0f);
					}else if(pos == UIPosition.UpperLeft)
					{
						rectTransform.anchorMin = new Vector2(0.0f,1.0f);
						rectTransform.anchorMax = new Vector2(0.0f,1.0f);
						rectTransform.pivot = new Vector2(0.0f,1.0f);
					}else if(pos == UIPosition.BottomRight)
					{
						rectTransform.anchorMin = new Vector2(1.0f,0.0f);
						rectTransform.anchorMax = new Vector2(1.0f,0.0f);
						rectTransform.pivot = new Vector2(1.0f,0.0f);
					}else if(pos == UIPosition.BottomLeft)
					{
						rectTransform.anchorMin = new Vector2(0.0f,0.0f);
						rectTransform.anchorMax = new Vector2(0.0f,0.0f);
						rectTransform.pivot = new Vector2(0.0f,0.0f);
					}
					
					//setto la dimensione
					int tempButtonSize = sizeVariables.buttonSprite;
					rectTransform.sizeDelta = new Vector2(tempButtonSize, tempButtonSize);
					
					//setto le sprites
					tempImage.sprite = spritesGroup[i].buttonSprite;
					
					//setto la posizione
					switch(pos)
					{
					case UIPosition.UpperRight:
						rectTransform.anchoredPosition = new Vector2(-XPosition, -YPosition);
						break;
					case UIPosition.UpperLeft:
						rectTransform.anchoredPosition = new Vector2(XPosition, -YPosition);
						break;
					case UIPosition.BottomRight:
						rectTransform.anchoredPosition = new Vector2(-XPosition, YPosition);
						break;
					case UIPosition.BottomLeft:
						rectTransform.anchoredPosition = new Vector2(XPosition, YPosition);
						break;
					}
					
					tempImage.preserveAspect = true;
					
					rectTransform.localScale = new Vector3(1.0f,1.0f,1.0f);

				}

			}
		}
	}
}

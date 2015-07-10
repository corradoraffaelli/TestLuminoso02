using UnityEngine;
using System.Collections;

public class RepeatingTiles : MonoBehaviour {

	public int rightTiles = 4;
	public int leftTiles = 2;
	public int upTiles = 2;
	public int downTiles = 1;
	int actualRightTiles;
	int actualLeftTiles;
	int actualUpTiles;
	int actualDownTiles;

	SpriteRenderer spriteRenderer;
	float sizeX;
	float sizeY;
	Vector3 originalPosition;

	Vector3[] positions;
	GameObject[] tiles;
	int positionNumbers = 1;

	Vector3 startingPosition;

	// Use this for initialization
	void Start () {

		setActualNumbers();

		spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer != null)
		{
			sizeX = spriteRenderer.bounds.size.x;
			sizeY = spriteRenderer.bounds.size.y;
		}

		originalPosition = transform.position;

		generatePositions();

		createAllTiles();

		hideActualSpriteRenderer();

	}
	
	// Update is called once per frame
	void Update () {
		if (verifyIfChanged()){
			//rimette lo sprite renderer visibile, cancella i vecchi tiles e rimette i valori "actual" con quelli opportuni
			hideActualSpriteRenderer(false);
			removeTiles();
			setActualNumbers();

			//reinizializzazione
			generatePositions();
			createAllTiles();
			hideActualSpriteRenderer(true);
		}
	}

	//crea un singolo tile, in base alla posizione passata in input ed all'indice relativo all'array di tiles
	void createTile(Vector3 position, int i = 0)
	{
		//istanzio un nuovo oggetto nella posizione specificata
		GameObject tempGO = Instantiate(gameObject, position, gameObject.transform.rotation) as GameObject;
		tempGO.transform.parent = transform.parent;
		tiles[i] = tempGO;
		//tempGO.transform.SetParent(transform);
		//tempGO.transform.parent = transform;
		//cancello il component, altrimenti vengono generate copie all'infinito
		Destroy(tempGO.GetComponent<RepeatingTiles>());
	}

	//crea i tile
	void createAllTiles()
	{
		for (int i = 0; i < positions.Length; i++)
		{
			createTile(positions[i], i);
		}
	}

	//disabilita/abilita lo sprite renderer dell'oggetto
	void hideActualSpriteRenderer(bool hide = true)
	{
		spriteRenderer.enabled = !hide;
	}

	//riempie l'array di posizioni in base al numero di tiles da creare e crea anche l'array di gameObjects
	void generatePositions()
	{
		int orizontalTiles = rightTiles + leftTiles + 1;
		int verticalTiles = upTiles + downTiles + 1;

		positionNumbers = orizontalTiles * verticalTiles;
		positions = new Vector3[positionNumbers];
		tiles = new GameObject[positionNumbers];

		float startX = originalPosition.x - leftTiles * sizeX;
		float startY = originalPosition.y - downTiles * sizeY;

		startingPosition = new Vector3(startX, startY, originalPosition.z);

		for (int i = 0; i < orizontalTiles; i++)
		{
			for (int j = 0; j < verticalTiles; j++)
			{
				Vector3 tempPosition = new Vector3(startingPosition.x + i* sizeX, startingPosition.y + j* sizeY, originalPosition.z);
				positions[verticalTiles*i + j] = tempPosition;
			}
		}
	}

	//setta i valori delle variabili "actual", utili a capire se cè stato un cambiamento
	void setActualNumbers()
	{
		actualUpTiles = upTiles;
		actualRightTiles = rightTiles;
		actualLeftTiles = leftTiles;
		actualDownTiles = downTiles;
	}

	//distrugge tutti gli oggetti di tipo tiles
	void removeTiles()
	{
		for (int i = 0; i<tiles.Length; i++)
		{
			if (tiles[i] != null)
			{
				Destroy(tiles[i]);
			}

		}
	}

	//ritorna vero se si è cambiato il numero di tiles in ingresso
	bool verifyIfChanged()
	{
		if (actualUpTiles != upTiles || actualRightTiles != rightTiles || actualLeftTiles != leftTiles || actualDownTiles != downTiles)
			return true;
		else
			return false;

	}
}

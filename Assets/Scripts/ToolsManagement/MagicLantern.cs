using UnityEngine;
using System.Collections;

public class MagicLantern : Tool {

	//public GameObject lantern;
	//public GameObject cube;

	public Glass[] glassList;
	int glassIndex = 0;
	Glass actualGlass;

	//public GameObject glasses;
	public GameObject raggio_cerchio;
	public GameObject raggio;
	public GameObject cameraPoint;
	public GameObject camera;

	public float zPositionEnvironment = 0.0f;
	public float resizeFactor = 4.0f;
	public float maxProjectingDistance = 8.5f;

	public Sprite normalRay;
	public Sprite normalCircle;
	public Sprite pressedRay;
	public Sprite pressedCircle;

	Sprite goodGlass;
	Sprite badGlass;
	Sprite projectionSprite;
	Sprite blurredSprite;

	public GameObject projectionPrefab;

	Vector3 cameraPointPos;
	Vector3 _direction;
	Vector3 pos_move;

	//grandezza della sprite con il raggio
	float xSize;
	float ySize;
	Bounds boundsRay;

	SpriteRenderer spRendRay;
	SpriteRenderer spRendCircle;

	ProjectionCollision PC;
	bool wasGoodProjection = false;

	//--------INITIALIZATION AND ACTIVATION-------------------------------------

	protected override void initializeTool() {
		//componenti Sprite Renderer del raggio
		spRendRay = raggio.GetComponent<SpriteRenderer> ();
		spRendCircle = raggio_cerchio.GetComponent<SpriteRenderer> ();

		boundsRay = spRendRay.bounds;
		xSize = boundsRay.size.x;
		ySize = boundsRay.size.y;

		//lo script ProjectionCollision si occupa di controllare che la proiezione collida con i muri
		PC = transform.GetComponent<ProjectionCollision>();

		//la prima volta che attivo la lanterna, prendo il primo vetrino usabile
		actualGlass = nextUsableGlass (true);
	}

	protected override void activationToolFunc()
	{
		//posiziono la lenterna di fronte al personaggio
		//!!!!!!--------------DA CONTROLLARE!!!!-------- probabilmente basta settare la lanterna come figlio del player nella giusta posizione
		toolGameObject.transform.position = new Vector3(player.transform.position.x+0.4f,player.transform.position.y+0.8f,player.transform.position.z);
	}

	//ritorna true, solo se esiste almeno un vetrino usabile
	public override bool canBeActivated()
	{
		Glass tempGlass = nextUsableGlass (true);
		if (tempGlass != null) {
			return true;
		} else {
			return false;
		}
	}

	//--------------------UPDATE---------------------------

	//qui va inserita la logica del tool, usata nell'update...
	protected override void useTool() {

		//movimenti del raggio e della proiezione sotto al mouse
		normalMovementsUnderMouse ();

		//se il mouse è tenuto premuto, cambia la sprite del raggio
		if (usingDrag)
			changeRaySprite (true);

		//quando si rilascia il mouse, viene proiettato l'oggetto, se le condizioni sono soddisfatte
		if (Input.GetMouseButtonUp (0)) {
			changeRaySprite (false);
			if (PC.isColliding() && !verifyIfTooFar ())
				placeImage();
		}

		//DEBUG: se si preme E, si passa al prossimo vetrino
		if (Input.GetKeyUp (KeyCode.E)) {
			actualGlass = nextUsableGlass();
		}

		//si aggiornano le sprites del vetrino
		//viene fatto ad ogni update, da verificare se troppo pesante, precedentemente queste funzioni erano poste in modo più intelligente,
		//sono state inserite nell'update per semplificare
		glassSpriteUpdate ();
		updateSpriteAfterChanging ();
			
		//switch delle sprite nel caso la proiezione si trovi o meno su un muro adatto
		if (PC.isColliding() && !wasGoodProjection) {
			switchProjection (true);
			wasGoodProjection = true;
		} else if (!PC.isColliding() && wasGoodProjection) {
			switchProjection(false);
			wasGoodProjection = false;
		}

		//switch delle sprite nel caso la proiezione si trovi o meno troppo lontano dal player
		if (verifyIfTooFar())
			PC.changeSprite (blurredSprite);
	}

	//verifica se la proiezione è troppo lontana dal player
	bool verifyIfTooFar()
	{
		Vector3 pos_cursor = raggio_cerchio.transform.position;
		Vector3 pos_player = player.transform.position;
		float distance = Vector3.Distance (pos_player, pos_cursor);
		if (Mathf.Abs (distance) > maxProjectingDistance)
			return true;
		else
			return false;
	}

	//funzione per aggiornare le variabili contenenti le sprites
	//----!!!!! il cambiamento di queste variabili non modifica ciò che viene mostrato a schermo, a meno di un aggiornamento
	// (vedi updateSpriteAfterChanging())
	void glassSpriteUpdate()
	{
		if (actualGlass != null) {
			goodGlass = actualGlass.goodProjection;
			badGlass = actualGlass.badProjection;
			projectionSprite = actualGlass.spriteObject;
			blurredSprite = actualGlass.blurredProjection;
		}

	}

	//fa l'update delle sprites della proiezione, a seconda che questa si trovi o meno in corrispondenza di un muro
	void updateSpriteAfterChanging()
	{
		if (PC.isColliding ()) {
			PC.changeSprite (goodGlass);
		} else {
			PC.changeSprite (badGlass);
		}
	}

	//movimenti del raggio e della proiezione in relazione alla posizione del player e del cursore
	void normalMovementsUnderMouse(){
		
		//posiziono la lenterna di fronte al personaggio (ora non serve, la lanterna è "child" del player, da verificare la correttezza della cosa
		//toolGameObject.transform.position = new Vector3(player.transform.position.x+0.4f,player.transform.position.y+0.8f,player.transform.position.z);
		
		//posiziono l'origine del cerchio sotto il mouse
		pos_move = new Vector3 (actualMousePosition.x, actualMousePosition.y, zPositionEnvironment);
		raggio_cerchio.transform.position = new Vector3( pos_move.x, pos_move.y, pos_move.z );
		
		//prendo la posizione del punto frontale della sprite della camera, e ci piazzo l'origine del raggio
		cameraPointPos = cameraPoint.transform.position;
		raggio.transform.position = cameraPointPos;
		
		//prendo la direzione tra l'inizio del raggio e la posizione del cerchio
		_direction = (raggio_cerchio.transform.position - cameraPointPos).normalized;
		
		//cambio la scala del raggio in base alla distanza tra camera e cerchio
		float distance = Vector3.Distance (raggio_cerchio.transform.position, cameraPointPos);
		raggio.transform.localScale = new Vector3(distance / xSize,distance / (ySize*resizeFactor),1);
		raggio_cerchio.transform.localScale = new Vector3(raggio.transform.localScale.y,raggio.transform.localScale.y,1);
		
		//setto la direzione del raggio
		//se il personaggio non sta guardando verso destra la sua scala è -1, devo perciò correggere quella della direzione, di conseguenza
		PlayerMovements PM = player.GetComponent<PlayerMovements> ();
		if (!PM.FacingRight) {
			_direction = new Vector3 (-_direction.x, _direction.y,_direction.z);
			//il cerchio e la proiezione contenuta in esso non devono flippare insieme al personaggio
			raggio_cerchio.transform.localScale = new Vector3(-raggio_cerchio.transform.localScale.x,raggio_cerchio.transform.localScale.y,raggio_cerchio.transform.localScale.z);
		}
		raggio.transform.right = _direction;
		
		//setto la direzione della camera
		if (raggio.transform.localEulerAngles.z > 180)
			camera.transform.localEulerAngles = new Vector3 (raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.x, (raggio.transform.localEulerAngles.z-360) / 2);
		else
			camera.transform.localEulerAngles = new Vector3 (raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.z / 2);
		
		//test flipping personaggio
		//flippa se il raggio punta dietro al personaggio
		if ((PM.FacingRight && raggio_cerchio.transform.position.x < player.transform.position.x)
		    || (!PM.FacingRight && raggio_cerchio.transform.position.x > player.transform.position.x))
			PM.c_flip ();
	}

	//funzione da chiamare per abilitare disabilitare la possibilità di usare un vetrino
	//------!!! DA TESTARE!!!!------------------
	public void enableGlass(int glassIndexToEnable, bool enable = true)
	{
		if (enable) {
			glassList[glassIndexToEnable].usable = true;
			actualGlass = glassList[glassIndexToEnable];
			//glassSpriteUpdate ();
		}else{
			glassList[glassIndexToEnable].usable = false;
			actualGlass = nextUsableGlass();
			//glassSpriteUpdate ();
		}
	}

	//passa al prossimo vetrino (ORA INUTILIZZATA)
	void nextGlass()
	{
		if (glassIndex < (glassList.Length-1)) {
			glassIndex = glassIndex + 1;
		} else {
			glassIndex = 0;
		}
		actualGlass = glassList [glassIndex];
	}

	//ritorna il prossimo vetrino usabile, se first è abilitato, ritorna il prossimo, a partire da quello attuale (utile per primo avvio)
	Glass nextUsableGlass(bool first = false)
	{
		//salvo l'indice del vetrino prima dell'incremento
		int indexBeforeIncrement = glassIndex;

		while (true) {
			//incremento l'indice (solo se non uso il parametro first)
			if (!first)
			{
				if (glassIndex < (glassList.Length-1)) {
					glassIndex = glassIndex + 1;
				} else {
					glassIndex = 0;
				}
			}
			first = false;
			if (glassList[glassIndex].Usable)
			{
				return glassList[glassIndex];
			}else{
				if (glassIndex == indexBeforeIncrement)
				{
					return null;
				}
			}	
		}
	}





	//cambia la sprite del raggio, se needToChangeSprite è true, imposta la sprite più "visibile"
	void changeRaySprite(bool needToChangeSprite)
	{
		if (needToChangeSprite) {
			spRendRay.sprite = pressedRay;
			spRendCircle.sprite = pressedCircle;
		} else {
			spRendRay.sprite = normalRay;
			spRendCircle.sprite = normalCircle;
		}
	}

	//istanzia un nuovo oggetto di tipo "Projection" una volta che si clicca sulla posizione voluta
	void placeImage()
	{
		deleteOldProjection ();

		Bounds objBounds = PC.getSpriteBounds ();

		actualGlass.projectionObject = Instantiate <GameObject> (projectionPrefab);
		
		actualGlass.projectionObject.transform.position = new Vector3(actualMousePosition.x, actualMousePosition.y, zPositionEnvironment);
		
		SpriteRenderer actualSprite = actualGlass.projectionObject.transform.GetComponent<SpriteRenderer> ();
		actualSprite.sprite = projectionSprite;
		Bounds newObjBounds = actualSprite.bounds;
		
		float spriteScale = objBounds.size.x / newObjBounds.size.x;
		actualGlass.projectionObject.transform.localScale = new Vector3 (spriteScale, spriteScale, spriteScale);

		actualGlass.projectionObject.SetActive (true);

	}

	//cancella il vecchio oggetto instanziato
	void deleteOldProjection()
	{
		if (actualGlass.projectionObject)
			Destroy (actualGlass.projectionObject);
	}

	//cambia la sprite della proiezione, se good = true imposta la sprite utile a comprendere che è possibile proiettare e quindi creare l'oggetto
	void switchProjection(bool good)
	{
		if (good)
			PC.changeSprite (goodGlass);
		else
			PC.changeSprite (badGlass);
	}
}

using UnityEngine;
using System.Collections;

public class MagicLantern : Tool {

	//public GameObject lantern;
	//public GameObject cube;

	Glass[] glassList;
	int glassIndex;
	//Glass actualGlass;

	int projectedGlass;

	//public GameObject glasses;
	GameObject raggio_cerchio;
	GameObject raggio;
	GameObject cameraPoint;
	GameObject camera;
	GameObject projectionObject;

	public float zPositionEnvironment = 0.0f;
	public float resizeFactor = 4.0f;
	public float maxProjectingDistance = 8.5f;
	public float minProjectingDistance = 1.5f;
	public bool newSizeImplementation = true;
	public float defaultCircleSize = 3.5f;
	public float defaultDircleDistance = 4.0f;
	[Range(0.0f,3.0f)]
	public float changeSizeMultiplier = 1.8f;
	public bool limitProjectionDistance = true;
	[Range(0.0f,1.0f)]
	public float alphaProjection = 0.5f;

	public Sprite normalRay;
	public Sprite normalCircle;
	public Sprite pressedRay;
	public Sprite pressedCircle;

	Sprite goodGlassSprite;
	Sprite badGlassSprite;
	Sprite projectionSprite;
	Sprite blurredSprite;
	Sprite emptySprite;

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
	GlassesManager glassesManager;

	bool createdProjection = false;
	bool leftLantern = false;

	//parametri per il fine livello
	//bool endedProjected = false;
	//bool timerStarted = false;
	//float timer = 0.0f;
	//public float projectionTimer = 6.0f;

	//ParticleSystem partSyst;
	//public float maxParticleEmission = 5000.0f;

	//GameObject tempProjectedObject;
	//SpriteRenderer tempSR;

	//public bool proiettaInSagomaMode = false;

	//ParticleEmitter partEmit;

	//--------INITIALIZATION AND ACTIVATION-------------------------------------

	protected override void initializeTool() {
	
		//prendo gli oggetti dalla scena e li salvo nelle rispettive variabili
		takeObjectsFromScene ();

		//prendo i components a partire dalle variabili appena salvate
		takeComponentsFromObjects ();

		boundsRay = spRendRay.bounds;
		xSize = boundsRay.size.x;
		ySize = boundsRay.size.y;

		PC.setProjectionObject(projectionObject);
	}



	protected override void activationToolFunc()
	{
		//posiziono la lenterna di fronte al personaggio
		//!!!!!!--------------DA CONTROLLARE!!!!-------- probabilmente basta settare la lanterna come figlio del player nella giusta posizione
		//toolGameObject.transform.position = new Vector3(player.transform.position.x+0.4f,player.transform.position.y+0.8f,player.transform.position.z);

		//presupponendo che la lanterna sia figlia dell'oggetto
		toolGameObject.transform.localPosition = new Vector3 (0.4f, 0.8f, 0.0f);
		//actualGlass = nextUsableGlass (true);
		/*
		if (glassesManager.isThereAUsableGlass ()) {
			actualGlass = glassesManager.getActualGlass();
			glassSpriteUpdate ();
		}
			*/
		
	}

	//ritorna true, solo se esiste almeno un vetrino usabile
	public override bool canBeActivated()
	{
		if (glassesManager.isThereAUsableGlass() && activable)
			return true;
		else
			return false;
	}

	public void setActivable(bool activableOrNot)
	{
		activable = activableOrNot;
	}

	//--------------------UPDATE---------------------------

	//qui va inserita la logica del tool, usata nell'update...
	protected override void useTool() {
		//caso in cui la lanterna non è lasciata a terra
		if (!leftLantern) {
			//slowTime();
			//movimenti del raggio e della proiezione sotto al mouse
			normalMovementsUnderMouse ();

			//si aggiornano le sprites del vetrino
			//viene fatto ad ogni update, da verificare se troppo pesante, precedentemente queste funzioni erano poste in modo più intelligente,
			//sono state inserite nell'update per semplificare
			if (glassesManager.isThereAUsableGlass ()) {
				//actualGlass = glassesManager.getActualGlass();
				glassSpriteUpdate ();
			}

			//disattivo la lanterna se non sono a terra
			if (!player.GetComponent<PlayerMovements>().OnGround)
			{
				toolSwitcher TS = transform.parent.gameObject.GetComponent<toolSwitcher> ();
				TS.useTool (false);
				TS.switchUsingTool (false);
			}

			changeRayAndCircleSprites (normalRay, normalCircle);
			changeProjectionSprite (projectionSprite);
			deleteInstantiatedPrefab();
			//deleteActualProjection ();

			if (verifyIfTooFar () && !limitProjectionDistance)
				changeProjectionSprite (blurredSprite);

			//abbasso l'alpha della sprite in modalità "mira"
			PC.setAlphaSprite(alphaProjection);

			//se lascio il tasto
			//if (Input.GetButtonUp("Mira") || (Input.GetAxis("Mira")<0.5f))
			//Debug.Log (Input.GetAxisRaw("Mira"));
			if (Input.GetButtonUp("Mira"))
			{
				//fastTime();

				//la lascio e proietto se sono nella buona posizione
				if (!verifyIfTooFar () || limitProjectionDistance) {
					
					//è un vetrino normale
					changeRayAndCircleSprites (pressedRay, pressedCircle);
					changeProjectionSprite (emptySprite);
					if (!createdProjection) {
						createdProjection = true;
						instantiatePrefab ();
					}
					
					leftLantern = true;
					toolGameObject.transform.parent = transform;
					
				}else{
					//disattivo la lanterna se non è nella buona posizione
					toolSwitcher TS = transform.parent.gameObject.GetComponent<toolSwitcher> ();
					TS.useTool (false);
					TS.switchUsingTool (false);
				}
			}


		}

		//se ho lasciato la lanterna
		else 
		{
			//se ripremo il tasto, la riprendo in mano
			if (Input.GetButtonDown("Mira"))
			{
				//deleteOldProjection();
				//deleteInstantiatedPrefab();
				leftLantern = false;
				setPlayerAsParent();
			}
		}


	}


	//chiamata quando la lanterna viene disattivata
	protected override void disactivationToolFunc()
	{
		setPlayerAsParent ();
		leftLantern = false;
	}


	void slowTime()
	{
		float tempoLento = 0.1f;
		Time.timeScale = Mathf.Lerp (1.0f, tempoLento, 0.5f*Time.deltaTime);

	}

	void fastTime()
	{
		float tempoLento = 0.1f;
		Time.timeScale = Mathf.Lerp (tempoLento, 1.0f, 1.0f*Time.deltaTime);
	}

	//impone il player come oggetto parent della lanterna
	void setPlayerAsParent()
	{
		toolGameObject.transform.parent = player.transform;
		toolGameObject.transform.localPosition = new Vector3(0.4f, 0.8f, 0.0f);
		//risolvo in maniera malamente il problema del flipping inspiegabile dell'oggetto
		Vector3 actualScale = toolGameObject.transform.localScale;
		toolGameObject.transform.localScale = new Vector3 (Mathf.Abs(actualScale.x),Mathf.Abs(actualScale.y),Mathf.Abs(actualScale.z));
	}

	void changeRayAndCircleSprites(Sprite RaySprite, Sprite CircleSprite)
	{
		spRendRay.sprite = RaySprite;
		spRendCircle.sprite = CircleSprite;
	}

	void setAplhaProjectionSprite (float alphaValue)
	{
		PC.setAlphaSprite (alphaValue);
	}

	void changeProjectionSprite(Sprite neoProjectionSprite)
	{
		PC.changeSprite (neoProjectionSprite);
	}

	//verifica se la proiezione è troppo lontana dal player
	bool verifyIfTooFar()
	{
		//Vector3 pos_cursor = raggio_cerchio.transform.position;
		Vector3 pos_cursor = cursorHandler.getCursorWorldPosition ();
		Vector3 pos_player = cameraPoint.transform.position;
		//Vector3 pos_player = player.transform.position;
		float distance = Vector3.Distance (pos_player, pos_cursor);
		if (Mathf.Abs (distance) > maxProjectingDistance)
			return true;
		else
			return false;
	}

	//verifica se la proiezione è troppo vicina al player
	bool verifyIfTooNear()
	{
		//Vector3 pos_cursor = raggio_cerchio.transform.position;
		Vector3 pos_cursor = cursorHandler.getCursorWorldPosition ();
		Vector3 pos_player = cameraPoint.transform.position;
		//Vector3 pos_player = player.transform.position;
		float distance = Vector3.Distance (pos_player, pos_cursor);
		if (Mathf.Abs (distance) < minProjectingDistance)
			return true;
		else
			return false;
	}

	//verifica se la proiezione è troppo vicina al player
	bool verifyIfCursorCameraTooNear()
	{
		//RaycastHit2D[] cacca =  RaycastAll(new Vector2(cameraPoint.transform.position.x, cameraPoint.transform.position.y), Vector2 direction, float distance = Mathf.Infinity, int layerMask = DefaultRaycastLayers, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity);

		//Vector3 pos_cursor = raggio_cerchio.transform.position;
		Vector3 pos_cursor = cursorHandler.getCursorWorldPosition ();
		Vector3 pos_player = cameraPoint.transform.position;

		Vector2 pos_cursor2 = new Vector2 (pos_cursor.x, pos_cursor.y);
		Vector2 pos_player2 = new Vector2 (pos_player.x, pos_player.y);
		Vector2 direction2 = (pos_player2 - pos_cursor2);
		RaycastHit2D[] cacca =  Physics2D.RaycastAll(pos_cursor2, direction2, Mathf.Infinity);
		for (int i = 0; i< cacca.Length; i++)
		{
			//Debug.Log (cacca[i].transform.gameObject.name);
		}
		//Debug.Log (cacca.Length);

		//Vector3 pos_player = player.transform.position;
		float distance = Vector3.Distance (pos_player, pos_cursor);
		//Debug.Log (Mathf.Abs (distance));
		if (Mathf.Abs (distance) < 0.3f)
			return true;
		else
			return false;
	}

	//funzione per aggiornare le variabili contenenti le sprites
	//----!!!!! il cambiamento di queste variabili non modifica ciò che viene mostrato a schermo, a meno di un aggiornamento
	void glassSpriteUpdate()
	{
		if (glassesManager.getActualGlass() != null) {
			goodGlassSprite = glassesManager.getActualGlass().goodProjection;
			badGlassSprite = glassesManager.getActualGlass().badProjection;
			projectionSprite = glassesManager.getActualGlass().spriteObject;
			blurredSprite = glassesManager.getActualGlass().blurredProjection;
			emptySprite = glassesManager.getActualGlass().emptySprite;
		}

	}

	//movimenti del raggio e della proiezione in relazione alla posizione del player e del cursore
	void normalMovementsUnderMouse(){
		//primo if per verificare che il cursore non sia troppo vicino alla lanterna, per evitare strani flipping
		//si potrebbe pensare di limitare il cursore, ma per ora troppo complesso
		if (!verifyIfCursorCameraTooNear ()) {

			//posiziono la lenterna di fronte al personaggio (ora non serve, la lanterna è "child" del player, da verificare la correttezza della cosa
			//toolGameObject.transform.position = new Vector3(player.transform.position.x+0.4f,player.transform.position.y+0.8f,player.transform.position.z);
			
			//posiziono l'origine del cerchio sotto il mouse
			if (limitProjectionDistance && verifyIfTooFar ())
				raggio_cerchio.transform.position = getPositionAlongDirection (cameraPoint.transform.position, cursorHandler.getCursorWorldPosition (), maxProjectingDistance);
			else if (limitProjectionDistance && verifyIfTooNear ())
				raggio_cerchio.transform.position = getPositionAlongDirection (cameraPoint.transform.position, cursorHandler.getCursorWorldPosition (), minProjectingDistance);
			else
				raggio_cerchio.transform.position = cursorHandler.getCursorWorldPosition ();
			
			//prendo la posizione del punto frontale della sprite della camera, e ci piazzo l'origine del raggio
			cameraPointPos = cameraPoint.transform.position;
			raggio.transform.position = cameraPointPos;
			
			//prendo la direzione tra l'inizio del raggio e la posizione del cerchio
			_direction = (raggio_cerchio.transform.position - cameraPointPos).normalized;
			
			//cambio la scala del raggio in base alla distanza tra camera e cerchio
			float distance = Vector3.Distance (raggio_cerchio.transform.position, cameraPointPos);

			//calcolo della scala
			if (newSizeImplementation)
			{
				float actualSize = defaultCircleSize + (defaultCircleSize*(distance-defaultDircleDistance)*changeSizeMultiplier/10.0f);
				raggio.transform.localScale = new Vector3(distance / xSize,actualSize,1);
				raggio_cerchio.transform.localScale = new Vector3(raggio.transform.localScale.y,raggio.transform.localScale.y,1);
			}else{
				raggio.transform.localScale = new Vector3(distance / xSize,distance / (ySize*resizeFactor),1);
				raggio_cerchio.transform.localScale = new Vector3(raggio.transform.localScale.y,raggio.transform.localScale.y,1);
			}



			
			//setto la direzione del raggio
			//se il personaggio non sta guardando verso destra la sua scala è -1, devo perciò correggere quella della direzione, di conseguenza
			PlayerMovements PM = player.GetComponent<PlayerMovements> ();
			if (!PM.FacingRight) {
				_direction = new Vector3 (-_direction.x, _direction.y,_direction.z);
				//il cerchio e la proiezione contenuta in esso non devono flippare insieme al personaggio
				raggio_cerchio.transform.localScale = new Vector3(-raggio_cerchio.transform.localScale.x,raggio_cerchio.transform.localScale.y,raggio_cerchio.transform.localScale.z);
			}
			raggio.transform.right = _direction;

			Debug.Log (raggio.transform.localEulerAngles.z);

			//setto la direzione della camera
			if (!(raggio.transform.localEulerAngles.z > 179 && raggio.transform.localEulerAngles.z < 220)) {
				if (raggio.transform.localEulerAngles.z > 180)
					camera.transform.localEulerAngles = new Vector3 (raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.x, (raggio.transform.localEulerAngles.z-360) / 2);
				else
					camera.transform.localEulerAngles = new Vector3 (raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.z / 2);
			}
			
			
			//test flipping personaggio
			//flippa se il raggio punta dietro al personaggio
			/*
		if (((PM.FacingRight && raggio_cerchio.transform.position.x < player.transform.position.x)
		    || (!PM.FacingRight && raggio_cerchio.transform.position.x > player.transform.position.x))
		    && !player.GetComponent<PlayerMovements>().running)
			PM.c_flip ();
			*/
		}

	}


	Vector3 getPositionAlongDirection(Vector3 startPosition, Vector3 endingPosition, float distance)
	{
		float catetoX = Mathf.Abs (startPosition.x - endingPosition.x);
		float catetoY = Mathf.Abs (startPosition.y - endingPosition.y);
		float ipotenusa = Mathf.Sqrt(catetoX * catetoX + catetoY * catetoY);
		float dX = distance * catetoX / ipotenusa;
		float dY = distance * catetoY / ipotenusa;

		float posX, posY;
		if (startPosition.x < endingPosition.x)
			posX = dX + startPosition.x;
		else
			posX = startPosition.x - dX;

		if (startPosition.y < endingPosition.y)
			posY = dY + startPosition.y;
		else
			posY = startPosition.y - dY;

		Vector3 pos = new Vector3 (posX, posY, zPositionEnvironment);
		return pos;
	}

	//funzione per istanziare il prefab dell'oggetto relativo al vetrino, sotto il cursore
	void instantiatePrefab()
	{
		if (glassesManager.getActualGlass().prefabObject) {
			//istanzio un prefab e lo sposto sotto il mouse
			glassesManager.getActualGlass().projectionObject = Instantiate <GameObject> (glassesManager.getActualGlass().prefabObject);
			glassesManager.getActualGlass().projectionObject.transform.position = raggio_cerchio.transform.position;
			//actualGlass.projectionObject.transform.position = new Vector3(actualMousePosition.x, actualMousePosition.y, zPositionEnvironment);
			
			//prendo i bounds della sprite della proiezione
			Bounds objBounds = PC.getSpriteBounds ();
			//prendo lo spriteRenderer del prefab ed i suoi bounds
			SpriteRenderer actualSprite = glassesManager.getActualGlass().projectionObject.transform.GetComponent<SpriteRenderer> ();
			//actualSprite.sprite = projectionSprite;
			Bounds newObjBounds = actualSprite.bounds;
			
			//scalo l'oggetto per far sì che la sua sprite coincida con quella sottostante (teoricamente vuota)
			float spriteScale = objBounds.size.x / newObjBounds.size.x;
			glassesManager.getActualGlass().projectionObject.transform.localScale = new Vector3 (spriteScale, spriteScale, spriteScale);
			
			//metto l'oggetto come figlio del raggio_cerchio
			glassesManager.getActualGlass().projectionObject.transform.parent = raggio_cerchio.transform;
			
			glassesManager.getActualGlass().projectionObject.SetActive (true);

			projectedGlass = glassesManager.getActualGlassIndex();
			Debug.Log(projectedGlass);
		}

	}

	void deleteInstantiatedPrefab()
	{
		//cancello solo la proiezione creata (se dovessero esserci problemi, prova ad usare il ciclo sotto
		//Destroy (glassesManager.getGlassList () [projectedGlass].projectionObject);

		createdProjection = false;

		//cancella tutte le proiezioni

		for (int i = 0; i< glassesManager.getGlassList().Length; i++) {
			if (glassesManager.getGlassList()[i].projectionObject != null)
				Destroy (glassesManager.getGlassList()[i].projectionObject);
		}

	}

	//chiamato quando è necessario eliminare il vecchio prefab istanziato (quando si cambia vetrino, quando si esce da un muro o si va fuori fuoco)
	/*
	void deleteActualProjection()
	{
		createdProjection = false;

		if (glassesManager.getActualGlass()) {
			if (glassesManager.getActualGlass().projectionObject)
				Destroy (glassesManager.getActualGlass().projectionObject);
		}

	}


	//cancella il vecchio oggetto instanziato
	void deleteOldProjection()
	{

		if (glassesManager.getActualGlass().projectionObject)
			Destroy (glassesManager.getActualGlass().projectionObject);
	}
	*/

	void takeObjectsFromScene()
	{
		foreach (Transform child in player.transform) {
			if (child.name == "MagicLantern") {
				toolGameObject = child.gameObject;
				break;
			}
		}
		
		foreach (Transform child in toolGameObject.transform) {
			if (child.name == "Proiettore")
			{
				camera = child.gameObject;
				foreach (Transform subChild in child.transform) {
					if (subChild.name == "Proiettore_punta")
					{
						cameraPoint = subChild.gameObject;
					}
				}
			}else if(child.name == "raggio"){
				raggio = child.gameObject;
			}else if (child.name == "raggio_cerchio"){
				raggio_cerchio = child.gameObject;
				foreach (Transform subChild in child.transform) {
					if (subChild.name == "ProjectedObject")
					{
						projectionObject = subChild.gameObject;
					}
				}
			}
		}
	}
	
	void takeComponentsFromObjects()
	{
		//lo script ProjectionCollision si occupa di controllare che la proiezione collida con i muri
		PC = transform.GetComponent<ProjectionCollision>();
		
		//sistema particellare
		//partSyst = raggio_cerchio.GetComponent<ParticleSystem> ();
		
		//componenti Sprite Renderer del raggio
		spRendRay = raggio.GetComponent<SpriteRenderer> ();
		spRendCircle = raggio_cerchio.GetComponent<SpriteRenderer> ();

		glassesManager = transform.GetComponent<GlassesManager> ();
	}
}

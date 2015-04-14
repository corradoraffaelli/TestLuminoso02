using UnityEngine;
using System.Collections;

public class MagicLantern : Tool {

	//public GameObject lantern;
	//public GameObject cube;

	public Glass[] glassList;
	int glassIndex = 0;
	Glass actualGlass;

	//public GameObject glasses;
	GameObject raggio_cerchio;
	GameObject raggio;
	GameObject cameraPoint;
	GameObject camera;
	GameObject projectionObject;

	//public LayerMask toChase;
	//public LayerMask hiding;

	public float zPositionEnvironment = 0.0f;
	public float resizeFactor = 4.0f;
	public float maxProjectingDistance = 8.5f;

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

	bool createdProjection = false;
	bool leftLantern = false;

	//parametri per il fine livello
	//bool endedProjected = false;
	bool timerStarted = false;
	float timer = 0.0f;
	public float projectionTimer = 6.0f;

	ParticleSystem partSyst;
	public float maxParticleEmission = 5000.0f;

	GameObject tempProjectedObject;
	SpriteRenderer tempSR;

	public bool proiettaInSagomaMode = false;

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
		actualGlass = nextUsableGlass (true);
		glassSpriteUpdate ();
	}

	//ritorna true, solo se esiste almeno un vetrino usabile
	public override bool canBeActivated()
	{
		Glass tempGlass = nextUsableGlass (true);

		if (tempGlass != null) {
			return true;
		} else {
			Debug.Log ("non attivabile");
			return false;
		}
	}

	//--------------------UPDATE---------------------------

	//qui va inserita la logica del tool, usata nell'update...
	protected override void useTool() {
		//caso in cui la lanterna non è lasciata a terra
		if (!leftLantern) {
			//movimenti del raggio e della proiezione sotto al mouse
			normalMovementsUnderMouse ();

			//si aggiornano le sprites del vetrino
			//viene fatto ad ogni update, da verificare se troppo pesante, precedentemente queste funzioni erano poste in modo più intelligente,
			//sono state inserite nell'update per semplificare
			glassSpriteUpdate ();




				
			//disattivo la lanterna se non sono a terra
			if (!player.GetComponent<PlayerMovements>().OnGround)
			{
				toolSwitcher TS = transform.parent.gameObject.GetComponent<toolSwitcher> ();
				TS.useTool (false);
				TS.switchUsingTool (false);
			}

			if (!actualGlass.endingLevelGlass) {
				changeRayAndCircleSprites (normalRay, normalCircle);
				changeProjectionSprite (badGlassSprite);
				deleteActualProjection ();

				if (proiettaInSagomaMode)
				{
					if (PC.isColliding () && verifyIfTooFar ()) {
						changeRayAndCircleSprites (pressedRay, pressedCircle);
						changeProjectionSprite (blurredSprite);
						deleteActualProjection ();
						
						// la proiezione è OK
					} else if (PC.isColliding () && !verifyIfTooFar ()) {
						
						//è un vetrino normale
						changeRayAndCircleSprites (pressedRay, pressedCircle);
						changeProjectionSprite (emptySprite);
						if (!createdProjection) {
							createdProjection = true;
							instantiatePrefab ();
						}
					}
				}

				//se lascio il tasto
				if (Input.GetButtonUp("Mira"))
				{
					//la lascio e proietto se sono nella buona posizione
					if (PC.isColliding () && !verifyIfTooFar ()) {
						
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

				resetEndingGlassVariables ();
			}

			/*
			if (proiettaInSagomaMode)
			{

			}
			if (!actualGlass.endingLevelGlass) {
				// la proiezione si trova fuori da un muro adatto
				if (!PC.isColliding ()) {
					changeRayAndCircleSprites (normalRay, normalCircle);
					changeProjectionSprite (badGlassSprite);
					deleteActualProjection ();
					
					// la proiezione si trova in un muro adatto, ma è troppo lontana e risulta fuori fuoco
				} else if (PC.isColliding () && verifyIfTooFar ()) {
					changeRayAndCircleSprites (pressedRay, pressedCircle);
					changeProjectionSprite (blurredSprite);
					deleteActualProjection ();
					
					// la proiezione è OK
				} else if (PC.isColliding () && !verifyIfTooFar ()) {
					
					//è un vetrino normale
					changeRayAndCircleSprites (pressedRay, pressedCircle);
					changeProjectionSprite (emptySprite);
					if (!createdProjection) {
						createdProjection = true;
						instantiatePrefab ();
					}
				}
				
				resetEndingGlassVariables ();
			}
			*/
			//immplementazione vecchia
			/*
			if (!actualGlass.endingLevelGlass) {
				// la proiezione si trova fuori da un muro adatto
				if (!PC.isColliding ()) {
					changeRayAndCircleSprites (normalRay, normalCircle);
					changeProjectionSprite (badGlassSprite);
					deleteActualProjection ();
					
					// la proiezione si trova in un muro adatto, ma è troppo lontana e risulta fuori fuoco
				} else if (PC.isColliding () && verifyIfTooFar ()) {
					changeRayAndCircleSprites (pressedRay, pressedCircle);
					changeProjectionSprite (blurredSprite);
					deleteActualProjection ();
					
					// la proiezione è OK
				} else if (PC.isColliding () && !verifyIfTooFar ()) {
					
					//è un vetrino normale
					changeRayAndCircleSprites (pressedRay, pressedCircle);
					changeProjectionSprite (emptySprite);
					if (!createdProjection) {
						createdProjection = true;
						instantiatePrefab ();
					}
				}

				resetEndingGlassVariables ();
			} else {
				//si tratta di un vetrino di fine livello
				changeRayAndCircleSprites (normalRay, normalCircle);
				changeProjectionSprite (badGlassSprite);

				//se mi trovo nella posizione giusta e non ho ancora proiettato il vetrino
				if (actualGlass.controlIfOverlap (PC.getSpriteBounds ()) && !actualGlass.endedProjected) {
					//il timer è partito
					if (timerStarted) {
						EndingGlassTimerStarted();
						//sto attivando ora il timer, devo fare tutte le operazioni di avvio
					} else {
						EndingGlassFirstTime();
					}
				} else {
					resetEndingGlassVariables ();
				}

			}
			*/

			//DEBUG: se si preme E, si passa al prossimo vetrino
			if (Input.GetKeyUp (KeyCode.E)) {
				deleteActualProjection ();
				actualGlass = nextUsableGlass ();
			}
		}

		//se ho lasciato la lanterna
		else 
		{
			//se ripremo il tasto, la riprendo in mano
			if (Input.GetButtonDown("Mira"))
			{
				leftLantern = false;
				setPlayerAsParent();
			}
		}

		//se premo C o il tasto sinistro del mouse lascio la lanterna
		/*
		if (Input.GetKeyUp (KeyCode.C) || Input.GetMouseButtonUp(0)) {
			leftLantern = !leftLantern;
			if (leftLantern)
				toolGameObject.transform.parent = transform;
			else
			{
				setPlayerAsParent ();
			}
				
		}
		*/
	}

	//gestione di quello che deve succedere la prima volta che la proiezione del vetrino di fine livello si trova
	//nella giusta posizione
	void EndingGlassFirstTime()
	{
		//istanzio un nuovo oggetto proiezione, da sfumare
		tempProjectedObject = Instantiate (projectionObject);
		//tempProjectedObject = new GameObject();
		//UnityEditorInternal.ComponentUtility.CopyComponent(PC.getSpriteRenderer());
		//UnityEditorInternal.ComponentUtility.PasteComponentAsNew(tempProjectedObject);
		tempProjectedObject.transform.parent = raggio_cerchio.transform;
		tempProjectedObject.transform.localPosition = projectionObject.transform.localPosition;
		tempProjectedObject.transform.localScale = projectionObject.transform.localScale;
		
		tempSR = tempProjectedObject.GetComponent<SpriteRenderer>();
		tempSR.sprite = projectionSprite;
		tempSR.color = new Color(tempSR.color.r, tempSR.color.g, tempSR.color.b, 0.0f);

		partSyst.enableEmission = true;

		timerStarted = true;
	}

	//gestione di quello che deve succedere una volta istanziato ed avviato un nuovo timer per il vetrino di fine livello
	void EndingGlassTimerStarted()
	{
		//changeProjectionSprite (projectionSprite);
		projectionEffects ();
		timer += Time.deltaTime;
		
		//ho superato il momento in cui devo istanziare la proiezione del vetrino di fine livello
		if (timer > projectionTimer) {
			//attivo tutti i GameObjects relativi al particolare vetrino
			actualGlass.activeEndingLevelObjects ();
			
			//disattiva la lanterna
			actualGlass.endedProjected = true;
			actualGlass.Usable = false;
			toolSwitcher TS = transform.parent.gameObject.GetComponent<toolSwitcher> ();
			TS.useTool (false);
			TS.switchUsingTool (false);
			
			resetEndingGlassVariables();
		}
	}

	//gestione della disattivazione degli eventi relativi alla proiezione del vetrino di fine livello
	//(non più nella giusta posizione ecc.)
	void resetEndingGlassVariables()
	{
		timerStarted = false;
		timer = 0.0f;
		if (tempProjectedObject)
			Destroy(tempProjectedObject);
		partSyst.enableEmission = false;
		PC.setAlphaSprite (1.0f);
	}

	//chiamata quando la lanterna viene disattivata
	protected override void disactivationToolFunc()
	{
		setPlayerAsParent ();
		leftLantern = false;
	}

	//effetti durante la proiezione del vetrino di fine livello
	void projectionEffects()
	{
		partSyst.emissionRate = (timer*maxParticleEmission)/projectionTimer;
		//SpriteRenderer tempSR = tempProjectedObject.GetComponent<SpriteRenderer>();
		//tempSR.color = new Color(tempSR.color.r, tempSR.color.g, tempSR.color.b, (timer*1.0f)/projectionTimer);
		tempSR.color = new Color(tempSR.color.r, tempSR.color.g, tempSR.color.b, (timer*1.0f)/projectionTimer);
		setAplhaProjectionSprite (1.0f-((timer * 1.0f) / projectionTimer));
	}

	//impone il player come oggetto pparent della lanterna
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
	void glassSpriteUpdate()
	{
		if (actualGlass != null) {
			goodGlassSprite = actualGlass.goodProjection;
			badGlassSprite = actualGlass.badProjection;
			projectionSprite = actualGlass.spriteObject;
			blurredSprite = actualGlass.blurredProjection;
			emptySprite = actualGlass.emptySprite;
		}

	}

	//movimenti del raggio e della proiezione in relazione alla posizione del player e del cursore
	void normalMovementsUnderMouse(){
		
		//posiziono la lenterna di fronte al personaggio (ora non serve, la lanterna è "child" del player, da verificare la correttezza della cosa
		//toolGameObject.transform.position = new Vector3(player.transform.position.x+0.4f,player.transform.position.y+0.8f,player.transform.position.z);
		
		//posiziono l'origine del cerchio sotto il mouse
		//pos_move = new Vector3 (cursorHandler.getCursorWorl.x, actualMousePosition.y, zPositionEnvironment);
		//raggio_cerchio.transform.position = new Vector3( pos_move.x, pos_move.y, pos_move.z );
		raggio_cerchio.transform.position = cursorHandler.getCursorWorldPosition ();
		
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
		/*
		if (((PM.FacingRight && raggio_cerchio.transform.position.x < player.transform.position.x)
		    || (!PM.FacingRight && raggio_cerchio.transform.position.x > player.transform.position.x))
		    && !player.GetComponent<PlayerMovements>().running)
			PM.c_flip ();
			*/
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

			if (glassList[glassIndex].Usable)
			{
				return glassList[glassIndex];
			}else{
				if (glassIndex == indexBeforeIncrement && first == false)
				{
					return null;
				}
			}	
			first = false;
		}
	}

	//funzione per istanziare il prefab dell'oggetto relativo al vetrino, sotto il cursore
	void instantiatePrefab()
	{
		if (actualGlass.prefabObject) {
			//istanzio un prefab e lo sposto sotto il mouse
			actualGlass.projectionObject = Instantiate <GameObject> (actualGlass.prefabObject);
			actualGlass.projectionObject.transform.position = new Vector3(actualMousePosition.x, actualMousePosition.y, zPositionEnvironment);
			
			//prendo i bounds della sprite della proiezione
			Bounds objBounds = PC.getSpriteBounds ();
			//prendo lo spriteRenderer del prefab ed i suoi bounds
			SpriteRenderer actualSprite = actualGlass.projectionObject.transform.GetComponent<SpriteRenderer> ();
			//actualSprite.sprite = projectionSprite;
			Bounds newObjBounds = actualSprite.bounds;
			
			//scalo l'oggetto per far sì che la sua sprite coincida con quella sottostante (teoricamente vuota)
			float spriteScale = objBounds.size.x / newObjBounds.size.x;
			actualGlass.projectionObject.transform.localScale = new Vector3 (spriteScale, spriteScale, spriteScale);
			
			//metto l'oggetto come figlio del raggio_cerchio
			actualGlass.projectionObject.transform.parent = raggio_cerchio.transform;
			
			actualGlass.projectionObject.SetActive (true);
		}

	}

	//Attualmente inutilizzata
	/*
	void prefabFollowingCursor()
	{
		//actualGlass.projectionObject.transform.position = ;

		Bounds objBounds = PC.getSpriteBounds ();
		Bounds newObjBounds = actualGlass.projectionObject.transform.GetComponent<SpriteRenderer> ().bounds;
		//float spriteScale = objBounds.size.x / newObjBounds.size.x;
		//actualGlass.projectionObject.transform.localScale = new Vector3 (spriteScale, spriteScale, spriteScale);
		//newObjBounds.SetMinMax (objBounds.min, objBounds.max);
		//float spriteScale = objBounds.size.x / newObjBounds.size.x;
		//actualGlass.projectionObject.transform.localScale = new Vector3 (spriteScale, spriteScale, spriteScale);
		//Debug.Log (spriteScale);
		Debug.Log (newObjBounds.size);
		Debug.Log ("obj " + objBounds.min);
		Debug.Log ("obj " + objBounds.max);
		Debug.Log ("new " + newObjBounds.min);
		Debug.Log ("new " + newObjBounds.max);
	}

	//istanzia un nuovo oggetto di tipo "Projection" una volta che si clicca sulla posizione voluta (attualmente inutilizzato)

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
	*/

	//chiamato quando è necessario eliminare il vecchio prefab istanziato (quando si cambia vetrino, quando si esce da un muro o si va fuori fuoco)
	void deleteActualProjection()
	{
		createdProjection = false;
		if (actualGlass.projectionObject)
			Destroy (actualGlass.projectionObject);
	}

	//cancella il vecchio oggetto instanziato
	void deleteOldProjection()
	{
		if (actualGlass.projectionObject)
			Destroy (actualGlass.projectionObject);
	}


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
		partSyst = raggio_cerchio.GetComponent<ParticleSystem> ();
		
		//componenti Sprite Renderer del raggio
		spRendRay = raggio.GetComponent<SpriteRenderer> ();
		spRendCircle = raggio_cerchio.GetComponent<SpriteRenderer> ();
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

	private int convertBinToDec(int binintval) {
		
		switch (binintval) {
			
		case 256 :
			return 8;
			break;
			
		case 512 :
			return 9;
			break;
			
		case 1024 :
			return 10;
			break;
			
		case 2048 :
			return 11;
			break;
			
		case 4096 :
			return 12;
			break;
			
		case 8192 :
			return 13;
			break;
			
		case 16384 :
			return 14;
			break;
			
		case 32768 :
			return 15;				
			break;
			
		case 65536 :
			return 16;
			break;
			
		case 131072 :
			return 17;
			break;
			
		default :
			break;
			
		}
		return 0;
	}
}

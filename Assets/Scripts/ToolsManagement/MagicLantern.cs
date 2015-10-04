using UnityEngine;
using System.Collections;

public class MagicLantern : Tool {

	//public GameObject lantern;
	//public GameObject cube;

	/*
	 * Le variabili FA (First Activation) servono per identificare la prima volta che si verifica un cambiamento di stato, servono per non appesantire le funzioni dell'update
	 */

	public enum lanternState
	{
		None,
		NotUsed,
		InHand,
		Left,
		TurnedOff,
		Falling
	};

	public lanternState actualState = lanternState.NotUsed;
	//utile per verificare i primi cambiamenti
	lanternState previousState = lanternState.NotUsed;
	
	float turnedOffTime = 0.0f;
	public float timeReturnFalling = 1.5f;

	//indica se posso prendere in mano la lanterna o meno, ad esempio può essere utile per le particles
	bool usable = true;
	public bool touchedByEnemy = false;

	public bool collidingBadWall = false;

	//Glass[] glassList;
	//int glassIndex;
	//Glass actualGlass;

	int projectedGlass;

	public bool leftLantern = true;

	//public GameObject glasses;
	/*
	GameObject raggio_cerchio;
	GameObject raggio;
	GameObject cameraPoint;
	GameObject camera;
	GameObject projectionObject;

	public float zPositionEnvironment = 0.0f;
	public float resizeFactor = 4.0f;
	public float maxProjectingDistance = 8.5f;
	public float minProjectingDistance = 1.0f;
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
	*/

	public GameObject projectionPrefab;

	/*
	Vector3 cameraPointPos;
	Vector3 _direction;
	Vector3 pos_move;


	//grandezza della sprite con il raggio
	float xSize;
	float ySize;
	Bounds boundsRay;

	SpriteRenderer spRendRay;
	SpriteRenderer spRendCircle;
	*/

	//ProjectionCollision PC;
	GlassesManager glassesManager;

	GraphicLantern graphicLantern;

	public bool disabledByEnemy = true;
	public bool canFall = true;

	bool createdProjection = false;
	
	ReturnParticles returnParticles;
	Vector3 lastLanternPosition;

	AudioHandler audioHandler;

	Vector3 lastInstatiatePosition;

	bool inPlayOLD = true;
	public bool canChangeState = true;
	float timeBeforeCanChangeState = 0.3f;
	float lastTimeNotPlay = 0.0f;

	//--------INITIALIZATION AND ACTIVATION-------------------------------------

	protected override void initializeTool() {
		glassesManager = transform.GetComponent<GlassesManager> ();
		graphicLantern = GetComponent<GraphicLantern> ();
		returnParticles = GetComponentInChildren<ReturnParticles> ();
		audioHandler = GetComponent<AudioHandler> ();

		/*
		//prendo gli oggetti dalla scena e li salvo nelle rispettive variabili
		takeObjectsFromScene ();

		//prendo i components a partire dalle variabili appena salvate
		takeComponentsFromObjects ();

		boundsRay = spRendRay.bounds;
		xSize = boundsRay.size.x;
		ySize = boundsRay.size.y;

		PC.setProjectionObject(projectionObject);
		*/
	}



	protected override void activationToolFunc()
	{
		//posiziono la lenterna di fronte al personaggio
		//!!!!!!--------------DA CONTROLLARE!!!!-------- probabilmente basta settare la lanterna come figlio del player nella giusta posizione
		//toolGameObject.transform.position = new Vector3(player.transform.position.x+0.4f,player.transform.position.y+0.8f,player.transform.position.z);

		//presupponendo che la lanterna sia figlia dell'oggetto
		//toolGameObject.transform.localPosition = new Vector3 (0.4f, 0.8f, 0.0f);
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

		if (inputKeeper!=null && inputKeeper.isButtonUp ("GlassModifier") && glassesManager.isGlassModifiable ())
			glassesManager.callGlassModifier ();

		if (actualState != lanternState.Left) {
			if (!glassesManager.getActualGlass ().rotateWithLantern)
				graphicLantern.resetCircleAngle ();
			
			if (!glassesManager.getActualGlass ().canBeModified)
				graphicLantern.resetTempProjectionAngle ();
		}



		//DEBUG
		if (Input.GetKeyUp(KeyCode.G))
			enemyTouchLantern(true);

		setStates ();

		if (actualState == lanternState.InHand) {
			//AZIONI DA FARE APPENA PRESA IN MANO
			if (previousState != actualState)
			{
				deleteInstantiatedPrefab();

				if (previousState != lanternState.NotUsed)
					callParticles();
					
				graphicLantern.setSuperSortingLayer();
				graphicLantern.switchOnLantern();
				graphicLantern.switchSupport(false);
				graphicLantern.takeLantern();

				if (!collidingBadWall)
					graphicLantern.setLeftLanternRaySrites(false);
				else
				{
					graphicLantern.setBadRaySprites(true);
				}
					
				deleteInstantiatedPrefab();

				audioHandler.playClipByName("Accensione");
				audioHandler.playClipByName("Mira");
				//returnParticles.activeParticles(graphicLantern.getLanternPosition());
			}

			if (!collidingBadWall)
				graphicLantern.setLeftLanternRaySrites(false);
			else
			{
				graphicLantern.setBadRaySprites(true);
			}

			//AZIONI CHE VENGONO FATTE SEMPRE CON LA LANTERNA IN MANO
			//importante aggiornare le sprites solo se la lanterna è in mano, per ora non cambia niente, ma secondo me è meglio farlo così, poi vediamo
			if (glassesManager.isThereAUsableGlass ()) {
				graphicLantern.glassSpriteUpdate();
			}

			graphicLantern.normalMovementsUnderMouse();
			graphicLantern.setNormalFakeProjectionSprite();

			//if (glassesManager.getActualGlass().rotateWithLantern)
			//	graphicLantern.rotateCircle();

			touchedByEnemy = false;
		}

		if (actualState == lanternState.NotUsed) {
			//AZIONI DA FARE APPENA LA LANTERNA SCOMPARE (NOT ON GROUND oppure TASTO SINISTRO)
			if (previousState != actualState)
			{
				audioHandler.playClipByName("Spegnimento");
				if (previousState != lanternState.InHand)
					callParticles();

				graphicLantern.switchOffLantern();
			}
			
		}

		if (actualState == lanternState.Left) {
			//AZIONI DA FARE APPENA LA LANTERNA E' LASCIATA A TERRA
			if (previousState != actualState)
			{
				graphicLantern.switchSupport();
				graphicLantern.leaveLantern();
				graphicLantern.changeAlphaFakeProjectionSprite(0.0f);

				graphicLantern.setLeftLanternRaySrites();

				instantiatePrefab();

				graphicLantern.putLanternOnPlayer();

				audioHandler.playClipByName("ApparizioneProiezione");
			}
			
		}

		if (actualState == lanternState.TurnedOff) {
			//AZIONI DA FARE APPENA LA LANTERNA VIENE TOCCATA DA UN NEMICO
			if (previousState != actualState)
			{
				graphicLantern.switchOffRay();
				enemyTouchLantern(false);

				audioHandler.playClipByName("Spegnimento");
			}
			
		}

		if (actualState == lanternState.Falling) {
			if (previousState != actualState)
			{
				graphicLantern.switchOffRay();
				graphicLantern.addRigidbody();

				turnedOffTime = Time.time;

				if (previousState != lanternState.TurnedOff)
					audioHandler.playClipByName("Spegnimento");
				//audioHandler.playClipByName("FallingLantern");
			}

			//dopo un po' che la lanterna sta cadendo, mi torna in mano, come NotUsed
			if (Mathf.Abs (Time.time - turnedOffTime) > timeReturnFalling)
			{
				callParticles();
				actualState = lanternState.NotUsed;
			}
				
		}

		if (actualState != lanternState.Falling) {
			if (previousState != actualState)
			{

				graphicLantern.removeRigidbody();
			}
		}

		if (actualState != lanternState.InHand) {
			if (previousState == lanternState.InHand)
			{
				graphicLantern.setSuperSortingLayer(false);
				audioHandler.stopClipByName("Mira");
			}

		}

		if (actualState != lanternState.Left)
		{
			if (previousState == lanternState.Left && lastInstatiatePosition!=null)
			{
				graphicLantern.instantiateClouds(lastInstatiatePosition);
			}
		}

		//aggiorno il previousState con quello attuale, al prossimo frame servirà per i controlli
		previousState = actualState;

		updateLeftLantern ();

		handleCanChangeState();
	}


	protected override void useToolPaused() {
		if (!PlayStatusTracker.inPlay)
		{
			lastTimeNotPlay = Time.unscaledTime;
			canChangeState = false;
		}
	}

	void setStates()
	{
		if (inputKeeper != null && canChangeState) {
			if (inputKeeper.isButtonPressed ("Mira") && player.GetComponent<PlayerMovements> ().onGround && usable) {
				actualState = lanternState.InHand;
			}
			
			if (inputKeeper.isButtonPressed ("Mira") && !player.GetComponent<PlayerMovements> ().onGround) {
				actualState = lanternState.NotUsed;
			}
			
			if (inputKeeper.isButtonUp ("PickLantern") && (actualState == lanternState.Left || actualState == lanternState.TurnedOff)) {
				actualState = lanternState.NotUsed;
			}
			
			//questo controllo va prima di quando si setta a Left o TurnedOff, per evitare che nel setStates venga prima messo a Left e, subito dopo, a Falling,
			//così da perdere ciò che si fa nell'update del Left
			if (canFall) {
				if (actualState == lanternState.TurnedOff || actualState == lanternState.Left)
				{
					if (!graphicLantern.groundCheck() && graphicLantern.LanternOnPlayerPosition)
					{
						actualState = lanternState.Falling;
					}
				}
			}
			
			//questo controllo va prima di quando si setta a Left, per evitare che nel setStates venga prima messo a Left e, subito dopo, a TurnedOff,
			//così da perdere ciò che si fa nell'update del Left
			if (actualState == lanternState.Left && touchedByEnemy) {
				actualState = lanternState.TurnedOff;
			}

			/*
			if (actualState == lanternState.InHand && inputKeeper.isButtonUp("Mira") && collidingBadWall) {
				actualState = lanternState.NotUsed;
			}
			*/

			if (actualState == lanternState.InHand && inputKeeper.isButtonUp("Mira")) {
				actualState = lanternState.Left;
			}

			if (actualState == lanternState.Left && collidingBadWall) {
				actualState = lanternState.NotUsed;
				collidingBadWall = false;
			}
		}




			
	}

	void callParticles()
	{
		audioHandler.playClipByName("Ritorno");
		returnParticles.activeParticles(graphicLantern.getLanternPosition());
	}

	//da implementare, dovrebbe ritornare se il nemico tocca la lanterna, così da spegnerla
	bool enemyTouchLantern(bool touchedOrNot)
	{
		touchedByEnemy = touchedOrNot;
		return touchedByEnemy;
	}

	public void c_touchedByEnemy()
	{
		enemyTouchLantern (true);
		Debug.Log ("spenta dal nemico");
	}
		


	//il valore leftLantern è usato da playerMovements, magari si può pensare ad altro, poiché per ora non indica propriamente un "left"
	void updateLeftLantern()
	{
		if (actualState != lanternState.InHand)
			leftLantern = true;
		else
			leftLantern = false;
	}


	/*
	void slowTime()
	{
		float tempoLento = 0.1f;
		Time.timeScale = Mathf.Lerp (Time.timeScale, tempoLento, 0.5f*Time.deltaTime);

	}

	void fastTime()
	{
		float tempoLento = 0.1f;
		Time.timeScale = Mathf.Lerp (Time.timeScale, 1.0f, 1.0f*Time.deltaTime);
	}

	*/


	//funzione per istanziare il prefab dell'oggetto relativo al vetrino, sotto il cursore
	void instantiatePrefab()
	{

		if (glassesManager.getActualGlass().prefabObject) {
			//cancello un eventuale vecchio prefab
			deleteInstantiatedPrefab();
			if (glassesManager.getActualGlass().projectionObject != null)
				Destroy(glassesManager.getActualGlass().projectionObject);

			//istanzio un prefab e lo sposto sotto il mouse
			glassesManager.getActualGlass().projectionObject = Instantiate <GameObject> (glassesManager.getActualGlass().prefabObject);
			glassesManager.getActualGlass().projectionObject.transform.position = graphicLantern.getCircleRay().position;
			//actualGlass.projectionObject.transform.position = new Vector3(actualMousePosition.x, actualMousePosition.y, zPositionEnvironment);

			//3 righe inutili nel caso in cui la proiezione non debba ruotare insieme alla lanterna
			//glassesManager.getActualGlass().projectionObject.transform.parent = graphicLantern.getCircleRay();
			//glassesManager.getActualGlass().projectionObject.transform.localEulerAngles = new Vector3(0.0f,0.0f,0.0f);
			//glassesManager.getActualGlass().projectionObject.transform.parent = null;

			//vecchia implementazione, da vedere, attualmente funziona solo se la scala è la stessa, sia della sprite della proiezione che della sagoma
			/*
			//prendo i bounds della sprite della proiezione
			Bounds objBounds = graphicLantern.getProjectionBounds();
			//prendo lo spriteRenderer del prefab ed i suoi bounds
			SpriteRenderer actualSprite = glassesManager.getActualGlass().projectionObject.transform.GetComponent<SpriteRenderer> ();
			//actualSprite.sprite = projectionSprite;
			Bounds newObjBounds = actualSprite.bounds;
			
			//scalo l'oggetto per far sì che la sua sprite coincida con quella sottostante (teoricamente vuota)
			float spriteScale = objBounds.size.x / newObjBounds.size.x;
			glassesManager.getActualGlass().projectionObject.transform.localScale = new Vector3 (spriteScale, spriteScale, spriteScale);
			
			//metto l'oggetto come figlio del raggio_cerchio
			glassesManager.getActualGlass().projectionObject.transform.parent = graphicLantern.getCircleRay();
			*/

			//nuova implementazione - INIZIO
			glassesManager.getActualGlass().projectionObject.transform.parent = graphicLantern.getCircleRay();
			glassesManager.getActualGlass().projectionObject.transform.localScale = graphicLantern.getTempProjectionScale();


			//glassesManager.getActualGlass().projectionObject.transform.localEulerAngles = graphicLantern.getTempProjectionAngle();
			glassesManager.getActualGlass().projectionObject.transform.localEulerAngles = new Vector3(0.0f,0.0f,graphicLantern.getStandardFakeProjectionRotation());
			//nuova implementazione - FINE


			glassesManager.getActualGlass().projectionObject.SetActive (true);

			projectedGlass = glassesManager.getActualGlassIndex();

			lastInstatiatePosition = glassesManager.getActualGlass().projectionObject.transform.position;
			//Debug.Log(projectedGlass);
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

	public void setCollidingBadWall(bool colliding)
	{
		collidingBadWall = colliding;
	}

	void handleCanChangeState()
	{
		if (PlayStatusTracker.inPlay)
		{
			if ((Time.unscaledTime - lastTimeNotPlay) > timeBeforeCanChangeState)
				canChangeState = true;
			else
				canChangeState = false;
		}
	}
	
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Classe che gestisce tutta la logica della lanterna magica
/// </summary>

//Corrado
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

	public GameObject projectionPrefab;

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
	
	}

	protected override void activationToolFunc()
	{
		
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

			glassesManager.getActualGlass().projectionObject.transform.parent = graphicLantern.getCircleRay();
			glassesManager.getActualGlass().projectionObject.transform.localScale = graphicLantern.getTempProjectionScale();

			glassesManager.getActualGlass().projectionObject.transform.localEulerAngles = new Vector3(0.0f,0.0f,graphicLantern.getStandardFakeProjectionRotation());

			glassesManager.getActualGlass().projectionObject.SetActive (true);

			projectedGlass = glassesManager.getActualGlassIndex();

			lastInstatiatePosition = glassesManager.getActualGlass().projectionObject.transform.position;
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

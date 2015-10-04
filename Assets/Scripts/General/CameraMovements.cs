using UnityEngine;
using System.Collections;

public class CameraMovements : MonoBehaviour {

	public bool newImplementation = false;
	public bool limitCameraMovements = false;

	GameObject player;
	GameObject GameOverObj;
	MagicLantern magicLanternLogic;

	Vector3 cameraCenter;
	Vector3 beginCamera;
	float xDistFromBeginning;
	float yDistFromBeginning;

	Vector3 playerPosition;
	Vector3 cursorWorldPosition;
	GameObject controller;

	Transform UpperLimit;
	Transform BottomLimit;
	Transform RightLimit;
	Transform LeftLimit;

	public bool cameraOnPlayer = false;
	public bool changeRatioIfAiming = true;
	public bool changeSize = true;
	public bool onlyIfAiming = true;
	public bool fixedChangedSize = false;
	public bool returnToPlayer = true;

	[Range(0,0.5f)]
	public float standardRatioDistance = 0.15f;
	[Range(0,0.5f)]
	public float aimingRatioDistance = 0.3f;
	float ratioDistanceFromPlayer;
	[Range(0.1f,30)]
	public float smooth = 10.0f;

	Vector3 BLActualLimit;
	Vector3 URActualLimit;

	CameraHandler cameraHandler;
	CursorHandler cursorHandler;

	[Range(0.1f,30)]
	public float changingSizeVelocity = 2.5f;
	[Range(1,30)]
	public float defaultSize = 5.5f;
	[Range(1,7)]
	public float enlargment = 1.0f;
	[Range(0,1)]
	public float ratioBeforeEnlargment = 0.75f;

	[Range(0,5)]
	public float timeBeforeGoingPlayer = 2.0f;
	float lastTimeMouseMoved = 0.0f;
	public float actualRatio = 0.0f;
	
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");

		//assignMainCameraToPlayer ();

		getGameOverObject ();

		controller = GameObject.FindGameObjectWithTag ("Controller");
		cursorHandler = controller.GetComponent<CursorHandler> ();

		cameraHandler = Camera.main.gameObject.GetComponent<CameraHandler> ();

		magicLanternLogic = GameObject.FindGameObjectWithTag ("MagicLanternLogic").GetComponent<MagicLantern> ();

		UpperLimit = cursorHandler.getUpperLimit();
		BottomLimit = cursorHandler.getBottomLimit();
		RightLimit = cursorHandler.getRightLimit();
		LeftLimit = cursorHandler.getLeftLimit();

		if (cameraHandler == null) {
			cameraCenter = new Vector3 (Camera.main.gameObject.transform.position.x, Camera.main.gameObject.transform.position.y, player.transform.position.z);
			beginCamera = Camera.main.ScreenToWorldPoint (new Vector3 (0.0f, 0.0f, player.transform.position.z));
			xDistFromBeginning = Mathf.Abs (cameraCenter.x - beginCamera.x);
			yDistFromBeginning = Mathf.Abs (cameraCenter.y - beginCamera.y);

			Debug.Log ("CameraHandler non trovato nell'oggetto Camera");
		} else {
			xDistFromBeginning = cameraHandler.getXDistFromBeginning();
			yDistFromBeginning = cameraHandler.getYDistFromBeginning();
		}

		ratioDistanceFromPlayer = standardRatioDistance;
		setCameraSize (defaultSize);
	}

	/*
	private void assignMainCameraToPlayer(){

		if(player!=null)
			player.GetComponent<PlayerMovements> ().MainCamera = this.gameObject;

	}
	*/

	private void getGameOverObject(){

		bool found = false;

		foreach (Transform child in transform) {

			if(child.name=="GameOver") {
				GameOverObj = child.gameObject;
				found = true;
			}
		}

		if (!found)
			Debug.Log ("ATTENZIONE - oggetto figlio GameOver dentro la camera, non trovato");

	}

	void Update () {

		if (!PlayStatusTracker.inPlay)
			return;

		if (cameraHandler != null) {
			xDistFromBeginning = cameraHandler.getXDistFromBeginning();
			yDistFromBeginning = cameraHandler.getYDistFromBeginning();
		}

		if (newImplementation) {

			//se il cursore si sta muovendo allora muovo la camera, altrimenti torno sul player
			if (!returnToPlayer || cursorHandler.isCursorMoving() || magicLanternLogic.actualState == MagicLantern.lanternState.InHand)
			//if (!cursorHandler.useController || !returnToPlayer || cursorHandler.isCursorMoving())
			{
				//11/06/2015 IMPLEMENTATION
				/*
				float tempStandardRatioDistance;
				if (cameraOnPlayer)
					tempStandardRatioDistance = 0.0f;
				else
					tempStandardRatioDistance = standardRatioDistance;
				
				if (changeRatioIfAiming){
					if (magicLanternLogic.actualState == MagicLantern.lanternState.InHand)
						setRatioDistance(aimingRatioDistance, true);
					else if (magicLanternLogic.actualState != MagicLantern.lanternState.InHand)
						setRatioDistance(tempStandardRatioDistance, true);
					
				}
				
				cursorWorldPosition = cursorHandler.getCursorWorldPosition ();
				//uso il centro della sprite anzichè la base del personaggio
				playerPosition = player.GetComponent<SpriteRenderer>().bounds.center;
				//playerPosition = player.transform.position;
				
				
				
				Vector3 newPosition =  getCameraPosition (ratioDistanceFromPlayer, cursorWorldPosition, playerPosition);
				//transform.position = newPosition;

				//ho pensato di aggiungere un moltiplicatore per fare in modo che, se il cursore è lontano dalla posizione attuale della camera, 
				//lo smooth è più lento (altrimenti dovremmo togliere il moltiplicatore "distance")
				float distance = Vector3.Distance(transform.position, newPosition);
				//Debug.Log (distance);

				if (distance > 1.0f)
				{
					transform.position = Vector3.Lerp (transform.position, newPosition, Time.deltaTime * smooth / distance);
				}
				else
				{
					transform.position = Vector3.Lerp (transform.position, newPosition, Time.deltaTime * smooth);
				}
				*/

				//12/06/2015 IMPLEMENTATION
				cursorWorldPosition = cursorHandler.getCursorWorldPosition ();
				playerPosition = player.GetComponent<SpriteRenderer>().bounds.center;
				actualRatio = Mathf.Lerp(actualRatio, standardRatioDistance, Time.deltaTime * smooth / 6);
				Vector3 newPosition =  getCameraPosition (actualRatio, cursorWorldPosition, playerPosition);
				transform.position = Vector3.Lerp (transform.position, newPosition, Time.deltaTime * smooth);
				//transform.position = newPosition;
			}
			else
			//tornare sul player
			{
				cursorWorldPosition = cursorHandler.getCursorWorldPosition ();
				playerPosition = player.GetComponent<SpriteRenderer>().bounds.center;
				actualRatio = Mathf.Lerp(actualRatio, 0.0f, Time.deltaTime * smooth / 10);
				Vector3 newPosition =  getCameraPosition (actualRatio, cursorWorldPosition, playerPosition);
				transform.position = Vector3.Lerp (transform.position, newPosition, Time.deltaTime * smooth);
				//transform.position = newPosition;
				/*
					playerPosition = player.GetComponent<SpriteRenderer>().bounds.center;
					Vector3 objVector = new Vector3(playerPosition.x, playerPosition.y, transform.position.z);
					
					float distance = Vector3.Distance(transform.position, objVector);
					
					transform.position = Vector3.Lerp (transform.position, objVector, Time.deltaTime * smooth  / (Mathf.Sqrt(distance) * 4));
				*/

			}


			if (limitCameraMovements)
				cameraLimitations();

		} else {
		
			if (UpperLimit)
			{
				if ((Mathf.Abs (beginCamera.x - player.transform.position.x) > xDistFromBeginning) && 
				    (Mathf.Abs (player.transform.position.x - UpperLimit.transform.position.x) > xDistFromBeginning)) {
					Camera.main.gameObject.transform.position = new Vector3 (player.transform.position.x, Camera.main.gameObject.transform.position.y, Camera.main.gameObject.transform.position.z);
				}
			}else if (Mathf.Abs (beginCamera.x - player.transform.position.x) > xDistFromBeginning){
				Camera.main.gameObject.transform.position = new Vector3 (player.transform.position.x, Camera.main.gameObject.transform.position.y, Camera.main.gameObject.transform.position.z);
			}


			if (Mathf.Abs (beginCamera.y - player.transform.position.y) > yDistFromBeginning) {
				Camera.main.gameObject.transform.position = new Vector3 (Camera.main.gameObject.transform.position.x, player.transform.position.y, Camera.main.gameObject.transform.position.z);
			}
		}

		if (changeSize && cameraHandler!=null)
			sizeManagement ();
	}

	//piazza la camera lungo la direzione tra il player ed il cursore, alla distanza passata come parametro
	Vector3 getCameraPosition(float distanceRatioFromPlayer, Vector3 curPos, Vector3 plPos)
	{
		float xCamera = (curPos.x - plPos.x) * distanceRatioFromPlayer + plPos.x;
		float yCamera = (curPos.y - plPos.y) * distanceRatioFromPlayer + plPos.y;
		Vector3 cameraPos = new Vector3(xCamera, yCamera, Camera.main.transform.position.z);
		return cameraPos;
	}

	void setCameraSize(float sizeInput, bool lerping = false)
	{
		if (!lerping)
			Camera.main.orthographicSize = sizeInput;
		else
			Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize, sizeInput, changingSizeVelocity * Time.deltaTime);
	}

	void setRatioDistance(float distanceInput, bool lerping = false)
	{
		if (!lerping)
			ratioDistanceFromPlayer = distanceInput;
		else
			ratioDistanceFromPlayer = Mathf.Lerp (ratioDistanceFromPlayer, distanceInput, changingSizeVelocity * Time.deltaTime);

	}

	void sizeManagement()
	{
		if (!fixedChangedSize) {
			if (!onlyIfAiming || (onlyIfAiming && magicLanternLogic.actualState == MagicLantern.lanternState.InHand))
			{
				float diffX = 0.0f;
				float xDistanceAllowed = Mathf.Abs (cameraHandler.getXDistFromBeginning ()) * ratioBeforeEnlargment;
				float xDistanceEffective = Mathf.Abs (cameraHandler.getCameraPositionZEnvironment ().x - cursorHandler.getCursorWorldPosition ().x);
				
				if (xDistanceAllowed < xDistanceEffective) {
					diffX = xDistanceEffective - xDistanceAllowed;
				}
				
				float diffY = 0.0f;
				float yDistanceAllowed = Mathf.Abs (cameraHandler.getYDistFromBeginning ()) * ratioBeforeEnlargment;
				float yDistanceEffective = Mathf.Abs (cameraHandler.getCameraPositionZEnvironment ().y - cursorHandler.getCursorWorldPosition ().y);
				
				if (yDistanceAllowed < yDistanceEffective) {
					diffY = yDistanceEffective - yDistanceAllowed;
				}
				
				float newSize = defaultSize + enlargment * 0.05f * diffX + enlargment * 0.05f * diffY;
				setCameraSize (newSize, true);
			}

		}

		if (fixedChangedSize) {
			if (onlyIfAiming && magicLanternLogic.actualState == MagicLantern.lanternState.InHand) {
				float newSize = defaultSize + enlargment * 0.4f;
				setCameraSize (newSize, true);
			}
		}

		if (onlyIfAiming && magicLanternLogic.actualState != MagicLantern.lanternState.InHand) {
			setCameraSize (defaultSize, true);
		}
	}

	//limita i movimenti della camera a seconda della scena
	void cameraLimitations()
	{
		//copiato dall'implementazione precedente
		cameraCenter = new Vector3 (Camera.main.gameObject.transform.position.x, Camera.main.gameObject.transform.position.y, player.transform.position.z);
		beginCamera = Camera.main.ScreenToWorldPoint (new Vector3 (0.0f, 0.0f, player.transform.position.z));
		xDistFromBeginning = Mathf.Abs (cameraCenter.x - beginCamera.x);
		yDistFromBeginning = Mathf.Abs (cameraCenter.y - beginCamera.y);

		//posizione nello spazio del mondo del vertice in basso a sinistra della camera
		if (LeftLimit || BottomLimit) {
			BLActualLimit = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, transform.position.z));
		}
		//posizione nello spazio del mondo del vertice in alto a destra della camera
		if (RightLimit || UpperLimit) {
			URActualLimit = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, transform.position.z));
		}

		if (LeftLimit) {
			if (LeftLimit.position.x > BLActualLimit.x){
				transform.position = new Vector3(LeftLimit.position.x+xDistFromBeginning, transform.position.y, transform.position.z);
			}	
		}
		if (RightLimit) {
			if (RightLimit.position.x < URActualLimit.x){
				transform.position = new Vector3(RightLimit.position.x-xDistFromBeginning, transform.position.y, transform.position.z);
			}	
		}
		if (UpperLimit) {
			if (UpperLimit.position.y < URActualLimit.y){
				transform.position = new Vector3(transform.position.x, UpperLimit.position.y-yDistFromBeginning, transform.position.z);
			}	
		}

		if (BottomLimit) {
			if (BottomLimit.position.y > BLActualLimit.y){
				transform.position = new Vector3(transform.position.x, BottomLimit.position.y+yDistFromBeginning, transform.position.z);
			}	
		}

	}

	public void GameOver(){

		if (GameOverObj != null) {
			StartCoroutine (handleGameOver ());
		}

	}

	private IEnumerator handleGameOver() {

		SpriteRenderer sr = GameOverObj.GetComponent<SpriteRenderer> ();
		sr.enabled = true;

		for (int i = 1; i<=20; i++) {
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, ((float)i)/20);
			yield return new WaitForSeconds(0.01f);

		}

		yield return new WaitForSeconds(1.0f);

		for (int i = 20; i>=0; i--) {
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, ((float)i)/15);
			yield return new WaitForSeconds(0.01f);
			
		}

		sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.0f);
		sr.enabled = false;
	}


}

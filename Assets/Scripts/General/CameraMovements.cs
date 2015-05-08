using UnityEngine;
using System.Collections;

public class CameraMovements : MonoBehaviour {

	public bool newImplementation = false;
	public bool limitCameraMovements = false;

	GameObject player;
	GameObject GameOverObj;

	Vector3 cameraCenter;
	Vector3 beginCamera;
	float xDistFromBeginning;
	float yDistFromBeginning;

	Vector3 playerPosition;
	Vector3 cursorWorldPosition;
	GameObject controller;
	CursorHandler CH;

	Transform UpperLimit;
	Transform BottomLimit;
	Transform RightLimit;
	Transform LeftLimit;

	public float RatioDistanceFromPlayer = 0.3f;
	public float smooth = 10.0f;

	Vector3 BLActualLimit;
	Vector3 URActualLimit;

	CameraHandler cameraHandler;
	CursorHandler cursorHandler;

	public bool changeSize = true;
	public float defaultSize = 5.5f;
	public float enlargment = 1.0f;
	public float ratioBeforeEnlargment = 0.75f;
	
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");

		assignMainCameraToPlayer ();

		getGameOverObject ();

		controller = GameObject.FindGameObjectWithTag ("Controller");
		CH = controller.GetComponent<CursorHandler> ();

		cameraHandler = Camera.main.gameObject.GetComponent<CameraHandler> ();

		UpperLimit = CH.getUpperLimit();
		BottomLimit = CH.getBottomLimit();
		RightLimit = CH.getRightLimit();
		LeftLimit = CH.getLeftLimit();

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
	}

	private void assignMainCameraToPlayer(){

		if(player!=null)
			player.GetComponent<PlayerMovements> ().MainCamera = this.gameObject;

	}

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
		if (cameraHandler != null) {
			xDistFromBeginning = cameraHandler.getXDistFromBeginning();
			yDistFromBeginning = cameraHandler.getYDistFromBeginning();
		}

		if (newImplementation) {

			cursorWorldPosition = CH.getCursorWorldPosition ();
			//uso il centro della sprite anzichè la base del personaggio
			playerPosition = player.GetComponent<SpriteRenderer>().bounds.center;
			//playerPosition = player.transform.position;

			Vector3 newPosition =  getCameraPosition (RatioDistanceFromPlayer, cursorWorldPosition, playerPosition);
			//transform.position = newPosition;
			transform.position = Vector3.Lerp (transform.position, newPosition, Time.deltaTime * smooth);

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

		if (changeSize)
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

	void setCameraSize(float sizeInput)
	{
		Camera.main.orthographicSize = sizeInput;
	}

	void sizeManagement()
	{
		float diffX = 0.0f;
		float xDistanceAllowed = Mathf.Abs (cameraHandler.getXDistFromBeginning ()) * ratioBeforeEnlargment;
		float xDistanceEffective = Mathf.Abs (cameraHandler.getCameraPositionZEnvironment ().x - CH.getCursorWorldPosition ().x);

		if (xDistanceAllowed < xDistanceEffective) {
			diffX = xDistanceEffective - xDistanceAllowed;
		}

		float diffY = 0.0f;
		float yDistanceAllowed = Mathf.Abs (cameraHandler.getYDistFromBeginning ()) * ratioBeforeEnlargment;
		float yDistanceEffective = Mathf.Abs (cameraHandler.getCameraPositionZEnvironment ().y - CH.getCursorWorldPosition ().y);

		if (yDistanceAllowed < yDistanceEffective) {
			diffY = yDistanceEffective - yDistanceAllowed;
		}

		float newSize = defaultSize + enlargment * 0.05f * diffX + enlargment * 0.05f * diffY;
		setCameraSize (newSize);

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

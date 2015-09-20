using UnityEngine;
using System.Collections;

public class CursorHandler : MonoBehaviour {

	public bool controllerAutomaticVar = true;

	public bool useController = false;
	public float xControllerMultiplier = 15.0f;
	public float yControllerMultiplier = 15.0f;

	Vector3 cursorPosition;
	public float zPositionEnvironment = 0.0f;
	GameObject player;
	PlayerMovements playerMovements;
	Vector3 playerPosition;

	Vector3 standardPosition;
	float xDiffPlayer;
	float yDiffPlayer;

	public Transform BottomLimit;
	public Transform LeftLimit;
	public Transform RightLimit;
	public Transform UpperLimit;

	Vector3 cameraCenter;
	Vector3 beginCamera;
	float xDistFromBeginning;
	float yDistFromBeginning;

	//variabili per determinare se il cursore si sta muovendo o meno
	//[HideInInspector]
	public bool cursorIsMoving;
	Vector3 oldCursorPosition;
	bool firstCursorPosition = true;
	public float ScreenRatioMovement = 0.3f;
	public float ScreenControllerRatioMovement = 0.3f;
	int timesControllerMoving = 0;
	public int maxTimesControllerMoving = 2;
	float lastTimeControllerMovingCheck = 0.0f;
	float timeBetweenControllerMoving = 0.1f;
	public float timeAfterMoving = 1.0f;
	float lastTimeMoving = 0.0f;

	//indica se c'è un intervallo di tempo, dopo che il cursore si è fermato per considerarsi ancora in movimento
	//settato di default per il mouse, la variabile indica se è necessario anche per il controller
	public bool movingAfterController = false;
	public bool movingIfStanding = true;

	bool oldGameState;

	CameraHandler cameraHandler;

	InputKeeper inputKeeper;

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Confined;
		player = GameObject.FindGameObjectWithTag ("Player");
		if (player != null)
			playerMovements = player.GetComponent<PlayerMovements>();
		cameraHandler = Camera.main.gameObject.GetComponent<CameraHandler> ();
		inputKeeper = GetComponent<InputKeeper> ();

		standardPosition = new Vector3 (Camera.main.gameObject.transform.position.x, Camera.main.gameObject.transform.position.z, zPositionEnvironment);

		//xDiffPlayer = standardPosition.x - player.transform.position.x;
		//yDiffPlayer = standardPosition.y - player.transform.position.y;
		xDiffPlayer = 1.0f;
		yDiffPlayer = 1.0f;

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

		oldGameState = PlayStatusTracker.inPlay;
		if (oldGameState)
			Cursor.visible = false;
		else if (!oldGameState && !useController)
		{
			Cursor.visible = true;
		}

		cursorIsMoving = false;

		/*
		cameraCenter = new Vector3 (Camera.main.gameObject.transform.position.x, Camera.main.gameObject.transform.position.y, player.transform.position.z);
		beginCamera = Camera.main.ScreenToWorldPoint (new Vector3 (0.0f, 0.0f, player.transform.position.z));
		xDistFromBeginning = Mathf.Abs (cameraCenter.x - beginCamera.x);
		yDistFromBeginning = Mathf.Abs (cameraCenter.y - beginCamera.y);
		*/
	}
	
	// Update is called once per frame
	void Update () {

		if (oldGameState != PlayStatusTracker.inPlay)
		{
			oldGameState = PlayStatusTracker.inPlay;
			if (oldGameState)
				Cursor.visible = false;
			else if (!oldGameState && !useController)
			{
				Cursor.visible = true;
				return;
			}
		}

		if (cameraHandler != null) {
			xDistFromBeginning = cameraHandler.getXDistFromBeginning();
			yDistFromBeginning = cameraHandler.getYDistFromBeginning();
		}

		if(inputKeeper != null)
			setCursorPosition ();
		//Debug.Log (getCursorWorldPosition());
		verifyCursorMoving ();

		changeUseController();
	}

	void setCursorPosition()
	{
		//Debug.Log (xDiffPlayer);


		if (!useController) {
			Vector3 actualMousePosition = Camera.main.ScreenToWorldPoint (new Vector3 (inputKeeper.getMousePosition().x, inputKeeper.getMousePosition().y, 0.0f));
			cursorPosition = new Vector3 (actualMousePosition.x, actualMousePosition.y, zPositionEnvironment);
			cursorPosition = limitCursorScreenPosition(cursorPosition);
		} else {
			float xSumPosition, ySumPosition;

			if (inputKeeper.getAxis("CursorHorizontal") > 0.2f || inputKeeper.getAxis("CursorHorizontal") < -0.2f)
			{
				xSumPosition = xControllerMultiplier * Time.deltaTime * inputKeeper.getAxis("CursorHorizontal")* inputKeeper.getAxis("CursorHorizontal")* inputKeeper.getAxis("CursorHorizontal");
				//if (Input.GetAxis("CursorHorizontal") < 0.0f)
				//	xSumPosition = -xSumPosition;
			}
				
			else
				//xSumPosition = xDiffPlayer;
				xSumPosition = 0.0f;

			if (inputKeeper.getAxis("CursorVertical") > 0.2f || inputKeeper.getAxis("CursorVertical") < -0.2f)
			{
				ySumPosition = -yControllerMultiplier * Time.deltaTime * inputKeeper.getAxis("CursorVertical")* inputKeeper.getAxis("CursorVertical")* inputKeeper.getAxis("CursorVertical");
				//if (Input.GetAxis("CursorVertical") < 0.0f)
				//	ySumPosition = -ySumPosition;
			}
				
			else
				//ySumPosition = yDiffPlayer;
				ySumPosition = 0.0f;


			//standardPosition = new Vector3(standardPosition.x + xSumPosition, standardPosition.y + ySumPosition, zPositionEnvironment);
			standardPosition = new Vector3(player.transform.position.x + xSumPosition + xDiffPlayer, player.transform.position.y + ySumPosition + yDiffPlayer, zPositionEnvironment);

			standardPosition = limitCursorScreenPosition(standardPosition);

			cursorPosition = Vector3.Lerp (cursorPosition, standardPosition, Time.deltaTime*10.0f);
			//Debug.Log (Input.GetAxis("CursorHorizontal"));
			//Debug.Log (standardPosition);
			//xDiffPlayer = player.transform.position.x - standardPosition.x;
			//yDiffPlayer = player.transform.position.y - standardPosition.y;
			xDiffPlayer = standardPosition.x - player.transform.position.x;
			yDiffPlayer = standardPosition.y - player.transform.position.y;
		}

		//limitCursorPosition ();
	}

	//per ora inutilizzata
	void limitCursorPosition()
	{
		//trovo la retta passante per i due punti
		//l'equazione y-y1 = m (x-x1) la trasformo in y = mx + a con a = y1 - mx1
		Vector3 playerPosition = player.transform.position;
		float m = (cursorPosition.y - playerPosition.y) / (cursorPosition.x - playerPosition.x);
		float a = playerPosition.y - m * playerPosition.x;

		//setto i limiti
		if (BottomLimit) {
			if (BottomLimit.position.y > cursorPosition.y)
				cursorPosition = new Vector3(findX(m,a,BottomLimit.position.y), BottomLimit.position.y, cursorPosition.z);
		}
		if (RightLimit) {
			if (RightLimit.position.x < cursorPosition.x)
				cursorPosition = new Vector3(RightLimit.position.x, findY(m,a,RightLimit.position.x), cursorPosition.z);
		}
		if (UpperLimit) {
			if (UpperLimit.position.y < cursorPosition.y)
				cursorPosition = new Vector3(findX(m,a,UpperLimit.position.y), UpperLimit.position.y, cursorPosition.z);
		}
		if (LeftLimit) {
			if (LeftLimit.position.x > cursorPosition.x)
				cursorPosition = new Vector3(LeftLimit.position.x, findY(m,a,LeftLimit.position.x), cursorPosition.z);
		}
	}

	Vector3 limitCursorScreenPosition(Vector3 actualPosition)
	{
		if (actualPosition.x < (Camera.main.gameObject.transform.position.x-xDistFromBeginning))
			actualPosition.x = Camera.main.gameObject.transform.position.x-xDistFromBeginning;
		if (actualPosition.x > (Camera.main.gameObject.transform.position.x+xDistFromBeginning))
			actualPosition.x = Camera.main.gameObject.transform.position.x+xDistFromBeginning;
		if (actualPosition.y < (Camera.main.gameObject.transform.position.y-yDistFromBeginning))
			actualPosition.y = Camera.main.gameObject.transform.position.y-yDistFromBeginning;
		if (actualPosition.y > (Camera.main.gameObject.transform.position.y+yDistFromBeginning))
			actualPosition.y = Camera.main.gameObject.transform.position.y+yDistFromBeginning;
		return actualPosition;
	}

	float findX(float m, float a, float y){
		return (y - a) / m;
	}

	float findY(float m, float a, float x){
		return m * x + a;
	}

	void changeUseController()
	{
		if (Input.GetKey (KeyCode.LeftShift) && Input.GetKeyDown (KeyCode.C)) {
			//if (Input.GetKeyDown(KeyCode.C))
			useController = !useController;
			if (controllerAutomaticVar)
			{
				movingIfStanding = !useController;
			}
		}
	}

	void verifyCursorMoving()
	{
		/*
		 * nel caso di utilizzo del mouse faccio un controllo sulla posizione rispetto alla grandezza dello schermo.
		 * nel caso di utilizzo del controller, è più opportuno fare un controllo sul valore dell'asse.
		 */

		bool actualCursorMoving = false;

		if (!useController){
			if (firstCursorPosition) {
				oldCursorPosition = inputKeeper.getMousePosition();
				firstCursorPosition = false;
			}
			else {
				float ratioTemp;
				ratioTemp = ScreenRatioMovement;

				Vector3 actualCursorPosition = inputKeeper.getMousePosition();

				if ((Mathf.Abs(oldCursorPosition.x - actualCursorPosition.x) > ratioTemp*xDistFromBeginning)||
				    (Mathf.Abs(oldCursorPosition.y - actualCursorPosition.y) > ratioTemp*yDistFromBeginning))
					actualCursorMoving = true;
				else
					actualCursorMoving = false;
				//	cursorIsMoving = true;
				//else
				//	cursorIsMoving = false;
				
				oldCursorPosition = inputKeeper.getMousePosition();
			}
		}
		else{
			//Debug.Log ("Controller");

			if (timesControllerMoving > maxTimesControllerMoving)
				actualCursorMoving = true;
			else
				actualCursorMoving = false;
			//	cursorIsMoving = true;
			//else
			//	cursorIsMoving = false;

			if ((Time.time - lastTimeControllerMovingCheck) > timeBetweenControllerMoving)
			{
				if (inputKeeper.getAxis("CursorHorizontal") > ScreenControllerRatioMovement || inputKeeper.getAxis("CursorHorizontal") < -ScreenControllerRatioMovement ||
				    inputKeeper.getAxis("CursorVertical") > ScreenControllerRatioMovement || inputKeeper.getAxis("CursorVertical") < -ScreenControllerRatioMovement)
				{
					lastTimeControllerMovingCheck = Time.time;
					timesControllerMoving++;
				}
				else
				{
					timesControllerMoving = 0;
				}
			}

		}

		if (!movingIfStanding)
		{
			//per quanto tempo rimane moving anche se non muovo più
			if (useController && !movingAfterController)
			{
				cursorIsMoving = actualCursorMoving;
			}else{
				if (actualCursorMoving)
				{
					cursorIsMoving = true;
					lastTimeMoving = Time.time;
				}
				else if (Time.time - lastTimeMoving > timeAfterMoving )
				{
					cursorIsMoving = false;
				}
			}
		}
		else
		{

			if (actualCursorMoving)
			{
				cursorIsMoving = true;
				lastTimeMoving = Time.time;
			}
			else
			{
				//Debug.Log ("non si muove");
				if ((playerMovements.isRunning() && playerMovements.onGround) || !playerMovements.onGround)
				{
					if (((useController && movingAfterController) || !useController))
					
						cursorIsMoving = false;
				}
				//il player è a terra e fermo
				//else
				//{
				//	cursorIsMoving = true;
				//}
			}
		}
		//per quanto tempo rimane moving anche se non muovo più
		/*
		if (useController && !movingAfterController)
		{
			if (movingIfStanding)
			{
				if (actualCursorMoving)
				{
					cursorIsMoving = true;
				}else
				{
					if(playerMovements.isRunning())
						cursorIsMoving = false;
				}
			}
			else
				cursorIsMoving = actualCursorMoving;
		}else{
			if (actualCursorMoving)
			{
				cursorIsMoving = true;
				lastTimeMoving = Time.time;
			}
			else if (movingIfStanding)
			{
				if(((playerMovements.isRunning() && playerMovements.onGround) || !playerMovements.onGround) && (Time.time - lastTimeMoving > timeAfterMoving))
					cursorIsMoving = false;
			}
			else if (Time.time - lastTimeMoving > timeAfterMoving )
			{
				cursorIsMoving = false;
			}
		}
		*/
			


		//cursorIsMoving = false;
		/*
		if (firstCursorPosition) {
			oldCursorPosition = getCursorScreenPosition();
			firstCursorPosition = false;
		}
		else {
			float ratioTemp;
			if (useController)
				ratioTemp = ScreenControllerRatioMovement;
			else
				ratioTemp = ScreenRatioMovement;

			if ((Mathf.Abs(oldCursorPosition.x - getCursorScreenPosition().x) > ratioTemp*xDistFromBeginning)||
			    (Mathf.Abs(oldCursorPosition.y - getCursorScreenPosition().y) > ratioTemp*yDistFromBeginning))
				cursorIsMoving = true;
			else
				cursorIsMoving = false;

			oldCursorPosition = getCursorScreenPosition();
		}
		*/
	}

	public bool isCursorMoving()
	{
		return cursorIsMoving;
	}

	public Vector3 getCursorWorldPosition()
	{
		return cursorPosition;
	}

	public Vector3 getCursorScreenPosition()
	{
		return Camera.main.WorldToScreenPoint (cursorPosition);
	}

	public Transform getBottomLimit()
	{
		if (BottomLimit) 
			return BottomLimit;
		else
			return null;
	}

	public Transform getUpperLimit()
	{
		if (UpperLimit) 
			return UpperLimit;
		else
			return null;
	}

	public Transform getLeftLimit()
	{
		if (LeftLimit) 
			return LeftLimit;
		else
			return null;
	}

	public Transform getRightLimit()
	{
		if (RightLimit) 
			return RightLimit;
		else
			return null;
	}
}

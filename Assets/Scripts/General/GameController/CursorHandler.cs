using UnityEngine;
using System.Collections;

public class CursorHandler : MonoBehaviour {

	public bool useController = false;
	public float xControllerMultiplier = 15.0f;
	public float yControllerMultiplier = 15.0f;

	Vector3 cursorPosition;
	public float zPositionEnvironment = 0.0f;
	GameObject player;
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
	public float ScreenRatioMovement = 0.01f;

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Confined;
		player = GameObject.FindGameObjectWithTag ("Player");

		standardPosition = new Vector3 (Camera.main.gameObject.transform.position.x, Camera.main.gameObject.transform.position.z, zPositionEnvironment);

		//xDiffPlayer = standardPosition.x - player.transform.position.x;
		//yDiffPlayer = standardPosition.y - player.transform.position.y;
		xDiffPlayer = 1.0f;
		yDiffPlayer = 1.0f;

		cameraCenter = new Vector3 (Camera.main.gameObject.transform.position.x, Camera.main.gameObject.transform.position.y, player.transform.position.z);
		beginCamera = Camera.main.ScreenToWorldPoint (new Vector3 (0.0f, 0.0f, player.transform.position.z));
		xDistFromBeginning = Mathf.Abs (cameraCenter.x - beginCamera.x);
		yDistFromBeginning = Mathf.Abs (cameraCenter.y - beginCamera.y);
	}
	
	// Update is called once per frame
	void Update () {
		setCursorPosition ();
		//Debug.Log (getCursorWorldPosition());
		verifyCursorMoving ();
	}

	void setCursorPosition()
	{
		//Debug.Log (xDiffPlayer);


		if (!useController) {
			Vector3 actualMousePosition = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0.0f));
			cursorPosition = new Vector3 (actualMousePosition.x, actualMousePosition.y, zPositionEnvironment);
		} else {
			float xSumPosition, ySumPosition;

			if (Input.GetAxis("CursorHorizontal") > 0.2f || Input.GetAxis("CursorHorizontal") < -0.2f)
				xSumPosition = xControllerMultiplier * Time.deltaTime * Input.GetAxis("CursorHorizontal");
			else
				//xSumPosition = xDiffPlayer;
				xSumPosition = 0.0f;

			if (Input.GetAxis("CursorVertical") > 0.2f || Input.GetAxis("CursorVertical") < -0.2f)
				ySumPosition = yControllerMultiplier * Time.deltaTime * Input.GetAxis("CursorVertical");
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

	void verifyCursorMoving()
	{
		//cursorIsMoving = false;
		if (firstCursorPosition) {
			oldCursorPosition = getCursorScreenPosition();
			firstCursorPosition = false;
		}
		else {
			if ((Mathf.Abs(oldCursorPosition.x - getCursorScreenPosition().x) > ScreenRatioMovement*xDistFromBeginning)||
			    (Mathf.Abs(oldCursorPosition.y - getCursorScreenPosition().y) > ScreenRatioMovement*yDistFromBeginning))
				cursorIsMoving = true;
			else
				cursorIsMoving = false;

			oldCursorPosition = getCursorScreenPosition();
		}
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

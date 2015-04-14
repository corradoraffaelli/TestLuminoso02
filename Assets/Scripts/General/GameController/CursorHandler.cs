using UnityEngine;
using System.Collections;

public class CursorHandler : MonoBehaviour {

	Vector3 cursorPosition;
	public float zPositionEnvironment = 0.0f;
	GameObject player;
	Vector3 playerPosition;

	public Transform BottomLimit;
	public Transform LeftLimit;
	public Transform RightLimit;
	public Transform UpperLimit;

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Confined;
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		setCursorPosition ();
		//Debug.Log (getCursorScreenPosition());
	}

	void setCursorPosition()
	{
		Vector3 actualMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, 0.0f));
		cursorPosition = new Vector3 (actualMousePosition.x, actualMousePosition.y, zPositionEnvironment);

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

	float findX(float m, float a, float y){
		return (y - a) / m;
	}

	float findY(float m, float a, float x){
		return m * x + a;
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

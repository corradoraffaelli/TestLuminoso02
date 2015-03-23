using UnityEngine;
using System.Collections;

public class MoveCircle : MonoBehaviour {

	float distance_to_screen;
	Vector3 pos_move;
	public GameObject raggio;
	public GameObject cameraPoint;
	public GameObject camera;
	Vector3 cameraPointPos;

	public float RotationSpeed = 100;
	private Quaternion _lookRotation;
	private Vector3 _direction;

	float xSize;
	private Bounds boundsObj;

	void Start()
	{
		boundsObj = raggio.GetComponent<SpriteRenderer>().bounds;
		xSize = boundsObj.size.x;
	}

	void Update()
	{
		/*
		cameraPointPos = cameraPoint.transform.position;
		raggio.transform.position = cameraPointPos;

		_direction = (transform.position - cameraPointPos).normalized;

		//setto la direzione del raggio
		raggio.transform.right = _direction;

		//setto la direzione della camera
		if (raggio.transform.localEulerAngles.z > 180)
			camera.transform.localEulerAngles = new Vector3 (raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.x, (raggio.transform.localEulerAngles.z-360) / 2);
		else
			camera.transform.localEulerAngles = new Vector3 (raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.x, raggio.transform.localEulerAngles.z / 2);


		float distance = Vector3.Distance (transform.position, cameraPointPos);

		raggio.transform.localScale = new Vector3(distance / xSize,1,1);
		*/
	}
	
	void OnMouseDrag()
	{
		//move the circle under the mouse
		/*
		distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
		pos_move = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen ));
		transform.position = new Vector3( pos_move.x, pos_move.y, pos_move.z );
		*/
	}
}

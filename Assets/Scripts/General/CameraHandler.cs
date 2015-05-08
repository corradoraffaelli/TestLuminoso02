using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour {

	Vector3 cameraCenter;
	Vector3 beginCamera;
	float xDistFromBeginning;
	float yDistFromBeginning;

	float zPositionEnvironment = 0.0f;

	// Use this for initialization
	void Start () {
		setVariables ();
	}
	
	// Update is called once per frame
	void Update () {
		setVariables ();
	}

	void setVariables()
	{
		cameraCenter = new Vector3 (Camera.main.gameObject.transform.position.x, Camera.main.gameObject.transform.position.y, zPositionEnvironment);
		beginCamera = Camera.main.ScreenToWorldPoint (new Vector3 (0.0f, 0.0f, zPositionEnvironment));
		xDistFromBeginning = Mathf.Abs (cameraCenter.x - beginCamera.x);
		yDistFromBeginning = Mathf.Abs (cameraCenter.y - beginCamera.y);
	}

	public float getXDistFromBeginning()
	{
		return xDistFromBeginning;
	}

	public float getYDistFromBeginning()
	{
		return yDistFromBeginning;
	}

	public Vector3 getCameraPositionZEnvironment()
	{
		return cameraCenter;
	}

	public Vector3 getCameraPosition()
	{
		return new Vector3(cameraCenter.x, cameraCenter.y, Camera.main.gameObject.transform.position.z);
	}
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Specifica dell'intro. Si occupa di gestire i movimenti della camera.
/// </summary>

// Corrado
public class CameraControllerIntro : MonoBehaviour {

	public Transform finalDestination;


	public float lateralView = 14.3f;
	float finalCameraSize;
	float beginCameraSize;
	float diffSize;

	CameraMovements cameraMovements;

	bool needToChange = false;
	float referenceNum = 0.0f;
	public float changingSpeed = 1.0f;

	Vector3 cameraBeginPos;
	float diffX;
	float diffY;

	void Start () {
		cameraMovements = Camera.main.gameObject.GetComponent<CameraMovements>();

		beginCameraSize = Camera.main.orthographicSize;

		float aspectRatio = Camera.main.aspect;
		finalCameraSize = lateralView / aspectRatio;

		diffSize = finalCameraSize - beginCameraSize;
	}

	void Update () {
		if (needToChange)
		{
			referenceNum = Mathf.MoveTowards(referenceNum, 1.0f, Time.deltaTime * changingSpeed / 10.0f);
			changeCameraVariables(referenceNum);
		}
	}

	void OnTriggerEnter2D()
	{
		if (cameraMovements != null)
			cameraMovements.enabled = false;

		needToChange = true;

		cameraBeginPos = Camera.main.transform.position;

		if (finalDestination != null)
		{
			diffX = finalDestination.transform.position.x - cameraBeginPos.x;
			diffY = finalDestination.transform.position.y - cameraBeginPos.y;
		}

		//positionDifference = Vector3.
	}

	void changeCameraVariables(float inputReference)
	{
		float actualChangingSize = diffSize * inputReference;
		Camera.main.orthographicSize = beginCameraSize + actualChangingSize;

		float actualChangingX = diffX * inputReference;
		float actualChangingY = diffY * inputReference;
		//Debug.Log (actualChangingY);

		Vector3 newPosition = new Vector3(cameraBeginPos.x + actualChangingX, cameraBeginPos.y + actualChangingY, cameraBeginPos.z);
		Camera.main.transform.position = newPosition;

	}
}

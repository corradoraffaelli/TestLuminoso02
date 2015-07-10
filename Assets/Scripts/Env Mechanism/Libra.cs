using UnityEngine;
using System.Collections;

public class Libra : MonoBehaviour {

	[System.Serializable]
	public class LibraObjects
	{
		public GameObject leftArm;
		public GameObject rightArm;
		public GameObject leftCounter;
		public GameObject rightCounter;
		public GameObject wheel;
	}

	[SerializeField]
	LibraObjects libraObjects;

	LibraCounter leftCounterScript;
	LibraCounter rightCounterScript;

	Vector3 leftStandardPosition;
	Vector3 rightStandardPosition;
	Vector3 wheelStandardRotation;

	public float deltaPosition = 0.5f;
	public float deltaRotation = 10.0f;
	public float speed = 0.4f;

	void Start () {
		if (libraObjects.leftCounter != null)
			leftCounterScript = libraObjects.leftCounter.GetComponent<LibraCounter>();
		if (libraObjects.rightCounter != null)
			rightCounterScript = libraObjects.rightCounter.GetComponent<LibraCounter>();

		takeStandardPositions();
	}

	void Update () {
		changePositions();
	}

	void takeStandardPositions()
	{
		if (libraObjects.leftArm != null)
			leftStandardPosition = libraObjects.leftArm.transform.position;
		if (libraObjects.rightArm != null)
			rightStandardPosition = libraObjects.rightArm.transform.position;
		if (libraObjects.wheel != null)
			wheelStandardRotation = libraObjects.wheel.transform.localEulerAngles;
	}

	void changePositions()
	{
		if (leftCounterScript != null && rightCounterScript!= null)
		{
			int diffBetween = leftCounterScript.elementsNumber - rightCounterScript.elementsNumber;

			float leftDestY = leftStandardPosition.y - diffBetween * deltaPosition;
			float rightDestY = rightStandardPosition.y + diffBetween * deltaPosition;

			Vector3 leftDestPosition = new Vector3(leftStandardPosition.x, leftDestY, leftStandardPosition.z);
			//Debug.Log (leftDestPosition);
			Vector3 rightDestPosition = new Vector3(rightStandardPosition.x, rightDestY, rightStandardPosition.z);

			if (libraObjects.leftArm != null)
				libraObjects.leftArm.transform.position = Vector3.MoveTowards(libraObjects.leftArm.transform.position, leftDestPosition, Time.deltaTime * speed);
			if (libraObjects.rightArm != null)
				libraObjects.rightArm.transform.position = Vector3.MoveTowards(libraObjects.rightArm.transform.position, rightDestPosition, Time.deltaTime * speed);


			float wheelDestRot = wheelStandardRotation.z + diffBetween * deltaRotation;
			Debug.Log (wheelDestRot);
			Vector3 wheelDestAngles = new Vector3(wheelStandardRotation.x, wheelStandardRotation.y, wheelDestRot);
			if (libraObjects.wheel != null)
				libraObjects.wheel.transform.localEulerAngles = Vector3.MoveTowards(libraObjects.wheel.transform.localEulerAngles, wheelDestAngles, Time.deltaTime * speed * deltaRotation / deltaPosition);
		}

	}
}

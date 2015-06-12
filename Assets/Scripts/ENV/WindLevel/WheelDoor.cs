using UnityEngine;
using System.Collections;

public class WheelDoor : MonoBehaviour {

	public GameObject wheel;
	public GameObject chainPiece;

	public GameObject activator;
	AreaEffector2D actualAreaEffector;

	public bool windFromLeft = true;

	public bool active = false;
	bool oldActive = false;

	public float maxRotationSpeed = 3.0f;
	public float lerpSpeed = 1.0f;
	float actualSpeed = 0.0f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if ((activator == null) || (activator != null && activator.activeInHierarchy == false) || (actualAreaEffector != null && !actualAreaEffector.enabled))
			active = false;

		rotateWheel();
		doorBehave();
	}
	
	void OnTriggerStay2D(Collider2D other)
	{
		actualAreaEffector = other.gameObject.GetComponent<AreaEffector2D>();
		if (actualAreaEffector != null && active == false)
		{
			if (windFromLeft)
			{
				if ((actualAreaEffector.forceAngle < 90 && actualAreaEffector.forceAngle > -90) || (actualAreaEffector.forceAngle > 270) || (actualAreaEffector.forceAngle < -270))
				{
					//Debug.Log ("girandola attivata da sinistra");
					active = true;
					activator = other.gameObject;
				}
			}else{
				if ((actualAreaEffector.forceAngle < -90 && actualAreaEffector.forceAngle > -270) || (actualAreaEffector.forceAngle > 90 && actualAreaEffector.forceAngle < 270))
				{
					//Debug.Log ("girandola attivata da destra");
					active = true;
					activator = other.gameObject;
				}
			}

		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		actualAreaEffector = other.gameObject.GetComponent<AreaEffector2D>();
		if (actualAreaEffector != null)
		{
			active = false;
		}
	}

	void rotateWheel()
	{
		if (wheel != null)
		{
			if (active)
			{
				Vector3 angles = wheel.transform.localEulerAngles;
				actualSpeed = Mathf.Lerp(actualSpeed, maxRotationSpeed, Time.deltaTime*lerpSpeed);
				wheel.transform.localEulerAngles = new Vector3 (angles.x, angles.y, angles.z + actualSpeed);
				//float newAngle = Mathf.Lerp(angles.z, 
				//wheel.transform.localEulerAngles =
			}else{
				Vector3 angles = wheel.transform.localEulerAngles;
				actualSpeed = Mathf.Lerp(actualSpeed, 0.0f, Time.deltaTime*lerpSpeed);
				wheel.transform.localEulerAngles = new Vector3 (angles.x, angles.y, angles.z + actualSpeed);
			}
		}
	}

	void doorBehave()
	{
		if (chainPiece != null && active != oldActive)
		{
			if (active)
				chainPiece.SendMessage("buttonPushed", true);
			else
				chainPiece.SendMessage("buttonPushed", false);

			oldActive = active;
		}

	}
}

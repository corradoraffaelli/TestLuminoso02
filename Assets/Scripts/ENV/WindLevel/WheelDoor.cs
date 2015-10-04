using UnityEngine;
using System.Collections;

/// <summary>
/// Specifica del livello del vento. Gestisce il comportamento delle girandole che aprono le porte.
/// </summary>

// Corrado
public class WheelDoor : MonoBehaviour {

	public GameObject wheel;
	public GameObject chainPiece;

	//public GameObject activator;
	//public AreaEffector2D actualAreaEffector;

	public bool windFromLeft = true;

	public bool active = false;
	bool oldActive = false;

	public float maxRotationSpeed = 3.0f;
	public float lerpSpeed = 1.0f;
	float actualSpeed = 0.0f;
	public float stopSoundSpeed = 0.6f;
	bool soundActive = false;
	bool wasSoundActive = false;

	AudioHandler audioHandler;
	public string[] soundNames;


	//public bool active = false;
	bool savedGO = false;
	GameObject windGO;
	AreaEffector2D areaEffector;
	bool colliding = false;
	bool needToReset = true;

	bool actualDirectionLeft = true;

	void Start () {
		audioHandler = GetComponent<AudioHandler>();
	}

	void Update () {
		
		if ((areaEffector == null && windGO != null) || (windGO != null && needToReset))
			areaEffector = windGO.GetComponent<AreaEffector2D>();
		
		if (areaEffector != null)
		{
			if(areaEffector.enabled && colliding)
			{
				if ((areaEffector.forceAngle < 90 && areaEffector.forceAngle > -90) || (areaEffector.forceAngle > 270) || (areaEffector.forceAngle < -270))
					actualDirectionLeft = true;
				else
					actualDirectionLeft = false;
				
				active = true;
				
			}else{
				active = false;
			}
			
		}
		
		if (windGO != null && !windGO.activeInHierarchy)
		{
			active = false;
		}
		
		if (windGO == null || !windGO.activeInHierarchy)
		{
			savedGO = false;
			needToReset = true;
		}



		considerDirectionToActive();

		rotateWheel();
		doorBehave();
		
		soundHandler();
		
	}
	
	void OnTriggerStay2D(Collider2D other)
	{
		if (!savedGO) 
		{
			AreaEffector2D areaEffector = other.gameObject.GetComponent<AreaEffector2D>();
			if (areaEffector != null)
			{
				windGO = other.gameObject;
				savedGO = true;
				colliding = true;
			}
			
		}
		
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		//Debug.Log ("Esco01");
		if (savedGO) 
		{
			if (other.gameObject == windGO)
			{
				//Debug.Log ("Esco");
				colliding = false;
				active = false;
			}
			
		}
		
	}

	void considerDirectionToActive()
	{
		if (active)
		{
			active = ((windFromLeft && actualDirectionLeft)||(!windFromLeft && !actualDirectionLeft));
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
				wheel.transform.localEulerAngles = new Vector3 (angles.x, angles.y, angles.z + actualSpeed*Time.deltaTime);
				//float newAngle = Mathf.Lerp(angles.z, 
				//wheel.transform.localEulerAngles =
			}else{
				Vector3 angles = wheel.transform.localEulerAngles;
				actualSpeed = Mathf.Lerp(actualSpeed, 0.0f, Time.deltaTime*lerpSpeed);
				wheel.transform.localEulerAngles = new Vector3 (angles.x, angles.y, angles.z + actualSpeed*Time.deltaTime);
			}
		}
	}
	
	void doorBehave()
	{
		if (chainPiece != null && active != oldActive)
		{
			if (active) {
				chainPiece.SendMessage("buttonPushed", true);
			}
			else {
				chainPiece.SendMessage("buttonPushed", false);
			}
			
			oldActive = active;
		}
		
	}
	
	void soundHandler()
	{
		if (audioHandler != null)
		{
			soundActive = (actualSpeed > Mathf.Abs(stopSoundSpeed));

			if (soundActive != wasSoundActive)
			{
				if (soundActive)
				{
					for (int i = 0; i < soundNames.Length; i++)
					{
						if (soundNames[i] != null)
						{
							audioHandler.playClipByName(soundNames[i]);
						}
							
					}
				}
				else
				{
					for (int i = 0; i < soundNames.Length; i++)
					{
						if (soundNames[i] != null)
							audioHandler.stopClipByName(soundNames[i]);
					}
				}
				
			}
			
			wasSoundActive = soundActive;
		}
		
	}


	/*
	// Use this for initialization
	void Start () {
		audioHandler = GetComponent<AudioHandler>();
	}
	
	// Update is called once per frame
	void Update () {
		if ((activator == null) || (activator != null && activator.activeInHierarchy == false) || (actualAreaEffector != null && !actualAreaEffector.enabled))
		{
			active = false;
			actualAreaEffector = null;
		}
			

		rotateWheel();
		doorBehave();

		soundHandler();
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
		AreaEffector2D newEffector = other.gameObject.GetComponent<AreaEffector2D>();
		if (newEffector != null)
		{
			active = false;
		}

		//actualAreaEffector = other.gameObject.GetComponent<AreaEffector2D>();
		//if (actualAreaEffector != null)
		//{
		//	active = false;
		//}

	}

	void rotateWheel()
	{
		if (wheel != null)
		{
			if (active)
			{
				Vector3 angles = wheel.transform.localEulerAngles;
				actualSpeed = Mathf.Lerp(actualSpeed, maxRotationSpeed, Time.deltaTime*lerpSpeed);
				wheel.transform.localEulerAngles = new Vector3 (angles.x, angles.y, angles.z + actualSpeed*Time.deltaTime);
				//float newAngle = Mathf.Lerp(angles.z, 
				//wheel.transform.localEulerAngles =
			}else{
				Vector3 angles = wheel.transform.localEulerAngles;
				actualSpeed = Mathf.Lerp(actualSpeed, 0.0f, Time.deltaTime*lerpSpeed);
				wheel.transform.localEulerAngles = new Vector3 (angles.x, angles.y, angles.z + actualSpeed*Time.deltaTime);
			}
		}
	}

	void doorBehave()
	{
		if (chainPiece != null && active != oldActive)
		{
			if (active) {
				chainPiece.SendMessage("buttonPushed", true);
			}
			else {
				chainPiece.SendMessage("buttonPushed", false);
			}

			oldActive = active;
		}

	}

	void soundHandler()
	{
		if (audioHandler != null)
		{
			soundActive = (actualSpeed > Mathf.Abs(stopSoundSpeed));
			if (soundActive != wasSoundActive)
			{
				if (soundActive)
				{
					for (int i = 0; i < soundNames.Length; i++)
					{
						if (soundNames[i] != null)
							audioHandler.playClipByName(soundNames[i]);
					}
				}
				else
				{
					for (int i = 0; i < soundNames.Length; i++)
					{
						if (soundNames[i] != null)
							audioHandler.stopClipByName(soundNames[i]);
					}
				}
					
			}

			wasSoundActive = soundActive;
		}

	}
	*/
}

using UnityEngine;
using System.Collections;

public class FakeLanternBehaviour : MonoBehaviour {

	public bool disableObject = false;
	public bool intermittance = false;
	public bool disabledByEnemy = true;

	public enum fakeLanternState
	{
		None,
		On,
		Off,
	};

	public fakeLanternState actualState = fakeLanternState.On;
	fakeLanternState previousState = fakeLanternState.On;

	public bool continuousFollow = false;
	
	bool canBeReenabled = true;

	float startTime = 0.0f;
	[Range(0.1f, 10.0f)]
	public float turningOnTime = 5.0f;

	float startOffTime = 0.0f;

	public Glass[] glassesToEnable;
	public Glass[] glassesToDisable;
	public bool enableLantern = true;

	GameObject magicLanternLogicOBJ;
	AudioHandler audioHandler;
	GlassesManager glassesManager;

	GameObject lantern;
	GameObject raggio_cerchio;
	GameObject raggio;
	GameObject cameraPoint;
	GameObject proiettore;
	GameObject cerchio_collider;
	GameObject sostegno;

	float xSize = 0.0f;

	// Use this for initialization
	void Start () {
		takeObjectsFromScene ();

		magicLanternLogicOBJ = GameObject.FindGameObjectWithTag ("MagicLanternLogic");

		if (magicLanternLogicOBJ != null)
			glassesManager = magicLanternLogicOBJ.GetComponent<GlassesManager> ();

		audioHandler = GetComponent<AudioHandler> ();

		if (raggio != null) {
			xSize = raggio.GetComponent<SpriteRenderer> ().bounds.size.x;
		}

		setLanternPosition ();
	}
	
	// Update is called once per frame
	void Update () {
		setStates ();

		if (actualState == fakeLanternState.Off) {
			if (actualState != previousState){
				turnOnLantern(false);
				audioHandler.playClipByName("Clic");
			}

		}

		if (actualState == fakeLanternState.On) {
			if (actualState != previousState){
				turnOnLantern(true);
				audioHandler.playClipByName("Clic");
			}
			
		}

		previousState = actualState;

		if (continuousFollow && actualState == fakeLanternState.On)
			setLanternPosition ();
	}

	void setStates()
	{
		if (actualState == fakeLanternState.Off) {
			if (canBeReenabled && Mathf.Abs(Time.time - startTime) > turningOnTime)
			{
				actualState = fakeLanternState.On;
			}

			startOffTime = Time.time;
		}

		if (actualState == fakeLanternState.On) {

			if (intermittance && Mathf.Abs(Time.time - startOffTime) > turningOnTime)
			{
				actualState = fakeLanternState.Off;
			}

			startTime = Time.time;
		}
	}

	public void canLanternBeEnabled(bool canOrNot)
	{
		canBeReenabled = canOrNot;
	}

	void setLanternPosition()
	{
		Vector3 circlePosition;

		//metto la lanterna che guarda verso detra
		lantern.transform.localScale = new Vector3(Mathf.Abs (lantern.transform.localScale.x), lantern.transform.localScale.y, 1.0f);

		//mi salvo la posizione del raggio e, se si trova a sinistra della lanterna, lo metto a destra
		float tempCircleLocalPosition = raggio_cerchio.transform.localPosition.x;
		raggio_cerchio.transform.localPosition = new Vector3 (Mathf.Abs (tempCircleLocalPosition), raggio_cerchio.transform.localPosition.y, raggio_cerchio.transform.localPosition.z);
		                                                     
		circlePosition = raggio_cerchio.transform.position;

		//prendo la direzione dal sostegno al raggio (che è sempre a destra)
		Bounds boundsSostegno = sostegno.GetComponent<SpriteRenderer> ().bounds;
		Vector3 sostegnoUpperPos = new Vector3 (boundsSostegno.center.x, boundsSostegno.max.y, 0.0f);
		Vector3 otherDirection = (circlePosition - sostegnoUpperPos).normalized;

		//setto la direzione del proiettore
		proiettore.transform.right = new Vector3 (otherDirection.x, otherDirection.y / 2, otherDirection.z);

		//rimetto la posizione salvata al cerchio
		raggio_cerchio.transform.localPosition = new Vector3 (tempCircleLocalPosition, raggio_cerchio.transform.localPosition.y, raggio_cerchio.transform.localPosition.z);

		//se il cerchio si trova a sinistra della lanterna, specchio la lanterna
		if (raggio_cerchio.transform.position.x < sostegno.transform.position.x) {
			lantern.transform.localScale = new Vector3(-lantern.transform.localScale.x, lantern.transform.localScale.y, 1.0f);
		}

		//prendo la posizione del punto frontale della sprite della camera, e ci piazzo l'origine del raggio
		Vector3 cameraPointPos = cameraPoint.transform.position;
		raggio.transform.position = cameraPointPos;
		
		//prendo la direzione tra l'inizio del raggio e la posizione del cerchio
		Vector3 _direction = (raggio_cerchio.transform.position - cameraPointPos).normalized;
		
		//cambio la scala del raggio in base alla distanza tra camera e cerchio
		float distance = Vector3.Distance (raggio_cerchio.transform.position, cameraPointPos);
		raggio.transform.localScale = new Vector3(distance / xSize / 2,raggio_cerchio.transform.localScale.y,1);

		//setto la direzione del raggio
		raggio.transform.right = _direction;
	}

	void takeObjectsFromScene()
	{
		foreach (Transform child in transform) {
			if (child.name == "Lantern") {
				lantern = child.gameObject;
				foreach (Transform subChild in lantern.transform) {
					if (subChild.name == "Proiettore") {
						proiettore = subChild.gameObject;
						foreach (Transform subSubChild in subChild.transform) {
							if (subSubChild.name == "Proiettore_punta") {
								cameraPoint = subSubChild.gameObject;
							}
						}
					} else if (subChild.name == "Sostegno") {
						sostegno = subChild.gameObject;
					}
				}
			}else if (child.name == "raggio") {
				raggio = child.gameObject;
			} else if (child.name == "raggio_cerchio") {
				raggio_cerchio = child.gameObject;
			} 
		}
	}

	void turnOnLantern(bool turnOn)
	{
		raggio.SetActive (turnOn);
		raggio_cerchio.SetActive (turnOn);
	}

	public void c_turnOnLantern(bool turnOn) {

		turnOnLantern (turnOn);

	}

	public void changeLanternState(fakeLanternState state)
	{
		actualState = state;
	}

	public void InteractingMethod()
	{
		Debug.Log ("Interagito con fake lantern");
		changeLanternState (fakeLanternState.Off);
		canLanternBeEnabled (false);

		if (audioHandler != null)
			audioHandler.playClipByName ("Leva");

		if (enableLantern && magicLanternLogicOBJ != null)
			magicLanternLogicOBJ.GetComponent<MagicLantern> ().setActivable (true);

		for (int i = 0; i<glassesToEnable.Length; i++) {
			if (glassesToEnable[i] !=null)
			{

				if (glassesManager != null){
					glassesToEnable[i].canBeEnabled = true;
					glassesManager.enableGlassByName(glassesToEnable[i].glassType, true);
				}


			}
		}

		for (int i = 0; i<glassesToDisable.Length; i++) {
			if (glassesToDisable[i] !=null)
			{
				
				if (glassesManager != null){
					glassesManager.enableGlassByName(glassesToDisable[i].glassType, false);
					glassesToDisable[i].canBeEnabled = false;
				}
			}
		}

		if (disableObject) {
			foreach (Transform child in transform) {
				child.gameObject.SetActive(false);
			}
		}
	}

}

using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce il comportamento delle lanterne poste nelle scene, con cui è possibile interagire per raccoglierne i vetrini.
/// </summary>

// Corrado
public class FakeLanternBehaviour : MonoBehaviour {

	public bool unlockContent = false;
	public bool unlockGlass = true;
	int sectionIndex;
	int contentIndex;

	public bool disableObject = false;
	public bool intermittance = false;
	public bool disabledByEnemy = true;
	public bool emitParticles = false;
	public GameObject particlesToEnable;
	//public LanternFanfareController fanfare;

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
	[Range(0.1f, 10.0f)]
	public float turningOffTime = 5.0f;
	[Range(0.1f, 10.0f)]
	public float timeAfterFlashing = 2.0f;
	[Range(0.1f, 10.0f)]
	public float flashingTime = 0.15f;
	[Range(0.0f, 1.0f)]
	public float flashingAlpha = 0.25f;

	bool flashingVisible = false;
	bool actualFlashingVisible = false;

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

	SpriteRenderer rendererRay;
	SpriteRenderer rendererCircle;

	float xSize = 0.0f;

	bool firstClic = true;
	float lastTimeClic = 0.0f;
	[Range(0.1f, 2.0f)]
	public float maxClicTime = 0.6f;
	[Range(0.01f, 0.5f)]
	public float minClicTime = 0.05f;

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
		setContentVariables();
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
				audioHandler.playClipByName("ApparizioneProiezione");
			}
		}

		previousState = actualState;

		if (continuousFollow && actualState == fakeLanternState.On)
			setLanternPosition ();

		flashingHandler();
		setClicks();
	}

	void setStates()
	{
		if (actualState == fakeLanternState.Off) {
			if (canBeReenabled && Mathf.Abs(Time.time - startTime) > turningOnTime)
			{
				actualState = fakeLanternState.On;
				setRayAlpha(1.0f);
				setCircleAlpha(1.0f);
				//startTime = Time.time;
			}

			startOffTime = Time.time;
		}

		if (actualState == fakeLanternState.On) {

			if (intermittance && Mathf.Abs(Time.time - startOffTime) > turningOffTime)
			{
				actualState = fakeLanternState.Off;
				//startOffTime = Time.time;
			}

			startTime = Time.time;
		}
	}

	public void canLanternBeEnabled(bool canOrNot)
	{
		canBeReenabled = canOrNot;
	}

	void setClicks()
	{
		if (actualState == fakeLanternState.On) {
			
			if (intermittance && Mathf.Abs(Time.time - startOffTime) < turningOffTime)
			{
				float diff = turningOffTime - Mathf.Abs(Time.time - startOffTime);

				float value = minClicTime + ((maxClicTime - minClicTime) * diff) / turningOffTime;
				//Debug.Log (value);

				if ((Time.time - lastTimeClic) > value)
				{
					if (firstClic)
						audioHandler.playForcedClipByName("FirstTimerClic");
					else
						audioHandler.playForcedClipByName("SecondTimerClic");

					firstClic = !firstClic;
					lastTimeClic = Time.time;
				}
			}
		}
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

		if (raggio != null)
			rendererRay = raggio.GetComponent<SpriteRenderer>();
		if (raggio_cerchio != null)
			rendererCircle = raggio_cerchio.GetComponent<SpriteRenderer>();
	}

	//se la fakeLantern deve sbloccare qualche contenuto, prendo gli indici necessari per farlo
	void setContentVariables()
	{
		if (unlockContent)
		{
			if (GeneralFinder.informativeManager != null)
			{
				for (int i = 0; i < GeneralFinder.informativeManager.sections.Length; i++)
				{
					if (GeneralFinder.informativeManager.sections[i] != null)
					{
						for (int j = 0; j < GeneralFinder.informativeManager.sections[i].contents.Length; j++)
						{
							if (GeneralFinder.informativeManager.sections[i].contents[j] != null)
							{
								if (GeneralFinder.informativeManager.sections[i].contents[j].unlockerObject == gameObject)
								{
									sectionIndex = i;
									contentIndex = j;
									Debug.Log (i);
									Debug.Log (j);
									return;
								}
							}
						}
					}
				}
			}
		}
	}

	void unlockContentMethod()
	{
		if (unlockContent)
		{
			if (GeneralFinder.informativeManager != null)
			{
				GeneralFinder.informativeManager.c_canShowNewContent(sectionIndex, contentIndex);
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
		//Debug.Log ("Interagito con fake lantern");
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
					if (unlockGlass)
						UIHandler(glassesToEnable[i].glassSprite);
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

		unlockContentMethod();

		if (emitParticles && particlesToEnable != null)
			particlesToEnable.SetActive(true);

	}

	void setRayAlpha(float inputAlpha)
	{
		if (rendererRay != null)
		{
			rendererRay.color = new Color (rendererRay.color.r, rendererRay.color.g, rendererRay.color.b, inputAlpha);
		}
	}

	void setCircleAlpha(float inputAlpha)
	{
		if (rendererCircle != null)
		{
			rendererCircle.color = new Color (rendererCircle.color.r, rendererCircle.color.g, rendererCircle.color.b, inputAlpha);
		}
	}

	void flashingHandler()
	{
		if (actualState == fakeLanternState.On) {

			//float tempTempo = Mathf.Abs(Time.time - startOffTime);
			if (intermittance && Mathf.Abs(Time.time - startOffTime) < turningOffTime)
			{
				if ((turningOffTime - Mathf.Abs(Time.time - startOffTime)) < timeAfterFlashing)
				{
					//Debug.Log(Mathf.FloorToInt((turningOffTime - Mathf.Abs(Time.time - startOffTime))/flashingTime));
					if (Mathf.FloorToInt((turningOffTime - Mathf.Abs(Time.time - startOffTime))/flashingTime) % 2 == 0)
					{
						setRayAlpha (1.0f);
						setCircleAlpha(1.0f);
					}
					else
					{
						setRayAlpha (flashingAlpha);
						setCircleAlpha(flashingAlpha);
					}
				}
			}
		}
	}

	void UIHandler(Sprite inputSprite)
	{
		//Debug.Log ("called");
		PickingObjectGraphic pick = gameObject.AddComponent<PickingObjectGraphic>();
		pick.setVariables(inputSprite, PlayingUI.UIPosition.BottomRight, 0);
		pick.setTimeToGetSmall(1.5f);
	}

}

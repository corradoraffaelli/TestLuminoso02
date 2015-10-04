using UnityEngine;
using System.Collections;

/// <summary>
/// Classe che si occupa di mostrare il bottone pulsante al raccoglimento di un oggetto.
/// </summary>

// Corrado
public class PulsingInfoButton : MonoBehaviour {

	RectTransform rectTransform;

	public bool needToPulse = true;
	bool needToPulseOLD = true;

	UnityEngine.UI.Image imageComponent;

	float baseScale = 1.0f;
	float maxScale = 1.8f;
	float pulseSpeed = 4.0f;
	int timePulsing = 4;
	int actualPulsing = 0;

	bool needToGetBig = true;

	void Start () {
		rectTransform = GetComponent<RectTransform> ();
		imageComponent = GetComponent<UnityEngine.UI.Image> ();
	}

	void Update () {
		if (needToPulse) {
			pulseHandling();
		}

		if (needToPulse != needToPulseOLD) {
			if (needToPulse)
			{
				imageComponent.enabled = true;
			}
			else
			{
				imageComponent.enabled = false;
			}
		}

		needToPulseOLD = needToPulse;

		transform.SetAsLastSibling ();
	}

	public void active(bool active = true)
	{
		needToPulse = active;
	}

	void pulseHandling()
	{
		Vector3 objScale;
		if (needToGetBig)
			objScale = new Vector3 (maxScale, maxScale, maxScale);
		else
			objScale = new Vector3 (baseScale, baseScale, baseScale);

		if (rectTransform != null) {
			rectTransform.localScale = Vector3.MoveTowards(rectTransform.localScale, objScale, Time.deltaTime * pulseSpeed);
		}

		if (rectTransform.localScale == objScale) {
			if (needToGetBig)
			{
				needToGetBig = false;
			}
			else
			{
				needToGetBig = true;
				actualPulsing++;
				//Debug.Log (actualPulsing);
				if (actualPulsing == timePulsing)
				{
					needToPulse = false;
					actualPulsing = 0;
				}
			}
		}
	}

	public void changeSprite(Sprite inputSprite)
	{
		if (imageComponent != null)
			imageComponent.sprite = inputSprite;
	}

	public void changeSpeed(float inputSpeed)
	{
		pulseSpeed = inputSpeed;
	}

	public void changeMaxScale(float inputScale)
	{
		maxScale = inputScale;
	}

	public void changeBaseScale(float inputScale)
	{
		baseScale = inputScale;
	}

	public void changePulsingTimes(int inputTimes)
	{
		timePulsing = inputTimes;
	}

	public void changeYDist(int inputDistance)
	{
		if (rectTransform != null) {
			Vector2 oldPos = rectTransform.anchoredPosition;
			rectTransform.anchoredPosition = new Vector2(oldPos.x, inputDistance);
		}
	}
}

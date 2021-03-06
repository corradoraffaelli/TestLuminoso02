﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Classe che fornisce un'interfaccia per far pulsare un qualsiasi elemento della UI.
/// </summary>

// Corrado
public class PulsingUIElement : MonoBehaviour {

	public GameObject pulsingGameObject;
	public RectTransform rectTransform;
	public float totalPulsingTime;
	public float pulseScale;
	public float pulseSpeed;

	float beginningTime;

	bool needToReachDest = true;

	//costruttore
	public PulsingUIElement(GameObject inputGameObject, float inputPulsingTime, float inputPulseScale, float inputPulseSpeed)
	{
		pulsingGameObject = inputGameObject;
		if (pulsingGameObject != null)
			rectTransform = pulsingGameObject.GetComponent<RectTransform>();
		totalPulsingTime = inputPulsingTime;
		pulseScale = inputPulseScale;
		pulseSpeed = inputPulseSpeed;
	}

	public void setVariables(GameObject inputGameObject, float inputPulsingTime, float inputPulseScale, float inputPulseSpeed)
	{
		pulsingGameObject = inputGameObject;
		if (pulsingGameObject != null)
			rectTransform = pulsingGameObject.GetComponent<RectTransform>();
		totalPulsingTime = inputPulsingTime;
		pulseScale = inputPulseScale;
		pulseSpeed = inputPulseSpeed;
	}

	void Start () {
		beginningTime = Time.time;
		deleteOtherPulsingElements();
	}

	void Update () {
		if (rectTransform != null)
		{
			if (rectTransform.localScale.x == pulseScale)
				needToReachDest = false;
			else if (rectTransform.localScale.x == 1.0f)
			{
				if ((Time.time - beginningTime) > totalPulsingTime)
					Destroy(this);
				needToReachDest = true;
			}

			float destScale = 1.0f;
			if (needToReachDest)
				destScale = pulseScale;
			
			float tempScale = Mathf.MoveTowards(rectTransform.localScale.x, destScale, pulseSpeed*Time.deltaTime);
			rectTransform.localScale = new Vector3(tempScale, tempScale, tempScale);
		}
	}

	void deleteOtherPulsingElements()
	{
		PulsingUIElement[] elements = GetComponents<PulsingUIElement>();
		for (int i = 0; i < elements.Length; i++)
		{
			//faccio un controllo anche sul tempo per evitare che si autodistrugga
			if (pulsingGameObject == elements[i].getPulsingGameObject() && beginningTime != elements[i].getBeginningTime())
			{
				Debug.Log ("distruggo altro elemento");
				Destroy(elements[i]);
			}
				
		}
	}

	public GameObject getPulsingGameObject()
	{
		return pulsingGameObject;
	}

	public float getBeginningTime()
	{
		return beginningTime;
	}

	public void stopPulsing()
	{
		rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		Destroy(this);
	}
}

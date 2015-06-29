using UnityEngine;
using System.Collections;

public class InteragibleObjectPulse : MonoBehaviour {

	public bool needToPulse;
	public float factor = 4;
	float pulseDelay = 2.0f;
	bool effectiveNeedToPulse = false;
	SpriteRenderer[] renderers;
	TimeSyncronizerInteragible sync;

	float beginningTime;

	void Start () {
		renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
		GameObject controller = GeneralFinder.controller;
		if (controller != null)
			sync = controller.GetComponent<TimeSyncronizerInteragible>();
		beginningTime = Time.time;
	}

	void Update () {
		//considero un ulteriore bool, per far partire il pulse, che viene messo a true, se anche quello pubblico è true, e dopo tot secondi dallo start
		//per evitare problemi con gli spriteRenderers degli oggetti comparsi in fade in
		if ((needToPulse && !effectiveNeedToPulse) && ((Time.time - beginningTime) > pulseDelay))
		{
			effectiveNeedToPulse = true;
		}

		if (sync!= null && effectiveNeedToPulse && sync.NeedToPulse)
		{
			float destColor = (1 - 1/factor) + sync.NormalizedValue/factor;
			changeColor(destColor);
		}
	}

	void changeAlpha(float inputValue)
	{
		if (renderers != null)
		{
			for (int i = 0; i <renderers.Length; i++)
			{
				if (renderers[i] != null)
				{
					Color actColor = renderers[i].color;
					renderers[i].color = new Color(actColor.r, actColor.g, actColor.b, inputValue);
				}
			}
		}
	}

	void changeColor(float inputValue)
	{
		if (renderers != null)
		{
			for (int i = 0; i <renderers.Length; i++)
			{
				if (renderers[i] != null)
				{
					Color actColor = renderers[i].color;
					renderers[i].color = new Color(inputValue, inputValue, inputValue, actColor.a);
				}
			}
		}
	}
}

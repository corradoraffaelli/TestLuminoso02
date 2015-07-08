using UnityEngine;
using System.Collections;

public class LeavesParticlesController : MonoBehaviour {

	public GameObject referenceWind;
	AreaEffector2D effector;

	public GameObject leavesParticles;
	public GameObject grassParticles;

	public float maxLeavesEmission = 1;
	public int maxGrassEmission = 3;

	public float minLeavesSize = 0.25f;
	public float maxLeavesSize = 0.45f;
	public float minGrassSize = 0.15f;
	public float maxGrassSize = 0.35f;

	ParticleRenderer[] leavesRenderers;
	ParticleEmitter[] leavesEmitters;
	ParticleRenderer[] grassRenderers;
	ParticleEmitter[] grassEmitters;

	bool windActive = false;
	bool wasWindActive = false;

	public float changingAlphaSpeed = 1.0f;
	public float maxLeavesAlpha = 0.5f;
	bool needToShow = false;
	bool needToHide = false;

	void Start () {
		fillEmittersRenderers();

		setLeavesEmission(maxLeavesEmission);
		setParticlesLeavesAlpha(maxLeavesAlpha);

		setGrassEmission(maxGrassEmission);
		setParticlesGrassAlpha(maxLeavesAlpha);

		setSizes();

		if (referenceWind != null)
			effector = referenceWind.GetComponent<AreaEffector2D>();
	}

	void Update () {
		windActive = isWindActive();
		if (wasWindActive != windActive)
		{
			if (windActive)
			{
				needToShow = true;
				needToHide = false;
			}
			else
			{
				needToShow = false;
				needToHide = true;
			}
			wasWindActive = windActive;
		}

		updateAlphas();
	}

	void fillEmittersRenderers()
	{
		if (leavesParticles != null)
		{
			leavesRenderers = leavesParticles.GetComponentsInChildren<ParticleRenderer>();
			leavesEmitters = leavesParticles.GetComponentsInChildren<ParticleEmitter>();
		}
		if (grassParticles != null)
		{
			grassRenderers = grassParticles.GetComponentsInChildren<ParticleRenderer>();
			grassEmitters = grassParticles.GetComponentsInChildren<ParticleEmitter>();
		}
	}

	void setLeavesEmission(float inputEmission)
	{
		if (leavesEmitters != null)
		{
			for (int i = 0; i < leavesEmitters.Length; i++)
			{
				if (leavesEmitters[i] != null)
				{
					leavesEmitters[i].minEmission = inputEmission;
					leavesEmitters[i].maxEmission = inputEmission;
				}
			}
		}
	}

	void setParticlesLeavesAlpha(float inputAlpha)
	{
		if (leavesRenderers != null)
		{
			for (int i = 0; i < leavesRenderers.Length; i++)
			{
				if (leavesRenderers[i] != null)
				{
					Color actColor = leavesRenderers[i].material.GetColor("_TintColor");
					Color tempColor = new Color(actColor.r, actColor.g, actColor.b, inputAlpha);
					leavesRenderers[i].material.SetColor ("_TintColor", tempColor);
				}
			}
		}
	}

	float getActualLeavesAlpha()
	{
		if (leavesRenderers != null)
		{
			for (int i = 0; i < leavesRenderers.Length; i++)
			{
				if (leavesRenderers[i] != null)
				{
					Color actColor = leavesRenderers[i].material.GetColor("_TintColor");
					return actColor.a;
				}
			}
		}
		return 0.0f;
	}

	void setGrassEmission(float inputEmission)
	{
		if (grassEmitters != null)
		{
			for (int i = 0; i < grassEmitters.Length; i++)
			{
				if (grassEmitters[i] != null)
				{
					grassEmitters[i].minEmission = inputEmission;
					grassEmitters[i].maxEmission = inputEmission;
				}
			}
		}
	}
	
	void setParticlesGrassAlpha(float inputAlpha)
	{
		if (grassRenderers != null)
		{
			for (int i = 0; i < grassRenderers.Length; i++)
			{
				if (grassRenderers[i] != null)
				{
					Color actColor = grassRenderers[i].material.GetColor("_TintColor");
					Color tempColor = new Color(actColor.r, actColor.g, actColor.b, inputAlpha);
					grassRenderers[i].material.SetColor ("_TintColor", tempColor);
				}
			}
		}
	}
	
	float getActualGrassAlpha()
	{
		if (grassRenderers != null)
		{
			for (int i = 0; i < grassRenderers.Length; i++)
			{
				if (grassRenderers[i] != null)
				{
					Color actColor = grassRenderers[i].material.GetColor("_TintColor");
					return actColor.a;
				}
			}
		}
		return 0.0f;
	}

	void updateAlphas()
	{
		if (needToShow)
		{
			float actAlpha = getActualLeavesAlpha();
			float newAlpha = Mathf.MoveTowards(actAlpha, maxLeavesAlpha, Time.deltaTime * changingAlphaSpeed);
			setParticlesLeavesAlpha(newAlpha);
			setParticlesGrassAlpha(newAlpha);
			if (newAlpha == maxLeavesAlpha)
				needToShow = false;
		}
		if (needToHide)
		{
			float actAlpha = getActualLeavesAlpha();
			float newAlpha = Mathf.MoveTowards(actAlpha, 0.0f, Time.deltaTime * changingAlphaSpeed);
			setParticlesLeavesAlpha(newAlpha);
			setParticlesGrassAlpha(newAlpha);
			if (newAlpha == 0.0f)
				needToHide = false;
		}
	}

	void setSizes()
	{
		if (leavesEmitters != null)
		{
			for (int i = 0; i < leavesEmitters.Length; i++)
			{
				if (leavesEmitters[i] != null)
				{
					leavesEmitters[i].minSize = minLeavesSize;
					leavesEmitters[i].maxSize = maxLeavesSize;
				}
			}
		}
		if (grassEmitters != null)
		{
			for (int i = 0; i < grassEmitters.Length; i++)
			{
				if (grassEmitters[i] != null)
				{
					grassEmitters[i].minSize = minGrassSize;
					grassEmitters[i].maxSize = maxGrassSize;
				}
			}
		}
	}

	bool isWindActive()
	{
		if (referenceWind != null && effector != null)
		{
			if (referenceWind.activeInHierarchy == true && effector.enabled == true)
				return true;
		}
		return false;
	}
}

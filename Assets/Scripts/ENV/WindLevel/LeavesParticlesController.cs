using UnityEngine;
using System.Collections;

/// <summary>
/// Specifica del livello del vento. Gestisce il comportamento delle foglie PARTICLES
/// </summary>

// Corrado
public class LeavesParticlesController : MonoBehaviour {

	public bool defaultVerseRight = true;

	public GameObject[] referenceWind;
	AreaEffector2D[] effector;

	public GameObject leavesParticles;
	public GameObject grassParticles;

	public float maxLeavesEmission = 1;
	public int maxGrassEmission = 3;

	public float minLeavesSize = 0.25f;
	public float maxLeavesSize = 0.45f;
	public float minGrassSize = 0.15f;
	public float maxGrassSize = 0.35f;
	public float minEnergy = 30.0f;
	public float maxEnergy = 30.0f;

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
		//setParticlesLeavesAlpha(maxLeavesAlpha);
		setParticlesLeavesAlpha(0.0f);

		setGrassEmission(maxGrassEmission);
		//setParticlesGrassAlpha(maxLeavesAlpha);
		setParticlesGrassAlpha(0.0f);

		setSizes();

		effector = new AreaEffector2D[referenceWind.Length];
		for (int i = 0; i < referenceWind.Length; i++)
		{
			if (referenceWind[i] != null)
				effector[i] = referenceWind[i].GetComponent<AreaEffector2D>();
		}

		setEnergy();
		setVerse();
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
		}
		wasWindActive = windActive;

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

	void setVerse()
	{
		if (!defaultVerseRight)
		{
			if (leavesEmitters != null)
			{
				for (int i = 0; i < leavesEmitters.Length; i++)
				{
					if (leavesEmitters[i] != null)
					{
						Vector3 actualWorldSpeed = leavesEmitters[i].worldVelocity;
						leavesEmitters[i].worldVelocity = new Vector3(-actualWorldSpeed.x, actualWorldSpeed.y, actualWorldSpeed.z);
						Vector3 actualRndSpeed = leavesEmitters[i].rndVelocity;
						leavesEmitters[i].rndVelocity = new Vector3(actualRndSpeed.x, -actualRndSpeed.y, actualRndSpeed.z);
					}
				}
			}
			if (grassEmitters != null)
			{
				for (int i = 0; i < grassEmitters.Length; i++)
				{
					if (grassEmitters[i] != null)
					{
						Vector3 actualWorldSpeed = grassEmitters[i].worldVelocity;
						grassEmitters[i].worldVelocity = new Vector3(-actualWorldSpeed.x, actualWorldSpeed.y, actualWorldSpeed.z);
						Vector3 actualRndSpeed = grassEmitters[i].rndVelocity;
						grassEmitters[i].rndVelocity = new Vector3(actualRndSpeed.x, -actualRndSpeed.y, actualRndSpeed.z);
					}
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

	void setEnergy()
	{
		if (leavesEmitters != null)
		{
			for (int i = 0; i < leavesEmitters.Length; i++)
			{
				if (leavesEmitters[i] != null)
				{
					leavesEmitters[i].minEnergy = minEnergy;
					leavesEmitters[i].maxEnergy = maxEnergy;
				}
			}
		}
		if (grassEmitters != null)
		{
			for (int i = 0; i < grassEmitters.Length; i++)
			{
				if (grassEmitters[i] != null)
				{
					grassEmitters[i].minEnergy = minEnergy;
					grassEmitters[i].maxEnergy = maxEnergy;
				}
			}
		}
	}

	bool isWindActive()
	{
		for (int i = 0; i < referenceWind.Length; i++)
		{
			if (referenceWind[i] != null && effector[i] != null)
			{
				if (referenceWind[i].activeInHierarchy == true && effector[i].enabled == true)
					return true;
			}
		}

		return false;
	}
}

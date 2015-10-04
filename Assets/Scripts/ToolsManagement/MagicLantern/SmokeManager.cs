using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce il fumo che esce dalla Lanterna Magica.
/// </summary>

// Corrado
public class SmokeManager : MonoBehaviour {

	ParticleSystem particleSystem;

	float nextEmission;
	float nextStop;

	public float maxDurationRange = 1.0f;
	public float maxStopRange = 2.0f;

	public float maxEmission = 10.0f;
	public float minEmission = 3.0f;
	float actualRandomEmission;
	//float actualEmission = 0.0f;
	public float changingSpeed = 10.0f;

	float beginEmission = 0.0f;
	float stopEmission = 0.0f;

	float standardEmission;

	bool emitting = false;
	bool firstStop = true;
	bool firstEmitting = false;
	
	void Start () {
		particleSystem = GetComponent<ParticleSystem>();
		//nextEmission = getRandomNum();

		standardEmission = particleSystem.emissionRate;
		particleSystem.emissionRate = 0.0f;

		actualRandomEmission = Random.Range(minEmission, maxEmission);
		//actualEmission = 0.0f;
	}

	void Update () {
		handleTiming();
		handleEmission();
	}

	float getRandomStop()
	{
		return Random.Range (0.2f, maxStopRange);
	}

	float getRandomDuration()
	{
		return Random.Range (0.2f, maxDurationRange);
	}

	void handleTiming()
	{
		if (emitting)
		{
			//ad ogni emissione mi salvo il tempo, così quando smette, quello è l'ultimo istante
			//stopEmission = Time.time;
			if (firstEmitting)
			{
				nextStop = getRandomDuration();
				firstEmitting = false;
			}
			else if ((Time.time - beginEmission) > nextStop)
			{
				firstStop = true;
				emitting = false;
				stopEmission = Time.time;
			}

		}
		else
		{
			if (firstStop)
			{
				nextEmission = getRandomStop();
				firstStop = false;
			}
			else
			{
				if ((Time.time - stopEmission) > nextEmission)
				{
					firstEmitting = true;
					emitting = true;
					beginEmission = Time.time;
				}
			}
		}
	}

	void handleEmission()
	{
		if (emitting)
		{
			if (particleSystem != null)
			{
				//Debug.Log("zaza");
				//particleSystem.emissionRate = standardEmission;

				if (particleSystem.emissionRate == actualRandomEmission)
					actualRandomEmission = Random.Range(minEmission, maxEmission);
				particleSystem.emissionRate = Mathf.MoveTowards(particleSystem.emissionRate, actualRandomEmission, Time.deltaTime * changingSpeed);
			}
		}
		else
		{
			if (particleSystem != null)
			{
				particleSystem.emissionRate = 0.0f;
			}
		}
		//actualEmission = particleSystem.emissionRate;
	}
}

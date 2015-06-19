using UnityEngine;
using System.Collections;

public class SmokeManager : MonoBehaviour {

	ParticleSystem particleSystem;

	float nextEmission;

	public float duration = 0.3f;
	public float maxRandomRange = 2.0f;
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
	}

	void Update () {
		handleTiming();
		handleEmission();
	}

	float getRandomNum()
	{
		return Random.Range (0.2f, maxRandomRange);
	}

	void handleTiming()
	{
		if (emitting)
		{
			//ad ogni emissione mi salvo il tempo, così quando smette, quello è l'ultimo istante
			//stopEmission = Time.time;
			if ((Time.time - beginEmission) > duration)
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
				nextEmission = getRandomNum();
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
				particleSystem.emissionRate = standardEmission;
			}
		}
		else
		{
			if (particleSystem != null)
			{
				particleSystem.emissionRate = 0.0f;
			}
		}
	}
}

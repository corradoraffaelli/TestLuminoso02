using UnityEngine;
using System.Collections;

public class SpiralParticleHandler : MonoBehaviour {

	ParticleSystem partSystem;

	public float changingColorSpeed = 1.0f;

	bool stampato = false;
	
	void Start () {
		partSystem = GetComponent<ParticleSystem>();
	}

	void Update () {

		 
	}

	void FixedUpdate()
	{

		if (partSystem != null)
		{
			Color actColor = partSystem.startColor;
			float actAlpha = actColor.a;
			float newAlpha = Mathf.MoveTowards(actAlpha, 0.0f, Time.deltaTime*changingColorSpeed / 5.0f);
			
			partSystem.startColor = new Color(actColor.r, actColor.g, actColor.b, newAlpha);
			
			if (!stampato && newAlpha == 0.0f)
			{
				Debug.Log ("alpha zero " + Time.time);
				stampato = true;
			}
			
		}
	}
}

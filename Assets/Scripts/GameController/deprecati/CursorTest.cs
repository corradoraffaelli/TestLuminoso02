using UnityEngine;
using System.Collections;

public class CursorTest : MonoBehaviour {

	GameObject controller;
	CursorHandler CH;
	SpriteRenderer spriteRenderer;
	bool oldCursorMoving = true;

	ParticleSystem[] particleSystems;
	float[] particleNumbers;

	// Use this for initialization
	void Awake () {
		controller = GameObject.FindGameObjectWithTag ("Controller");
		CH = controller.GetComponent<CursorHandler> ();
		spriteRenderer = GetComponent<SpriteRenderer>();

		particleSystems = GetComponentsInChildren<ParticleSystem>();
		particleNumbers = new float[particleSystems.Length];

		for (int i = 0; i< particleNumbers.Length;i++)
		{
			particleNumbers[i] = particleSystems[i].emissionRate;
		}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 cursorPos = CH.getCursorWorldPosition();
		transform.position = new Vector3 (cursorPos.x, cursorPos.y, transform.position.z);

		bool actualCursorMoving = CH.isCursorMoving();

		if (oldCursorMoving != actualCursorMoving)
		{
			oldCursorMoving = actualCursorMoving;
			if (actualCursorMoving)
			{
				spriteRenderer.enabled = true;
				if (particleSystems != null && particleNumbers!=null)
				for (int i = 0; i< particleNumbers.Length;i++)
				{
					if (particleSystems[i] != null && particleNumbers[i]!=null)
						particleSystems[i].emissionRate = particleNumbers[i];
				}
			}
			else
			{
				spriteRenderer.enabled = false;
				for (int i = 0; i< particleNumbers.Length;i++)
				{
					if (particleSystems[i] != null && particleNumbers[i]!=null)
						particleSystems[i].emissionRate = 0.0f;
				}
					
			}
		}
	}
}

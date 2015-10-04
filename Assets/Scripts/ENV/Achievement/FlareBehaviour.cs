using UnityEngine;
using System.Collections;

/// <summary>
/// Si occupa di mostrare un riflesso (flare) sopra gli oggetti collezionabili
/// </summary>

// Corrado
public class FlareBehaviour : MonoBehaviour {

	public GameObject[] circles;
	public GameObject star;
	public GameObject line;



	SpriteRenderer[] circlesRenderers;
	SpriteRenderer starRenderer;
	SpriteRenderer lineRenderer;

	Transform[] circlesTransform;
	Transform starTransform;
	Transform lineTransform;

	float maxLineScale;
	float maxCirclesScale;

	public float maxRotationAngle = 180.0f;
	public float minReached = 0.45f;
	public float maxReached = 0.75f;

	public float referenceNumber = 0.0f;
	[Range(0.1f, 15.0f)]
	public float changingSpeed = 5.0f;

	void Start () {
		takeComponents();
		maxLineScale = lineTransform.localScale.x;
		maxCirclesScale = circlesTransform[0].localScale.x;
		setInitialStates();
	}

	void Update () {

			updateRefNumber();
			
			updateStar();
			updateLine();
			updateCircles();
			
			destroyManager();

	}

	void updateRefNumber()
	{
		referenceNumber = Mathf.MoveTowards(referenceNumber, 1.0f, Time.deltaTime*changingSpeed / 10);
	}

	void updateStar()
	{
		//updateRotation
		starTransform.localEulerAngles = new Vector3(0.0f,0.0f,referenceNumber * maxRotationAngle);

		//updateAlpha
		if (referenceNumber < minReached)
		{
			float actualAlpha = referenceNumber / minReached;
			setStarAlpha(actualAlpha);
		}
		else if (referenceNumber < maxReached)
		{
			//float actualAlpha = (referenceNumber - minReached) / (maxReached - minReached);
			setStarAlpha(1.0f);
		}
		else
		{
			float actualAlpha = 1.0f - ((referenceNumber - maxReached) / (1.0f - maxReached));
			setStarAlpha(actualAlpha);
		}
	}

	void updateLine()
	{
		//updateScale
		if (referenceNumber < minReached)
		{
			float newScale = referenceNumber / minReached;
			newScale = newScale * maxLineScale;
			lineTransform.localScale = new Vector3(newScale, newScale, 1.0f);
		}
		else
		{
			lineTransform.localScale = new Vector3(maxLineScale, maxLineScale, 1.0f);
		}

		//updateAlpha
		if (referenceNumber < minReached)
		{
			float actualAlpha = referenceNumber / minReached;
			setLineAlpha(actualAlpha);
		}
		else if (referenceNumber < maxReached)
		{
			setLineAlpha(1.0f);
		}
		else
		{
			float actualAlpha = 1.0f - ((referenceNumber - maxReached) / (1.0f - maxReached));
			setLineAlpha(actualAlpha);
		}
	}

	void updateCircles()
	{
		/*
		//updateAlpha
		if (referenceNumber < minReached)
		{
			float actualAlpha = referenceNumber / minReached;
			setCirclesAlpha(actualAlpha);
		}
		else if (referenceNumber < maxReached)
		{
			setCirclesAlpha(1.0f);
		}
		else
		{
			float actualAlpha = 1.0f - ((referenceNumber - maxReached) / (1.0f - maxReached));
			setCirclesAlpha(actualAlpha);
		}
		*/

		//updateScale
		if (referenceNumber < maxReached)
		{
			float actualScale = referenceNumber / maxReached;
			actualScale = actualScale*maxCirclesScale;
			setCirclesScale(actualScale);
		}
		/*
		else if (referenceNumber < maxReached)
		{
			setCirclesScale(maxCirclesScale);
		}
		*/
		else
		{
			float actualScale = 1.0f - ((referenceNumber - maxReached) / (1.0f - maxReached));
			actualScale = actualScale*maxCirclesScale;
			setCirclesScale(actualScale);
		}

		setCirclesAlpha(1.0f);

	}

	void destroyManager()
	{
		if (referenceNumber == 1.0f)
			Destroy(this.gameObject);
	}

	void takeComponents()
	{
		//spriteRenderer = GetComponent<SpriteRenderer>();

		if (circles != null)
		{
			circlesRenderers = new SpriteRenderer[circles.Length];
			circlesTransform = new Transform[circles.Length];
			for (int i = 0; i< circles.Length; i++)
			{
				if (circles[i] != null)
				{
					circlesRenderers[i] = circles[i].GetComponent<SpriteRenderer>();
					circlesTransform[i] = circles[i].GetComponent<Transform>();
				}
			}
		}

		if (star != null)
		{
			starRenderer = star.GetComponent<SpriteRenderer>();
			starTransform = star.GetComponent<Transform>();
		}

		if (line != null)
		{
			lineRenderer = line.GetComponent<SpriteRenderer>();
			lineTransform = line.GetComponent<Transform>();
		}
	}

	void setInitialStates()
	{
		setLineAlpha(0.0f);
		setStarAlpha(0.0f);
		setCirclesAlpha(0.0f);
	}

	void setCirclesAlpha(float inputAlpha)
	{
		if (circlesRenderers != null)
		{
			for (int i = 0; i< circlesRenderers.Length; i++)
			{
				if (circlesRenderers[i] != null)
				{
					Color defColor = circlesRenderers[i].color;
					circlesRenderers[i].color = new Color(defColor.r, defColor.g, defColor.b, inputAlpha);
				}
			}
		}
	}

	void setCirclesScale(float inputScale)
	{
		if (circlesTransform != null)
		{
			for (int i = 0; i< circlesTransform.Length; i++)
			{
				if (circlesTransform[i] != null)
				{
					circlesTransform[i].localScale = new Vector3(inputScale, inputScale, 1.0f);
				}
			}
		}
	}

	void setStarAlpha(float inputAlpha)
	{
		if (starRenderer != null)
		{
			Color defColor = starRenderer.color;
			starRenderer.color = new Color(defColor.r, defColor.g, defColor.b, inputAlpha);
		}
	}

	void setLineAlpha(float inputAlpha)
	{
		if (lineRenderer != null)
		{
			Color defColor = lineRenderer.color;
			lineRenderer.color = new Color(defColor.r, defColor.g, defColor.b, inputAlpha);
		}
	}
}

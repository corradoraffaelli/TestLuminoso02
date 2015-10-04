using UnityEngine;
using System.Collections;

/// <summary>
/// Permette di fare movimenti lineari tra un'origine ed una destinazione.
/// </summary>

// Corrado
public class SimpleLinearMovement : MonoBehaviour {

	public Transform destination;
	Vector3 originPosition;
	Vector3 destinationPosition;

	public bool active = true;

	[Range(0.2f,20.0f)]
	public float speed = 4.0f;
	public bool lerp = false;
	[Range(0.2f,20.0f)]
	public float lerpSpeed = 1.0f;
	public float lerpThreshold = 0.1f;
	
	public bool canReturnBack = true;

	[Range(0.0f,20.0f)]
	public float pauseToSwitch = 0.5f;
	float stopTime = 0.0f;

	bool destReached = false;
	public bool DestReached
	{
		get{return destReached;}
	}
	bool origReached = true;

	bool needToReachDest = true;
	bool needToReachOrig = false;



	// Use this for initialization
	void Start () {
		if (destination != null)
			destinationPosition = destination.position;
		else
			destinationPosition = transform.position;
		originPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (active)
		{
			if (needToReachDest && !destReached)
				moveToDest();
			else if (needToReachOrig && !origReached)
				moveToOrigin();
			
			if (canReturnBack && Mathf.Abs (Time.time - stopTime) > pauseToSwitch)
			{
				if (destReached)
				{
					needToReachOrig = true;
					needToReachDest = false;
				}
				
				if (origReached)
				{
					needToReachOrig = false;
					needToReachDest = true;
				}
				
			}
		}

	}

	void moveToDest()
	{
		origReached = false;

		bool isReached = false;

		if (!lerp)
		{
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, destinationPosition, step);

			if (transform.position == destinationPosition)
				isReached = true;
		}else{
			transform.position = Vector3.Lerp(transform.position, destinationPosition, lerpSpeed*Time.deltaTime);

			if (Vector3.Distance(transform.position, destinationPosition) < lerpThreshold)
				isReached = true;
		}

		//actualPos = transform.position;
		if (isReached)
		{
			destReached = true;
			stopTime = Time.time;
		}
			
	}

	void moveToOrigin()
	{
		destReached = false;

		bool isReached = false;

		if (!lerp)
		{
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, originPosition, step);

			if (transform.position == originPosition)
				isReached = true;
		}else{
			transform.position = Vector3.Lerp(transform.position, originPosition, lerpSpeed*Time.deltaTime);

			if (Vector3.Distance(transform.position, originPosition) < lerpThreshold)
				isReached = true;
		}
		
		//actualPos = transform.position;
		if (isReached)
		{
			origReached = true;
			stopTime = Time.time;
		}
	}

	public void setOriginPosition(Vector3 position)
	{
		originPosition = position;
	}

	public void setDestinationPosition(Vector3 position)
	{
		destinationPosition = position;
	}

	public bool isOnDestination()
	{
		return destReached;
	}

	public bool isOnOrigin()
	{
		return origReached;
	}
}

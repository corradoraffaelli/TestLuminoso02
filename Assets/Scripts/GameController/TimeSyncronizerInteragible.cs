using UnityEngine;
using System.Collections;

public class TimeSyncronizerInteragible : MonoBehaviour {

	[SerializeField]
	[Range(1.0f, 200.0f)]
	float timeBeforePulse = 10.0f;
	[SerializeField]
	[Range(0.05f, 30.0f)]
	float pulsingSpeed = 2.0f;
	[SerializeField]
	[Range(1, 10)]
	int pulsingTimes = 4;

	float lastPulseTime = 0.0f;

	bool needToPulse;
	public bool NeedToPulse{
		get{return needToPulse;}
	}

	float normalizedValue = 1.0f;
	public float NormalizedValue{
		get{return normalizedValue;}
	}

	int actualPulsingTime = 0;

	bool reachedMaxValue = true;
	bool firstMin = false;

	void Start () {
		lastPulseTime = Time.time;
	}

	void Update () {
		//bool startPulse = isTimeToPulse();
		if (isTimeToPulse())
			pulseHandler();
	}

	bool isTimeToPulse()
	{
		if (needToPulse || (Time.time - lastPulseTime) > timeBeforePulse)
		{
			lastPulseTime = Time.time;
			return true;
		}
		else
			return false;
	}

	void pulseHandler()
	{
		if  (actualPulsingTime < pulsingTimes)
		{
			needToPulse = true;
			float destValue = 0.0f;
			if (!reachedMaxValue)
			{
				destValue = 1.0f;
			}
			normalizedValue = Mathf.MoveTowards(normalizedValue, destValue, Time.deltaTime * pulsingSpeed);

			if (normalizedValue == 1.0f)
			{
				reachedMaxValue = true;
				actualPulsingTime ++;
			}else if (normalizedValue == 0.0f)
			{
				reachedMaxValue = false;
			}

		}
		else
		{
			actualPulsingTime = 0;
			needToPulse = false;
		}
			
	}
}

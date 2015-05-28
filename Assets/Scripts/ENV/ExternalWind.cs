using UnityEngine;
using System.Collections;

public class ExternalWind : MonoBehaviour {

	[Range(0.0f,15.0f)]
	public float offTime = 3.0f;
	[Range(0.0f,15.0f)]
	public float onTime = 6.0f;

	float beginOnTime = 0.0f;
	float beginOffTime = 0.0f;

	public bool turnedOn = true;

	AreaEffector2D areaEffector;

	public bool intermittance = true;

	public bool subWind = false;
	public GameObject masterWind;
	ExternalWind masterExtWind;


	// Use this for initialization
	void Start () {
		areaEffector = GetComponent<AreaEffector2D>();
		if (subWind && masterWind != null)
			masterExtWind = masterWind.GetComponent<ExternalWind>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!subWind)
		{
			if (intermittance)
			{
				if (turnedOn)
				{
					if (Mathf.Abs(Time.time - beginOnTime) > onTime)
					{
						turnedOn = false;
						areaEffector.enabled = false;
					}
					beginOffTime = Time.time;
				}else{
					if (Mathf.Abs(Time.time - beginOffTime) > offTime)
					{
						turnedOn = true;
						areaEffector.enabled = true;
					}
					beginOnTime = Time.time;
				}
			}
		}else{
			if (masterExtWind != null)
			{
				if (!masterExtWind.turnedOn)
				{
					areaEffector.enabled = false;
					turnedOn = false;
				}else{
					areaEffector.enabled = true;
					turnedOn = true;
				}
			}
				
					
		}

	}

	void flipDirection()
	{

	}

	void setIntermittance()
	{

	}
}

using UnityEngine;
using System.Collections;

public class PipeParticles : MonoBehaviour {

	public enum State{Initial, Pipe01Down, Pipe02Down}

	public State actualState = State.Initial;

	public GameObject pipe01;
	public GameObject pipe02;
	SimpleLinearMovement linearMov01;
	SimpleLinearMovement linearMov02;
	Pipe02Down pipe02Down;

	public GameObject particleExit;
	public GameObject particleMiddle;
	public GameObject particleRight;
	ParticleSystem partSystExit;
	ParticleSystem partSystMiddle;
	ParticleSystem partSystRight;

	public GameObject wind;
	ExternalWind externalWind;

	public GameObject mechanism;

	public float particleMiddleChangingSpeed = 1.0f;

	void Start () {
		if (pipe01 != null)
			linearMov01 = pipe01.GetComponent<SimpleLinearMovement>();
		if (pipe02 != null)
		{
			linearMov02 = pipe02.GetComponent<SimpleLinearMovement>();
			pipe02Down = pipe02.GetComponent<Pipe02Down>();
		}
		if (particleExit != null)
			partSystExit = particleExit.GetComponent<ParticleSystem>();
		if (particleMiddle != null)
			partSystMiddle = particleMiddle.GetComponent<ParticleSystem>();
		if (particleRight != null)
			partSystRight = particleRight.GetComponent<ParticleSystem>();

		if (wind != null)
			externalWind = wind.GetComponent<ExternalWind>();
	}
	
	// Update is called once per frame
	void Update () {
		updateState();
		updateParticlesState();
	}

	void updateState()
	{
		if (actualState == State.Initial)
		{
			if (linearMov01 != null && linearMov01.active)
				actualState = State.Pipe01Down;
		}
	}

	void updateParticlesState()
	{
		bool turnedOn = true;
		if (externalWind != null)
			turnedOn = externalWind.turnedOn;

		if (turnedOn){
			partSystExit.enableEmission = true;
			partSystMiddle.enableEmission = true;
			partSystRight.enableEmission = true;
		}else{
			partSystExit.enableEmission = false;
			partSystMiddle.enableEmission = false;
			partSystRight.enableEmission = false;
		}
		if (actualState == State.Pipe01Down)
		{
			if (partSystMiddle != null)
			{
				float newMidRate = Mathf.MoveTowards(partSystMiddle.emissionRate, 100.0f, particleMiddleChangingSpeed*Time.deltaTime*20);
				partSystMiddle.emissionRate = newMidRate;
			}

			if (partSystRight != null)
			{
				float newMidRate = Mathf.MoveTowards(partSystMiddle.emissionRate, 100.0f, particleMiddleChangingSpeed*Time.deltaTime*20);
				partSystRight.emissionRate = newMidRate;
			}
		}
	}
}

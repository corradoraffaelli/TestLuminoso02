using UnityEngine;
using System.Collections;

public class LeavesBehaviour : MonoBehaviour {

	public Sprite[] leavesSprites;
	public float changingTime = 4.0f;
	public float minTorque = -5.0f;
	public float maxTorque = 5.0f;
	public float minYForce = 2.0f;
	public float maxYForce = 70.0f;
	public float maxXVelocity = 2.0f;
	public float maxFallingSpeed = -1.2f;
	public float XChangingSpeed = 0.2f;

	public float minTimeBeforeDeath = 15.0f;
	public float maxTimeBeforeDeath = 35.0f;
	float timeBeforeDeath;
	float beginningTime;

	Rigidbody2D rigidbody;
	ConstantForce2D force;
	public bool hitByWind = false;

	float lastChanging = -20.0f;

	GameObject wind;
	AreaEffector2D effector;

	void Start () {
		rigidbody = GetComponent<Rigidbody2D>();
		force = GetComponent<ConstantForce2D>();
		assignSprite();

		beginningTime = Time.time;
		timeBeforeDeath = Random.Range(minTimeBeforeDeath, maxTimeBeforeDeath);
	}

	void Update () {
		if (wind == null || wind.activeInHierarchy == false || effector == null || effector.enabled == false)
			hitByWind = false;
		else if (wind != null && wind.activeInHierarchy == true && effector != null && effector.enabled == true)
			hitByWind = true;


		if (hitByWind || rigidbody.velocity.y < 0)
		{
			if ((Time.time - lastChanging) > changingTime)
			{
				lastChanging = Time.time;
				float randomTorque = Random.Range(minTorque, maxTorque);
				force.torque = randomTorque;
				float randomForce = Random.Range(minYForce, maxYForce);
				if (hitByWind)
					force.force = new Vector2(force.force.x, randomForce);
				else
					force.force = new Vector2(force.force.x, 0.0f);
			}
		}
		else
		{
			force.torque = 0.0f;
			force.force = new Vector2(force.force.x, 0.0f);
		}


		if (rigidbody.velocity.x > maxXVelocity)
			rigidbody.velocity = new Vector2(maxXVelocity, rigidbody.velocity.y);
		else if (rigidbody.velocity.x < -maxXVelocity)
			rigidbody.velocity = new Vector2(-maxXVelocity, rigidbody.velocity.y);

		if (rigidbody.velocity.y < maxFallingSpeed)
			rigidbody.velocity = new Vector2(rigidbody.velocity.x, maxFallingSpeed);

		adjustXSpeed();

		if ((Time.time - beginningTime) > timeBeforeDeath)
			Destroy(this.gameObject);
	}

	void adjustXSpeed()
	{
		if (!hitByWind && rigidbody.velocity.x!=0.0f)
		{
			float newX = Mathf.MoveTowards(rigidbody.velocity.x, 0.0f, Time.deltaTime * XChangingSpeed);
			rigidbody.velocity = new Vector2(newX, rigidbody.velocity.y);
		}
	}

	void assignSprite()
	{
		int index = Random.Range (0, leavesSprites.Length);
		if (leavesSprites[index] != null)
			GetComponent<SpriteRenderer>().sprite = leavesSprites[index];
		else
			assignSprite();
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<AreaEffector2D>() != null)
		{
			wind = other.gameObject;
			effector = wind.GetComponent<AreaEffector2D>();
			hitByWind = true;

		}	
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<AreaEffector2D>() != null)
		{
			effector = null;
			hitByWind = false;
			wind = null;
		}	
	}

	public void setXForce(float inputForce)
	{
		if (force != null)
			force.force = new Vector2(inputForce, force.force.y);
		else
			Debug.Log ("non quadra");
	}
}

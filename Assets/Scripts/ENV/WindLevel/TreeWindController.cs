using UnityEngine;
using System.Collections;

public class TreeWindController : MonoBehaviour {

	public GameObject referenceWind;
	AreaEffector2D effector;

	bool windRight = true;

	bool windActive = false;
	bool wasWindActive = false;

	Animator animator;

	public GameObject leafPrefab;
	public Transform[] randomPositions;
	public float timeToSpawn = 0.4f;
	float lastSpawn = 0.0f;

	public float minForceLeaf = 10.0f;
	public float maxForceLeaf = 50.0f;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		if (referenceWind != null)
			effector = referenceWind.GetComponent<AreaEffector2D>();
	}
	
	// Update is called once per frame
	void Update () {
		windActive = isWindActive();
		if (windActive != wasWindActive)
		{
			animator.SetBool("activating", windActive);
			wasWindActive = windActive;
		}
		createLeaves();

		if (effector != null && effector.enabled)
		{
			if ((effector.forceAngle < 90 && effector.forceAngle > -90) || (effector.forceAngle > 270) || (effector.forceAngle < -270))
				windRight = true;
			else
				windRight = false;
		}
	}

	void createLeaves()
	{
		if (windActive && (Time.time - lastSpawn) > timeToSpawn && leafPrefab != null)
		{
			GameObject leaf = Instantiate(leafPrefab, chooseRandomPosition().position, Quaternion.identity) as GameObject;
			float randomForce = Random.Range (minForceLeaf, maxForceLeaf);
			//Debug.Log (randomForce);
			//leaf.GetComponent<LeavesBehaviour>().setXForce(randomForce);
			/*
			ConstantForce2D forza = leaf.GetComponent<ConstantForce2D>();
			if (forza != null)
				forza.force = new Vector2 (randomForce, forza.force.y);
			else
				Debug.Log ("minchia");

			Debug.Log (leaf.GetComponent<ConstantForce2D>().force);
			*/

			Rigidbody2D rigidbodyLeaf = leaf.GetComponent<Rigidbody2D>();
			if (rigidbodyLeaf != null)
			{
				rigidbodyLeaf.velocity = new Vector2 (randomForce/10.0f, rigidbodyLeaf.velocity.y);
			}

			lastSpawn = Time.time;
		}
			
	}

	bool isWindActive()
	{
		if (referenceWind != null && effector != null)
		{
			if (referenceWind.activeInHierarchy == true && effector.enabled == true)
				return true;
		}
		return false;
	}

	Transform chooseRandomPosition()
	{
		int randomIndex = Random.Range(0, randomPositions.Length);
		if (randomPositions[randomIndex] != null)
			return randomPositions[randomIndex];
		else
			return chooseRandomPosition();
	}
}

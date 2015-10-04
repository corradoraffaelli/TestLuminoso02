using UnityEngine;
using System.Collections;

/// <summary>
/// Specifica del livello del vento. Gestisce il comportamento delle piante spostate dal vento.
/// </summary>

// Corrado
public class TreeWindController : MonoBehaviour {

	GameObject referenceWind;
	AreaEffector2D effector;
	bool savedGO = false;
	//public bool colliding = false;

	bool wasWindRight = true;
	bool windRight = true;

	bool windActive = false;
	bool wasWindActive = false;

	public bool IsWindActive
	{
		get{return windActive;}
	}

	Animator animator;

	public GameObject leafPrefab;
	public Transform[] randomPositions;
	public float timeToSpawn = 0.4f;
	float lastSpawn = 0.0f;

	public float minForceLeaf = 10.0f;
	public float maxForceLeaf = 50.0f;

	bool needToFlip = false;
	//bool flipped = false;
	
	void Start () {
		animator = GetComponent<Animator>();
		//if (referenceWind != null)
		//	effector = referenceWind.GetComponent<AreaEffector2D>();
	}

	void Update () {
		windActive = isWindActive();

		if (effector != null && effector.enabled)
		{
			if ((effector.forceAngle < 90 && effector.forceAngle > -90) || (effector.forceAngle > 270) || (effector.forceAngle < -270))
				windRight = true;
			else
				windRight = false;
		}

		handleFlippingNeed();

		if (windActive != wasWindActive)
		{
			handleFlipping();

			animator.SetBool("activating", windActive);
			wasWindActive = windActive;
		}
		createLeaves();
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

			if (!windRight)
				randomForce = -randomForce;

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
			else
			{
				referenceWind = null;
				effector = null;
			}
		}
		savedGO = false;
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

	void OnTriggerStay2D(Collider2D other)
	{
		if (!savedGO) 
		{
			AreaEffector2D areaEffector = other.gameObject.GetComponent<AreaEffector2D>();
			if (areaEffector != null)
			{
				referenceWind = other.gameObject;
				savedGO = true;
				effector = areaEffector;
			}
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (savedGO) 
		{
			if (other.gameObject == referenceWind)
			{
				referenceWind = null;
				effector = null;
			}
		}
	}

	void handleFlippingNeed()
	{
		if (windRight != wasWindRight)
		{
			needToFlip = true;
		}
		wasWindRight = windRight;
	}

	void handleFlipping()
	{
		if (windActive && needToFlip)
		{
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
			needToFlip = false;
		}
	}
}

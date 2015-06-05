using UnityEngine;
using System.Collections;

public class FlagBehaviour : MonoBehaviour {

	public bool active = false;
	public bool savedGO = false;
	public GameObject windGO;
	AreaEffector2D areaEffector;

	Animator animator;

	bool needToReset = true;
	bool colliding = false;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

		if ((areaEffector == null && windGO != null) || (windGO != null && needToReset))
			areaEffector = windGO.GetComponent<AreaEffector2D>();

		if (areaEffector != null)
		{
			if(areaEffector.enabled && colliding)
			{
				if ((areaEffector.forceDirection < 90 && areaEffector.forceDirection > -90) || (areaEffector.forceDirection > 270) || (areaEffector.forceDirection < -270))
					transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
				else
					transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);

				active = true;
				animator.SetBool("Active", true);

			}else{

				active = false;
				animator.SetBool("Active", false);
			}

		}

		if (windGO != null && !windGO.activeInHierarchy)
		{
			active = false;
			animator.SetBool("Active", false);
		}

		if (windGO == null || !windGO.activeInHierarchy)
		{
			savedGO = false;
			needToReset = true;
		}
			
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (!savedGO) 
		{
			AreaEffector2D areaEffector = other.gameObject.GetComponent<AreaEffector2D>();
			if (areaEffector != null)
			{
				windGO = other.gameObject;
				savedGO = true;
				colliding = true;
			}

		}

	}

	void OnTriggerExit2D(Collider2D other)
	{
		//Debug.Log ("Esco01");
		if (savedGO) 
		{
			if (other.gameObject == windGO)
			{
				//Debug.Log ("Esco");
				colliding = false;
				active = false;
				animator.SetBool("Active", false);
			}
			
		}
		
	}
}

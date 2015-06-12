using UnityEngine;
using System.Collections;

public class SimpleCloudMove : MonoBehaviour {

	bool started = false;
	bool comeBack = false;
	public Transform destinationPosition;

	Vector3 originPosition;

	public float speed = 1.0f;
	
	// Use this for initialization
	void Start () {
		originPosition = transform.parent.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (started)
		{
			transform.parent.position= Vector3.MoveTowards(transform.parent.position, destinationPosition.transform.position, speed*Time.deltaTime);
			if (transform.parent.position == destinationPosition.transform.position)
				started = false;
		}

		if (comeBack)
		{
			transform.parent.position= Vector3.MoveTowards(transform.parent.position, originPosition, speed*Time.deltaTime);
			if (transform.parent.position == originPosition)
				comeBack = false;
		}
			
	}
	
	void OnTriggerStay2D(Collider2D other)
	{
		if (!started)
		{
			AreaEffector2D areaEffector = other.gameObject.GetComponent<AreaEffector2D>();
			if (areaEffector != null)
			{
				if ((areaEffector.forceAngle < 90 && areaEffector.forceAngle > -90) || (areaEffector.forceAngle > 270) || (areaEffector.forceAngle < -270))
				{
					Debug.Log ("nuvola partita da sinistra");
					started = true;
				}else{
					Debug.Log ("nuvola partita da destra");
					comeBack = true;
				}
				
			}
		}
	}

	//void OnCollisionEnter

}

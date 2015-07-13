using UnityEngine;
using System.Collections;

public class SailsManagement : MonoBehaviour {

	public bool active = false;
	GameObject activator;

	void Start () {
	
	}

	void Update () {
		if ((activator == null) || (activator != null && activator.activeInHierarchy == false))
			active = false;
	}

	void OnTriggerStay2D(Collider2D other)
	{
		AreaEffector2D areaEffector = other.gameObject.GetComponent<AreaEffector2D>();
		if (areaEffector != null && active == false)
		{
			if ((areaEffector.forceAngle < 90 && areaEffector.forceAngle > -90) || (areaEffector.forceAngle > 270) || (areaEffector.forceAngle < -270))
			{
				Debug.Log ("vela attivata");
				active = true;
				activator = other.gameObject;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		AreaEffector2D areaEffector = other.gameObject.GetComponent<AreaEffector2D>();
		if (areaEffector != null)
		{
				active = false;
		}
	}
}

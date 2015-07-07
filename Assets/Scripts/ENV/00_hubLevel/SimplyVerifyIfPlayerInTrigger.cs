using UnityEngine;
using System.Collections;

public class SimplyVerifyIfPlayerInTrigger : MonoBehaviour {

	bool playerColliding = false;
	public bool PlayerColliding
	{
		get{return playerColliding;}
	}


	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag.ToString() == "Player")
			playerColliding = true;
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag.ToString() == "Player")
			playerColliding = false;
	}
}

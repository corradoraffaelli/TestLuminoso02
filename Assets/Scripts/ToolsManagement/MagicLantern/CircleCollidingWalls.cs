using UnityEngine;
using System.Collections;

/// <summary>
/// Si occupa di gestire i muri dove la lanterna non può proiettare. INUTILIZZATO.
/// </summary>

// Corrado
public class CircleCollidingWalls : MonoBehaviour {

	GameObject magicLanternLogicOBJ;
	MagicLantern magicLanternLogic;

	// Use this for initialization
	void Start () {
		magicLanternLogicOBJ = GameObject.FindGameObjectWithTag ("MagicLanternLogic");
		//if (magicLanternLogicOBJ != null)
		//	magicLanternLogic = magicLanternLogicOBJ.GetComponent<MagicLantern> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.tag == "ProjectionWall" && magicLanternLogicOBJ != null) {
			magicLanternLogicOBJ.SendMessage("setCollidingBadWall", true);
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "ProjectionWall" && magicLanternLogicOBJ != null) {
			magicLanternLogicOBJ.SendMessage("setCollidingBadWall", false);
		}
	}
}

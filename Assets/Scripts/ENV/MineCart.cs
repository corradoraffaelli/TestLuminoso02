using UnityEngine;
using System.Collections;

public class MineCart : MonoBehaviour {

	GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void setPlayerParent(Transform parentTransform)
	{
		player.transform.parent = parentTransform;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject == player)
			setPlayerParent(transform);
	}
}

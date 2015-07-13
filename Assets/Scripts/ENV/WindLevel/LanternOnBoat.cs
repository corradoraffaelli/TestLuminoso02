using UnityEngine;
using System.Collections;

public class LanternOnBoat : MonoBehaviour {

	bool playerOnTrigger = false;
	bool lanternOnBoat = false;
	public bool IsLanternOnBoat
	{
		get {return lanternOnBoat;}
	}

	MagicLantern lanternLogic;

	public MagicLantern.lanternState actualState;
	public MagicLantern.lanternState oldState;

	void Start () {
		lanternLogic = GeneralFinder.magicLanternLogic;
	}

	void Update () {
		actualState = lanternLogic.actualState;
		if (actualState == MagicLantern.lanternState.Left && oldState != actualState)
		{
			if (playerOnTrigger)
				lanternOnBoat = true;
			else
				lanternOnBoat = false;

		}
		oldState = actualState;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
			playerOnTrigger = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
			playerOnTrigger = false;
	}
}

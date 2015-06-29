using UnityEngine;
using System.Collections;

public class UnlockedLevelsController : MonoBehaviour {

	[System.Serializable]
	public class LevelInfo{
		string name;
		int index;
		bool unlocked = false;
		bool wasUnlocked = false;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

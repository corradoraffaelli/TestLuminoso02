using UnityEngine;
using System.Collections;

public class levelChanger : MonoBehaviour {

	public bool reloadThisLevel = true;
	public string sceneName;
	public int levelToUnlock = 2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void unlockLevel()
	{
		if (GeneralFinder.informativeManager.getActualCollectiblesSection(levelToUnlock) != null)
			GeneralFinder.informativeManager.getActualCollectiblesSection(levelToUnlock).locked = false;
		if (GeneralFinder.informativeManager.getActualFragmentSection(levelToUnlock) != null)
			GeneralFinder.informativeManager.getActualFragmentSection(levelToUnlock).locked = false;
		if (GeneralFinder.informativeManager.getActualFunFactsSection(levelToUnlock) != null)
			GeneralFinder.informativeManager.getActualFunFactsSection(levelToUnlock).locked = false;
	}

	public void changeScene()
	{
		if (reloadThisLevel)
			Application.LoadLevel (Application.loadedLevel);
		else
			if (sceneName != "")
				Application.LoadLevel (sceneName);
	}

	public void InteractingMethod()
	{
		unlockLevel();

		changeScene ();
	}
}

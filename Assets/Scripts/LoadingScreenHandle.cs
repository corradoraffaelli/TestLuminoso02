using UnityEngine;
using System.Collections;

public class LoadingScreenHandle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void showLoadingScreen() {

		SubContent subC = c_getRandomSubContent ();

		if (subC != null) {



		}
		else {



		}

	}

	public SubContent c_getRandomSubContent(){

		//InformativeManager infM = GeneralFinder.informativeManager;

		InformativeSection [] sections = GeneralFinder.informativeManager.getUnlockedSections ();

		if (sections != null) {

			int randomSectionIndex = Random.Range (0, sections.Length);

			int randomContentIndex = 0;

			while (true) {

				randomContentIndex = Random.Range (0, sections [randomSectionIndex].contents.Length);

				if (!sections [randomSectionIndex].contents [randomContentIndex].lockedContent) {

					break;

				}

			}

			int randomSubContentIndex = Random.Range (0, sections [randomSectionIndex].contents[randomContentIndex].subContents.Length);

			return sections [randomSectionIndex].contents[randomContentIndex].subContents[randomSubContentIndex];



		}
		else {
			return null;
		}

	}



}

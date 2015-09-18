using UnityEngine;
using System.Collections;
using System;

public class TestInformativeManager : MonoBehaviour {

	int activeSection = -1;
	int activeContent = -1;
	int activeSubContent = -1;
	//int activeImage = -1;

	public float sogliaConteggio = 2.0f;

	DateTime startContentTime;
	//DateTime startImageTime;
	DateTime startSubContentTime;


	//TODO: eventuali controlli su inattività utente

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (!PlayStatusTracker.inPlay && (GeneralFinder.menuManager.getStatusMenu() || GeneralFinder.informativeManager.invokeWithoutMenu )) {

			//Debug.Log("test inform");

			if(GeneralFinder.menuManager.isInformativeCanvasActive() || GeneralFinder.informativeManager.invokeWithoutMenu) {

				int newActSect = GeneralFinder.informativeManager.ActiveSection;
				int newActCont = GeneralFinder.informativeManager.sections[newActSect].activeContent;
				int newActSubCont = GeneralFinder.informativeManager.sections[GeneralFinder.informativeManager.ActiveSection].contents[newActCont].activeSubContentIndex;


				if(activeSection!= newActSect || activeContent!= newActCont) {

					changeContentTimer(newActSect, newActCont, newActSubCont);

				}
				else {

					if(activeSection== newActSect && 
					   activeContent== newActCont &&
					   activeSubContent != newActSubCont ) {

						//TODO:
						//controllo su immagini
						//changeImageTimer(newActImg);

						changeSubContentTimer(newActSubCont);
					}


				}

				//TODO:
				//tutto uguale... non faccio nulla?
			}
			else {

				if(activeSection != -1) {
					stopRecord(activeSection, activeContent, activeSubContent);
					resetIndexes();
				}

			}

		}
		else {

			//TODO : nulla qui, no?
			//metto ulteriore controllo, ma non dovrebbe servire
			if(activeSection != -1) {
				stopRecord(activeSection, activeContent, activeSubContent);
				resetIndexes();
			}

		}

	}

	void changeSubContentTimer(int _newActiveSubContent) {
		
		stopRecordSubContent (activeSection, activeContent, activeSubContent);
		
		startRecordSubContent ();
		
		changeIndexes (activeSection, activeContent, _newActiveSubContent);
		
	}


	void changeContentTimer(int _newActiveSection, int _newActiveContent, int _newActiveSubContent) {

		if (activeSection != -1 && activeContent != -1) {

			stopRecord(activeSection, activeContent, activeSubContent);

			startRecord();


		}
		else {
			//first change
			startRecord();

		}

		changeIndexes (_newActiveSection, _newActiveContent, _newActiveSubContent);

	}

	void stopRecord(int _activeSection, int _activeContent, int _activeSubContent) {

		stopRecordContent (_activeSection, _activeContent, _activeSubContent);

		stopRecordSubContent (_activeSection, _activeContent, _activeSubContent);

	}

	void stopRecordContent(int _activeSection, int _activeContent, int _activeImage) {

		TimeSpan timeSp = DateTime.Now - startContentTime;
		
		float t = (float) timeSp.TotalSeconds;

		if (t > sogliaConteggio) {

			GeneralFinder.informativeManager.sections[_activeSection].contents[_activeContent].contentViewingTimer += t;
			GeneralFinder.informativeManager.sections[_activeSection].contents[_activeContent].contentViewsCounter ++;
		
		}

	}

	void stopRecordSubContent(int _activeSection, int _activeContent, int _activeSubContent) {
		
		TimeSpan timeSp = DateTime.Now - startSubContentTime;
		float t2 = (float) timeSp.TotalSeconds;

		if (!GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].locked) {

			GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].subContents [_activeSubContent].subContentViewingTimer += t2;
			GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].subContents [_activeSubContent].subContentViewsCounter ++;
		
		}
	
	}

	void startRecord() {

		startRecordContent ();

		startRecordSubContent ();

	}

	void startRecordContent() {

		startContentTime = DateTime.Now;

	}

	void startRecordSubContent() {
		
		startSubContentTime = DateTime.Now;
		
	}

	void changeIndexes(int _newActiveSection, int _newActiveContent, int _newActiveSubContent) {
		
		activeSection = _newActiveSection;
		activeContent = _newActiveContent;
		activeSubContent = _newActiveSubContent;
	}

	void resetIndexes() {
		
		activeSection = -1;
		activeContent = -1;
		activeSubContent = -1;
		
	}

	public void c_setShowWhenUnlocked(int sect, int cont) {

		GeneralFinder.informativeManager.sections [sect].contents [cont].shownWhenUnlocked = true;

	}

}

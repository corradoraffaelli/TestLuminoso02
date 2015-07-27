using UnityEngine;
using System.Collections;
using System;

public class TestInformativeManager : MonoBehaviour {

	int activeSection = -1;
	int activeContent = -1;
	int activeImage = -1;

	public float sogliaConteggio = 2.0f;

	DateTime startContentTime;
	DateTime startImageTime;

	//TODO: eventuali controlli su inattività utente

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (!PlayStatusTracker.inPlay && (GeneralFinder.menuManager.getStatusMenu() || GeneralFinder.informativeManager.invokeWithoutMenu )) {

			Debug.Log("test inform");

			if(GeneralFinder.menuManager.isInformativeCanvasActive() || GeneralFinder.informativeManager.invokeWithoutMenu) {

				int newActSect = GeneralFinder.informativeManager.ActiveSection;
				int newActCont = GeneralFinder.informativeManager.sections[newActSect].activeContent;
				int newActImg = GeneralFinder.informativeManager.sections[GeneralFinder.informativeManager.ActiveSection].contents[newActCont].mainImageIndex;


				if(activeSection!= newActSect || activeContent!= newActCont) {

					changeContentTimer(newActSect, newActCont, newActImg);

				}
				else {

					if(activeSection== newActSect && 
					   activeContent== newActCont &&
					   activeImage != newActImg ) {

						//TODO:
						//controllo su immagini
						changeImageTimer(newActImg);
					}


				}

				//TODO:
				//tutto uguale... non faccio nulla?
			}
			else {

				if(activeSection != -1) {
					stopRecord(activeSection, activeContent, activeImage);
					resetIndexes();
				}

			}

		}
		else {

			//TODO : nulla qui, no?
			//metto ulteriore controllo, ma non dovrebbe servire
			if(activeSection != -1) {
				stopRecord(activeSection, activeContent, activeImage);
				resetIndexes();
			}

		}

	}

	void changeImageTimer(int _newActiveImage) {

		stopRecordImage (activeSection, activeContent, activeImage);

		startRecordImage ();

		changeIndexes (activeSection, activeContent, _newActiveImage);

	}

	void changeContentTimer(int _newActiveSection, int _newActiveContent, int _newActiveImage) {

		if (activeSection != -1 && activeContent != -1) {

			stopRecord(activeSection, activeContent, activeImage);

			startRecord();


		}
		else {
			//first change
			startRecord();

		}

		changeIndexes (_newActiveSection, _newActiveContent, _newActiveImage);

	}

	void stopRecord(int _activeSection, int _activeContent, int _activeImage) {

		stopRecordContent (_activeSection, _activeContent, _activeImage);

		stopRecordImage (_activeSection, _activeContent, _activeImage);

	}

	void stopRecordContent(int _activeSection, int _activeContent, int _activeImage) {

		TimeSpan timeSp = DateTime.Now - startContentTime;
		
		float t = (float) timeSp.TotalSeconds;

		if (t > sogliaConteggio) {

			GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].timerViewsContent += t;
		
			GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].numberViewsContent++;
		
		}

	}

	void stopRecordImage(int _activeSection, int _activeContent, int _activeImage) {

		TimeSpan timeSp = DateTime.Now - startImageTime;
		float t2 = (float) timeSp.TotalSeconds;

		if (t2 > sogliaConteggio) {

			if (GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].timerViewsImages == null) {
			
				int len = GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].mainImages.Length;
			
				GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].timerViewsImages = new float[len];
				GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].numberViewsImages = new int[len];
			
			}
		
			if (GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].timerViewsImages.Length == 0) {
			
				int len = GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].mainImages.Length;
			
				GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].timerViewsImages = new float[len];
				GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].numberViewsImages = new int[len];
			
			}
		
			GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].timerViewsImages [_activeImage] += t2;
			GeneralFinder.informativeManager.sections [_activeSection].contents [_activeContent].numberViewsImages [_activeImage]++;
		}
	}

	void startRecord() {

		startRecordContent ();

		startRecordImage ();

	}

	void startRecordContent() {

		startContentTime = DateTime.Now;

	}

	void startRecordImage() {

		startImageTime = DateTime.Now;

	}

	void changeIndexes(int _newActiveSection, int _newActiveContent, int _newActiveImage) {

		activeSection = _newActiveSection;
		activeContent = _newActiveContent;
		activeImage = _newActiveImage;
	}

	void resetIndexes() {

		activeSection = -1;
		activeContent = -1;
		activeImage = -1;

	}

	public void c_setShowWhenUnlocked(int sect, int cont) {

		GeneralFinder.informativeManager.sections [sect].contents [cont].shownWhenUnlocked = true;

	}

}

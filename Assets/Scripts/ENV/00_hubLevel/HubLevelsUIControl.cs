using UnityEngine;
using System.Collections;

public class HubLevelsUIControl : MonoBehaviour {

	UnlockedLevelControl levelControl;
	//InteragibileObject[] interagibleScripts;
	int actualColliding = -1;
	bool changedColliding = false;
	Sprite[] rightSprites;
	Sprite[] leftSprites;

	void Start () {
		levelControl = GetComponent<UnlockedLevelControl>();
	}

	void Update () {
		actualColliding = verifyIfColliding();
		handleLateralUI();
	}

	int verifyIfColliding()
	{
		int levelColliding = -1;
		for (int i = 0; i < levelControl.unlockedLevelElements.Length; i++)
		{
			if (levelControl.unlockedLevelElements[i] != null && levelControl.unlockedLevelElements[i].interagibleScript != null)
			{
				if (levelControl.unlockedLevelElements[i].interagibleScript.playerColliding)
				{
					levelColliding = levelControl.unlockedLevelElements[i].levelNumber;
					if (levelColliding != actualColliding)
						changedColliding = true;
					else
						changedColliding = false;
					return levelColliding;
				}
			}
		}
		if (levelColliding != actualColliding)
			changedColliding = true;
		else
			changedColliding = false;
		return levelColliding;
	}

	void handleLateralUI()
	{
		if (changedColliding)
		{
			//se non sto collidendo con nessuna porta, nascondo le sezioni laterali
			if (actualColliding == -1)
			{
				GeneralFinder.playingUILateral.showIcons(PlayingUILateral.UIPosition.Left, false);
				GeneralFinder.playingUILateral.showIcons(PlayingUILateral.UIPosition.Right, false);
			}
			else
			{
				InformativeSection objectSection = GeneralFinder.informativeManager.getActualCollectiblesSection(actualColliding);
				InformativeSection fragmentSection = GeneralFinder.informativeManager.getActualFragmentSection(actualColliding);
				//riempio l'insieme di sprites che dovranno riempire la sezione destra
				if (objectSection != null)
				{
					rightSprites = new Sprite[objectSection.contents.Length];
					for (int i = 0; i < objectSection.contents.Length; i++)
					{
						if (objectSection.contents[i] != null)
						{
							if (objectSection.contents[i].locked)
								rightSprites[i] = objectSection.contents[i].iconLock;
							else
								rightSprites[i] = objectSection.contents[i].iconUnlock;
						}
					}
				}
				//riempio l'insieme di sprites che dovranno riempire la sezione sinistra
				if (fragmentSection != null)
				{
					leftSprites = new Sprite[fragmentSection.contents.Length];
					for (int i = 0; i < fragmentSection.contents.Length; i++)
					{
						if (fragmentSection.contents[i] != null)
						{
							if (fragmentSection.contents[i].locked)
								leftSprites[i] = fragmentSection.contents[i].iconLock;
							else
								leftSprites[i] = fragmentSection.contents[i].iconUnlock;
						}
					}
				}
				//mostro
				if (rightSprites != null && rightSprites.Length != 0)
				{
					GeneralFinder.playingUILateral.setSprites(rightSprites, PlayingUILateral.UIPosition.Right);
					GeneralFinder.playingUILateral.updateSpritesOnScreen(PlayingUILateral.UIPosition.Right);
					GeneralFinder.playingUILateral.showIcons(PlayingUILateral.UIPosition.Right);
				}
				if (leftSprites != null && leftSprites.Length != 0)
				{
					GeneralFinder.playingUILateral.setSprites(leftSprites, PlayingUILateral.UIPosition.Left);
					GeneralFinder.playingUILateral.updateSpritesOnScreen(PlayingUILateral.UIPosition.Left);
					GeneralFinder.playingUILateral.showIcons(PlayingUILateral.UIPosition.Left);
				}
			}
		}
	}	
}

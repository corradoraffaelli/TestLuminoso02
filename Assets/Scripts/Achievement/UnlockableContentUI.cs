using UnityEngine;
using System.Collections;

public class UnlockableContentUI : MonoBehaviour {

	public float timeToHideLateral = 4.0f;
	public float timeToPulse = 4.0f;

	public InformativeContent[] rightContents;
	public Sprite[] rightSprites;
	public Sprite[] leftSprites;

	public InformativeSection actualSection;

	bool showRight = false;
	float showRightTime;
	bool showLeft = false;
	float showLeftTime;

	void Start () {
		/*
		actualSection = findActualSection();
		updateContentSprites(actualSection.title);
		*/
	}

	void Update () {
		/*
		hideLateralManager();
		*/
	}

	public void unlockFragment(string id)
	{
		Debug.Log ("ho sbloccato il frammento "+id);

		//se è un frammento devo
		//1. aggiornare l'array di sprites a sinistra
		//2. mostrare le sprites a sinistra aggiornate
		//3. eseguire l'animazione dell'oggetto raccolto

		/*
		GeneralFinder.playingUILateral.showIcons(PlayingUILateral.UIPosition.Left, true);
		showLeft = true;
		showLeftTime = Time.time;
		*/
	}

	public void unlockContent(string name)
	{
		Debug.Log ("ho sbloccato l'oggetto "+name);

		//se è un collezionabile devo
		//1. aggiornare l'array di sprites a destra
		//2. mostrare le sprites a destra aggiornate
		//3. eseguire l'animazione dell'oggetto raccolto

		/*
		//1.
		updateContentSprites(section);

		//2.
		GeneralFinder.playingUILateral.showIcons(PlayingUILateral.UIPosition.Right, true);
		showRight = true;
		showRightTime = Time.time;

		//3.
		PickingObjectGraphic pick = gameObject.AddComponent<PickingObjectGraphic>();
		pick.setVariables(findContent(section, name).iconUnlock, PlayingUILateral.UIPosition.Right, findContentIndex(name));

		//in entrambi i casi devo
		//1. mostrare l'icona del libro lampeggiante per tot secondi

		//if (findContent(section, name).

		*/
	}

	public void unlockFact(string name)
	{
		Debug.Log ("ho sbloccato il fun fact "+name);
	}

	void hideLateralManager()
	{
		if (showRight)
		{
			if ((Time.time - showRightTime) > timeToHideLateral)
			{
				GeneralFinder.playingUILateral.showIcons(PlayingUILateral.UIPosition.Right, false);
				showRight = false;
			}
		}

		if (showLeft)
		{
			if ((Time.time - showLeftTime) > timeToHideLateral)
			{
				GeneralFinder.playingUILateral.showIcons(PlayingUILateral.UIPosition.Left, false);
				showLeft = false;
			}
		}
	}

	InformativeContent findContent(string section, string name)
	{
		for (int i = 0; i < GeneralFinder.informativeManager.sections.Length; i++)
		{
			if (GeneralFinder.informativeManager.sections[i].title == section)
			{

				for (int j = 0; j < GeneralFinder.informativeManager.sections[i].contents.Length; j++)
				{
					if (GeneralFinder.informativeManager.sections[i].contents[j].name == name)
						return GeneralFinder.informativeManager.sections[i].contents[j];
				}
				break;
			}
		}
		return null;
	}

	int countContent(string section)
	{
		int contentNumber = 0;

		for (int i = 0; i < GeneralFinder.informativeManager.sections.Length; i++)
		{
			if (GeneralFinder.informativeManager.sections[i].title == section)
			{
				
				for (int j = 0; j < GeneralFinder.informativeManager.sections[i].contents.Length; j++)
				{
					if (!GeneralFinder.informativeManager.sections[i].contents[j].funFact)
						contentNumber++;
				}
				break;
			}
		}

		return contentNumber;
	}
	
	void updateContentSprites(string section)
	{
		rightSprites = new Sprite[countContent(section)];
		rightContents = new InformativeContent[countContent(section)];
		int actualIndex = 0;
		for (int i = 0; i < GeneralFinder.informativeManager.sections.Length; i++)
		{
			if (GeneralFinder.informativeManager.sections[i].title == section)
			{
				for (int j = 0; j < GeneralFinder.informativeManager.sections[i].contents.Length; j++)
				{
					if (!GeneralFinder.informativeManager.sections[i].contents[j].funFact)
					{
						rightContents[actualIndex] = GeneralFinder.informativeManager.sections[i].contents[j];
						if (GeneralFinder.informativeManager.sections[i].contents[j].locked)
							rightSprites[actualIndex] = GeneralFinder.informativeManager.sections[i].contents[j].iconLock;
						else
							rightSprites[actualIndex] = GeneralFinder.informativeManager.sections[i].contents[j].iconUnlock;

						actualIndex++;
					}
				}
				break;
			}
		}
	}

	//trovo l'indice del content attuale rispetto al numero totale (senza fun facts)
	//funzione che non servirebbe se i fun facts fossero gestiti separatamente
	int findContentIndex(string name)
	{
		if (rightContents != null && rightContents.Length != 0)
		{
			for (int i = 0; i < rightContents.Length; i++)
			{
				if (rightContents[i].name == name)
					return i;
			}
		}
		return 0;
	}

	
	void initialSetup()
	{
		
	}

	//per aggiornare la prima volta le sprites, ho bisogno di sapere quale sezione devo considerare
	//sfrutto perciò la prima che trovo senza gameObject nulli
	InformativeSection findActualSection()
	{
		if (GeneralFinder.informativeManager.sections != null)
		{
			for (int i = 0; i < GeneralFinder.informativeManager.sections.Length; i++)
			{
				if (GeneralFinder.informativeManager.sections[i] != null && GeneralFinder.informativeManager.sections[i].contents != null)
				{
					for (int j = 0; j< GeneralFinder.informativeManager.sections[i].contents.Length; j++)
					{
						if (GeneralFinder.informativeManager.sections[i].contents[j] != null && 
						    GeneralFinder.informativeManager.sections[i].contents[j].unlockerObject != null)
							return GeneralFinder.informativeManager.sections[i];
					}
				}
			}
		}
		return null;
	}
}

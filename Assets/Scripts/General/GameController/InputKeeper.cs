using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class InputKeeper : MonoBehaviour {



	/*
	 * --------------- BOTTONI ---------------
	 */

	//struttura che contiene le info riguardo un particolare bottone, ed il suo stato
	[System.Serializable]
	public class InputButton{
		public string name;

		[HideInInspector]
		public bool buttonStay = false;
		[HideInInspector]
		public bool buttonUp = false;
		[HideInInspector]
		public bool buttonDown = false;

		[HideInInspector]
		public bool needToResetUp = false;
		[HideInInspector]
		public bool needToResetDown = false;
	}

	[SerializeField]
	public InputButton[] inputButtons;

	//struttura che contiene lo stato di un bottone, al momento di un cambiamento di stato, ed il relativo tempo
	[System.Serializable]
	class ChangedButtons{
		public float changingTime;
		public InputButton inputButton;
		public ChangedButtons(float inputTime, InputButton inputButton){
			this.changingTime = inputTime;
			this.inputButton = inputButton;
		}
	}

	//[SerializeField]
	List<ChangedButtons> changedButtonsList = new List<ChangedButtons>();

	/*
	 * ----------------- ASSI -------------------
	 */

	[System.Serializable]
	public class InputAxis{
		public string name;
		
		//[HideInInspector]
		public float getAxisNormal = 0.0f;
		[HideInInspector]
		public float getAxisRaw = 0.0f;
	}
	
	[SerializeField]
	public InputAxis[] inputAxis;

	//struttura che contiene lo stato di un asse, al momento di un cambiamento di stato, ed il relativo tempo
	[System.Serializable]
	class ChangedAxis{
		public float changingTime;
		public InputAxis inputAxis;
		public ChangedAxis(float inputTime, InputAxis inputAxis){
			this.changingTime = inputTime;
			this.inputAxis = inputAxis;
		}
	}
	
	//[SerializeField]
	List<ChangedAxis> changedAxisList = new List<ChangedAxis>();

	/*
	 * ----------------- MOUSE -------------------
	 */

	//[System.Serializable]
	//[SerializeField]
	[System.Serializable]
	public class InputMousePosition{
		/*
		public InputMousePosition(float inputX, float inputY, float inputZ){
			this.inputX = inputX;
			this.inputY = inputY;
			this.inputZ = inputZ;
		}
		*/
		public float inputX;
		public float inputY;
		public float inputZ;
	}

	InputMousePosition mousePosition = new InputMousePosition ();
	
	//[SerializeField]
	//public InputAxis[] inputAxis;
	
	//struttura che contiene lo stato di un asse, al momento di un cambiamento di stato, ed il relativo tempo
	[System.Serializable]
	class ChangedMouse{
		public float changingTime;
		public InputMousePosition newMousePosition;
		public ChangedMouse(float inputTime, InputMousePosition inputMousePosition){
			this.changingTime = inputTime;
			this.newMousePosition = inputMousePosition;
		}
	}
	
	//[SerializeField]
	List<ChangedMouse> changedMouseList = new List<ChangedMouse>();

	/*
	 * ----------------- OGGETTI DA MONITORARE -------------------
	 */

	[System.Serializable]
	public class GameObjectToControl{
		public string tag;
		public Vect3 position = new Vect3();
		public Vect3 scale = new Vect3();
		public Quat rotation = new Quat();
	}

	[SerializeField]
	List<GameObjectToControl> gameObjectsToControl = new List<GameObjectToControl>();

	[System.Serializable]
	class ChangedObjects{
		public float changingTime;
		public GameObjectToControl gameObjectToControl;
		public ChangedObjects(float inputTime, GameObjectToControl inputGameObjectToControl){
			this.changingTime = inputTime;
			this.gameObjectToControl = inputGameObjectToControl;
		}
	}
	
	[SerializeField]
	List<ChangedObjects> changedObjectsList = new List<ChangedObjects>();
	
	//Classi inutili che non dovrebbero esistere, se solo i vector3 e quaternion fossero serializzabili
	[System.Serializable]
	public class Vect3{
		public float x;
		public float y;
		public float z;
		public Vect3(){
			this.x = 0; this.y = 0; this.z = 0;
		}
		public Vect3(float inpX, float inpY, float inpZ){
			this.x = inpX; this.y = inpY; this.z = inpZ;
		}
	}

	[System.Serializable]
	public class Quat{
		public float x;
		public float y;
		public float z;
		public float w;
		public Quat(){
			this.x = 0; this.y = 0; this.z = 0; this.w = 0;
		}
		public Quat(float inpX, float inpY, float inpZ, float inpW){
			this.x = inpX; this.y = inpY; this.z = inpZ; this.w = inpW;
		}
	}

	/*
	 * ----------------- ALTRE VARIABILI -------------------
	 */

	public enum LoadSaveState{None, Load, Save}
	public LoadSaveState loadSaveState = LoadSaveState.None;

	public string savedButtonFile = "savedInputButtons";
	public string savedAxisFile = "savedInputAxis";
	public string savedMouseFile = "savedInputMouse";
	public string savedObjectsFile = "savedObjects";
	public string savedFileExtention = ".dat";
	int savedInputFileIndex = 0;
	public int indexToLoad = 0;

	[Range(0.0f, 10.0f)]
	public float correctionTime = 1.5f;
	float lastCorrectionTime = 0.0f;

	public bool adjustPositions = false;
	public bool debugPath = true;

	// Use this for initialization
	void Awake () {
		if (loadSaveState == LoadSaveState.Load)
			load ();
	}

	/*
	void OnDestroy() {
		if (loadSaveState == LoadSaveState.Save)
			save ();
	}
	*/

	void Start(){
		updateFileIndex();

		if (debugPath)
			Debug.Log (Application.persistentDataPath);
	}

	// Update is called once per frame
	void Update () {
		//creo il bool per vedere se un bottone ha cambiato il suo stato
		bool[] changedButtonsBool = new bool[inputButtons.Length];
		//creo il bool per vedere se un asse ha cambiato il suo stato
		bool[] changedAxisBool = new bool[inputAxis.Length];
		//creo il bool per vedere se un asse ha cambiato il suo stato
		bool changedMouseBool = false;

		if (loadSaveState == LoadSaveState.Save) {
			//se un bottone cambia il suo stato, metto il relativo bool a true
			for (int i = 0; i<inputButtons.Length; i++)
				changedButtonsBool [i] = verifyIfButtonChanged (inputButtons [i].name);

			//se un asse cambia il suo stato, metto il relativo bool a true
			for (int i = 0; i<inputAxis.Length; i++)
				changedAxisBool [i] = verifyIfAxisChanged (inputAxis [i].name);

			//Debug.Log (verifyIfMouseChanged());
			if (Input.mousePresent)
				changedMouseBool = verifyIfMouseChanged();


		}


		if (loadSaveState != LoadSaveState.Load) {
			//funzioni per gestire l'input effettivo dell'utente, non quello caricato
			setButtons ();
			setAxis ();
			if (Input.mousePresent)
				setMouse();
		}

		//se un bottone risulta aver cambiato il suo stato, dal proprio bool, lo salvo nella lista di bottoni cambiati, insieme al tempo di cambiamento
		if (loadSaveState == LoadSaveState.Save) {
			for (int i = 0; i<changedButtonsBool.Length; i++) {
				if (changedButtonsBool[i] == true)
				{
					InputButton tempInputButton = new InputButton();
					tempInputButton.name = inputButtons[i].name;
					tempInputButton.buttonDown = inputButtons[i].buttonDown;
					tempInputButton.buttonUp = inputButtons[i].buttonUp;
					tempInputButton.buttonStay = inputButtons[i].buttonStay;
					
					ChangedButtons tempChanged = new ChangedButtons(Time.time,tempInputButton);
					changedButtonsList.Add(tempChanged);
				}
			}

			for (int i = 0; i<changedAxisBool.Length; i++) {
				if (changedAxisBool[i] == true)
				{
					InputAxis tempInputAxis = new InputAxis();
					tempInputAxis.name = inputAxis[i].name;
					tempInputAxis.getAxisNormal = inputAxis[i].getAxisNormal;
					tempInputAxis.getAxisRaw = inputAxis[i].getAxisRaw;
					
					ChangedAxis tempChanged = new ChangedAxis(Time.time,tempInputAxis);
					changedAxisList.Add(tempChanged);
				}
			}

			if (Input.mousePresent && changedMouseBool)
			{
				Vector3 tempInputMouse = Input.mousePosition;

				//sarebbe meglio salvare le posizioni nel worldSpace, altrimenti sono in pixel, perciò relative alla risoluzione
				InputMousePosition tempInputMousePosition = new InputMousePosition();
				/*
				tempInputMousePosition.inputX = Input.mousePosition.x;
				tempInputMousePosition.inputY = Input.mousePosition.y;
				tempInputMousePosition.inputZ = Input.mousePosition.z;
				*/

				tempInputMousePosition.inputX = Camera.main.ScreenToWorldPoint(tempInputMouse).x;
				tempInputMousePosition.inputY = Camera.main.ScreenToWorldPoint(tempInputMouse).y;
				tempInputMousePosition.inputZ = Camera.main.ScreenToWorldPoint(tempInputMouse).z;

				ChangedMouse tempChanged = new ChangedMouse(Time.time, tempInputMousePosition);
				changedMouseList.Add (tempChanged);
			}

			/*
			//salvataggio delle variabili del component transform degli oggetti da "correggere"
			if ((Mathf.Abs (Time.time - lastCorrectionTime)) > correctionTime)
			{
				lastCorrectionTime = Time.time;
				for (int i = 0; i<gameObjectsToControl.Count; i++)
				{
					if (gameObjectsToControl[i] != null && gameObjectsToControl[i].tag != "")
					{
						GameObjectToControl tempGOTC = new GameObjectToControl();

						GameObject tempGO = GameObject.FindGameObjectWithTag(gameObjectsToControl[i].tag);

						tempGOTC.tag = gameObjectsToControl[i].tag;
						//tempGOTC.gameObject = gameObjectsToControl[i].gameObject;
						Vector3 tempPos = tempGO.transform.position;
						tempGOTC.position = new Vect3(tempPos.x, tempPos.y, tempPos.z);
						Vector3 tempScale = tempGO.transform.localScale;
						tempGOTC.scale = new Vect3(tempScale.x, tempScale.y, tempScale.z);
						Quaternion tempQuaternion = tempGO.transform.rotation;
						tempGOTC.rotation = new Quat(tempQuaternion.x, tempQuaternion.y, tempQuaternion.z, tempQuaternion.w);

						ChangedObjects tempChanged = new ChangedObjects(Time.time, tempGOTC);
						changedObjectsList.Add(tempChanged);
					}
				}
			}
			*/


		}

		//se carico l'input da file, allora setto le relative variabili in modo che sembri input effettivo
		if (loadSaveState == LoadSaveState.Load) {
			setLoadedButtons ();
			setLoadedAxis();
			setLoadedMouse();
		}

	}

	//nel LateUpdate correggo le posizioni ed eventuali derive degli oggetti specificati
	void LateUpdate()
	{
		//se un evento di un bottone è stato consumato, lo resetto
		resetButtons();

		if (loadSaveState == LoadSaveState.Save) {
			if ((Mathf.Abs (Time.time - lastCorrectionTime)) > correctionTime)
			{
				lastCorrectionTime = Time.time;
				for (int i = 0; i<gameObjectsToControl.Count; i++)
				{
					if (gameObjectsToControl[i] != null && gameObjectsToControl[i].tag != "")
					{
						GameObjectToControl tempGOTC = new GameObjectToControl();
						
						GameObject tempGO = GameObject.FindGameObjectWithTag(gameObjectsToControl[i].tag);
						
						tempGOTC.tag = gameObjectsToControl[i].tag;
						//tempGOTC.gameObject = gameObjectsToControl[i].gameObject;
						Vector3 tempPos = tempGO.transform.position;
						tempGOTC.position = new Vect3(tempPos.x, tempPos.y, tempPos.z);
						Vector3 tempScale = tempGO.transform.localScale;
						tempGOTC.scale = new Vect3(tempScale.x, tempScale.y, tempScale.z);
						Quaternion tempQuaternion = tempGO.transform.rotation;
						tempGOTC.rotation = new Quat(tempQuaternion.x, tempQuaternion.y, tempQuaternion.z, tempQuaternion.w);
						
						ChangedObjects tempChanged = new ChangedObjects(Time.time, tempGOTC);
						changedObjectsList.Add(tempChanged);
					}
				}
			}
		}

		//List<string> stringList = new List<string>();
		if (loadSaveState == LoadSaveState.Load && adjustPositions) {
			for (int i = 0; i< changedObjectsList.Count; i++)
			{
				if (changedObjectsList [i] != null && changedObjectsList[i].gameObjectToControl.tag != "")
				{
					//se ho passato il tempo
					if (changedObjectsList [i].changingTime < Time.time)
					{
						GameObject tempGO = GameObject.FindGameObjectWithTag(changedObjectsList[i].gameObjectToControl.tag);
						
						Vect3 tempPos = changedObjectsList[i].gameObjectToControl.position;
						Vect3 tempScale = changedObjectsList[i].gameObjectToControl.scale;
						Quat tempRot = changedObjectsList[i].gameObjectToControl.rotation;
						
						tempGO.transform.position = new Vector3(tempPos.x, tempPos.y, tempPos.z);
						tempGO.transform.localScale = new Vector3(tempScale.x, tempScale.y, tempScale.z);
						tempGO.transform.rotation = new Quaternion(tempRot.x, tempRot.y, tempRot.z, tempRot.w);

						changedObjectsList.RemoveAt(i);
						
					}else{
						return;
					}
				}
			}
		}

	}

	void resetButtons()
	{
		for (int i = 0; i< inputButtons.Length; i++) {
			if (inputButtons[i].needToResetDown)
			{
				inputButtons[i].needToResetDown = false;
				inputButtons[i].buttonDown = false;
			}
			if (inputButtons[i].needToResetUp)
			{
				inputButtons[i].needToResetUp = false;
				inputButtons[i].buttonUp = false;
			}
		}
	}

	void setButtons()
	{
		for (int i = 0; i< inputButtons.Length; i++) {
			if (inputButtons[i] != null && inputButtons[i].name != null && inputButtons[i].name != "")
			{
				if (!GeneralFinder.cursorHandler.useController)
				{
					if (Input.GetButton(inputButtons[i].name))
						inputButtons[i].buttonStay = true;
					else
						inputButtons[i].buttonStay = false;
					
					
					if (Input.GetButtonDown(inputButtons[i].name))
						inputButtons[i].buttonDown = true;
					else
						inputButtons[i].buttonDown = false;
					
					if (Input.GetButtonUp(inputButtons[i].name))
						inputButtons[i].buttonUp = true;
					else
						inputButtons[i].buttonUp = false;
					
					//MODIFICA PER PERMETTERE CHE LA LANTERNA VENGA USATA ANCHE COL TASTO SINISTRO
					/*
				if (inputButtons[i].name == "Mira")
				{
					if (Input.GetKey(KeyCode.Mouse0))
						inputButtons[i].buttonStay = true;
					if (Input.GetKeyUp(KeyCode.Mouse0))
						inputButtons[i].buttonUp = true;
					if (Input.GetKeyDown(KeyCode.Mouse0))
						inputButtons[i].buttonDown = true;
				}
				*/
					if (inputButtons[i].name == "PickLantern")
					{
						inputButtons[i].buttonStay = false;
						inputButtons[i].buttonUp = false;
						inputButtons[i].buttonDown = false;
						
					}
				}
				else
				{
					if (GeneralFinder.buttonController.getButton(inputButtons[i].name))
						inputButtons[i].buttonStay = true;
					else
						inputButtons[i].buttonStay = false;
					
					
					if (GeneralFinder.buttonController.getButtonDown(inputButtons[i].name))
						inputButtons[i].buttonDown = true;
					else
						inputButtons[i].buttonDown = false;
					
					if (GeneralFinder.buttonController.getButtonUp(inputButtons[i].name))
						inputButtons[i].buttonUp = true;
					else
						inputButtons[i].buttonUp = false;
				}



			}
		}
	}

	void setAxis()
	{
		for (int i = 0; i< inputAxis.Length; i++) {
			if (inputAxis[i] != null && inputAxis[i].name != null && inputAxis[i].name != "")
			{
				if (!GeneralFinder.cursorHandler.useController)
				{
					inputAxis[i].getAxisNormal = Input.GetAxis(inputAxis[i].name);
					inputAxis[i].getAxisRaw = Input.GetAxisRaw(inputAxis[i].name);
				}
				else
				{
					inputAxis[i].getAxisNormal = GeneralFinder.buttonController.getAxis(inputAxis[i].name);
					inputAxis[i].getAxisRaw = GeneralFinder.buttonController.getAxisRaw(inputAxis[i].name);
				}

			}
		}
	}

	void setMouse()
	{
		///*
		mousePosition.inputX = Input.mousePosition.x;
		mousePosition.inputY = Input.mousePosition.y;
		mousePosition.inputZ = Input.mousePosition.z;
		//*/
		/*
		mousePosition.inputX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
		mousePosition.inputY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
		mousePosition.inputZ = Camera.main.ScreenToWorldPoint(Input.mousePosition).z;
		*/
	}

	void updateFileIndex()
	{
		if (File.Exists(Application.persistentDataPath + "/" + Application.loadedLevelName +savedButtonFile + savedInputFileIndex+ savedFileExtention))
		{
			savedInputFileIndex++;
			updateFileIndex();
		}
		else
			return;
	}

	public void save()
	{
		BinaryFormatter bf = new BinaryFormatter ();

		FileStream file = File.Create (Application.persistentDataPath + "/" + Application.loadedLevelName +savedButtonFile + savedInputFileIndex+ savedFileExtention);
		bf.Serialize (file, changedButtonsList);
		file.Close ();

		FileStream file02 = File.Create (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedAxisFile + savedInputFileIndex+ savedFileExtention);
		bf.Serialize (file02, changedAxisList);
		file02.Close ();

		if (Input.mousePresent) {
			FileStream file03 = File.Create (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedMouseFile + savedInputFileIndex+ savedFileExtention);
			bf.Serialize (file03, changedMouseList);
			file03.Close ();
		}

		FileStream file04 = File.Create (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedObjectsFile + savedInputFileIndex+ savedFileExtention);
		bf.Serialize (file04, changedObjectsList);
		file04.Close ();
	}

	void load()
	{
		if (File.Exists (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedButtonFile + indexToLoad + savedFileExtention)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedButtonFile + indexToLoad + savedFileExtention, FileMode.Open);

			changedButtonsList = (List<ChangedButtons>)bf.Deserialize(file);
		}

		if (File.Exists (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedAxisFile + indexToLoad + savedFileExtention)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file02 = File.Open (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedAxisFile + indexToLoad + savedFileExtention, FileMode.Open);
			
			changedAxisList = (List<ChangedAxis>)bf.Deserialize(file02);
		}

		if (Input.mousePresent && File.Exists (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedMouseFile + indexToLoad + savedFileExtention)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file03 = File.Open (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedMouseFile + indexToLoad + savedFileExtention, FileMode.Open);
			
			changedMouseList = (List<ChangedMouse>)bf.Deserialize(file03);
		}

		if (File.Exists (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedObjectsFile + savedInputFileIndex+ savedFileExtention)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file04 = File.Open (Application.persistentDataPath + "/"+ Application.loadedLevelName + savedObjectsFile + savedInputFileIndex+ savedFileExtention, FileMode.Open);
			
			changedObjectsList = (List<ChangedObjects>)bf.Deserialize(file04);
		}
	}

	bool verifyIfButtonChanged(string buttonName)
	{
		for (int i = 0; i< inputButtons.Length; i++) {
			if (inputButtons[i] != null && inputButtons[i].name == buttonName && inputButtons[i].name!="")
			{
				//CONTIENE UNA MODIFICA PER PERMETTERE L'UTILIZZO DEL TASTO SINISTRO DEL MOUSE, COME IL DESTRO
				//LA VERSIONE COMMENTATA E' QUELLA VECCHIA
				/*
				if (Input.GetButton(inputButtons[i].name) != inputButtons[i].buttonStay)
					return true;

				if (Input.GetButtonUp(inputButtons[i].name) != inputButtons[i].buttonUp)
					return true;

				if (Input.GetButtonDown(inputButtons[i].name) != inputButtons[i].buttonDown)
					return true;
				break;
				*/
				if (!GeneralFinder.cursorHandler.useController)
				{
					if (Input.GetButton(inputButtons[i].name) != inputButtons[i].buttonStay)
						return true;
					
					if (Input.GetButtonUp(inputButtons[i].name) != inputButtons[i].buttonUp)
						return true;
					
					if (Input.GetButtonDown(inputButtons[i].name) != inputButtons[i].buttonDown)
						return true;

				}
				else
				{
					if (GeneralFinder.buttonController.getButton(inputButtons[i].name) != inputButtons[i].buttonStay)
						return true;
					
					if (GeneralFinder.buttonController.getButtonUp(inputButtons[i].name) != inputButtons[i].buttonUp)
						return true;
					
					if (GeneralFinder.buttonController.getButtonDown(inputButtons[i].name) != inputButtons[i].buttonDown)
						return true;
				}
				break;
			}
		}
		return false;
	}

	bool verifyIfAxisChanged(string axisName)
	{
		for (int i = 0; i< inputAxis.Length; i++) {
			if (inputAxis[i] != null && inputAxis[i].name == axisName && inputAxis[i].name!="")
			{
				if (!GeneralFinder.cursorHandler.useController)
				{
					if (Input.GetAxis(inputAxis[i].name) != inputAxis[i].getAxisNormal)
						return true;
				}
				else
				{
					if (GeneralFinder.buttonController.getAxis(inputAxis[i].name) != inputAxis[i].getAxisNormal)
						return true;
				}


				//non credo sia necessario fare il controllo sul getAxisRaw

				//if (Input.GetAxisRaw(inputAxis[i].name) != inputAxis[i].getAxisRaw)
				//	return true;

				break;
			}
		}
		return false;
	}

	bool verifyIfMouseChanged()
	{

		if (mousePosition.inputX != Input.mousePosition.x)
				return true;
		if (mousePosition.inputY != Input.mousePosition.y)
			return true;
		if (mousePosition.inputZ != Input.mousePosition.z)
			return true;
		return false;
		/*
		if (Input.mousePosition.x != Camera.main.ScreenToWorldPoint(Input.mousePosition).x)
			return true;
		if (Input.mousePosition.y != Camera.main.ScreenToWorldPoint(Input.mousePosition).y)
			return true;
		if (Input.mousePosition.z != Camera.main.ScreenToWorldPoint(Input.mousePosition).z)
			return true;
		return false;
		*/
	}

	void setLoadedButtons()
	{
		//creo una lista di stringhe che conterranno, ad ogni update, il tasto premuto, in modo che non vengano considerati
		//due cambiamenti dello stesso tasto nello stesso update, così da evitare di perdersi degli eventi
		List<string> stringList = new List<string>();

		for (int i = 0; i< changedButtonsList.Count; i++)
		{
			if (changedButtonsList [i] != null)
			{
				//se ho passato il tempo
				if (changedButtonsList [i].changingTime < Time.time)
				{
					//Debug.Log ("changed "+changedButtonsList [i].changingTime);
					//Debug.Log ("actual "+Time.time);
					//procede solo se non c'è un evento relativo allo stesso tasto
					if (!isThereAnotherString(stringList, changedButtonsList [i].inputButton.name))
					{
						stringList.Add(changedButtonsList [i].inputButton.name);
						setLoadedButton(changedButtonsList [i].inputButton);
						changedButtonsList.RemoveAt(i);
					}
					    
				}else{
					return;
				}
			}
		}
	}

	//restituisce true se c'è almeno una stringa uguale nella lista passata come input
	bool isThereAnotherString(List<string> stringList, string inputString){
		for (int i = 0; i<stringList.Count; i++) {
			if (stringList[i] == inputString)
				return true;
		}
		return false;
	}

	void setLoadedButton(InputButton inputMethodButton)
	{
		for (int i = 0; i< inputButtons.Length; i++) {
			if (inputButtons[i] != null && inputButtons[i].name == inputMethodButton.name && inputButtons[i].name != "")
			{
				//Debug.Log (Time.time);
				inputButtons[i].buttonStay = inputMethodButton.buttonStay;
				inputButtons[i].buttonDown = inputMethodButton.buttonDown;
				inputButtons[i].buttonUp = inputMethodButton.buttonUp;
			}
		}
	}



	void setLoadedAxis()
	{
		//creo una lista di stringhe che conterranno, ad ogni update, il tasto premuto, in modo che non vengano considerati
		//due cambiamenti dello stesso tasto nello stesso update, così da evitare di perdersi degli eventi
		//List<string> stringList = new List<string>();
		
		for (int i = 0; i< changedAxisList.Count; i++)
		{
			if (changedAxisList [i] != null)
			{
				//se ho passato il tempo
				if (changedAxisList [i].changingTime < Time.time)
				{
					setLoadedSingleAxis(changedAxisList [i].inputAxis);
					changedAxisList.RemoveAt(i);
					
				}else{
					return;
				}
			}
		}
	}

	void setLoadedSingleAxis(InputAxis inputMethodAxis)
	{
		for (int i = 0; i< inputAxis.Length; i++) {
			if (inputAxis[i] != null && inputAxis[i].name == inputMethodAxis.name && inputAxis[i].name != "")
			{
				inputAxis[i].getAxisNormal = inputMethodAxis.getAxisNormal;
				inputAxis[i].getAxisRaw = inputMethodAxis.getAxisRaw;
			}
		}
	}


	void setLoadedMouse()
	{
			
		for (int i = 0; i< changedMouseList.Count; i++)
		{
			if (changedMouseList [i] != null)
			{
				//se ho passato il tempo
				if (changedMouseList [i].changingTime < Time.time)
				{
					//mousePosition = changedMouseList[i].newMousePosition;

					InputMousePosition mousePos = changedMouseList[i].newMousePosition;
					Vector3 tempMouseWorldPos = new Vector3(mousePos.inputX, mousePos.inputY, mousePos.inputZ);
					Vector3 tempMouseScreenPos = Camera.main.WorldToScreenPoint(tempMouseWorldPos);
					InputMousePosition mousePosScreen = new InputMousePosition();
					mousePosScreen.inputX = tempMouseScreenPos.x;
					mousePosScreen.inputY = tempMouseScreenPos.y;
					mousePosScreen.inputZ = tempMouseScreenPos.z;
					//mousePosition = new InputMousePosition(tempMouseScreenPos.x, tempMouseScreenPos.y, tempMouseScreenPos.z);
					mousePosition = mousePosScreen;

					changedMouseList.RemoveAt(i);
				}else{
					return;
				}
			}
		}
	}
	/*
	 * 
	 * -----------------  METODI DI INTERFACCIA ---------------------
	 * 
	 */ 

	public bool isButtonPressed(string name)
	{
		for (int i = 0; i< inputButtons.Length; i++) {
			if (inputButtons[i] != null && inputButtons[i].name == name)
			{
				return inputButtons[i].buttonStay;
			}
		}
		return false;
	}

	public bool isButtonDown(string name)
	{
		for (int i = 0; i< inputButtons.Length; i++) {
			if (inputButtons[i] != null && inputButtons[i].name == name)
			{
				//viene messo a false per eviare che la lettura su due successivi update dia due volte true
				bool actualValue = inputButtons[i].buttonDown;
				//inputButtons[i].needToResetDown = true;
				inputButtons[i].buttonDown = false;
				return actualValue;
			}
		}
		return false;
	}

	public bool isButtonUp(string name)
	{
		for (int i = 0; i< inputButtons.Length; i++) {
			if (inputButtons[i] != null && inputButtons[i].name == name)
			{
				//viene messo a false per eviare che la lettura su due successivi update dia due volte true
				bool actualValue = inputButtons[i].buttonUp;
				//inputButtons[i].needToResetUp = true;
				inputButtons[i].buttonUp = false;
				return actualValue;
			}
		}
		return false;
	}

	public float getAxis(string name)
	{
		for (int i = 0; i< inputAxis.Length; i++) {
			if (inputAxis[i] != null && inputAxis[i].name == name)
			{
				return inputAxis[i].getAxisNormal;
			}
		}
		return 0.0f;
	}

	public float getAxisRaw(string name)
	{
		for (int i = 0; i< inputAxis.Length; i++) {
			if (inputAxis[i] != null && inputAxis[i].name == name)
			{
				return inputAxis[i].getAxisRaw;
			}
		}
		return 0.0f;
	}

	public Vector3 getMousePosition()
	{
		Vector3 tempPosition = new Vector3(mousePosition.inputX, mousePosition.inputY, mousePosition.inputZ);
		Vector3 tempPositionScreen = Camera.main.WorldToScreenPoint (tempPosition);
		return tempPosition;
		//return tempPositionScreen;
	}
}

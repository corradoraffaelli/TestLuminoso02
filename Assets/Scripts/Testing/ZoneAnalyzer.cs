using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

/// <summary>
/// Gestisce il salvataggio delle informazioni su quanto tempo il player ha passato in ogni zona di gioco. Mostra anche le morti e chi le ha causate. (DEBUG)
/// </summary>

// Corrado
public class ZoneAnalyzer : MonoBehaviour {

	string directory = "ZoneAnalyzer";
	string pathFileName = "zoneAnalyzerInfos";

	public enum Type
	{
		analyzer,
		collector
	}

	public Type type = Type.analyzer;

	[System.Serializable]
	public class ZoneInfos
	{
		public string name;
		[HideInInspector]
		public float timeSpent = 0.0f;
		public int enemyDeath = 0;
		public int spikesDeath = 0;
		public int doorDeath = 0;
	}

	[SerializeField]
	public ZoneInfos zoneInfos;

	public bool playerColliding;

	public bool newImplementation = false;

	ZoneAnalyzerContainer zoneContainer;

	void Update () {
		updateTime();
	}


	float lastKill = 0.0f;
	float diffKill = 0.5f;
	//-------------------------------
	//---METODI USATI DAL COLLECTOR--
	//-------------------------------

	/*
	void OnDestroy()
	{
		saveInfos();
	}
	*/

	[XmlRoot("ZoneAnalyzerCollection")]
	public class ZoneAnalyzerContainer
	{
		[XmlArray("ZoneInfos"),XmlArrayItem("ZoneInfo")]
		public ZoneInfos[] zoneInfos;
		
		public void Save(string path)
		{
			var serializer = new XmlSerializer(typeof(ZoneAnalyzerContainer));
			using(var stream = new FileStream(path, FileMode.Create))
			{
				serializer.Serialize(stream, this);
			}
		}
		
		public static ZoneAnalyzerContainer Load(string path)
		{
			var serializer = new XmlSerializer(typeof(ZoneAnalyzerContainer));
			using(var stream = new FileStream(path, FileMode.Open))
			{
				return serializer.Deserialize(stream) as ZoneAnalyzerContainer;
			}
		}
		
		//Loads the xml directly from the given string. Useful in combination with www.text.
		public static ZoneAnalyzerContainer LoadFromText(string text) 
		{
			var serializer = new XmlSerializer(typeof(ZoneAnalyzerContainer));
			return serializer.Deserialize(new StringReader(text)) as ZoneAnalyzerContainer;
		}
	}

	//-------------------------------
	//---METODI USATI DALL'ANALYZER--
	//-------------------------------

	/*
	void OnTriggerEnter2D(Collider2D other)
	{
		if (type == Type.analyzer)
		{
			if (other.gameObject.tag == "Player")
			{
				playerColliding = true;
			}
		}
	}
	*/

	void OnTriggerStay2D(Collider2D other)
	{
		if (type == Type.analyzer && !playerColliding)
		{
			if (other.gameObject.tag == "Player")
			{
				playerColliding = true;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (type == Type.analyzer)
		{
			if (other.gameObject.tag == "Player")
			{
				playerColliding = false;
			}
		}
	}

	public void c_playerKilled(string objectKiller)
	{
		//Debug.Log ("ricevuto messaggio");
		if (type == Type.analyzer && playerColliding) {
			if ((Time.time - lastKill) > diffKill)
			{
				lastKill = Time.time;
				switch (objectKiller)
				{
				case "Enemy":
					zoneInfos.enemyDeath++;
					break;
				case "Spikes":
					zoneInfos.spikesDeath++;
					break;
				case "KillingObj":
					zoneInfos.doorDeath++;
					break;
				default:
					break;
				}
			}

		}
	}

	void updateTime()
	{
		if (type == Type.analyzer)
		{
			if (playerColliding)
				zoneInfos.timeSpent = zoneInfos.timeSpent + Time.deltaTime;
		}
	}

	public void saveInfos()
	{
		if (type == Type.collector)
		{
			//Debug.Log ("analyzer");

			ZoneInfos[] infos;
			string[] names;

			if (newImplementation)
			{
				//prende tutti i component zoneAnalyzer presenti nella scena
				GameObject casualAnalyzer = GameObject.FindGameObjectWithTag("ZoneAnalyzer");
				Transform zoneFather = casualAnalyzer.transform.parent;
				Transform[] analyzersTransform = new Transform[zoneFather.childCount];
				
				ZoneAnalyzer[] analyzers = new ZoneAnalyzer[analyzersTransform.Length];
				infos = new ZoneInfos[analyzers.Length];
				names = new string[analyzers.Length];
				
				for (int i = 0; i < analyzersTransform.Length; i++)
				{
					analyzersTransform[i] = zoneFather.GetChild(i);
					if (analyzersTransform[i] != null)
					{
						analyzers[i] = analyzersTransform[i].GetComponent<ZoneAnalyzer>();

						if (analyzers[i] != null && analyzers[i].type == Type.analyzer)
						{
							infos[i] = analyzers[i].zoneInfos;
							names[i] = analyzers[i].zoneInfos.name;
						}
					}
				}
			}
			else
			{
				GameObject[] objs = GameObject.FindGameObjectsWithTag("ZoneAnalyzer");
				
				ZoneAnalyzer[] analyzers = new ZoneAnalyzer[objs.Length];
				infos = new ZoneInfos[analyzers.Length];
				
				names = new string[objs.Length];
				
				for (int i = 0; i < objs.Length; i++)
				{
					if (objs[i] != null)
					{
						analyzers[i] = objs[i].GetComponent<ZoneAnalyzer>();
						
						//prende tutti gli elementi ZoneInfos
						if (analyzers[i] != null && analyzers[i].type == Type.analyzer)
						{
							infos[i] = analyzers[i].zoneInfos;
							names[i] = analyzers[i].zoneInfos.name;
						}
					}
				}
			}

			//ORDINO

			Array.Sort(names);

			ZoneInfos[] orderedInfos = new ZoneInfos[infos.Length];

			for (int i = 0; i < names.Length; i++)
			{
				for (int j = 0; j < infos.Length; j++)
				{
					if (names[i] == infos[j].name)
						orderedInfos[i] = infos[j];
				}
			}

			//creo l'opportuna directory se non esiste già
			if (!Directory.Exists(directory)) 
			{
				Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, directory));
			}
			
			//cerca l'elemento con indice maggiore
			int actualIndex = 0;
			string finalPath01 = Path.Combine(Application.persistentDataPath,  directory + "/" + Application.loadedLevelName + "_" + pathFileName + "_");
			string finalPath02;
			
			while (true)
			{
				finalPath02 = finalPath01 + actualIndex.ToString() + ".xml";
				if (File.Exists (finalPath02)) 
				{
					actualIndex++;
				}
				else
				{
					break;
				}
			}
			
			//salva tutti gli elementi su file
			zoneContainer = new ZoneAnalyzerContainer();
			zoneContainer.zoneInfos = orderedInfos;
			zoneContainer.Save(finalPath02);
		}
	}
}

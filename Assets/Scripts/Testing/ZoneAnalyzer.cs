using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

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
	}

	[SerializeField]
	public ZoneInfos zoneInfos;

	bool playerColliding;

	ZoneAnalyzerContainer zoneContainer;

	void Update () {
		updateTime();
	}


	//-------------------------------
	//---METODI USATI DAL COLLECTOR--
	//-------------------------------

	void OnDestroy()
	{
		if (type == Type.collector)
		{
			//prende tutti i component zoneAnalyzer presenti nella scena
			GameObject[] objs = GameObject.FindGameObjectsWithTag("ZoneAnalyzer");
			ZoneAnalyzer[] analyzers = new ZoneAnalyzer[objs.Length];
			ZoneInfos[] infos = new ZoneInfos[analyzers.Length];

			for (int i = 0; i < objs.Length; i++)
			{
				if (objs[i] != null)
				{
					analyzers[i] = objs[i].GetComponent<ZoneAnalyzer>();

					//prende tutti gli elementi ZoneInfos
					if (analyzers[i] != null && analyzers[i].type == Type.analyzer)
					{
						infos[i] = analyzers[i].zoneInfos;
					}
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
			zoneContainer.zoneInfos = infos;
			zoneContainer.Save(finalPath02);
		}
	}

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

	void updateTime()
	{
		if (type == Type.analyzer)
		{
			if (playerColliding)
				zoneInfos.timeSpent = zoneInfos.timeSpent + Time.deltaTime;
		}
	}
}

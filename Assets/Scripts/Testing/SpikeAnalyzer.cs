﻿using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

/// <summary>
/// Gestisce il salvataggio delle informazioni su quante volte sono stati attivati gli Spikes, utili per capire dove il player è caduto (DEBUG)
/// </summary>

// Corrado
public class SpikeAnalyzer : MonoBehaviour {
	
	string directory = "SpikesAnalyzer";
	string pathFileName = "spikesAnalyzerInfos";

	float lastTime = 0.0f;
	float timeToTrigger = 1.0f;
	
	public enum Type
	{
		analyzer,
		collector
	}
	
	public Type type = Type.analyzer;
	
	[System.Serializable]
	public class SpikesInfos
	{
		public string name;
		[HideInInspector]
		public int enteredTimes = 0;
	}
	
	[SerializeField]
	public SpikesInfos spikesInfos;
	
	bool playerColliding;
	
	SpikesAnalyzerContainer spikesContainer;

	/*
	void Update () {
		updateTime();
	}
	*/
	
	
	//-------------------------------
	//---METODI USATI DAL COLLECTOR--
	//-------------------------------

	/*
	void OnDestroy()
	{
		saveInfos();
	}
	*/
	
	[XmlRoot("SpikesAnalyzerCollection")]
	public class SpikesAnalyzerContainer
	{
		[XmlArray("SpikesInfos"),XmlArrayItem("SpikesInfo")]
		public SpikesInfos[] spikesInfos;
		
		public void Save(string path)
		{
			var serializer = new XmlSerializer(typeof(SpikesAnalyzerContainer));
			using(var stream = new FileStream(path, FileMode.Create))
			{
				serializer.Serialize(stream, this);
			}
		}
		
		public static SpikesAnalyzerContainer Load(string path)
		{
			var serializer = new XmlSerializer(typeof(SpikesAnalyzerContainer));
			using(var stream = new FileStream(path, FileMode.Open))
			{
				return serializer.Deserialize(stream) as SpikesAnalyzerContainer;
			}
		}
		
		//Loads the xml directly from the given string. Useful in combination with www.text.
		public static SpikesAnalyzerContainer LoadFromText(string text) 
		{
			var serializer = new XmlSerializer(typeof(SpikesAnalyzerContainer));
			return serializer.Deserialize(new StringReader(text)) as SpikesAnalyzerContainer;
		}
	}
	
	//-------------------------------
	//---METODI USATI DALL'ANALYZER--
	//-------------------------------
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (type == Type.analyzer)
		{
			if (other.gameObject.tag == "Player" && ((Time.time - lastTime) > timeToTrigger))
			{
				spikesInfos.enteredTimes++;
				lastTime = Time.time;
				//playerColliding = true;
			}
		}
	}

	public void saveInfos()
	{
		if (type == Type.collector)
		{
			//prende tutti i component zoneAnalyzer presenti nella scena
			GameObject[] objs = GameObject.FindGameObjectsWithTag("SpikesAnalyzer");

			/*
			for (int i = 0; i < objs.Length; i++)
			{
				if (objs[i] != null)
				{
					//Debug.Log (objs[i].name);
				}
			}
			*/
			
			SpikeAnalyzer[] analyzers = new SpikeAnalyzer[objs.Length];
			SpikesInfos[] infos = new SpikesInfos[analyzers.Length];

			string[] names = new string[objs.Length];
			
			for (int i = 0; i < objs.Length; i++)
			{
				if (objs[i] != null)
				{
					analyzers[i] = objs[i].GetComponent<SpikeAnalyzer>();
					
					//prende tutti gli elementi ZoneInfos
					if (analyzers[i] != null && analyzers[i].type == Type.analyzer)
					{
						infos[i] = analyzers[i].spikesInfos;
						names[i] = analyzers[i].spikesInfos.name;
					}
				}
			}

			Array.Sort(names);

			SpikesInfos[] orderedInfos = new SpikesInfos[infos.Length];

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
			spikesContainer = new SpikesAnalyzerContainer();
			spikesContainer.spikesInfos = orderedInfos;
			spikesContainer.Save(finalPath02);
		}
	}

	/*
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
	*/
}

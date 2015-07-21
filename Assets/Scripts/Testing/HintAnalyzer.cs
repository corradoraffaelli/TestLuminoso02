using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class HintAnalyzer : MonoBehaviour {

	string directory = "HintAnalyzer";
	string pathFileName = "hintAnalyzerInfos";

	HintAnalyzerContainer hintContainer;

	[System.Serializable]
	public class HintElement
	{
		public string stringa;
		public int activationNumber = 0;
	}

	List<HintElement> hintElements = new List<HintElement>();

	public void addElement(string inputString)
	{
		HintElement newHintElem = new HintElement();
		newHintElem.stringa = inputString;

		hintElements.Add(newHintElem);
	}
	
	public void activateElement(string inputString)
	{
		hintElements.Find(x => x.stringa.Equals(inputString)).activationNumber++;
	}

	void OnDestroy()
	{
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
		hintContainer = new HintAnalyzerContainer();
		hintContainer.hintElements = hintElements.ToArray();
		hintContainer.Save(finalPath02);
	}

	[XmlRoot("HintAnalyzerCollection")]
	public class HintAnalyzerContainer
	{
		[XmlArray("HintInfos"),XmlArrayItem("HintInfo")]
		public HintElement[] hintElements;
		
		public void Save(string path)
		{
			var serializer = new XmlSerializer(typeof(HintAnalyzerContainer));
			using(var stream = new FileStream(path, FileMode.Create))
			{
				serializer.Serialize(stream, this);
			}
		}
		
		public static HintAnalyzerContainer Load(string path)
		{
			var serializer = new XmlSerializer(typeof(HintAnalyzerContainer));
			using(var stream = new FileStream(path, FileMode.Open))
			{
				return serializer.Deserialize(stream) as HintAnalyzerContainer;
			}
		}
		
		//Loads the xml directly from the given string. Useful in combination with www.text.
		public static HintAnalyzerContainer LoadFromText(string text) 
		{
			var serializer = new XmlSerializer(typeof(HintAnalyzerContainer));
			return serializer.Deserialize(new StringReader(text)) as HintAnalyzerContainer;
		}
	}
}

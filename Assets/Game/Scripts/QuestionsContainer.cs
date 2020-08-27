using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
/// <summary>
/// This script reads the data from xml file and stores in it
/// </summary>
[XmlRoot("QuestionsCollection")]
public class QuestionsContainer {

	[XmlArray("Questions")]
	[XmlArrayItem("Question")]
	public List<Questions> questionsList = new List<Questions>();

	public static QuestionsContainer Load(string path)
	{
		TextAsset _xml = Resources.Load<TextAsset>(path);

		XmlSerializer serializer = new XmlSerializer(typeof(QuestionsContainer));

		StringReader reader = new StringReader(_xml.text);

		QuestionsContainer questionsList = serializer.Deserialize(reader) as QuestionsContainer;

		reader.Close();

		return questionsList;
	}
}

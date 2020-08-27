using System.Xml;
using System.Xml.Serialization;
/// <summary>
/// It the basic struction of question and answer
/// </summary>
[System.Serializable]
public class Questions {

	//here we tell that the question is value privided by name 
	[XmlAttribute("name")]
	public string question;
	//here we get the element of question array
	[XmlElement("Answer")]
	public string isTrue;

}

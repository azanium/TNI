using UnityEngine; 
using System.Collections; 
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text; 

// http://www.eggheadcafe.com/articles/system.xml.xmlserialization.asp

public class XmlManager: MonoBehaviour
{
    public static string XmlPath = Application.dataPath + "/Resources/Xml/";

	public static object LoadInstanceAsXml(string filename, System.Type type)
	{
		string xml = LoadXML(filename); 
		if((xml != null) && (xml.ToString() != ""))
		{ 
			// notice how I use a reference to System.Type here, you need this 
			// so that the returned object is converted into the correct type 
			return DeserializeObject(type, xml); 
		}
		return null;
	}
	
	public static void SaveInstanceAsXml(string filename, System.Type type, object instance)
	{
		string xml = SerializeObject(type, instance);
		SaveXML(filename, xml);
	}
	
	// Here we deserialize it back into its original form 
	public static object DeserializeObject(System.Type type, string pXmlizedString) 
	{ 
		XmlSerializer xs = new XmlSerializer(type); 
		MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString)); 
		XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
		if(xmlTextWriter != null)
		{
			return xs.Deserialize(memoryStream);
		}
		return null;
	} 
	
	public static string SerializeObject(System.Type type, object instance)
	{
		string xmlizedString = null; 
		MemoryStream memoryStream = new MemoryStream(); 
		XmlSerializer xs = new XmlSerializer(type); 
		XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xmlTextWriter.Formatting = Formatting.Indented;

		xs.Serialize(xmlTextWriter, instance); 
		memoryStream = (MemoryStream)xmlTextWriter.BaseStream; 
		xmlizedString = UTF8ByteArrayToString(memoryStream.ToArray()); 
		return xmlizedString; 
	} 
	
	private static void SaveXML(string filename, string xml) 
	{ 
		StreamWriter writer; 
		FileInfo t = new FileInfo(Application.dataPath + "/Resources/Xml/" + filename); 
		if(!t.Exists) 
		{ 
			writer = t.CreateText(); 
		} 
		else 
		{ 
			//t.Delete(); 
			writer = t.CreateText(); 
		} 
		writer.Write(xml); 
		writer.Close();
	} 
	
	private static string LoadXML(string filename)
	{
		TextAsset textAsset = (TextAsset)Resources.Load(filename, typeof(TextAsset));
		if(textAsset != null)
		{
			string info = textAsset.text;
			return info; 
		}
		else
		{
			return null;
		}
	}

	// the following methods came from the referenced URL
	private static string UTF8ByteArrayToString(byte[] characters) 
	{
		UTF8Encoding encoding = new UTF8Encoding(); 
		string constructedString = encoding.GetString(characters); 
		return (constructedString); 
	} 
	
	private static byte[] StringToUTF8ByteArray(string pXmlString) 
	{
		UTF8Encoding encoding = new UTF8Encoding(); 
		byte[] byteArray = encoding.GetBytes(pXmlString); 
		return byteArray; 
	} 	
} 

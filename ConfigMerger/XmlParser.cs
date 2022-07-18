using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace ConfigMerger;

public class XmlParser
{
    public T Load<T>(string content)
    {
        XmlSerializer ser = new XmlSerializer(typeof(T));
        TextReader reader = new StringReader(content);
        return (T)ser.Deserialize(reader)!;
    }

    public T LoadFromFile<T>(string filePath)
    {
        if (File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}", filePath);

        XmlSerializer ser = new XmlSerializer(typeof(T));
        using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
        {
            return (T)ser.Deserialize(sr)!;
        }
    }

    public string Save<T>(T obj, bool indent = false) => Save(obj, new XmlWriterSettings() { Indent = indent });
    public string Save<T>(T obj, XmlWriterSettings? options = null)
    {
        XmlSerializer ser = new XmlSerializer(typeof(T));
        TextWriter writer = new StringWriter();
        using (XmlWriter xmlwriter = XmlWriter.Create(writer, options ?? new XmlWriterSettings()))
        {
            ser.Serialize(xmlwriter, obj);
            return writer.ToString()!;
        }
    }
    public void SaveToFile<T>(T obj, string filePath, bool indent = false) => SaveToFile(obj, filePath, new XmlWriterSettings() { Indent = indent });

    public void SaveToFile<T>(T obj, string filePath, XmlWriterSettings? options = null)
    {

        XmlSerializer ser = new XmlSerializer(typeof(T));
        using (StreamWriter sw = new StreamWriter(new FileStream(filePath, FileMode.Create), Encoding.UTF8))
        using (XmlWriter xmlwriter = XmlWriter.Create(sw, options ?? new XmlWriterSettings()))
        {
            ser.Serialize(xmlwriter, obj);
        }
    }
}



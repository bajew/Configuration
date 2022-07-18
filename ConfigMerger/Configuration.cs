using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Reflection;
using ConfigMerger;

namespace ConfigMerger;

public static class Configuration
{
    public static T LoadArgs<T>() where T : new() => LoadArgs<T>(Environment.GetCommandLineArgs());

    #region CommandLineArguments - Deserialization
        /// <summary>
        /// Erstellt ein Objekt aus den Komandozeilen Aurufparametern
        /// </summary>
        /// <typeparam name="T">Typ des Objekts</typeparam>
        /// <param name="args">Kommandozeilenparameter</param>
        /// <returns>Gibt ein neues Objekt zurück</returns>
    public static T LoadArgs<T>(string[] args) where T : new()
    {
        return new CommandLineParser().Parse<T>(args);
    }
    #endregion


    /// <summary>
    /// Erstellt ein neues Object welches mit den Werten aus System.Environment.GetEnvironmentVariables befüllt wird
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T LoadEnv<T>() where T :new()
    {
        return new EnvironmentParser().Parse<T>();
    }

    /// <summary>
    /// Erstellt ein neues Object welche mit den Environment Varialben aus einer Text-Datei gelesen werden.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static T LoadEnvFromFile<T>(string filename) where T: new()
    {
        return new EnvironmentParser().ParseFromFile<T>(filename);
    }


    #region XML Serialisation - Deserialization
    /// <summary>
    /// Erstellt ein Objekt aus eimem xml-string
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="xml">Xml als string</param>
    /// <returns>Gibt das Deserialisierte Objekt zurück</returns>
    public static T LoadXml<T>(string xml)
    {
        return new XmlParser().Load<T>(xml);
    }

    /// <summary>
    /// Erstellt ein Object aus einer xml-Datei
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="file">Dateiname</param>
    /// <returns>Gibt das Deserialisierte Objekt zurück</returns>
    public static T LoadXmlFromFile<T>(string file)
    {
        return new XmlParser().LoadFromFile<T>(file);
    }

    public static string SaveXml<T>(T obj, bool indent = false) => SaveXml(obj, new XmlWriterSettings() { Indent = indent });


    /// <summary>
    /// Sereialisiert das Objekt in ein xml-string
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="obj">Objekt welches serialisiert werden soll</param>
    /// <param name="options">XmlWriterSettings</param>
    /// <returns>Gibt den xml-string zurück</returns>
    public static string SaveXml<T>(T obj, XmlWriterSettings? options = null)
    {
        return new XmlParser().Save<T>(obj, options);
    }

    public static void SaveXmlToFile<T>(T obj, string file, bool indent = false) => SaveXmlToFile(obj, file, new XmlWriterSettings { Indent = indent });

    /// <summary>
    /// Serialisiert das Objekt und speichert das Ergebnis in eine Datei ab
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="obj">Objekt welches serialisiert und gespeichert werden soll</param>
    /// <param name="file">Dateiname</param>
    /// <param name="options">XmlWriterSettings</param>
    public static void SaveXmlToFile<T>(T obj, string file, XmlWriterSettings? options = null)
    {
        new XmlParser().SaveToFile<T>(obj, file, options);
    }
    #endregion

    #region JSON Serialisation - Deserialization


    /// <summary>
    /// Deserialisiert ein Objekt aus einem json-string
    /// </summary>
    /// <typeparam name="T">Typ des Objects</typeparam>
    /// <param name="json">Json-string</param>
    /// <returns>Gibt das deserialisierte Objekt zurück</returns>
    public static T LoadJson<T>(string json)
    {
        return new JsonParser().Load<T>(json);
    }

    /// <summary>
    /// Deserialisert ein Objekt aus einer json-datei
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="file">Dateiname</param>
    /// <returns>Gibt das deserialiserte Objekt zurück</returns>
    public static T LoadJsonFromFile<T>(string file)
    {
        return new JsonParser().LoadFromFile<T>(file);
    }

    public static string SaveJson<T>(T obj, bool indent = false) => SaveJson(obj, new JsonSerializerOptions { WriteIndented = indent });

    /// <summary>
    /// Serialisiert das Objekt in einen json-string
    /// </summary>
    /// <typeparam name="T">Typ des Objects</typeparam>
    /// <param name="obj">Objekt welches serialisiert wird</param>
    /// <param name="options">Optionale JsonOptions</param>
    /// <returns>Gibt den serialisierten Json-string zurück</returns>
    public static string SaveJson<T>(T obj, JsonSerializerOptions? options = null)
    {
        return new JsonParser().Save<T>(obj, options);
    }

    public static void SaveJsonToFile<T>(T obj, string file, bool indent = false) => SaveJsonToFile(obj, file, new JsonSerializerOptions { WriteIndented = indent });

    /// <summary>
    /// Serialisert das Objekt und speichert es in eine Datei ab
    /// </summary>
    /// <typeparam name="T">Typ des Objekts</typeparam>
    /// <param name="obj">Das Objekt welches serialisiert werden soll</param>
    /// <param name="file">Dateiname</param>
    /// <param name="options">Optionale JsonOptions</param>
    public static void SaveJsonToFile<T>(T obj, string file, JsonSerializerOptions? options = null)
    {
        new JsonParser().SaveToFile<T>(obj, file, options);
    }
    #endregion

    /// <summary>
    /// Fügt mehrere Objekte zu einem Objekt zusammen. 
    /// Dabei werden nur public beschreibare Eigenschaften (Properties) verwendet. 
    /// Dabei gilt, dass obj1.prop1 nur dann überschrieben wird, wenn obj2.prop1 nicht null ist. 
    /// </summary>
    /// <typeparam name="T">Typ der Objekte</typeparam>
    /// <param name="objects">Objekte die zusammgefasst werden sollen</param>
    /// <returns>Gibt ein zusammengefasstest Object zurück</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static T Merge<T>(params T[] objects) where T : new()
    {
        if (objects == null || objects.Length == 0)
            throw new ArgumentNullException(nameof(objects));

        PropertyInfo[]? propinfos = typeof(T).GetProperties();

        T result = objects.First();

        foreach (var obj in objects.Skip(1))
        {
            result = Merge2(result, obj, propinfos);
        }

        return result;

        T Merge2(T obj1, T obj2, PropertyInfo[]? propinfos)
        {
            if (propinfos == null || propinfos.Length == 0)
                return obj1;

            foreach (var prop in propinfos)
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var val2 = prop.GetValue(obj2);
                if (val2 != null)
                {
                    prop.SetValue(obj1, val2);
                }
            }
            return obj1;
        }

    }
}



using System.Text;
using System.Text.Json;

namespace ConfigMerger;

public class JsonParser
{
    public T Load<T>(string content)
    {
        return JsonSerializer.Deserialize<T>(content)!;
    }

    public T LoadFromFile<T>(string filePath)
    {
        if (File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}", filePath);

        using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
        {
            return JsonSerializer.Deserialize<T>(sr.BaseStream)!;
        }
    }

    public string Save<T>(T obj, bool indent = false) => Save(obj, new JsonSerializerOptions() { WriteIndented = indent });

    public string Save<T>(T obj, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize<T>(obj, options ?? new JsonSerializerOptions())!;
    }

    public void SaveToFile<T>(T obj, string file, bool indent = false) => SaveToFile(obj, file, new JsonSerializerOptions() { WriteIndented = indent });

    public void SaveToFile<T>(T obj, string file, JsonSerializerOptions? options = null)
    {
        using (StreamWriter sw = new StreamWriter(
           new FileStream(file, FileMode.Create), Encoding.UTF8))
        {
            JsonSerializer.Serialize<T>(sw.BaseStream, obj, options ?? new JsonSerializerOptions());
        }
    }
}



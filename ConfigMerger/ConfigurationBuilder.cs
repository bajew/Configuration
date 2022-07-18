namespace ConfigMerger;

public class ConfigurationBuilder<T> where T : new()
{
    public T Config { get; private set; } = new();

    public static ConfigurationBuilder<T> Create()
    {
        return new ConfigurationBuilder<T>(); 
    }

    public ConfigurationBuilder<T> LoadFromXml(string xmlfilepath)
    {
        Configuration.Merge(Config, Configuration.LoadXmlFromFile<T>(xmlfilepath));
        return this;
    }
    public  ConfigurationBuilder<T> LoadFromJson(string jsonfilepath)
    {
        Configuration.Merge(Configuration.LoadJsonFromFile<T>(jsonfilepath));
        return this;
    }
    public ConfigurationBuilder<T> LoadFromEnv(string envfilepath)
    {
        Configuration.Merge(Configuration.LoadEnvFromFile<T>(envfilepath));
        return this;
    }

    public ConfigurationBuilder<T> LoadFromArgs()
    {
        Configuration.Merge(Configuration.LoadArgs<T>());
        return this;
    }




}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMerger;

public class EnvAttribute : Attribute
{
    public EnvAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

}
public class EnvironmentParser
{
    public T Parse<T>() where T : new()
    {
        T newT = new();
        List<AttributePropertyInfo<EnvAttribute>> evnPropInfos = GetPropertyInfos<T>();
        foreach (var envProp in evnPropInfos)
        {
            var env = Environment.GetEnvironmentVariable(envProp.AttributeInfo.Name);
            if (env == null) continue;

            envProp.PropInfo.SetValueByType(newT, env);
        }
        return newT;
    }

    public T ParseFromFile<T>(string envfile) where T : new()
    {
        T newT = new();
        List<AttributePropertyInfo<EnvAttribute>> evnPropInfos = GetPropertyInfos<T>();
        var envdic = File.ReadAllLines(envfile)
                    .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#"))
                    .Distinct()
                    .Select(x => x.Split("="))
                    .Where(x => x.Length > 1)
                    .ToDictionary(x => x[0].Trim(), x => x[1].Trim());

        foreach (var envProp in evnPropInfos)
        {
            string? env;
            if (!envdic.TryGetValue(envProp.AttributeInfo.Name, out env))
                continue;
            envProp.PropInfo.SetValueByType(newT, env);
        }
        return (T)newT;
    }

    private static List<AttributePropertyInfo<EnvAttribute>> GetPropertyInfos<T>() where T : new()
    {
        var props = typeof(T).GetProperties();
        List<AttributePropertyInfo<EnvAttribute>> evnPropInfos = new();
        foreach (var prop in props)
        {
            if (!(prop.CanWrite && prop.CanRead))
                continue;

            EnvAttribute? flagAtt;
            if ((flagAtt = prop.GetCustomAttributes<EnvAttribute>().FirstOrDefault()) != null)
            {
                evnPropInfos.Add(new AttributePropertyInfo<EnvAttribute>(prop, flagAtt));
            }
        }

        return evnPropInfos;
    }

}


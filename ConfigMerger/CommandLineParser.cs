
using System.Reflection;

namespace ConfigMerger;

public class ArgAttribute : Attribute
{
    public ArgAttribute(string longName)
    {
        LongName = longName;
    }
    public ArgAttribute(string longName, string shortName)
    {
        LongName = longName;
        ShortName = shortName;
    }
    public ArgAttribute()
    {
        IsRest = true;
    }
    public string LongName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public bool IsRest { get; set; }

}
public partial class CommandLineParser
{
    public Dictionary<string, List<string>> Args { get; set; } = new Dictionary<string, List<string>>();
    public List<string> Rest { get; set; } = new List<string>();

    public string Value(string key)
    {
        List<string>? tmp;
        if (Args.TryGetValue(key, out tmp))
        {
            return tmp?.FirstOrDefault() ?? string.Empty;
        }
        else
        {
            return string.Empty;
        }
    }
    public List<string> Values(string key)
    {
        List<string>? tmp;
        if (Args.TryGetValue(key, out tmp))
        {
            return tmp;
        }
        else
        {
            return new List<string>();
        }
    }
    public bool IsSet(string key)
    {
        return Args.ContainsKey(key);
    }
    public void Parse(string[] args)
    {
        bool parseRest = false;
        string currentName = string.Empty;
        foreach (var arg in args)
        {
            if (arg == "--")
            {
                parseRest = true;
            }
            else if (parseRest)
            {
                Rest.Add(arg);
            }
            else if (arg.StartsWith("-"))
            {
                //Falls beim vorherigen arg, keine Wert gesetzt wurde, dann war dieser ein bool flag, diesen auf true setzten
                SetBool(currentName);

                int valueDelimiterIndex = arg.IndexOf("=");
                string? value = null;
                string? name = null;
                if (valueDelimiterIndex > -1)
                {
                    if (arg.Length > valueDelimiterIndex)
                        value = arg.Substring(valueDelimiterIndex + 1);
                    name = arg.Substring(0, valueDelimiterIndex);
                }
                if (name != string.Empty)
                    currentName = arg;
                else
                    currentName = name;

                if (!Args.ContainsKey(currentName))
                {
                    Args[currentName] = new List<string>();
                    if (value != null)
                        Args[currentName].Add(value);
                }


            }
            else if (!string.IsNullOrWhiteSpace(currentName))
            {
                Args[currentName].Add(arg);
            }
            else
            {
                Rest.Add(arg);
            }
        }
        //Letzt Bool Option hinzufügen, falls sie nicht schon vorhanden ist
         SetBool(currentName);

        void SetBool(string currentName)
        {
            if (Args.ContainsKey(currentName) && (Args[currentName] == null || Args[currentName].Count == 0))
            {
                var boolValue = "true";
                if (currentName.StartsWith("--no-") || currentName.StartsWith("-no-"))
                {
                    currentName = currentName.Replace("no-", "");
                    boolValue = "false";
                }
                Args[currentName] = new List<string>() { boolValue };
            }

        }
    }


    public T Parse<T>(string[] args) where T : new()
    {
        Parse(args);
        var props = typeof(T).GetProperties();
        T newT = new();
        List<AttributePropertyInfo<ArgAttribute>> argPropInfos = new ();
        foreach (var prop in props)
        {
            if (!(prop.CanWrite && prop.CanRead))
                continue;

            ArgAttribute? flagAtt;
            if ((flagAtt = prop.GetCustomAttributes<ArgAttribute>().FirstOrDefault()) != null)
            {
                argPropInfos.Add(new AttributePropertyInfo<ArgAttribute>(prop, flagAtt));
            }
        }

        foreach (var flagProp in argPropInfos)
        {
            if (!string.IsNullOrWhiteSpace(flagProp.AttributeInfo.LongName) && Args.ContainsKey(flagProp.AttributeInfo.LongName))
            {
                flagProp.PropInfo.SetValueByType<T>(newT, Args[flagProp.AttributeInfo.LongName]);
            }
            else if (!string.IsNullOrWhiteSpace(flagProp.AttributeInfo.ShortName) && Args.ContainsKey(flagProp.AttributeInfo.ShortName))
            {
                flagProp.PropInfo.SetValueByType<T>(newT, Args[flagProp.AttributeInfo.ShortName]);
            }
            else if (flagProp.AttributeInfo.IsRest)
            {
                flagProp.PropInfo.SetValueByType<T>(newT, Rest);
            }
        }


        return newT;
    }


    public override string ToString()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var kvp in Args)
        {
            sb.Append($"{kvp.Key}: {string.Join(',', kvp.Value.ToArray())}; ");
        }
        return sb.ToString() + string.Join(',', Rest.ToArray());
    }

}


using ConfigMerger;
using System.Text.Json;
using System.Xml;

Config cfg1 = new Config() { Programm = new ProgramConfig() { Intervall = 10, Name = "Test1" }, };
Config cfg2 = new Config() { Connection = new ConnectionConfig() { Host = "Host2", Port = 2, Locations = new LocationConnections[] { new LocationConnections() { LineNo = "101", StatNo = "201" } }, Names = new List<string>() { "name1" } } };
Config cfg3 = new Config() { Typen = new List<string>() { "typ1", "typ2" }, Name = "cfg3" };


var result = Configuration.Merge(cfg1, cfg2, cfg3);

Console.WriteLine(result);

var cfg = ConfigurationBuilder<Config>.Create()
    .LoadFromXml("file1")
    .LoadFromXml("file2")
    .Config;


Console.WriteLine(Configuration.SaveXml(result, indent: false));
Console.WriteLine(Configuration.SaveJson(result, indent: false));

public class Config
{
    public string? Name { get; set; }
    public ProgramConfig? Programm { get; set; }
    public ConnectionConfig? Connection { get; set; }

    public List<string>? Typen { get; set; }
    public override string ToString()
    {
        return $"Name:{Name} Programm:{{ {Programm} }} Connection:{{ {Connection}}} Typen:{{{string.Join(",", Typen ?? new List<string>())}}}";
    }
}

public class ProgramConfig
{
    public int Intervall { get; set; }

    public string? Name { get; set; }

    public override string ToString()
    {
        return $"Intervall:{Intervall}, Name:{Name}";
    }

}

public class ConnectionConfig
{
    public int Port { get; set; }
    public string? Host { get; set; }

    public LocationConnections[]? Locations { get; set; }

    public List<string>? Names { get; set; }

    public override string ToString()
    {
        return $"Port:{Port}, Host:{Host}, {string.Join(",", Locations?.AsEnumerable() ?? Enumerable.Empty<LocationConnections>())}, Names:{string.Join(",", Names ?? new List<string>())}";
    }
}

public class LocationConnections
{
    public string? LineNo { get; set; }
    public string? StatNo { get; set; }

    public override string ToString()
    {
        return $"LineNo:{LineNo}, StatNo:{StatNo}";
    }
}
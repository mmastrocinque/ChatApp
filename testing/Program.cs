// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using System.Text.Json.Serialization;

var test = new TestClass();
var msg = JsonSerializer.Serialize(test);
Console.WriteLine(msg);


public class TestClass : testItnerface
{
    [JsonIgnore]
    public string value => "Secret";
    public string value2 => "Exposed";
}

public interface testItnerface
{
    public string value { get; }
}
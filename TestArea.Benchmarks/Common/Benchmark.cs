using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;

namespace TestArea.Benchmarks.Common;

[MemoryDiagnoser]
public class Benchmark : IBenchmark
{
    private static string ct = "123";

    [Benchmark]
    public void SerializeObject()
    {
        for (int i = 0; i < 1000; i++)
        {
            var jopa = SerializeObject(ct);
            var st = Dez<string>(jopa);
        }
    }

    [Benchmark]
    public void ToByteArrayBf()
    {
        for (int i = 0; i < 1000; i++)
        {
            var obj = ToByteArrayBf(ct);
            var st = FromByteArray<string>(obj);
        }
    }


    private static byte[] SerializeObject(object value) 
        => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));

    private static T Dez<T>(byte[] value) where T : class 
        => JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(value));

    private static byte[] ToByteArrayBf(object obj)
    {
        var binaryFormatter = new BinaryFormatter();
        using (var memoryStream = new MemoryStream())
        {
            binaryFormatter.Serialize(memoryStream, obj);
            return memoryStream.ToArray();
        }
    }

    private static T FromByteArray<T>(byte[] byteArray)
        where T : class
    {
        var binaryFormatter = new BinaryFormatter();
        using (var memoryStream = new MemoryStream(byteArray))
        {
            return binaryFormatter.Deserialize(memoryStream) as T;
        }
    }
}
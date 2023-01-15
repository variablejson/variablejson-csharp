using NUnit.Framework;
using System.Diagnostics;
using System.Text.Json;
using VariableJson;

namespace test;

public class Tests
{
    private void LoadData(out string inputData, out string truthData)
    {
        string testName = new StackTrace().GetFrame(1)!.GetMethod()!.Name;
        inputData = System.IO.File.ReadAllText($"./data/{testName}.input.json");
        truthData = System.IO.File.ReadAllText($"./data/{testName}.truth.json");
    }

    private void Test(string inputData, string truthData)
    {
        Dictionary<string, object?>? _jsonObject = Json.Deserialize<Dictionary<string, object?>>(inputData);
        Dictionary<string, object?>? truth = JsonSerializer.Deserialize<Dictionary<string, object?>>(truthData);

        Assert.That(Json.Serialize(truth!), Is.EqualTo(Json.Serialize(_jsonObject!)));
    }

    [Test]
    public void Test1()
    {
        LoadData(out string inputData, out string truthData);
        Test(inputData, truthData);
    }

    [Test]
    public void Test2()
    {
        LoadData(out string inputData, out string truthData);
        Test(inputData, truthData);
    }

    [Test]
    public void Test3()
    {
        LoadData(out string inputData, out string truthData);
        Test(inputData, truthData);
    }
}
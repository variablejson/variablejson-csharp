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

        string truthString = JsonSerializer.Serialize(truth);
        string inputString = Json.Serialize(_jsonObject);

        Assert.That(inputString, Is.EqualTo(truthString));
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

    [Test]
    public void Test4()
    {
        LoadData(out string inputData, out string truthData);
        Assert.Throws<KeyNotFoundException>(() => Test(inputData, truthData));
    }

    [Test]
    public void Test5()
    {
        LoadData(out string inputData, out string truthData);
        StackOverflowException? ex = Assert.Throws<StackOverflowException>(() => Test(inputData, truthData));

        Assert.NotNull(ex);
        Assert.That(ex!.Message, Is.EqualTo("Max recursion reached."));
    }

    [Test]
    public void Test6()
    {
        LoadData(out string inputData, out string truthData);
        Test(inputData, truthData);
    }

    [Test]
    public void Test7()
    {
        LoadData(out string inputData, out string truthData);
        Test(inputData, truthData);
    }

    [Test]
    public void Test8()
    {
        LoadData(out string inputData, out string truthData);

        VariableJsonOptions options = new()
        {
            VariableKey = "$variables"
        };

        Dictionary<string, object?>? _jsonObject = Json.Deserialize<Dictionary<string, object?>>(inputData, options);
        Dictionary<string, object?>? truth = JsonSerializer.Deserialize<Dictionary<string, object?>>(truthData);

        string truthString = JsonSerializer.Serialize(truth);
        string inputString = Json.Serialize(_jsonObject);

        Assert.That(inputString, Is.EqualTo(truthString));
    }

    [Test]
    public void Test9()
    {
        LoadData(out string inputData, out string truthData);

        VariableJsonOptions options = new()
        {
            KeepVars = true
        };

        Dictionary<string, object?>? _jsonObject = Json.Deserialize<Dictionary<string, object?>>(inputData, options);
        Dictionary<string, object?>? truth = JsonSerializer.Deserialize<Dictionary<string, object?>>(truthData);

        string truthString = JsonSerializer.Serialize(truth);
        string inputString = Json.Serialize(_jsonObject);

        Assert.That(inputString, Is.EqualTo(truthString));
    }

    [Test]
    public void Test10()
    {
        LoadData(out string inputData, out string truthData);
        Exception? ex = Assert.Throws<JsonException>(() => Test(inputData, truthData));
    }

    [Test]
    public void Test11()
    {
        LoadData(out string inputData, out string truthData);
        KeyNotFoundException? ex = Assert.Throws<KeyNotFoundException>(() => Test(inputData, truthData));

        Assert.NotNull(ex);
        Assert.That(ex!.Message, Is.EqualTo("Variable john.name not found."));
    }
}
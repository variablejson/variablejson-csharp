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

        Assert.That(truthString, Is.EqualTo(inputString));
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
        Exception? ex = Assert.Throws<Exception>(() => Test(inputData, truthData));

        Assert.NotNull(ex);
        Assert.That(ex!.Message, Is.EqualTo("Max recursion reached."));
    }
}
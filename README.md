[You can find information about the VariableJson project in the community repo.](https://github.com/variablejson/variablejson)

### Adding VariableJson to your project

VariableJson is published to NuGet and can be installed using the following command:

```bash
dotnet add package VariableJson
```

If you need a different build artifact, you can clone this repository and run `dotnet pack` to create a NuGet package or `dotnet build` to create a DLL.

### Usage

To use VariableJson in a .NET C# project you must first add a reference to the `VariableJson` namespace and then call the `Json.Parse()` function, passing in the JSON string to be parsed and any (optional) parser options.

```csharp
using VariableJson;

string json = System.IO.File.ReadAllText("example.json");
string parsedJson = Json.Parse(json); // "Json" is part of the "VariableJson" namespace
```

`parsedJson` is now the parsed version of the `example.json` file. You can then use the `parsedJson` string as you would any other JSON string, such as deserializing it into a C# object using `System.Text.Json`, `Newtonsoft.Json`, or any other JSON library.

> **Note**
> These examples show us loading JSON data in by reading a file, but how you get your JSON data is up to you. You can load it from a file, a database, a web service, or any other source, just as you'd expect.

### VariableJsonOptions

If you need to change any of the default parser options, you can pass in a `VariableJsonOptions` object to the `Json.Parse()` function.

```csharp
using VariableJson;

string json = System.IO.File.ReadAllText("example.json");

VariableJsonOptions options = new VariableJsonOptions();
options.KeepVars = true;

string parsedJson = Json.Parse(json, options);
```

In this example, we set the `KeepVars` option to `true`. This will cause the variable container to be kept in the output. The variable container will **not** be parsed, it will remain an identical copy of the one from the input JSON.
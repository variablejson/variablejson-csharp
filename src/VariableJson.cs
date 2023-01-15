using System.Collections;
using System.Text.Json;

namespace VariableJson;

public static class Json
{
    public static string Serialize(object? obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    public static T? Deserialize<T>(string json, VariableJsonOptions? options = default(VariableJsonOptions))
    {
        return JsonSerializer.Deserialize<T>(Parse(json, options));
    }

    public static string Parse(string json, VariableJsonOptions? options = default(VariableJsonOptions))
    {
        return JsonSerializer.Serialize(new VariableJsonParser(json, options).Parse());
    }
}

public class VariableJsonOptions
{
    public string VariableKey = "$vars";
    public string Delimiter { get; set; } = ".";
    public int MaxRecurse { get; set; } = 1024;
    public bool KeepVars { get; set; } = false;
    public string EmittedName { get; set; } = "$vars";
}

internal class VariableJsonParser
{
    private readonly string json;
    private readonly VariableJsonOptions options;
    private readonly Dictionary<string, object>? jsonObject;
    private readonly Dictionary<string, object>? variables = new();
    private Dictionary<string, object> outObject = new();
    private int recurse = 0;

    public VariableJsonParser(string json, VariableJsonOptions? options)
    {
        this.json = json;
        this.options = options ?? new VariableJsonOptions();

        Dictionary<string, object>? _jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        if (_jsonObject!.ContainsKey(this.options.VariableKey))
        {
            variables = JsonSerializer.Deserialize<Dictionary<string, object>>(Json.Serialize(_jsonObject[this.options.VariableKey]));

            _jsonObject.Remove(this.options.VariableKey);
        }

        jsonObject = _jsonObject;
    }

    internal Dictionary<string, object>? Parse()
    {
        if (variables!.Count == 0)
        {
            return jsonObject;
        }

        ParseDFS(jsonObject, outObject);

        if (options.KeepVars)
        {
            outObject.Add(options.EmittedName, variables);
        }

        return outObject;
    }

    internal void ParseDFS(object? node, ICollection? outNode, string path = "")
    {
        if (node is JsonElement)
        {
            if (((JsonElement)node).ValueKind == JsonValueKind.Object)
            {
                node = ((JsonElement)node).Deserialize<Dictionary<string, object>>();
            }
            else if (((JsonElement)node).ValueKind == JsonValueKind.Array)
            {
                node = ((JsonElement)node).Deserialize<List<object>>();
            }
        }

        if (node is IDictionary)
        {
            foreach (DictionaryEntry entry in (IDictionary)node)
            {
                ParseDFS(entry, outNode, path);
            }
        }
        else if (node is IList)
        {
            for (int i = 0; i < ((IList)node).Count; i++)
            {
                InsertNode((List<object?>?)outNode, $"{path}{options.Delimiter}{i}", ((IList)node)[i]);
            }
        }
        else if (node is DictionaryEntry)
        {
            DictionaryEntry entry = (DictionaryEntry)node;
            InsertNode((Dictionary<string, object?>?)outNode, $"{path}{options.Delimiter}{entry.Key}", entry.Value);
        }
    }

    internal void InsertNode(ICollection? node, string path, object? value)
    {
        if (IsRef(value, out string? variable))
        {
            recurse = 0;
            if (!FindRef(variable!, out value))
            {
                throw new KeyNotFoundException($"Variable {variable} not found.");
            }
        }

        ParsePath(path, out string[] parts, out string key);

        if (value is null)
        {
            InsertNodeUntyped(node, null, key);
        }
        else if (((JsonElement)value!).ValueKind == JsonValueKind.Object)
        {
            Dictionary<string, object?>? vnode = new();
            ParseDFS(value, vnode);
            InsertNodeUntyped(node, vnode, key);
        }
        else if (((JsonElement)value!).ValueKind == JsonValueKind.Array)
        {
            List<object?> vnode = new();
            ParseDFS(value, vnode);
            InsertNodeUntyped(node, vnode, key);
        }
        else
        {
            InsertNodeUntyped(node, value, key);
        }
    }

    internal void InsertNodeUntyped(ICollection? node, object? value, string key = "")
    {
        if (node is IDictionary)
        {
            ((IDictionary)node).Add(key, value);
        }
        else if (node is IList)
        {
            ((IList)node).Add(value);
        }
    }

    internal void ParsePath(string path, out string[] parts, out string key)
    {
        if (path.StartsWith(options.Delimiter))
        {
            path = path[(options.Delimiter.Length)..];
        }

        parts = path.Split(options.Delimiter);
        key = parts[^1];
        parts = parts[..^1];
    }

    internal bool IsRef(object? value, out string? variable)
    {
        variable = null;

        if ((value as JsonElement?)?.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        bool isRef = value!.ToString()!.Length > 3 && value.ToString()!.StartsWith("$(") && value.ToString()!.EndsWith(")");

        if (isRef)
        {
            variable = value.ToString()![2..^1];
        }

        return isRef;
    }

    internal bool FindRef(string variable, out object? value)
    {
        ParsePath(variable, out string[] parts, out string key);

        return FindRefDFS(variables!, parts, key, out value);
    }

    internal bool FindRefDFS(Dictionary<string, object> node, string[] path, string key, out object? value)
    {
        recurse++;
        if (recurse > options.MaxRecurse)
        {
            throw new StackOverflowException("Max recursion reached.");
        }

        if (path.Length == 0 && node.ContainsKey(key))
        {
            if (IsRef(node[key], out string? variable))
            {
                return FindRef(variable!, out value);
            }

            value = node[key];
            return true;
        }

        if (path.Length > 0 && node.ContainsKey(path[0]))
        {
            return FindRefDFS(((JsonElement)node[path[0]]!).Deserialize<Dictionary<string, object?>>()!, path[1..], key, out value);
        }

        value = null;
        return false;
    }
}
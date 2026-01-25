using System.Text.Json.Serialization;

namespace VopPico.App.Interfaces;

[JsonSourceGenerationOptions(WriteIndented = true)]
//[JsonSerializable(typeof(PicoJsInterface.ComputationResult))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(Dictionary<string, string>))]
internal partial class VopHybridJSContext : JsonSerializerContext
{
    // This type's attributes specify JSON serialization info to preserve type structure
    // for trimmed builds.
}
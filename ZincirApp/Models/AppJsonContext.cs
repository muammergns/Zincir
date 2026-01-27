namespace ZincirApp.Models;

using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNameCaseInsensitive = true,
    PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate)]
[JsonSerializable(typeof(AppSettings))]
public partial class AppJsonContext : JsonSerializerContext
{
}
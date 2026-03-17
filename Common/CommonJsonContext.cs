using System.Text.Json.Serialization;

namespace Common;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified)]
[JsonSerializable(typeof(VersionCheckBody))]
internal partial class CommonJsonContext : JsonSerializerContext { }

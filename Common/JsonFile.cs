using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Json
{
	public static class JsonFile
	{
		const int BufferSize = 4096;

		[JsonIgnore]
		public static JsonSerializerOptions DefaultSerializerOptions { get; } = new()
		{
			WriteIndented = true,
			AllowTrailingCommas = true
		};

		public static async Task SerializeToFileAsync<T>(T obj, string filePath, JsonSerializerOptions? options = null)
		{
			await using (var fileStream = new FileStream(
				filePath,
				FileMode.Create,
				FileAccess.Write,
				FileShare.None,
				bufferSize: BufferSize,
				useAsync: true))
			{
				await JsonSerializer.SerializeAsync(fileStream, obj, options ?? DefaultSerializerOptions).ConfigureAwait(false);
			}
		}

		public static async Task<T?> DeserializeFromFileAsync<T>(string filePath, JsonSerializerOptions? options = null)
		{
			await using (var fileStream = new FileStream(
				filePath,
				FileMode.Open,
				FileAccess.Read,
				FileShare.Read,
				bufferSize: BufferSize,
				useAsync: true))
			{
				return await JsonSerializer.DeserializeAsync<T?>(fileStream, options ?? DefaultSerializerOptions).ConfigureAwait(false);
			}
		}
	}
}

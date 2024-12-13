using System.Text.Json;

namespace Common.Json
{
	public static class JsonFile
	{
		const int BufferSize = 4096;

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
				await JsonSerializer.SerializeAsync(fileStream, obj, options).ConfigureAwait(false);
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
				return await JsonSerializer.DeserializeAsync<T?>(fileStream, options).ConfigureAwait(false);
			}
		}
	}
}

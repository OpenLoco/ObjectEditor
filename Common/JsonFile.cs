using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Json;

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
		var dir = Path.GetDirectoryName(filePath);
		if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
		{
			_ = Directory.CreateDirectory(dir);
		}

		var tempFile = Path.Combine(dir ?? string.Empty, $"{Path.GetFileName(filePath)}.{Guid.NewGuid():N}.tmp");
		try
		{
			await using (var fileStream = new FileStream(
				tempFile,
				FileMode.CreateNew,
				FileAccess.Write,
				FileShare.None,
				bufferSize: BufferSize,
				useAsync: true))
			{
				await JsonSerializer.SerializeAsync(fileStream, obj, options ?? DefaultSerializerOptions).ConfigureAwait(false);
				await fileStream.FlushAsync().ConfigureAwait(false);
			}

			File.Move(tempFile, filePath, overwrite: true);
		}
		finally
		{
			if (File.Exists(tempFile))
			{
				try
				{
					File.Delete(tempFile);
				}
				catch
				{
					// Best-effort cleanup of temporary file
				}
			}
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

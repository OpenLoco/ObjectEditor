using OpenLoco.Common.Logging;
using OpenLoco.Definitions.DTO;
using System.IO.Hashing;

namespace OpenLoco.Definitions.Web
{
	public static class Client
	{
		public static async Task<IEnumerable<DtoObjectEntry>> GetObjectListAsync(HttpClient client, ILogger? logger = null)
			=> await ClientHelpers.GetAsync<IEnumerable<DtoObjectEntry>>(
				client,
				Routes.Objects,
				null,
				logger) ?? [];

		public static async Task<DtoObjectDescriptor?> GetObjectAsync(HttpClient client, int id, ILogger? logger = null)
			=> await ClientHelpers.GetAsync<DtoObjectDescriptor>(
				client,
				Routes.Objects,
				id,
				logger);

		public static async Task<byte[]?> GetObjectFileAsync(HttpClient client, int id, ILogger? logger = null)
			=> await ClientHelpers.SendRequestAsync(
				client,
				Routes.Objects + $"/{id}/file",
				() => client.GetAsync(Routes.Objects + $"/{id}/file"),
				ClientHelpers.ReadBinaryContentAsync,
				logger) ?? default;

		public static async Task UploadDatFileAsync(HttpClient client, string filename, byte[] datFileBytes, DateTimeOffset creationDate, DateTimeOffset modifiedDate, ILogger logger)
		{
			var xxHash3 = XxHash3.HashToUInt64(datFileBytes);
			logger.Debug($"Posting {filename} to {client.BaseAddress?.OriginalString}{Routes.Objects}");
			var request = new DtoUploadDat(Convert.ToBase64String(datFileBytes), xxHash3, creationDate, modifiedDate);
			_ = await ClientHelpers.PostAsync(client, Routes.Objects, request);
		}
	}
}

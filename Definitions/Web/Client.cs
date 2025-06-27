using Definitions;
using OpenLoco.Common.Logging;
using OpenLoco.Definitions.DTO;
using System.IO.Hashing;

namespace OpenLoco.Definitions.Web
{
	public static class Client
	{
		public const string ApiVersion = ApiVersionRoutePrefix.V2;

		public static async Task<IEnumerable<DtoObjectEntry>> GetObjectListAsync(HttpClient client, ILogger? logger = null)
			=> await ClientHelpers.GetAsync<IEnumerable<DtoObjectEntry>>(
				client,
				ApiVersion,
				RoutesV2.Objects,
				null,
				logger) ?? [];

		public static async Task<DtoObjectDescriptor?> GetObjectAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null)
			=> await ClientHelpers.GetAsync<DtoObjectDescriptor>(
				client,
				ApiVersion,
				RoutesV2.Objects,
				id,
				logger);

		public static async Task<byte[]?> GetObjectFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null)
			=> await ClientHelpers.SendRequestAsync(
				client,
				ApiVersion + RoutesV2.Objects + $"/{id}/file",
				() => client.GetAsync(RoutesV2.Objects + $"/{id}/file"),
				ClientHelpers.ReadBinaryContentAsync,
				logger) ?? default;

		public static async Task UploadDatFileAsync(HttpClient client, string filename, byte[] datFileBytes, DateTimeOffset creationDate, DateTimeOffset modifiedDate, ILogger logger)
		{
			var xxHash3 = XxHash3.HashToUInt64(datFileBytes);
			logger.Debug($"Posting {filename} to {client.BaseAddress?.OriginalString}{RoutesV2.Objects}");
			var request = new DtoUploadDat(Convert.ToBase64String(datFileBytes), xxHash3, ObjectAvailability.Available, creationDate, modifiedDate);
			_ = await ClientHelpers.PostAsync(
				client,
				ApiVersion,
				RoutesV2.Objects,
				request);
		}
	}
}

using Definitions.DTO;
using Definitions.DTO.Identity;
using Microsoft.Extensions.Logging;
using System.IO.Hashing;
using System.Net.Http.Json;

namespace Definitions.Web;

public static class Client
{
	public const string ApiVersion = Routes.Prefix;

	public static ApiEndpointGroup ObjectsEndpointGroup { get; } = new(Routes.Objects);
	public static ApiEndpointGroup ObjectPacksEndpointGroup { get; } = new(Routes.ObjectPacks);
	public static ApiEndpointGroup ScenariosEndpointGroup { get; } = new(Routes.Scenarios);
	public static ApiEndpointGroup ScenarioPacksEndpointGroup { get; } = new(Routes.ScenarioPacks);
	public static ApiEndpointGroup AuthorsEndpointGroup { get; } = new(Routes.Authors);
	public static ApiEndpointGroup TagsEndpointGroup { get; } = new(Routes.Tags);
	public static ApiEndpointGroup LicencesEndpointGroup { get; } = new(Routes.Licences);
	public static ApiEndpointGroup MissingObjectsEndpointGroup { get; } = new(Routes.Objects + Routes.Missing);
	public static ApiEndpointGroup MusicEndpointGroup { get; } = new(Routes.Music);
	public static ApiEndpointGroup SoundEffectsEndpointGroup { get; } = new(Routes.SoundEffects);
	public static ApiEndpointGroup TutorialsEndpointGroup { get; } = new(Routes.Tutorials);
	public static ApiEndpointGroup GraphicsEndpointGroup { get; } = new(Routes.Graphics);
	public static ApiEndpointGroup UsersEndpointGroup { get; } = new(Routes.Users);
	public static ApiEndpointGroup RolesEndpointGroup { get; } = new(Routes.Roles);
	public static ApiEndpointGroup ServerEndpointGroup { get; } = new(Routes.Server);

	public static async Task<IEnumerable<T>> GetListAsync<T>(HttpClient client, ApiEndpointGroup endpointGroup, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.GetAsync<IEnumerable<T>>(
			client,
			endpointGroup.Prefix,
			endpointGroup.Route,
			null,
			logger,
			cancellationToken) ?? [];

	public static async Task<IEnumerable<DtoObjectEntry>> GetObjectListAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoObjectEntry>(client, ObjectsEndpointGroup, logger, cancellationToken);

	/// <summary>
	/// Queries the server's read-only state (public, unauthenticated route). Returns null when the
	/// status could not be retrieved (e.g. the server is unreachable).
	/// </summary>
	public static async Task<DtoServerStatus?> GetServerStatusAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.GetAsync<DtoServerStatus>(
			client,
			ApiVersion,
			Routes.Server + Routes.Status,
			logger: logger,
			cancellationToken: cancellationToken);

	public static async Task<DtoObjectPostResponse?> GetObjectAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.GetAsync<DtoObjectPostResponse>(
			client,
			ApiVersion,
			Routes.Objects,
			id,
			logger,
			cancellationToken);

	public static async Task<DtoObjectPostResponse?> UpdateObjectAsync(HttpClient client, UniqueObjectId id, DtoObjectPostResponse request, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			client,
			ApiVersion,
			Routes.Objects,
			id,
			request,
			logger,
			cancellationToken);

	public static async Task<byte[]?> GetObjectFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + Routes.Objects + $"/{id}/file",
			ct => client.GetAsync(ApiVersion + Routes.Objects + $"/{id}/file", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	public static async Task<byte[]?> GetObjectImagesAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + Routes.Objects + $"/{id}{Routes.Images}",
			ct => client.GetAsync(ApiVersion + Routes.Objects + $"/{id}{Routes.Images}", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	public static async Task<byte[]?> GetObjectImageAsync(HttpClient client, UniqueObjectId id, int imageId, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + Routes.Objects + $"/{id}{Routes.Images}/{imageId}",
			ct => client.GetAsync(ApiVersion + Routes.Objects + $"/{id}{Routes.Images}/{imageId}", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	public static async Task<byte[]?> GetScenarioFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + Routes.Scenarios + $"/{id}/file",
			ct => client.GetAsync(ApiVersion + Routes.Scenarios + $"/{id}/file", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	public static async Task<byte[]?> GetScenarioPackFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + Routes.ScenarioPacks + $"/{id}/file",
			ct => client.GetAsync(ApiVersion + Routes.ScenarioPacks + $"/{id}/file", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	public static async Task<byte[]?> GetObjectPackFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + Routes.ObjectPacks + $"/{id}/file",
			ct => client.GetAsync(ApiVersion + Routes.ObjectPacks + $"/{id}/file", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	public static async Task<DtoObjectPostResponse?> UploadDatFileAsync(HttpClient client, string filename, byte[] datFileBytes, DateOnly creationDate, DateOnly modifiedDate, ILogger logger, CancellationToken cancellationToken = default)
	{
		var xxHash3 = XxHash3.HashToUInt64(datFileBytes);
		logger.LogDebug("Posting {Filename} to {OriginalString}{Objects}", filename, client.BaseAddress?.OriginalString, Routes.Objects);
		var request = new DtoObjectPost(Convert.ToBase64String(datFileBytes), xxHash3, ObjectAvailability.Available, creationDate, modifiedDate);
		return await ClientHelpers.PostAsync<DtoObjectPost, DtoObjectPostResponse>(
			client,
			ApiVersion,
			Routes.Objects,
			request,
			logger,
			cancellationToken);
	}

	public static async Task<DtoObjectMissingEntry?> AddMissingObjectAsync(HttpClient client, DtoObjectMissingPost entry, ILogger? logger = null, CancellationToken cancellationToken = default)
	{
		logger?.LogDebug("Posting missing object {DatName} with checksum {DatChecksum} to {OriginalString}{Objects}{Missing}", entry.DatName, entry.DatChecksum, client.BaseAddress?.OriginalString, Routes.Objects, Routes.Missing);
		return await ClientHelpers.PostAsync<DtoObjectMissingPost, DtoObjectMissingEntry>(
			client,
			ApiVersion,
			Routes.Objects + Routes.Missing,
			entry,
			logger,
			cancellationToken);
	}

	public static async Task<IEnumerable<DtoLicenceEntry>> GetLicencesAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoLicenceEntry>(client, LicencesEndpointGroup, logger, cancellationToken);

	public static async Task<IEnumerable<DtoAuthorEntry>> GetAuthorsAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoAuthorEntry>(client, AuthorsEndpointGroup, logger, cancellationToken);

	public static async Task<IEnumerable<DtoTagEntry>> GetTagsAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoTagEntry>(client, TagsEndpointGroup, logger, cancellationToken);

	public static async Task<IEnumerable<DtoItemPackEntry>> GetObjectPacksAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoItemPackEntry>(client, ObjectPacksEndpointGroup, logger, cancellationToken);

	public static async Task<DtoItemPackDescriptor<DtoObjectEntry>?> GetObjectPackAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.GetAsync<DtoItemPackDescriptor<DtoObjectEntry>>(
			client,
			ApiVersion,
			Routes.ObjectPacks,
			id,
			logger,
			cancellationToken);

	public static async Task<IEnumerable<DtoScenarioListEntry>> GetScenariosAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoScenarioListEntry>(client, ScenariosEndpointGroup, logger, cancellationToken);

	public static async Task<DtoScenarioDescriptor?> GetScenarioAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.GetAsync<DtoScenarioDescriptor>(
			client,
			ApiVersion,
			Routes.Scenarios,
			id,
			logger,
			cancellationToken);

	public static async Task<IEnumerable<DtoItemPackEntry>> GetScenarioPacksAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoItemPackEntry>(client, ScenarioPacksEndpointGroup, logger, cancellationToken);

	public static async Task<DtoItemPackDescriptor<DtoScenarioEntry>?> GetScenarioPackAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.GetAsync<DtoItemPackDescriptor<DtoScenarioEntry>>(
			client,
			ApiVersion,
			Routes.ScenarioPacks,
			id,
			logger,
			cancellationToken);

	public static async Task<IEnumerable<DtoObjectMissingEntry>> GetMissingObjectsAsync(HttpClient client, ILogger? logger = null)
		=> await GetListAsync<DtoObjectMissingEntry>(client, MissingObjectsEndpointGroup, logger);

	#region Generic CRUD

	public static async Task<T?> GetResourceAsync<T>(HttpClient client, ApiEndpointGroup endpointGroup, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		where T : class
		=> await ClientHelpers.GetAsync<T>(client, endpointGroup.Prefix, endpointGroup.Route, id, logger, cancellationToken);

	public static async Task<TResponse?> CreateResourceAsync<TRequest, TResponse>(HttpClient client, ApiEndpointGroup endpointGroup, TRequest request, ILogger? logger = null, CancellationToken cancellationToken = default)
		where TResponse : class
		=> await ClientHelpers.PostAsync<TRequest, TResponse>(client, endpointGroup.Prefix, endpointGroup.Route, request, logger, cancellationToken);

	public static async Task<TResponse?> UpdateResourceAsync<TRequest, TResponse>(HttpClient client, ApiEndpointGroup endpointGroup, UniqueObjectId id, TRequest request, ILogger? logger = null, CancellationToken cancellationToken = default)
		where TResponse : class
		=> await ClientHelpers.PutAsync<TRequest, TResponse>(client, endpointGroup.Prefix, endpointGroup.Route, id, request, logger, cancellationToken);

	public static async Task<bool> DeleteResourceAsync(HttpClient client, ApiEndpointGroup endpointGroup, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.DeleteAsync(client, endpointGroup.Prefix, endpointGroup.Route, id, logger, cancellationToken);

	#endregion

	#region Reference data descriptors

	public static async Task<DtoAuthorDescriptor?> GetAuthorDescriptorAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetDescriptorAsync<DtoAuthorDescriptor>(client, Routes.Authors, id, logger, cancellationToken);

	public static async Task<DtoTagDescriptor?> GetTagDescriptorAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetDescriptorAsync<DtoTagDescriptor>(client, Routes.Tags, id, logger, cancellationToken);

	public static async Task<DtoLicenceDescriptor?> GetLicenceDescriptorAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetDescriptorAsync<DtoLicenceDescriptor>(client, Routes.Licences, id, logger, cancellationToken);

	static async Task<TDescriptor?> GetDescriptorAsync<TDescriptor>(HttpClient client, string baseRoute, UniqueObjectId id, ILogger? logger, CancellationToken cancellationToken)
		where TDescriptor : class
	{
		var route = ApiVersion + baseRoute + $"/{id}" + Routes.Descriptor;
		return await ClientHelpers.SendRequestAsync(
			client,
			route,
			ct => client.GetAsync(route, ct),
			ClientHelpers.ReadJsonContentAsync<TDescriptor>,
			logger,
			cancellationToken);
	}

	#endregion

	#region Object packs

	public static async Task<IEnumerable<DtoObjectPackListEntry>> GetObjectPackListEntriesAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoObjectPackListEntry>(client, ObjectPacksEndpointGroup, logger, cancellationToken);

	public static async Task<DtoObjectPackDescriptor?> GetObjectPackDescriptorAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetDescriptorAsync<DtoObjectPackDescriptor>(client, Routes.ObjectPacks, id, logger, cancellationToken);

	public static async Task<DtoObjectPackDescriptor?> UpdateObjectPackAsync(HttpClient client, DtoObjectPackDescriptor request, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await UpdateResourceAsync<DtoObjectPackDescriptor, DtoObjectPackDescriptor>(client, ObjectPacksEndpointGroup, request.Id, request, logger, cancellationToken);

	public static Task<bool> DeleteObjectPackAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> DeleteResourceAsync(client, ObjectPacksEndpointGroup, id, logger, cancellationToken);

	#endregion

	#region Scenario (SC5) file packs

	public static async Task<IEnumerable<DtoScenarioPackListEntry>> GetScenarioPackListEntriesAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoScenarioPackListEntry>(client, ScenarioPacksEndpointGroup, logger, cancellationToken);

	public static async Task<DtoScenarioPackDescriptor?> GetScenarioPackDescriptorAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetDescriptorAsync<DtoScenarioPackDescriptor>(client, Routes.ScenarioPacks, id, logger, cancellationToken);

	public static async Task<DtoScenarioPackDescriptor?> UpdateScenarioPackAsync(HttpClient client, DtoScenarioPackDescriptor request, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await UpdateResourceAsync<DtoScenarioPackDescriptor, DtoScenarioPackDescriptor>(client, ScenarioPacksEndpointGroup, request.Id, request, logger, cancellationToken);

	public static Task<bool> DeleteScenarioPackAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> DeleteResourceAsync(client, ScenarioPacksEndpointGroup, id, logger, cancellationToken);

	#endregion

	#region Scenario files

	public static async Task<DtoScenarioDescriptor?> UpdateScenarioAsync(HttpClient client, DtoScenarioDescriptor request, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await UpdateResourceAsync<DtoScenarioDescriptor, DtoScenarioDescriptor>(client, ScenariosEndpointGroup, request.Id, request, logger, cancellationToken);

	public static Task<bool> DeleteScenarioAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> DeleteResourceAsync(client, ScenariosEndpointGroup, id, logger, cancellationToken);

	#endregion

	#region Objects

	public static async Task<IEnumerable<DtoObjectEntry>> GetMyObjectsAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.GetAsync<IEnumerable<DtoObjectEntry>>(client, ApiVersion, Routes.Objects + Routes.Mine, logger: logger, cancellationToken: cancellationToken) ?? [];

	#endregion

	#region Identity / user management

	public static async Task<IEnumerable<DtoRoleEntry>> GetRolesAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoRoleEntry>(client, RolesEndpointGroup, logger, cancellationToken);

	public static async Task<IEnumerable<DtoUserListEntry>> GetUsersAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.GetAsync<IEnumerable<DtoUserListEntry>>(client, ApiVersion, Routes.Users, logger: logger, cancellationToken: cancellationToken) ?? [];

	public static async Task<DtoUserDetailDescriptor?> GetUserDetailAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.GetAsync<DtoUserDetailDescriptor>(client, ApiVersion, Routes.Users + $"/{id}" + Routes.Detail, logger: logger, cancellationToken: cancellationToken);

	public static Task<DtoUserDetailDescriptor?> ToggleUserRoleAsync(HttpClient client, UniqueObjectId id, string role, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> ClientHelpers.PostAsync<DtoUserRoleRequest, DtoUserDetailDescriptor>(
			client, ApiVersion, Routes.Users + $"/{id}" + Routes.Roles, new DtoUserRoleRequest(role), logger, cancellationToken);

	public static Task<DtoUserDetailDescriptor?> ToggleUserClaimAsync(HttpClient client, UniqueObjectId id, string claim, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> ClientHelpers.PostAsync<DtoUserClaimRequest, DtoUserDetailDescriptor>(
			client, ApiVersion, Routes.Users + $"/{id}" + Routes.ClaimsSubRoute, new DtoUserClaimRequest(claim), logger, cancellationToken);

	public static Task<DtoUserDetailDescriptor?> ToggleUserLockoutAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> ClientHelpers.PostAsync<object, DtoUserDetailDescriptor>(
			client, ApiVersion, Routes.Users + $"/{id}" + Routes.Lockout, new { }, logger, cancellationToken);

	public static Task<DtoUserDetailDescriptor?> ToggleUserEmailConfirmedAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> ClientHelpers.PostAsync<object, DtoUserDetailDescriptor>(
			client, ApiVersion, Routes.Users + $"/{id}" + Routes.EmailConfirmed, new { }, logger, cancellationToken);

	public static Task<DtoPasswordResetTokenResponse?> ForceUserPasswordResetAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> ClientHelpers.PostAsync<object, DtoPasswordResetTokenResponse>(
			client, ApiVersion, Routes.Users + $"/{id}" + Routes.PasswordReset, new { }, logger, cancellationToken);

	public static Task<DtoUserEntry?> SetUserDisplayNameAsync(HttpClient client, UniqueObjectId id, string displayName, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> ClientHelpers.PutAsync<DtoUserEntry, DtoUserEntry>(
			client, ApiVersion, Routes.Users, id, new DtoUserEntry(id, displayName), logger, cancellationToken);

	public static async Task<DtoUserEntry?> SetCurrentUserDisplayNameAsync(HttpClient client, string displayName, ILogger? logger = null, CancellationToken cancellationToken = default)
	{
		var route = ApiVersion + Routes.Users + Routes.Me;
		return await ClientHelpers.SendRequestAsync(
			client,
			route,
			ct => client.PutAsJsonAsync(route, new DtoUserEntry(0, displayName), ct),
			ClientHelpers.ReadJsonContentAsync<DtoUserEntry>,
			logger,
			cancellationToken);
	}

	public static Task<bool> DeleteUserAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> ClientHelpers.DeleteAsync(client, ApiVersion, Routes.Users, id, logger, cancellationToken);

	public static async Task<bool> DeleteCurrentUserAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
	{
		try
		{
			var route = ApiVersion + Routes.Users + Routes.Me;
			using var response = await client.DeleteAsync(route, cancellationToken);
			return response.IsSuccessStatusCode;
		}
		catch (HttpRequestException ex)
		{
			logger?.LogError(ex, "Failed to delete the current user.");
			return false;
		}
	}

	#endregion
	#region Music files

	public static async Task<IEnumerable<DtoMusicListEntry>> GetMusicListEntriesAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoMusicListEntry>(client, MusicEndpointGroup, logger, cancellationToken);

	public static async Task<DtoMusicDescriptor?> GetMusicDescriptorAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetDescriptorAsync<DtoMusicDescriptor>(client, Routes.Music, id, logger, cancellationToken);

	public static async Task<DtoMusicDescriptor?> UpdateMusicAsync(HttpClient client, DtoMusicDescriptor request, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await UpdateResourceAsync<DtoMusicDescriptor, DtoMusicDescriptor>(client, MusicEndpointGroup, request.Id, request, logger, cancellationToken);

	public static Task<bool> DeleteMusicAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> DeleteResourceAsync(client, MusicEndpointGroup, id, logger, cancellationToken);

	public static async Task<byte[]?> GetMusicFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			$"{ApiVersion}{Routes.Music}/{id}{Routes.File}",
			ct => client.GetAsync($"{ApiVersion}{Routes.Music}/{id}{Routes.File}", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	#endregion

	#region Sound effect files

	public static async Task<IEnumerable<DtoSoundEffectListEntry>> GetSoundEffectListEntriesAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoSoundEffectListEntry>(client, SoundEffectsEndpointGroup, logger, cancellationToken);

	public static async Task<DtoSoundEffectDescriptor?> GetSoundEffectDescriptorAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetDescriptorAsync<DtoSoundEffectDescriptor>(client, Routes.SoundEffects, id, logger, cancellationToken);

	public static async Task<DtoSoundEffectDescriptor?> UpdateSoundEffectAsync(HttpClient client, DtoSoundEffectDescriptor request, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await UpdateResourceAsync<DtoSoundEffectDescriptor, DtoSoundEffectDescriptor>(client, SoundEffectsEndpointGroup, request.Id, request, logger, cancellationToken);

	public static Task<bool> DeleteSoundEffectAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> DeleteResourceAsync(client, SoundEffectsEndpointGroup, id, logger, cancellationToken);

	public static async Task<byte[]?> GetSoundEffectFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			$"{ApiVersion}{Routes.SoundEffects}/{id}{Routes.File}",
			ct => client.GetAsync($"{ApiVersion}{Routes.SoundEffects}/{id}{Routes.File}", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	#endregion


	#region Tutorial files

	public static async Task<IEnumerable<DtoTutorialListEntry>> GetTutorialListEntriesAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoTutorialListEntry>(client, TutorialsEndpointGroup, logger, cancellationToken);

	public static async Task<DtoTutorialDescriptor?> GetTutorialDescriptorAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetDescriptorAsync<DtoTutorialDescriptor>(client, Routes.Tutorials, id, logger, cancellationToken);

	public static async Task<DtoTutorialDescriptor?> UpdateTutorialAsync(HttpClient client, DtoTutorialDescriptor request, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await UpdateResourceAsync<DtoTutorialDescriptor, DtoTutorialDescriptor>(client, TutorialsEndpointGroup, request.Id, request, logger, cancellationToken);

	public static Task<bool> DeleteTutorialAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> DeleteResourceAsync(client, TutorialsEndpointGroup, id, logger, cancellationToken);

	public static async Task<byte[]?> GetTutorialFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			$"{ApiVersion}{Routes.Tutorials}/{id}{Routes.File}",
			ct => client.GetAsync($"{ApiVersion}{Routes.Tutorials}/{id}{Routes.File}", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	#endregion

	#region Graphics files

	public static async Task<IEnumerable<DtoGraphicsListEntry>> GetGraphicsListEntriesAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoGraphicsListEntry>(client, GraphicsEndpointGroup, logger, cancellationToken);

	public static async Task<DtoGraphicsDescriptor?> GetGraphicsDescriptorAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetDescriptorAsync<DtoGraphicsDescriptor>(client, Routes.Graphics, id, logger, cancellationToken);

	public static async Task<DtoGraphicsDescriptor?> UpdateGraphicsAsync(HttpClient client, DtoGraphicsDescriptor request, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await UpdateResourceAsync<DtoGraphicsDescriptor, DtoGraphicsDescriptor>(client, GraphicsEndpointGroup, request.Id, request, logger, cancellationToken);

	public static Task<bool> DeleteGraphicsAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> DeleteResourceAsync(client, GraphicsEndpointGroup, id, logger, cancellationToken);

	public static async Task<byte[]?> GetGraphicsFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			$"{ApiVersion}{Routes.Graphics}/{id}{Routes.File}",
			ct => client.GetAsync($"{ApiVersion}{Routes.Graphics}/{id}{Routes.File}", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	#endregion


}

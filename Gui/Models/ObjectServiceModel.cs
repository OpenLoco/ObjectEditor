using Avalonia.Threading;
using Definitions.DTO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gui.Models;

// Caches "global" server-side reference data (licences, authors, tags, object packs) that is
// shared across all viewmodels. The data is fetched once when the model is constructed; call
// ReloadAsync() to refresh it explicitly.
public class ObjectServiceModel
{
	readonly ObjectServiceClient client;
	readonly ILogger logger;
	readonly SemaphoreSlim loadLock = new(1, 1);

	public ObservableCollection<DtoLicenceEntry?> AvailableLicences { get; } = [];
	public ObservableCollection<DtoAuthorEntry> AvailableAuthors { get; } = [];
	public ObservableCollection<DtoTagEntry> AvailableTags { get; } = [];
	public ObservableCollection<DtoItemPackEntry> AvailableObjectPacks { get; } = [];

	public Task InitialLoadTask { get; }

	public ObjectServiceModel(ObjectServiceClient client, ILogger logger)
	{
		this.client = client;
		this.logger = logger;
		InitialLoadTask = ReloadAsync();
	}

	public async Task ReloadAsync()
	{
		if (client == null)
		{
			return;
		}

		await loadLock.WaitAsync();
		try
		{
			List<DtoLicenceEntry?> licences;
			List<DtoAuthorEntry> authors;
			List<DtoTagEntry> tags;
			List<DtoItemPackEntry> packs;
			try
			{
				var licenceList = new List<DtoLicenceEntry?> { null }; // None option
				licenceList.AddRange(await client.GetLicencesAsync());
				licences = [.. licenceList.OrderBy(x => x?.Name)];

				authors = [.. (await client.GetAuthorsAsync()).OrderBy(x => x.Name)];
				tags = [.. (await client.GetTagsAsync()).OrderBy(x => x.Name)];
				packs = [.. (await client.GetObjectPacksAsync()).OrderBy(x => x.Name)];
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to load server metadata (licences/authors/tags/object packs)");
				licences = [null];
				authors = [];
				tags = [];
				packs = [];
			}

			Dispatcher.UIThread.Post(() =>
			{
				Replace(AvailableLicences, licences);
				Replace(AvailableAuthors, authors);
				Replace(AvailableTags, tags);
				Replace(AvailableObjectPacks, packs);
			});
		}
		finally
		{
			_ = loadLock.Release();
		}
	}

	static void Replace<T>(ObservableCollection<T> target, IEnumerable<T> source)
	{
		target.Clear();
		foreach (var item in source)
		{
			target.Add(item);
		}
	}
}

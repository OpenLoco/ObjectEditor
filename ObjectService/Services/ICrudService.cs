using Definitions;

namespace ObjectService.Services;

public interface ICrudService<TDto, TRow>
where TDto : class, IHasId
where TRow : class, IHasId
{
	Task<IEnumerable<TDto>> ListAsync(HttpContext context, CancellationToken ct);
	Task<TDto> CreateAsync(TDto request, CancellationToken ct);
	Task<TDto?> ReadAsync(UniqueObjectId id, CancellationToken ct);
	Task<TDto?> UpdateAsync(UniqueObjectId id, TDto request, CancellationToken ct);
	Task<bool> DeleteAsync(UniqueObjectId id, CancellationToken ct);

	bool TryValidateCreate(TDto request, out string? errorMessage);
}

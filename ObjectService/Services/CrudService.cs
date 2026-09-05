using Definitions;
using Definitions.Database;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Services;

public class CrudService<TDto, TRow> : ICrudService<TDto, TRow>
where TDto : class, IHasId
where TRow : class, IHasId
{
	private readonly LocoDbContext _db;
	private readonly Func<LocoDbContext, DbSet<TRow>> _tableSelector;
	private readonly Func<TRow, TDto> _toDto;
	private readonly Func<TDto, TRow> _toRow;
	private readonly Action<TDto, TRow> _updateRow;
	private readonly Func<TDto, string?> _validate;

	public CrudService(
	LocoDbContext db,
	Func<LocoDbContext, DbSet<TRow>> tableSelector,
	Func<TRow, TDto> toDto,
	Func<TDto, TRow> toRow,
	Action<TDto, TRow> updateRow,
	Func<TDto, string?> validate)
	{
		_db = db;
		_tableSelector = tableSelector;
		_toDto = toDto;
		_toRow = toRow;
		_updateRow = updateRow;
		_validate = validate;
	}

	private DbSet<TRow> Table => _tableSelector(_db);

	public bool TryValidateCreate(TDto request, out string? errorMessage)
	{
		errorMessage = _validate(request);
		return errorMessage == null;
	}

	public async Task<IEnumerable<TDto>> ListAsync(HttpContext context, CancellationToken ct)
	=> await Table.Select(x => _toDto(x)).ToListAsync(ct);

	public async Task<TDto> CreateAsync(TDto request, CancellationToken ct)
	{
		var row = _toRow(request);
		_ = await Table.AddAsync(row, ct);
		_ = await _db.SaveChangesAsync(ct);
		return _toDto(row);
	}

	public async Task<TDto?> ReadAsync(UniqueObjectId id, CancellationToken ct)
	{
		var row = await Table.FindAsync([id], ct);
		return row != null ? _toDto(row) : null;
	}

	public async Task<TDto?> UpdateAsync(UniqueObjectId id, TDto request, CancellationToken ct)
	{
		var row = await Table.FindAsync([id], ct);
		if (row == null)
		{
			return null;
		}

		_updateRow(request, row);
		_ = await _db.SaveChangesAsync(ct);
		return _toDto(row);
	}

	public async Task<bool> DeleteAsync(UniqueObjectId id, CancellationToken ct)
	{
		var row = await Table.FindAsync([id], ct);
		if (row == null)
		{
			return false;
		}

		_ = Table.Remove(row);
		_ = await _db.SaveChangesAsync(ct);
		return true;
	}
}

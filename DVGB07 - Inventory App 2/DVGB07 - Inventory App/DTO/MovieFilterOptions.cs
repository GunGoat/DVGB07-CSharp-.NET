using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.DTO;

/// <summary>
/// Represents filter options specific to movies.
/// </summary>
public class MovieFilterOptions : ProductFilterOptions
{
    private string? _genre;
    private string? _format;
    private int? _minPlaytime;
    private int? _maxPlaytime;

    /// <summary>
    /// Gets or sets the genre filter.
    /// </summary>
    public string? Genre
    {
        get => _genre;
        set => SetProperty(ref _genre, string.IsNullOrWhiteSpace(value) ? null : value, nameof(Genre), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets or sets the format filter.
    /// </summary>
    public string? Format
    {
        get => _format;
        set => SetProperty(ref _format, string.IsNullOrWhiteSpace(value) ? null : value, nameof(Format), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets or sets the minimum playtime filter.
    /// </summary>
    public int? MinPlaytime
    {
        get => _minPlaytime;
        set => SetProperty(ref _minPlaytime, value, nameof(MinPlaytime), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets or sets the maximum playtime filter.
    /// </summary>
    public int? MaxPlaytime
    {
        get => _maxPlaytime;
        set => SetProperty(ref _maxPlaytime, value, nameof(MaxPlaytime), UpdateAnyFilterEnabled);
    }

    protected override void UpdateAnyFilterEnabled()
    {
        AnyFilterEnabled = _name != null || _minPrice != null || _maxPrice != null || _genre != null || _format != null || _minPlaytime != null || _maxPlaytime != null;
    }

    public override void ClearFilters() => (Name, MinPrice, MaxPrice, Genre, Format, MinPlaytime, MaxPlaytime) = (null, null, null, null, null, null, null);
}

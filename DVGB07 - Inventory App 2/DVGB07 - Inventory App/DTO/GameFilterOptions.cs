using System;

namespace DVGB07_Inventory_App.DTO;

/// <summary>
/// Represents filter options specific to games.
/// </summary>
public class GameFilterOptions : ProductFilterOptions
{
    private string? _platform;

    /// <summary>
    /// Gets or sets the platform filter.
    /// </summary>
    public string? Platform
    {
        get => _platform;
        set => SetProperty(ref _platform, string.IsNullOrWhiteSpace(value) ? null : value, nameof(Platform), UpdateAnyFilterEnabled);
    }

    protected override void UpdateAnyFilterEnabled()
    {
        AnyFilterEnabled = _name != null || _minPrice != null || _maxPrice != null || _platform != null;
    }

    public override void ClearFilters() => (Name, MinPrice, MaxPrice, Platform) = (null, null, null, null);
}

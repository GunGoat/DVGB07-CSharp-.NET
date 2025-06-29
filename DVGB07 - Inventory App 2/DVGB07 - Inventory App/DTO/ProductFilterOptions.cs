using DVGB07_Inventory_App.ViewModels;
using System;

namespace DVGB07_Inventory_App.DTO;

/// <summary>
/// Represents the base class for product filter options.
/// </summary>
public abstract class ProductFilterOptions : PropertyChangedBase
{
    protected string? _name;
    protected decimal? _minPrice;
    protected decimal? _maxPrice;
    protected bool _anyFilterEnabled;

    /// <summary>
    /// Gets or sets the name filter.
    /// </summary>
    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, string.IsNullOrWhiteSpace(value) ? null : value, nameof(Name), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets or sets the minimum price filter.
    /// </summary>
    public decimal? MinPrice
    {
        get => _minPrice;
        set => SetProperty(ref _minPrice, value, nameof(MinPrice), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets or sets the maximum price filter.
    /// </summary>
    public decimal? MaxPrice
    {
        get => _maxPrice;
        set => SetProperty(ref _maxPrice, value, nameof(MaxPrice), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Indicates whether any filter is enabled.
    /// </summary>
    public bool AnyFilterEnabled
    {
        get => _anyFilterEnabled;
        set => SetProperty(ref _anyFilterEnabled, value, nameof(AnyFilterEnabled));
    }

    /// <summary>
    /// Updates the AnyFilterEnabled property based on active filters.
    /// </summary>
    protected virtual void UpdateAnyFilterEnabled()
    {
        AnyFilterEnabled = _name != null || _minPrice != null || _maxPrice != null;
    }

    /// <summary>
    /// Clears all applied filters.
    /// </summary>
    public virtual void ClearFilters() => (Name, MinPrice, MaxPrice) = (null, null, null);
}

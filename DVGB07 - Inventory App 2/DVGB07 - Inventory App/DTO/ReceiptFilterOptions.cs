using DVGB07_Inventory_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.DTO;

/// <summary>
/// Represents filter options for searching receipts.
/// </summary>
public class ReceiptFilterOptions : PropertyChangedBase
{
    private DateTime? _startDate;
    private DateTime? _endDate;
    private decimal? _minTotalPrice;
    private decimal? _maxTotalPrice;
    private string? _productName;
    private int? _receiptId;
    private bool _anyFilterEnabled;

    /// <summary>
    /// Gets or sets the start date for filtering receipts.
    /// </summary>
    public DateTime? StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value, nameof(StartDate), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets or sets the end date for filtering receipts.
    /// </summary>
    public DateTime? EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value, nameof(EndDate), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets or sets the minimum total price for filtering receipts.
    /// </summary>
    public decimal? MinTotalPrice
    {
        get => _minTotalPrice;
        set => SetProperty(ref _minTotalPrice, value, nameof(MinTotalPrice), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets or sets the maximum total price for filtering receipts.
    /// </summary>
    public decimal? MaxTotalPrice
    {
        get => _maxTotalPrice;
        set => SetProperty(ref _maxTotalPrice, value, nameof(MaxTotalPrice), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets or sets the product name for filtering receipts.
    /// </summary>
    public string? ProductName
    {
        get => _productName;
        set => SetProperty(ref _productName, value, nameof(ProductName), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets or sets the receipt ID for filtering receipts.
    /// </summary>
    public int? ReceiptId
    {
        get => _receiptId;
        set => SetProperty(ref _receiptId, value, nameof(ReceiptId), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets a value indicating whether any filter is enabled.
    /// </summary>
    public bool AnyFilterEnabled
    {
        get => _anyFilterEnabled;
        set => SetProperty(ref _anyFilterEnabled, value, nameof(AnyFilterEnabled));
    }

    /// <summary>
    /// Updates the AnyFilterEnabled property based on whether any filters are applied.
    /// </summary>
    private void UpdateAnyFilterEnabled()
    {
        AnyFilterEnabled = _startDate != null || _endDate != null || _minTotalPrice != null ||
                           _maxTotalPrice != null || _productName != null || _receiptId != null;
    }

    /// <summary>
    /// Clears all applied filters.
    /// </summary>
    public virtual void ClearFilters() => (StartDate, EndDate, MinTotalPrice, MaxTotalPrice, ProductName, ReceiptId) = (null, null, null, null, null, null);
}
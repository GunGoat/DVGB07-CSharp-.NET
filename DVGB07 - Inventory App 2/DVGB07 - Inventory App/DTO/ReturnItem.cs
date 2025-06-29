using DVGB07_Inventory_App.Models;
using DVGB07_Inventory_App.ViewModels;

namespace DVGB07_Inventory_App.DTO;

/// <summary>
/// Represents an item being returned, including its associated product and receipt details.
/// </summary>
public class ReturnItem : PropertyChangedBase
{
    private int _returnQuantity;

    /// <summary>
    /// Gets or sets the product associated with the return.
    /// </summary>
    public Product Product { get; set; }

    /// <summary>
    /// Gets or sets the receipt item that corresponds to the returned item.
    /// </summary>
    public ReceiptItem ReceiptItem { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product being returned.
    /// The value is clamped between 0 and the maximum quantity allowed based on the receipt item.
    /// </summary>
    public int ReturnQuantity
    {
        get => _returnQuantity;
        set
        {
            // If ReceiptItem is null, default to 0
            int maxQuantity = ReceiptItem?.Quantity ?? 0;
            int newValue = Math.Clamp(value, 0, maxQuantity);
            SetProperty(ref _returnQuantity, newValue, nameof(ReturnQuantity));
        }
    }
}
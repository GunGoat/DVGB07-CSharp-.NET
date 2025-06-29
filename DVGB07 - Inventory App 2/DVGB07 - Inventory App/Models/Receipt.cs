using DVGB07_Inventory_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace DVGB07_Inventory_App.Models;

/// <summary>
/// Represents a receipt for a purchase, containing purchased items and total price.
/// </summary>
public class Receipt : PropertyChangedBase
{
    List<ReceiptItem> _items = new List<ReceiptItem>();

    /// <summary>
    /// Gets or sets the unique identifier for the receipt.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the date of the purchase. Defaults to the current date and time.
    /// </summary>
    public DateTime PurchaseDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the list of items included in the receipt.
    /// </summary>
    public List<ReceiptItem> Items 
    { 
        get => _items; 
        set => SetProperty(ref _items, value, nameof(Items)); 
    }

    /// <summary>
    /// Gets the number of unique products in the receipt.
    /// </summary>
    public int NumberOfProducts => Items.Count();

    /// <summary>
    /// Gets the total price of all items in the receipt.
    /// </summary>
    public decimal TotalPrice => Items.Any() ? Items.Sum(i => i.Quantity * i.ProductPrice) : 0m;

    /// <summary>
    /// Returns a string representation of the receipt, including details of each item.
    /// </summary>
    /// <returns>A formatted string containing receipt details.</returns>
    public override string ToString()
    {
        // Formatting the PurchaseDate to only show Year, Month, and Day
        string purchaseDate = PurchaseDate.ToString("yyyy-MM-dd");

        // Creating a string for the receipt details
        StringBuilder receiptDetails = new StringBuilder();
        receiptDetails.AppendLine($"Receipt ID: {Id}");
        receiptDetails.AppendLine($"Purchase Date: {purchaseDate}");
        receiptDetails.AppendLine($"Number of Products: {NumberOfProducts}");
        receiptDetails.AppendLine($"Total Price: {TotalPrice:C}");  // C format for currency

        // Adding Receipt Items
        receiptDetails.AppendLine("\nItems:");
        foreach (var item in Items)
        {
            receiptDetails.AppendLine(item.ToString());
        }

        return receiptDetails.ToString();
    }
}
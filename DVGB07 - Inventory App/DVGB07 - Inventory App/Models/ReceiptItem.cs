using DVGB07_Inventory_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.Models;

/// <summary>
/// Represents an item in a receipt, detailing the purchased product.
/// </summary>
public class ReceiptItem : PropertyChangedBase
{
    int _quantity;
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public int ProductId { get; set; } 
    
    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string ProductName { get; set; } 
    
    /// <summary>
    /// Gets or sets the price per unit of the product.
    /// </summary>
    public decimal ProductPrice { get; set; }
    
    /// <summary>
    /// Gets or sets the quantity of the product purchased.
    /// </summary>
    public int Quantity 
    { 
        get => _quantity; 
        set => SetProperty(ref _quantity, value, nameof(Quantity)); 
    }
    
    /// <summary>
    /// Returns a string representation of the receipt item, including total price.
    /// </summary>
    /// <returns>A formatted string representing the item.</returns>
    public override string ToString()
    {
        decimal totalPrice = Quantity * ProductPrice;
        return $"{ProductId} {ProductName}: {Quantity} x {ProductPrice:C} = {totalPrice:C}";
    }
}
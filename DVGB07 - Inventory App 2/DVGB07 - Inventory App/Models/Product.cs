using DVGB07_Inventory_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.Models;

/// <summary>
/// Represents a base class for products in the inventory.
/// </summary>
public abstract class Product : PropertyChangedBase
{
    private string? _name;
    private decimal? _price;
    private int _quantityInStock;

    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value, nameof(Name));
    }

    /// <summary>
    /// Gets or sets the price of the product.
    /// </summary>
    public decimal? Price
    {
        get => _price;
        set => SetProperty(ref _price, value, nameof(Price));
    }

    /// <summary>
    /// Gets or sets the quantity of the product in stock.
    /// </summary>
    public int Stock
    {
        get => _quantityInStock;
        set => SetProperty(ref _quantityInStock, value, nameof(Stock));
    }

    /// <summary>
    /// Determines whether the product has valid attributes.
    /// </summary>
    /// <returns>True if valid, otherwise false.</returns>
    public virtual bool IsValid()
    {
        var validName = !string.IsNullOrEmpty(Name);
        var validPrice = Price.HasValue && Price >= 0;
        return validName && validPrice;
    }

    /// <summary>
    /// Creates a copy of the current product instance.
    /// </summary>
    /// <returns>A new instance of <see cref="Product"/> with the same values.</returns>
    public abstract Product Clone();

    /// <summary>
    /// Returns a string representation of the product.
    /// </summary>
    /// <returns>A formatted string with product details.</returns>
    public override string ToString() => $"ID: {Id}, Name: {Name}, Price: {Price:C}, Stock: {Stock}";
}
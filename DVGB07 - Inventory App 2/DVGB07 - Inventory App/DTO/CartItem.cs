using DVGB07_Inventory_App.Models;
using DVGB07_Inventory_App.ViewModels;
using System;

namespace DVGB07_Inventory_App.DTO;

/// <summary>
/// Represents an item in the store's purchase cart.
/// This class holds information about a product and its quantity in the cart.
/// </summary>
public class CartItem : PropertyChangedBase
{
    private int _quantity;
    private Product _product;

    /// <summary>
    /// Gets or sets the quantity of the product in the cart.
    /// </summary>
    public int Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value, nameof(Quantity));
    }

    /// <summary>
    /// Gets the type of the product as a string.
    /// </summary>
    public string ProductType => Product?.GetType().Name ?? "Unknown";

    /// <summary>
    /// Gets or sets the product associated with this cart item.
    /// </summary>
    public Product Product
    {
        get => _product;
        set => SetProperty(ref _product, value, nameof(Product));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CartItem"/> class with the specified product.
    /// The initial quantity is set to 1.
    /// </summary>
    /// <param name="product">The product to be added to the cart.</param>
    public CartItem(Product product)
    {
        _product = product;
        _quantity = 1;
    }
}

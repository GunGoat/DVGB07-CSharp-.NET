using DVGB07_Inventory_App.Models;
using System;

namespace DVGB07_Inventory_App.DTO;

/// <summary>
/// Represents an item in the popular products list.
/// This class contains information about a product, including its sales count and revenue.
/// </summary>
public class PopularProductItem
{
    /// <summary>
    /// Gets or sets the unique identifier of the popular product item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the product associated with this popular product item.
    /// </summary>
    public Product Product { get; set; }

    /// <summary>
    /// Gets or sets the total number of sales for the product.
    /// </summary>
    public int SalesCount { get; set; }

    /// <summary>
    /// Gets or sets the total revenue generated from the sales of the product.
    /// </summary>
    public decimal SalesRevenue { get; set; }
}

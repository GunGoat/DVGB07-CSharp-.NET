using DVGB07_Inventory_App.Commands;
using DVGB07_Inventory_App.Models;
using System;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DVGB07_Inventory_App.ViewModels;

/// <summary>
/// ViewModel for managing the refill of product stock in the inventory.
/// This class allows updating the stock of a specific product by adding a specified quantity.
/// </summary>
public class StockDialogViewModel : ProductDialogViewModel
{
    private int _quantityToAdd;

    /// <summary>
    /// The product selected for stock update.
    /// </summary>
    public Product SelectedInventoryItem { get; set; }

    /// <summary>
    /// The quantity to add to the selected product's stock.
    /// </summary>
    public int QuantityToAdd
    {
        get => _quantityToAdd;
        set => SetProperty(ref _quantityToAdd, value, nameof(QuantityToAdd));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StockDialogViewModel"/> class with the selected inventory item.
    /// </summary>
    /// <param name="item">The product to be updated.</param>
    public StockDialogViewModel(Product item)
    {
        SelectedInventoryItem = item.Clone();
        WindowTitle = $"Update {SelectedInventoryItem.GetType().Name} stock";
    }

    /// <summary>
    /// Saves the changes by updating the stock of the selected inventory item.
    /// </summary>
    /// <param name="parameter">An optional parameter passed with the command. Not used in this case.</param>
    public override void Save(object parameter)
    {
        // Add the specified quantity to the stock of the selected product.
        SelectedInventoryItem.Stock += QuantityToAdd;
        OnItemSaved(SelectedInventoryItem);
        CloseWindow?.Invoke();
    }
}
using DVGB07_Inventory_App.Commands;
using DVGB07_Inventory_App.DTO;
using DVGB07_Inventory_App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DVGB07_Inventory_App.ViewModels;
using DVGB07_Inventory_App.DTO;
using DVGB07_Inventory_App.Models;
using System.ComponentModel;

namespace DVGB07_Inventory_App.ViewModels.Dialog;

/// <summary>
/// ViewModel for handling return receipt operations.
/// </summary>
public class ReturnReceiptDialogViewModel : PropertyChangedBase
{
    private string _windowTitle;
    private Receipt _receipt;
    private List<ReturnItem> _returnableProducts;

    /// <summary>
    /// Gets or sets the title of the window.
    /// </summary>
    public string WindowTitle
    {
        get => _windowTitle;
        set => SetProperty(ref _windowTitle, value, nameof(WindowTitle));
    }

    /// <summary>
    /// Gets or sets the receipt associated with the return.
    /// </summary>
    private Receipt Receipt
    {
        get => _receipt;
        set => SetProperty(ref _receipt, value, nameof(Receipt));
    }

    /// <summary>
    /// Gets or sets the list of returnable products.
    /// </summary>
    public List<ReturnItem> ReturnableProducts
    {
        get => _returnableProducts;
        set => SetProperty(ref _returnableProducts, value, nameof(ReturnableProducts), null);
    }

    /// <summary>
    /// Gets the total count of products being returned.
    /// </summary>
    public int TotalReturnedProductCount => ReturnableProducts?.Sum(item => item.ReturnQuantity) ?? 0;

    /// <summary>
    /// Gets the total refund amount for the returned products.
    /// </summary>
    public decimal TotalRefundAmount => ReturnableProducts?.Sum(item => item.ReturnQuantity * item.ReceiptItem.ProductPrice) ?? 0m;

    /// <summary>
    /// Command to cancel the current operation and close the window.
    /// </summary>
    public ICommand CancelCommand { get; protected set; }

    /// <summary>
    /// Command to process the return receipt operation.
    /// </summary>
    public ICommand ReturnReceiptCommand { get; protected set; }

    /// <summary>
    /// Action delegate to close the window.
    /// </summary>
    public Action CloseWindow { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturnReceiptDialogViewModel"/> class.
    /// </summary>
    /// <param name="receipt">The receipt associated with the return operation.</param>
    public ReturnReceiptDialogViewModel(Receipt receipt)
    {
        _windowTitle = "Return Receipt";
        CancelCommand = new RelayCommand(Cancel);
        ReturnReceiptCommand = new RelayCommand(ReturnReceipt, CanReturnReceipt);
        Receipt = receipt;

        _returnableProducts = receipt.Items
            .Select(item => new ReturnItem
            {
                Product = App.DataService.Products.FirstOrDefault(p => p.Id == item.ProductId),
                ReceiptItem = item,
                ReturnQuantity = 0
            })
            .ToList();

        SubscribeToReturnableProductsChanges();
    }

    /// <summary>
    /// Cancels the current operation and closes the window.
    /// </summary>
    /// <param name="parameter">The command parameter (not used).</param>
    public virtual void Cancel(object parameter) => CloseWindow?.Invoke();

    /// <summary>
    /// Processes the return receipt operation.
    /// </summary>
    /// <param name="parameter">The command parameter (not used).</param>
    public void ReturnReceipt(object parameter)
    {
        Receipt.PurchaseDate = DateTime.Now;
        
        // Update product stock and receipt quantity
        foreach (var returnItem in ReturnableProducts)
        {
            if (returnItem.ReturnQuantity > 0)
            {
                returnItem.Product.Stock += returnItem.ReturnQuantity;
                returnItem.ReceiptItem.Quantity -= returnItem.ReturnQuantity;
            }
        }

        // Remove items that have zero quantity from the receipt
        Receipt.Items = Receipt.Items.Where(item => item.Quantity > 0).ToList();
        OnPropertyChanged(nameof(Receipt));

        // Remove receipt if no items remains
        if (Receipt.Items.Count == 0)
        {
            App.DataService.RemoveReceipt(Receipt);
        }

        // Close the window after processing the return
        CloseWindow?.Invoke();
    }

    /// <summary>
    /// Determines whether the return receipt operation can be executed.
    /// </summary>
    /// <param name="parameter">The command parameter (not used).</param>
    /// <returns>True if at least one product has a return quantity greater than zero, otherwise false.</returns>
    public bool CanReturnReceipt(object parameter)
    {
        return ReturnableProducts.Any(p => p.ReturnQuantity > 0);
    }

    /// <summary>
    /// Subscribes to property change events for returnable products to update command availability.
    /// </summary>
    private void SubscribeToReturnableProductsChanges()
    {
        foreach (var returnableProduct in ReturnableProducts)
        {
            returnableProduct.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(returnableProduct.ReturnQuantity))
                {
                    OnPropertyChanged(nameof(TotalReturnedProductCount));
                    OnPropertyChanged(nameof(TotalRefundAmount));
                    ((RelayCommand)ReturnReceiptCommand).RaiseCanExecuteChanged();
                }
            };
        }
    }
}
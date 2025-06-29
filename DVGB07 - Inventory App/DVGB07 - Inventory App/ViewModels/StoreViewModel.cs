using DVGB07_Inventory_App.ViewModels.Dialog;
using DVGB07_Inventory_App.Commands;
using DVGB07_Inventory_App.DTO;
using DVGB07_Inventory_App.Models;
using DVGB07_Inventory_App.Views.Dialog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace DVGB07_Inventory_App.ViewModels;

/// <summary>
/// ViewModel for the store interface, managing the shopping cart, receipts, and their actions.
/// </summary>
public class StoreViewModel : ViewModelBase
{
    private Receipt? _selectedReceipt;
    private CartItem? _selectedCartItem;
    private ReceiptFilterOptions _receiptFilterOptions = new ReceiptFilterOptions();

    /// <summary>
    /// Collection of items in the user's shopping cart.
    /// </summary>
    public ObservableCollection<CartItem> CartItems { get; set; }

    /// <summary>
    /// The currently selected receipt in the UI.
    /// </summary>
    public Receipt? SelectedReceipt
    {
        get => _selectedReceipt;
        set => SetProperty(ref _selectedReceipt, value, nameof(SelectedReceipt));
    }

    /// <summary>
    /// The currently selected cart item in the UI.
    /// </summary>
    public CartItem? SelectedCartItem
    {
        get => _selectedCartItem;
        set => SetProperty(ref _selectedCartItem, value, nameof(SelectedCartItem));
    }

    /// <summary>
    /// Filter options for receipts.
    /// </summary>
    public ReceiptFilterOptions ReceiptFilterOptions
    {
        get => _receiptFilterOptions;
        set => SetProperty(ref _receiptFilterOptions, value, nameof(ReceiptFilterOptions));
    }

    /// <summary>
    /// The selected product from the selected cart item.
    /// </summary>
    public Product? SelectedProduct => SelectedCartItem?.Product;

    /// <summary>
    /// Collection view for displaying filtered receipts.
    /// </summary>
    public ICollectionView ReceiptView { get; }

    /// <summary>
    /// Command for adding a product to the cart.
    /// </summary>
    public ICommand AddToCartCommand { get; }

    /// <summary>
    /// Command for removing a product from the cart.
    /// </summary>
    public ICommand RemoveFromCartCommand { get; }

    /// <summary>
    /// Command for completing the purchase.
    /// </summary>
    public ICommand CompletePurchaseCommand { get; }

    /// <summary>
    /// Command for clearing the receipt filter.
    /// </summary>
    public ICommand ClearReceiptFilter { get; }

    /// <summary>
    /// Command for returning a receipt.
    /// </summary>
    public ICommand ReturnReceiptCommand { get; }

    /// <summary>
    /// Initializes the ViewModel with necessary commands and collections.
    /// </summary>
    public StoreViewModel()
    {
        // Initialize the CartItems collection
        CartItems = new ObservableCollection<CartItem>();

        // Initialize cart commands
        AddToCartCommand = new RelayCommand(parameter => AddToCart(parameter as Product));
        RemoveFromCartCommand = new RelayCommand(parameter => RemoveFromCart(parameter as Product));
        CompletePurchaseCommand = new RelayCommand(_ => CompletePurchase());

        // Initialize receipt commands
        ClearReceiptFilter = new RelayCommand(parameter =>
        {
            if (parameter is ReceiptFilterOptions filter)
                filter.ClearFilters();
            else
                throw new ArgumentException("Unknown filter type");
        });
        ReturnReceiptCommand = new RelayCommand(parameter => OpenReturnReceiptDialogWindow(parameter as Receipt));

        // Initialize the receipt view
        ReceiptView = CollectionViewSource.GetDefaultView(App.DataService.Receipts);
        ReceiptView.Filter = item => Filter(item, ReceiptFilterOptions);
        _receiptFilterOptions.PropertyChanged += FilterOptions_PropertyChanged;

        // Set initial visibility states
        IsInventoryActionsVisible = false;
        IsStoreActionsVisible = true;
    }

    /// <summary>
    /// Adds a product to the shopping cart if it's not already in the cart and in stock.
    /// </summary>
    /// <param name="product">The product to add.</param>
    private void AddToCart(Product? product)
    {
        if (product == null)
            return;

        if (product.Stock <= 0)
        {
            MessageBox.Show("Selected product is out of stock and can't be added to the cart!",
            "Out of stock",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
            return;
        }

        var existingCartItem = CartItems.FirstOrDefault(ci => ci.Product.Id == product.Id);
        if (existingCartItem == null)
        {
            CartItems.Add(new CartItem(product));
        }
    }

    /// <summary>
    /// Removes a product from the shopping cart.
    /// </summary>
    /// <param name="product">The product to remove.</param>
    private void RemoveFromCart(Product? product)
    {
        if (product == null)
            return;

        var existingCartItem = CartItems.FirstOrDefault(ci => ci.Product.Id == product.Id);
        if (existingCartItem != null)
        {
            CartItems.Remove(existingCartItem);
        }
    }

    /// <summary>
    /// Filters receipts based on the selected filter options.
    /// </summary>
    /// <param name="obj">The receipt object to check against the filter.</param>
    /// <param name="filter">The filter options to apply.</param>
    /// <returns>True if the receipt matches the filter criteria, false otherwise.</returns>
    private bool Filter(object obj, ReceiptFilterOptions filter)
    {
        if (obj is not Receipt receipt) return false;

        // Local helper method to match string filters
        bool MatchesFilter(string? value, string? filterValue) =>
            string.IsNullOrEmpty(filterValue) ||
            (!string.IsNullOrEmpty(value) && value.Contains(filterValue, StringComparison.OrdinalIgnoreCase));

        // Local helper method to match numeric range filters
        bool MatchesRange(decimal? value, decimal? min, decimal? max) =>
            (!min.HasValue || (value.HasValue && value >= min.Value)) &&
            (!max.HasValue || (value.HasValue && value <= max.Value));

        // Local helper method to match date range filters
        bool MatchesDateRange(DateTime? value, DateTime? start, DateTime? end) =>
            (!start.HasValue || (value.HasValue && value >= start.Value)) &&
            (!end.HasValue || (value.HasValue && value <= end.Value));

        if (filter.ReceiptId.HasValue && receipt.Id != filter.ReceiptId.Value) return false;
        if (!MatchesDateRange(receipt.PurchaseDate, filter.StartDate, filter.EndDate)) return false;
        if (!MatchesRange(receipt.TotalPrice, filter.MinTotalPrice, filter.MaxTotalPrice)) return false;

        return true;
    }

    /// <summary>
    /// Refreshes the receipt view when the filter options change.
    /// </summary>
    private void FilterOptions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        ReceiptView.Refresh();
    }

    /// <summary>
    /// Completes the purchase by checking for any stock issues, creating a receipt, and updating product stock.
    /// </summary>
    public void CompletePurchase()
    {
        if (CartItems.Count == 0)
        {
            return;
        }

        // Check if any item exceeds available stock
        if (CartItems.Any(ci => ci.Product.Stock < ci.Quantity))
        {
            MessageBox.Show("One or more items in the cart exceed available stock!",
                            "Stock Warning",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
            return;
        }

        // Check if any item has zero or negative quantity
        if (CartItems.Any(ci => ci.Quantity <= 0))
        {
            MessageBox.Show("One or more items in the cart have an invalid quantity! " +
                            "Please ensure all quantities are greater than zero before completing the purchase.",
                            "Invalid Quantity",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
            return;
        }

        // Create a new receipt
        var receipt = new Receipt
        {
            PurchaseDate = DateTime.Now,
            Items = CartItems.Select(p => new ReceiptItem
            {
                ProductId = p.Product.Id ?? throw new ArgumentException("Product ID cannot be null"),
                ProductName = p.Product.Name ?? throw new ArgumentException("Product Name cannot be null"),
                ProductPrice = p.Product.Price ?? throw new ArgumentException("Product Price cannot be null"),
                Quantity = p.Quantity
            }).ToList()
        };

        // Update stock of products
        foreach (var cartItem in CartItems)
        {
            cartItem.Product.Stock -= cartItem.Quantity;
        }

        App.DataService.AddReceipt(receipt);
        CartItems.Clear();

        // Ask the user if they want to print the receipt
        var result = MessageBox.Show("Purchase completed successfully! Would you like to print the receipt?",
                                     "Success",
                                     MessageBoxButton.YesNo,
                                     MessageBoxImage.Information);

        if (result == MessageBoxResult.Yes)
        {
            PrintReceipt(receipt);
        }
    }

    /// <summary>
    /// Prints the receipt to the printer.
    /// </summary>
    /// <param name="receipt">The receipt to print.</param>
    private void PrintReceipt(Receipt receipt)
    {
        // Format the receipt as a string
        var receiptContent = new StringBuilder();

        receiptContent.AppendLine($"=== PURCHASE RECEIPT {receipt.Id} ===");
        receiptContent.AppendLine($"Date: {receipt.PurchaseDate}");
        receiptContent.AppendLine("========================");

        foreach (var item in receipt.Items)
        {
            receiptContent.AppendLine($"{item}");
        }

        receiptContent.AppendLine("========================");
        receiptContent.AppendLine($"Total: ${receipt.Items.Sum(i => i.ProductPrice * i.Quantity)}");
        receiptContent.AppendLine("========================");

        // Send the formatted receipt to the printer
        App.PrintService.Print(receiptContent.ToString(), $"Receipt {receipt.Id}");
    }

    /// <summary>
    /// Opens a dialog window for returning a receipt.
    /// </summary>
    /// <param name="receipt">The receipt to return.</param>
    private void OpenReturnReceiptDialogWindow(Receipt receipt)
    {
        if (receipt is null)
        {
            MessageBox.Show("Please select an item to restock.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Create View and ViewModel
        var viewModel = new ReturnReceiptDialogViewModel(receipt);
        var window = new ReturnReceiptDialogWindow
        {
            DataContext = viewModel,
            Owner = App.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        // Assign Actions and events
        viewModel.CloseWindow = window.Close;

        // Show the dialog
        window.ShowDialog();
    }
}

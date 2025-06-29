using DVGB07_Inventory_App.Commands;
using DVGB07_Inventory_App.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DVGB07_Inventory_App.ViewModels.Dialog;

/// <summary>
/// ViewModel for displaying and managing popular products based on sales data.
/// This class allows users to specify a date range and a number of products to view the most popular products during that time.
/// </summary>
public class PopularProductDialogViewModel : PropertyChangedBase
{
    private string _windowTitle;
    private DateTime _startDate;
    private DateTime _endDate;
    private int _numberOfProducts;
    private List<PopularProductItem> _popularProducts;

    /// <summary>
    /// Gets or sets the title of the window.
    /// </summary>
    public string WindowTitle
    {
        get => _windowTitle;
        set => SetProperty(ref _windowTitle, value, nameof(WindowTitle));
    }

    /// <summary>
    /// Gets or sets the start date for the sales report.
    /// </summary>
    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value, nameof(StartDate), FetchPopularProducts);
    }

    /// <summary>
    /// Gets or sets the end date for the sales report.
    /// </summary>
    public DateTime EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value, nameof(EndDate), FetchPopularProducts);
    }

    /// <summary>
    /// Gets or sets the number of popular products to display.
    /// </summary>
    public int NumberOfProducts
    {
        get => _numberOfProducts;
        set => SetProperty(ref _numberOfProducts, value, nameof(NumberOfProducts), FetchPopularProducts);
    }

    /// <summary>
    /// Gets or sets the list of popular product items.
    /// </summary>
    public List<PopularProductItem> PopularProducts
    {
        get => _popularProducts;
        set => SetProperty(ref _popularProducts, value, nameof(PopularProducts), null, nameof(TotalSalesCount), nameof(TotalSalesRevenue));
    }

    /// <summary>
    /// Gets the total sales count of the popular products.
    /// </summary>
    public int TotalSalesCount => PopularProducts.Sum(p => p.SalesCount);

    /// <summary>
    /// Gets the total sales revenue of the popular products.
    /// </summary>
    public decimal TotalSalesRevenue => PopularProducts.Sum(p => p.SalesRevenue);

    /// <summary>
    /// Command to cancel the current operation and close the window.
    /// </summary>
    public ICommand CancelCommand { get; protected set; }

    /// <summary>
    /// Action to close the window.
    /// </summary>
    public Action CloseWindow { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PopularProductDialogViewModel"/> class.
    /// </summary>
    public PopularProductDialogViewModel()
    {
        _windowTitle = "Popular Products";
        _endDate = DateTime.Today;
        _startDate = EndDate.AddMonths(-1);
        _numberOfProducts = 10;

        CancelCommand = new RelayCommand(Cancel);
        FetchPopularProducts();
    }

    /// <summary>
    /// Fetches the most popular products based on sales data within the specified date range.
    /// This method groups products by their ID and calculates their sales count and sales revenue.
    /// </summary>
    private void FetchPopularProducts()
    {
        if (App.DataService?.Receipts == null) return;

        var popularProducts = App.DataService.Receipts
            .Where(r => r.PurchaseDate.Date >= StartDate.Date && r.PurchaseDate.Date <= EndDate.Date) // Filter by date (ignoring time)
            .SelectMany(r => r.Items) // Flatten into individual receipt items
            .GroupBy(item => item.ProductId) // Group by ProductId
            .Select(g => new PopularProductItem
            {
                Id = g.Key,
                Product = App.DataService.Products.FirstOrDefault(p => p.Id == g.Key), // Fetch product from DataService
                SalesCount = g.Sum(item => item.Quantity),
                SalesRevenue = g.Sum(item => item.Quantity * item.ProductPrice)
            })
            .OrderByDescending(s => s.SalesCount) // Sort by most sold
            .Take(NumberOfProducts) // Get the top N products
            .ToList();

        // Store the list of popular products
        PopularProducts = popularProducts;
    }

    /// <summary>
    /// Cancels the operation and closes the window.
    /// </summary>
    /// <param name="parameter">An optional parameter passed with the command. Not used in this case.</param>
    public virtual void Cancel(object parameter) => CloseWindow?.Invoke();
}

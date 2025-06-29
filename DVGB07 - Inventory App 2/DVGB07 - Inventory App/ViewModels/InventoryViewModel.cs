using DVGB07_Inventory_App.Commands;
using DVGB07_Inventory_App.Models;
using DVGB07_Inventory_App.Views;
using DVGB07_Inventory_App;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DVGB07_Inventory_App.ViewModels;
using System.Windows;
using System.Diagnostics;
using DVGB07_Inventory_App.Views.Dialog;
using System.Collections;
using System.ComponentModel;
using System.Windows.Data;
using DVGB07_Inventory_App.DTO;
using System.Xml.Linq;

namespace DVGB07_Inventory_App.ViewModels;

/// <summary>
/// ViewModel for the inventory interface, managing the products and their actions.
/// </summary>
public class InventoryViewModel : ViewModelBase
{
    /// <summary>
    /// Command to add a new book to the inventory.
    /// </summary>
    public ICommand AddBookCommand { get; }

    /// <summary>
    /// Command to add a new game to the inventory.
    /// </summary>
    public ICommand AddGameCommand { get; }

    /// <summary>
    /// Command to add a new movie to the inventory.
    /// </summary>
    public ICommand AddMovieCommand { get; }

    /// <summary>
    /// Command to update an existing book.
    /// </summary>
    public ICommand UpdateBookCommand { get; }

    /// <summary>
    /// Command to update an existing game.
    /// </summary>
    public ICommand UpdateGameCommand { get; }

    /// <summary>
    /// Command to update an existing movie.
    /// </summary>
    public ICommand UpdateMovieCommand { get; }

    /// <summary>
    /// Command to remove a product from the inventory.
    /// </summary>
    public ICommand RemoveProductCommand { get; }

    /// <summary>
    /// Command to refill stock for a selected product.
    /// </summary>
    public ICommand RefillStockCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryViewModel"/> class.
    /// </summary>
    public InventoryViewModel()
    {
        AddBookCommand = new RelayCommand(_ => OpenAddProductWindow<BookDialogViewModel, BookDialogWindow>());
        AddGameCommand = new RelayCommand(_ => OpenAddProductWindow<GameDialogViewModel, GameDialogWindow>());
        AddMovieCommand = new RelayCommand(_ => OpenAddProductWindow<MovieDialogViewModel, MovieDialogWindow>());

        UpdateBookCommand = new RelayCommand(parameter => OpenUpdateProductWindow<BookDialogViewModel, BookDialogWindow>((Book)parameter));
        UpdateGameCommand = new RelayCommand(parameter => OpenUpdateProductWindow<GameDialogViewModel, GameDialogWindow>((Game)parameter));
        UpdateMovieCommand = new RelayCommand(parameter => OpenUpdateProductWindow<MovieDialogViewModel, MovieDialogWindow>((Movie)parameter));

        RemoveProductCommand = new RelayCommand(RemoveProduct);
        RefillStockCommand = new RelayCommand(parameter => OpenRefillStockWindow((Product)parameter));

        IsInventoryActionsVisible = true;
        IsStoreActionsVisible = false;
    }

    /// <summary>
    /// Adds a new product to the inventory.
    /// </summary>
    private void AddProduct(object sender, Product product)
    {
        try
        {
            App.DataService.AddProduct(product);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to add product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Updates an existing product in the inventory.
    /// </summary>
    private void UpdateProduct(object sender, Product product)
    {
        try
        {
            App.DataService.UpdateProduct(product);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Removes a selected product from the inventory.
    /// </summary>
    /// <param name="parameter">The product to be removed.</param>
    private void RemoveProduct(object parameter)
    {
        if (parameter is null)
        {
            MessageBox.Show("Please select an item to remove.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (parameter is not Product itemToRemove)
        {
            Debug.WriteLine("Unsupported item type.");
            return;
        }

        if (itemToRemove.Stock > 0)
        {
            string message = $"The product '{itemToRemove.Name}' has {itemToRemove.Stock} unit(s) in stock. Are you sure you want to remove it?";
            MessageBoxResult result = MessageBox.Show(message, "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
        }

        try
        {
            App.DataService.RemoveProduct(itemToRemove);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to remove product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Opens a dialog window to update an existing product.
    /// </summary>
    private void OpenUpdateProductWindow<TViewModel, TWindow>(Product item)
       where TViewModel : ProductDialogViewModel
       where TWindow : Window, new()
    {
        if (item is null)
        {
            MessageBox.Show("Please select an item to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var viewModel = (TViewModel)Activator.CreateInstance(typeof(TViewModel), item)!;
        var window = new TWindow
        {
            DataContext = viewModel,
            Owner = App.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        viewModel.CloseWindow = window.Close;
        viewModel.ItemCreated += UpdateProduct;

        window.ShowDialog();
    }

    /// <summary>
    /// Opens a dialog window to add a new product.
    /// </summary>
    private void OpenAddProductWindow<TViewModel, TWindow>()
        where TViewModel : ProductDialogViewModel, new()
        where TWindow : Window, new()
    {
        var viewModel = new TViewModel();
        var window = new TWindow
        {
            DataContext = viewModel,
            Owner = App.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        viewModel.CloseWindow = window.Close;
        viewModel.ItemCreated += AddProduct;

        window.ShowDialog();
    }

    /// <summary>
    /// Opens a dialog window to refill stock for a selected product.
    /// </summary>
    /// <param name="item">The product to be restocked.</param>
    private void OpenRefillStockWindow(Product item)
    {
        if (item is null)
        {
            MessageBox.Show("Please select an item to restock.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var viewModel = new StockDialogViewModel(item);
        var window = new StockDialogWindow
        {
            DataContext = viewModel,
            Owner = App.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        viewModel.CloseWindow = window.Close;
        viewModel.ItemCreated += UpdateProduct;

        window.ShowDialog();
    }
}

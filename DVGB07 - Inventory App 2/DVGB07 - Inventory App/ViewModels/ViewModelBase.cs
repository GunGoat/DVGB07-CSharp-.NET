using DVGB07_Inventory_App.Commands;
using DVGB07_Inventory_App.DTO;
using DVGB07_Inventory_App.Models;
using DVGB07_Inventory_App.Service;
using DVGB07_Inventory_App.ViewModels.Dialog;
using DVGB07_Inventory_App.Views.Dialog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace DVGB07_Inventory_App.ViewModels;

/// <summary>
/// The base ViewModel class for managing the inventory of products (Books, Games, Movies).
/// It provides filtering capabilities and manages visibility of inventory and store actions.
/// </summary>
public class ViewModelBase : PropertyChangedBase
{
    private Book? _selectedBook;
    private Game? _selectedGame;
    private Movie? _selectedMovie;
    private BookFilterOptions _bookFilterOptions = new BookFilterOptions();
    private GameFilterOptions _gameFilterOptions = new GameFilterOptions();
    private MovieFilterOptions _movieFilterOptions = new MovieFilterOptions();
    private bool _isInventoryActionsVisible;
    private bool _isStoreActionsVisible;
    private bool _isSyncWarningVisible;
    private string _syncWarningMessage;

    /// <summary>
    /// The currently selected Book.
    /// </summary>
    public Book? SelectedBook
    {
        get => _selectedBook;
        set => SetProperty(ref _selectedBook, value, nameof(SelectedBook));
    }

    /// <summary>
    /// The currently selected Game.
    /// </summary>
    public Game? SelectedGame
    {
        get => _selectedGame;
        set => SetProperty(ref _selectedGame, value, nameof(SelectedGame));
    }

    /// <summary>
    /// The currently selected Movie.
    /// </summary>
    public Movie? SelectedMovie
    {
        get => _selectedMovie;
        set => SetProperty(ref _selectedMovie, value, nameof(SelectedMovie));
    }

    /// <summary>
    /// The filter options for Books.
    /// </summary>
    public BookFilterOptions BookFilterOptions
    {
        get => _bookFilterOptions;
        set => SetProperty(ref _bookFilterOptions, value, nameof(BookFilterOptions));
    }

    /// <summary>
    /// The filter options for Games.
    /// </summary>
    public GameFilterOptions GameFilterOptions
    {
        get => _gameFilterOptions;
        set => SetProperty(ref _gameFilterOptions, value, nameof(GameFilterOptions));
    }

    /// <summary>
    /// The filter options for Movies.
    /// </summary>
    public MovieFilterOptions MovieFilterOptions
    {
        get => _movieFilterOptions;
        set => SetProperty(ref _movieFilterOptions, value, nameof(MovieFilterOptions));
    }

    /// <summary>
    /// Flag to determine whether inventory actions are visible.
    /// </summary>
    public bool IsInventoryActionsVisible
    {
        get => _isInventoryActionsVisible;
        set => SetProperty(ref _isInventoryActionsVisible, value, nameof(IsInventoryActionsVisible), additionalProperties: nameof(InventoryActionsVisibility));
    }

    /// <summary>
    /// Flag to determine whether store actions are visible.
    /// </summary>
    public bool IsStoreActionsVisible
    {
        get => _isStoreActionsVisible;
        set => SetProperty(ref _isStoreActionsVisible, value, nameof(IsStoreActionsVisible), null, nameof(StoreActionsVisibility));
    }

    /// <summary>
    /// Flag to determine whether the synchronization warning is visible.
    /// </summary>
    public bool IsSyncWarningVisible
    {
        get => _isSyncWarningVisible;
        set => SetProperty(ref _isSyncWarningVisible, value, nameof(IsSyncWarningVisible), null, nameof(SyncWarningVisibility), nameof(SyncWarningMessage));
    }

    /// <summary>
    /// The message displayed in the synchronization warning.
    /// </summary>
    public string SyncWarningMessage
    {
        get => _syncWarningMessage;
        set => SetProperty(ref _syncWarningMessage, value, nameof(SyncWarningMessage));
    }

    /// <summary>
    /// The visibility of the inventory actions, based on the <see cref="IsInventoryActionsVisible"/> flag.
    /// </summary>
    public Visibility InventoryActionsVisibility => IsInventoryActionsVisible ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// The visibility of the store actions, based on the <see cref="IsStoreActionsVisible"/> flag.
    /// </summary>
    public Visibility StoreActionsVisibility => IsStoreActionsVisible ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// The visibility of the synchronization warning, based on the <see cref="IsSyncWarningVisible"/> flag.
    /// </summary>
    public Visibility SyncWarningVisibility => IsSyncWarningVisible ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// The collection of Books to be displayed in the view.
    /// </summary>
    public ICollectionView BooksView { get; }

    /// <summary>
    /// The collection of Games to be displayed in the view.
    /// </summary>
    public ICollectionView GamesView { get; }

    /// <summary>
    /// The collection of Movies to be displayed in the view.
    /// </summary>
    public ICollectionView MoviesView { get; }

    /// <summary>
    /// Command to clear all product filters.
    /// </summary>
    public ICommand ClearProductFilter { get; }

    /// <summary>
    /// Command to show the popular products dialog window.
    /// </summary>
    public ICommand PopularProductsCommand { get; }

    /// <summary>
    /// Command to show the price and stock log dialog window.
    /// </summary>
    public ICommand PriceAndStockLogsCommand { get; }

    /// <summary>
    /// Command to download product data for synchronization.
    /// </summary>
    public ICommand SyncDownloadProductsCommand { get; }

    /// <summary>
    /// Command to upload product data for synchronization.
    /// </summary>
    public ICommand SyncUploadProductsCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
    /// Sets up the filter and view collections and initializes commands.
    /// </summary>
    public ViewModelBase()
    {
        _syncWarningMessage = "Products are not synchronized";

        BooksView = CollectionViewSource.GetDefaultView(App.DataService.Books);
        GamesView = CollectionViewSource.GetDefaultView(App.DataService.Games);
        MoviesView = CollectionViewSource.GetDefaultView(App.DataService.Movies);

        _bookFilterOptions.PropertyChanged += FilterOptions_PropertyChanged;
        _gameFilterOptions.PropertyChanged += FilterOptions_PropertyChanged;
        _movieFilterOptions.PropertyChanged += FilterOptions_PropertyChanged;

        BooksView.Filter = item => Filter(item, BookFilterOptions);
        GamesView.Filter = item => Filter(item, GameFilterOptions);
        MoviesView.Filter = item => Filter(item, MovieFilterOptions);

        ClearProductFilter = new RelayCommand(parameter =>
        {
            if (parameter is ProductFilterOptions filter)
                filter.ClearFilters();
            else
                throw new ArgumentException("Unknown filter type");
        });

        PopularProductsCommand = new RelayCommand(OpenPopularProductDialogWindow);
        PriceAndStockLogsCommand = new RelayCommand(OpenPriceAndStockLogsDialogWindow);
        SyncDownloadProductsCommand = new RelayCommand(async () => await SyncDownloadProducts());
        SyncUploadProductsCommand = new RelayCommand(async () => await SyncUploadProducts());

        App.TimerService.TimerTick += OnTimerTick;
    }

    /// <summary>
    /// Opens the dialog window to display popular products.
    /// </summary>
    private void OpenPopularProductDialogWindow()
    {
        var viewModel = new PopularProductDialogViewModel();
        var window = new PopularProductDialogWindow
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

    /// <summary>
    /// Opens the dialog window to display popular products.
    /// </summary>
    private void OpenPriceAndStockLogsDialogWindow()
    {
        var viewModel = new PriceAndStockLogsDialogViewModel();
        var window = new PriceAndStockLogsDialogWindow
        {
            DataContext = viewModel,
            Owner = App.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        // Show the dialog
        window.ShowDialog();
    }

    /// <summary>
    /// Applies filters to a product (Book, Game, or Movie) based on the provided filter options.
    /// </summary>
    /// <param name="obj">The product to be filtered.</param>
    /// <param name="filter">The filter options to be applied.</param>
    /// <returns>True if the product matches the filter criteria; otherwise, false.</returns>
    private bool Filter(object obj, ProductFilterOptions filter)
    {
        if (obj is not Product product) return false;

        // Local helper method to match string filters
        bool MatchesFilter(string? value, string? filterValue) =>
            string.IsNullOrEmpty(filterValue) ||
            (!string.IsNullOrEmpty(value) && value.Contains(filterValue, StringComparison.OrdinalIgnoreCase));

        // Local helper method to match numeric range filters
        bool MatchesRange(decimal? value, decimal? min, decimal? max) =>
            (!min.HasValue || (value.HasValue && value >= min.Value)) &&
            (!max.HasValue || (value.HasValue && value <= max.Value));

        if (!MatchesFilter(product.Name, filter.Name)) return false;
        if (!MatchesRange(product.Price, filter.MinPrice, filter.MaxPrice)) return false;

        // Apply type-specific filters based on the provided filter object
        switch (product)
        {
            case Book book when filter is BookFilterOptions bookFilter:
                if (!MatchesFilter(book.Genre, bookFilter.Genre)) return false;
                if (!MatchesFilter(book.Format, bookFilter.Format)) return false;
                if (!MatchesFilter(book.Language, bookFilter.Language)) return false;
                if (!MatchesFilter(book.Author, bookFilter.Author)) return false;
                break;

            case Game game when filter is GameFilterOptions gameFilter:
                if (!MatchesFilter(game.Platform, gameFilter.Platform)) return false;
                break;

            case Movie movie when filter is MovieFilterOptions movieFilter:
                if (!MatchesFilter(movie.Format, movieFilter.Format)) return false;
                if (!MatchesRange(movie.Playtime, movieFilter.MinPlaytime, movieFilter.MaxPlaytime)) return false;
                break;
        }

        return true;
    }

    /// <summary>
    /// Refreshes the respective view when a filter option changes.
    /// </summary>
    private void FilterOptions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is BookFilterOptions bookFilterOptions)
        {
            BooksView.Refresh();
        }
        else if (sender is GameFilterOptions gameFilterOptions)
        {
            GamesView.Refresh();
        }
        else if (sender is MovieFilterOptions movieFilterOptions)
        {
            MoviesView.Refresh();
        }
    }

    /// <summary>
    /// Event handler for the timer tick event. Calls the SyncDownloadProducts method to synchronize products from the server.
    /// </summary>
    /// <param name="sender">The sender object that triggered the event.</param>
    /// <param name="e">Event data related to the tick event.</param>
    private async void OnTimerTick(object sender, EventArgs e)
    {
        try
        {
            // Calls the method to sync the products
            SyncDownloadProducts();
        }
        catch (Exception ex)
        {
            // Displays a warning if an error occurs during the synchronization
            IsSyncWarningVisible = true;
        }
    }

    /// <summary>
    /// Downloads and synchronizes products from the server with the local data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task SyncDownloadProducts()
    {
        // Get the products from the API asynchronously
        var result = await App.ApiService.GetProductsAsync();

        // Check if there was an error while fetching the products
        if (result.ErrorMessage != null)
        {
            SyncWarningMessage = result.ErrorMessage;
            if (!IsSyncWarningVisible)
                System.Media.SystemSounds.Exclamation.Play();
            IsSyncWarningVisible = true;
            return;
        }

        // Loop through the products and synchronize them locally based on their type
        foreach (var product in result.Products)
        {
            switch (product)
            {
                case Book book:
                    SynchronizeLocalProduct(App.DataService.Books.FirstOrDefault(b => b.Id == book.Id), book);
                    break;

                case Game game:
                    SynchronizeLocalProduct(App.DataService.Games.FirstOrDefault(g => g.Id == game.Id), game);
                    break;

                case Movie movie:
                    SynchronizeLocalProduct(App.DataService.Movies.FirstOrDefault(m => m.Id == movie.Id), movie);
                    break;

                default:
                    // Throw an exception if an unrecognized product type is encountered
                    throw new NotImplementedException();
            }
        }

        // Hide the sync warning after successful synchronization
        IsSyncWarningVisible = false;
    }

    /// <summary>
    /// Uploads the local products to the server to synchronize the data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task SyncUploadProducts()
    {
        // Get the products from the API asynchronously
        var result = await App.ApiService.GetProductsAsync();

        // Check if there was an error while fetching the products
        if (result.ErrorMessage != null)
        {
            SyncWarningMessage = result.ErrorMessage;
            if (!IsSyncWarningVisible)
                System.Media.SystemSounds.Exclamation.Play();
            IsSyncWarningVisible = true;
            return;
        }

        // Loop through the products and synchronize them online based on their type
        foreach (var product in result.Products)
        {
            switch (product)
            {
                case Book book:
                    SynchronizeOnlineProduct(App.DataService.Books.FirstOrDefault(b => b.Id == book.Id), book);
                    break;

                case Game game:
                    SynchronizeOnlineProduct(App.DataService.Games.FirstOrDefault(g => g.Id == game.Id), game);
                    break;

                case Movie movie:
                    SynchronizeOnlineProduct(App.DataService.Movies.FirstOrDefault(m => m.Id == movie.Id), movie);
                    break;

                default:
                    // Throw an exception if an unrecognized product type is encountered
                    throw new NotImplementedException();
            }
        }

        // Hide the sync warning after successful synchronization
        IsSyncWarningVisible = false;
    }

    /// <summary>
    /// Synchronizes the local product with the online product by updating the price and stock information.
    /// If the product does not exist locally, it adds the online product to the local data.
    /// </summary>
    /// <typeparam name="T">The type of the product (Book, Game, Movie).</typeparam>
    /// <param name="localProduct">The local product to be synchronized.</param>
    /// <param name="onlineProduct">The online product to synchronize with the local product.</param>
    private void SynchronizeLocalProduct<T>(T? localProduct, T onlineProduct) where T : Product
    {
        if (localProduct != null)
        {
            // Update the local product's price and stock to match the online product
            if (localProduct.Price != onlineProduct.Price)
            {
                localProduct.Price = onlineProduct.Price;
                Debug.WriteLine($"Updated Price for {onlineProduct.Name}: {onlineProduct.Price:C}");
            }
            if (localProduct.Stock != onlineProduct.Stock)
            {
                localProduct.Stock = onlineProduct.Stock;
                Debug.WriteLine($"Updated Stock for {onlineProduct.Name}: {onlineProduct.Stock}");
            }
        }
        else
        {
            // If the product doesn't exist locally, add it from the online data
            App.DataService.AddProduct(onlineProduct);
            Debug.WriteLine($"Added new product: {onlineProduct.Name}");
        }
    }

    /// <summary>
    /// Synchronizes the online product stock with the local product stock by updating the server if the stock differs.
    /// </summary>
    /// <typeparam name="T">The type of the product (Book, Game, Movie).</typeparam>
    /// <param name="localProduct">The local product to be synchronized.</param>
    /// <param name="onlineProduct">The online product to compare with the local product.</param>
    private async void SynchronizeOnlineProduct<T>(T? localProduct, T onlineProduct) where T : Product
    {
        if (localProduct != null && localProduct.Stock != onlineProduct.Stock)
        {
            // If the stock differs, update the server with the local product's stock value
            var result = await App.ApiService.UpdateStockAsync(localProduct.Id.Value, localProduct.Stock);
            if (result.ErrorMessage != null)
            {
                SyncWarningMessage = result.ErrorMessage;
                if (!IsSyncWarningVisible)
                    System.Media.SystemSounds.Exclamation.Play();
                IsSyncWarningVisible = true;
                return;
            }
            Debug.WriteLine($"Updated stock for {localProduct.Name} to {localProduct.Stock} on the server.");
        }
    }
}

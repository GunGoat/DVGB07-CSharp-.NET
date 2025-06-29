using DVGB07_Inventory_App.DTO;
using DVGB07_Inventory_App.Models;
using DVGB07_Inventory_App.Service;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace DVGB07_Inventory_App.Service;

/// <summary>
/// Provides data management services for the inventory system, making the collections of books, games, movies, and receipts accessible throughout the application.
/// </summary>
/// <remarks>
/// This class also handles the assignment of unique product and receipt IDs.
/// </remarks>
public class DataService : IDataService
{
    private readonly ObservableCollection<Book> _books = new();
    private readonly ObservableCollection<Game> _games = new();
    private readonly ObservableCollection<Movie> _movies = new();
    private readonly ObservableCollection<Receipt> _receipts = new();
    private readonly Dictionary<int, List<PriceLog>> _priceLogsByProduct = new();
    private readonly Dictionary<int, List<StockLog>> _stockLogsByProduct = new();
    private int _nextProductId = 10000;
    private int _nextReceiptId = 1;

    /// <summary>
    /// Gets the collection of books.
    /// </summary>
    public IReadOnlyCollection<Book> Books => _books;

    /// <summary>
    /// Gets the collection of games.
    /// </summary>
    public IReadOnlyCollection<Game> Games => _games;

    /// <summary>
    /// Gets the collection of movies.
    /// </summary>
    public IReadOnlyCollection<Movie> Movies => _movies;

    /// <summary>
    /// Gets the collection of receipts.
    /// </summary>
    public IReadOnlyCollection<Receipt> Receipts => _receipts;

    /// <summary>
    /// Gets a collection of all products (books, games, movies).
    /// </summary>
    public IEnumerable<Product> Products => _books.Cast<Product>()
        .Concat(_games)
        .Concat(_movies);

    /// <summary>
    /// Gets the next available product ID.
    /// </summary>
    public int NextProductId => _nextProductId;

    /// <summary>
    /// Gets the next available receipt ID.
    /// </summary>
    public int NextReceiptId => _nextReceiptId;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataService"/> class.
    /// </summary>
    /// <param name="storageService">The storage service used to load data.</param>
    public DataService(IStorageService storageService)
    {
        var storageData = storageService.Load();

        foreach (var book in storageData.Books)
        {
            AttachPropertyChangedListener(book);
            _books.Add(book);
        }

        foreach (var game in storageData.Games)
        {
            AttachPropertyChangedListener(game);
            _games.Add(game);
        }

        foreach (var movie in storageData.Movies)
        {
            AttachPropertyChangedListener(movie);
            _movies.Add(movie);
        }

        foreach (var receipt in storageData.Receipts)
            _receipts.Add(receipt);

        _nextProductId = storageData.NextProductId;
        _nextReceiptId = storageData.NextReceiptId;
    }

    /// <summary>
    /// Attaches a property changed listener to a product to track price and stock changes.
    /// </summary>
    /// <param name="product">The product to monitor for changes.</param>
    private void AttachPropertyChangedListener(Product product)
    {
        if (product == null || !product.Id.HasValue) return;

        int productId = product.Id.Value;

        // Log Initial Values
        AddPriceLog(productId, product.Price);
        AddStockLog(productId, product.Stock);

        // Attach Change Listener
        product.PropertyChanged += (sender, e) =>
        {
            if (sender is Product changedProduct && changedProduct.Id.HasValue)
            {
                int changedProductId = changedProduct.Id.Value;

                if (e.PropertyName == nameof(Product.Price))
                {
                    AddPriceLog(changedProductId, changedProduct.Price);
                }
                else if (e.PropertyName == nameof(Product.Stock))
                {
                    AddStockLog(changedProductId, changedProduct.Stock);
                }
            }
        };
    }

    /// <summary>
    /// Adds a price log entry for the given product ID.
    /// </summary>
    private void AddPriceLog(int productId, decimal? price)
    {
        if (!price.HasValue) return;

        if (!_priceLogsByProduct.ContainsKey(productId))
            _priceLogsByProduct[productId] = new List<PriceLog>();

        _priceLogsByProduct[productId].Add(new PriceLog(productId, price.Value));
    }

    /// <summary>
    /// Adds a stock log entry for the given product ID.
    /// </summary>
    private void AddStockLog(int productId, int stock)
    {
        if (!_stockLogsByProduct.ContainsKey(productId))
            _stockLogsByProduct[productId] = new List<StockLog>();

        _stockLogsByProduct[productId].Add(new StockLog(productId, stock));
    }

    // <summary>
    /// Retrieves the price logs for a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <returns>A list of price logs for the product, or an empty list if no logs exist.</returns>
    public IReadOnlyList<PriceLog> GetPriceLogs(int productId)
    {
        return _priceLogsByProduct.TryGetValue(productId, out var logs) ? logs : new List<PriceLog>();
    }

    // <summary>
    /// Retrieves the stock logs for a specific product.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <returns>A list of stock logs for the product, or an empty list if no logs exist.</returns>
    public IReadOnlyList<StockLog> GetStockLogs(int productId)
    {
        return _stockLogsByProduct.TryGetValue(productId, out var logs) ? logs : new List<StockLog>();
    }

    /// <summary>
    /// Adds a product to the appropriate collection (book, game, or movie).
    /// </summary>
    /// <param name="product">The product to add.</param>
    /// <exception cref="ArgumentException">Thrown if the product ID already exists.</exception>
    public void AddProduct(Product product)
    {
        if (product.Id.HasValue && Products.Any(p => p.Id == product.Id))
        {
            throw new ArgumentException($"A product with ID {product.Id} already exists.", nameof(product));
        }

        if (!product.Id.HasValue || product.Id.Value < 10000)
        {
            // Ensure the next product ID is unique
            while (Products.Any(p => p.Id == _nextProductId))
            {
                _nextProductId++;
            }

            product.Id = _nextProductId;
            _nextProductId++;
        }

        AttachPropertyChangedListener(product);

        switch (product)
        {
            case Book book: _books.Add(book); break;
            case Game game: _games.Add(game); break;
            case Movie movie: _movies.Add(movie); break;
            default: throw new ArgumentException("Unknown product type", nameof(product));
        }
    }

    /// <summary>
    /// Updates an existing product in the appropriate collection (book, game, or movie).
    /// </summary>
    /// <param name="product">The product to update.</param>
    /// <exception cref="ArgumentException">Thrown if the product type is unknown.</exception>
    public void UpdateProduct(Product product)
    {
        switch (product)
        {
            case Book book:
                var existingBook = _books.FirstOrDefault(b => b.Id == book.Id);
                if (existingBook != null)
                {
                    existingBook.Name = book.Name;
                    existingBook.Price = book.Price;
                    existingBook.Author = book.Author;
                    existingBook.Stock = book.Stock;
                    existingBook.Genre = book.Genre;
                    existingBook.Format = book.Format;
                    existingBook.Language = book.Language;
                }
                break;
            case Game game:
                var existingGame = _games.FirstOrDefault(g => g.Id == game.Id);
                if (existingGame != null)
                {
                    existingGame.Name = game.Name;
                    existingGame.Price = game.Price;
                    existingGame.Stock = game.Stock;
                    existingGame.Platform = game.Platform;
                }
                break;
            case Movie movie:
                var existingMovie = _movies.FirstOrDefault(m => m.Id == movie.Id);
                if (existingMovie != null)
                {
                    existingMovie.Name = movie.Name;
                    existingMovie.Price = movie.Price;
                    existingMovie.Stock = movie.Stock;
                    existingMovie.Format = movie.Format;
                    existingMovie.Playtime = movie.Playtime;
                }
                break;
            default:
                throw new ArgumentException("Unknown product type", nameof(product));
        }
    }

    /// <summary>
    /// Removes a product from the appropriate collection (book, game, or movie).
    /// </summary>
    /// <param name="product">The product to remove.</param>
    /// <exception cref="ArgumentException">Thrown if the product type is unknown.</exception>
    public void RemoveProduct(Product product)
    {
        switch (product)
        {
            case Book book: _books.Remove(book); break;
            case Game game: _games.Remove(game); break;
            case Movie movie: _movies.Remove(movie); break;
            default: throw new ArgumentException("Unknown product type", nameof(product));
        }
        _priceLogsByProduct.Remove(product.Id.Value);
        _stockLogsByProduct.Remove(product.Id.Value);
    }

    /// <summary>
    /// Adds a receipt to the collection.
    /// </summary>
    /// <param name="receipt">The receipt to add.</param>
    /// <exception cref="ArgumentException">Thrown if the receipt ID already exists.</exception>
    public void AddReceipt(Receipt receipt)
    {
        if (receipt.Id.HasValue && _receipts.Any(r => r.Id == receipt.Id))
        {
            throw new ArgumentException($"A receipt with ID {receipt.Id} already exists.", nameof(receipt));
        }

        if (!receipt.Id.HasValue)
        {
            // Ensure the next receipt ID is unique
            while (_receipts.Any(r => r.Id == _nextReceiptId))
            {
                _nextReceiptId++;
            }

            receipt.Id = _nextReceiptId;
            _nextReceiptId++;
        }

        _receipts.Add(receipt);
    }

    /// <summary>
    /// Updates an existing receipt in the collection.
    /// </summary>
    /// <param name="receipt">The receipt to update.</param>
    public void UpdateReceipt(Receipt receipt)
    {
        var existingReceipt = _receipts.FirstOrDefault(b => b.Id == receipt.Id);
        if (existingReceipt != null)
        {
            existingReceipt.Id = receipt.Id;
            existingReceipt.PurchaseDate = receipt.PurchaseDate;
            existingReceipt.Items = receipt.Items;
        }
        _receipts.Add(receipt);
    }

    /// <summary>
    /// Removes a receipt from the collection.
    /// </summary>
    /// <param name="receipt">The receipt to remove.</param>
    public void RemoveReceipt(Receipt receipt)
    {
        _receipts.Remove(receipt);
    }
}
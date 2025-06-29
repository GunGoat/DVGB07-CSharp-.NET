using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using DVGB07_Inventory_App.DTO;
using DVGB07_Inventory_App.Models;

namespace DVGB07_Inventory_App.Service;

/// <summary>
/// Service class for saving and loading inventory data to/from a CSV file.
/// </summary>
public class StorageService : IStorageService
{
    private const string FilePath = "Inventory.csv";

    /// <summary>
    /// Saves the inventory data to a CSV file.
    /// </summary>
    /// <param name="data">The inventory data to be saved.</param>
    public void Save(StorageData data)
    {
        var sb = new StringBuilder();

        // Save header with NextProductId and NextReceiptId
        sb.AppendLine($"NextProductId;{data.NextProductId}");
        sb.AppendLine($"NextReceiptId;{data.NextReceiptId}");

        sb.AppendLine("Book");
        sb.AppendLine("Id;Name;Price;Stock;Genre;Format;Language;Author"); 
        foreach (var book in data.Books)
        {
            sb.AppendLine($"{book.Id};{book.Name};{book.Price};{book.Stock};{book.Genre};{book.Format};{book.Language};{book.Author}");
        }

        sb.AppendLine("Game");
        sb.AppendLine("Id;Name;Price;Stock;Platform");
        foreach (var game in data.Games)
        {
            sb.AppendLine($"{game.Id};{game.Name};{game.Price};{game.Stock};{game.Platform}");
        }

        sb.AppendLine("Movie");
        sb.AppendLine("Id;Name;Price;Stock;Format;Playtime");
        foreach (var movie in data.Movies)
        {
            sb.AppendLine($"{movie.Id};{movie.Name};{movie.Price};{movie.Stock};{movie.Format};{movie.Playtime}");
        }

        sb.AppendLine("Receipt");
        sb.AppendLine("Id;ProductName;PurchaseDate;ProductPrice;Quantity;ProductId");
        foreach (var receipt in data.Receipts)
        {
            foreach (var item in receipt.Items)
            {
                sb.AppendLine($"{receipt.Id};{item.ProductName};{receipt.PurchaseDate};{item.ProductPrice};{item.Quantity};{item.ProductId}");
            }
        }

        // Write the CSV data to the file.
        File.WriteAllText(FilePath, sb.ToString());
    }

    /// <summary>
    /// Loads inventory data from a CSV file.
    /// If the file does not exist, creates initial data.
    /// </summary>
    /// <returns>The loaded or initialized inventory data.</returns>
    public StorageData Load()
    {
        // If the file does not exist, create initial data, save it, and return it.
        if (!File.Exists(FilePath))
        {
            var initialData = CreateInitialData();
            Save(initialData);
            return initialData;
        }

        // Initialize empty lists to store books, games, movies, and receipts.
        var books = new List<Book>();
        var games = new List<Game>();
        var movies = new List<Movie>();
        var receipts = new List<Receipt>();
        int nextProductId = 10000, nextReceiptId = 1;

        // Read all lines from the file.
        var lines = File.ReadAllLines(FilePath);
        var section = ""; // Keeps track of the current section being processed.
        var receiptDict = new Dictionary<int, Receipt>(); // Helps group receipt items under the correct receipt.
        bool skipNextLine = false; // Used to skip section headers.

        foreach (var line in lines)
        {
            // Skip empty lines 
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Extract next available product and receipt IDs.
            if (line.StartsWith("NextProductId"))
            {
                nextProductId = int.Parse(line.Split(';')[1]);
                continue;
            }
            if (line.StartsWith("NextReceiptId"))
            {
                nextReceiptId = int.Parse(line.Split(';')[1]);
                continue;
            }

            // Identify the section type (Book, Game, Movie, Receipt) and mark the next line to be skipped.
            if (line == "Book" || line == "Game" || line == "Movie" || line == "Receipt")
            {
                section = line;
                skipNextLine = true;
                continue;
            }

            // Skip the section header line to avoid processing it as data.
            if (skipNextLine)
            {
                skipNextLine = false;
                continue;
            }

            // Split the line into parts using ';' as a delimiter.
            var parts = line.Split(';');

            // Ensure there are enough parts to process a valid record.
            if (parts.Length < 2) continue;

            // Process the data based on the current section type.
            switch (section)
            {
                case "Book":
                    // Parse and add a book
                    books.Add(new Book
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Price = decimal.Parse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture),
                        Stock = int.Parse(parts[3]),
                        Genre = parts[4],
                        Format = parts[5],
                        Language = parts[6],
                        Author = parts[7]
                    });
                    break;
                case "Game":
                    // Parse and add a game
                    games.Add(new Game
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Price = decimal.Parse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture),
                        Stock = int.Parse(parts[3]),
                        Platform = parts[4]
                    });
                    break;
                case "Movie":
                    // Parse and add a movie
                    movies.Add(new Movie
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Price = decimal.Parse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture),
                        Stock = int.Parse(parts[3]),
                        Format = parts[4],
                        Playtime = (parts.Length > 5 && int.TryParse(parts[5], out int playtime)) ? playtime : (int?)null
                    });
                    break;
                case "Receipt":
                    // Parse and add a receipt item to the appropriate receipt.
                    int id = int.Parse(parts[0]);
                    DateTime purchaseDate = DateTime.Parse(parts[2]);
                    var item = new ReceiptItem
                    {
                        ProductId = int.Parse(parts[5]),
                        ProductName = parts[1],
                        ProductPrice = decimal.Parse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture),
                        Quantity = int.Parse(parts[4])
                    };

                    // If the receipt does not exist in the dictionary, create and add it.
                    if (!receiptDict.TryGetValue(id, out var receipt))
                    {
                        receipt = new Receipt { Id = id, PurchaseDate = purchaseDate };
                        receiptDict[id] = receipt;
                        receipts.Add(receipt);
                    }
                    receipt.Items.Add(item);
                    break;
            }
        }

        // Return a StorageData object populated with the loaded data.
        return new StorageData
        {
            Books = books,
            Games = games,
            Movies = movies,
            Receipts = receipts,
            NextProductId = nextProductId,
            NextReceiptId = nextReceiptId
        };
    }

    /// <summary>
    /// Creates initial inventory data with sample books, games, and movies. (Same list as on Canvas for testing purposes)
    /// </summary>
    /// <returns>Initialized StorageData object.</returns>
    private StorageData CreateInitialData()
    {
        var books = new List<Book>
    {
        new Book { Id = 1, Name = "Bello Gallico et Civili", Price = 449m,  Author = "Julius Caesar", Stock = 10, Genre = "Historia", Format = "Inbunden", Language = "Latin"},
        new Book { Id = 2, Name = "How to Read a Book", Price = 149m,  Author = "Mortimer J. Adler", Stock = 10, Genre = "Kursliteratur", Format = "Pocket"},
        new Book { Id = 3, Name = "Moby Dick", Price = 49m,  Author = "Herman Melville", Stock = 10, Genre = "Äventyr", Format = "Pocket"},
        new Book { Id = 4, Name = "Great Gatsby", Price = 79m,  Author = "F. Scott Fitzgerald", Stock = 10, Genre = "Noir", Format = "E-Bok"},
        new Book { Id = 5, Name = "House of Leaves", Price = 49m,  Author = "Mark Z. Danielewski", Stock = 10, Genre = "Skräck"},
    };

        var games = new List<Game>
    {
        new Game { Id = 6, Name = "Elden Ring", Price = 599m, Stock = 5, Platform = "Playstation 5" },
        new Game { Id = 7, Name = "Demon's Souls", Price = 499m, Stock = 7, Platform = "Playstation 5" },
        new Game { Id = 8, Name = "Microsoft Flight Simulator", Price = 499m, Stock = 4, Platform = "PC" },
        new Game { Id = 9, Name = "Planescape Torment", Price = 99m, Stock = 3, Platform = "PC" },
        new Game { Id = 10, Name = "Disco Elysium", Price = 399m, Stock = 0, Platform = "PC" },
    };

        var movies = new List<Movie>
    {
        new Movie { Id = 11, Name = "Nyckeln till frihet", Price = 99m, Stock = 10, Format = "DVD", Playtime = 142 },
        new Movie { Id = 12, Name = "Gudfadern", Price = 99m, Stock = 10, Format = "DVD", Playtime = 152 },
        new Movie { Id = 13, Name = "Konungens återkomst", Price =  199m, Stock = 10, Format = "Blu-Ray", Playtime = 154 },
        new Movie { Id = 14, Name = "Pulp fiction", Price =  199m, Stock = 10, Format = "Blu-Ray"},
        new Movie { Id = 15, Name = "Schindlers list", Price =  99m, Stock = 10, Format = "DVD"},
    };

        return new StorageData
        {
            Books = books,
            Games = games,
            Movies = movies,
            Receipts = new List<Receipt>(), 
            NextProductId = 10000, // First 9999 IDs reserved for the inventory API products (future implementation)
            NextReceiptId = 1 // Start receipts from ID 1
        };
    }
}
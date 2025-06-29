using DVGB07_Inventory_App.Models;
using System;
using System.Collections.Generic;

namespace DVGB07_Inventory_App.DTO;

/// <summary>
/// Represents the storage data structure used by the StorageService and IStorageService.
/// This class serves as both input and output for storing and retrieving inventory data.
/// </summary>
public class StorageData
{
    /// <summary>
    /// Gets or sets the collection of books in storage.
    /// </summary>
    public IEnumerable<Book> Books { get; set; }

    /// <summary>
    /// Gets or sets the collection of games in storage.
    /// </summary>
    public IEnumerable<Game> Games { get; set; }

    /// <summary>
    /// Gets or sets the collection of movies in storage.
    /// </summary>
    public IEnumerable<Movie> Movies { get; set; }

    /// <summary>
    /// Gets or sets the collection of receipts in storage.
    /// </summary>
    public IEnumerable<Receipt> Receipts { get; set; }

    /// <summary>
    /// Gets or sets the next available product ID.
    /// Used to generate unique product identifiers.
    /// </summary>
    public int NextProductId { get; set; }

    /// <summary>
    /// Gets or sets the next available receipt ID.
    /// Used to generate unique receipt identifiers.
    /// </summary>
    public int NextReceiptId { get; set; }
}

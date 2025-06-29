using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.Models;

/// <summary>
/// Represents a book product in the inventory.
/// </summary>
public class Book : Product
{
    private string? _genre;
    private string? _format;
    private string? _language;
    private string? _author;

    /// <summary>
    /// Gets or sets the genre of the book.
    /// </summary>
    public string? Genre
    {
        get => _genre;
        set => SetProperty(ref _genre, value, nameof(Genre));
    }

    /// <summary>
    /// Gets or sets the format of the book (e.g., Hardcover, Paperback, eBook).
    /// </summary>
    public string? Format
    {
        get => _format;
        set => SetProperty(ref _format, value, nameof(Format));
    }

    /// <summary>
    /// Gets or sets the language of the book.
    /// </summary>
    public string? Language
    {
        get => _language;
        set => SetProperty(ref _language, value, nameof(Language));
    }

    /// <summary>
    /// Gets or sets the author of the book.
    /// </summary>
    public string? Author
    {
        get => _author;
        set => SetProperty(ref _author, value, nameof(Author));
    }

    /// <inheritdoc/>
    public override Product Clone()
    {
        return new Book
        {
            Id = this.Id,
            Name = this.Name,
            Price = this.Price,
            Stock = this.Stock,
            Genre = this.Genre,
            Format = this.Format,
            Language = this.Language,
            Author = this.Author
        };
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return base.ToString() +
               $", Genre: {Genre}, Format: {Format}, Language: {Language}, Author: {Author}";
    }
}

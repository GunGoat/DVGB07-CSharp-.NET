using System;

namespace DVGB07_Inventory_App.DTO;

/// <summary>
/// Represents filter options specific to books.
/// </summary>
public class BookFilterOptions : ProductFilterOptions
{
    private string? _genre;
    private string? _format;
    private string? _language;
    private string? _author;

    /// <summary>
    /// Gets or sets the genre filter.
    /// </summary>
    public string? Genre
    {
        get => _genre;
        set => SetProperty(ref _genre, string.IsNullOrWhiteSpace(value) ? null : value, nameof(Genre), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets or sets the format filter.
    /// </summary>
    public string? Format
    {
        get => _format;
        set => SetProperty(ref _format, string.IsNullOrWhiteSpace(value) ? null : value, nameof(Format), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets or sets the language filter.
    /// </summary>
    public string? Language
    {
        get => _language;
        set => SetProperty(ref _language, string.IsNullOrWhiteSpace(value) ? null : value, nameof(Language), UpdateAnyFilterEnabled);
    }

    /// <summary>
    /// Gets or sets the author filter.
    /// </summary>
    public string? Author
    {
        get => _author;
        set => SetProperty(ref _author, string.IsNullOrWhiteSpace(value) ? null : value, nameof(Author), UpdateAnyFilterEnabled);
    }

    protected override void UpdateAnyFilterEnabled()
    {
        AnyFilterEnabled = _name != null || _minPrice != null || _maxPrice != null || _genre != null || _format != null || _language != null || _author != null;
    }

    public override void ClearFilters() => (Name, MinPrice, MaxPrice, Genre, Format, Language, Author) = (null, null, null, null, null, null, null);
}

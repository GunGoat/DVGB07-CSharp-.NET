using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.Models;

/// <summary>
/// Represents a movie product in the inventory.
/// </summary>
public class Movie : Product
{
    private string? _format;
    private int? _duration;

    /// <summary>
    /// Gets or sets the format of the movie (e.g., Blu-ray, DVD, Digital).
    /// </summary>
    public string? Format
    {
        get => _format;
        set => SetProperty(ref _format, value, nameof(Format));
    }

    /// <summary>
    /// Gets or sets the duration of the movie in minutes.
    /// </summary>
    public int? Playtime
    {
        get => _duration;
        set => SetProperty(ref _duration, value, nameof(Playtime));
    }

    /// <inheritdoc/>
    public override Product Clone()
    {
        return new Movie
        {
            Id = this.Id,
            Name = this.Name,
            Price = this.Price,
            Stock = this.Stock,
            Format = this.Format,
            Playtime = this.Playtime
        };
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return base.ToString() + $", Format: {Format}, Playtime: {Playtime} mins";
    }
}
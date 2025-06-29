using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.Models;

/// <summary>
/// Represents a game product in the inventory.
/// </summary>
public class Game : Product
{
    private string? _platform;

    /// <summary>
    /// Gets or sets the platform for the game (e.g., PC, Xbox, PlayStation).
    /// </summary>
    public string? Platform
    {
        get => _platform;
        set => SetProperty(ref _platform, value, nameof(Platform));
    }

    /// <inheritdoc/>
    public override Product Clone()
    {
        return new Game
        {
            Id = this.Id,
            Name = this.Name,
            Price = this.Price,
            Stock = this.Stock,
            Platform = this.Platform,
        };
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return base.ToString() + $", Platform: {Platform}";
    }
}

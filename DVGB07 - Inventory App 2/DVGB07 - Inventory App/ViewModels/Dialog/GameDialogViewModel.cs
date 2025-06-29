using DVGB07_Inventory_App.Commands;
using DVGB07_Inventory_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.ViewModels;

/// <summary>
/// View model for managing game product dialogs (add or update).
/// </summary>
public class GameDialogViewModel : ProductDialogViewModel
{
    /// <summary>
    /// Gets or sets the selected game.
    /// </summary>
    public Game SelectedGame { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameDialogViewModel"/> class for adding a new game.
    /// </summary>
    public GameDialogViewModel()
    {
        SelectedGame = new Game();
        WindowTitle = "Add New Game";
        SubscribeToGameChanges();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameDialogViewModel"/> class for updating an existing game.
    /// </summary>
    public GameDialogViewModel(Game game)
    {
        SelectedGame = (Game)game.Clone();
        WindowTitle = "Update Game";
        SubscribeToGameChanges();
    }

    /// <summary>
    /// Saves the selected game if it is valid.
    /// </summary>
    public override void Save(object parameter)
    {
        if (SelectedGame.IsValid())
        {
            OnItemSaved(SelectedGame);
            CloseWindow?.Invoke();
            SubscribeToGameChanges();
        }
    }

    /// <summary>
    /// Determines whether the game can be saved.
    /// </summary>
    public override bool CanSave(object parameter)
    {
        return !string.IsNullOrWhiteSpace(SelectedGame?.Name)
               && SelectedGame?.Price >= 0;
    }

    /// <summary>
    /// Subscribes to property changes for the selected game and raises <see cref="SaveCommand"/> can execute changes.
    /// </summary>
    private void SubscribeToGameChanges()
    {
        SelectedGame.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(SelectedGame.Name) || args.PropertyName == nameof(SelectedGame.Price))
            {
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        };
    }
}
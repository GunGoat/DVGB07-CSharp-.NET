using DVGB07_Inventory_App.Commands;
using DVGB07_Inventory_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DVGB07_Inventory_App.ViewModels;

/// <summary>
/// View model for managing movie product dialogs (add or update).
/// </summary>
public class MovieDialogViewModel : ProductDialogViewModel
{
    /// <summary>
    /// Gets or sets the selected movie.
    /// </summary>
    public Movie SelectedMovie { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MovieDialogViewModel"/> class for adding a new movie.
    /// </summary>
    public MovieDialogViewModel()
    {
        SelectedMovie = new Movie();
        WindowTitle = "Add New Movie";
        SubscribeToMovieChanges();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MovieDialogViewModel"/> class for updating an existing movie.
    /// </summary>
    public MovieDialogViewModel(Movie movie)
    {
        SelectedMovie = (Movie)movie.Clone();
        WindowTitle = "Update Movie";
        SubscribeToMovieChanges();
    }

    /// <summary>
    /// Saves the selected movie if it is valid.
    /// </summary>
    public override void Save(object parameter)
    {
        if (SelectedMovie.IsValid())
        {
            OnItemSaved(SelectedMovie);
            CloseWindow?.Invoke();
        }
    }

    /// <summary>
    /// Determines whether the movie can be saved.
    /// </summary>
    public override bool CanSave(object parameter)
    {
        return !string.IsNullOrWhiteSpace(SelectedMovie?.Name)
               && SelectedMovie?.Price >= 0;
    }

    /// <summary>
    /// Subscribes to property changes for the selected movie and raises <see cref="SaveCommand"/> can execute changes.
    /// </summary>
    private void SubscribeToMovieChanges()
    {
        SelectedMovie.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(SelectedMovie.Name) || args.PropertyName == nameof(SelectedMovie.Price))
            {
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        };
    }
}

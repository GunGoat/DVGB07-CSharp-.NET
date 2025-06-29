using DVGB07_Inventory_App.Commands;
using DVGB07_Inventory_App.Models;
using System.Media;
using System.Windows.Input;

namespace DVGB07_Inventory_App.ViewModels;

/// <summary>
/// View model for managing book product dialogs (add or update).
/// </summary>
public class BookDialogViewModel : ProductDialogViewModel
{
    /// <summary>
    /// Gets or sets the selected book.
    /// </summary>
    public Book SelectedBook { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BookDialogViewModel"/> class for adding a new book.
    /// </summary>
    public BookDialogViewModel()
    {
        SelectedBook = new Book();
        WindowTitle = "Add New Book";
        SubscribeToBookChanges();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BookDialogViewModel"/> class for updating an existing book.
    /// </summary>
    public BookDialogViewModel(Book book)
    {
        SelectedBook = (Book)book.Clone();
        WindowTitle = "Update Book";
        SubscribeToBookChanges();
    }

    /// <summary>
    /// Saves the selected book if it is valid.
    /// </summary>
    public override void Save(object parameter)
    {
        if (SelectedBook.IsValid())
        {
            OnItemSaved(SelectedBook);
            CloseWindow?.Invoke();
        }
    }

    /// <summary>
    /// Determines whether the book can be saved.
    /// </summary>
    public override bool CanSave(object parameter)
    {
        return !string.IsNullOrWhiteSpace(SelectedBook?.Name)
               && SelectedBook.Price.HasValue
               && SelectedBook?.Price >= 0;
    }

    /// <summary>
    /// Subscribes to property changes for the selected book and raises <see cref="SaveCommand"/> can execute changes.
    /// </summary>
    private void SubscribeToBookChanges()
    {
        SelectedBook.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(SelectedBook.Name) || args.PropertyName == nameof(SelectedBook.Price))
            {
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        };
    }
}

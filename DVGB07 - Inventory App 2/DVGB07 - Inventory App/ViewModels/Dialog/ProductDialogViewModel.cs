using DVGB07_Inventory_App.Commands;
using DVGB07_Inventory_App.Models;
using System;
using System.Windows.Input;

namespace DVGB07_Inventory_App.ViewModels;

/// <summary>
/// Abstract base class for product dialog view models. Handles common functionality for saving and cancelling changes to products.
/// </summary>
public abstract class ProductDialogViewModel : PropertyChangedBase
{
    protected string _windowTitle;

    /// <summary>
    /// Event triggered when an item is created.
    /// </summary>
    public event EventHandler<Product> ItemCreated;

    /// <summary>
    /// Gets or sets the title of the dialog window.
    /// </summary>
    public string WindowTitle
    {
        get => _windowTitle;
        set => SetProperty(ref _windowTitle, value, nameof(WindowTitle));
    }

    /// <summary>
    /// Action to close the window.
    /// </summary>
    public Action CloseWindow { get; set; }

    /// <summary>
    /// Command to save the product.
    /// </summary>
    public ICommand SaveCommand { get; protected set; }

    /// <summary>
    /// Command to cancel changes.
    /// </summary>
    public ICommand CancelCommand { get; protected set; }

    /// <summary>
    /// Initializes the commands.
    /// </summary>
    protected ProductDialogViewModel()
    {
        SaveCommand = new RelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel);
    }

    /// <summary>
    /// Saves the product. This method should be implemented in derived classes.
    /// </summary>
    public abstract void Save(object parameter);

    /// <summary>
    /// Determines whether the product can be saved. This method can be overridden by derived classes.
    /// </summary>
    public virtual bool CanSave(object parameter) => true;

    /// <summary>
    /// Cancels the operation and closes the window.
    /// </summary>
    public virtual void Cancel(object parameter) => CloseWindow?.Invoke();

    /// <summary>
    /// Invokes the ItemCreated event when an item is saved.
    /// </summary>
    protected void OnItemSaved(Product newItem) => ItemCreated?.Invoke(this, newItem);
}

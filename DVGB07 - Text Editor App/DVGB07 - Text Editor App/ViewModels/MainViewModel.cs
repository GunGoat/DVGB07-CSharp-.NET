using DVGB07_Text_Editor_App.Commands;
using DVGB07_Text_Editor_App.DTO;
using DVGB07_Text_Editor_App.Models;
using System.ComponentModel;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace DVGB07_Text_Editor_App.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private DocumentModel _document;

    /// <summary>
    /// Gets or sets the current document being edited.
    /// </summary>
    public DocumentModel Document
    {
        get => _document;
        set
        {
            if (_document != value)
            {
                _document = value;
                OnPropertyChanged(nameof(Document));
                OnPropertyChanged(nameof(WindowTitle)); 
            }
        }
    }

    /// <summary>
    /// Gets the title of the window, including an asterisk (*) if the document is modified.
    /// </summary>
    public string WindowTitle
    {
        get
        {
            if (string.IsNullOrEmpty(Document.FilePath))
                return "Untitled" + (Document.IsModified ? "*" : string.Empty);

            return Path.GetFileName(Document.FilePath) + (Document.IsModified ? "*" : string.Empty);
        }
    }

    public ICommand OpenFileCommand { get; }
    public ICommand CreateFileCommand { get; }
    public ICommand SaveFileCommand { get; }
    public ICommand SaveFileAsCommand { get; }
    public ICommand FileDropCommand { get; }

    public MainViewModel()
    {
        Document = new DocumentModel();
        OpenFileCommand = new RelayCommand(OpenFile);
        CreateFileCommand = new RelayCommand(CreateFile);
        SaveFileCommand = new RelayCommand(SaveFile);
        SaveFileAsCommand = new RelayCommand(SaveFileAs);
        FileDropCommand = new RelayCommand(ExecuteFileDrop);

        // Listen for changes to the Document content to track if it's modified
        Document.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(Document.Content) || args.PropertyName == nameof(Document.IsModified))
            {
                OnPropertyChanged(nameof(WindowTitle));
            }
        };
    }

    /// <summary>
    /// Handles file drop actions based on the provided parameters.
    /// </summary>
    /// <param name="parameter">The file drop arguments, including file path and modifier keys.</param>
    private void ExecuteFileDrop(object parameter)
    {
        if (parameter is FileDropArgs args)
        {
            try
            {
                string fileContent = File.ReadAllText(args.FilePath, Encoding.UTF8);

                if (args.ModifierKeys == ModifierKeys.Control)
                {
                    // Append content
                    _document.Content += fileContent;
                }
                else if (args.ModifierKeys == ModifierKeys.Shift)
                {
                    // Insert at cursor position
                    _document.Content = _document.Content.Insert(args.CaretIndex, fileContent);
                }
                else if(PromptAndHandleUnsavedChanges() != MessageBoxResult.Cancel)
                {
                    // Open new document
                    Document.FilePath = args.FilePath;
                    Document.Content = fileContent; 
                    Document.MarkAsSaved();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Opens a text file and loads its content into the document.
    /// </summary>
    private void OpenFile(object parameter)
    {
        var result = PromptAndHandleUnsavedChanges();

        if (result == MessageBoxResult.Cancel)
        {
            return; 
        }

        var openFileDialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            Title = "Open Text File"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            string fileContent = File.ReadAllText(openFileDialog.FileName);
            Document.FilePath = openFileDialog.FileName;
            Document.Content = fileContent;
            Document.MarkAsSaved();
        }
    }

    /// <summary>
    /// Creates a new document, clearing the current content.
    /// </summary>
    private void CreateFile(object parameter)
    {
        var result = PromptAndHandleUnsavedChanges();

        if (result == MessageBoxResult.Cancel)
        {
            return; // User canceled the operation
        }

        Document.Content = string.Empty;
        Document.FilePath = string.Empty;
        Document.MarkAsSaved();
    }

    /// <summary>
    /// Saves the current document to its file path.
    /// If the file path is not set, prompts the user to choose a file path.
    /// </summary>
    private void SaveFile(object parameter)
    {
        if (string.IsNullOrEmpty(Document.FilePath))
        {
            // If the file path is not set, use SaveFileAs logic
            SaveFileAs(parameter);
        }
        else
        {
            // Save the current content to the existing file
            File.WriteAllText(Document.FilePath, Document.Content);
            Document.MarkAsSaved();
        }
    }

    /// <summary>
    /// Prompts the user to save the current document to a new file path.
    /// </summary>
    private void SaveFileAs(object parameter)
    {
        // Create a SaveFileDialog to prompt the user for a file path
        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*", // Filter for text files
            Title = "Save Text File" // Title of the file dialog
        };

        // Show the dialog and check if the user selected a file path
        if (saveFileDialog.ShowDialog() == true)
        {
            // Save the content to the selected path
            File.WriteAllText(saveFileDialog.FileName, Document.Content);
            Document.FilePath = saveFileDialog.FileName;
            Document.MarkAsSaved();
        }
    }

    /// <summary>
    /// Attempts to close the window by checking if there are unsaved changes.
    /// If there are unsaved changes, prompts the user to save, discard, or cancel.
    /// </summary>
    /// <returns>True if the user chooses to proceed with closing the window; otherwise, false.</returns>
    public bool CloseWindow()
    {
        var result = PromptAndHandleUnsavedChanges();
        return result != MessageBoxResult.Cancel;
    }

    /// <summary>
    /// Checks if the document has unsaved changes and prompts the user to save, discard, or cancel.
    /// </summary>
    /// <returns>The user's choice: Yes (save), No (discard), or Cancel.</returns>
    private MessageBoxResult PromptAndHandleUnsavedChanges()
    {
        if (Document.IsModified)
        {
            var result = System.Windows.MessageBox.Show(
                "You have unsaved changes. Do you want to save them before proceeding?",
                "Unsaved Changes",
                System.Windows.MessageBoxButton.YesNoCancel,
                System.Windows.MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                SaveFile(null); 
            }

            return result;
        }

        // If document is not modified just proceed
        return MessageBoxResult.Yes;
    }

    /// <summary>
    /// Event triggered when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

using DVGB07_Text_Editor_App.DTO;
using DVGB07_Text_Editor_App.ViewModels;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace DVGB07_Text_Editor_App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool _isCloseButtonClicked = false;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    /// <summary>
    /// Handles the Window Closing event.
    /// Prompts the user to save changes before closing, if necessary.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event arguments containing information about the closing event.</param>
    private void Window_Closing(object sender, CancelEventArgs e)
    {
        if (DataContext is MainViewModel viewModel && !_isCloseButtonClicked)
        {
            if (!viewModel.CloseWindow())
            {
                e.Cancel = true;
            }
        }
    }

    /// <summary>
    /// Alternative method to handle the Window Closing event using a button.
    /// Prompts the user to save changes before closing, if necessary.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event arguments for the button click event.</param>
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            if (viewModel.CloseWindow())
            {
                _isCloseButtonClicked = true;
                this.Close();
            }
        }
    }

    /// <summary>
    /// Handles the Drop event for the TextBox.
    /// Processes a dropped text file and updates the TextBox content.
    /// </summary>
    /// <param name="sender">The TextBox that is the target of the drop operation.</param>
    /// <param name="e">The event arguments containing information about the drop operation.</param>
    private void TextBox_Drop(object sender, DragEventArgs e)
    {
        if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var filePaths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (filePaths != null && filePaths.Length > 0)
            {
                string filePath = filePaths.First();

                // Ensure it's a text file
                if (Path.GetExtension(filePath).ToLower() == ".txt")
                {
                    var textBox = sender as TextBox;
                    if (textBox != null && DataContext is MainViewModel viewModel)
                    {
                        var args = new FileDropArgs
                        {
                            FilePath = filePath,
                            ModifierKeys = Keyboard.Modifiers,
                            CaretIndex = textBox.CaretIndex
                        };

                        if (viewModel.FileDropCommand.CanExecute(args))
                        {
                            viewModel.FileDropCommand.Execute(args);
                        }
                    }
                }
            }

            e.Handled = true;
        }
    }

    /// <summary>
    /// Handles the DragOver event for the TextBox.
    /// Determines whether a drag operation is allowed and provides visual feedback.
    /// </summary>
    /// <param name="sender">The TextBox that is the target of the drag operation.</param>
    /// <param name="e">The event arguments containing information about the drag operation.</param>

    private void TextBox_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    /// <summary>
    /// Handles the PreviewDragOver event for the TextBox.
    /// Prevents the default drag-and-drop behavior.
    /// </summary>
    /// <param name="sender">The TextBox that is the target of the drag operation.</param>
    /// <param name="e">The event arguments containing information about the drag operation.</param>
    private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
    {
        e.Handled = true;
    }
}
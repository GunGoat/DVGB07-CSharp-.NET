using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DVGB07_Inventory_App.Views.Template;

/// <summary>
/// A custom UserControl that displays a labelled ComboBox.
/// Allows binding a list of selectable items and a selected value.
/// </summary>
public partial class LabelledComboBox : UserControl
{
    /// <summary>
    /// DependencyProperty for the label text displayed next to the ComboBox.
    /// </summary>
    public static readonly DependencyProperty LabelTextProperty =
        DependencyProperty.Register(nameof(LabelText), typeof(string), typeof(LabelledComboBox), new PropertyMetadata(string.Empty));

    /// <summary>
    /// DependencyProperty for the items displayed in the ComboBox.
    /// This should be an <see cref="ObservableCollection{T}"/> of <see cref="ComboBoxItemModel"/>.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(ObservableCollection<ComboBoxItemModel>), typeof(LabelledComboBox), new PropertyMetadata(null));

    /// <summary>
    /// DependencyProperty for the currently selected value in the ComboBox.
    /// </summary>
    public static readonly DependencyProperty SelectedValueProperty =
        DependencyProperty.Register(nameof(SelectedValue), typeof(ComboBoxItemModel), typeof(LabelledComboBox), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the label text displayed next to the ComboBox.
    /// </summary>
    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the list of items available for selection in the ComboBox.
    /// </summary>
    public ObservableCollection<ComboBoxItemModel> ItemsSource
    {
        get => (ObservableCollection<ComboBoxItemModel>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently selected item in the ComboBox.
    /// </summary>
    public ComboBoxItemModel? SelectedValue
    {
        get => (ComboBoxItemModel?)GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelledComboBox"/> class.
    /// </summary>
    public LabelledComboBox()
    {
        InitializeComponent();
    }
}

/// <summary>
/// Represents an item in the ComboBox, containing both display text and a value.
/// </summary>
public class ComboBoxItemModel
{
    /// <summary>
    /// Gets or sets the text displayed in the ComboBox for this item.
    /// </summary>
    public string DisplayText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the underlying value associated with this item.
    /// </summary>
    public object Value { get; set; } = default!;
}

using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DVGB07_Inventory_App.Views.Template;

/// <summary>
/// Interaction logic for LabelledIntegerBox.xaml
/// </summary>
public partial class LabelledIntegerBox : UserControl
{
    public static readonly DependencyProperty LabelTextProperty =
        DependencyProperty.Register(nameof(LabelText), typeof(string), typeof(LabelledIntegerBox), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty InputTextProperty =
        DependencyProperty.Register(nameof(InputText), typeof(string), typeof(LabelledIntegerBox), new PropertyMetadata(string.Empty));

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public string InputText
    {
        get => (string)GetValue(InputTextProperty);
        set => SetValue(InputTextProperty, value);
    }

    public LabelledIntegerBox()
    {
        InitializeComponent();
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Only allow positive integer input (no decimals, no negative signs)
        var regex = new Regex("^[0-9]+$");

        e.Handled = !regex.IsMatch(e.Text);
    }
}
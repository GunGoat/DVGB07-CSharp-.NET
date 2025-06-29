using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DVGB07_Inventory_App.Views.Template;

/// <summary>
/// Interaction logic for LabelledNumberBox.xaml
/// </summary>
public partial class LabelledNumberBox : UserControl
{
    public static readonly DependencyProperty LabelTextProperty =
        DependencyProperty.Register(nameof(LabelText), typeof(string), typeof(LabelledNumberBox), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty InputTextProperty =
        DependencyProperty.Register(nameof(InputText), typeof(string), typeof(LabelledNumberBox), new PropertyMetadata(string.Empty));

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

    public LabelledNumberBox()
    {
        InitializeComponent();
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Allow only numbers and a single dot (for decimals)
        var textBox = sender as TextBox;
        string currentText = textBox?.Text ?? "";

        // Regex pattern explanation:
        // ^ - Start of the string
        // \d* - Allow any number of digits before the decimal
        // \.?\d* - Allow an optional decimal point followed by digits
        // $ - End of the string
        var regex = new Regex(@"^\d*\.?\d*$");

        // Prevent multiple dots
        if (e.Text == "." && currentText.Contains("."))
        {
            e.Handled = true; // Block additional dots
        }
        else
        {
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
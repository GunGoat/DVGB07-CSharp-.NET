using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
/// Interaction logic for LabelledTextBox.xaml
/// </summary>
public partial class LabelledTextBox : UserControl
{
    public static readonly DependencyProperty LabelTextProperty =
        DependencyProperty.Register(nameof(LabelText), typeof(string), typeof(LabelledTextBox), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty InputTextProperty =
        DependencyProperty.Register(nameof(InputText), typeof(string), typeof(LabelledTextBox), new PropertyMetadata(string.Empty));

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

    public LabelledTextBox()
    {
        InitializeComponent();
        this.AddHandler(TextBox.PreviewTextInputEvent, new TextCompositionEventHandler(OnPreviewTextInput), true);
    }

    // <summary>
    /// Handles the PreviewTextInput event to prevent the input of the CSV delimiter ';'.
    /// </summary>
    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (e.Text.Contains(";"))
        {
            // Setting Handled to true marks the event as handled
            // preventing the character from being entered into the TextBox.
            e.Handled = true; 
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;

namespace DVGB07_Inventory_App.Views.Template;

/// <summary>
/// Interaction logic for LabelledDatePicker.xaml
/// </summary>
public partial class LabelledDatePicker : UserControl
{
    public static readonly DependencyProperty LabelTextProperty =
        DependencyProperty.Register(nameof(LabelText), typeof(string), typeof(LabelledDatePicker), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SelectedDateProperty =
        DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(LabelledDatePicker), new PropertyMetadata(null));

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public DateTime? SelectedDate
    {
        get => (DateTime?)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public LabelledDatePicker()
    {
        InitializeComponent();
    }
}

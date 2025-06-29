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

namespace DVGB07_Inventory_App.Views;

/// <summary>
/// Interaction logic for BooksView.xaml
/// </summary>
public partial class BooksWindow : UserControl
{
    public BooksWindow()
    {
        InitializeComponent();
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {
        if (toggleButton.IsChecked == true)
        {
            toggleButton.Content = "Hide";
            filterOptions.Visibility = Visibility.Visible; 
        }
        else
        {
            toggleButton.Content = "Show";
            filterOptions.Visibility = Visibility.Collapsed;
        }
    }
}

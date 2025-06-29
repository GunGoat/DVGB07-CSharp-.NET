using DVGB07_Inventory_App.ViewModels;
using DVGB07_Inventory_App;
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
using System.Windows.Shapes;

namespace DVGB07_Inventory_App.Views;

/// <summary>
/// Interaction logic for StoreWindow.xaml
/// </summary>
public partial class StoreWindow : Window
{
    public StoreWindow()
    {
        InitializeComponent();
        DataContext = new StoreViewModel();
    }

    /// <summary>
    /// Handles the click event for navigating to the Store window.
    /// Preserves the current window's position and size when opening the new window.
    /// </summary>
    private void GoToInventoryButton_Click(object sender, RoutedEventArgs e)
    {
        double left = this.Left;
        double top = this.Top;
        double width = this.Width;
        double height = this.Height;

        var inventoryWindow = new InventoryWindow
        {
            Left = left,
            Top = top,
            Width = width,
            Height = height
        };

        inventoryWindow.Show();
        this.Close();
    }
}

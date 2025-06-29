using DVGB07_Inventory_App.Views;
using DVGB07_Inventory_App.ViewModels;
using DVGB07_Inventory_App.Views;
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

namespace DVGB07_Inventory_App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class InventoryWindow : Window
{
    public InventoryWindow()
    {
        InitializeComponent();
        DataContext = new InventoryViewModel();
    }

    /// <summary>
    /// Handles the click event for navigating to the Inventory window.
    /// Preserves the current window's position and size when opening the new window.
    /// </summary>
    private void GoToStoreButton_Click(object sender, RoutedEventArgs e)
    {
        double left = this.Left;
        double top = this.Top;
        double width = this.Width;
        double height = this.Height;

        var storeWindow = new StoreWindow
        {
            Left = left,
            Top = top,
            Width = width,
            Height = height
        };

        storeWindow.Show();
        this.Close();
    }
}
using DVGB07_Inventory_App.DTO;
using DVGB07_Inventory_App.Service;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

namespace DVGB07_Inventory_App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IStorageService _storageService;
    public static IDataService DataService { get; private set; }
    public static IPrintService PrintService { get; private set; }
    public static IApiService ApiService { get; private set; }
    public static ITimerService TimerService { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Set the culture globally
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

        // Initialize services
        _storageService = new StorageService();
        DataService = new DataService(_storageService);
        PrintService = new PrintService();
        ApiService = new ApiService();
        TimerService = new TimerService();
        TimerService.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        TimerService?.Stop();
        var storageData = new StorageData()
        {
            Movies = DataService.Movies,
            Books = DataService.Books,
            Games = DataService.Games,
            Receipts = DataService.Receipts,
            NextProductId = DataService.NextProductId,
            NextReceiptId = DataService.NextReceiptId,
        };
        _storageService.Save(storageData);
        base.OnExit(e);
    }
}
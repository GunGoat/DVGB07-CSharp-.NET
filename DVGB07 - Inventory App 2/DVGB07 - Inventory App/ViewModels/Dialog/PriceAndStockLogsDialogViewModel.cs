using DVGB07_Inventory_App.Service;
using DVGB07_Inventory_App.Views.Template;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System.Collections.ObjectModel;

namespace DVGB07_Inventory_App.ViewModels.Dialog;

/// <summary>
/// ViewModel for the Price and Stock Logs dialog.
/// </summary>
public class PriceAndStockLogsDialogViewModel : PropertyChangedBase
{
    private ComboBoxItemModel? _selectedOption;
    private PlotModel? _plotModelPrice;
    private PlotModel? _plotModelStock;
    private DateTime? _startDate;
    private DateTime? _endDate;

    /// <summary>
    /// Gets or sets the plot model for the price graph.
    /// </summary>
    public PlotModel? PlotModelPrice
    {
        get => _plotModelPrice;
        set
        {
            _plotModelPrice = value;
            OnPropertyChanged(nameof(PlotModelPrice));
        }
    }

    /// <summary>
    /// Gets or sets the plot model for the stock graph.
    /// </summary>
    public PlotModel? PlotModelStock
    {
        get => _plotModelStock;
        set
        {
            _plotModelStock = value;
            OnPropertyChanged(nameof(PlotModelStock));
        }
    }

    /// <summary>
    /// Gets or sets the selected product from the ComboBox.
    /// </summary>
    public ComboBoxItemModel? SelectedOption
    {
        get => _selectedOption;
        set => SetProperty(ref _selectedOption, value, nameof(SelectedOption), DrawGraphs);
    }

    /// <summary>
    /// Filters logs from this date onward. Null disables filtering.
    /// </summary>
    public DateTime? StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value, nameof(StartDate), DrawGraphs);
    }

    /// <summary>
    /// Filters logs up to this date. Null disables filtering.
    /// </summary>
    public DateTime? EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value, nameof(EndDate), DrawGraphs);
    }

    /// <summary>
    /// Collection of products available for selection in the ComboBox.
    /// </summary>
    public ObservableCollection<ComboBoxItemModel> Options { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PriceAndStockLogsDialogViewModel"/> class.
    /// Populates the ComboBox options with product data from the data service.
    /// </summary>
    public PriceAndStockLogsDialogViewModel()
    {
        Options = new ObservableCollection<ComboBoxItemModel>();

        foreach (var product in App.DataService.Products)
        {
            if (!product.Id.HasValue || string.IsNullOrEmpty(product.Name))
                continue;

            Options.Add(new ComboBoxItemModel
            {
                DisplayText = $"{product.Name} ({product.GetType().Name})",
                Value = product.Id
            });
        }
    }

    /// <summary>
    /// Loads product price and stock logs based on the currently selected product.
    /// </summary>
    private void DrawGraphs()
    {
        DrawPriceGraph();
        DrawStockGraph();
    }

    /// <summary>
    /// Draws the price graph based on the selected product and date range.
    /// </summary>
    private void DrawPriceGraph()
    {
        if (SelectedOption?.Value is int productId)
        {
            var priceLogs = App.DataService.GetPriceLogs(productId);

            // Filter the price logs based on the selected date range
            if (StartDate.HasValue)
            {
                priceLogs = priceLogs.Where(log => log.Timestamp >= StartDate.Value).ToList();
            }
            if (EndDate.HasValue)
            {
                priceLogs = priceLogs.Where(log => log.Timestamp <= EndDate.Value).ToList();
            }

            // Create a new plot model for the graph
            var plotModel = new PlotModel
            {
                Title = "Price Log Over Time",
                TitleColor = OxyColors.Red
            };

            // Create the line series for price changes
            var priceSeries = new LineSeries
            {
                Title = "Price Log",
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = OxyColors.Red,
                Color = OxyColors.Red
            };

            // Add data points to the series
            foreach (var log in priceLogs.OrderBy(log => log.Timestamp))
            {
                // Convert Timestamp to a numeric value (double) for plotting
                priceSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(log.Timestamp), (double)log.Price));
            }

            // Add series to the plot model
            plotModel.Series.Add(priceSeries);

            // Add an axis for the timestamps (X-axis)
            plotModel.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "MM/dd/yyyy",
                Title = "Date",
                IntervalType = DateTimeIntervalType.Days
            });

            // Add an axis for the prices (Y-axis)
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Price",
                MinimumPadding = 0.05,
                MaximumPadding = 0.05
            });

            PlotModelPrice = plotModel;
        }
    }

    /// <summary>
    /// Draws the stock graph based on the selected product and date range.
    /// </summary>
    private void DrawStockGraph()
    {
        if (SelectedOption?.Value is int productId)
        {
            var stockLogs = App.DataService.GetStockLogs(productId);

            // Filter the stock logs based on the selected date range
            if (StartDate.HasValue)
            {
                stockLogs = stockLogs.Where(log => log.Timestamp >= StartDate.Value).ToList();
            }
            if (EndDate.HasValue)
            {
                stockLogs = stockLogs.Where(log => log.Timestamp <= EndDate.Value).ToList();
            }

            // Create a new plot model for the stock graph
            var plotModel = new PlotModel
            {
                Title = "Stock Log Over Time",
                TitleColor = OxyColors.Blue
            };

            // Create the line series for stock changes
            var stockSeries = new LineSeries
            {
                Title = "Stock Log",
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = OxyColors.Blue,
                Color = OxyColors.Blue
            };

            // Add data points to the series
            foreach (var log in stockLogs.OrderBy(log => log.Timestamp))
            {
                // Convert Timestamp to a numeric value (double) for plotting
                stockSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(log.Timestamp), log.Stock));
            }

            // Add series to the plot model
            plotModel.Series.Add(stockSeries);

            // Add an axis for the timestamps (X-axis)
            plotModel.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "MM/dd/yyyy",
                Title = "Date",
                IntervalType = DateTimeIntervalType.Days
            });

            // Add an axis for the stock (Y-axis)
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Stock",
                MinimumPadding = 0.05,
                MaximumPadding = 0.05
            });

            PlotModelStock = plotModel;
        }
    }
}

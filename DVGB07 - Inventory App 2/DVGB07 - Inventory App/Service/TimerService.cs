using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DVGB07_Inventory_App.Service;

/// <summary>
/// A service that provides a configurable repeating timer.
/// </summary>
public class TimerService : ITimerService
{
    private readonly DispatcherTimer _timer;

    /// <summary>
    /// Event triggered at every timer tick.
    /// </summary>
    public event EventHandler TimerTick;

    /// <summary>
    /// Initializes a new instance of TimerService with a default interval.
    /// </summary>
    /// <param name="intervalInSeconds">Interval in seconds (default: 60s).</param>
    public TimerService(double intervalInSeconds = 60)  // Default: 60 seconds
    {
        _timer = new DispatcherTimer();
        SetInterval(intervalInSeconds);
        _timer.Tick += (sender, args) => TimerTick?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    public void Start() => _timer.Start();

    /// <summary>
    /// Stops the timer.
    /// </summary>
    public void Stop() => _timer.Stop();

    /// <summary>
    /// Updates the timer interval.
    /// </summary>
    /// <param name="interval">The interval in seconds.</param>
    public void SetInterval(double interval)
    {
        if (interval <= 0)
            throw new ArgumentException("Interval must be greater than zero.");

        _timer.Interval = TimeSpan.FromSeconds(interval);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVGB07_Inventory_App.Service;

/// <summary>
/// Interface for a timer service that triggers events at a set interval.
/// </summary>
public interface ITimerService
{
    /// <summary>
    /// Event triggered when the timer interval elapses.
    /// </summary>
    event EventHandler TimerTick;

    /// <summary>
    /// Starts the timer.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the timer.
    /// </summary>
    void Stop();

    /// <summary>
    /// Sets the timer interval.
    /// </summary>
    /// <param name="interval">The time interval in seconds.</param>
    void SetInterval(double interval);
}
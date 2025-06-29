using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DVGB07_Inventory_App.ViewModels;

/// <summary>
/// Provides an implementation of <see cref="INotifyPropertyChanged"/> to support property change notifications.
/// </summary>
public abstract class PropertyChangedBase : INotifyPropertyChanged
{
    /// <summary>
    /// Event triggered when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event for the specified property names.
    /// </summary>
    /// <param name="propertyNames">The names of the properties that changed.</param>
    protected void OnPropertyChanged(params string[] propertyNames)
    {
        if (PropertyChanged != null)
        {
            foreach (var propertyName in propertyNames)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// Sets the value of a property and raises the <see cref="PropertyChanged"/> event if the value has changed.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="field">A reference to the backing field of the property.</param>
    /// <param name="value">The new value to be assigned.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="onChanged">An optional action to invoke if the property value changes.</param>
    /// <param name="additionalProperties">Additional property names to notify.</param>
    protected void SetProperty<T>(ref T field, T value, string propertyName, Action? onChanged = null, params string[] additionalProperties)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
            if (additionalProperties.Length > 0)
            {
                OnPropertyChanged(additionalProperties);
            }
            onChanged?.Invoke();
        }
    }
}

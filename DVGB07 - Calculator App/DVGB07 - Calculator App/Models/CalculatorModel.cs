using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DVGB07_Calculator_App.Models;

/// <summary>
/// Represents a calculator model that performs basic arithmetic operations and maintains a calculation history.
/// </summary>
public class CalculatorModel
{
    /// <summary>
    /// Gets right operand in current calculation.
    /// Value that the user can add digits to.
    /// </summary>
    public long CurrentValue { get; private set; }

    /// <summary>
    /// Gets left operand in current calculation.
    /// Also used for storing the result of the previous calculation.
    /// </summary>
    public long StoredValue { get; private set; }

    /// <summary>
    /// Gets the operator for the current calculation.
    /// </summary>
    public string? Operator { get; private set; }

    /// <summary>
    /// Gets a collection of calculation history entries.
    /// </summary>
    public ObservableCollection<HistoryEntry> History {get; set;} =new ObservableCollection<HistoryEntry>();

    private enum InputType { None, Digit, Operator, Equals }
    private InputType _lastInputType = InputType.None;

    /// <summary>
    /// Handles the input of a digit and updates the current value.
    /// </summary>
    /// <param name="digit">The digit to add to the current value.</param>
    /// <exception cref="OverflowException">Thrown if the current value exceeds the range of a long.</exception>
    public void HandleDigit(int digit)
    {
        try
        {
            if (_lastInputType == InputType.Equals)
            {
                // Equal followed by digit: start a new calculation
                Clear();
            }

            CurrentValue = checked(CurrentValue * 10 + digit);
            _lastInputType = InputType.Digit;
        }
        catch (OverflowException ex)
        {
            throw new OverflowException("The value exceeds the limits of a long.", ex);
        }
    }

    /// <summary>
    /// Handles the input of an operator and performs necessary state transitions.
    /// </summary>
    /// <param name="operatorSymbol">The operator to apply (+, -, *, /).</param>
    public void HandleOperator(string operatorSymbol)
    {

        if (_lastInputType == InputType.Operator)
        {
            // If the last input was an operator, replace it with the new one.
            // Example: User enters "10 + -" which means the "+" is replaced with "-".
            Operator = operatorSymbol;
            return;
        }

        if (_lastInputType == InputType.Equals)
        {
            // If the last input was "=", continue the calculation with the previous result as the left operand.
            // Example: User enters "15 + 5 =" (result is 20), then "*", meaning "20 *".
            StoredValue = CurrentValue;
        }
        else if (_lastInputType == InputType.Digit && Operator != null)
        {
            // If a digit was entered before the operator and there's already a pending operator,
            // perform the calculation and store the result as the left operand.
            // Example: User enters "15 + 5 -", which means "15 + 5" is calculated (result 20), then "-" is stored.
            StoredValue = CalculateResult();
        }
        else
        {
            // If it's the first operator in the calculation, store the current value as the left operand.
            // Example: User enters "10 *" → "10" is stored as the left operand.
            StoredValue = CurrentValue;
        }

        // Set the new operator and reset the right operand (current value).
        Operator = operatorSymbol;
        CurrentValue = 0;
        _lastInputType = InputType.Operator;
    }

    /// <summary>
    /// Calculates the result of the current operation and updates the history.
    /// </summary>
    /// <returns>The result of the calculation.</returns>
    /// <exception cref="OverflowException">Thrown if the result exceeds the range of a long.</exception>
    /// <exception cref="DivideByZeroException">Thrown if attempting to divide by zero.</exception>
    public long CalculateResult()
    {
        if (Operator == null && _lastInputType == InputType.Equals && History.Any())
        {
            // Repeat the most recent operation on the result
            var equation = History.First();
            StoredValue = equation.Result;
            CurrentValue = equation.Operand2;
            Operator = equation.Operator;
        }
        else if (Operator == null)
        {
            // No operation to perform
            return CurrentValue;
        }

        try
        {
            // Store original values for history tracking.
            long originalStoredValue = StoredValue;
            long originalCurrentValue = CurrentValue;
            long result = 0;

            // Perform the calculation based on the operator.
            switch (Operator)
            {
                case "+":
                    result = checked(StoredValue + CurrentValue);
                    break;
                case "-":
                    result = checked(StoredValue - CurrentValue);
                    break;
                case "*":
                    result = checked(StoredValue * CurrentValue);
                    break;
                case "/":
                    if (CurrentValue != 0)
                        result = StoredValue / CurrentValue;
                    else
                        throw new DivideByZeroException("Cannot divide by zero.");
                    break;
            }

            // Save the calculation to history.
            // History.Add(new HistoryEntry(originalStoredValue, Operator, originalCurrentValue, result));
            History.Insert(0, new HistoryEntry(originalStoredValue, Operator, originalCurrentValue, result));

           // Update the current value to the result.
           CurrentValue = result;
        }
        catch (OverflowException ex)
        {
            throw new OverflowException("The result exceeds the limits of a long.", ex);
        }

        // Reset operator and set last input type to Equals.
        Operator = null;
        _lastInputType = InputType.Equals;
        return CurrentValue;
    }

    /// <summary>
    /// Clears the calculator's current state and history.
    /// </summary>
    public void Clear()
    {
        CurrentValue = 0;
        StoredValue = 0;
        Operator = null;
        History.Clear();
        _lastInputType = InputType.None;
    }
}

using DVGB07_Calculator_App.Models;
using System.Diagnostics;
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

namespace DVGB07_Calculator_App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    private CalculatorModel _calculatorModel;
    public MainWindow()
    {
        InitializeComponent();
        _calculatorModel = new CalculatorModel();
        DataContext = _calculatorModel;
    }

    /// <summary>
    /// Handles button click events from the calculator UI.
    /// Routes the input to the appropriate methods in the calculator model.
    /// </summary>
    /// <param name="sender">The button that was clicked.</param>
    /// <param name="e">Event data for the button click.</param>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            string content = button.Content?.ToString() ?? string.Empty;
            try
            {
                switch (content)
                {
                    // Handle digit input
                    case "0":
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8":
                    case "9":
                        _calculatorModel.HandleDigit(int.Parse(content));
                        UpdateDisplay();
                        break;
                    
                    // Handle operand input
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                        _calculatorModel.HandleOperator(content);
                        UpdateDisplay();
                        break;

                    // Handle equals input
                    case "=":
                        _calculatorModel.CalculateResult();
                        UpdateDisplay();
                        break;

                    // Handle clear input
                    case "C":
                        _calculatorModel.Clear();
                        UpdateDisplay();
                        break;

                    // Handle unknown input
                    default:
                        throw new InvalidOperationException("Unknown button content");
                }
            }
            catch (DivideByZeroException)
            {
                // Display error for division by zero
                UpdateDisplay("Error: Division by zero");
                _calculatorModel.Clear(); // Reset state
            }
            catch (OverflowException)
            {
                // Display error for overflow
                UpdateDisplay("Error: Overflow");
                _calculatorModel.Clear(); // Reset state
            }
            catch (Exception ex)
            {
                // Handle unexpected errors (optional)
                UpdateDisplay($"Error: {ex.Message}");
            }

            Debug.WriteLine($"StoredValue..: {_calculatorModel.StoredValue} {System.Environment.NewLine}" +
                              $"CurrentValue.: {_calculatorModel.CurrentValue} {System.Environment.NewLine}" +
                              $"LastOperator.: {_calculatorModel.Operator}");
            if (_calculatorModel.History.Any())
            {
                foreach (var equation in _calculatorModel.History)
                {
                    Debug.WriteLine(equation);
                }
            }
        }
    }

    /// <summary>
    /// Updates the calculator display based on the current state of the calculator model.
    /// Optionally displays an error message.
    /// </summary>
    /// <param name="errorMessage">Optional error message to display. If null, displays the current calculation state.</param>
    private void UpdateDisplay(string errorMessage = null)
    {
        // An error has occurred in calculation, inform the user.
        if (!string.IsNullOrEmpty(errorMessage))
        {
            DisplayTextbox01.Text = string.Empty;
            DisplayTextbox02.Text = errorMessage;
            return;
        }

        // No error has occurred, display calculation.
        bool hasOperator = !string.IsNullOrEmpty(_calculatorModel.Operator);
        if (hasOperator)
        {
            // Display left operand and operator.
            DisplayTextbox01.Text = $"{_calculatorModel.StoredValue} {_calculatorModel.Operator}";
        }
        else if (_calculatorModel.History.Any())
        {
            // Display the left side of the most recent equation.
            var historyEntry = _calculatorModel.History.First();
            DisplayTextbox01.Text = historyEntry.Equation;
        }
        else
        {
            // Display nothing when its a new calculation.
            DisplayTextbox01.Text = string.Empty;
        }

        // Display right operand
        DisplayTextbox02.Text = $"{_calculatorModel.CurrentValue}";
    }
}

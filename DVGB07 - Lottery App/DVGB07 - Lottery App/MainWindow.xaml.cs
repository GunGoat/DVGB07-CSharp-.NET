using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DVGB07_Lottery_App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const int MinValue = 1;  // Minimum allowed lottery number value
    private const int MaxValue = 35; // Maximum allowed lottery number value
    private readonly IEnumerable<TextBox> LotteryNumbersInput;

    public MainWindow()
    {
        InitializeComponent();
        LotteryNumbersInput = NumberInputPanel.Children
            .OfType<TextBox>()
            .Where(tb => tb.Tag?.ToString() == "LotteryNumber");
    }

    /// <summary>
    /// Validates the range and uniqueness of user inputs in the lottery number and lottery iterations fields.
    /// Updates the Play button's enabled state accordingly.
    /// </summary>
    private void ValidateLotteryInput(object sender, TextChangedEventArgs e)
    {
        bool allValid = true;
        foreach (TextBox input in LotteryNumbersInput)
        {
            bool isInRange = ValidateRange(input);
            bool isUnique = ValidateUniqueness(input);
            string tooltipMessage = string.Empty;

            switch ((isInRange, isUnique))
            {
                case (true, false):
                    tooltipMessage = "This number is already entered in another box.";
                    break;

                case (false, true):
                    tooltipMessage = $"Number must be between {MinValue} and {MaxValue}.";
                    break;

                case (false, false):
                    tooltipMessage = $"Number must be unique and between {MinValue} and {MaxValue}.";
                    break;
            }

            UpdateTextBoxAppearance(input, isInRange && isUnique, tooltipMessage);

            if (!(isInRange && isUnique))
            {
                allValid = false;
            }
        }

        // Enable or disable the Play button based on all valid inputs
        PlayButton.IsEnabled = allValid && !LotteryNumbersInput.Any(tb => tb.Text == string.Empty) && IterationNumberInput.Text != string.Empty;
    }

    /// <summary>
    /// Handles the Play button click event.
    /// </summary>
    private void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Initialize the inputs and counters
            var lotteryNumbers = LotteryNumbersInput
                .Select(tb => int.TryParse(tb.Text, out int number) ? number : -1)
                .Where(num => num >= 0) // Filter out invalid numbers
                .ToHashSet();

            var iterationNumber = int.TryParse(IterationNumberInput.Text, out int result) ? result : 0;

            long fiveCorrect = 0, sixCorrect = 0, sevenCorrect = 0;

            // Run iterations in parallel, each thread has its own local counters. (https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel.for?view=net-9.0)
            Parallel.For(0, iterationNumber, // Start from 0, run until iterationNumber (exclusive).
                () => (fiveCorrect: 0, sixCorrect: 0, sevenCorrect: 0), // Initialize local counters for each thread.
                (i, state, result) => // Loop body: i is the current iteration, state can stop the loop, result stores the thread's counts.
                {
                    var winningNumbers = GenerateUniqueNumbers();
                    int numberOfMatches = winningNumbers.Count(number => lotteryNumbers.Contains(number)); 
                    switch (numberOfMatches)
                    {
                        case 5: result.fiveCorrect++; break;
                        case 6: result.sixCorrect++; break;
                        case 7: result.sevenCorrect++; break;
                    }
                    return result; 
                },
                (result) => // Final step: accumulate results from each parallel task safely.
                {
                    Interlocked.Add(ref fiveCorrect, result.fiveCorrect);
                    Interlocked.Add(ref sixCorrect, result.sixCorrect);
                    Interlocked.Add(ref sevenCorrect, result.sevenCorrect);
                });

            // Update result in UI
            FiveCorrectOutput.Text = fiveCorrect.ToString();
            SixCorrectOutput.Text = sixCorrect.ToString();
            SevenCorrectOutput.Text = sevenCorrect.ToString();
        }
        catch (AggregateException aggEx)
        {
            // Handle parallel execution errors
            foreach (var ex in aggEx.InnerExceptions)
            {
                MessageBox.Show($"A parallel execution error occurred: {ex.Message}", "Parallel Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            // Handle any unexpected error
            MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Generates a set of seven unique random numbers within the allowed range.
    /// </summary>
    /// <returns>A collection of unique random numbers.</returns
    private static IEnumerable<int> GenerateUniqueNumbers()
    {
        var numbers = new HashSet<int>();
        while (numbers.Count < 7)
        {
            int number = Random.Shared.Next(MinValue, MaxValue + 1);
            numbers.Add(number);
        }

        return numbers;
    }

    /// <summary>
    /// Restricts input to numeric characters only.
    /// </summary>
    private void OnlyNumbers(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
    }

    /// <summary>
    /// Validates if the value in the specified TextBox is within the allowed range.
    /// </summary>
    /// <param name="currentTextBox">The TextBox to validate.</param>
    /// <returns>True if the value is in range; otherwise, false.</returns>
    private static bool ValidateRange(TextBox currentTextBox)
    {
        if (currentTextBox.Text.Length == 0) return true;

       if (int.TryParse(currentTextBox.Text, out int number))
        {
            return number >= MinValue && number <= MaxValue;
        }

        return false;
    }

    /// <summary>
    /// Validates if the value in the specified TextBox is unique among all lottery number inputs.
    /// </summary>
    /// <param name="currentTextBox">The TextBox to validate.</param>
    /// <returns>True if the value is unique; otherwise, false.</returns>
    private bool ValidateUniqueness(TextBox currentTextBox)
    {
        if (currentTextBox.Text.Length == 0) return true;

        var enteredValues = LotteryNumbersInput
            .Where(tb => tb != currentTextBox) 
            .Select(tb => tb.Text)
            .Where(text => int.TryParse(text, out _))
            .ToList();

        return !enteredValues.Contains(currentTextBox.Text);
    }

    /// <summary>
    /// Updates the appearance of a TextBox based on its validity.
    /// </summary>
    /// <param name="textBox">The TextBox to update.</param>
    /// <param name="isValid">Indicates whether the TextBox is valid.</param>
    /// <param name="tooltipMessage">The tooltip message to display if invalid.</param>
    private static void UpdateTextBoxAppearance(TextBox textBox, bool isValid, string tooltipMessage)
    {
        if (isValid)
        {
            textBox.BorderBrush = SystemColors.ControlDarkBrush; // Default border color
            textBox.ToolTip = null; // Clear tooltip
        }
        else
        {
            textBox.BorderBrush = Brushes.Red; // Red border color to invalid state
            textBox.ToolTip = tooltipMessage; // Tooltip inform user of the details
        }
    }
}
